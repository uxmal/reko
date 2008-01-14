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

namespace Decompiler.Core.Lib
{
	/// <summary>
	/// Encapsulates Tarjan's algorithm for finding strongly connected components in a directed graph.
	/// </summary>
	/// <remarks>
	/// The algorithm itself is generic, and depends on an implementation of the ISccFinderHost interface for specific
	/// nodes.
	/// </remarks>
	public class SccFinder
	{
		private ISccFinderHost host;
		private int nextDfs = 0;
		private Stack stack = new Stack();
		private Hashtable map = new Hashtable();

		public SccFinder(ISccFinderHost host)
		{
			this.host = host;
			this.nextDfs = 0;
		}

		private Node AddNode(object o)
		{
			Node node = (Node) map[o];
			if (node == null)
			{
				node = new Node(o);
				map[o] = node;
			}
			return node;
		}

		private void Dfs(Node node)
		{
			node.dfsNumber = nextDfs++;
			node.visited = true;
			node.low = node.dfsNumber;
			stack.Push(node);
			foreach (Node o in GetSuccessors(node))
			{
				if (!o.visited)
				{
					Dfs(o);
					node.low = Math.Min(node.low, o.low);
				}
				if (o.dfsNumber < node.dfsNumber && stack.Contains(o))
				{
					node.low = Math.Min(o.dfsNumber, node.low);
				}
			}
			if (node.low == node.dfsNumber)
			{
				ArrayList scc = new ArrayList();
				Node x;
				do
				{
					x = (Node) stack.Pop();
					scc.Add(x.o);
				} while (x != node);
				host.ProcessScc(scc);
			}
		}

		public void Find(object start)
		{
			if (map[start] == null)
				Dfs(AddNode(start));
		}

		private ArrayList GetSuccessors(Node node)
		{
			ArrayList succ = new ArrayList();
			host.AddSuccessors(node.o, succ);
			for (int i = 0; i < succ.Count; ++i)
				succ[i] = AddNode(succ[i]);
			return succ;
		}

		private class Node
		{
			public int dfsNumber;
			public bool visited;
			public int low;
			public object o;

			public Node(object o)
			{
				this.o = o;
			}
		}
	}


	public interface ISccFinderHost
	{
		void AddSuccessors(object o, ArrayList succ);

		void ProcessScc(ArrayList scc);
	}
}
