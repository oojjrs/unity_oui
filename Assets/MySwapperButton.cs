using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MySwapperButton : MonoBehaviour, MyButton.CallbackInterface
    {
        void MyButton.CallbackInterface.OnClick()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Switch;
            GetComponentInParent<MySwapper>().OuiClick();
        }
    }
}
