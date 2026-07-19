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

        private bool _isPressBlockedUntilPointerUp;
        private Coroutine _pressCoroutine;
        private PressInterface[] _presses;
        private bool _pressing;
        private float _pressTime;

        private void Update()
        {
            if ((_presses != null) && (IsLocked == false))
            {
                if (_pressing)
                {
                    foreach (var press in _presses)
                    {
                        if ((_pressing == false) || IsLocked)
                            break;

                        press.OnPressing(Time.time - _pressTime);
                    }
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_presses != null)
            {
                if (IsLocked)
                {
                    _isPressBlockedUntilPointerUp = true;
                }
                else if ((IsInteractable) && (eventData.button == PointerEventData.InputButton.Left))
                {
                    _isPressBlockedUntilPointerUp = false;

                    if (_pressCoroutine != null)
                        StopCoroutine(_pressCoroutine);

                    _pressCoroutine = StartCoroutine(PressStartCoroutine());
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_presses != null)
            {
                if ((_isPressBlockedUntilPointerUp == false) && (IsLocked == false))
                    ReleasePress(false, true);
                else
                    ReleasePress(false, false);
            }
        }

        private void BlockPressForLock()
        {
            ReleasePress(true, _pressing);
        }

        private IEnumerator PressStartCoroutine()
        {
            yield return __waitForSeconds;

            _pressCoroutine = null;
            _pressing = true;
            _pressTime = Time.time;

            if (_presses != null)
            {
                foreach (var press in _presses)
                {
                    if ((_pressing == false) || IsLocked)
                        break;

                    press.OnDown();
                }
            }
        }

        private void ReleasePress(bool blockPointerUp, bool notify)
        {
            var hadPress = (_pressCoroutine != null) || (_pressing) || (_isPressBlockedUntilPointerUp);

            if (_pressCoroutine != null)
            {
                StopCoroutine(_pressCoroutine);
                _pressCoroutine = null;
            }

            _isPressBlockedUntilPointerUp = (blockPointerUp) && (hadPress);
            _pressing = false;

            if ((notify) && (_presses != null))
            {
                foreach (var press in _presses)
                    press.OnUp();
            }
        }
    }
}
