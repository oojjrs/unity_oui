using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.oojjrs.oui
{
    [RequireComponent(typeof(Button))]
    public partial class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        public interface CallbackInterface
        {
            void OnClick();
        }

        public interface HoverInterface
        {
            void OnHoverEnter();
            void OnHoverExit();
        }

        [SerializeField]
        private GameObject _dimmedCover;
        [SerializeField]
        private bool _hoverSoundDisabled;
        [SerializeField]
        private Color _textDisableColor;
        [SerializeField]
        private Color _textNormalColor;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private MyText _text;

        private CallbackInterface Callback { get; set; }
        public ClickSoundEnum ClickSound { get; set; }
        private HoverInterface Hover { get; set; }
        public bool Interactable { get => GetComponent<Button>().interactable; set => GetComponent<Button>().interactable = value; }
        public bool InteractableWithDimmedCover
        {
            set
            {
                Interactable = value;

                if (_dimmedCover != default)
                    _dimmedCover.SetActive(value == false);
            }
        }
        public bool InteractableWithSprite
        {
            set
            {
                GetComponent<Button>().interactable = value;
                SpriteActive = value;
            }
        }
        public bool InteractableWithText
        {
            set
            {
                GetComponent<Button>().interactable = value;
                TextInteractable = value;
            }
        }
        public Sprite Sprite
        {
            get
            {
                if (_image != default)
                {
                    return _image.sprite;
                }
                else
                {
                    Debug.Assert(false, "이미지 컨트롤 없는 버튼인데?");
                    return default;
                }
            }
            set
            {
                if (_image != default)
                {
                    if (value != default)
                        _image.sprite = value;
                    else
                        _image.sprite = MyControl.Image.Null;
                }
                else
                    Debug.Assert(false, "이미지 컨트롤 없는 버튼이라니까?");
            }
        }
        public bool SpriteActive
        {
            get
            {
                if (_image != default)
                    return _image.gameObject.activeSelf;
                else
                    return false;
            }
            set
            {
                if (_image != default)
                    _image.gameObject.SetActive(value);
                else
                    Debug.Assert(false, "이미지 컨트롤 없다구요");
            }
        }
        public MyText Text => _text;
        public bool TextActive
        {
            get
            {
                if (_text != default)
                    return _text.gameObject.activeSelf;
                else
                    return false;
            }
            set
            {
                if (_text != default)
                    _text.gameObject.SetActive(value);
                else
                    Debug.Assert(false, "텍스트 컨트롤 없다구요");
            }
        }
        private bool TextInteractable
        {
            set
            {
                if (_text != default)
                {
                    if (value)
                        _text.Color = _textNormalColor;
                    else
                        _text.Color = _textDisableColor;
                }
                else
                {
                    Debug.Assert(false, "텍스트 컨트롤 없다구요");
                }
            }
        }

        private void Start()
        {
            Callback = GetComponent<CallbackInterface>();
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            DoubleClick = GetComponent<DoubleClickInterface>();
            Hover = GetComponent<HoverInterface>();
            Press = GetComponent<PressInterface>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable && (_hoverSoundDisabled == false))
                MyControl.Audio.PlayHoverSfx?.Invoke();

            if (Interactable && (Hover != default))
                Hover.OnHoverEnter();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (Interactable && (Hover != default))
                Hover.OnHoverExit();
        }

        public void OnClick()
        {
            if (Callback != default)
            {
                ClickSound = ClickSoundEnum.Click;
                Callback.OnClick();

                switch (ClickSound)
                {
                    case ClickSoundEnum.Cancel:
                        MyControl.Audio.PlayCancelSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Click:
                        MyControl.Audio.PlayClickSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Confirm:
                        MyControl.Audio.PlayConfirmSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Error:
                        MyControl.Audio.PlayErrorSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Start:
                        MyControl.Audio.PlayStartSfx?.Invoke();
                        break;
                    case ClickSoundEnum.Switch:
                        MyControl.Audio.PlaySwitchSfx?.Invoke();
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
            else
            {
                MyControl.Audio.PlayClickSfx?.Invoke();
            }
        }

        public void PlayClick()
        {
            GetComponent<Button>().OnSubmit(default);
        }

        private void __OnPointerClick(PointerEventData eventData)
        {
            if (Interactable)
                PlayClick();
        }
    }
}
