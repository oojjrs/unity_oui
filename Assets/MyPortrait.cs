using UnityEngine;

namespace oojjrs.oui
{
    public class MyPortrait : MonoBehaviour
    {
        [Tooltip("Sprite에 null이 대입될 때 이것으로 대체됩니다. 또한 Portrait Image가 null인 경우 이 값으로 반환됩니다.")]
        [SerializeField]
        private Sprite _nullSprite;
        [SerializeField]
        private MyImage _portraitImage;
        [SerializeField]
        private MyText _text;
        [SerializeField]
        private MyImage _textBackImage;

        public Sprite Sprite
        {
            get => (_portraitImage != default) ? _portraitImage.Sprite : _nullSprite;
            set
            {
                if (_portraitImage != default)
                {
                    if (value != default)
                        _portraitImage.Sprite = value;
                    else
                        _portraitImage.Sprite = _nullSprite;
                }
                else
                {
                    Debug.LogWarning($"Sprite Setter> Portrait Image PROPERTY IS NULL.");
                }
            }
        }
        public string Text
        {
            get => _text.Text;
            set
            {
                _text.Text = value;
                if (_textBackImage != default)
                    _textBackImage.gameObject.SetActive(string.IsNullOrWhiteSpace(value) == false);
            }
        }

        private void Awake()
        {
            if (_nullSprite != default)
            {
                if (Sprite == default)
                    Sprite = _nullSprite;
            }
        }
    }
}
