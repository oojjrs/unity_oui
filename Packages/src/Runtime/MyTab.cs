using System;
using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(MyRadioGroup))]
    [RequireComponent(typeof(MySelector))]
    public class MyTab : MonoBehaviour, MyRadioGroup.CallbackInterface, MyRadioGroup.InitializerInterface, MySelector.CallbackInterface
    {
        public interface CallbackInterface
        {
            void OnEnter();
            void OnExit();
        }

        public interface InitializerInterface
        {
            int InitialIndex { get; }
        }

        private MySelector _body;
        private CallbackInterface[] _callbacks;
        private MyRadioGroup _header;
        private InitializerInterface _initializer;

        int MyRadioGroup.InitializerInterface.InitialIndex => (_initializer != null) ? _initializer.InitialIndex : 0;

        private void Awake()
        {
            _body = GetComponent<MySelector>();
            _callbacks = GetComponents<CallbackInterface>();
            _header = GetComponent<MyRadioGroup>();
            _initializer = GetComponent<InitializerInterface>();
        }

        private void OnDisable()
        {
            if (Application.isPlaying && (MyControl.IsQuitting == false))
            {
                if (_callbacks != null)
                {
                    foreach (var callback in _callbacks)
                        callback.OnExit();
                }
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying && (MyControl.IsQuitting == false))
            {
                if (_callbacks != null)
                {
                    foreach (var callback in _callbacks)
                        callback.OnEnter();
                }
            }
        }

        private void Start()
        {
            if ((_header != null) && (_header.SelectionMode != MyRadioGroup.SelectionModeEnum.Required))
                Debug.LogWarning($"{name}> {nameof(MyTab)} REQUIRES {nameof(MyRadioGroup)}.{nameof(MyRadioGroup.SelectionMode)} TO BE {nameof(MyRadioGroup.SelectionModeEnum.Required)}.");
        }

        void MyRadioGroup.CallbackInterface.OnValueChanged(int index, MyRadio radio)
        {
            if (_body != null)
                _body.OuiSelect(index);
        }

        int MySelector.CallbackInterface.GetIndex()
        {
            if (_header != null)
                return _header.Index;
            else
                return -1;
        }

        private bool CanSelectHeader(int index)
        {
            if (_header == null)
                return index >= 0;

            var radio = _header.OuiGetRadio(index);
            return (radio != null) && radio.IsInteractable;
        }

        public void OuiMoveNext(bool allowWrapAround = false)
        {
            if ((_header == null) || (_header.Count <= 0))
                return;

            var currentIndex = Math.Clamp(_header.Index, 0, _header.Count - 1);
            var startIndex = currentIndex + 1;
            for (int i = startIndex; i < _header.Count; ++i)
            {
                if (CanSelectHeader(i))
                {
                    OuiSelect(i);
                    return;
                }
            }

            if (allowWrapAround)
            {
                for (int i = 0; i < startIndex; ++i)
                {
                    if (CanSelectHeader(i))
                    {
                        OuiSelect(i);
                        return;
                    }
                }
            }
        }

        public void OuiMovePrevious(bool allowWrapAround = false)
        {
            if ((_header == null) || (_header.Count <= 0))
                return;

            var currentIndex = Math.Clamp(_header.Index, 0, _header.Count - 1);
            var startIndex = currentIndex - 1;
            for (int i = startIndex; i >= 0; --i)
            {
                if (CanSelectHeader(i))
                {
                    OuiSelect(i);
                    return;
                }
            }

            if (allowWrapAround)
            {
                for (int i = _header.Count - 1; i > startIndex; --i)
                {
                    if (CanSelectHeader(i))
                    {
                        OuiSelect(i);
                        return;
                    }
                }
            }
        }

        public void OuiSelect(int index)
        {
            if (_header != null)
                _header.OuiSelect(index);
        }
    }
}
