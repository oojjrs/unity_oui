using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MyAskerYesButton : MonoBehaviour, MyButton.CallbackInterface
    {
        void MyButton.CallbackInterface.OnClick()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Confirm;

            var asker = GetComponentInParent<MyAsker>();
            if (asker != default)
                asker.OuiClickYes();
        }
    }
}
