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
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Extension methods on BTreeDictionary that provide support for lower- and
    /// upper bound searchers.
    /// </summary>
    public static class BTreeDictionaryEx
    {
        /// <summary>
        /// Finds the lower bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="value">Value which is the lower bound of the key.</param>
        /// <returns>True if a lower bound could be find.
        /// </returns>
        public static bool TryGetLowerBound<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out V value)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            value = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    value = dictionary.Values[mid];
                    return true;
                }
                if (c < 0)
                {
                    value = dictionary.Values[mid];
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
        /// Finds the upper bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="value">Value which is the upper bound of the key.</param>
        /// <returns>True if a upper bound could be find.
        /// </returns>

        public static bool TryGetUpperBound<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out V value)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            value = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    value = dictionary.Values[mid];
                    return true;
                }
                if (c > 0)
                {
                    value = dictionary.Values[mid];
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
        /// Finds the lower bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="closestKey">Key which is the lower bound of the key.</param>
        /// <returns>True if a lower bound could be find.
        /// </returns>
        public static bool TryGetLowerBoundKey<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out K closestKey)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            closestKey = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
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
        /// Finds the upper bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="closestKey">Key which is the upper bound of the key.</param>
        /// <returns>True if a upper bound could be find.
        /// </returns>
        public static bool TryGetUpperBoundKey<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out K closestKey)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            closestKey = default!;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
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
        /// Finds the index of lower bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="closestIndex">Index which is the lower bound of the key.</param>
        /// <returns>True if a lower bound could be find.
        /// </returns>
        public static bool TryGetLowerBoundIndex<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out int closestIndex)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            closestIndex = -1;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
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
        /// Finds the index of upper bound of a key in a BTreeDictionary.
        /// </summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="closestIndex">Index which is the lower bound of the key.</param>
        /// <returns>True if a lower bound could be find.
        /// </returns>
        public static bool TryGetUpperBoundIndex<K, V>(this BTreeDictionary<K, V> dictionary, K key, [MaybeNullWhen(false)] out int closestIndex)
        {
            var cmp = dictionary.Comparer;
            int lo = 0;
            int hi = dictionary.Count - 1;
            closestIndex = -1;
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = dictionary.Keys[mid];
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
