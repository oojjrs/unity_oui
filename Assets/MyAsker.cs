using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyAsker : MonoBehaviour
    {
        public interface OkInterface
        {
            void OnOk();
        }

        public interface YesNoInterface
        {
            void OnNo();
            void OnYes();
        }

        private OkInterface[] _oks;
        private YesNoInterface[] _yesNos;

        private void Awake()
        {
            _oks = GetComponentsInChildren<OkInterface>();
            _yesNos = GetComponentsInChildren<YesNoInterface>();
        }

        private void Start()
        {
            if ((_oks?.Length <= 0) && (_yesNos?.Length <= 0))
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
        }

        internal void OnNo()
        {
            if (_yesNos != default)
            {
                foreach (var c in _yesNos)
                    c.OnNo();
            }
        }

        internal void OnOk()
        {
            if (_oks != default)
            {
                foreach (var c in _oks)
                    c.OnOk();
            }
        }

        internal void OnYes()
        {
            if (_yesNos != default)
            {
                foreach (var c in _yesNos)
                    c.OnYes();
            }
        }
    }
}
