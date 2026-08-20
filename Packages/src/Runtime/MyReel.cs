using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class MyReel : MonoBehaviour
    {
        public enum ScrollEnum
        {
            None,
            AlwaysBottom,
            FollowBottom,
        }

        public interface SizeResolverInterface
        {
            RectTransform RectTransform { get; }

            void ResolveSize();
        }

        public interface EntryInterface<TValue> : MyListEntry<TValue>, SizeResolverInterface
        {
        }

        public interface Master<TEntry, TValue> where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            TEntry Prefab { get; }
        }

        public interface PostscriptInterface<TEntry, TValue> where TEntry : EntryInterface<TValue>
        {
            void OnAdded(TEntry entry, TValue value);
        }

        private interface StateInterface
        {
            int Count { get; }

            void Clear();
            void Refresh(float offset, float viewportHeight);
        }

        private sealed class State<TEntry, TValue> : StateInterface where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            private readonly Dictionary<int, TEntry> _actives = new();
            private readonly List<float> _heights = new();
            private readonly Master<TEntry, TValue> _master;
            private readonly MyReel _owner;
            private readonly List<TEntry> _pool = new();
            private readonly List<float> _positions = new();
            private readonly PostscriptInterface<TEntry, TValue> _postscript;
            private readonly TEntry _prefab;
            private readonly List<int> _removeds = new();
            private readonly List<TValue> _values = new();
            private TEntry _measurementEntry;

            public int Count => _values.Count;

            public State(MyReel owner, Master<TEntry, TValue> master)
            {
                _master = master;
                _owner = owner;
                _postscript = master as PostscriptInterface<TEntry, TValue>;
                _prefab = master.Prefab;
                if (_prefab == default)
                    throw new ArgumentNullException(nameof(master), "MyReel master must provide a prefab.");

                _positions.Add(0);
            }

            public void Clear()
            {
                foreach (var entry in _actives.Values)
                    DestroyEntry(entry);

                foreach (var entry in _pool)
                    DestroyEntry(entry);

                DestroyEntry(_measurementEntry);
                _actives.Clear();
                _heights.Clear();
                _measurementEntry = default;
                _pool.Clear();
                _positions.Clear();
                _positions.Add(0);
                _values.Clear();
            }

            public void Refresh(float offset, float viewportHeight)
            {
                var visibleStart = Mathf.Max(0, offset - _owner._overscan);
                var visibleEnd = offset + viewportHeight + _owner._overscan;
                var first = GetFirstVisibleIndex(visibleStart);
                var last = first;
                while ((last < _values.Count) && (_positions[last] < visibleEnd))
                    ++last;

                Recycle(first, last);

                for (var index = first; index < last; ++index)
                {
                    if (_actives.TryGetValue(index, out var entry) == false)
                    {
                        entry = GetEntry();
                        ConfigureEntry(entry.RectTransform, _positions[index], _heights[index]);
                        Bind(entry, _values[index]);
                        _actives.Add(index, entry);
                    }

                    ConfigureEntry(entry.RectTransform, _positions[index], _heights[index]);
                }
            }

            public bool Update(Master<TEntry, TValue> master, IEnumerable<TValue> values)
            {
                if ((ReferenceEquals(_master, master) == false) || (ReferenceEquals(_prefab, master.Prefab) == false))
                    return false;

                var updates = new List<TValue>();
                updates.AddRange(values);
                var heights = new List<float>(updates.Count);

                for (var index = 0; index < updates.Count; ++index)
                {
                    if ((index < _values.Count) && EqualityComparer<TValue>.Default.Equals(_values[index], updates[index]))
                        heights.Add(_heights[index]);
                    else
                        heights.Add(Measure(updates[index]));
                }

                RecycleChanged(updates);
                _values.Clear();
                _values.AddRange(updates);
                _heights.Clear();
                _heights.AddRange(heights);
                RebuildPositions();
                return true;
            }

            private void Bind(TEntry entry, TValue value)
            {
                entry.gameObject.SetActive(true);
                entry.RectTransform.ForceUpdateRectTransforms();
                entry.Value = value;
                _postscript?.OnAdded(entry, value);
                ResolveSize(entry);
            }

            private static void ConfigureEntry(RectTransform rectTransform, float position, float height)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(0, -position);
                rectTransform.sizeDelta = new Vector2(0, height);
            }

            private static void DestroyEntry(TEntry entry)
            {
                if (entry != default)
                {
                    entry.gameObject.SetActive(false);
                    Destroy(entry.gameObject);
                }
            }

            private TEntry GetEntry()
            {
                if (_pool.Count > 0)
                {
                    var index = _pool.Count - 1;
                    var entry = _pool[index];
                    _pool.RemoveAt(index);
                    return entry;
                }

                var instance = Instantiate(_prefab, _owner.transform);
                instance.gameObject.SetActive(false);
                return instance;
            }

            private int GetFirstVisibleIndex(float position)
            {
                var minimum = 0;
                var maximum = _values.Count;
                while (minimum < maximum)
                {
                    var middle = (minimum + maximum) / 2;
                    if (_positions[middle + 1] <= position)
                        minimum = middle + 1;
                    else
                        maximum = middle;
                }

                return minimum;
            }

            private float Measure(TValue value)
            {
                if (_measurementEntry == default)
                {
                    _measurementEntry = Instantiate(_prefab, _owner.transform);
                    _measurementEntry.gameObject.SetActive(false);
                }

                var entry = _measurementEntry;
                ConfigureEntry(entry.RectTransform, 0, Mathf.Max(1, _prefab.RectTransform.rect.height));
                entry.gameObject.SetActive(true);
                entry.RectTransform.ForceUpdateRectTransforms();
                entry.Value = value;
                _postscript?.OnAdded(entry, value);
                ResolveSize(entry);
                var height = entry.RectTransform.rect.height;
                ConfigureEntry(entry.RectTransform, 0, height);
                entry.RectTransform.ForceUpdateRectTransforms();
                ResolveSize(entry);
                height = entry.RectTransform.rect.height;
                entry.gameObject.SetActive(false);

                if (height <= 0)
                    throw new InvalidOperationException($"{entry.name} must resolve to a positive height.");

                return height;
            }

            private void RebuildPositions()
            {
                _positions.Clear();
                _positions.Add(0);

                foreach (var height in _heights)
                    _positions.Add(_positions[_positions.Count - 1] + height + _owner._spacing);

                if (_values.Count > 0)
                    _positions[_positions.Count - 1] -= _owner._spacing;

                ((RectTransform)_owner.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _positions[_positions.Count - 1]);
            }

            private void Recycle(int first, int last)
            {
                _removeds.Clear();
                foreach (var pair in _actives)
                {
                    if ((pair.Key < first) || (pair.Key >= last))
                        _removeds.Add(pair.Key);
                }

                foreach (var index in _removeds)
                {
                    var entry = _actives[index];
                    _actives.Remove(index);
                    entry.gameObject.SetActive(false);
                    _pool.Add(entry);
                }
            }

            private void RecycleChanged(List<TValue> updates)
            {
                _removeds.Clear();
                foreach (var pair in _actives)
                {
                    if ((pair.Key >= updates.Count) || (EqualityComparer<TValue>.Default.Equals(_values[pair.Key], updates[pair.Key]) == false))
                        _removeds.Add(pair.Key);
                }

                foreach (var index in _removeds)
                {
                    var entry = _actives[index];
                    _actives.Remove(index);
                    entry.gameObject.SetActive(false);
                    _pool.Add(entry);
                }
            }
        }

        [SerializeField]
        private MyText _emptyText;
        [SerializeField]
        [Min(0)]
        private float _overscan;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private ScrollEnum _scroll;
        [SerializeField]
        [Min(0)]
        private float _spacing;
        [SerializeField]
        private RectTransform _viewport;

        private StateInterface CurrentState { get; set; }
        private float Offset { get; set; }
        private Vector2 ViewportSize { get; set; }

        private void Awake()
        {
            var rectTransform = (RectTransform)transform;
            if (_scrollRect == default)
                _scrollRect = GetComponentInParent<ScrollRect>();

            if ((_scrollRect != default) && (_scrollRect.content != rectTransform))
                throw new InvalidOperationException($"{name} must be the content RectTransform of its ScrollRect.");

            if (_viewport == default)
            {
                if ((_scrollRect != default) && (_scrollRect.viewport != default))
                    _viewport = _scrollRect.viewport;
                else if (_scrollRect != default)
                    _viewport = (RectTransform)_scrollRect.transform;
                else
                    _viewport = transform.parent as RectTransform;
            }

            if (_viewport == default)
                throw new InvalidOperationException($"{name} must resolve to a viewport RectTransform.");

            ViewportSize = _viewport.rect.size;
        }

        private void LateUpdate()
        {
            var viewportSize = _viewport.rect.size;
            if (viewportSize == ViewportSize)
                return;

            ViewportSize = viewportSize;
            Refresh();
        }

        private void OnDestroy()
        {
            CurrentState?.Clear();
        }

        private void OnDisable()
        {
            if (_scrollRect != default)
                _scrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        private void OnEnable()
        {
            if (_scrollRect != default)
                _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void OnScroll(Vector2 _)
        {
            Offset = Mathf.Max(0, ((RectTransform)transform).anchoredPosition.y);
            Refresh();
        }

        private static void ResolveSize(SizeResolverInterface resolver)
        {
            var rectTransform = resolver.RectTransform;
            var hasChild = false;
            var minimum = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var maximum = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            var corners = new Vector3[4];

            for (var index = 0; index < rectTransform.childCount; ++index)
            {
                var childResolver = rectTransform.GetChild(index).GetComponent<SizeResolverInterface>();
                if (childResolver == default)
                    continue;

                hasChild = true;
                ResolveSize(childResolver);
                childResolver.RectTransform.GetWorldCorners(corners);

                foreach (var corner in corners)
                {
                    var point = (Vector2)rectTransform.InverseTransformPoint(corner);
                    minimum = Vector2.Min(minimum, point);
                    maximum = Vector2.Max(maximum, point);
                }
            }

            if (hasChild == false)
            {
                resolver.ResolveSize();
                return;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetRequiredSize(minimum.x, maximum.x, rectTransform.pivot.x));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GetRequiredSize(minimum.y, maximum.y, rectTransform.pivot.y));

            static float GetRequiredSize(float minimum, float maximum, float pivot)
            {
                if ((pivot <= 0) && (minimum < 0))
                    throw new InvalidOperationException("A size resolver cannot contain a child behind its pivot without moving itself.");

                if ((pivot >= 1) && (maximum > 0))
                    throw new InvalidOperationException("A size resolver cannot contain a child behind its pivot without moving itself.");

                var negativeSize = pivot > 0 ? -minimum / pivot : 0;
                var positiveSize = pivot < 1 ? maximum / (1 - pivot) : 0;
                return Mathf.Max(0, negativeSize, positiveSize);
            }
        }

        public void Clear()
        {
            CurrentState?.Clear();
            CurrentState = default;

            if (_scrollRect != default)
                _scrollRect.StopMovement();

            ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            SetOffset(0);

            if (_emptyText != default)
                _emptyText.gameObject.SetActive(true);
        }

        private void ClampOffset()
        {
            var rectTransform = (RectTransform)transform;
            SetOffset(Mathf.Clamp(Offset, 0, Mathf.Max(0, rectTransform.rect.height - _viewport.rect.height)));
        }

        private bool IsBottom()
        {
            var rectTransform = (RectTransform)transform;
            return Offset >= (rectTransform.rect.height - _viewport.rect.height - 1);
        }

        public void OuiScroll(float offset)
        {
            SetOffset(offset);
            Refresh();
        }

        public void OuiScrollToBottom()
        {
            var rectTransform = (RectTransform)transform;
            var offset = Mathf.Max(0, rectTransform.rect.height - _viewport.rect.height);

            if (_scrollRect != default)
            {
                _scrollRect.StopMovement();
                _scrollRect.verticalNormalizedPosition = 0;
            }

            SetOffset(offset);
            Refresh();
        }

        public void Refresh()
        {
            CurrentState?.Refresh(Offset, _viewport.rect.height);
        }

        private void SetOffset(float offset)
        {
            Offset = Mathf.Max(0, offset);

            var rectTransform = (RectTransform)transform;
            var anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.y = Offset;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> values) where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            var followsBottom = (_scroll == ScrollEnum.AlwaysBottom) || ((_scroll == ScrollEnum.FollowBottom) && IsBottom());
            var state = CurrentState as State<TEntry, TValue>;
            if ((state == default) || (state.Update(master, values) == false))
            {
                CurrentState?.Clear();
                state = new State<TEntry, TValue>(this, master);
                CurrentState = state;
                _ = state.Update(master, values);
            }

            if (followsBottom)
                OuiScrollToBottom();
            else
            {
                ClampOffset();
                Refresh();
            }

            if (_emptyText != default)
                _emptyText.gameObject.SetActive(state.Count <= 0);
        }
    }
}
