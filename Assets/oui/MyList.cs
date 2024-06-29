using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.oui
{
    public partial class MyList : MonoBehaviour
    {
        public interface Master<TEntry, TValue> where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            public Data<TEntry, TValue> Data { get; }
            public TEntry Prefab { get; }
        }

        public interface PostscriptInterface<TEntry, TValue> where TEntry : MyListEntry<TValue>
        {
            void OnAdded(TEntry entry, TValue value);
        }

        public interface SorterInterface<TValue>
        {
            int GetSortingOrder(TValue value);
        }

        // -_- 유니티가 삭제를 제대로 못해서 땜빵겸 들고 있다. 2022.3.8f1부터 10f1까지.
        private List<GameObject> References { get; } = new();

        private void OnDestroy()
        {
            foreach (var r in References)
            {
                if (r != default)
                    Destroy(r);
            }
        }

        public void Clear<TEntry, TValue>(Master<TEntry, TValue> master) where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            master.Data.Clear();

            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (child != default)
                    Destroy(child.gameObject);
            }

            References.Clear();
        }

        public void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> entries) where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            UpdateEntries(master, entries, master as PostscriptInterface<TEntry, TValue>, master as SorterInterface<TValue>);
        }

        private void UpdateEntries<TEntry, TValue>(Master<TEntry, TValue> master, IEnumerable<TValue> entries, PostscriptInterface<TEntry, TValue> ps, SorterInterface<TValue> sorter) where TEntry : MonoBehaviour, MyListEntry<TValue>
        {
            master.Data.Update(entries);

            if (master.Data.Dirty)
            {
                foreach (var entry in master.Data.Removeds)
                {
                    // TODO : 이 가드가 왜 필요한지 이해는 1도 못했는데 없으니까 터져?
                    if (entry != default)
                    {
                        References.Remove(entry.gameObject);
                        Destroy(entry.gameObject);
                    }
                }

                foreach (var value in master.Data.Addeds)
                {
                    var entry = Instantiate(master.Prefab, transform);
                    References.Add(entry.gameObject);

                    if (sorter != default)
                        entry.SortingOrder = sorter.GetSortingOrder(value);

                    // 절대 예외가 발생하지 않아 프레임워크 레벨로 편입. 이제부터는 예외를 인정하지 않겠다는 얘기.
                    entry.Value = value;

                    ps?.OnAdded(entry, value);

                    master.Data.Set(entry, value);
                }

                if (sorter != default)
                {
                    var es = GetComponentsInChildren<TEntry>(true);
                    foreach (var e in es)
                        e.transform.SetParent(null);

                    foreach (var e in es.OrderBy(entry => entry.SortingOrder))
                        e.transform.SetParent(transform);
                }
            }
        }
    }
}
