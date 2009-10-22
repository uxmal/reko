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

using System;
using System.Collections.Generic;

namespace Decompiler.Core.Lib
{
    /// <summary>
    /// Iterates through a directed graph in Depth-First order.
    /// </summary>
    public class DfsIterator<T>
    {
        private HashSet<T> visited = new HashSet<T>();
        private Converter<T, IEnumerable<T>> getSuccessors;

        public DfsIterator(Converter<T, IEnumerable<T>> getSuccessors)
        {
            this.visited = new HashSet<T>();
            this.getSuccessors = getSuccessors;
        }

        public IEnumerable<T> PreOrder(T item)
        {
            Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
            visited.Add(item);
            yield return item;
            stack.Push(getSuccessors(item).GetEnumerator());
            while (stack.Count > 0)
            {
                IEnumerator<T> cur = stack.Pop();
                if (cur.MoveNext())
                {
                    stack.Push(cur);
                    T succ = cur.Current;
                    if (!visited.Contains(succ))
                    {
                        visited.Add(succ);
                        yield return succ;
                        stack.Push(getSuccessors(succ).GetEnumerator());
                    }
                }
            }
        }

        private struct PostOrderItem
        {
            public T Item;
            public IEnumerator<T> Children;
            public PostOrderItem(T item, IEnumerator<T> children) { Item = item; Children = children; }
        }

        public IEnumerable<T> PostOrder(T item)
        {
            Stack<PostOrderItem> stack = new Stack<PostOrderItem>();
            visited.Add(item);
            stack.Push(new PostOrderItem(item, getSuccessors(item).GetEnumerator()));
            while (stack.Count > 0)
            {
                PostOrderItem cur = stack.Pop();
                if (cur.Children.MoveNext())
                {
                    stack.Push(cur);
                    T succ = cur.Children.Current;
                    if (!visited.Contains(succ))
                    {
                        visited.Add(succ);
                        stack.Push(new PostOrderItem(succ, getSuccessors(succ).GetEnumerator()));
                    }
                }
                else
                {
                    yield return cur.Item;
                }

            }
        }


        public virtual IEnumerable<T> GetSuccessors(T item)
        {
            return null;
        }

    }
}
