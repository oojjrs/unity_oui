using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(Button))]
    public partial class MyButton : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        public enum ClickSoundEnum
        {
            Cancel,
            Click,
            Confirm,
            Error,
            Start,
            Switch,
        }

        [System.Serializable]
        public struct SoundOverrides
        {
            public AudioSource Cancel;
            public AudioSource Click;
            public AudioSource Confirm;
            public AudioSource Error;
            public AudioSource Hover;
            public AudioSource Start;
            public AudioSource Switch;
        }

        public interface CallbackInterface
        {
            void OnClick();
        }

        public interface FocusInterface
        {
            void OnFocusEnter();
            void OnFocusExit();
        }

        public interface HoverInterface
        {
            void OnHoverEnter();
            void OnHoverExit();
        }

        public interface LockInterface
        {
            bool KeepWaiting { get; }
        }

        private sealed class InfiniteLock : LockInterface
        {
            bool LockInterface.KeepWaiting => true;
        }

        private static readonly InfiniteLock __infiniteLock = new();

        [SerializeField]
        private Animator _animator;
        private CallbackInterface[] _callbacks;
        private FocusInterface[] _focuses;
        private Coroutine _focusHoverSoundCoroutine;
        private HoverInterface[] _hovers;
        [SerializeField]
        private bool _hoverSoundDisabled;
        [SerializeField]
        private MyImage _image;
        [SerializeField]
        private Color _imageDisableColor = Color.gray;
        [SerializeField]
        private Color _imageNormalColor = Color.white;
        private bool _isCooldowning;
        private bool _isFocused;
        private bool _isStarted;
        [Tooltip("켜면 비활성 상태에서도 이미지를 숨기지 않고 비활성 색상으로 표시합니다.")]
        [SerializeField]
        private bool _isImageVisibleWhenDisabled;
        private bool _isInteractableBeforeCooldown;
        private int _lastClickFrame = -1;
        private int _lastHoverSoundFrame = -1;
        private LockInterface _lock;
        private Coroutine _lockCoroutine;
        private int _lockVersion;
        [SerializeField]
        private SoundOverrides _soundOverrides;
        [SerializeField]
        private MyText _text;
        [SerializeField]
        private Color _textDisableColor = Color.gray;
        [SerializeField]
        private Color _textNormalColor = Color.white;

        public Animator Animator => _animator;
        public ClickSoundEnum ClickSound { get; set; }
        [System.Obsolete("Use IsInteractable instead.")]
        public bool Interactable
        {
            get => IsInteractable;
            set => IsInteractable = value;
        }
        public bool IsInteractable
        {
            get => GetComponent<Button>().interactable;
            set
            {
                GetComponent<Button>().interactable = value;

                if (_image != null)
                {
                    _image.gameObject.SetActive(value || _isImageVisibleWhenDisabled);

                    if (_isImageVisibleWhenDisabled)
                    {
                        if (value)
                            _image.Color = _imageNormalColor;
                        else
                            _image.Color = _imageDisableColor;
                    }
                }

                if (_text != null)
                {
                    if (value)
                        _text.Color = _textNormalColor;
                    else
                        _text.Color = _textDisableColor;
                }
            }
        }
        public bool IsLocked => _lock != null;
        public Sprite Sprite
        {
            get
            {
                if (_image != null)
                {
                    return _image.Sprite;
                }
                else
                {
                    Debug.Assert(false, "이미지 컨트롤 없는 버튼인데?");
                    return null;
                }
            }
            set
            {
                if (_image != null)
                {
                    if (value != null)
                        _image.Sprite = value;
                    else
                        _image.Sprite = MyControl.Image.Null;
                }
                else
                {
                    Debug.Assert(false, "이미지 컨트롤 없는 버튼이라니까?");
                }
            }
        }
        public MyText Text => _text;

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();
            _doubleClicks = GetComponents<DoubleClickInterface>();
            _focuses = GetComponents<FocusInterface>();
            _hovers = GetComponents<HoverInterface>();
            _presses = GetComponents<PressInterface>();
        }

        private void OnDisable()
        {
            var notifyPress = (Application.isPlaying) && (MyControl.IsQuitting == false) && (_pressing);

            OuiFree();
            ReleasePress(false, notifyPress);

            if ((Application.isPlaying == false) || MyControl.IsQuitting)
                return;

            var eventSystem = EventSystem.current;
            if ((eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
                eventSystem.SetSelectedGameObject(null);

            ExitFocus();

            if (_focusHoverSoundCoroutine != null)
            {
                StopCoroutine(_focusHoverSoundCoroutine);
                _focusHoverSoundCoroutine = null;
            }

            if (_isCooldowning)
            {
                _isCooldowning = false;
                IsInteractable = _isInteractableBeforeCooldown;
            }
        }

        private void OnEnable()
        {
            IsInteractable = IsInteractable;

            if (Application.isPlaying && _isStarted)
                SynchronizeFocus();
        }

        private void OnValidate()
        {
            IsInteractable = IsInteractable;
        }

        private void Start()
        {
            if (Application.isPlaying == false)
                return;

            if (_callbacks.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            _isStarted = true;
            SynchronizeFocus();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            ExitFocus();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable)
                PlayHoverSfx();

            if (IsInteractable && (_hovers != null))
            {
                foreach (var hover in _hovers)
                    hover.OnHoverEnter();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (IsInteractable && (_hovers != null))
            {
                foreach (var hover in _hovers)
                    hover.OnHoverExit();
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (IsInteractable)
            {
                PlayFocusHoverSfx(eventData);
                EnterFocus();
            }
        }

        private void CompleteLock(LockInterface condition, int lockVersion)
        {
            if ((_lockVersion == lockVersion) && (object.ReferenceEquals(_lock, condition)))
            {
                ++_lockVersion;
                _lock = null;
                _lockCoroutine = null;
            }
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

        public void OnClick()
        {
            _lastClickFrame = Time.frameCount;

            if (IsLocked)
                return;

            if (_callbacks != null)
            {
                ClickSound = ClickSoundEnum.Click;
                foreach (var callback in _callbacks)
                    callback.OnClick();

                switch (ClickSound)
                {
                    case ClickSoundEnum.Cancel:
                        if (_soundOverrides.Cancel != null)
                            PlaySfxSafety(_soundOverrides.Cancel);
                        else
                            MyControl.Audio.PlayCancelSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Click:
                        if (_soundOverrides.Click != null)
                            PlaySfxSafety(_soundOverrides.Click);
                        else
                            MyControl.Audio.PlayClickSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Confirm:
                        if (_soundOverrides.Confirm != null)
                            PlaySfxSafety(_soundOverrides.Confirm);
                        else
                            MyControl.Audio.PlayConfirmSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Error:
                        if (_soundOverrides.Error != null)
                            PlaySfxSafety(_soundOverrides.Error);
                        else
                            MyControl.Audio.PlayErrorSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Start:
                        if (_soundOverrides.Start != null)
                            PlaySfxSafety(_soundOverrides.Start);
                        else
                            MyControl.Audio.PlayStartSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Switch:
                        if (_soundOverrides.Switch != null)
                            PlaySfxSafety(_soundOverrides.Switch);
                        else
                            MyControl.Audio.PlaySwitchSfx?.Invoke();
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
            else
            {
                if (_soundOverrides.Click != null)
                    PlaySfxSafety(_soundOverrides.Click);
                else
                    MyControl.Audio.PlayClickSfx?.Invoke();
            }
        }

        public void OuiCooldown(float seconds)
        {
            _isCooldowning = true;
            _isInteractableBeforeCooldown = IsInteractable;
            IsInteractable = false;

            _ = StartCoroutine(Func());

            IEnumerator Func()
            {
                if (seconds > 0)
                    yield return new WaitForSeconds(seconds);

                _isCooldowning = false;
                IsInteractable = _isInteractableBeforeCooldown;
            }
        }

        public void OuiFree()
        {
            ++_lockVersion;
            _lock = null;

            if (_lockCoroutine != null)
            {
                StopCoroutine(_lockCoroutine);
                _lockCoroutine = null;
            }
        }

        public void OuiLock()
        {
            OuiLock(__infiniteLock);
        }

        public void OuiLock(LockInterface condition)
        {
            if ((condition == null) || ((condition is Object unityObject) && (unityObject == null)))
                throw new System.ArgumentNullException(nameof(condition));

            var lockVersion = _lockVersion;
            var keepWaiting = condition.KeepWaiting;

            if (_lockVersion != lockVersion)
                return;

            OuiFree();

            if ((Application.isPlaying == false) || (isActiveAndEnabled == false) || (keepWaiting == false))
                return;

            _lock = condition;
            lockVersion = _lockVersion;

            if (object.ReferenceEquals(condition, __infiniteLock) == false)
                _lockCoroutine = StartCoroutine(WatchLockCoroutine(condition, lockVersion));

            BlockPressForLock();
        }

        public void OuiPlayAnimation(string trigger)
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
                animator.SetTrigger(trigger);
            else
                Debug.LogWarning($"{name}> I'M NOT ANIMATED BUTTON.");
        }

        private void OuiPlayClick()
        {
            GetComponent<Button>().OnSubmit(null);
        }

        public void OuiPlayDisabledAnimation()
        {
            OuiPlayAnimation(GetComponent<Button>().animationTriggers.disabledTrigger);
        }

        public void OuiPlayHighlightedAnimation()
        {
            OuiPlayAnimation(GetComponent<Button>().animationTriggers.highlightedTrigger);
        }

        public void OuiPlayNormalAnimation()
        {
            OuiPlayAnimation(GetComponent<Button>().animationTriggers.normalTrigger);
        }

        public void OuiPlayPressedAnimation()
        {
            OuiPlayAnimation(GetComponent<Button>().animationTriggers.pressedTrigger);
        }

        public void OuiPlaySelectedAnimation()
        {
            OuiPlayAnimation(GetComponent<Button>().animationTriggers.selectedTrigger);
        }

        private void __OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable)
                OuiPlayClick();
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

        private void SynchronizeFocus()
        {
            var eventSystem = EventSystem.current;
            if (IsInteractable && (eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
                EnterFocus();
            else
                ExitFocus();
        }

        private IEnumerator WatchLockCoroutine(LockInterface condition, int lockVersion)
        {
            while ((_lockVersion == lockVersion) && (object.ReferenceEquals(_lock, condition)))
            {
                yield return null;

                if ((_lockVersion != lockVersion) || (object.ReferenceEquals(_lock, condition) == false) || ((condition is Object unityObject) && (unityObject == null)))
                    break;

                bool keepWaiting;
                try
                {
                    keepWaiting = condition.KeepWaiting;
                }
                catch
                {
                    CompleteLock(condition, lockVersion);
                    throw;
                }

                if (keepWaiting == false)
                    break;
            }

            CompleteLock(condition, lockVersion);
        }
    }
}
