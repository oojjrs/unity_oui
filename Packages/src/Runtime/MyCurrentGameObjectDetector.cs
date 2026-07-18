using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    public class MyCurrentGameObjectDetector : MonoBehaviour
    {
        public interface CallbackInterface
        {
            void Update(GameObject previousGameObject, GameObject currentGameObject);
        }

        private CallbackInterface _callback;
        private GameObject _currentGameObject;
        [SerializeField]
        private bool _debugLog;

        private void Awake()
        {
            _callback = GetComponent<CallbackInterface>();
        }

        private void Start()
        {
            if (_callback == null)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        private void Update()
        {
            var eventSystem = EventSystem.current;
            var currentGameObject = (eventSystem != null) ? eventSystem.currentSelectedGameObject : null;
            if (currentGameObject == _currentGameObject)
                return;

            var previousGameObject = _currentGameObject;
            _currentGameObject = currentGameObject;

            if (_debugLog)
            {
                var previousName = (previousGameObject != null) ? previousGameObject.name : "None";
                var currentName = (currentGameObject != null) ? currentGameObject.name : "None";
                Debug.Log($"{name}> CURRENT GAMEOBJECT : {previousName} -> {currentName}.", this);
            }

            _callback?.Update(previousGameObject, currentGameObject);
        }
    }
}
