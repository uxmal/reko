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
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Lib
{
    /// <summary>
    /// A <see cref="Counter{T}"/> is a wrapper around a <see cref="Dictionary{TKey, int}"/>
    /// which provides convenient counts of items.
    /// </summary>
    /// <typeparam name="T">Item type to be tallied.</typeparam>
    public class Counter<T> : Dictionary<T, int>
    {
        public Counter()
        {
        }

        public Counter(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (TryGetValue(item, out int count))
                {
                    this[item] = count + 1;
                }
                else
                {
                    this.Add(item, 1);
                }
            }
        }

        /// <summary>
        /// Increments the count of an item by 1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Add(T item)
        {
            if (TryGetValue(item, out int count))
            {
                int newCount = count + 1;
                this[item] = newCount;
                return newCount;
            }
            else
            {
                this.Add(item, 1);
                return 1;
            }
        }

        /// <summary>
        /// Decrement the count of the item by one, removing it if count reaches zero.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the count was more than zero, otherwise false.</returns>
        public new bool Remove(T item)
        {
            if (TryGetValue(item, out int count))
            {
                int newCount = count - 1;
                if (newCount > 0)
                    this[item] = newCount;
                else
                    this.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a sequence of items, where each item is replicated "count" times.
        /// </summary>
        public IEnumerable<T> GetElements()
        {
            foreach (var de in this)
            {
                int c = de.Value;
                while (c > 0)
                {
                    yield return de.Key;
                }
            }
        }
    }
}
