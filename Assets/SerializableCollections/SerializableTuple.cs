using System;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
    [Serializable]
    public abstract class SerializableTuple<T1, T2> : IEquatable<SerializableTuple<T1, T2>>
    {
        [SerializeField]
        T1 item1;
        public T1 Item1 { get { return item1; } }

        [SerializeField]
        T2 item2;
        public T2 Item2 { get { return item2; } }

        public SerializableTuple()
        {

        }

        public SerializableTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public bool Equals(SerializableTuple<T1, T2> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;

            int h0;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            return h0;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", item1, item2);
        }
    }

    [Serializable]
    public abstract class SerializableTuple<T1, T2, T3> : IEquatable<SerializableTuple<T1, T2, T3>>
    {
        [SerializeField]
        T1 item1;
        public T1 Item1 { get { return item1; } }

        [SerializeField]
        T2 item2;
        public T2 Item2 { get { return item2; } }

        [SerializeField]
        T3 item3;
        public T3 Item3 { get { return item3; } }

        public SerializableTuple()
        {

        }

        public SerializableTuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        public bool Equals(SerializableTuple<T1, T2, T3> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2) &&
                comparer3.Equals(item3, other.item3);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;

            int h0;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h0 = (h0 << 5) + h0 ^ comparer3.GetHashCode(item3);
            return h0;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", item1, item2, item3);
        }
    }

    [Serializable]
    public abstract class SerializableTuple<T1, T2, T3, T4> : IEquatable<SerializableTuple<T1, T2, T3, T4>>
    {
        [SerializeField]
        T1 item1;
        public T1 Item1 { get { return item1; } }

        [SerializeField]
        T2 item2;
        public T2 Item2 { get { return item2; } }

        [SerializeField]
        T3 item3;
        public T3 Item3 { get { return item3; } }

        [SerializeField]
        T4 item4;
        public T4 Item4 { get { return item4; } }

        public SerializableTuple()
        {

        }

        public SerializableTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
        }

        public bool Equals(SerializableTuple<T1, T2, T3, T4> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2) &&
                comparer3.Equals(item3, other.item3) &&
                comparer4.Equals(item4, other.item4);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;

            int h0, h1;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", item1, item2, item3, item4);
        }
    }
}
