#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Extension methods for Enumerable classes
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// Creates a <see cref="SortedList{TKey, TSource}"/> from an <see cref="IEnumerable{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the input sequence</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="source">Input sequence.</param>
        /// <param name="keySelector">Function to select the <typeparamref name="TKey"/> from
        /// a <typeparamref name="TSource"/> element.
        /// </param>
        /// <returns>A <see cref="SortedList{TKey, TSource}"/>.
        /// </returns>
        public static SortedList<TKey, TSource> ToSortedList<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : notnull
        {
            var list = new SortedList<TKey, TSource>();
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), item);
            }
            return list;
        }

        /// <summary>
        /// Creates a <see cref="SortedList{TKey, TElement}"/> from an <see cref="IEnumerable{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the input sequence</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TElement">Value type.</typeparam>
        /// <param name="source">Input sequence.</param>
        /// <param name="keySelector">Function to select the <typeparamref name="TKey"/> from
        /// a <typeparamref name="TSource"/> element.
        /// </param>
        /// <param name="elementSelector">Function to select the <typeparamref name="TElement"/>
        /// from a <typeparamref name="TSource"/> element.
        /// </param>
        /// <returns>A <see cref="SortedList{TKey, TSource}"/>.
        /// </returns>
        public static SortedList<TKey, TElement> ToSortedList<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            where TKey : notnull
        {
            var list = new SortedList<TKey, TElement>();
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), elementSelector(item));
            }
            return list;
        }

        /// <summary>
        /// Creates a <see cref="SortedList{TKey, TSource}"/> from an <see cref="IEnumerable{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the input sequence</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="source">Input sequence.</param>
        /// <param name="keySelector">Function to select the <typeparamref name="TKey"/> from
        /// a <typeparamref name="TSource"/> element.
        /// </param>
        /// <param name="comparer">Comparer to use in the sorted list.</param>
        /// <returns>A <see cref="SortedList{TKey, TSource}"/>.
        /// </returns>
        public static SortedList<TKey, TSource> ToSortedList<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
            where TKey : notnull
        {
            var list = new SortedList<TKey, TSource>(comparer);
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), item);
            }
            return list;
        }

        /// <summary>
        /// Creates a <see cref="SortedList{TKey, TElement}"/> from an <see cref="IEnumerable{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">Type of the input sequence</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="source">Input sequence.</param>
        /// <param name="keySelector">Function to select the <typeparamref name="TKey"/> from
        /// a <typeparamref name="TSource"/> element.
        /// </param>
        /// <param name="valueSelector">Function to select the <typeparamref name="TValue"/>
        /// from a <typeparamref name="TSource"/> element.
        /// </param>
        /// <param name="comparer">Comparer to use in the sorted list.</param>
        /// <returns>A <see cref="SortedList{TKey, TSource}"/>.
        /// </returns>
        public static SortedList<TKey, TValue> ToSortedList<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IComparer<TKey> comparer)
            where TKey : notnull
        {
            var list = new SortedList<TKey, TValue>(comparer);
            foreach (TSource item in source)
            {
                list.Add(keySelector(item), valueSelector(item));
            }
            return list;
        }

        /// <summary>
        /// Creates a <see cref="SortedSet{TElement}"/> from an <see cref="IEnumerable{TElement}"/>.
        /// </summary>
        /// <typeparam name="TElement">Element type.</typeparam>
        /// <param name="source">Input sequece.</param>
        /// <returns>A <see cref="SortedSet{TElement}"/>.
        /// </returns>
        public static SortedSet<TElement> ToSortedSet<TElement>(
            this IEnumerable<TElement> source)
        {
            return new SortedSet<TElement>(source);
        }

        /// <summary>
        /// "Chops up" a sequence of elements into chunks; sub-sequences of a given size.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="enumerable">Input seqience.</param>
        /// <param name="chunkSize">Chunk size.</param>
        /// <returns>A chunked sequence.
        /// </returns>
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
        public static TValue Get<TKey, TValue>(this IDictionary<TKey,TValue> d, TKey key, TValue def = default!)
        {
            if (d.TryGetValue(key, out var value))
                return value;
            else
                return def!;
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

        /// <summary>
        /// Enqueues a range of items into a queue.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="queue">Queue to add items to.</param>
        /// <param name="items">Items to add to queue.</param>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
