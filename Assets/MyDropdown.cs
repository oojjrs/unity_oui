using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Dropdown))]
    public class MyDropdown : MonoBehaviour
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
            get => GetComponent<Dropdown>().interactable;
            set
            {
                GetComponent<Dropdown>().interactable = value;

                if (_dimmedCover != default)
                    _dimmedCover.SetActive(value == false);
            }
        }

        private void Start()
        {
            Callbacks = GetComponents<CallbackInterface>();
            if (Callbacks == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");

            Initializer = GetComponent<InitializerInterface>();
        }

        public void OnValueChanged(int index)
        {
            MyControl.Audio.PlaySwitchSfx?.Invoke();

            if (Callbacks != default)
            {
                foreach (var callback in Callbacks)
                    callback.OnValueChanged(index, GetComponent<Dropdown>().options[index].text);
            }
        }

        public void OuiUpdateValue()
        {
            if (Initializer != default)
            {
                var dropdown = GetComponent<Dropdown>();
                dropdown.options = Initializer.InitialOptions.Select(t => new Dropdown.OptionData(t)).ToList();
                dropdown.SetValueWithoutNotify(Initializer.GetInitialIndex(dropdown.options.Select(t => t.text)));
            }
        }
    }
}
