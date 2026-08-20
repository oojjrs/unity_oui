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
            void Clear();
            void Refresh(float offset, float viewportHeight);
        }

        private sealed class State<TEntry, TValue> : StateInterface where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            private readonly Dictionary<int, TEntry> _actives = new();
            private readonly Dictionary<TValue, float> _heightCache = new();
            private readonly List<float> _heights = new();
            private readonly Master<TEntry, TValue> _master;
            private readonly MyReel _owner;
            private readonly List<TEntry> _pool = new();
            private readonly List<float> _positions = new();
            private readonly PostscriptInterface<TEntry, TValue> _postscript;
            private readonly TEntry _prefab;
            private readonly List<int> _removeds = new();
            private readonly List<TValue> _values = new();
            private bool _hasNullHeight;
            private TEntry _measurementEntry;
            private GameObject _measurementRoot;
            private float _nullHeight;
            private int _version;
            private float _width = float.NaN;

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
                ++_version;

                foreach (var entry in _actives.Values)
                    DestroyEntry(entry);

                foreach (var entry in _pool)
                    DestroyEntry(entry);

                DestroyMeasurementRoot();
                _actives.Clear();
                _heightCache.Clear();
                _heights.Clear();
                _hasNullHeight = false;
                _nullHeight = 0;
                _pool.Clear();
                _positions.Clear();
                _positions.Add(0);
                _values.Clear();
                _width = float.NaN;
            }

            public void Refresh(float offset, float viewportHeight)
            {
                if (_values.Count > 0)
                    ValidateWidth(((RectTransform)_owner.transform).rect.width);

                var version = _version;
                var visibleStart = Mathf.Max(0, offset - _owner._overscan);
                var visibleEnd = offset + viewportHeight + _owner._overscan;
                var first = GetFirstVisibleIndex(visibleStart);
                var last = first;
                while ((last < _values.Count) && (_positions[last] < visibleEnd))
                    ++last;

                Recycle(first, last);

                for (int index = first; index < last; ++index)
                {
                    var isNew = _actives.TryGetValue(index, out var entry) == false;
                    if (isNew)
                    {
                        entry = GetEntry();
                        if (version != _version)
                        {
                            DestroyEntry(entry);
                            return;
                        }

                        _actives.Add(index, entry);
                    }

                    var rectTransform = GetRectTransform(entry);
                    ConfigureEntry(rectTransform, _positions[index], _heights[index]);

                    if (isNew)
                    {
                        entry.gameObject.SetActive(true);
                        if (version != _version)
                            return;

                        rectTransform.ForceUpdateRectTransforms();
                        if (TryResolveHeight(entry, _values[index], version, out var height) == false)
                            return;

                        if (Mathf.Approximately(height, _heights[index]) == false)
                            throw new InvalidOperationException("Equal MyReel values must resolve to the same height.");

                        ConfigureEntry(rectTransform, _positions[index], _heights[index]);
                    }
                }
            }

            public bool Update(Master<TEntry, TValue> master, IEnumerable<TValue> values, bool followsBottom)
            {
                if ((ReferenceEquals(_master, master) == false) || (ReferenceEquals(_prefab, master.Prefab) == false))
                    return false;

                var version = ++_version;
                if (_values.Count > 0)
                    ValidateWidth(((RectTransform)_owner.transform).rect.width);

                var updates = new List<TValue>();
                updates.AddRange(values);
                if (version != _version)
                    return true;

                if (updates.Count > 0)
                    ValidateWidth(((RectTransform)_owner.transform).rect.width);

                var updateHeightCache = new Dictionary<TValue, float>(_heightCache);
                var updateHasNullHeight = _hasNullHeight;
                var updateHeights = new List<float>(updates.Count);
                var updateNullHeight = _nullHeight;
                foreach (var value in updates)
                {
                    float height;
                    if (value is null)
                    {
                        if (updateHasNullHeight)
                        {
                            height = updateNullHeight;
                        }
                        else
                        {
                            if (TryMeasureHeight(value, version, out height) == false)
                                return true;

                            updateHasNullHeight = true;
                            updateNullHeight = height;
                        }
                    }
                    else if (updateHeightCache.TryGetValue(value, out height) == false)
                    {
                        if (TryMeasureHeight(value, version, out height) == false)
                            return true;

                        updateHeightCache.Add(value, height);
                    }

                    updateHeights.Add(height);
                }

                if (version == _version)
                    Commit(updates, updateHeights, followsBottom);

                return true;
            }

            private void CacheHeight(TValue value, float height)
            {
                if (value is null)
                {
                    if (_hasNullHeight && (Mathf.Approximately(_nullHeight, height) == false))
                        throw new InvalidOperationException("Equal MyReel values must resolve to the same height.");

                    _hasNullHeight = true;
                    _nullHeight = height;
                    return;
                }

                if (_heightCache.TryGetValue(value, out var cachedHeight))
                {
                    if (Mathf.Approximately(cachedHeight, height) == false)
                        throw new InvalidOperationException("Equal MyReel values must resolve to the same height.");

                    return;
                }

                _heightCache.Add(value, height);
            }

            private void Commit(List<TValue> updates, List<float> updateHeights, bool followsBottom)
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

                _values.Clear();
                _values.AddRange(updates);
                _heights.Clear();
                _heights.AddRange(updateHeights);
                RebuildHeightCache();
                RebuildPositions();

                if (followsBottom)
                    _owner.OuiScrollToBottom();
                else
                {
                    _owner.ClampOffset();
                    _owner.Refresh();
                }
            }

            private void ConfigureEntry(RectTransform rectTransform, float position, float height)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(0, -position);
                rectTransform.sizeDelta = new Vector2(0, height);
            }

            private bool TryCreateMeasurementRoot(int version)
            {
                var measurementRoot = new GameObject("MyReel Measurement", typeof(RectTransform), typeof(CanvasGroup), typeof(LayoutElement));
                try
                {
                    measurementRoot.transform.SetParent(_owner.transform, false);

                    var canvasGroup = measurementRoot.GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;

                    measurementRoot.GetComponent<LayoutElement>().ignoreLayout = true;

                    var rectTransform = (RectTransform)measurementRoot.transform;
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.ForceUpdateRectTransforms();

                    var measurementEntry = Instantiate(_prefab, measurementRoot.transform);
                    measurementEntry.gameObject.SetActive(false);
                    if (version != _version)
                    {
                        measurementRoot.SetActive(false);
                        Destroy(measurementRoot);
                        return false;
                    }

                    _measurementEntry = measurementEntry;
                    _measurementRoot = measurementRoot;
                    return true;
                }
                catch
                {
                    measurementRoot.SetActive(false);
                    Destroy(measurementRoot);
                    throw;
                }
            }

            private void DestroyEntry(TEntry entry)
            {
                if (entry != default)
                {
                    entry.gameObject.SetActive(false);
                    Destroy(entry.gameObject);
                }
            }

            private void DestroyMeasurementRoot()
            {
                if (_measurementRoot != default)
                {
                    _measurementRoot.SetActive(false);
                    Destroy(_measurementRoot);
                }

                _measurementRoot = default;
                _measurementEntry = default;
            }

            private TEntry GetEntry()
            {
                while (_pool.Count > 0)
                {
                    var index = _pool.Count - 1;
                    var entry = _pool[index];
                    _pool.RemoveAt(index);
                    if (entry != default)
                        return entry;
                }

                var instance = Instantiate(_prefab, _owner.transform);
                instance.gameObject.SetActive(false);
                return instance;
            }

            private float GetEstimatedHeight()
            {
                var rectTransform = _prefab.transform as RectTransform;
                if (rectTransform == default)
                    throw new InvalidOperationException($"{_prefab.name} must use RectTransform.");

                return Mathf.Max(1, rectTransform.rect.height);
            }

            private int GetFirstVisibleIndex(float position)
            {
                var min = 0;
                var max = _values.Count;
                while (min < max)
                {
                    var middle = (min + max) / 2;
                    if (_positions[middle + 1] <= position)
                        min = middle + 1;
                    else
                        max = middle;
                }

                return min;
            }

            private RectTransform GetRectTransform(TEntry entry)
            {
                var rectTransform = entry.transform as RectTransform;
                if (rectTransform == default)
                    throw new InvalidOperationException($"{entry.name} must use RectTransform.");

                return rectTransform;
            }

            private bool TryMeasureHeight(TValue value, int version, out float height)
            {
                height = 0;
                if (_measurementEntry == default)
                {
                    DestroyMeasurementRoot();
                    if (TryCreateMeasurementRoot(version) == false)
                        return false;
                }

                var entry = _measurementEntry;
                var rectTransform = GetRectTransform(entry);
                ConfigureEntry(rectTransform, 0, GetEstimatedHeight());
                entry.gameObject.SetActive(true);
                try
                {
                    if (version != _version)
                        return false;

                    rectTransform.ForceUpdateRectTransforms();
                    if (TryResolveHeight(entry, value, version, out height) == false)
                        return false;
                }
                finally
                {
                    if (entry != default)
                        entry.gameObject.SetActive(false);
                }

                ValidateWidth(rectTransform.rect.width);
                return true;
            }

            private void RebuildHeightCache()
            {
                _heightCache.Clear();
                _hasNullHeight = false;
                _nullHeight = 0;

                for (int index = 0; index < _values.Count; ++index)
                    CacheHeight(_values[index], _heights[index]);
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

            private bool TryResolveHeight(TEntry entry, TValue value, int version, out float height)
            {
                height = 0;
                entry.Value = value;
                if (version != _version)
                    return false;

                _postscript?.OnAdded(entry, value);
                if (version != _version)
                    return false;

                entry.ResolveSize();
                if (version != _version)
                    return false;

                height = GetRectTransform(entry).rect.height;
                if (height <= 0)
                    throw new InvalidOperationException($"{entry.name} must resolve to a positive height.");

                return true;
            }

            private void ValidateWidth(float width)
            {
                if (width <= 0)
                    throw new InvalidOperationException($"{_owner.name} must resolve to a positive width before MyReel measures entries.");

                if (float.IsNaN(_width))
                {
                    _width = width;
                    return;
                }

                if (Mathf.Approximately(_width, width) == false)
                    throw new InvalidOperationException($"{_owner.name} width changed after MyReel measured its entries. Call Clear() and add the values again.");
            }
        }

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

            if ((Mathf.Approximately(rectTransform.anchorMin.y, 1) == false) || (Mathf.Approximately(rectTransform.anchorMax.y, 1) == false) || (Mathf.Approximately(rectTransform.pivot.y, 1) == false))
                throw new InvalidOperationException($"{name} must use top vertical anchors and pivot.");

            if (_viewport == default)
            {
                if (_scrollRect != default)
                {
                    if (_scrollRect.viewport != default)
                        _viewport = _scrollRect.viewport;
                    else
                        _viewport = (RectTransform)_scrollRect.transform;
                }
                else
                {
                    _viewport = transform.parent as RectTransform;
                }
            }

            if (_viewport == default)
                throw new InvalidOperationException($"{name} must resolve to a viewport RectTransform.");

            if ((_scrollRect != default) && (_scrollRect.verticalScrollbar != default) && (_scrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport))
                throw new InvalidOperationException($"{name} requires a vertical scrollbar that does not change the viewport width.");

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

        public void Clear()
        {
            CurrentState?.Clear();
            CurrentState = default;

            if (_scrollRect != default)
                _scrollRect.StopMovement();

            ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            SetOffset(0);
        }

        private void ClampOffset()
        {
            if (_viewport == default)
                return;

            var rectTransform = (RectTransform)transform;
            var offset = Mathf.Clamp(Offset, 0, Mathf.Max(0, rectTransform.rect.height - _viewport.rect.height));
            if (Mathf.Approximately(Offset, offset) && Mathf.Approximately(rectTransform.anchoredPosition.y, offset))
                return;

            if (_scrollRect != default)
                _scrollRect.StopMovement();

            SetOffset(offset);
        }

        private bool IsBottom()
        {
            if (_viewport == default)
                return true;

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
            if (_viewport == default)
                return;

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
            if ((CurrentState == default) || (_viewport == default))
                return;

            CurrentState.Refresh(Offset, _viewport.rect.height);
        }

        private void SetOffset(float offset)
        {
            Offset = Mathf.Max(0, offset);

            var rectTransform = (RectTransform)transform;
            var anchoredPosition = rectTransform.anchoredPosition;
            if (Mathf.Approximately(anchoredPosition.y, Offset))
                return;

            anchoredPosition.y = Offset;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> values) where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            var followsBottom = (_scroll == ScrollEnum.AlwaysBottom) || ((_scroll == ScrollEnum.FollowBottom) && IsBottom());
            var state = CurrentState as State<TEntry, TValue>;
            if ((state != default) && state.Update(master, values, followsBottom))
                return;

            CurrentState?.Clear();
            state = new State<TEntry, TValue>(this, master);
            CurrentState = state;
            _ = state.Update(master, values, followsBottom);
        }
    }
}
