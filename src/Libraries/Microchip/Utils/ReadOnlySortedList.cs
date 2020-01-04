#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Libraries.Microchip
{
    public class ReadOnlySortedList<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> 
    {
        private readonly SortedList<TKey, TValue> list;

        public ReadOnlySortedList(SortedList<TKey, TValue> sourceList)
        {
            list = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => list.Contains(item);
        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => list.ContainsKey(key);
        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => list.ContainsKey(key);
        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => list.TryGetValue(key, out value);
        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => list.TryGetValue(key, out value);
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => list[key];
        TValue IDictionary<TKey, TValue>.this[TKey key] { get => list[key]; set => throw new NotImplementedException(); }
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => list.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => list.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => list.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => list.Values;
        int ICollection<KeyValuePair<TKey, TValue>>.Count => list.Count;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => list.Count;
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
            => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => throw new NotImplementedException();
        bool IDictionary<TKey, TValue>.Remove(TKey key)
            => throw new NotImplementedException();
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
            => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => throw new NotImplementedException();

    }

}
