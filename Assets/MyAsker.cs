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

        public void OuiClickNo()
        {
            OuiClose();

            OnNo?.Invoke();
        }

        public void OuiClickOk()
        {
            OuiClose();

            OnOk?.Invoke();
        }

        public void OuiClickYes()
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

            // public 함수라서 gameObject의 상태에 상관 없이 불릴 수 있으므로 다른 컨트롤과는 달리 Awake에서 하면 안 되는겨.
            if (_askTexts == default)
                _askTexts = GetComponents<AskTextInterface>();

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

            // public 함수라서 gameObject의 상태에 상관 없이 불릴 수 있으므로 다른 컨트롤과는 달리 Awake에서 하면 안 되는겨.
            if (_askTexts == default)
                _askTexts = GetComponents<AskTextInterface>();

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
