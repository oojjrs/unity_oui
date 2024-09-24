using UnityEngine;

namespace oojjrs.oui
{
    public class MySelector : MonoBehaviour
    {
        public interface CallbackInterface
        {
            int GetIndex();
        }

        [SerializeField]
        private bool _emptyCallbackInterface;
        [SerializeField]
        private GameObject[] _values;

        private CallbackInterface Callback { get; set; }
        private int? Index { get; set; }

        private void Awake()
        {
            Callback = GetComponent<CallbackInterface>();
        }

        private void Start()
        {
            if (_emptyCallbackInterface == false)
            {
                if (Callback == default)
                    Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
            }
        }

        public void OuiSelect(int index)
        {
            if (index != Index)
            {
                Index = index;

                foreach (var value in _values)
                {
                    if (value != default)
                        value.SetActive(false);
                }

                if ((Index >= 0) && (Index < _values.Length))
                {
                    if (_values[Index.Value] != default)
                        _values[Index.Value].SetActive(true);
                }
            }
        }

        public void OuiUpdate()
        {
            if (Callback != default)
                OuiSelect(Callback.GetIndex());
            else if (_emptyCallbackInterface == false)
                OuiSelect(-1);
        }
    }
}
