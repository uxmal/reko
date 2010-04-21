/* 
 * Copyright (C) 1999-2010 John Källén.
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
using System.Collections.Generic;

namespace Decompiler.Core
{
	/// <summary>
	/// A WorkList contains a list of items to be processed. 
	/// </summary>
	public class WorkList<T>
	{
		private Dictionary<T,T> inQ;
		private Queue<T> q;

		public WorkList()
		{
			q = new Queue<T>();
			inQ = new Dictionary<T,T>();
		}

		public WorkList(ICollection<T> coll)
		{
			q = new Queue<T>(coll);
			inQ = new Dictionary<T,T>(coll.Count);
			foreach (T t in coll)
			{
				Add(t);
			}
		}

		public void Add(T  t)
		{
			if (!inQ.ContainsKey(t))
			{
				q.Enqueue(t);
				inQ.Add(t, t);
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

		public bool GetWorkItem(out T item)
		{
			while (!IsEmpty)
			{
				T t = q.Dequeue();
				if (inQ.ContainsKey(t))
				{
					inQ.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default(T);
            return false;
		}

		public void Remove(T t)
		{
			inQ.Remove(t);
		}
	}
}
