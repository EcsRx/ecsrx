using System;
using System.Collections;
using System.Collections.Generic;
using EcsRx.MicroRx;
using EcsRx.MicroRx.Operators;
using EcsRx.MicroRx.Subjects;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.ReactiveData.Dictionaries
{
    [Serializable]
    public class ReactiveDictionary<TKey, TValue> : IReactiveDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
    {
        [NonSerialized]
        bool isDisposed = false;

        readonly Dictionary<TKey, TValue> inner;

        public ReactiveDictionary()
        {
            inner = new Dictionary<TKey, TValue>();
        }

        public ReactiveDictionary(IEqualityComparer<TKey> comparer)
        {
            inner = new Dictionary<TKey, TValue>(comparer);
        }

        public ReactiveDictionary(Dictionary<TKey, TValue> innerDictionary)
        {
            inner = innerDictionary;
        }

        public TValue this[TKey key]
        {
            get
            {
                return inner[key];
            }

            set
            {
                TValue oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    inner[key] = value;
                    if (dictionaryReplace != null) dictionaryReplace.OnNext(new DictionaryReplaceEvent<TKey, TValue>(key, oldValue, value));
                }
                else
                {
                    inner[key] = value;
                    if (dictionaryAdd != null) dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
            }
        }

        public int Count => inner.Count;
        public IEnumerable<TKey> Keys => inner.Keys;
        public IEnumerable<TValue> Values => inner.Values;

        public void Add(TKey key, TValue value)
        {
            inner.Add(key, value);

            if (dictionaryAdd != null) dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public void Clear()
        {
            var beforeCount = Count;
            inner.Clear();

            if (collectionReset != null) collectionReset.OnNext(Unit.Default);
            if (beforeCount <= 0) return;
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public bool Remove(TKey key)
        {
            TValue oldValue;
            if (!inner.TryGetValue(key, out oldValue)) return false;
            
            var isSuccessRemove = inner.Remove(key);
            if (!isSuccessRemove) return isSuccessRemove;
            
            if (dictionaryRemove != null) dictionaryRemove.OnNext(new DictionaryRemoveEvent<TKey, TValue>(key, oldValue));
            if (countChanged != null) countChanged.OnNext(Count);
            return isSuccessRemove;
        }

        public bool ContainsKey(TKey key)
        {
            return inner.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return inner.TryGetValue(key, out value);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject != null)
            {
                try
                {
                    subject.OnCompleted();
                }
                finally
                {
                    subject.Dispose();
                    subject = null;
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeSubject(ref countChanged);
                    DisposeSubject(ref collectionReset);
                    DisposeSubject(ref dictionaryAdd);
                    DisposeSubject(ref dictionaryRemove);
                    DisposeSubject(ref dictionaryReplace);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion


        #region Observe

        [NonSerialized]
        Subject<int> countChanged = null;
        public IObservable<int> ObserveCountChanged()
        {
            if (isDisposed) return ImmutableEmptyObservable<int>.Instance;
            return countChanged ?? (countChanged = new Subject<int>());
        }

        [NonSerialized]
        Subject<Unit> collectionReset = null;
        public IObservable<Unit> ObserveReset()
        {
            if (isDisposed) return ImmutableEmptyObservable<Unit>.Instance;
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        [NonSerialized]
        Subject<DictionaryAddEvent<TKey, TValue>> dictionaryAdd = null;
        public IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd()
        {
            if (isDisposed) return ImmutableEmptyObservable<DictionaryAddEvent<TKey, TValue>>.Instance;
            return dictionaryAdd ?? (dictionaryAdd = new Subject<DictionaryAddEvent<TKey, TValue>>());
        }

        [NonSerialized]
        Subject<DictionaryRemoveEvent<TKey, TValue>> dictionaryRemove = null;
        public IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove()
        {
            if (isDisposed) return ImmutableEmptyObservable<DictionaryRemoveEvent<TKey, TValue>>.Instance;
            return dictionaryRemove ?? (dictionaryRemove = new Subject<DictionaryRemoveEvent<TKey, TValue>>());
        }

        [NonSerialized]
        Subject<DictionaryReplaceEvent<TKey, TValue>> dictionaryReplace = null;
        public IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
        {
            if (isDisposed) return ImmutableEmptyObservable<DictionaryReplaceEvent<TKey, TValue>>.Instance;
            return dictionaryReplace ?? (dictionaryReplace = new Subject<DictionaryReplaceEvent<TKey, TValue>>());
        }

        #endregion

        #region implement explicit

        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }


        bool IDictionary.IsFixedSize => ((IDictionary)inner).IsFixedSize;

        bool IDictionary.IsReadOnly => ((IDictionary)inner).IsReadOnly;

        bool ICollection.IsSynchronized => ((IDictionary)inner).IsSynchronized;

        ICollection IDictionary.Keys => ((IDictionary)inner).Keys;

        object ICollection.SyncRoot => ((IDictionary)inner).SyncRoot;

        ICollection IDictionary.Values => ((IDictionary)inner).Values;


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)inner).IsReadOnly;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => inner.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => inner.Values;

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)inner).Contains(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)inner).CopyTo(array, index);
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add((TKey)item.Key, (TValue)item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)inner).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)inner).CopyTo(array, arrayIndex);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)inner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue v;
            if (TryGetValue(item.Key, out v))
            {
                if (EqualityComparer<TValue>.Default.Equals(v, item.Value))
                {
                    Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)inner).GetEnumerator();
        }

        #endregion
    }
}