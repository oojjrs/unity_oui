using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
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

        // overrideSprite는 사용 목적이 맞지 않으므로 전용 함수를 두지 않는다.
        public void SetNativeSizeSprite(Sprite sprite, float nativeSizeScale = 1f)
        {
            var image = GetComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            image.rectTransform.sizeDelta *= nativeSizeScale;
        }
    }
}
