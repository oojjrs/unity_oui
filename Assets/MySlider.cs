using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Slider))]
    public class MySlider : MonoBehaviour
    {
        public interface CallbackInterface
        {
            void OnValueChanged(float value);
        }

        public interface InitializerInterface
        {
            float InitialValue { get; }
        }

        [SerializeField]
        private MyText _text;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
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
            if (Initializer != default)
                GetComponent<Slider>().value = Initializer.InitialValue;
        }

        private void Start()
        {
            Callbacks = GetComponents<CallbackInterface>();
            if (Callbacks == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            Initializer = GetComponent<InitializerInterface>();
            if (Initializer != default)
                GetComponent<Slider>().value = Initializer.InitialValue;
        }

        public void OnValueChanged(float value)
        {
            if (Callbacks != default)
            {
                foreach (var callback in Callbacks)
                    callback.OnValueChanged(value);
            }
        }
    }
}
