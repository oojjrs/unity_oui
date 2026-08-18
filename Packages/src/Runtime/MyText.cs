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
        public string Text
        {
            get => GetComponent<Text>().text;
            set
            {
                GetComponent<Text>().text = value;
                ResizeToPreferredSize();
            }
        }
        public int TextFromInt32
        {
            set
            {
                Text = value.ToString();
            }
        }

        public void ResizeToPreferredSize()
        {
            var text = GetComponent<Text>();

            if (_autoWidth)
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
            if (_autoHeight)
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight);
        }
    }
}
