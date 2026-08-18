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

        public interface Master<TEntry, TValue> where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            TEntry Prefab { get; }
        }

        public interface PostscriptInterface<TEntry, TValue> where TEntry : MyListEntry<TValue>
        {
            void OnShown(TEntry entry, TValue value);
        }

        private interface StateInterface
        {
            void Clear();
            void Refresh(float offset, float viewportHeight);
        }

        private sealed class State<TEntry, TValue> : StateInterface where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            private readonly Dictionary<int, TEntry> _actives = new();
            private readonly List<float> _heights = new();
            private readonly Master<TEntry, TValue> _master;
            private readonly MyReel _owner;
            private readonly List<TEntry> _pool = new();
            private readonly List<float> _positions = new();
            private readonly PostscriptInterface<TEntry, TValue> _postscript;
            private readonly List<int> _removeds = new();
            private readonly List<TValue> _updates = new();
            private readonly List<TValue> _values = new();

            public State(MyReel owner, Master<TEntry, TValue> master, IEnumerable<TValue> values)
            {
                _master = master;
                _owner = owner;
                _postscript = master as PostscriptInterface<TEntry, TValue>;
                _values.AddRange(values);
                ResetHeights();
                RebuildPositions();
            }

            public void Clear()
            {
                foreach (var entry in _actives.Values)
                {
                    if (entry != default)
                        Destroy(entry.gameObject);
                }

                foreach (var entry in _pool)
                {
                    if (entry != default)
                        Destroy(entry.gameObject);
                }

                _actives.Clear();
                _pool.Clear();
            }

            public void Refresh(float offset, float viewportHeight)
            {
                for (int pass = 0; pass < 4; ++pass)
                {
                    var visibleStart = Mathf.Max(0, offset - _owner._overscan);
                    var visibleEnd = offset + viewportHeight + _owner._overscan;
                    var first = GetFirstVisibleIndex(visibleStart);
                    var last = first;
                    while ((last < _values.Count) && (_positions[last] < visibleEnd))
                        ++last;

                    Recycle(first, last);

                    var changed = false;
                    for (int index = first; index < last; ++index)
                    {
                        var isNew = _actives.TryGetValue(index, out var entry) == false;
                        if (isNew)
                        {
                            entry = GetEntry();
                            _actives.Add(index, entry);
                        }

                        var rectTransform = entry.transform as RectTransform;
                        if (rectTransform == default)
                            throw new InvalidOperationException($"{entry.name} must use RectTransform.");

                        rectTransform.anchorMin = new Vector2(0, 1);
                        rectTransform.anchorMax = new Vector2(1, 1);
                        rectTransform.pivot = new Vector2(0.5f, 1);
                        rectTransform.anchoredPosition = new Vector2(0, -_positions[index]);

                        if (isNew)
                        {
                            rectTransform.sizeDelta = new Vector2(0, _heights[index]);
                            entry.Value = _values[index];
                            _postscript?.OnShown(entry, _values[index]);
                        }

                        var height = rectTransform.sizeDelta.y;
                        if (height <= 0)
                            throw new InvalidOperationException($"{entry.name} must set a positive sizeDelta.y while assigning Value.");

                        if (Mathf.Approximately(_heights[index], height) == false)
                        {
                            _heights[index] = height;
                            changed = true;
                        }
                    }

                    if (changed == false)
                        break;

                    RebuildPositions();
                }
            }

            public bool Update(Master<TEntry, TValue> master, IEnumerable<TValue> values)
            {
                if (ReferenceEquals(_master, master) == false)
                    return false;

                _updates.Clear();
                _updates.AddRange(values);

                _removeds.Clear();
                foreach (var pair in _actives)
                {
                    if ((pair.Key >= _updates.Count) || (EqualityComparer<TValue>.Default.Equals(_values[pair.Key], _updates[pair.Key]) == false))
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
                _values.AddRange(_updates);
                ResetHeights();
                RebuildPositions();
                return true;
            }

            private TEntry GetEntry()
            {
                TEntry entry;
                if (_pool.Count > 0)
                {
                    var index = _pool.Count - 1;
                    entry = _pool[index];
                    _pool.RemoveAt(index);
                    entry.gameObject.SetActive(true);
                }
                else
                {
                    entry = Instantiate(_master.Prefab, _owner.transform);
                }

                return entry;
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

            private void ResetHeights()
            {
                var prefabRectTransform = _master.Prefab.transform as RectTransform;
                if (prefabRectTransform == default)
                    throw new InvalidOperationException($"{_master.Prefab.name} must use RectTransform.");

                var height = prefabRectTransform.sizeDelta.y;
                if (height <= 0)
                    throw new InvalidOperationException($"{_master.Prefab.name} prefab height must be positive.");

                _heights.Clear();
                foreach (var value in _values)
                    _heights.Add(height);
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

        private void Awake()
        {
            if (_scrollRect == default)
                _scrollRect = GetComponentInParent<ScrollRect>();

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

        private void OnScroll(Vector2 value)
        {
            OuiScroll(((RectTransform)transform).anchoredPosition.y);
        }

        public void Clear()
        {
            CurrentState?.Clear();
            CurrentState = default;
            Offset = 0;

            ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
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
            Offset = Mathf.Max(0, offset);
            Refresh();
        }

        public void OuiScrollToBottom()
        {
            if (_viewport == default)
                return;

            var rectTransform = (RectTransform)transform;
            Offset = Mathf.Max(0, rectTransform.rect.height - _viewport.rect.height);

            if (_scrollRect != default)
            {
                _scrollRect.StopMovement();
                _scrollRect.verticalNormalizedPosition = 0;
            }

            Refresh();
        }

        public void Refresh()
        {
            if ((CurrentState == default) || (_viewport == default))
                return;

            CurrentState.Refresh(Offset, _viewport.rect.height);
        }

        public void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> values) where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            var followsBottom = (_scroll == ScrollEnum.AlwaysBottom) || ((_scroll == ScrollEnum.FollowBottom) && IsBottom());

            if (((CurrentState is State<TEntry, TValue> state) && state.Update(master, values)) == false)
            {
                CurrentState?.Clear();
                CurrentState = new State<TEntry, TValue>(this, master, values);
            }

            Refresh();

            if (followsBottom)
                OuiScrollToBottom();
        }
    }
}
