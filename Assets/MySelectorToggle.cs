using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MySelector))]
    public class MySelectorToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, MySelector.CallbackInterface
    {
        public interface CallbackInterface
        {
            bool Interactable { get; }
            bool IsOn { get; }

            void OnClick(bool isOn);
        }

        public interface DoubleClickInterface
        {
            bool Interactable { get; }

            void OnDoubleClick();
        }

        // 크흠... 일단 허용
        public interface ValidationInterface
        {
            bool IsTooltipPermitted { get; }
            bool IsValid { get; }
        }

        [SerializeField]
        private bool _hoverSoundDisabled;

        private CallbackInterface Callback { get; set; }
        private DoubleClickInterface DoubleClick { get; set; }
        private bool Hovering { get; set; }
        private ValidationInterface Validation { get; set; }

        private void Start()
        {
            Callback = GetComponent<CallbackInterface>();
            if (Callback != default)
                GetComponent<MySelector>().UpdateSelection();
            else
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            DoubleClick = GetComponent<DoubleClickInterface>();
            Validation = GetComponent<ValidationInterface>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Validation != default)
            {
                if (Validation.IsTooltipPermitted == false)
                    return;
            }

            if (Callback?.Interactable == true)
            {
                if (_hoverSoundDisabled == false)
                    MyControl.Audio.PlayHoverSfx?.Invoke();

                Hovering = true;

                GetComponent<MySelector>().UpdateSelection();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Hovering = false;

            GetComponent<MySelector>().UpdateSelection();
        }

        int MySelector.CallbackInterface.GetIndex()
        {
            if (Validation != default)
            {
                if (Validation.IsValid == false)
                    return -1;
            }

            if (Callback != default)
            {
                if (Callback.IsOn || Hovering)
                    return 1;
                else if (Callback.Interactable)
                    return 0;
                else
                    return 2;
            }
            else
            {
                return 2;
            }
        }

        public void OnClick()
        {
            if (Validation != default)
            {
                if (Validation.IsValid == false)
                    return;
            }

            if (Callback != default)
            {
                if (Callback.Interactable)
                    Callback.OnClick(Callback.IsOn == false);
            }

            Hovering = false;
            GetComponent<MySelector>().UpdateSelection();
        }

        public void OnDoubleClick()
        {
            if (Validation != default)
            {
                if (Validation.IsValid == false)
                    return;
            }

            if (DoubleClick != default)
            {
                if (DoubleClick.Interactable)
                    DoubleClick.OnDoubleClick();
            }

            Hovering = false;
            GetComponent<MySelector>().UpdateSelection();
        }
    }
}
