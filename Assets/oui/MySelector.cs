using System.Linq;
using UnityEngine;

namespace Assets.oui
{
    public class MySelector : MonoBehaviour
    {
        public interface CallbackInterface
        {
            int GetIndex();
        }

        [SerializeField]
        private GameObject[] _values;

        private CallbackInterface Callback { get; set; }
        private int? Index { get; set; }

        private void OnEnable()
        {
            UpdateSelection();
        }

        private void Start()
        {
            MyControl.Selector.OnUpdate += UpdateSelection;

            Callback = GetComponent<CallbackInterface>();
            if (Callback == default)
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        public int GetIndexByName(string name)
        {
            return _values.TakeWhile(t => t.name != name).Count();
        }

        public void UpdateSelection()
        {
            if (Callback != default)
            {
                var index = Callback.GetIndex();
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
            else
            {
                foreach (var value in _values)
                {
                    if (value != default)
                        value.SetActive(false);
                }
            }
        }
    }
}
