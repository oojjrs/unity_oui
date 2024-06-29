using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.oui
{
    public partial class MyButton : IPointerDownHandler, IPointerUpHandler
    {
        public interface PressInterface
        {
            void OnDown();
            void OnPressing(float time);
            void OnUp();
        }

        private PressInterface Press { get; set; }
        private Coroutine PressCoroutine { get; set; }
        private bool Pressing { get; set; }
        private float PressTime { get; set; }

        private void Update()
        {
            if (Press != default)
            {
                if (Pressing)
                    Press.OnPressing(Time.time - PressTime);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (Press != default)
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
            if (Press != default)
            {
                Press.OnUp();

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

            Press.OnDown();
        }
    }
}
