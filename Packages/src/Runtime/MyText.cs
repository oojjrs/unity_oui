using System.Globalization;
using System.Text.RegularExpressions;
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
        [SerializeField]
        private bool _ellipsis;

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

                if (_ellipsis)
                    ApplyEllipsis(text);
                else if (_autoWidth)
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
                if (_autoHeight)
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight);
            }
        }

        private void ApplyEllipsis(Text text)
        {
            var width = text.rectTransform.rect.width;
            if (text.preferredWidth <= width)
                return;

            var value = text.text;
            var elements = StringInfo.ParseCombiningCharacters(value);
            var tags = text.supportRichText ? Regex.Matches(value, @"</?(?:b|i|size|color|material)(?:=[^<>]*)?>", RegexOptions.IgnoreCase) : null;
            var tagIndex = (tags?.Count ?? 0) - 1;
            var suffix = string.Empty;
            var settings = text.GetGenerationSettings(Vector2.zero);
            for (var index = elements.Length - 1; index >= 0; --index)
            {
                var length = elements[index];
                if ((tagIndex >= 0) && (length >= tags[tagIndex].Index))
                {
                    suffix = tags[tagIndex].Value + suffix;
                    while ((index > 0) && (elements[index - 1] >= tags[tagIndex].Index))
                        --index;
                    --tagIndex;
                    continue;
                }

                var candidate = value.Substring(0, length) + "…" + suffix;
                if (text.cachedTextGeneratorForLayout.GetPreferredWidth(candidate, settings) / text.pixelsPerUnit <= width)
                {
                    text.text = candidate;
                    return;
                }
            }

            text.text = string.Empty;
        }
    }
}
