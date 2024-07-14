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
            set
            {
                // 유니티의 Localization을 사용할 경우 prefab text 값을 edit time에 자꾸 바꿔버리기 때문에 방지해보았다.
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    GetComponent<Text>().text = value;
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
