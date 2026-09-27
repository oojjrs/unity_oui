using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class MyView : MonoBehaviour
    {
        public interface CallbackInterface
        {
            void OnViewChanged(GameObject instance);
        }

        private CallbackInterface[] _callbacks;
        [SerializeField]
        private GameObject _prefab;

        public GameObject Instance { get; private set; }
        public GameObject Prefab
        {
            get => _prefab;
            set
            {
                if (_prefab == value)
                    return;

                _prefab = value;

                ApplyPrefab();
            }
        }

        private void Awake()
        {
            _callbacks = GetComponents<CallbackInterface>();

            ApplyPrefab();
        }

        private void OnDestroy()
        {
            DestroyInstance();
        }

        private void ApplyPrefab()
        {
            DestroyInstance();

            if (_prefab != default)
                Instance = Instantiate(_prefab, transform);

            if (_callbacks != default)
            {
                foreach (var callback in _callbacks)
                    callback.OnViewChanged(Instance);
            }
        }

        private void DestroyInstance()
        {
            if (Instance == default)
                return;

            var instance = Instance;
            Instance = default;

            instance.SetActive(false);
            Destroy(instance);
        }

        public void OuiRecreate()
        {
            ApplyPrefab();
        }
    }
}
