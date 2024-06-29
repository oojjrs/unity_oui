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
            IEnumerable<string> InitialOptions { get; }

            int GetInitialIndex(IEnumerable<string> options);
            void OnValueChanged(int index, string option);
        }

        [SerializeField]
        private GameObject _dimmedCover;

        private CallbackInterface Callback { get; set; }
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
            MyControl.Dropdown.OnUpdate += UpdateValue;

            Callback = GetComponent<CallbackInterface>();
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        public void OnValueChanged(int index)
        {
            MyControl.Audio.PlaySwitchSfx?.Invoke();

            Callback?.OnValueChanged(index, GetComponent<Dropdown>().options[index].text);
        }

        private void UpdateValue()
        {
            if (Callback != default)
            {
                var dropdown = GetComponent<Dropdown>();
                dropdown.options = Callback.InitialOptions.Select(t => new Dropdown.OptionData(t)).ToList();
                dropdown.SetValueWithoutNotify(Callback.GetInitialIndex(dropdown.options.Select(t => t.text)));
            }
        }
    }
}
