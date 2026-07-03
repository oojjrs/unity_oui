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

        private static readonly WaitForSeconds __waitForSeconds = new(0.2f);

        private Coroutine _pressCoroutine;
        private PressInterface[] _presses;
        private bool _pressing;
        private float _pressTime;

        private void Update()
        {
            if (_presses != null)
            {
                if (_pressing)
                {
                    foreach (var press in _presses)
                        press.OnPressing(Time.time - _pressTime);
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_presses != null)
            {
                if (IsInteractable && (eventData.button == PointerEventData.InputButton.Left))
                {
                    if (_pressCoroutine != null)
                        StopCoroutine(_pressCoroutine);

                    _pressCoroutine = StartCoroutine(PressStart());
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_presses != null)
            {
                foreach (var press in _presses)
                    press.OnUp();

                if (_pressCoroutine != null)
                {
                    StopCoroutine(_pressCoroutine);
                    _pressCoroutine = null;
                }

                _pressing = false;
            }
        }

        private IEnumerator PressStart()
        {
            yield return __waitForSeconds;

            _pressing = true;
            _pressTime = Time.time;

            if (_presses != null)
            {
                foreach (var press in _presses)
                    press.OnDown();
            }
        }
    }
}
