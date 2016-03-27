using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Extension methods for Enumerable classes
    /// </summary>
    public static class EnumerableEx
    {
        public static SortedList<TKey, TSource> ToSortedList<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            SortedList<TKey, TSource> list = new SortedList<TKey, TSource>();
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), item);
            }
            return list;
        }

        public static SortedList<TKey, TElement> ToSortedList<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            SortedList<TKey, TElement> list = new SortedList<TKey, TElement>();
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), elementSelector(item));
            }
            return list;
        }

        public static SortedList<TKey, TSource> ToSortedList<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            SortedList<TKey, TSource> list = new SortedList<TKey, TSource>(comparer);
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), item);
            }
            return list;
        }



        public static SortedList<TKey, TValue> ToSortedList<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IComparer<TKey> comparer)
        {
            SortedList<TKey, TValue> list = new SortedList<TKey, TValue>(comparer);
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), valueSelector(item));
            }
            return list;
        }

        public static HashSet<TElement> ToHashSet<TElement>(
            this IEnumerable<TElement> source)
        {
            return new HashSet<TElement>(source);
        }

        public static SortedSet<TElement> ToSortedSet<TElement>(
            this IEnumerable<TElement> source)
        {
            SortedSet<TElement> set = new SortedSet<TElement>();
            foreach (var element in source)
            {
                set.Add(element);
            }
            return set;
        }
    }
}
