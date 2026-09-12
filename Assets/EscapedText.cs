using oojjrs.oui;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MyText))]
    public class EscapedText : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        private string _text = "<닉네임> <b>Bold</b> <i>Italic</i> <size=40>Size</size> <color=red>Color</color>";

        [ContextMenu("Test Escaped Text")]
        private void Start()
        {
            GetComponent<MyText>().EscapedText = _text;
        }

        [ContextMenu("Test Escaped Text With Color")]
        private void TestWithColor()
        {
            GetComponent<MyText>().Text = $"<color=yellow>{MyText.Escape(_text)}</color>";
        }
    }
}
