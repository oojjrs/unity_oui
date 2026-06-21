using System;
using System.Linq;
using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class MyRadioGroup : MonoBehaviour, MyRadio.GroupInterface
    {
        public enum SelectionModeEnum
        {
            Required,
            Optional,
            Multiple,
        }

        public interface CallbackInterface
        {
            void OnValueChanged(int index, MyRadio radio);
        }

        public interface InitializerInterface
        {
            int InitialIndex { get; }
        }

        private CallbackInterface[] _callbacks;
        [Tooltip("현재 선택 인덱스입니다. Optional의 -1은 선택 없음이고 Multiple은 첫 번째 On 라디오 인덱스로 갱신됩니다.")]
        [SerializeField]
        private int _index = -1;
        private InitializerInterface _initializer;
        private bool _isStarted;
        private MyRadio[] _radios = Array.Empty<MyRadio>();
        [Tooltip("Required는 항상 하나 선택, Optional은 선택 없음 허용, Multiple은 각 라디오를 독립 on/off로 다룹니다.")]
        [SerializeField]
        private SelectionModeEnum _selectionMode;

        public bool AllowSwitchOff
        {
            get => _selectionMode != SelectionModeEnum.Required;
            set => SelectionMode = value ? SelectionModeEnum.Optional : SelectionModeEnum.Required;
        }
        public int Count => _radios.Length;
        public int Index => _index;
        public MyRadio Radio
        {
            get
            {
                if ((_index >= 0) && (_index < _radios.Length))
                    return _radios[_index];
                else
                    return null;
            }
        }
        public SelectionModeEnum SelectionMode
        {
            get => _selectionMode;
            set
            {
                var previousIndex = _index;
                var previousRadio = Radio;

                _selectionMode = value;

                ApplyCurrentIndex(false);

                if ((previousIndex != _index) || (previousRadio != Radio))
                    NotifyValueChanged(_index, Radio);
            }
        }

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
            _initializer = GetComponent<InitializerInterface>();
            _radios = GetComponentsInChildren<MyRadio>(true).Where(t => t.GetComponentInParent<MyRadioGroup>() == this).ToArray();
        }

        private void OnEnable()
        {
            ApplyCurrentIndex(_isStarted);
        }

        private void OnValidate()
        {
            _radios = GetComponentsInChildren<MyRadio>(true).Where(t => t.GetComponentInParent<MyRadioGroup>() == this).ToArray();

            if (_radios.Length > 0)
                ApplyCurrentIndex(true);
        }

        private void Start()
        {
            _isStarted = true;

            if (_radios.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE RADIO.");

            if ((_selectionMode != SelectionModeEnum.Multiple) && (_initializer != null))
                SetIndex(GetValidIndex(_initializer.InitialIndex), true);
            else
                ApplyCurrentIndex(true);
        }

        void MyRadio.GroupInterface.OnClick(MyRadio radio)
        {
            var index = GetIndex(radio);
            if (index < 0)
                return;

            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                radio.OuiSetIsOn(radio.IsOn == false);
                _index = GetFirstOnIndex();
                NotifyValueChanged(index, radio);
            }
            else if ((_selectionMode == SelectionModeEnum.Optional) && (index == _index))
                OuiSelect(-1);
            else
                OuiSelect(index);
        }

        private void Apply(bool notifyRadios)
        {
            if (_selectionMode == SelectionModeEnum.Multiple)
                return;

            for (int i = 0; i < _radios.Length; ++i)
            {
                if (_radios[i] == null)
                    continue;

                var isOn = i == _index;
                if (notifyRadios)
                    _radios[i].OuiSetIsOn(isOn);
                else
                    _radios[i].OuiSetIsOnWithoutNotify(isOn);
            }
        }

        private void ApplyCurrentIndex(bool notify)
        {
            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                _index = GetFirstOnIndex();

                if (notify)
                    NotifyValueChanged(_index, Radio);

                return;
            }

            SetIndex(GetValidIndex(_index), notify);
        }

        private int GetFirstOnIndex()
        {
            for (int i = 0; i < _radios.Length; ++i)
            {
                if ((_radios[i] != null) && _radios[i].IsOn)
                    return i;
            }

            return -1;
        }

        private int GetIndex(MyRadio radio)
        {
            if (radio == null)
                return -1;

            var index = Array.IndexOf(_radios, radio);
            if (index < 0)
                Debug.LogWarning($"{name}> DON'T HAVE RADIO {radio.name}.");

            return index;
        }

        private int GetValidIndex(int index)
        {
            if (_radios.Length <= 0)
                return -1;

            if ((index >= 0) && (index < _radios.Length))
                return index;

            if (_selectionMode == SelectionModeEnum.Required)
                return 0;
            else
                return -1;
        }

        private bool IsIndexValid(int index)
        {
            if (index == -1)
                return (_selectionMode != SelectionModeEnum.Required) || (_radios.Length <= 0);

            return (index >= 0) && (index < _radios.Length);
        }

        private void NotifyValueChanged(int index, MyRadio radio)
        {
            if (_callbacks != null)
            {
                foreach (var callback in _callbacks)
                    callback.OnValueChanged(index, radio);
            }
        }

        public MyRadio OuiGetRadio(int index)
        {
            if ((index >= 0) && (index < _radios.Length))
                return _radios[index];
            else
                return null;
        }

        public void OuiSelect(int index)
        {
            SetIndex(index, true);
        }

        public void OuiSelect(MyRadio radio)
        {
            var index = GetIndex(radio);
            if (index >= 0)
                OuiSelect(index);
        }

        public void OuiSetIndexWithoutNotify(int index)
        {
            SetIndex(index, false);
        }

        private void SetIndex(int index, bool notify)
        {
            if (IsIndexValid(index) == false)
            {
                Debug.LogWarning($"{name}> INVALID RADIO INDEX {index}.");
                return;
            }

            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                if (index == -1)
                {
                    for (int i = 0; i < _radios.Length; ++i)
                    {
                        if ((_radios[i] != null) && _radios[i].IsOn)
                        {
                            if (notify)
                                _radios[i].OuiSetIsOn(false);
                            else
                                _radios[i].OuiSetIsOnWithoutNotify(false);
                        }
                    }

                    _index = -1;

                    if (notify)
                        NotifyValueChanged(-1, null);
                }
                else
                {
                    var radio = _radios[index];

                    if (radio != null)
                    {
                        if (notify)
                            radio.OuiSetIsOn(true);
                        else
                            radio.OuiSetIsOnWithoutNotify(true);
                    }

                    _index = GetFirstOnIndex();

                    if (notify)
                        NotifyValueChanged(index, radio);
                }

                return;
            }

            if (_index == index)
            {
                Apply(false);
                if (notify)
                    NotifyValueChanged(_index, Radio);
                return;
            }

            _index = index;
            Apply(notify);

            if (notify)
            {
                NotifyValueChanged(_index, Radio);
            }
        }
    }
}
