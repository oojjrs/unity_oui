using UnityEngine;

namespace Assets.oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MySelectorToggleButton : MonoBehaviour, MyButton.CallbackInterface, MyButton.DoubleClickInterface
    {
        void MyButton.CallbackInterface.OnClick()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Switch;
            GetComponentInParent<MySelectorToggle>().OnClick();
        }

        void MyButton.DoubleClickInterface.OnDoubleClick()
        {
            GetComponentInParent<MySelectorToggle>().OnDoubleClick();
        }
    }
}
