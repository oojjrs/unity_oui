using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Image))]
    public class MyImage : MonoBehaviour
    {
        public Sprite Sprite
        {
            get => GetComponent<Image>().sprite;
            set => GetComponent<Image>().sprite = value;
        }
    }
}
