using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(RectTransform))]
    public partial class MyDropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
    {
        public interface CallbackInterface
        {
            void OnValueChanged(int index, string option);
        }

        public interface InitializerInterface
        {
            IEnumerable<string> InitialOptions { get; }

            int GetInitialIndex(IEnumerable<string> options);
        }

        [SerializeField]
        private GameObject _dimmedCover;

        private CallbackInterface[] Callbacks { get; set; }
        private InitializerInterface Initializer { get; set; }
        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;

                if (_dimmedCover != default)
                    _dimmedCover.SetActive(value == false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OuiUpdateValue();
        }

        protected override void Start()
        {
            // 아래 두 줄은 유니티 오리지널
            base.Start();
            RefreshShownValue();

            Callbacks = GetComponents<CallbackInterface>();
            if (Callbacks == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            Initializer = GetComponent<InitializerInterface>();

            OuiUpdateValue();
        }

        public void OnValueChanged(int index)
        {
            MyControl.Audio.PlaySwitchSfx?.Invoke();

            if (Callbacks != default)
            {
                foreach (var callback in Callbacks)
                    callback.OnValueChanged(index, options[index].text);
            }
        }

        private void OuiUpdateValue()
        {
            if (Initializer != default)
            {
                options = Initializer.InitialOptions.Select(t => new OptionData(t)).ToList();
                SetValueWithoutNotify(Initializer.GetInitialIndex(options.Select(t => t.text)));
            }
        }
    }
}
