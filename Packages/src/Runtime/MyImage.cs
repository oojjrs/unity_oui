using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Image))]
    public class MyImage : MonoBehaviour
    {
        public Sprite NativeSizeOverrideSprite
        {
            set
            {
                var image = GetComponent<Image>();
                image.overrideSprite = value;
                image.SetNativeSize();
            }
        }
        public Sprite NativeSizeSprite
        {
            set
            {
                var image = GetComponent<Image>();
                image.sprite = value;
                image.SetNativeSize();
            }
        }
        public Sprite OverrideSprite
        {
            get => GetComponent<Image>().overrideSprite;
            set => GetComponent<Image>().overrideSprite = value;
        }
        public Sprite Sprite
        {
            get => GetComponent<Image>().sprite;
            set => GetComponent<Image>().sprite = value;
        }
    }
}
