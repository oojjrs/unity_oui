using oojjrs.oui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets
{
    public class MyButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private MyTooltip _tooltip;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _tooltip.Open((RectTransform)transform, "툴팁을 테스트하는데 이 정도 길이로 만족할 순 없고 좀 더 긴 길이를 해봐야 나중에 원망하는 일이 줄어들지 않겠읍니까? 근데 이 정도 길이로도 완벽한 테스트가 어려워서 좀 더 길게 해봄", 400);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _tooltip.Close();
        }
    }
}
