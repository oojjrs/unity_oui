using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.oojjrs.oui
{
    public class MyEventUpwardsDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            SendMessageUpwards("__OnBeginDrag", eventData, SendMessageOptions.DontRequireReceiver);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            SendMessageUpwards("__OnDrag", eventData, SendMessageOptions.DontRequireReceiver);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            SendMessageUpwards("__OnEndDrag", eventData, SendMessageOptions.DontRequireReceiver);
        }
    }
}
