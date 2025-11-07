#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
	/// <summary>
	/// A WorkList contains a queue of items to be processed.
	/// </summary>
	public class WorkList<T>
	{
		private readonly HashSet<T> inQ;
		private readonly Queue<T> q;

        /// <summary>
        /// Creates an empty work list.
        /// </summary>
		public WorkList()
		{
			q = new Queue<T>();
			inQ = new HashSet<T>();
		}

        /// <summary>
        /// Creates a work list and populates it with some items.
        /// </summary>
        /// <param name="coll">Items to populate the work list with.</param>
		public WorkList(IEnumerable<T> coll)
		{
			q = new Queue<T>(coll);
			inQ = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="item">Item to add to the work list.</param>
		public void Add(T item)
		{
			if (!inQ.Contains(item))
			{
				q.Enqueue(item);
				inQ.Add(item);
			}
		}

        /// <summary>
        /// Adds a range of items to the work list.
        /// </summary>
        /// <param name="items">Items to add to the work list.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Gets the number of items in the worklist.
        /// </summary>
		public int Count => inQ.Count;

        /// <summary>
        /// Returns true if the worklist is empty.
        /// </summary>
		public bool IsEmpty => inQ.Count == 0;

        /// <summary>
        /// Returns true if the worklist contains the item.
        /// </summary>
        /// <param name="item">Item to check for</param>
        /// <returns>True if the worklist contains the item; otherwise false.
        /// </returns>
        public bool Contains(T item)
        {
            return inQ.Contains(item);
        }

        /// <summary>
        /// Try to extract a work item from the work list, removing it from the
        /// list if one is present.
        /// </summary>
        /// <param name="item">If there are work items in the work list, the
        /// extracted item.
        /// </param>
        /// <returns>
        /// True if an item is successfully removed; otherwise false.
        /// </returns>
		public bool TryGetWorkItem([MaybeNullWhen(false)] out T item)
		{
			while (q.TryDequeue(out var t))
			{
				if (inQ.Contains(t))
				{
					inQ.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default!;
            return false;
		}

        /// <summary>
        /// Removes an item from the work list.
        /// </summary>
		public void Remove(T t)
		{
			inQ.Remove(t);
		}
	}

    /// <summary>
    /// Helper class for worklist-related methods.
    /// </summary>
    public static class WorkList
    {
        /// <summary>
        /// Convenience function to create a worklist of items.
        /// </summary>
        public static WorkList<T> Create<T>(IEnumerable<T> items) =>
            new WorkList<T>(items);
    }


    /// <summary>
    /// A WorkStack contains a stack of items to be processed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorkStack<T>
    {
		private readonly HashSet<T> inStack;
		private readonly Stack<T> s;

        /// <summary>
        /// Creates an empty workstack.
        /// </summary>
		public WorkStack()
		{
			s = new Stack<T>();
			inStack = new HashSet<T>();
		}

        /// <summary>
        /// Creates a workstack and initializes it with items.
        /// </summary>
        /// <param name="coll"></param>
		public WorkStack(IEnumerable<T> coll)
		{
			s = new Stack<T>(coll);
			inStack = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="item">Item to add to the worklist.</param>
		public void Add(T item)
		{
			if (!inStack.Contains(item))
			{
				s.Push(item);
				inStack.Add(item);
			}
		}

        /// <summary>
        /// Adds a range of items to the work stack.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Gets the number of items in the workstack.
        /// </summary>
		public int Count => inStack.Count;

        /// <summary>
        /// Returns true if there are no items in the workstack.
        /// </summary>
		public bool IsEmpty => inStack.Count == 0;


        /// <summary>
        /// Gets a work item out of the workstack.
        /// </summary>
        /// <param name="item">If succesful, a work item.</param>
        /// <returns>True if there was at least one item in the workstack;
        /// otherwise false.</returns>
		public bool TryGetWorkItem([MaybeNullWhen(false)] out T item)
		{
			while (s.TryPop(out var t))
			{
				if (inStack.Contains(t))
				{
					inStack.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default;
            return false;
		}

        /// <summary>
        /// Removes an item from the workstack.
        /// </summary>
        /// <param name="item">Item to remove.</param>
		public void Remove(T item)
		{
			inStack.Remove(item);
		}
    }
}
