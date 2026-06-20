using System;
using System.Collections.Generic;
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

        [Tooltip("Required: always one selected. Optional: zero or one selected. Multiple: independent on/off radios.")]
        [SerializeField]
        private SelectionModeEnum _selectionMode;
        [Tooltip("Initial selected radio index for Required/Optional. Optional uses -1 for no selection. Multiple ignores this value.")]
        [SerializeField]
        private int _initialIndex = -1;
        private int _index = -1;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
        private MyRadio[] Radios { get; set; }
        public bool AllowSwitchOff
        {
            get => _selectionMode != SelectionModeEnum.Required;
            set => SelectionMode = value ? SelectionModeEnum.Optional : SelectionModeEnum.Required;
        }
        public int Count => Radios?.Length ?? 0;
        public int Index => _index;
        public MyRadio Radio
        {
            get
            {
                if ((Radios != null) && (_index >= 0) && (_index < Radios.Length))
                    return Radios[_index];
                else
                    return null;
            }
        }
        public SelectionModeEnum SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;

                if (Radios != null)
                {
                    if (_selectionMode == SelectionModeEnum.Multiple)
                        _index = GetFirstOnIndex();
                    else
                    {
                        if (IsIndexValid(_index) == false)
                            _index = GetInitialIndex();

                        Apply(false);
                    }
                }
            }
        }

        private void Awake()
        {
            RefreshDependencies();
            OuiRefresh();
        }

        private void OnEnable()
        {
            RefreshDependencies();
            OuiRefresh();
            ApplyInitialIndex(false);
        }

        private void OnTransformChildrenChanged()
        {
            var radio = Radio;

            OuiRefresh();

            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                _index = GetFirstOnIndex();
                return;
            }

            if (radio != null)
            {
                var index = Array.IndexOf(Radios, radio);
                if (index >= 0)
                    _index = index;
            }

            if ((Radios == null) || (_index >= Radios.Length))
                _index = -1;

            Apply(false);
        }

        private void OnValidate()
        {
            RefreshDependencies();
            OuiRefresh();
            NormalizeInitialIndex();
            ApplyInitialIndex(true);
        }

        private void Start()
        {
            if (Radios?.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE RADIO.");
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
            if (Radios == null)
                return;

            if (_selectionMode == SelectionModeEnum.Multiple)
                return;

            for (int i = 0; i < Radios.Length; ++i)
            {
                if (Radios[i] == null)
                    continue;

                var isOn = i == _index;
                if (notifyRadios)
                    Radios[i].OuiSetIsOn(isOn);
                else
                    Radios[i].OuiSetIsOnWithoutNotify(isOn);
            }
        }

        private void ApplyInitialIndex(bool forced)
        {
            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                _index = GetFirstOnIndex();
            }
            else if (Initializer != null)
                OuiSetIndexWithoutNotify(GetInitialIndex());
            else if (forced)
                OuiSetIndexWithoutNotify(GetInitialIndex());
            else if (IsIndexValid(_index))
                Apply(false);
            else
                OuiSetIndexWithoutNotify(GetInitialIndex());
        }

        private int GetFirstOnIndex()
        {
            if (Radios == null)
                return -1;

            for (int i = 0; i < Radios.Length; ++i)
            {
                if ((Radios[i] != null) && Radios[i].IsOn)
                    return i;
            }

            return -1;
        }

        private int GetIndex(MyRadio radio)
        {
            if (radio == null)
                return -1;

            if (Radios == null)
                OuiRefresh();

            var index = Array.IndexOf(Radios, radio);
            if (index >= 0)
                return index;

            OuiRefresh();
            index = Array.IndexOf(Radios, radio);
            if (index < 0)
                Debug.LogWarning($"{name}> DON'T HAVE RADIO {radio.name}.");

            return index;
        }

        private int GetInitialIndex()
        {
            if (Radios?.Length <= 0)
                return -1;

            var initialIndex = Initializer != null ? Initializer.InitialIndex : _initialIndex;
            if ((initialIndex >= 0) && (initialIndex < Radios.Length))
                return initialIndex;

            if (_selectionMode == SelectionModeEnum.Required)
                return 0;
            else
                return -1;
        }

        private bool IsIndexValid(int index)
        {
            if (index == -1)
                return (_selectionMode != SelectionModeEnum.Required) || (Radios == null) || (Radios.Length <= 0);

            return (Radios != null) && (index >= 0) && (index < Radios.Length);
        }

        private void NormalizeInitialIndex()
        {
            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                _initialIndex = -1;
                return;
            }

            if (Radios?.Length <= 0)
            {
                _initialIndex = -1;
                return;
            }

            if (_selectionMode == SelectionModeEnum.Required)
                _initialIndex = Mathf.Clamp(_initialIndex, 0, Radios.Length - 1);
            else
                _initialIndex = Mathf.Clamp(_initialIndex, -1, Radios.Length - 1);
        }

        private void NotifyValueChanged(int index, MyRadio radio)
        {
            if (Callbacks != null)
            {
                foreach (var callback in Callbacks)
                    callback.OnValueChanged(index, radio);
            }
        }

        public void OuiRefresh()
        {
            var radios = GetComponentsInChildren<MyRadio>(true);
            var filtered = new List<MyRadio>();

            foreach (var radio in radios)
            {
                if (radio.GetComponentInParent<MyRadioGroup>() == this)
                    filtered.Add(radio);
            }

            Radios = filtered.ToArray();
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

        private void RefreshDependencies()
        {
            Callbacks = GetComponents<CallbackInterface>();
            Initializer = GetComponent<InitializerInterface>();
        }

        private void SetIndex(int index, bool notify)
        {
            if (Radios == null)
                OuiRefresh();

            if (IsIndexValid(index) == false)
            {
                Debug.LogWarning($"{name}> INVALID RADIO INDEX {index}.");
                return;
            }

            if (_selectionMode == SelectionModeEnum.Multiple)
            {
                if (index == -1)
                {
                    var changed = false;

                    for (int i = 0; i < Radios.Length; ++i)
                    {
                        if ((Radios[i] != null) && Radios[i].IsOn)
                        {
                            changed = true;

                            if (notify)
                                Radios[i].OuiSetIsOn(false);
                            else
                                Radios[i].OuiSetIsOnWithoutNotify(false);
                        }
                    }

                    _index = -1;

                    if (changed && notify)
                        NotifyValueChanged(-1, null);
                }
                else
                {
                    var radio = Radios[index];
                    var changed = (radio != null) && (radio.IsOn == false);

                    if (radio != null)
                    {
                        if (notify)
                            radio.OuiSetIsOn(true);
                        else
                            radio.OuiSetIsOnWithoutNotify(true);
                    }

                    _index = GetFirstOnIndex();

                    if (changed && notify)
                        NotifyValueChanged(index, radio);
                }

                return;
            }

            if (_index == index)
            {
                Apply(false);
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
