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

namespace Reko.Core.Lib
{
	/// <summary>
	/// Encapsulates Tarjan's algorithm for finding strongly connected
    /// components (SCC's) in a directed graph.
	/// </summary>
	/// <remarks>
	/// The algorithm itself is generic, and uses the graph and 
    /// processScc procedure to perform the 
    /// actual work.
	/// </remarks>
	public class SccFinder<TNode>
	{
        private DirectedGraph<TNode> graph;
        private Action<IList<TNode>> processScc;
        private Action<TNode> firstVisit;
        private int nextDfs = 0;
		private Stack<Node> stack = new Stack<Node>();
		private Dictionary<TNode,Node> map = new Dictionary<TNode,Node>();

        public SccFinder(DirectedGraph<TNode> graph, Action<IList<TNode>> processScc) :
            this(graph, x => { }, processScc) 
        {
        }

        public SccFinder(DirectedGraph<TNode> graph, Action<TNode> firstVisit, Action<IList<TNode>> processScc)
        {
            this.graph = graph;
            this.firstVisit = firstVisit;
            this.processScc = processScc;
            this.nextDfs = 0;
        }

		private Node AddNode(TNode o)
		{
			Node node;
            if (!map.TryGetValue(o, out node))
			{
				node = new Node(o);
				map[o] = node;
			}
			return node;
		}

        private void Dfs(Node node)
        {
            firstVisit(node.o);

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
                List<TNode> scc = new List<TNode>();
                Node x;
                do
                {
                    x = stack.Pop();
                    scc.Add(x.o);
                } while (x != node);
                processScc(scc);
            }
        }

        /// <summary>
        /// Find all the SCC's starting at the node <paramref name="start" />.
        /// </summary>
        public void Find(TNode start)
        {
            if (!map.ContainsKey(start))
                Dfs(AddNode(start));
        }

        
        /// <summary>
        /// Find all the SCC's in the entire graph.
        /// </summary>
        public void FindAll()
        {
            foreach (TNode node in this.graph.Nodes)
            {
                if (!map.ContainsKey(node))
                    Dfs(AddNode(node));
            }
        }


        private IEnumerable<Node> GetSuccessors(Node node)
        {
            foreach (TNode successor in graph.Successors(node.o))
            {
                yield return AddNode(successor);
            }
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
