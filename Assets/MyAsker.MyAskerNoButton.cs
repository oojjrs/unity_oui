using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyAsker
    {
        [RequireComponent(typeof(MyButton))]
        public class MyAskerNoButton : MonoBehaviour, MyButton.CallbackInterface
        {
            void MyButton.CallbackInterface.OnClick()
            {
                GetComponent<MyButton>().ClickSound = MyButton.ClickSoundEnum.Cancel;

                var asker = GetComponentInParent<MyAsker>();
                if (asker != default)
                    asker.OnNo();
            }
        }
    }
}
