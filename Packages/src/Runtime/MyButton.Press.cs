using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    public partial class MyButton : IPointerDownHandler, IPointerUpHandler
    {
        public interface HoldInterface
        {
            void OnHoldStarted();
            void OnHolding(float elapsedSeconds);
            void OnHoldEnded();
        }

        public interface PressInterface
        {
            void OnPressStarted();
            void OnPressing(float elapsedSeconds);
            void OnPressEnded();
        }

        private static readonly WaitForSeconds __holdStartDelay = new(0.2f);

        private Coroutine _holdStartCoroutine;
        private HoldInterface[] _holds;
        private int _holdStartedCount;
        private float _holdStartedTime;
        private bool _isHolding;
        private bool _isPressBlockedUntilPointerUp;
        private bool _isPressing;
        private PressInterface[] _presses;
        private int _pressStartedCount;
        private float _pressStartedTime;

        private void Update()
        {
            if (IsLocked == false)
            {
                if ((_isPressing) && (_presses != null))
                {
                    foreach (var press in _presses)
                    {
                        if ((_isPressing == false) || IsLocked)
                            break;

                        press.OnPressing(Time.time - _pressStartedTime);
                    }
                }

                if ((_isHolding) && (_holds != null))
                {
                    foreach (var hold in _holds)
                    {
                        if ((_isHolding == false) || IsLocked)
                            break;

                        hold.OnHolding(Time.time - _holdStartedTime);
                    }
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (IsLocked)
                {
                    _isPressBlockedUntilPointerUp = true;
                }
                else if ((IsInteractable) && (_isPressing == false))
                {
                    _isPressBlockedUntilPointerUp = false;
                    _isPressing = true;
                    _pressStartedCount = 0;
                    _pressStartedTime = Time.time;

                    if (_presses != null)
                    {
                        foreach (var press in _presses)
                        {
                            if ((_isPressing == false) || IsLocked)
                                break;

                            ++_pressStartedCount;
                            press.OnPressStarted();
                        }
                    }

                    if ((_isPressing) && (IsLocked == false) && (_holds != null) && (_holds.Length > 0))
                        _holdStartCoroutine = StartCoroutine(StartHoldCoroutine());
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if ((_isPressBlockedUntilPointerUp == false) && (IsLocked == false))
                    ReleasePress(false, true);
                else
                    ReleasePress(false, false);
            }
        }

        private void BlockPressForLock()
        {
            ReleasePress(true, true);
        }

        private void ReleasePress(bool blockPointerUp, bool notify)
        {
            var hadPress = (_holdStartCoroutine != null) || (_isHolding) || (_isPressBlockedUntilPointerUp) || (_isPressing);
            var notifyHoldEndedCount = (notify) && (_isHolding) ? _holdStartedCount : 0;
            var notifyPressEndedCount = (notify) && (_isPressing) ? _pressStartedCount : 0;

            if (_holdStartCoroutine != null)
            {
                StopCoroutine(_holdStartCoroutine);
                _holdStartCoroutine = null;
            }

            _isHolding = false;
            _holdStartedCount = 0;
            _isPressBlockedUntilPointerUp = (blockPointerUp) && (hadPress);
            _isPressing = false;
            _pressStartedCount = 0;

            if (_holds != null)
            {
                for (var index = 0; index < notifyHoldEndedCount; ++index)
                    _holds[index].OnHoldEnded();
            }

            if (_presses != null)
            {
                for (var index = 0; index < notifyPressEndedCount; ++index)
                    _presses[index].OnPressEnded();
            }
        }

        private IEnumerator StartHoldCoroutine()
        {
            yield return __holdStartDelay;

            _holdStartCoroutine = null;

            if ((_isPressing == false) || IsLocked)
                yield break;

            _isHolding = true;
            _holdStartedCount = 0;
            _holdStartedTime = Time.time;

            if (_holds != null)
            {
                foreach (var hold in _holds)
                {
                    if ((_isHolding == false) || IsLocked)
                        break;

                    ++_holdStartedCount;
                    hold.OnHoldStarted();
                }
            }
        }
    }
}
