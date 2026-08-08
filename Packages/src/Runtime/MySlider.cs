using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public class MySlider : MonoBehaviour
    {
        private const float ContinuousStepRatio = 0.05f;
        private const float WholeNumberStep = 5f;

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
        private int _valueChangedVersion;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
        private bool Started { get; set; }
        private TextertInterface Texter { get; set; }
        public float Value
        {
            get => GetComponent<Slider>().value;
            set
            {
                var slider = GetComponent<Slider>();
                var valueChangedVersion = _valueChangedVersion;
                slider.value = value;

                if (_valueChangedVersion == valueChangedVersion)
                    OnValueChanged(slider.value);
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

        public void OnLeftButtonClick()
        {
            var slider = GetComponent<Slider>();
            if ((slider.IsActive() == false) || (slider.IsInteractable() == false))
                return;

            var step = slider.wholeNumbers ? WholeNumberStep : (slider.maxValue - slider.minValue) * ContinuousStepRatio;
            var value = Mathf.Clamp(slider.value + ((slider.direction == Slider.Direction.RightToLeft) ? step : -step), slider.minValue, slider.maxValue);
            if (value != slider.value)
                Value = value;
        }

        public void OnRightButtonClick()
        {
            var slider = GetComponent<Slider>();
            if ((slider.IsActive() == false) || (slider.IsInteractable() == false))
                return;

            var step = slider.wholeNumbers ? WholeNumberStep : (slider.maxValue - slider.minValue) * ContinuousStepRatio;
            var value = Mathf.Clamp(slider.value + ((slider.direction == Slider.Direction.RightToLeft) ? -step : step), slider.minValue, slider.maxValue);
            if (value != slider.value)
                Value = value;
        }

        public void OnValueChanged(float value)
        {
            ++_valueChangedVersion;

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
