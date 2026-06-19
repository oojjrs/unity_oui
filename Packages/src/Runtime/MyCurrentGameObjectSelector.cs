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
            if ((_previous != default) && _previous.activeInHierarchy)
                EventSystem.current.SetSelectedGameObject(_previous);
        }

        private void OnEnable()
        {
            _previous = EventSystem.current.currentSelectedGameObject;

            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
