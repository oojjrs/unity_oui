using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputField))]
    [RequireComponent(typeof(RectTransform))]
    public class MyInput : MonoBehaviour, IDeselectHandler, ISelectHandler
    {
        public interface EndEditInterface
        {
            void OnEndEdit(string s, bool wasCanceled);
        }

        public interface FocusInterface
        {
            void OnFocusEnter();
            void OnFocusExit();
        }

        public interface InitializerInterface
        {
            string InitialValue { get; }
        }

        public interface SubmitInterface
        {
            void OnSubmit(string s);
        }

        public interface ValueChangedInterface
        {
            void OnValueChanged(string s);
        }

        private EndEditInterface _endEdit;
        private FocusInterface[] _focuses;
        private InitializerInterface _initializer;
        [SerializeField]
        private bool _isClearAfterSubmit;
        [SerializeField]
        private bool _isClearWhenOpen;
        [SerializeField]
        private bool _isFocusAfterSubmit;
        private bool _isFocused;
        [SerializeField]
        private bool _isFocusWhenOpen;
        private SubmitInterface _submit;
        private ValueChangedInterface _valueChanged;

        public int CharacterLimit => GetComponent<InputField>().characterLimit;
        public bool IsFocused => _isFocused;
        public bool IsInteractable => GetComponent<InputField>().IsInteractable();
        public string Text
        {
            get => GetComponent<InputField>().text;
            set => GetComponent<InputField>().text = value;
        }

        private void Awake()
        {
            _endEdit = GetComponent<EndEditInterface>();
            _focuses = GetComponents<FocusInterface>();
            _initializer = GetComponent<InitializerInterface>();
            _submit = GetComponent<SubmitInterface>();
            _valueChanged = GetComponent<ValueChangedInterface>();
        }

        private void OnDisable()
        {
            if ((Application.isPlaying == false) || MyControl.IsQuitting)
                return;

            MyControl.MyInputs.Remove(this);

            var eventSystem = EventSystem.current;
            if ((eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
            {
                eventSystem.SetSelectedGameObject(default);

                ExitFocus();
            }
        }

        private void OnEnable()
        {
            MyControl.MyInputs.Add(this);

            if (_isClearWhenOpen)
                GetComponent<InputField>().text = string.Empty;

            if (_initializer != default)
                GetComponent<InputField>().text = _initializer.InitialValue;

            if (_isFocusWhenOpen)
                GetComponent<InputField>().Select();
            else
                SyncFocus();
        }

        private void Start()
        {
            if (_initializer != default)
            {
                GetComponent<InputField>().text = _initializer.InitialValue;

                if (_isFocusWhenOpen)
                    GetComponent<InputField>().Select();
            }
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            ExitFocus();
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            EnterFocus();
        }

        private void EnterFocus()
        {
            if (_isFocused)
                return;

            _isFocused = true;

            if (_focuses != null)
            {
                foreach (var focus in _focuses)
                    focus.OnFocusEnter();
            }
        }

        private void ExitFocus()
        {
            if (_isFocused == false)
                return;

            _isFocused = false;

            if (_focuses != null)
            {
                foreach (var focus in _focuses)
                    focus.OnFocusExit();
            }
        }

        // lost focus 때 부른다
        public void OnEndEdit(string s)
        {
            _endEdit?.OnEndEdit(s, GetComponent<InputField>().wasCanceled);
        }

        // enter 등이 입력되었을 때 호출되는데, OnEndEdit보다 빠르다.
        public void OnSubmit(string s)
        {
            _submit?.OnSubmit(s);

            if(_isClearAfterSubmit)
                GetComponent<InputField>().text = string.Empty;

            if (_isFocusAfterSubmit)
            {
                // TODO : 왜 여기선 Select만 갖고 안 되는 거지?
                GetComponent<InputField>().Select();
                GetComponent<InputField>().ActivateInputField();
            }
        }

        public void OnValueChanged(string s)
        {
            _valueChanged?.OnValueChanged(s);
        }

        private void SyncFocus()
        {
            if (IsInteractable && (EventSystem.current != null) && (EventSystem.current.currentSelectedGameObject == gameObject))
                EnterFocus();
            else
                ExitFocus();
        }
    }
}
