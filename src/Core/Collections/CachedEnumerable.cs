#region License
/* 
 * Copyright (C) 2018-2026 Stefano Moioli <smxdev4@gmail.com>.
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

#pragma warning disable CS1591

namespace Reko.Core.Collections
{
	public class CachedEnumerable<T> : IReadOnlyList<T>, IEnumerable<T>, IDisposable
	{
		private readonly IList<T> cache = new List<T>();
		private readonly IEnumerator<T> items;

		public CachedEnumerable(IEnumerable<T> items) {
			this.items = items.GetEnumerator();
		}

		public T this[int index] {
			get {
				// check if the item is already there
				if (this.cache.Count > index) {
					return this.cache[index];
				}

				// read until we find it
				foreach (var item in this) {
					if (this.cache.Count > index) {
						return item;
					}
				}

				// we didn't find it
				throw new IndexOutOfRangeException();
			}
		}

		public int Count {
			get {
				return this.cache.Count;
			}
		}

		public void Dispose() {
			this.items?.Dispose();
		}

		public IEnumerator<T> GetEnumerator() {
			foreach (var item in this.cache) {
				yield return item;
			}

			while (this.items.MoveNext()) {
				var item = this.items.Current;
				this.cache.Add(item);
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
