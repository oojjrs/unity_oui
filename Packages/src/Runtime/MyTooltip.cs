using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class MyTooltip : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float _targetSpacing = 10;
        [SerializeField]
        private MyText _text;

        public MyText Text => _text;

        private void Awake()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = false;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void FitInCanvas(RectTransform rectTransform, RectTransform canvasRectTransform)
        {
            var tooltipRect = GetRectInCanvas(rectTransform, canvasRectTransform);
            var canvasRect = canvasRectTransform.rect;
            var offset = Vector2.zero;
            if (tooltipRect.width > canvasRect.width)
                offset.x = canvasRect.center.x - tooltipRect.center.x;
            else if (tooltipRect.xMin < canvasRect.xMin)
                offset.x = canvasRect.xMin - tooltipRect.xMin;
            else if (tooltipRect.xMax > canvasRect.xMax)
                offset.x = canvasRect.xMax - tooltipRect.xMax;

            if (tooltipRect.height > canvasRect.height)
                offset.y = canvasRect.center.y - tooltipRect.center.y;
            else if (tooltipRect.yMin < canvasRect.yMin)
                offset.y = canvasRect.yMin - tooltipRect.yMin;
            else if (tooltipRect.yMax > canvasRect.yMax)
                offset.y = canvasRect.yMax - tooltipRect.yMax;

            rectTransform.position += canvasRectTransform.TransformVector(offset);
        }

        private Rect GetRectInCanvas(RectTransform rectTransform, RectTransform canvasRectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var minimum = (Vector2)canvasRectTransform.InverseTransformPoint(corners[0]);
            var maximum = minimum;
            for (var index = 1; index < corners.Length; ++index)
            {
                var point = (Vector2)canvasRectTransform.InverseTransformPoint(corners[index]);
                minimum = Vector2.Min(minimum, point);
                maximum = Vector2.Max(maximum, point);
            }

            return Rect.MinMaxRect(minimum.x, minimum.y, maximum.x, maximum.y);
        }

        public void Open(RectTransform target, string text, float width)
        {
            if (target == null)
                throw new System.ArgumentNullException(nameof(target));
            if (_text == null)
                throw new System.InvalidOperationException($"{name}> MyText is not assigned.");
            if (width <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than zero.");

            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                throw new System.InvalidOperationException($"{name}> A parent Canvas is required.");

            var targetCanvas = target.GetComponentInParent<Canvas>();
            if ((targetCanvas == null) || (targetCanvas.rootCanvas != canvas.rootCanvas))
                throw new System.InvalidOperationException($"{name}> Target and tooltip must share the same root Canvas.");

            gameObject.SetActive(true);

            var rectTransform = (RectTransform)transform;
            var textRectTransform = (RectTransform)_text.transform;
            rectTransform.ForceUpdateRectTransforms();
            textRectTransform.ForceUpdateRectTransforms();
            var heightPadding = rectTransform.rect.height - textRectTransform.rect.height;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.ForceUpdateRectTransforms();
            textRectTransform.ForceUpdateRectTransforms();

            _text.Text = text;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0, _text.PreferredHeight + heightPadding));
            rectTransform.ForceUpdateRectTransforms();

            var canvasRectTransform = (RectTransform)canvas.rootCanvas.transform;
            target.ForceUpdateRectTransforms();
            PlaceOutsideTarget(rectTransform, target, canvasRectTransform);
            FitInCanvas(rectTransform, canvasRectTransform);
        }

        private void PlaceOutsideTarget(RectTransform rectTransform, RectTransform target, RectTransform canvasRectTransform)
        {
            var canvasRect = canvasRectTransform.rect;
            var targetRect = GetRectInCanvas(target, canvasRectTransform);
            var tooltipRect = GetRectInCanvas(rectTransform, canvasRectTransform);
            var spacing = Mathf.Max(0, _targetSpacing);
            var spaceAbove = canvasRect.yMax - targetRect.yMax - spacing;
            var spaceBelow = targetRect.yMin - canvasRect.yMin - spacing;

            bool placeAbove;
            if (tooltipRect.height <= spaceAbove)
                placeAbove = true;
            else if (tooltipRect.height <= spaceBelow)
                placeAbove = false;
            else
                placeAbove = spaceAbove >= spaceBelow;

            var offset = new Vector2(targetRect.center.x - tooltipRect.center.x, 0);
            if (placeAbove)
                offset.y = targetRect.yMax + spacing - tooltipRect.yMin;
            else
                offset.y = targetRect.yMin - spacing - tooltipRect.yMax;

            rectTransform.position += canvasRectTransform.TransformVector(offset);
            rectTransform.ForceUpdateRectTransforms();
        }
    }
}
