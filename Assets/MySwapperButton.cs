using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MySwapperButton : MonoBehaviour, MyButton.CallbackInterface, MyButton.SelectInterface
    {
        void MyButton.CallbackInterface.OnClick()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Switch;
            GetComponentInParent<MySwapper>().OuiClick();
        }

        void MyButton.SelectInterface.OnDeselect()
        {
            GetComponentInParent<MySwapper>().OuiDeselect();
        }

        void MyButton.SelectInterface.OnSelect()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Switch;
            GetComponentInParent<MySwapper>().OuiSelect();
        }
    }
}
