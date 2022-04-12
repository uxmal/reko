#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Represents thread-safe collection that provides notifications
    /// when items get added, removed, or when the whole list is refreshed.
    /// This is lock-based wrapper of
    /// <see cref="Reko.Core.Lib.ObservableRangeCollection{T}"/>.
    /// Locks will make the collection thread safe but will also cause
    /// performance to drop if there is contention on the lock.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentObservableCollection<T> : IEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                lock (collection)
                {
                    collection.CollectionChanged += value;
                }
            }
            remove
            {
                lock (collection)
                {
                    collection.CollectionChanged -= value;
                }
            }
        }

        private readonly ObservableRangeCollection<T> collection;

        public ConcurrentObservableCollection()
        {
            this.collection = new ObservableRangeCollection<T>();
        }

        public void AddRange(IEnumerable<T> range)
        {
            lock (collection)
            {
                collection.AddRange(range);
            }
        }

        public void Add(T item)
        {
            lock (collection)
            {
                collection.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (collection)
            {
                return collection.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (collection)
            {
                return collection.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
