using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class MyRadio : MonoBehaviour, IDeselectHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler, ISubmitHandler
    {
        private enum State
        {
            OffNormal = 0,
            OffHighlighted = 1,
            OffToOnPressed = 2,
            OffSelected = 3,
            OffDisabled = 4,
            OnNormal = 5,
            OnHighlighted = 6,
            OnToOffPressed = 7,
            OnSelected = 8,
            OnDisabled = 9,
        }

        public interface CallbackInterface
        {
            void OnValueChanged(bool isOn);
        }

        public interface GroupInterface
        {
            void OnClick(MyRadio radio);
        }

        public interface HoverInterface
        {
            void OnHoverEnter();
            void OnHoverExit();
        }

        public interface InitializerInterface
        {
            bool InitialValue { get; }
        }

        [System.Serializable]
        public struct SoundOverrides
        {
            public AudioSource Click;
            public AudioSource Hover;
        }

        private CallbackInterface[] _callbacks;
        private GroupInterface _group;
        [SerializeField]
        private bool _hoverSoundDisabled;
        private HoverInterface[] _hovers;
        private InitializerInterface _initializer;
        [SerializeField]
        private bool _isInteractable = true;
        [SerializeField]
        private bool _isOn;
        [SerializeField]
        private GameObject _offNormal;
        [SerializeField]
        private GameObject _offHighlighted;
        [SerializeField]
        private GameObject _offToOnPressed;
        [SerializeField]
        private GameObject _offSelected;
        [SerializeField]
        private GameObject _offDisabled;
        [SerializeField]
        private GameObject _onNormal;
        [SerializeField]
        private GameObject _onHighlighted;
        [SerializeField]
        private GameObject _onToOffPressed;
        [SerializeField]
        private GameObject _onSelected;
        [SerializeField]
        private GameObject _onDisabled;
        [SerializeField]
        private SoundOverrides _soundOverrides;
        private State _state;

        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;

                if (value)
                    SetState(GetDefaultState());
                else if (IsOn)
                    SetState(State.OnDisabled);
                else
                    SetState(State.OffDisabled);
            }
        }
        public bool IsOn
        {
            get => _isOn;
            set => OuiSetIsOn(value);
        }

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
            _group = GetComponentInParent<GroupInterface>();
            _hovers = GetComponents<HoverInterface>();
            _initializer = GetComponent<InitializerInterface>();
        }

        private void OnDisable()
        {
            switch (_state)
            {
                case State.OffDisabled:
                case State.OnDisabled:
                    break;
                case State.OffHighlighted:
                case State.OffNormal:
                case State.OffSelected:
                case State.OffToOnPressed:
                    _state = State.OffNormal;
                    break;
                case State.OnHighlighted:
                case State.OnNormal:
                case State.OnSelected:
                case State.OnToOffPressed:
                    _state = State.OnNormal;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void OnEnable()
        {
            IsInteractable = _isInteractable;
        }

        private void OnTransformParentChanged()
        {
            _group = GetComponentInParent<GroupInterface>();
        }

        private void OnValidate()
        {
            IsInteractable = _isInteractable;
        }

        private void Start()
        {
            if (GetComponentInChildren<Graphic>() == null)
                Debug.LogWarning($"{name}> DON'T HAVE RAYCAST GRAPHIC.");

            if (_initializer != null)
                IsOn = _initializer.InitialValue;
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (IsInteractable)
            {
                if (IsOn)
                    SetState(State.OnNormal);
                else
                    SetState(State.OffNormal);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable && (eventData.button == PointerEventData.InputButton.Left))
                OuiClick();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (IsInteractable && (eventData.button == PointerEventData.InputButton.Left))
            {
                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(gameObject, eventData);

                if (IsOn)
                    SetState(State.OnToOffPressed);
                else
                    SetState(State.OffToOnPressed);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                if (IsOn)
                    SetState(State.OnHighlighted);
                else
                    SetState(State.OffHighlighted);

                if (_hoverSoundDisabled == false)
                {
                    if (_soundOverrides.Hover != null)
                        PlaySfxSafety(_soundOverrides.Hover);
                    else
                        MyControl.Audio.PlayHoverSfx?.Invoke();
                }

                if (_hovers != null)
                {
                    foreach (var hover in _hovers)
                        hover.OnHoverEnter();
                }
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                SetState(GetDefaultState());

                if (_hovers != null)
                {
                    foreach (var hover in _hovers)
                        hover.OnHoverExit();
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (IsInteractable && (eventData.button == PointerEventData.InputButton.Left))
                SetState(GetDefaultState());
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (IsInteractable && (_state != State.OffToOnPressed) && (_state != State.OnToOffPressed))
            {
                if (IsOn)
                    SetState(State.OnSelected);
                else
                    SetState(State.OffSelected);
            }
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable)
                OuiClick();
        }

        private State GetDefaultState()
        {
            return GetDefaultState(IsOn);
        }

        private State GetDefaultState(bool isOn)
        {
            if (isOn)
            {
                if ((EventSystem.current != null) && EventSystem.current.currentSelectedGameObject == gameObject)
                    return State.OnSelected;
                else
                    return State.OnNormal;
            }
            else
            {
                if ((EventSystem.current != null) && EventSystem.current.currentSelectedGameObject == gameObject)
                    return State.OffSelected;
                else
                    return State.OffNormal;
            }
        }

        public void OuiClick()
        {
            if (IsInteractable == false)
                return;

            if (_group != null)
                _group.OnClick(this);
            else
                OuiSetIsOn(IsOn == false);

            if (_soundOverrides.Click != null)
                PlaySfxSafety(_soundOverrides.Click);
            else
                MyControl.Audio.PlaySwitchSfx?.Invoke();

            OuiUpdate();
        }

        public void OuiSetIsOn(bool isOn)
        {
            if (isOn == IsOn)
            {
                OuiUpdate();
                return;
            }

            OuiSetIsOnWithoutNotify(isOn);

            if (_callbacks != null)
            {
                foreach (var callback in _callbacks)
                    callback.OnValueChanged(isOn);
            }
        }

        public void OuiSetIsOnWithoutNotify(bool isOn)
        {
            switch (_state)
            {
                case State.OffDisabled:
                case State.OnDisabled:
                    if (isOn)
                        SetState(State.OnDisabled);
                    else
                        SetState(State.OffDisabled);
                    break;
                case State.OffHighlighted:
                case State.OnHighlighted:
                    if (isOn)
                        SetState(State.OnHighlighted);
                    else
                        SetState(State.OffHighlighted);
                    break;
                case State.OffSelected:
                case State.OnSelected:
                    if (isOn)
                        SetState(State.OnSelected);
                    else
                        SetState(State.OffSelected);
                    break;
                case State.OffToOnPressed:
                case State.OnToOffPressed:
                    SetState(GetDefaultState(isOn));
                    break;
                default:
                    if (isOn)
                        SetState(State.OnNormal);
                    else
                        SetState(State.OffNormal);
                    break;
            }
        }

        public void OuiUpdate()
        {
            SetState(_state);
        }

        private void PlaySfxSafety(AudioSource audioSource)
        {
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

        private void SetState(State state)
        {
            _state = state;
            _isInteractable = (_state != State.OffDisabled) && (_state != State.OnDisabled);
            _isOn = _state switch
            {
                State.OnNormal or
                State.OnHighlighted or
                State.OnToOffPressed or
                State.OnSelected or
                State.OnDisabled => true,
                _ => false,
            };

            if (((state == State.OffNormal) && (_offNormal == null)) ||
                ((state == State.OffHighlighted) && (_offHighlighted == null)) ||
                ((state == State.OffToOnPressed) && (_offToOnPressed == null)) ||
                ((state == State.OffSelected) && (_offSelected == null)) ||
                ((state == State.OffDisabled) && (_offDisabled == null)) ||
                ((state == State.OnNormal) && (_onNormal == null)) ||
                ((state == State.OnHighlighted) && (_onHighlighted == null)) ||
                ((state == State.OnToOffPressed) && (_onToOffPressed == null)) ||
                ((state == State.OnSelected) && (_onSelected == null)) ||
                ((state == State.OnDisabled) && (_onDisabled == null)))
            {
                Debug.LogWarning($"{name}> DON'T HAVE RADIO STATE {state}.");
            }

            if (_offNormal != null)
                _offNormal.SetActive(state == State.OffNormal);

            if (_offHighlighted != null)
                _offHighlighted.SetActive(state == State.OffHighlighted);

            if (_offToOnPressed != null)
                _offToOnPressed.SetActive(state == State.OffToOnPressed);

            if (_offSelected != null)
                _offSelected.SetActive(state == State.OffSelected);

            if (_offDisabled != null)
                _offDisabled.SetActive(state == State.OffDisabled);

            if (_onNormal != null)
                _onNormal.SetActive(state == State.OnNormal);

            if (_onHighlighted != null)
                _onHighlighted.SetActive(state == State.OnHighlighted);

            if (_onToOffPressed != null)
                _onToOffPressed.SetActive(state == State.OnToOffPressed);

            if (_onSelected != null)
                _onSelected.SetActive(state == State.OnSelected);

            if (_onDisabled != null)
                _onDisabled.SetActive(state == State.OnDisabled);
        }
    }
}
