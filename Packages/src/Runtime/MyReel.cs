using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    public class MyReel : MonoBehaviour
    {
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
            Data<TEntry, TValue> Data { get; }
            TEntry Prefab { get; }
        }

        public interface PostscriptInterface<TEntry, TValue> where TEntry : EntryInterface<TValue>
        {
            void OnAdded(TEntry entry, TValue value);
        }

        public class Data<TEntry, TValue> where TEntry : EntryInterface<TValue>
        {
            public enum ManagementEnum
            {
                Counting,
                RefreshAlways,
                UniqueEntry,
            }

            private readonly List<TValue> _addeds;
            private readonly int _maxEntryCount;
            private readonly List<TEntry> _removeds;
            private readonly ManagementEnum _type;
            private readonly List<(TValue, TEntry)> _valuesA;
            private readonly Dictionary<TValue, TEntry> _valuesB;

            public IEnumerable<TValue> Addeds => _addeds;
            public bool Dirty => _addeds.Any() || _removeds.Any();
            public IEnumerable<TEntry> Removeds => _removeds;

            public Data()
                : this(ManagementEnum.UniqueEntry, -1)
            {
            }

            public Data(ManagementEnum type)
                : this(type, -1)
            {
            }

            public Data(int maxEntryCount)
                : this(ManagementEnum.Counting, maxEntryCount)
            {
            }

            public Data(ManagementEnum type, int maxEntryCount)
            {
                _addeds = new();
                _maxEntryCount = maxEntryCount;
                _removeds = new();
                _type = type;
                _valuesA = new();
                _valuesB = new();
            }

            public void Clear()
            {
                _addeds.Clear();
                _removeds.Clear();
                _valuesA.Clear();
                _valuesB.Clear();
            }

            public void Set(TEntry entry, TValue value)
            {
                switch (_type)
                {
                    case ManagementEnum.Counting:
                        _valuesA.Add((value, entry));
                        break;
                    case ManagementEnum.RefreshAlways:
                        _valuesA.Add((value, entry));
                        break;
                    case ManagementEnum.UniqueEntry:
                        _valuesB[value] = entry;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public void Update(IEnumerable<TValue> values)
            {
                switch (_type)
                {
                    case ManagementEnum.Counting:
                        {
                            _addeds.Clear();
                            _addeds.AddRange(values);
                            _removeds.Clear();

                            if (_valuesA.Count + _addeds.Count - _removeds.Count > _maxEntryCount)
                            {
                                _removeds.AddRange(_valuesA.Take(_valuesA.Count + _addeds.Count - _removeds.Count - _maxEntryCount).Select(tuple => tuple.Item2));
                                _valuesA.RemoveRange(0, _valuesA.Count + _addeds.Count - _removeds.Count - _maxEntryCount);
                            }
                        }
                        break;
                    case ManagementEnum.RefreshAlways:
                        {
                            _addeds.Clear();
                            _addeds.AddRange(values);
                            _removeds.Clear();
                            _removeds.AddRange(_valuesA.Select(tuple => tuple.Item2));
                            _valuesA.Clear();
                        }
                        break;
                    case ManagementEnum.UniqueEntry:
                        {
                            _addeds.Clear();
                            _addeds.AddRange(values.Except(_valuesB.Keys));

                            var removeds = _valuesB.Where(pair => values.Contains(pair.Key) == false);
                            if (removeds.Any())
                            {
                                var pairs = removeds.ToArray();
                                _removeds.Clear();
                                _removeds.AddRange(pairs.Select(pair => pair.Value));

                                foreach (var pair in pairs)
                                    _valuesB.Remove(pair.Key);
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        [SerializeField]
        private MyText _emptyText;
        private readonly List<GameObject> _references = new();

        private void OnDestroy()
        {
            foreach (var reference in _references)
            {
                if (reference != default)
                    Destroy(reference);
            }
        }

        public void Clear<TEntry, TValue>(Master<TEntry, TValue> master) where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            master.Data.Clear();

            for (var index = 0; index < transform.childCount; ++index)
            {
                var child = transform.GetChild(index);
                if (child != default)
                    Destroy(child.gameObject);
            }

            _references.Clear();
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

        public void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> values) where TEntry : MonoBehaviour, EntryInterface<TValue>
        {
            master.Data.Update(values);

            if (master.Data.Dirty)
            {
                foreach (var entry in master.Data.Removeds)
                {
                    if (entry != default)
                    {
                        _references.Remove(entry.gameObject);
                        Destroy(entry.gameObject);
                    }
                }

                var postscript = master as PostscriptInterface<TEntry, TValue>;
                foreach (var value in master.Data.Addeds)
                {
                    var entry = Instantiate(master.Prefab, transform);
                    _references.Add(entry.gameObject);

                    entry.Value = value;
                    postscript?.OnAdded(entry, value);
                    master.Data.Set(entry, value);
                    ResolveSize(entry);
                }
            }

            if (_emptyText != default)
                _emptyText.gameObject.SetActive(_references.Count <= 0);
        }
    }
}
