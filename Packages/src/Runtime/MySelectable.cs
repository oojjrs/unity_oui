using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Selectable))]
    public class MySelectable : MonoBehaviour, IDeselectHandler, ISelectHandler
    {
        public interface CallbackInterface
        {
            void OnDeselect();
            void OnSelect();
        }

        private CallbackInterface[] Callbacks { get; set; }
        private bool IsSelected { get; set; }
        public bool Interactable
        {
            get => GetComponent<Selectable>().interactable;
            set => GetComponent<Selectable>().interactable = value;
        }

        private void Awake()
        {
            Callbacks = GetComponents<CallbackInterface>();
        }

        private void OnDisable()
        {
            if ((Application.isPlaying == false) || MyControl.IsQuitting)
                return;

            var eventSystem = EventSystem.current;
            if ((eventSystem != null) && (eventSystem.currentSelectedGameObject == gameObject))
                eventSystem.SetSelectedGameObject(null);

            Deselect();
        }

        private void Start()
        {
            if (Callbacks?.Length <= 0)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            Deselect();
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (Interactable && (IsSelected == false))
            {
                IsSelected = true;

                if (Callbacks != default)
                {
                    foreach (var callback in Callbacks)
                        callback.OnSelect();
                }
            }
        }

        private void Deselect()
        {
            if (IsSelected == false)
                return;

            IsSelected = false;

            if (Callbacks != default)
            {
                foreach (var callback in Callbacks)
                    callback.OnDeselect();
            }
        }
    }
}
