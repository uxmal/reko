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
	/// <summary>
	/// A WorkList contains a list of items to be processed. 
	/// </summary>
	public class WorkList
	{
		private Hashtable inQ;
		private Queue q;

		public WorkList()
		{
			q = new Queue();
			inQ = new Hashtable();
		}

		public WorkList(ICollection coll)
		{
			q = new Queue(coll);
			inQ = new Hashtable(coll.Count);
			foreach (object o in coll)
			{
				Add(o);
			}
		}

		public void Add(object o)
		{
			if (!inQ.Contains(o))
			{
				q.Enqueue(o);
				inQ.Add(o, o);
			}
		}

		public int Count
		{
			get { return inQ.Count; }
		}

		public bool IsEmpty
		{
			get { return inQ.Count == 0; }
		}

		public object GetWorkItem()
		{
			while (!IsEmpty)
			{
				object o = q.Dequeue();
				if (inQ.Contains(o))
				{
					inQ.Remove(o);
					return o;
				}
			}
			return null;
		}

		public void Remove(object o)
		{
			inQ.Remove(o);
		}
	}
}
