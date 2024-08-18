using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace oojjrs.oui
{
    public partial class MyDropdown
    {
        [SerializeField]
        private RectTransform m_Template;

        [SerializeField]
        private Text m_CaptionText;

        [SerializeField]
        private Image m_CaptionImage;

        [Space]
        [SerializeField]
        private Text m_ItemText;

        [SerializeField]
        private Image m_ItemImage;

        [Space]
        [SerializeField]
        private int m_Value;

        [Space]
        [SerializeField]
        private List<OptionData> m_Options;

        [Space]
        [SerializeField]
        private DropdownEvent m_OnValueChanged = new();

        private GameObject m_Dropdown;

        private GameObject m_Blocker;

        private List<DropdownItem> m_Items = new();

        private bool validTemplate;

        private const int kHighSortingLayer = 30000;

        private static OptionData s_NoOptionData = new();

        public RectTransform template
        {
            get
            {
                return m_Template;
            }
            set
            {
                m_Template = value;
                RefreshShownValue();
            }
        }

        public Text captionText
        {
            get
            {
                return m_CaptionText;
            }
            set
            {
                m_CaptionText = value;
                RefreshShownValue();
            }
        }

        public Image captionImage
        {
            get
            {
                return m_CaptionImage;
            }
            set
            {
                m_CaptionImage = value;
                RefreshShownValue();
            }
        }

        public Text itemText
        {
            get
            {
                return m_ItemText;
            }
            set
            {
                m_ItemText = value;
                RefreshShownValue();
            }
        }

        public Image itemImage
        {
            get
            {
                return m_ItemImage;
            }
            set
            {
                m_ItemImage = value;
                RefreshShownValue();
            }
        }

        public List<OptionData> options
        {
            get
            {
                return m_Options;
            }
            set
            {
                m_Options = value;
                RefreshShownValue();
            }
        }

        public DropdownEvent onValueChanged
        {
            get
            {
                return m_OnValueChanged;
            }
            set
            {
                m_OnValueChanged = value;
            }
        }

        public int value
        {
            get
            {
                return m_Value;
            }
            set
            {
                Set(value);
                OnSelectItem(value);
            }
        }

        public void SetValueWithoutNotify(int input)
        {
            Set(input, sendCallback: false);
        }

        private void Set(int value, bool sendCallback = true)
        {
            if (!Application.isPlaying || (value != m_Value && options.Count != 0))
            {
                m_Value = Mathf.Clamp(value, 0, options.Count - 1);
                RefreshShownValue();
                if (sendCallback)
                {
                    UISystemProfilerApi.AddMarker("Dropdown.value", this);
                    m_OnValueChanged.Invoke(m_Value);
                }
            }
        }

        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                if ((bool)m_CaptionImage)
                {
                    m_CaptionImage.enabled = m_CaptionImage.sprite != null;
                }

                if ((bool)m_Template)
                {
                    m_Template.gameObject.SetActive(value: false);
                }
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (IsActive())
            {
                RefreshShownValue();
            }
        }

        protected override void OnDisable()
        {
            ImmediateDestroyDropdownList();
            if (m_Blocker != null)
            {
                DestroyBlocker(m_Blocker);
            }

            m_Blocker = null;
            base.OnDisable();
        }

        public void RefreshShownValue()
        {
            OptionData optionData = s_NoOptionData;
            if (options.Count > 0)
            {
                optionData = options[Mathf.Clamp(m_Value, 0, options.Count - 1)];
            }

            if ((bool)m_CaptionText)
            {
                if (optionData != null && optionData.text != null)
                {
                    m_CaptionText.text = optionData.text;
                }
                else
                {
                    m_CaptionText.text = "";
                }
            }

            if ((bool)m_CaptionImage)
            {
                if (optionData != null)
                {
                    m_CaptionImage.sprite = optionData.image;
                }
                else
                {
                    m_CaptionImage.sprite = null;
                }

                m_CaptionImage.enabled = m_CaptionImage.sprite != null;
            }
        }

        public void AddOptions(List<OptionData> options)
        {
            this.options.AddRange(options);
            RefreshShownValue();
        }

        public void AddOptions(List<string> options)
        {
            int count = options.Count;
            for (int i = 0; i < count; i++)
            {
                this.options.Add(new OptionData(options[i]));
            }

            RefreshShownValue();
        }

        public void AddOptions(List<Sprite> options)
        {
            int count = options.Count;
            for (int i = 0; i < count; i++)
            {
                this.options.Add(new OptionData(options[i]));
            }

            RefreshShownValue();
        }

        public void ClearOptions()
        {
            options.Clear();
            m_Value = 0;
            RefreshShownValue();
        }

        private void SetupTemplate(Canvas rootCanvas)
        {
            validTemplate = false;
            if (!m_Template)
            {
                Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Selectable component serving as the item.", this);
                return;
            }

            GameObject gameObject = m_Template.gameObject;
            gameObject.SetActive(value: true);
            var componentInChildren = m_Template.GetComponentInChildren<Selectable>();
            validTemplate = true;
            if (!componentInChildren || componentInChildren.transform == template)
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Selectable component serving as the item.", template);
            }
            else if (!(componentInChildren.transform.parent is RectTransform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The child GameObject with a Selectable component (the item) must have a RectTransform on its parent.", template);
            }
            else if (itemText != null && !itemText.transform.IsChildOf(componentInChildren.transform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", template);
            }
            else if (itemImage != null && !itemImage.transform.IsChildOf(componentInChildren.transform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", template);
            }

            if (!validTemplate)
            {
                gameObject.SetActive(value: false);
                return;
            }

            DropdownItem dropdownItem = componentInChildren.gameObject.AddComponent<DropdownItem>();
            dropdownItem.text = m_ItemText;
            dropdownItem.image = m_ItemImage;
            dropdownItem.selectable = componentInChildren;
            dropdownItem.rectTransform = (RectTransform)componentInChildren.transform;
            Canvas canvas = null;
            Transform parent = m_Template.parent;
            while (parent != null)
            {
                canvas = parent.GetComponent<Canvas>();
                if (canvas != null)
                {
                    break;
                }

                parent = parent.parent;
            }

            if (!gameObject.TryGetComponent<Canvas>(out var _))
            {
                Canvas canvas2 = gameObject.AddComponent<Canvas>();
                canvas2.overrideSorting = true;
                canvas2.sortingOrder = 30000;
                canvas2.sortingLayerID = rootCanvas.sortingLayerID;
            }

            if (canvas != null)
            {
                Component[] components = canvas.GetComponents<BaseRaycaster>();
                Component[] array = components;
                for (int i = 0; i < array.Length; i++)
                {
                    Type type = array[i].GetType();
                    if (gameObject.GetComponent(type) == null)
                    {
                        gameObject.AddComponent(type);
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>(gameObject);
            }

            GetOrAddComponent<CanvasGroup>(gameObject);
            gameObject.SetActive(value: false);
            validTemplate = true;
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T val = go.GetComponent<T>();
            if (!val)
            {
                val = go.AddComponent<T>();
            }

            return val;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Show();
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            Show();
        }

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            Hide();
        }

        public void Show()
        {
            if (!IsActive() || !IsInteractable() || m_Dropdown != null)
            {
                return;
            }

            List<Canvas> list = new();
            base.gameObject.GetComponentsInParent(includeInactive: false, list);
            if (list.Count == 0)
            {
                return;
            }

            int count = list.Count;
            Canvas canvas = list[count - 1];
            for (int i = 0; i < count; i++)
            {
                if (list[i].isRootCanvas || list[i].overrideSorting)
                {
                    canvas = list[i];
                    break;
                }
            }

            if (!validTemplate)
            {
                SetupTemplate(canvas);
                if (!validTemplate)
                {
                    return;
                }
            }

            m_Template.gameObject.SetActive(value: true);
            m_Dropdown = CreateDropdownList(m_Template.gameObject);
            m_Dropdown.name = "Dropdown List";
            m_Dropdown.SetActive(value: true);
            RectTransform rectTransform = m_Dropdown.transform as RectTransform;
            rectTransform.SetParent(m_Template.transform.parent, worldPositionStays: false);
            DropdownItem componentInChildren = m_Dropdown.GetComponentInChildren<DropdownItem>();
            RectTransform rectTransform2 = componentInChildren.rectTransform.parent.gameObject.transform as RectTransform;
            componentInChildren.rectTransform.gameObject.SetActive(value: true);
            Rect rect = rectTransform2.rect;
            Rect rect2 = componentInChildren.rectTransform.rect;
            Vector2 vector = rect2.min - rect.min + (Vector2)componentInChildren.rectTransform.localPosition;
            Vector2 vector2 = rect2.max - rect.max + (Vector2)componentInChildren.rectTransform.localPosition;
            Vector2 size = rect2.size;
            m_Items.Clear();
            Selectable selectable = null;
            int count2 = options.Count;
            for (int j = 0; j < count2; j++)
            {
                OptionData data = options[j];
                DropdownItem item = AddItem(data, componentInChildren, m_Items);
                if (!(item == null))
                {
                    if (value == j)
                    {
                        item.selectable.Select();
                        OnSelectItem(j);
                    }

                    if (selectable != null)
                    {
                        Navigation navigation = selectable.navigation;
                        Navigation navigation2 = item.selectable.navigation;
                        navigation.mode = Navigation.Mode.Explicit;
                        navigation2.mode = Navigation.Mode.Explicit;
                        navigation.selectOnDown = item.selectable;
                        navigation.selectOnRight = item.selectable;
                        navigation2.selectOnLeft = selectable;
                        navigation2.selectOnUp = selectable;
                        selectable.navigation = navigation;
                        item.selectable.navigation = navigation2;
                    }

                    selectable = item.selectable;
                }
            }

            Vector2 sizeDelta = rectTransform2.sizeDelta;
            sizeDelta.y = size.y * (float)m_Items.Count + vector.y - vector2.y;
            rectTransform2.sizeDelta = sizeDelta;
            float num = rectTransform.rect.height - rectTransform2.rect.height;
            if (num > 0f)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - num);
            }

            Vector3[] array = new Vector3[4];
            rectTransform.GetWorldCorners(array);
            RectTransform rectTransform3 = canvas.transform as RectTransform;
            Rect rect3 = rectTransform3.rect;
            for (int k = 0; k < 2; k++)
            {
                bool flag = false;
                for (int l = 0; l < 4; l++)
                {
                    Vector3 vector3 = rectTransform3.InverseTransformPoint(array[l]);
                    if ((vector3[k] < rect3.min[k] && !Mathf.Approximately(vector3[k], rect3.min[k])) || (vector3[k] > rect3.max[k] && !Mathf.Approximately(vector3[k], rect3.max[k])))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    RectTransformUtility.FlipLayoutOnAxis(rectTransform, k, keepPositioning: false, recursive: false);
                }
            }

            int count3 = m_Items.Count;
            for (int m = 0; m < count3; m++)
            {
                RectTransform rectTransform4 = m_Items[m].rectTransform;
                rectTransform4.anchorMin = new Vector2(rectTransform4.anchorMin.x, 0f);
                rectTransform4.anchorMax = new Vector2(rectTransform4.anchorMax.x, 0f);
                rectTransform4.anchoredPosition = new Vector2(rectTransform4.anchoredPosition.x, vector.y + size.y * (float)(count3 - 1 - m) + size.y * rectTransform4.pivot.y);
                rectTransform4.sizeDelta = new Vector2(rectTransform4.sizeDelta.x, size.y);
            }

            if ((bool)m_Dropdown)
            {
                m_Dropdown.GetComponent<CanvasGroup>().alpha = 1;
            }
            m_Template.gameObject.SetActive(value: false);
            componentInChildren.gameObject.SetActive(value: false);
            m_Blocker = CreateBlocker(canvas);
        }

        protected virtual GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject gameObject = new("Blocker")
            {
                layer = rootCanvas.gameObject.layer
            };
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent(rootCanvas.transform, worldPositionStays: false);
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            Canvas component = m_Dropdown.GetComponent<Canvas>();
            canvas.sortingLayerID = component.sortingLayerID;
            canvas.sortingOrder = component.sortingOrder - 1;
            Canvas canvas2 = null;
            Transform parent = m_Template.parent;
            while (parent != null)
            {
                if (parent.TryGetComponent<Canvas>(out canvas2))
                {
                    break;
                }

                parent = parent.parent;
            }

            if (canvas2 != null)
            {
                Component[] components = canvas2.GetComponents<BaseRaycaster>();
                Component[] array = components;
                for (int i = 0; i < array.Length; i++)
                {
                    Type type = array[i].GetType();
                    if (gameObject.GetComponent(type) == null)
                    {
                        gameObject.AddComponent(type);
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>(gameObject);
            }

            gameObject.AddComponent<Image>().color = Color.clear;
            gameObject.AddComponent<Button>().onClick.AddListener(Hide);
            gameObject.AddComponent<CanvasGroup>().ignoreParentGroups = true;
            return gameObject;
        }

        protected virtual void DestroyBlocker(GameObject blocker)
        {
            Destroy(blocker);
        }

        protected virtual GameObject CreateDropdownList(GameObject template)
        {
            return Instantiate(template);
        }

        protected virtual void DestroyDropdownList(GameObject dropdownList)
        {
            Destroy(dropdownList);
        }

        protected virtual DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            return Instantiate(itemTemplate);
        }

        protected virtual void DestroyItem(DropdownItem item)
        {
        }

        private DropdownItem AddItem(OptionData data, DropdownItem itemTemplate, List<DropdownItem> items)
        {
            DropdownItem dropdownItem = CreateItem(itemTemplate);
            dropdownItem.rectTransform.SetParent(itemTemplate.rectTransform.parent, worldPositionStays: false);
            dropdownItem.gameObject.SetActive(value: true);
            dropdownItem.gameObject.name = "Item " + items.Count + ((data.text != null) ? (": " + data.text) : "");

            if ((bool)dropdownItem.text)
            {
                dropdownItem.text.text = data.text;
            }

            if ((bool)dropdownItem.image)
            {
                dropdownItem.image.sprite = data.image;
                dropdownItem.image.enabled = dropdownItem.image.sprite != null;
            }

            items.Add(dropdownItem);
            return dropdownItem;
        }

        public void Hide()
        {
            if (m_Dropdown != null)
            {
                if ((bool)m_Dropdown)
                {
                    m_Dropdown.GetComponent<CanvasGroup>().alpha = 0;
                }
                if (IsActive())
                {
                    ImmediateDestroyDropdownList();
                }
            }

            if (m_Blocker != null)
            {
                DestroyBlocker(m_Blocker);
            }

            m_Blocker = null;
            Select();
        }

        private void ImmediateDestroyDropdownList()
        {
            int count = m_Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Items[i] != null)
                {
                    DestroyItem(m_Items[i]);
                }
            }

            m_Items.Clear();
            if (m_Dropdown != null)
            {
                DestroyDropdownList(m_Dropdown);
            }

            m_Dropdown = null;
        }

        private void OnSelectItem(int index)
        {
            var selectable = m_Items.ElementAtOrDefault(index);
            if (selectable != default)
                Hide();
        }
    }
}
