using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace tetryds.Reumpr.Collections
{
    public class ConcurrentHash<T> : ICollection<T>
    {
        private readonly ConcurrentDictionary<T, object> dict;

        public ConcurrentHash()
        {
            dict = new ConcurrentDictionary<T, object>();
        }

        public ConcurrentHash(IEqualityComparer<T> comparer)
        {
            dict = new ConcurrentDictionary<T, object>(comparer);
        }

        public int Count => dict.Count;

        public bool TryAdd(T item) => dict.TryAdd(item, null);
        public void Clear() => dict.Clear();
        public bool Contains(T item) => dict.ContainsKey(item);
        public bool Remove(T item) => dict.TryRemove(item, out _);

        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.Add(T item) => TryAdd(item);
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => dict.Keys.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
