using System.Collections;
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

        [System.Serializable]
        public struct SoundOverrides
        {
            public AudioSource Click;
            public AudioSource Hover;
        }

        [System.Serializable]
        public struct StateObjects
        {
            public GameObject OffNormal;
            public GameObject OffHighlighted;
            public GameObject OffToOnPressed;
            public GameObject OffSelected;
            public GameObject OffDisabled;
            public GameObject OnNormal;
            public GameObject OnHighlighted;
            public GameObject OnToOffPressed;
            public GameObject OnSelected;
            public GameObject OnDisabled;
        }

        public interface CallbackInterface
        {
            void OnValueChanged(bool isOn);
        }

        public interface FocusInterface
        {
            void OnFocusEnter();
            void OnFocusExit();
        }

        public interface GroupInterface
        {
            bool Contains(MyRadio radio);
            bool OnClick(MyRadio radio);
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

        private CallbackInterface[] _callbacks;
        private FocusInterface[] _focuses;
        private Coroutine _focusHoverSoundCoroutine;
        private HoverInterface[] _hovers;
        [SerializeField]
        private bool _hoverSoundDisabled;
        [SerializeField]
        private MyImage[] _images;
        private InitializerInterface _initializer;
        private bool _isFocused;
        private bool _isStarted;
        [SerializeField]
        private bool _isInteractable = true;
        [SerializeField]
        private bool _isOn;
        private int _lastClickFrame = -1;
        private int _lastHoverSoundFrame = -1;
        [SerializeField]
        private SoundOverrides _soundOverrides;
        private State _state;
        [SerializeField]
        private StateObjects _stateObjects;
        [SerializeField]
        private MyText[] _texts;

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

                if ((value == false) && Application.isPlaying)
                {
                    var eventSystem = EventSystem.current;
                    if ((eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
                        eventSystem.SetSelectedGameObject(null);
                }
            }
        }
        public bool IsOn
        {
            get => _isOn;
            set => OuiSetIsOn(value);
        }
        public Sprite Sprite
        {
            set
            {
                if (_images != null)
                {
                    if (value != null)
                    {
                        foreach (var image in _images)
                        {
                            if (image != null)
                                image.Sprite = value;
                        }
                    }
                    else
                    {
                        foreach (var image in _images)
                        {
                            if (image != null)
                                image.Sprite = MyControl.Image.Null;
                        }
                    }
                }
            }
        }
        public string Title
        {
            set
            {
                if (_texts != null)
                {
                    foreach (var text in _texts)
                    {
                        if (text != null)
                            text.Text = value;
                    }
                }
            }
        }

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
            _focuses = GetComponents<FocusInterface>();
            _hovers = GetComponents<HoverInterface>();
            _initializer = GetComponent<InitializerInterface>();
        }

        private void OnDisable()
        {
            if (Application.isPlaying && (MyControl.IsQuitting == false))
            {
                var eventSystem = EventSystem.current;
                if ((eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
                    eventSystem.SetSelectedGameObject(null);

                ExitFocus();
            }

            if (_focusHoverSoundCoroutine != null)
            {
                StopCoroutine(_focusHoverSoundCoroutine);
                _focusHoverSoundCoroutine = null;
            }

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

            if (Application.isPlaying && _isStarted)
                SynchronizeFocus();
        }

        private void OnValidate()
        {
            IsInteractable = _isInteractable;
        }

        private void Start()
        {
            if (GetComponentInChildren<Graphic>() == null)
                Debug.LogWarning($"{name}> DON'T HAVE RAYCAST GRAPHIC.");

            var group = GetComponentInParent<GroupInterface>();
            if (((group == null) || (group.Contains(this) == false)) && (_initializer != null))
                IsOn = _initializer.InitialValue;

            if (Application.isPlaying)
            {
                _isStarted = true;
                SynchronizeFocus();
            }
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

            ExitFocus();
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

                PlayHoverSfx();

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

            if (IsInteractable)
            {
                PlayFocusHoverSfx(eventData);
                EnterFocus();
            }
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable)
                OuiClick();
        }

        private void EnterFocus()
        {
            if ((_isStarted == false) || _isFocused)
                return;

            _isFocused = true;

            if (_focuses != null)
            {
                foreach (var focus in _focuses)
                    focus.OnFocusEnter();
            }
        }

        private void ExitFocus()
        {
            if (_isFocused == false)
                return;

            _isFocused = false;

            if (_focuses != null)
            {
                foreach (var focus in _focuses)
                    focus.OnFocusExit();
            }
        }

        private void SynchronizeFocus()
        {
            var eventSystem = EventSystem.current;
            if (IsInteractable && (eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
            {
                if ((_state != State.OffToOnPressed) && (_state != State.OnToOffPressed))
                {
                    if (IsOn)
                        SetState(State.OnSelected);
                    else
                        SetState(State.OffSelected);
                }

                EnterFocus();
            }
            else
            {
                ExitFocus();
            }
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

            _lastClickFrame = Time.frameCount;

            var group = GetComponentInParent<GroupInterface>();
            if ((group == null) || (group.Contains(this) == false) || (group.OnClick(this) == false))
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

        private void PlayFocusHoverSfx(BaseEventData eventData)
        {
            if ((Application.isPlaying == false) || _hoverSoundDisabled)
                return;

            if (_focusHoverSoundCoroutine != null)
                StopCoroutine(_focusHoverSoundCoroutine);

            _focusHoverSoundCoroutine = StartCoroutine(PlayFocusHoverSfxCoroutine(Time.frameCount));
        }

        private IEnumerator PlayFocusHoverSfxCoroutine(int focusFrame)
        {
            yield return null;

            _focusHoverSoundCoroutine = null;

            if ((IsInteractable == false) || (_lastClickFrame == focusFrame) || (_lastHoverSoundFrame == focusFrame))
                yield break;

            if ((EventSystem.current != null) && (EventSystem.current.currentSelectedGameObject != gameObject))
                yield break;

            PlayHoverSfx();
        }

        private void PlayHoverSfx()
        {
            if ((Application.isPlaying == false) || _hoverSoundDisabled || (_lastHoverSoundFrame == Time.frameCount))
                return;

            _lastHoverSoundFrame = Time.frameCount;

            if (_soundOverrides.Hover != null)
                PlaySfxSafety(_soundOverrides.Hover);
            else
                MyControl.Audio.PlayHoverSfx?.Invoke();
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

            if (((state == State.OffNormal) && (_stateObjects.OffNormal == null)) ||
                ((state == State.OffHighlighted) && (_stateObjects.OffHighlighted == null)) ||
                ((state == State.OffToOnPressed) && (_stateObjects.OffToOnPressed == null)) ||
                ((state == State.OffSelected) && (_stateObjects.OffSelected == null)) ||
                ((state == State.OffDisabled) && (_stateObjects.OffDisabled == null)) ||
                ((state == State.OnNormal) && (_stateObjects.OnNormal == null)) ||
                ((state == State.OnHighlighted) && (_stateObjects.OnHighlighted == null)) ||
                ((state == State.OnToOffPressed) && (_stateObjects.OnToOffPressed == null)) ||
                ((state == State.OnSelected) && (_stateObjects.OnSelected == null)) ||
                ((state == State.OnDisabled) && (_stateObjects.OnDisabled == null)))
            {
                Debug.LogWarning($"{name}> DON'T HAVE RADIO STATE {state}.");
            }

            if (_stateObjects.OffNormal != null)
                _stateObjects.OffNormal.SetActive(state == State.OffNormal);

            if (_stateObjects.OffHighlighted != null)
                _stateObjects.OffHighlighted.SetActive(state == State.OffHighlighted);

            if (_stateObjects.OffToOnPressed != null)
                _stateObjects.OffToOnPressed.SetActive(state == State.OffToOnPressed);

            if (_stateObjects.OffSelected != null)
                _stateObjects.OffSelected.SetActive(state == State.OffSelected);

            if (_stateObjects.OffDisabled != null)
                _stateObjects.OffDisabled.SetActive(state == State.OffDisabled);

            if (_stateObjects.OnNormal != null)
                _stateObjects.OnNormal.SetActive(state == State.OnNormal);

            if (_stateObjects.OnHighlighted != null)
                _stateObjects.OnHighlighted.SetActive(state == State.OnHighlighted);

            if (_stateObjects.OnToOffPressed != null)
                _stateObjects.OnToOffPressed.SetActive(state == State.OnToOffPressed);

            if (_stateObjects.OnSelected != null)
                _stateObjects.OnSelected.SetActive(state == State.OnSelected);

            if (_stateObjects.OnDisabled != null)
                _stateObjects.OnDisabled.SetActive(state == State.OnDisabled);
        }
    }
}
