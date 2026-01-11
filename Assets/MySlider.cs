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
        private bool Started { get; set; }
        private TextertInterface Texter { get; set; }
        public float Value
        {
            get => GetComponent<Slider>().value;
            set
            {
                // -_- 호출 안해줄 줄이야...
                GetComponent<Slider>().value = value;

                OnValueChanged(value);
            }
        }

        private void Awake()
        {
            Callbacks = GetComponents<CallbackInterface>();
            Initializer = GetComponent<InitializerInterface>();
            Texter = GetComponent<TextertInterface>();
        }

        private void OnEnable()
        {
            if (Started)
            {
                if (Initializer != default)
                    Value = Initializer.InitialValue;
            }
        }

        private void Start()
        {
            if (Callbacks?.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            if (Initializer != default)
                Value = Initializer.InitialValue;

            Started = true;
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
