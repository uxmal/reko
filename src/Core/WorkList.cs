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
using System.Collections.Generic;

namespace Reko.Core
{
	/// <summary>
	/// A WorkList contains a queue of items to be processed. 
	/// </summary>
	public class WorkList<T>
	{
		private HashSet<T> inQ;
		private Queue<T> q;

		public WorkList()
		{
			q = new Queue<T>();
			inQ = new HashSet<T>();
		}

		public WorkList(IEnumerable<T> coll)
		{
			q = new Queue<T>(coll);
			inQ = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="t"></param>
		public void Add(T item)
		{
			if (!inQ.Contains(item))
			{
				q.Enqueue(item);
				inQ.Add(item);
			}
		}

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

		public int Count
		{
			get { return inQ.Count; }
		}

		public bool IsEmpty
		{
			get { return inQ.Count == 0; }
		}

        public bool Contains(T item)
        {
            return inQ.Contains(item);
        }

		public bool GetWorkItem(out T item)
		{
			while (!IsEmpty)
			{
				T t = q.Dequeue();
				if (inQ.Contains(t))
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

    /// <summary>
    /// A WorkStack contains a stack of items to be processed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorkStack<T>
    {
		private HashSet<T> inStack;
		private Stack<T> s;

		public WorkStack()
		{
			s = new Stack<T>();
			inStack = new HashSet<T>();
		}

		public WorkStack(IEnumerable<T> coll)
		{
			s = new Stack<T>(coll);
			inStack = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="t"></param>
		public void Add(T item)
		{
			if (!inStack.Contains(item))
			{
				s.Push(item);
				inStack.Add(item);
			}
		}

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

		public int Count
		{
			get { return inStack.Count; }
		}

		public bool IsEmpty
		{
			get { return inStack.Count == 0; }
		}

		public bool GetWorkItem(out T item)
		{
			while (!IsEmpty)
			{
				T t = s.Pop();
				if (inStack.Contains(t))
				{
					inStack.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default(T);
            return false;
		}

		public void Remove(T t)
		{
			inStack.Remove(t);
		}
    }
}
