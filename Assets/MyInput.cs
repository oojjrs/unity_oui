using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(InputField))]
    public class MyInput : MonoBehaviour, IDeselectHandler, ISelectHandler
    {
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

        [SerializeField]
        private bool _clearAfterSubmit;
        [SerializeField]
        private bool _clearWhenOpen;
        [SerializeField]
        private bool _focusAfterSubmit;

        public int CharacterLimit => GetComponent<InputField>().characterLimit;
        private InitializerInterface Initializer { get; set; }
        private SubmitInterface Submit { get; set; }
        public string Text
        {
            get => GetComponent<InputField>().text;
            set => GetComponent<InputField>().text = value;
        }
        private ValueChangedInterface ValueChanged { get; set; }

        private void Awake()
        {
            Initializer = GetComponent<InitializerInterface>();
            Submit = GetComponent<SubmitInterface>();
            ValueChanged = GetComponent<ValueChangedInterface>();
        }

        private void OnDisable()
        {
            if (EventSystem.current?.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(default);

                MyControl.Texting = false;
            }
        }

        private void OnEnable()
        {
            if (_clearWhenOpen)
                GetComponent<InputField>().text = string.Empty;

            if (Initializer != default)
                GetComponent<InputField>().text = Initializer.InitialValue;

            GetComponent<InputField>().Select();
        }

        private void Start()
        {
            if (Initializer != default)
            {
                GetComponent<InputField>().text = Initializer.InitialValue;
                GetComponent<InputField>().Select();
            }
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            MyControl.Texting = false;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            MyControl.Texting = true;
        }

        // lost focus 때 부른다
        public void OnEndEdit(string s)
        {
        }

        // enter 등이 입력되었을 때 호출되는데, OnEndEdit보다 빠르다.
        public void OnSubmit(string s)
        {
            Submit?.OnSubmit(s);

            if(_clearAfterSubmit)
                GetComponent<InputField>().text = string.Empty;

            if (_focusAfterSubmit)
            {
                // TODO : 왜 여기선 Select만 갖고 안 되는 거지?
                GetComponent<InputField>().Select();
                GetComponent<InputField>().ActivateInputField();
            }
        }

        public void OnValueChanged(string s)
        {
            ValueChanged?.OnValueChanged(s);
        }
    }
}
