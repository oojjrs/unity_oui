using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Slider))]
    public class MySlider : MonoBehaviour
    {
        public interface CallbackInterface
        {
            float InitialValue { get; }

            void OnValueChanged(float value);
        }

        [SerializeField]
        private MyText _text;

        private CallbackInterface Callback { get; set; }
        public string Text
        {
            get => (_text != default) ? _text.Text : string.Empty;
            set
            {
                if (_text != default)
                    _text.Text = value;
            }
        }
        public float Value
        {
            get => GetComponent<Slider>().value;
            set => GetComponent<Slider>().value = value;
        }

        private void OnEnable()
        {
            if (Callback != default)
                GetComponent<Slider>().value = Callback.InitialValue;
        }

        private void Start()
        {
            Callback = GetComponent<CallbackInterface>();
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            if (Callback != default)
                GetComponent<Slider>().value = Callback.InitialValue;
        }

        public void OnValueChanged(float value)
        {
            Callback?.OnValueChanged(value);
        }
    }
}
