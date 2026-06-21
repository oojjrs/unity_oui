using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    public class MyCurrentGameObjectSelector : MonoBehaviour
    {
        private GameObject _previous;

        private void OnDisable()
        {
            if ((Application.isPlaying == false) || MyControl.IsQuitting)
                return;

            var eventSystem = EventSystem.current;
            if ((eventSystem != null) && (_previous != default) && _previous.activeInHierarchy)
                eventSystem.SetSelectedGameObject(_previous);
        }

        private void OnEnable()
        {
            if ((Application.isPlaying == false) || MyControl.IsQuitting)
                return;

            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return;

            _previous = eventSystem.currentSelectedGameObject;

            eventSystem.SetSelectedGameObject(gameObject);
        }
    }
}
