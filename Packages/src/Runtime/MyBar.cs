using System.Collections;
using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class MyBar : MonoBehaviour
    {
        public enum FillModeEnum
        {
            FillAmount,
            Resize,
        }

        public interface TextertInterface
        {
            string ToText(float value, float min, float max);
        }

        [SerializeField]
        private float _baringTimeSeconds = 0.2f;
        [SerializeField]
        private float _baringWaitTimeSeconds = 0.2f;
        [Tooltip("선택 사항입니다. 연결하지 않으면 델타 이미지를 갱신하지 않습니다.")]
        [SerializeField]
        private MyImage _deltaImage;
        [Tooltip("FillAmount는 Filled 이미지의 채움량, Resize는 Sliced 이미지의 가로 길이를 조절합니다. Resize는 아주 짧아지면 양끝도 압축됩니다.")]
        [SerializeField]
        private FillModeEnum _fillMode;
        [Tooltip("Resize에서는 부모가 전체 채움 영역입니다. 가로 앵커와 오프셋을 조절하며 세로 배치는 유지합니다.")]
        [SerializeField]
        private MyImage _frontImage;
        [Tooltip("선택 사항입니다. 연결하지 않으면 텍스트를 갱신하지 않습니다.")]
        [SerializeField]
        private MyTextBase _text;
        private float _value;

        private Coroutine Coroutine { get; set; }
        private float Delta
        {
            set
            {
                if (_deltaImage != default)
                {
                    var s = Max - Min;
                    if (s > 0)
                        ApplyFillAmount(_deltaImage, Mathf.Clamp(value, Min, Max) / s);
                    else
                        ApplyFillAmount(_deltaImage, 0);
                }
            }
        }
        // TODO : 좀 더 고민할 필요는 있는 속성
        public float FillAmount { set => ApplyFillAmount(_frontImage, value); }
        private bool Init { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
        private TextertInterface Texter { get; set; }
        public float Value
        {
            get => _value;
            private set
            {
                Init = true;
                _value = Mathf.Clamp(value, Min, Max);

                var s = Max - Min;
                if (s > 0)
                {
                    ApplyFillAmount(_frontImage, _value / s);

                    if ((_text != default) && (Texter != default))
                        _text.Text = Texter.ToText(_value, Min, Max);
                }
                else
                {
                    ApplyFillAmount(_frontImage, 0);

                    if (_text != default)
                        _text.Text = string.Empty;
                }
            }
        }
        public float ValueAndDelta
        {
            set
            {
                CancelAnimate();

                Delta = value;
                Value = value;
            }
        }
        private float ValueStart { get; set; }
        private float ValueTarget { get; set; }

        private void Awake()
        {
            Texter = GetComponent<TextertInterface>();
        }

        private void ApplyFillAmount(MyImage image, float amount)
        {
            amount = Mathf.Clamp01(amount);

            if (_fillMode == FillModeEnum.FillAmount)
            {
                image.FillAmount = amount;
            }
            else
            {
                var rectTransform = image.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, rectTransform.anchorMin.y);
                rectTransform.anchorMax = new Vector2(amount, rectTransform.anchorMax.y);
                rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
            }
        }

        private void CancelAnimate()
        {
            if (Coroutine != default)
            {
                StopCoroutine(Coroutine);
                Coroutine = default;
            }
        }

        public void Fill(float from, float to, float seconds)
        {
            if (Coroutine != default)
            {
                StopCoroutine(Coroutine);
                Coroutine = default;
            }

            Delta = 0;

            if (seconds > 0)
            {
                Coroutine = StartCoroutine(Func());

                IEnumerator Func()
                {
                    Value = from;

                    var time = Time.time;
                    while (Time.time - time < seconds)
                    {
                        var t = Mathf.Clamp01((Time.time - time) / seconds);
                        Value = Mathf.Lerp(from, to, t);

                        yield return default;
                    }

                    Value = to;
                    Coroutine = default;
                }
            }
            else
            {
                Value = to;
            }
        }

        public void Play(float value)
        {
            if (value != _value)
                CancelAnimate();

            if (value > _value)
            {
                Coroutine = StartCoroutine(AddFunc(value));
            }
            else if (value < _value)
            {
                Coroutine = StartCoroutine(SubtractFunc(value));
            }
            else
            {
                // 아무래도 여기에 걸릴 때가 있고, 그럼 걍 애니 없이 셋팅해버리는 걸로 결정했다.
                Delta = 0;
                Value = value;
            }

            IEnumerator AddFunc(float value)
            {
                Delta = value;
                ValueTarget = value;

                if (Init)
                {
                    yield return new WaitForSeconds(_baringWaitTimeSeconds);

                    var time = Time.time;
                    while (Time.time - time < _baringTimeSeconds)
                    {
                        var t = Mathf.Clamp01((Time.time - time) / _baringTimeSeconds);
                        Value = Mathf.Lerp(_value, ValueTarget, t);

                        yield return default;
                    }
                }

                Value = ValueTarget;

                Coroutine = default;
            }

            IEnumerator SubtractFunc(float value)
            {
                ValueStart = _value;
                Value = value;

                if (Init)
                {
                    yield return new WaitForSeconds(_baringWaitTimeSeconds);

                    var time = Time.time;
                    while (Time.time - time < _baringTimeSeconds)
                    {
                        var t = Mathf.Clamp01((Time.time - time) / _baringTimeSeconds);
                        Delta = Mathf.Lerp(ValueStart, _value, t);

                        yield return default;
                    }
                }

                Delta = _value;

                Coroutine = default;
            }
        }
    }
}
