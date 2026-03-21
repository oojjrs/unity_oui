using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyAsker : MonoBehaviour
    {
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
        private readonly MyAskerArguments _emptyArguments = new();

        public MyAskerArguments Arguments => _arguments ?? _emptyArguments;

        private event Action OnNo;
        private event Action OnOk;
        private event Action OnYes;

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

            OnOk = onOk;

            gameObject.SetActive(true);
        }

        public void OuiOpenYesNo(MyAskerArguments arguments, Action onYes, Action onNo)
        {
            if (gameObject == default)
                return;

            _arguments = arguments;

            OnNo = onNo;
            OnYes = onYes;

            gameObject.SetActive(true);
        }
    }
}
