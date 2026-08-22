using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(RectTransform))]
    public class MySlider : MonoBehaviour, IPointerDownHandler
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
        private AudioSource _clickAudioSource;
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

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            var slider = GetComponent<Slider>();
            if ((eventData.button != PointerEventData.InputButton.Left) || (slider.IsActive() == false) || (slider.IsInteractable() == false))
                return;

            var pointerTransform = eventData.pointerPressRaycast.gameObject != null ? eventData.pointerPressRaycast.gameObject.transform : null;
            if ((slider.handleRect != default) && (pointerTransform != default) && ((pointerTransform == slider.handleRect) || pointerTransform.IsChildOf(slider.handleRect)))
                PlaySfxSafety(_clickAudioSource);
        }

        public void OnLeftButtonClick()
        {
            var slider = GetComponent<Slider>();
            if ((slider.IsActive() == false) || (slider.IsInteractable() == false))
                return;

            PlaySfxSafety(_clickAudioSource);

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

            PlaySfxSafety(_clickAudioSource);

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

        private void PlaySfxSafety(AudioSource audioSource)
        {
            if ((audioSource == default) || (audioSource.clip == default))
                return;

            if (audioSource.gameObject.scene.IsValid())
            {
                audioSource.Play();
            }
            else
            {
                var instance = Instantiate(audioSource);
                instance.Play();

                Destroy(instance.gameObject, instance.clip.length);
            }
        }
    }
}
