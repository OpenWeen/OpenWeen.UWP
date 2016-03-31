using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenWeen.UWP.Common.Extension
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
            => source.Distinct(new EqualityComparer<T, V>(keySelector));
    }

    public class EqualityComparer<T, V> : IEqualityComparer<T>
    {
        private Func<T, V> keySelector;

        public EqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        public bool Equals(T x, T y) => EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));

        public int GetHashCode(T obj) => EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
    }
}