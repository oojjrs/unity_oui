using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MyCurrentGameObjectDetector))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
    public sealed class MyScrollRect : MonoBehaviour, MyCurrentGameObjectDetector.CallbackInterface, UnityEngine.UI.ICanvasElement
    {
        private RectTransform _focusTransform;
        private UnityEngine.UI.ScrollRect _scrollRect;

        private void Awake()
        {
            _scrollRect = GetComponent<UnityEngine.UI.ScrollRect>();
            if (_scrollRect.content == default)
                throw new InvalidOperationException($"{name} requires ScrollRect content.");
        }

        private void OnDisable()
        {
            UnityEngine.UI.CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            _focusTransform = default;
        }

        private void OnEnable()
        {
            OuiFocus();
        }

        void MyCurrentGameObjectDetector.CallbackInterface.Update(GameObject previousGameObject, GameObject currentGameObject)
        {
            QueueFocus(currentGameObject);
        }

        void UnityEngine.UI.ICanvasElement.GraphicUpdateComplete()
        {
        }

        bool UnityEngine.UI.ICanvasElement.IsDestroyed()
        {
            return this == null;
        }

        void UnityEngine.UI.ICanvasElement.LayoutComplete()
        {
        }

        void UnityEngine.UI.ICanvasElement.Rebuild(UnityEngine.UI.CanvasUpdate executing)
        {
            if (executing != UnityEngine.UI.CanvasUpdate.PostLayout)
                return;

            var focusTransform = _focusTransform;
            _focusTransform = default;

            if (focusTransform != default)
                Focus(focusTransform);
        }

        private void Focus(RectTransform focusTransform)
        {
            var content = _scrollRect.content;
            if ((focusTransform == content) || (focusTransform.IsChildOf(content) == false))
                return;

            var viewport = _scrollRect.viewport != default ? _scrollRect.viewport : (RectTransform)_scrollRect.transform;
            var focusBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, focusTransform);
            var viewportRect = viewport.rect;
            var offset = Vector2.zero;

            if (_scrollRect.horizontal)
            {
                if (focusBounds.min.x < viewportRect.xMin)
                    offset.x = viewportRect.xMin - focusBounds.min.x;
                else if (focusBounds.max.x > viewportRect.xMax)
                    offset.x = viewportRect.xMax - focusBounds.max.x;
            }

            if (_scrollRect.vertical)
            {
                if (focusBounds.max.y > viewportRect.yMax)
                    offset.y = viewportRect.yMax - focusBounds.max.y;
                else if (focusBounds.min.y < viewportRect.yMin)
                    offset.y = viewportRect.yMin - focusBounds.min.y;
            }

            if (offset == Vector2.zero)
                return;

            _scrollRect.StopMovement();
            var contentOffset = content.parent.InverseTransformVector(viewport.TransformVector(offset));
            content.anchoredPosition += (Vector2)contentOffset;
        }

        public void OuiFocus()
        {
            var eventSystem = EventSystem.current;
            QueueFocus(eventSystem != default ? eventSystem.currentSelectedGameObject : default);
        }

        private void QueueFocus(GameObject focusGameObject)
        {
            _focusTransform = focusGameObject != default ? focusGameObject.transform as RectTransform : default;
            if (_focusTransform != default)
                UnityEngine.UI.CanvasUpdateRegistry.TryRegisterCanvasElementForLayoutRebuild(this);
        }
    }
}
