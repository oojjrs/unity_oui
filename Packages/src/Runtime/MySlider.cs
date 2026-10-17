using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(RectTransform))]
    public class MySlider : MonoBehaviour, IInitializePotentialDragHandler, IPointerDownHandler
    {
        public interface CallbackInterface
        {
            void OnValueChanged(float value);
        }

        public interface InitializerInterface
        {
            float InitialValue { get; }
        }

        public interface IntCallbackInterface
        {
            void OnValueChanged(int value);
        }

        public interface IntInitializerInterface
        {
            int InitialValue { get; }
        }

        public interface IntTexterInterface
        {
            string ToText(int value);
        }

        public interface TextertInterface
        {
            string ToText(float value);
        }

        private CallbackInterface[] _callbacks;
        [SerializeField]
        private AudioSource _clickAudioSource;
        [SerializeField]
        [Range(0f, 1f)]
        private float _continuousStepRatio = 0.05f;
        private InitializerInterface _initializer;
        private IntCallbackInterface[] _intCallbacks;
        private IntInitializerInterface _intInitializer;
        private IntTexterInterface _intTexter;
        private bool _isStarted;
        [SerializeField]
        private MyTextBase _text;
        private TextertInterface _texter;
        private int _valueChangedVersion;
        [SerializeField]
        [Min(1)]
        private int _wholeNumberStep = 1;

        public float ContinuousStepRatio { get => Mathf.Clamp01(_continuousStepRatio); set => _continuousStepRatio = Mathf.Clamp01(value); }
        public float MaxValue
        {
            get => GetComponent<Slider>().maxValue;
            set
            {
                var slider = GetComponent<Slider>();
                var previousValue = slider.value;
                var valueChangedVersion = _valueChangedVersion;
                slider.maxValue = value;

                if ((slider.value != previousValue) && (_valueChangedVersion == valueChangedVersion))
                    OnValueChanged(slider.value);
            }
        }
        public int MaxValueAsInteger { get => Mathf.RoundToInt(MaxValue); set => MaxValue = value; }
        public float MinValue
        {
            get => GetComponent<Slider>().minValue;
            set
            {
                var slider = GetComponent<Slider>();
                var previousValue = slider.value;
                var valueChangedVersion = _valueChangedVersion;
                slider.minValue = value;

                if ((slider.value != previousValue) && (_valueChangedVersion == valueChangedVersion))
                    OnValueChanged(slider.value);
            }
        }
        public int MinValueAsInteger { get => Mathf.RoundToInt(MinValue); set => MinValue = value; }
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
        public int ValueAsInteger { get => Mathf.RoundToInt(Value); set => Value = value; }
        public bool WholeNumbers
        {
            get => GetComponent<Slider>().wholeNumbers;
            set
            {
                var slider = GetComponent<Slider>();
                var previousValue = slider.value;
                var valueChangedVersion = _valueChangedVersion;
                slider.wholeNumbers = value;

                if ((slider.value != previousValue) && (_valueChangedVersion == valueChangedVersion))
                    OnValueChanged(slider.value);
            }
        }
        public int WholeNumberStep { get => Mathf.Max(1, _wholeNumberStep); set => _wholeNumberStep = Mathf.Max(1, value); }

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
            _initializer = GetComponent<InitializerInterface>();
            _intCallbacks = GetComponents<IntCallbackInterface>();
            _intInitializer = GetComponent<IntInitializerInterface>();
            _intTexter = GetComponent<IntTexterInterface>();
            _texter = GetComponent<TextertInterface>();
        }

        private void OnEnable()
        {
            if (_isStarted)
                InitializeValue();
        }

        private void Start()
        {
            if ((_callbacks?.Length <= 0) && (_intCallbacks?.Length <= 0))
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            InitializeValue();

            _isStarted = true;
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            var pointerPress = eventData.pointerPress;
            if ((pointerPress != default) && (pointerPress != gameObject) && pointerPress.transform.IsChildOf(transform) && pointerPress.TryGetComponent<Button>(out _))
                eventData.pointerDrag = null;
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

        private void InitializeValue()
        {
            if (_intInitializer != default)
                ValueAsInteger = _intInitializer.InitialValue;
            else if (_initializer != default)
                Value = _initializer.InitialValue;
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

            if (_callbacks != default)
            {
                foreach (var callback in _callbacks)
                    callback.OnValueChanged(value);
            }

            if (_intCallbacks != default)
            {
                foreach (var callback in _intCallbacks)
                    callback.OnValueChanged(Mathf.RoundToInt(value));
            }

            if (_intTexter != default)
                _text.Text = _intTexter.ToText(Mathf.RoundToInt(value));
            else if (_texter != default)
                _text.Text = _texter.ToText(value);
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
