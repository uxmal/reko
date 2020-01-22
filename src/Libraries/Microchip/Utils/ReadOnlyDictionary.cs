#region License
/* 
 * Copyright (c) 2017-2020 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dict;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> sourceDictionary)
            => dict = sourceDictionary ?? throw new ArgumentNullException(nameof(sourceDictionary));

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => dict[key];
            set => throw new NotSupportedException();
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => dict[key];

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dict.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dict.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => dict.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dict.Values;
        int ICollection<KeyValuePair<TKey, TValue>>.Count => dict.Count;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => dict.Count;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => dict.ContainsKey(key);
        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => dict.ContainsKey(key);
        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);
        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dict.Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dict.CopyTo(array, arrayIndex);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) dict).GetEnumerator();

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
         => throw new NotSupportedException();
        bool IDictionary<TKey, TValue>.Remove(TKey key)
            => throw new NotSupportedException();
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => throw new NotSupportedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => throw new NotSupportedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
            => throw new NotSupportedException();

    }

}
