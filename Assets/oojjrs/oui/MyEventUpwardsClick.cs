using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.oojjrs.oui
{
    // ㅡ,.ㅡ 클릭 이벤트를 받으려면 down과 up이 있어야 하네?? 아니 근데 없어도 되는 놈들은 또 뭔데
    public class MyEventUpwardsClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            SendMessageUpwards("__OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SendMessageUpwards("__OnPointerDown", eventData, SendMessageOptions.DontRequireReceiver);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            SendMessageUpwards("__OnPointerUp", eventData, SendMessageOptions.DontRequireReceiver);
        }
    }
}
