using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    public class MySelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public interface CallbackInterface
        {
            int GetIndex();
        }

        public interface HoverInterface
        {
            void OnHoverEnter();
            void OnHoverExit();
        }

        [SerializeField]
        private bool _emptyCallbackInterface;
        [SerializeField]
        private GameObject[] _values;

        private CallbackInterface Callback { get; set; }
        private HoverInterface[] Hovers { get; set; }
        private int? Index { get; set; }

        private void Awake()
        {
            Callback = GetComponent<CallbackInterface>();
            Hovers = GetComponents<HoverInterface>();
        }

        private void Start()
        {
            if (_emptyCallbackInterface == false)
            {
                if (Callback == default)
                    Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Hovers != default)
            {
                foreach (var hover in Hovers)
                    hover.OnHoverEnter();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (Hovers != default)
            {
                foreach (var hover in Hovers)
                    hover.OnHoverExit();
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
