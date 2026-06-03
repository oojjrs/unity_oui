using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Image))]
    public class MyImage : MonoBehaviour
    {
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

        public void SetNativeSizeOverrideSprite(Sprite sprite, float nativeSizeScale = 1f)
        {
            var image = GetComponent<Image>();
            image.overrideSprite = sprite;
            SetNativeSize(image, nativeSizeScale);
        }

        public void SetNativeSizeSprite(Sprite sprite, float nativeSizeScale = 1f)
        {
            var image = GetComponent<Image>();
            image.sprite = sprite;
            SetNativeSize(image, nativeSizeScale);
        }

        private void SetNativeSize(Image image, float nativeSizeScale)
        {
            image.SetNativeSize();
            image.rectTransform.sizeDelta *= nativeSizeScale;
        }
    }
}
