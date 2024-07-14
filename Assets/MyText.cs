using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Text))]
    public class MyText : MonoBehaviour
    {
        public Color Color
        {
            get => GetComponent<Text>().color;
            set => GetComponent<Text>().color = value;
        }
        public float PreferredHeight => GetComponent<Text>().preferredHeight;
        public string Text
        {
            get => GetComponent<Text>().text;
            set => GetComponent<Text>().text = value;
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
