using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(MyButton))]
    public class MyTabHeaderButton : MonoBehaviour, MyButton.CallbackInterface
    {
        void MyButton.CallbackInterface.OnClick()
        {
            GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Switch;

            var tab = GetComponentInParent<MyTab>();
            if (tab != null)
                tab.OuiSelect(this);
        }
    }
}
