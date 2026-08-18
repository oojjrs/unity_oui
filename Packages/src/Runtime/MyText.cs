using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class MyText : MonoBehaviour
    {
        [SerializeField]
        private bool _autoHeight;
        [SerializeField]
        private bool _autoWidth;

        public Color Color
        {
            get => GetComponent<Text>().color;
            set => GetComponent<Text>().color = value;
        }
        public float PreferredHeight => GetComponent<Text>().preferredHeight;
        public float PreferredWidth => GetComponent<Text>().preferredWidth;
        public string Text
        {
            get => GetComponent<Text>().text;
            set
            {
                GetComponent<Text>().text = value;

                if (_autoHeight || _autoWidth)
                {
                    var text = GetComponent<Text>();
                    var size = text.rectTransform.rect.size;

                    if (_autoWidth)
                        size.x = text.preferredWidth;

                    if (_autoHeight)
                    {
                        if (_autoWidth)
                            size.y = text.cachedTextGeneratorForLayout.GetPreferredHeight(text.text, text.GetGenerationSettings(new Vector2(size.x, 0))) / text.pixelsPerUnit;
                        else
                            size.y = text.preferredHeight;
                    }

                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                }
            }
        }
        public int TextFromInt32
        {
            set
            {
                Text = value.ToString();
            }
        }
    }
}
