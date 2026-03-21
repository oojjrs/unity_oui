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
