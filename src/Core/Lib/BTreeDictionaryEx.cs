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

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Reko.Core.Lib
{
    /// <summary>
    /// Extension methods on BTreeDictionary that provide support for lower- and
    /// upper bound searchers.
    /// </summary>
    public static class BTreeDictionaryEx
    {
        public static bool TryGetLowerBound<K, V>(this BTreeDictionary<K, V> list, K key, out V value)
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            value = default(V);
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

        public static bool TryGetUpperBound<K, V>(this BTreeDictionary<K, V> list, K key, out V value)
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            value = default(V);
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

        public static bool TryGetLowerBoundKey<K, V>(this BTreeDictionary<K, V> list, K key, out K closestKey)
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestKey = default(K);
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

        public static bool TryGetUpperBoundKey<K, V>(this BTreeDictionary<K, V> list, K key, out K closestKey)
        {
            var cmp = list.Comparer;
            int lo = 0;
            int hi = list.Count - 1;
            closestKey = default(K);
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

        public static bool TryGetLowerBoundIndex<K, V>(this BTreeDictionary<K, V> list, K key, out int closestIndex)
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

        public static bool TryGetUpperBoundIndex<K, V>(this BTreeDictionary<K, V> list, K key, out int closestIndex)
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
