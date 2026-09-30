using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class MyText : MyTextBase
    {
        [SerializeField]
        private bool _autoHeight;
        [SerializeField]
        private bool _autoWidth;

        public override Color Color
        {
            get => GetComponent<Text>().color;
            set => GetComponent<Text>().color = value;
        }
        public override string EscapedText
        {
            set
            {
                if (GetComponent<Text>().supportRichText)
                {
                    Text = Escape(value);
                }
                else
                {
                    Debug.LogWarning($"{name}> RICH TEXT IS DISABLED : {nameof(EscapedText)}", this);

                    Text = value;
                }
            }
        }
        public override float PreferredHeight => GetComponent<Text>().preferredHeight;
        public override float PreferredWidth => GetComponent<Text>().preferredWidth;
        public override string Text
        {
            get => GetComponent<Text>().text;
            set
            {
                var text = GetComponent<Text>();
                text.text = value;

                if (_autoWidth)
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
                if (_autoHeight)
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight);
            }
        }
    }
}
