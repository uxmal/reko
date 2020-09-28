#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion
 
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

        // Well dang. System.Linq.Enumerable.ToHashSet() exists in .NET Core 2.0 and later,
        // .NET Framework 4.7.2 and later, but not .NET Standard 2.0. 
        //$REFACTOR: remove this when we're on .NET Standard 2.1
        public static HashSet<TElement> ToHashSet<TElement>(
            this IEnumerable<TElement> source)
        {
            return new HashSet<TElement>(source);
        }

        public static SortedSet<TElement> ToSortedSet<TElement>(
            this IEnumerable<TElement> source)
        {
            return new SortedSet<TElement>(source);
            }

        public static IEnumerable<IEnumerable<T>> Chunks<T>(
            this IEnumerable<T> enumerable,
            int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive.");

            using (var e = enumerable.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var remaining = chunkSize;    // elements remaining in the current chunk
                    var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                    yield return e.GetChunk(innerMoveNext);
                    while (innerMoveNext()) {/* discard elements skipped by inner iterator */}
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e,
                                                  Func<bool> innerMoveNext)
        {
            do yield return e.Current;
            while (innerMoveNext());
        }

        /// <summary>
        /// Gets a value from a dictionary. If the value is not present,
        /// returns a default value.
        /// </summary>
        /// <remarks>Patterned after Python's dict.get(key [,=defaultvalue]) function</remarks>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey,TValue> d, TKey key, TValue def = default(TValue))
        {
            if (d.TryGetValue(key, out var value))
                return value;
            else
                return def;
        }

        public static IEnumerable<TResult> ZipMany<TSource, TResult>(
            IEnumerable<IEnumerable<TSource>> source,
            Func<IEnumerable<TSource>, TResult> selector)
        {
            // ToList is necessary to avoid deferred execution
            var enumerators = source.Select(seq => seq.GetEnumerator()).ToList();
            try
            {
                while (true)
                {
                    bool moved = false;
                    foreach (var e in enumerators)
                    {
                        bool b = e.MoveNext();
                        if (!b) yield break;
                        moved = true;
                    }
                    if (!moved)
                        yield break;
                    // Again, ToList (or ToArray) is necessary to avoid deferred execution
                    yield return selector(enumerators.Select(e => e.Current).ToList());
                }
            }
            finally
            {
                foreach (var e in enumerators)
                    e.Dispose();
            }
        }

        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
