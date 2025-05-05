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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Iterates through a directed graph in Depth-First order.
    /// </summary>
    public class DfsIterator<T>
    {
        private readonly DirectedGraph<T> graph;
        private readonly HashSet<T> visited = new HashSet<T>();

        /// <summary>
        /// Creates a new depth-first iterator for the given graph.
        /// </summary>
        /// <param name="graph">Graph to traverse.</param>
        public DfsIterator(DirectedGraph<T> graph)
        {
            this.graph = graph;
            this.visited = new HashSet<T>();
        }

        /// <summary>
        /// Visit every node in the graph in pre-order depth first order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of the nodes in 
        /// pre-order.</returns>
        public IEnumerable<T> PreOrder()
        {
            foreach (T node in graph.Nodes)
            {
                if (visited.Contains(node))
                    continue;
                foreach (T n in PreOrder(node))
                    yield return n;
            }
        }

        /// <summary>
        /// Visit graph nodes in pre-order depth first order,
        /// starting at <paramref name="item"/>.
        /// </summary>
        /// <remarks>
        /// Note that this pre order visit consumes O(1) call stack space,
        /// although it  </remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T> PreOrder(T item)
        {
            var stack = new Stack<IEnumerator<T>>();
            visited.Add(item);
            yield return item;
            stack.Push(graph.Successors(item).GetEnumerator());
            while (stack.TryPop(out var cur))
            {
                if (cur.MoveNext())
                {
                    stack.Push(cur);
                    T succ = cur.Current;
                    if (!visited.Contains(succ))
                    {
                        visited.Add(succ);
                        yield return succ;
                        stack.Push(graph.Successors(succ).GetEnumerator());
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

        /// <summary>
        /// Visit every node in the graph in post-order depth first order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> PostOrder()
        {
            foreach (T item in graph.Nodes)
            {
                if (visited.Contains(item))
                    continue;
                foreach (T node in PostOrder(item))
                    yield return node;
            }
        }

        /// <summary>
        /// Visit graph nodes in post-order depth first order, starting at 
        /// <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Starting node.</param>
        /// <returns>The nodes in post-order.</returns>
        public IEnumerable<T> PostOrder(T item)
        {
            var stack = new Stack<PostOrderItem>();
            visited.Add(item);
            stack.Push(new PostOrderItem(item, graph.Successors(item).GetEnumerator()));
            while (stack.TryPop(out var cur))
            {
                if (cur.Children.MoveNext())
                {
                    stack.Push(cur);
                    T succ = cur.Children.Current;
                    if (!visited.Contains(succ))
                    {
                        visited.Add(succ);
                        stack.Push(new PostOrderItem(succ, graph.Successors(succ).GetEnumerator()));
                    }
                }
                else
                {
                    yield return cur.Item;
                }
            }
        }

        /// <summary>
        /// Visit all graph nodes in reverse post-order depth first order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of the nodes in RPO.</returns>
        public IEnumerable<T> ReversePostOrder()
        {
            return PostOrder().Reverse();
        }
    }
}
