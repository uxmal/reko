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
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.Core.Lib
{
	/// <summary>
	/// Iterates through a directed graph in Depth-First order.
	/// </summary>
	public class DfsIterator<T> //$TODO : IEnumerator<T> where T : class 
	{
#if NOTYET
		private DirectedGraph<T> graph;
		private Stack<StackItem> stack;
		private T startNode;
		private T curNode;
		private IEnumerator<T> enumCur;
		private Dictionary<T, T> visited;

		public DfsIterator(DirectedGraph<T> graph, T startNode)
		{
			this.graph = graph;
			this.startNode = startNode;

			Reset();
		}

		public DfsIterator(DirectedGraph<T> graph, T startNode, bool reverse)
		{
			this.graph = graph;
			this.startNode = startNode;
			Reset();
		}

		private void AddNodeToStack(T node)
		{
			stack.Push(new StackItem(node, graph.Successors(node).GetEnumerator()));
		}

        object IEnumerator.Current
        {
            get { return curNode; }
        }

		public T Current
		{
			get { return curNode; }
		}

        public void Dispose()
        {
            graph = null;
            enumCur = null;
            GC.SuppressFinalize(this);
        }

		public IEnumerator GetEnumerator()
		{
			return this; 
		}

		public bool MoveNext()
		{
			if (enumCur == null)
				return false;
			
			do
			{
				while (!enumCur.MoveNext())
				{
					if (stack.Count == 0)
					{
						enumCur = null;
						return false;
					}
                    StackItem item = stack.Pop();
                    enumCur = item.Enum;
				}
				curNode = enumCur.Current;
			} while (visited.ContainsKey(curNode));

			visited[curNode] = curNode;
			stack.Push(new enumCur);
			enumCur = graph.Successors(curNode).GetEnumerator();
			return true;
		}

		public void Reset()
		{
			stack = new Stack();
			visited = new Hashtable();
			object [] os = new object[] { startNode };
			enumCur = os.GetEnumerator();
		}

		private class StackItem
		{
			public object Node;
			public IEnumerator<T> Enum;

			public StackItem(object node, IEnumerator e)
			{
				Node = node;
				Enum = e;
			}
		}
#endif
	}

}
