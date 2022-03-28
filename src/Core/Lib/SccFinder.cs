#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Core.Lib
{
	/// <summary>
	/// Encapsulates Tarjan's algorithm for finding strongly connected
    /// components (SCC's) in a directed graph.
	/// </summary>
	/// </remarks>
	public class SccFinder<TNode>
        where TNode : notnull
	{
        private readonly DirectedGraph<TNode> graph;
        private int nextDfs = 0;
		private readonly Stack<Node> stack = new Stack<Node>();
		private readonly Dictionary<TNode,Node> map = new Dictionary<TNode,Node>();

        public SccFinder(DirectedGraph<TNode> graph)
        {
            this.graph = graph;
            this.nextDfs = 0;
        }

		private Node AddNode(TNode o)
		{
            if (!map.TryGetValue(o, out Node? node))
			{
				node = new Node(o);
				map[o] = node;
			}
			return node;
		}

        private void Dfs(Node node, List<TNode[]> result)
        {
            node.dfsNumber = nextDfs++;
            node.visited = true;
            node.low = node.dfsNumber;
            stack.Push(node);

            foreach (Node o in graph.Successors(node.o).Select(n => AddNode(n)))
            {
                if (!o.visited)
                {
                    Dfs(o, result);
                    node.low = Math.Min(node.low, o.low);
                }
                if (o.dfsNumber < node.dfsNumber && stack.Contains(o))
                {
                    node.low = Math.Min(o.dfsNumber, node.low);
                }
            }
            if (node.low == node.dfsNumber)
            {
                var scc = new List<TNode>();
                Node x;
                do
                {
                    x = stack.Pop();
                    scc.Add(x.o);
                } while (x != node);
                result.Add(scc.ToArray());
            }
        }

        /// <summary>
        /// Find all the SCC's starting at the node <paramref name="start" />.
        /// </summary>
        public List<TNode[]> Find(TNode start)
        {
            var list = new List<TNode[]>();
            if (!map.ContainsKey(start))
                Dfs(AddNode(start), list);
            return list;
        }

        
        /// <summary>
        /// Find all the SCC's in the entire graph.
        /// </summary>
        public List<TNode[]> FindAll()
        {
            var list = new List<TNode[]>();
            foreach (TNode node in this.graph.Nodes)
            {
                if (!map.ContainsKey(node))
                    Dfs(AddNode(node), list);
            }
            return list;
        }

		private class Node
		{
			public int dfsNumber;
			public bool visited;
			public int low;
			public TNode o;

			public Node(TNode o)
			{
				this.o = o;
			}
		}
    }
}
