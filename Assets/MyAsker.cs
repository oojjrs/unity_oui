using System;
using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyAsker : MonoBehaviour
    {
        private event Action OnNo;
        private event Action OnOk;
        private event Action OnYes;

        internal void ClickNo()
        {
            OnNo?.Invoke();
        }

        internal void ClickOk()
        {
            OnOk?.Invoke();
        }

        internal void ClickYes()
        {
            OnYes?.Invoke();
        }

        public void OuiOpenOk(Action onOk)
        {
            if (gameObject == default)
                return;

            OnOk = onOk;

            gameObject.SetActive(true);
        }

        public void OuiOpenYesNo(Action onYes, Action onNo)
        {
            if (gameObject == default)
                return;

            OnNo = onNo;
            OnYes = onYes;

            gameObject.SetActive(true);
        }
    }
}
