/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections;

namespace Decompiler.Structure
{
	/// <summary>
	/// Contains information about what blocks are in what interval and vice versa.
	/// </summary>
	public class IntervalCollection : ICollection
	{
		private SortedList items;

		public IntervalCollection(int initialCapacity)
		{
			this.items = new SortedList(initialCapacity);
		}

		public void Add(Interval i)
		{
			items.Add(i.Header.RpoNumber, i);
		}


		#region ICollection Members

		public bool IsSynchronized
		{
			get { return items.IsSynchronized; }
		}

		public int Count
		{
			get { return items.Count; }
		}

		public void CopyTo(Array array, int index)
		{
			items.Values.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get { return items.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return items.Values.GetEnumerator();
		}

		#endregion
	}
}
