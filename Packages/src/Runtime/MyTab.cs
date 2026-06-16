using System;
using UnityEngine;

namespace oojjrs.oui
{
    public class MyTab : MonoBehaviour
    {
        public interface CallbackInterface
        {
            void OnEnter();
            void OnExit();
        }

        [SerializeField]
        private bool _allowSwitchOff;
        private CallbackInterface[] _callbacks;
        [SerializeField]
        private GameObject _header;
        private int? _index;
        [SerializeField]
        private MySelector _selector;

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
        }

        private void OnDisable()
        {
            if (_callbacks != null)
            {
                foreach (var callback in _callbacks)
                    callback.OnExit();
            }
        }

        private void OnEnable()
        {
            if (_index.HasValue)
                OuiSelect(_index.Value);

            if (_callbacks != null)
            {
                foreach (var callback in _callbacks)
                    callback.OnEnter();
            }
        }

        private void Start()
        {
            if (_callbacks?.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            if (_header == null)
                Debug.LogWarning($"{name}> DON'T HAVE TAB HEADER.");

            if (_allowSwitchOff == false)
                OuiSelect(0);
        }

        public void OuiMoveNext(bool allowWrapAround = false)
        {
            if (_header == null)
                return;

            var headers = _header.GetComponentsInChildren<MyTabHeaderButton>();

            var currentIndex = Math.Clamp(_index ?? -1, -1, headers.Length - 1);
            var startIndex = currentIndex + 1;
            for (int i = startIndex; i < headers.Length; ++i)
            {
                if (headers[i].GetComponent<MyButton>().Interactable)
                {
                    OuiSelect(i);
                    return;
                }
            }

            if (allowWrapAround)
            {
                for (int i = 0; i < startIndex; ++i)
                {
                    if (headers[i].GetComponent<MyButton>().Interactable)
                    {
                        OuiSelect(i);
                        return;
                    }
                }
            }
        }

        public void OuiMovePrevious(bool allowWrapAround = false)
        {
            if (_header == null)
                return;

            var headers = _header.GetComponentsInChildren<MyTabHeaderButton>();

            var currentIndex = Math.Clamp(_index ?? headers.Length, 0, headers.Length);
            var startIndex = currentIndex - 1;
            for (int i = startIndex; i >= 0; --i)
            {
                if (headers[i].GetComponent<MyButton>().Interactable)
                {
                    OuiSelect(i);
                    return;
                }
            }

            if (allowWrapAround)
            {
                for (int i = headers.Length - 1; i > startIndex; --i)
                {
                    if (headers[i].GetComponent<MyButton>().Interactable)
                    {
                        OuiSelect(i);
                        return;
                    }
                }
            }
        }

        public void OuiSelect(int index)
        {
            _index = index;

            // TODO: 일단 애니메이션 버튼인 경우는 만들지 않겠다.
            //if (_header != null)
            //{
            //    var buttons = _header.GetComponentsInChildren<MyTabHeaderButton>();
            //    for (int i = 0; i < buttons.Length; ++i)
            //    {
            //        var button = buttons[i].GetComponent<MyButton>();
            //        if (button.Interactable)
            //        {
            //            if (i != index)
            //                button.OuiPlayNormalAnimation();
            //            else
            //                button.OuiPlaySelectedAnimation();
            //        }
            //    }
            //}

            if (_selector != null)
                _selector.OuiSelect(index);
        }

        internal void OuiSelect(MyTabHeaderButton header)
        {
            if (_header == null)
                return;

            var headers = _header.GetComponentsInChildren<MyTabHeaderButton>();
            var index = Array.IndexOf(headers, header);
            if (index >= 0)
                OuiSelect(index);
        }
    }
}
