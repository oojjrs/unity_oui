using System.Collections.Generic;
using System.Linq;

namespace Assets.oui
{
    public partial class MyList
    {
        public class Data<TEntry, TValue> where TEntry : MyListEntry<TValue>
        {
            public enum ManagementEnum
            {
                // 데이터 최대 개수로만 제어한다.
                Counting,
                // 항상 모든 데이터를 갱신한다.
                RefreshAlways,
                // 기본값. 중복되지 않는 데이터 목록을 갖는다.
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
                        throw new System.NotImplementedException();
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
                                _removeds.AddRange(_valuesA.Take(_valuesA.Count + _addeds.Count - _removeds.Count - _maxEntryCount).Select(t => t.Item2));

                                _valuesA.RemoveRange(0, _valuesA.Count + _addeds.Count - _removeds.Count - _maxEntryCount);
                            }
                        }
                        break;
                    case ManagementEnum.RefreshAlways:
                        {
                            _addeds.Clear();
                            _addeds.AddRange(values);

                            _removeds.Clear();
                            _removeds.AddRange(_valuesA.Select(t => t.Item2));

                            _valuesA.Clear();
                        }
                        break;
                    case ManagementEnum.UniqueEntry:
                        {
                            _addeds.Clear();
                            _addeds.AddRange(values.Except(_valuesB.Keys));

                            {
                                var ret = _valuesB.Where(kvp => values.Contains(kvp.Key) == false);
                                if (ret.Any())
                                {
                                    var kvps = ret.ToArray();
                                    _removeds.Clear();
                                    _removeds.AddRange(kvps.Select(kvp => kvp.Value));

                                    foreach (var kvp in kvps)
                                        _valuesB.Remove(kvp.Key);
                                }
                            }
                        }
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }
    }
}
