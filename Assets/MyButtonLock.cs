using oojjrs.oui;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MyButton))]
    public class MyButtonLock : MonoBehaviour, MyButton.CallbackInterface, MyButton.LockInterface
    {
        private float _time;

        bool MyButton.LockInterface.KeepWaiting
        {
            get
            {
                GetComponent<MyButton>().Text.TextFromInt32 = (int)(Time.time - _time);
                if (Time.time - _time >= 5)
                    GetComponent<MyButton>().Text.Text = "확인";

                return Time.time - _time < 5;
            }
        }

        void MyButton.CallbackInterface.OnClick()
        {
            _time = Time.time;

            GetComponent<MyButton>().OuiLock(this);
        }
    }
}
