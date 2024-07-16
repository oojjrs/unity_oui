using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Toggle))]
    public class MyToggle : MonoBehaviour, IPointerEnterHandler
    {
        public interface CallbackInterface
        {
            bool InitialValue { get; }

            void OnValueChanged(bool isOn);
        }

        [SerializeField]
        private bool _hoverSoundDisabled;

        private CallbackInterface Callback { get; set; }
        public bool OuiInteractable { get => GetComponent<Toggle>().interactable; set => GetComponent<Toggle>().interactable = value; }
        public bool OuiIsOn { get => GetComponent<Toggle>().isOn; set => GetComponent<Toggle>().isOn = value; }

        private void OnEnable()
        {
            if (Callback != default)
                GetComponent<Toggle>().isOn = Callback.InitialValue;
        }

        private void Start()
        {
            Callback = GetComponent<CallbackInterface>();
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            if (Callback != default)
                GetComponent<Toggle>().isOn = Callback.InitialValue;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (OuiInteractable && (_hoverSoundDisabled == false))
                MyControl.Audio.PlayHoverSfx?.Invoke();
        }

        public void OnValueChanged(bool isOn)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
                MyControl.Audio.PlaySwitchSfx?.Invoke();

            Callback?.OnValueChanged(isOn);
        }

        public void OuiSetIsOnWithoutNotify(bool value)
        {
            GetComponent<Toggle>().SetIsOnWithoutNotify(value);

            if (value == false)
            {
                if (EventSystem.current.currentSelectedGameObject == gameObject)
                    EventSystem.current.SetSelectedGameObject(default);
            }
        }

        private void __OnPointerClick(PointerEventData eventData)
        {
            if (OuiInteractable)
            {
                var t = GetComponent<Toggle>();
                t.isOn = t.isOn == false;
            }
        }
    }
}
