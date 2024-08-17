using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    public partial class MyDropdown
    {
        protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, ICancelHandler
        {
            [SerializeField]
            private Text m_Text;

            [SerializeField]
            private Image m_Image;

            [SerializeField]
            private RectTransform m_RectTransform;

            [SerializeField]
            private Selectable m_Selectable;

            public Text text
            {
                get
                {
                    return m_Text;
                }
                set
                {
                    m_Text = value;
                }
            }

            public Image image
            {
                get
                {
                    return m_Image;
                }
                set
                {
                    m_Image = value;
                }
            }

            public RectTransform rectTransform
            {
                get
                {
                    return m_RectTransform;
                }
                set
                {
                    m_RectTransform = value;
                }
            }

            public Selectable selectable
            {
                get
                {
                    return m_Selectable;
                }
                set
                {
                    m_Selectable = value;
                }
            }

            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject);
            }

            public virtual void OnCancel(BaseEventData eventData)
            {
                Dropdown componentInParent = GetComponentInParent<Dropdown>();
                if ((bool)componentInParent)
                {
                    componentInParent.Hide();
                }
            }
        }
    }
}
