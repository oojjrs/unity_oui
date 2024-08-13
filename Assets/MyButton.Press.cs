using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    public partial class MyButton : IPointerDownHandler, IPointerUpHandler
    {
        public interface PressInterface
        {
            void OnDown();
            void OnPressing(float time);
            void OnUp();
        }

        private PressInterface[] Presses { get; set; }
        private Coroutine PressCoroutine { get; set; }
        private bool Pressing { get; set; }
        private float PressTime { get; set; }

        private void Update()
        {
            if (Presses != default)
            {
                if (Pressing)
                {
                    foreach (var press in Presses)
                        press.OnPressing(Time.time - PressTime);
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (Presses != default)
            {
                if (Interactable && (eventData.button == PointerEventData.InputButton.Left))
                {
                    if (PressCoroutine != default)
                        StopCoroutine(PressCoroutine);

                    PressCoroutine = StartCoroutine(PressStart());
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (Presses != default)
            {
                foreach (var press in Presses)
                    press.OnUp();

                if (PressCoroutine != default)
                {
                    StopCoroutine(PressCoroutine);
                    PressCoroutine = default;
                }

                Pressing = false;
            }
        }

        private IEnumerator PressStart()
        {
            yield return new WaitForSeconds(0.2f);

            Pressing = true;
            PressTime = Time.time;

            if (Presses != default)
            {
                foreach (var press in Presses)
                    press.OnDown();
            }
        }
    }
}
