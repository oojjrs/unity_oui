using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class MyImage : MonoBehaviour
    {
        [Tooltip("레이캐스트가 이미지 픽셀에 맞은 것으로 판정할 최소 알파값입니다. 0보다 크면 Sprite 텍스처의 Read/Write를 켜고 Crunch와 Sprite Atlas 사용을 피해야 합니다.")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _alphaHitTestMinimumThreshold;

        public float AlphaHitTestMinimumThreshold
        {
            get => GetComponent<Image>().alphaHitTestMinimumThreshold;
            set => GetComponent<Image>().alphaHitTestMinimumThreshold = value;
        }
        public Color Color
        {
            get => GetComponent<Image>().color;
            set => GetComponent<Image>().color = value;
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

        private void OnEnable()
        {
            ApplyAlphaHitTestMinimumThreshold();
        }

        private void OnValidate()
        {
            ApplyAlphaHitTestMinimumThreshold();
        }

        private void ApplyAlphaHitTestMinimumThreshold()
        {
            var image = GetComponent<Image>();
            if (image.alphaHitTestMinimumThreshold == _alphaHitTestMinimumThreshold)
                return;

            image.alphaHitTestMinimumThreshold = _alphaHitTestMinimumThreshold;
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
