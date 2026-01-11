using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [RequireComponent(typeof(Selectable))]
    public class MySelectable : MonoBehaviour, IDeselectHandler, ISelectHandler
    {
        public interface CallbackInterface
        {
            void OnDeselect();
            void OnSelect();
        }

        private CallbackInterface[] Callbacks { get; set; }
        public bool Interactable
        {
            get => GetComponent<Selectable>().interactable;
            set => GetComponent<Selectable>().interactable = value;
        }

        private void Awake()
        {
            Callbacks = GetComponents<CallbackInterface>();
        }

        private void Start()
        {
            if (Callbacks?.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (Interactable && (Callbacks != default))
            {
                foreach (var callback in Callbacks)
                    callback.OnDeselect();
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (Interactable && (Callbacks != default))
            {
                foreach (var callback in Callbacks)
                    callback.OnSelect();
            }
        }
    }
}
