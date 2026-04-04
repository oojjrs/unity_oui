using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    public class MyCurrentGameObjectSelector : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
