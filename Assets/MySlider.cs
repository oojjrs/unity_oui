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

        public interface TextertInterface
        {
            string ToText(float value);
        }

        [SerializeField]
        private MyText _text;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
        private TextertInterface Texter { get; set; }
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

            Texter = GetComponent<TextertInterface>();

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

            if (Texter != default)
                _text.Text = Texter.ToText(value);
        }
    }
}
