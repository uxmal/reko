#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.Collections.Generic;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Extension methods on SortedList that provide support for lower- and
    /// upper bound searchers.
    /// </summary>
    public static class SortedListEx
    {
        /// <summary>
        /// Finds the largest key in the list that is less than or equal to
        /// the given key, and the corresponding value.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Resulting key.</param>
        /// <param name="value">Resulting value.</param>
        /// <returns>True if a lower bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetLowerBound<K,V>(this SortedList<K, V> list, K key, out V value)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            value = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    value = list.Values[mid];
                    return true;
                }
                if (c < 0)
                {
                    value = list.Values[mid];
                    set = true;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Finds the smallest key in the list that is greater than or equal to
        /// the given key, and the corresponding value.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Resulting key.</param>
        /// <param name="value">Resulting value.</param>
        /// <returns>True if an upper bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetUpperBound<K,V>(this SortedList<K, V> list, K key, out V value)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            value = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    value = list.Values[mid];
                    return true;
                }
                if (c > 0)
                {
                    value = list.Values[mid];
                    set = true;
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Finds the largest key in the list that is less than or equal to
        /// the given key.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Key to search for.</param>
        /// <param name="closestKey">Key of lower bound.</param>
        /// <returns>True if a lower bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetLowerBoundKey<K, V>(this SortedList<K, V> list, K key, out K closestKey)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestKey = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    closestKey = k;
                    return true;
                }
                if (c < 0)
                {
                    closestKey = k;
                    set = true;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Finds the smallest key in the list that is greater than or equal to
        /// the given key.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Resulting key.</param>
        /// <param name="closestKey">Key of lower bound.</param>
        /// <returns>True if an upper bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetUpperBoundKey<K,V>(this SortedList<K, V> list, K key, out K closestKey)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestKey = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    closestKey = k;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    closestKey = k;
                    set = true;
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Finds the index of the largest key in the list that is less than or equal to
        /// the given key.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Key whose lower bound index is requested.</param>
        /// <param name="closestIndex">Index of lower bound.</param>
        /// <returns>True if an lower bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetLowerBoundIndex<K, V>(this SortedList<K, V> list, K key, out int closestIndex)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestIndex = -1;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    closestIndex = mid;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                    closestIndex = mid;
                    set = true;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Finds the index of the smallest key in the list that is greater than or equal to
        /// the given key.
        /// </summary>
        /// <typeparam name="K">Key data type.</typeparam>
        /// <typeparam name="V">Value data type.</typeparam>
        /// <param name="list"><see cref="SortedDictionary{TKey, TValue}"/> instance.</param>
        /// <param name="key">Key whose upper bound index is requested.</param>
        /// <param name="closestIndex">Index of upper bound.</param>
        /// <returns>True if an upper bound could be found; otherwise false.
        /// </returns>
        public static bool TryGetUpperBoundIndex<K, V>(this SortedList<K, V> list, K key, out int closestIndex)
            where K : notnull
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestIndex = -1;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = list.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    closestIndex = mid;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    closestIndex = mid;
                    set = true;
                    hi = mid - 1;
                }
            }
            return set;
        }
    }
}
