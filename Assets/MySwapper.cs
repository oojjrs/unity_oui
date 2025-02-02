using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MySelector))]
    public class MySwapper : MonoBehaviour, MySelector.CallbackInterface
    {
        public interface CallbackInterface
        {
            bool Interactable { get; }
            bool IsOn { get; }

            void OnClick(bool isOn);
        }

        private CallbackInterface Callback { get; set; }

        private void Awake()
        {
            Callback = GetComponent<CallbackInterface>();
        }

        private void Start()
        {
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        int MySelector.CallbackInterface.GetIndex()
        {
            if (Callback != default)
            {
                if (Callback.Interactable)
                {
                    if (Callback.IsOn)
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    if (Callback.IsOn)
                        return 3;
                    else
                        return 2;
                }
            }
            else
            {
                return -1;
            }
        }

        public void OuiClick()
        {
            if (Callback != default)
            {
                if (Callback.Interactable)
                    Callback.OnClick(Callback.IsOn == false);
            }

            OuiUpdate();
        }

        public void OuiUpdate()
        {
            GetComponent<MySelector>().OuiUpdate();
        }
    }
}
