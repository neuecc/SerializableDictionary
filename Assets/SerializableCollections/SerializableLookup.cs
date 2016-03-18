using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace SerializableCollections
{
    // Mutable(Buildable) Serializable Lookup. Does not support remove.

    [Serializable]
    public class SerializableLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        // can't serialize internal generic struct.
        // extract to 4 arrays.

        //struct Entry
        //{
        //    public int hashCode;
        //    public int next;
        //    public TKey key;
        //    public TValue value;
        //}

        // serializable state.
        [SerializeField, HideInInspector]
        int[] buckets; // link index of first entry. empty is -1.
        [SerializeField, HideInInspector]
        int count;

        // Entry[] entries;
        [SerializeField, HideInInspector]
        int[] entriesHashCode;
        [SerializeField, HideInInspector]
        int[] entriesNext;
        [SerializeField, HideInInspector]
        TKey[] entriesKey;
        [SerializeField, HideInInspector]
        TElement[] entriesValue;

        int version; // version does not serialize

        // equality comparer is not serializable, use specified comparer.
        public virtual IEqualityComparer<TKey> Comparer
        {
            get
            {
                return EqualityComparer<TKey>.Default;
            }
        }

        protected SerializableLookup()
        {
            Initialize(0);
        }

        protected SerializableLookup(int initialCapacity)
        {
            Initialize(initialCapacity);
        }

        SerializableLookup(int staticCapacity, bool forceSize)
        {
            Initialize(staticCapacity, forceSize);
        }

        public int Count
        {
            get { return count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                return new LookupCollection(this, key);
            }
        }

        public void Add(TKey key, TElement value)
        {
            Insert(key, value);
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                Array.Clear(entriesHashCode, 0, count);
                Array.Clear(entriesKey, 0, count);
                Array.Clear(entriesNext, 0, count);
                Array.Clear(entriesValue, 0, count);

                count = 0;
                version++;
            }
        }

        public bool Contains(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        int FindEntry(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (buckets != null)
            {
                int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entriesNext[i])
                {
                    if (entriesHashCode[i] == hashCode && Comparer.Equals(entriesKey[i], key)) return i;
                }
            }

            return -1;
        }

        IEnumerable<int> FindEntries(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (buckets != null)
            {
                int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entriesNext[i])
                {
                    if (entriesHashCode[i] == hashCode && Comparer.Equals(entriesKey[i], key))
                    {
                        yield return i;
                    }
                }
            }
        }

        void Initialize(int capacity)
        {
            Initialize(capacity, false);
        }

        void Initialize(int capacity, bool forceSize)
        {
            int size = forceSize ? capacity : HashHelpers.GetPrime(capacity);
            buckets = new int[size];
            entriesHashCode = new int[size];
            entriesKey = new TKey[size];
            entriesNext = new int[size];
            entriesValue = new TElement[size];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
                entriesNext[i] = -1;
            }
        }

        void Insert(TKey key, TElement value) // add only.
        {
            if (key == null) throw new ArgumentNullException("key");
            if (buckets == null || buckets.Length == 0) Initialize(0);

            int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;

            int index;
            if (count == entriesHashCode.Length)
            {
                Resize();
                targetBucket = hashCode % buckets.Length;
            }
            index = count;
            count++;

            entriesHashCode[index] = hashCode;
            entriesKey[index] = key;
            entriesValue[index] = value;
            entriesNext[index] = buckets[targetBucket];
            buckets[targetBucket] = index;
            version++;
        }

        void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;

            var newEntriesKey = new TKey[newSize];
            var newEntriesValue = new TElement[newSize];
            var newEntriesHashCode = new int[newSize];
            var newEntriesNext = new int[newSize];
            Array.Copy(entriesKey, 0, newEntriesKey, 0, count);
            Array.Copy(entriesValue, 0, newEntriesValue, 0, count);
            Array.Copy(entriesHashCode, 0, newEntriesHashCode, 0, count);
            Array.Copy(entriesNext, 0, newEntriesNext, 0, count);

            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntriesHashCode[i] != -1)
                    {
                        newEntriesHashCode[i] = (Comparer.GetHashCode(newEntriesKey[i]) & 0x7FFFFFFF);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (newEntriesHashCode[i] >= 0)
                {
                    int bucket = newEntriesHashCode[i] % newSize;
                    newEntriesNext[i] = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            buckets = newBuckets;

            entriesKey = newEntriesKey;
            entriesValue = newEntriesValue;
            entriesHashCode = newEntriesHashCode;
            entriesNext = newEntriesNext;
        }

        public bool TryGetValue(TKey key, out TElement value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entriesValue[i];
                return true;
            }
            value = default(TElement);
            return false;
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var key in entriesKey.Distinct(Comparer))
            {
                yield return new Grouping(this, key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void TrimExcess()
        {
            var newLookup = new SerializableLookup<TKey, TElement>(Count, true);
            foreach (var g in this)
            {
                foreach (var item in g)
                {
                    newLookup.Add(g.Key, item);
                }
            }

            // copy internal field to this
            this.buckets = newLookup.buckets;
            this.count = newLookup.count;
            this.entriesHashCode = newLookup.entriesHashCode;
            this.entriesKey = newLookup.entriesKey;
            this.entriesNext = newLookup.entriesNext;
            this.entriesValue = newLookup.entriesValue;
        }

        class Grouping : IGrouping<TKey, TElement>
        {
            readonly SerializableLookup<TKey, TElement> lookup;
            public TKey Key { get; private set; }

            public Grouping(SerializableLookup<TKey, TElement> lookup, TKey key)
            {
                this.lookup = lookup;
                this.Key = key;
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                return lookup[Key].GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class LookupCollection : ICollection<TElement>
        {
            readonly SerializableLookup<TKey, TElement> lookup;
            readonly int version;
            readonly List<int> indexes;

            public LookupCollection(SerializableLookup<TKey, TElement> lookup, TKey key)
            {
                var indexes = new List<int>();
                foreach (var i in lookup.FindEntries(key))
                {
                    indexes.Add(i);
                }
                indexes.Reverse();

                this.version = lookup.version;
                this.indexes = indexes;
                this.lookup = lookup;
            }

            public int Count
            {
                get
                {
                    return indexes.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public void Add(TElement item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TElement item)
            {
                var comparer = EqualityComparer<TElement>.Default;
                using (var e = this.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        if (comparer.Equals(e.Current, item)) return true;
                    }
                }
                return false;
            }

            public void CopyTo(TElement[] array, int arrayIndex)
            {
                if (version != lookup.version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                for (int i = arrayIndex; i < indexes.Count; i++)
                {
                    array[i] = lookup.entriesValue[indexes[i]];
                }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    if (version != lookup.version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    yield return lookup.entriesValue[indexes[i]];
                }
            }

            public bool Remove(TElement item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        static class SR
        {
            public const string InvalidOperation_EnumFailedVersion = "InvalidOperation_EnumFailedVersion";
            public const string InvalidOperation_EnumOpCantHappen = "InvalidOperation_EnumOpCantHappen";
            public const string ArgumentOutOfRange_Index = "ArgumentOutOfRange_Index";
            public const string Argument_InvalidArrayType = "Argument_InvalidArrayType";
            public const string NotSupported_ValueCollectionSet = "NotSupported_ValueCollectionSet";
            public const string Arg_RankMultiDimNotSupported = "Arg_RankMultiDimNotSupported";
            public const string Arg_ArrayPlusOffTooSmall = "Arg_ArrayPlusOffTooSmall";
            public const string Arg_NonZeroLowerBound = "Arg_NonZeroLowerBound";
            public const string NotSupported_KeyCollectionSet = "NotSupported_KeyCollectionSet";
            public const string Arg_WrongType = "Arg_WrongType";
            public const string ArgumentOutOfRange_NeedNonNegNum = "ArgumentOutOfRange_NeedNonNegNum";
            public const string Arg_HTCapacityOverflow = "Arg_HTCapacityOverflow";
            public const string Argument_AddingDuplicate = "Argument_AddingDuplicate";

            public static string Format(string f, params object[] args)
            {
                return string.Format(f, args);
            }
        }

        static class HashHelpers
        {
            // Table of prime numbers to use as hash table sizes. 
            // A typical resize algorithm would pick the smallest prime number in this array
            // that is larger than twice the previous capacity. 
            // Suppose our Hashtable currently has capacity x and enough elements are added 
            // such that a resize needs to occur. Resizing first computes 2x then finds the 
            // first prime in the table greater than 2x, i.e. if primes are ordered 
            // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
            // Doubling is important for preserving the asymptotic complexity of the 
            // hashtable operations such as add.  Having a prime guarantees that double 
            // hashing does not lead to infinite loops.  IE, your hash function will be 
            // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
            public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, 8639249, 10367101,
            12440537, 14928671, 17914409, 21497293, 25796759, 30956117, 37147349, 44576837, 53492207, 64190669,
            77028803, 92434613, 110921543, 133105859, 159727031, 191672443, 230006941, 276008387, 331210079,
            397452101, 476942527, 572331049, 686797261, 824156741, 988988137, 1186785773, 1424142949, 1708971541,
            2050765853, MaxPrimeArrayLength };

            public static int GetPrime(int min)
            {
                if (min < 0)
                    throw new ArgumentException(SR.Arg_HTCapacityOverflow);

                for (int i = 0; i < primes.Length; i++)
                {
                    int prime = primes[i];
                    if (prime >= min) return prime;
                }

                return min;
            }

            public static int GetMinPrime()
            {
                return primes[0];
            }

            // Returns size of hashtable to grow to.
            public static int ExpandPrime(int oldSize)
            {
                int newSize = 2 * oldSize;

                // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                {
                    return MaxPrimeArrayLength;
                }

                return GetPrime(newSize);
            }


            // This is the maximum prime smaller than Array.MaxArrayLength
            public const int MaxPrimeArrayLength = 0x7FEFFFFD;
        }
    }
}