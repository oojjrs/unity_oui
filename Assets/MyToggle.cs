using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MyToggle : MonoBehaviour, IPointerEnterHandler, MyButton.CallbackInterface
    {
        public interface CallbackInterface
        {
            void OnValueChanged(bool isOn);
        }

        public interface InitializerInterface
        {
            bool InitialValue { get; }
        }

        [SerializeField]
        private bool _hoverSoundDisabled;
        private bool _isOn;

        [SerializeField]
        private GameObject _off;
        [SerializeField]
        private GameObject _on;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
        public bool Interactable { get; set; }
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (value != _isOn)
                {
                    _isOn = value;

                    OnValueChanged(_isOn);
                }
            }
        }

        private void OnEnable()
        {
            if (Initializer != default)
                SetIsOnWithoutNotify(Initializer.InitialValue);
        }

        private void Start()
        {
            Callbacks = GetComponents<CallbackInterface>();
            if (Callbacks == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            Initializer = GetComponent<InitializerInterface>();
            if (Initializer != default)
                SetIsOnWithoutNotify(Initializer.InitialValue);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable && (_hoverSoundDisabled == false))
                MyControl.Audio.PlayHoverSfx?.Invoke();
        }

        void MyButton.CallbackInterface.OnClick()
        {
            if (Interactable)
                IsOn = !_isOn;
        }

        private void OnValueChanged(bool isOn)
        {
            SetIsOnWithoutNotify(isOn);

            if (Callbacks != default)
            {
                foreach (var callback in Callbacks)
                    callback.OnValueChanged(isOn);
            }
        }

        public void SetIsOnWithoutNotify(bool isOn)
        {
            if (isOn)
            {
                if (EventSystem.current.currentSelectedGameObject == gameObject)
                    MyControl.Audio.PlaySwitchSfx?.Invoke();

                if (_off != default)
                    _off.SetActive(false);

                if (_on != default)
                    _on.SetActive(true);
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject == gameObject)
                    EventSystem.current.SetSelectedGameObject(default);

                if (_off != default)
                    _off.SetActive(true);

                if (_on != default)
                    _on.SetActive(false);
            }
        }

        private void __OnPointerClick(PointerEventData eventData)
        {
            (this as MyButton.CallbackInterface).OnClick();
        }
    }
}
