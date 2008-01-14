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

using System;
using System.Collections;

namespace Decompiler.Core
{
	public class ProcedureCollection : IDictionary
	{
		private SortedList innerList;

		public ProcedureCollection()
		{
			innerList = new SortedList();
		}

		public Procedure this[Address addr]
		{
			get { return (Procedure) innerList[addr]; }
			set { innerList[addr] = value; }
		}

		public Procedure this[int index]
		{
			get { return (Procedure) innerList.GetByIndex(index); }
		}

		public void Add(Address addr, Procedure proc)
		{
			innerList.Add(addr, proc);
		}

		public bool Contains(Address addr)
		{
			return innerList.Contains(addr);
		}

		#region IDictionary members
		public void CopyTo(Array a, int i)
		{
			innerList.CopyTo(a, i);
		}

		public int Count
		{
			get { return innerList.Count; }
		}

		public bool IsSynchronized
		{
			get { return innerList.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return innerList.SyncRoot; }
		}


		object IDictionary.this[object key]
		{
			get { return innerList[(Address) key]; }
			set { innerList[(Address) key] = value; }
		}

		void IDictionary.Add(object key, object value)
		{
			innerList.Add((Address) key, (Procedure) value);
		}

		public void Clear()
		{
			innerList.Clear();
		}

		bool IDictionary.Contains(object key)
		{
			return innerList.Contains((Address) key);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		public bool IsFixedSize
		{
			get { return innerList.IsFixedSize; }
		}

		public bool IsReadOnly
		{
			get { return innerList.IsReadOnly; }
		}

		public ICollection Keys
		{
			get { return innerList.Keys; }
		}

		void IDictionary.Remove(object key)
		{
			innerList.Remove((Address) key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		public ICollection Values
		{
			get { return innerList.Values; }
		}


		#endregion
	}
}
