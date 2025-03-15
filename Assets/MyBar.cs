using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    public class MyBar : MonoBehaviour
    {
        public interface TextertInterface
        {
            string ToText(float value, float min, float max);
        }

        [SerializeField]
        private float _baringTimeSeconds = 0.2f;
        [SerializeField]
        private float _baringWaitTimeSeconds = 0.2f;
        [SerializeField]
        private Image _deltaImage;
        [SerializeField]
        private Image _frontImage;
        [SerializeField]
        private MyText _text;
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
                        _deltaImage.fillAmount = Mathf.Clamp(value, Min, Max) / s;
                    else
                        _deltaImage.fillAmount = 0;
                }
            }
        }
        // TODO : 좀 더 고민할 필요는 있는 속성
        public float FillAmount { set => _frontImage.fillAmount = value; }
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
                    _frontImage.fillAmount = _value / s;

                    if ((_text != default) && (Texter != default))
                        _text.Text = Texter.ToText(_value, Min, Max);
                }
                else
                {
                    _frontImage.fillAmount = 0;

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
