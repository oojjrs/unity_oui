using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyAsker : MonoBehaviour
    {
        public interface AskTextInterface
        {
            void OnUpdate(MyText text);
        }

        public class MyAskerArguments
        {
            private Dictionary<string, object> NamedValues { get; } = new();
            private Dictionary<Type, object> TypedValues { get; } = new();

            public T Get<T>()
            {
                if (TypedValues.TryGetValue(typeof(T), out var value))
                    return (T)value;
                else
                    return default;
            }

            public T Get<T>(string key)
            {
                if (NamedValues.TryGetValue(key, out var value))
                    return (T)value;
                else
                    return default;
            }

            public void Set(object value)
            {
                TypedValues[value.GetType()] = value;
            }

            public void Set(string key, object value)
            {
                NamedValues[key] = value;
            }
        }

        private MyAskerArguments _arguments;
        private readonly MyAskerArguments _argumentsEmpty = new();
        [SerializeField]
        private MyText _askText;
        private AskTextInterface[] _askTexts;

        public MyAskerArguments Arguments => _arguments ?? _argumentsEmpty;

        private event Action OnNo;
        private event Action OnOk;
        private event Action OnYes;

        private void Awake()
        {
            _askTexts = GetComponentsInChildren<AskTextInterface>();
        }

        internal void ClickNo()
        {
            OuiClose();

            OnNo?.Invoke();
        }

        internal void ClickOk()
        {
            OuiClose();

            OnOk?.Invoke();
        }

        internal void ClickYes()
        {
            OuiClose();

            OnYes?.Invoke();
        }

        public void OuiClose()
        {
            gameObject.SetActive(false);
        }

        public void OuiOpenOk(MyAskerArguments arguments, Action onOk)
        {
            if (gameObject == default)
                return;

            _arguments = arguments;

            if (_askText != default)
            {
                if (_askTexts?.Length > 0)
                {
                    foreach (var c in _askTexts)
                        c.OnUpdate(_askText);
                }
                else
                {
                    Debug.LogWarning($"{name}> DON'T HAVE {nameof(AskTextInterface)}.");
                }
            }

            OnOk = onOk;

            gameObject.SetActive(true);
        }

        public void OuiOpenYesNo(MyAskerArguments arguments, Action onYes, Action onNo)
        {
            if (gameObject == default)
                return;

            _arguments = arguments;

            if (_askText != default)
            {
                if (_askTexts?.Length > 0)
                {
                    foreach (var c in _askTexts)
                        c.OnUpdate(_askText);
                }
                else
                {
                    Debug.LogWarning($"{name}> DON'T HAVE {nameof(AskTextInterface)}.");
                }
            }

            OnNo = onNo;
            OnYes = onYes;

            gameObject.SetActive(true);
        }
    }
}
