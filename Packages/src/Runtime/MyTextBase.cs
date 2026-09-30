using UnityEngine;

namespace oojjrs.oui
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class MyTextBase : MonoBehaviour
    {
        public abstract Color Color { get; set; }
        public abstract string EscapedText { set; }
        public abstract float PreferredHeight { get; }
        public abstract float PreferredWidth { get; }
        public abstract string Text { get; set; }
        public int TextFromInt32 { set => Text = value.ToString(); }

        public static string Escape(string text)
        {
            return text?.Replace('<', '\u02C2').Replace('>', '\u02C3');
        }
    }
}
