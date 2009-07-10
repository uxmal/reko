/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.Collections.Generic;

namespace Decompiler.Structure
{
	/// <summary>
	/// Contains information about what blocks are in what interval and vice versa.
	/// </summary>
	public class IntervalCollection : ICollection<IntNode>
	{
		private SortedList<int, IntNode> items;

		public IntervalCollection(int initialCapacity)
		{
            this.items = new SortedList<int, IntNode>(initialCapacity);
		}

		public void Add(IntNode i)
		{
            throw new NotImplementedException("items.Add(i.HeaderBlock.RpoNumber, i);");
		}

        public bool Remove(IntNode i)
        {
            throw new NotImplementedException("return items.Remove(i.HeaderBlock.RpoNumber);");
        }

		#region ICollection Members

		public bool IsSynchronized
		{
			get { return false; }
		}

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(IntNode interval)
        {
            throw new NotImplementedException("return items.ContainsKey(interval.HeaderBlock.RpoNumber);");
        }

		public int Count
		{
			get { return items.Count; }
		}

		public void CopyTo(IntNode [] array, int index)
		{
			items.Values.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get { return null; }
		}

		#endregion

		#region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        public IEnumerator<IntNode> GetEnumerator()
		{
			return items.Values.GetEnumerator();
		}

		#endregion
	}
}
