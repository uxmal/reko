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
using System.Collections;

namespace Reko.Core.Types
{
	/// <summary>
	/// Implements a sparse set over type variables.
	/// </summary>
	public class TypeVariableSet : ICollection, IComparer
	{
		private SortedList InnerList;

		public TypeVariableSet()
		{
			InnerList = new SortedList(this);
		}

		public void Add(TypeVariable var)
		{
			InnerList[var] = var;
		}

		public void CopyTo(Array arr, int i)
		{
			throw new NotImplementedException();
		}

		public int Compare(object a, object b)
		{
			TypeVariable ta = (TypeVariable) a;
			TypeVariable tb = (TypeVariable) b;
			return ta.Number - tb.Number;
		}

		public int Count { get { return InnerList.Count; } }

		public bool IsSynchronized { get { return InnerList.IsSynchronized; } }

		public object SyncRoot { get { return InnerList.SyncRoot; } }

		public IEnumerator GetEnumerator()
		{
			return InnerList.Values.GetEnumerator();
		}
	}
}
