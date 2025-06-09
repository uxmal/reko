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

namespace Reko.Core.Graphs
{
	/// <summary>
	/// A directed graph implementation.
	/// $TODO: recover lost space from deleted nodes and edges if this 
    /// proves necessary.
	/// </summary>
	public class DirectedGraphImpl<T> : DirectedGraph<T>
        where T : class
	{
		private Dictionary<T, int> mpobjectNode = new Dictionary<T,int>();		// Maps object to integer index into nodes array.
		private Node[]? nodes;
		private int cNodes;
		private Edge[]? edges;
		private int cEdges;
		private NodeCollection nodeCollection;

        /// <summary>
        /// Creates an instance of the <see cref="DirectedGraphImpl{T}"/> class.
        /// </summary>
		public DirectedGraphImpl()
		{
			this.nodeCollection = new NodeCollection(this);
		}

        /// <summary>
        /// Adds a node to the graph. If the node already exists, nothing changes.
        /// </summary>
        /// <param name="n">Node to be added.</param>
		public void AddNode(T n)
		{
			if (mpobjectNode.ContainsKey(n))
				return;

			AddNodeInternal(n);
		}

		private int AddNodeInternal(T n)
		{
			if (nodes is null)
			{
				nodes = new Node[8];
				cNodes = 0;
			}
			if (cNodes == nodes.Length)
			{
				Node [] newNodes = new Node[cNodes * 2];
				nodes.CopyTo(newNodes, 0);
				nodes = newNodes;
			}

			nodes[cNodes].Item = n;
			nodes[cNodes].firstPred = -1;
			nodes[cNodes].firstSucc = -1;
			nodes[cNodes].cPred = 0;
			nodes[cNodes].cSucc = 0;
			mpobjectNode.Add(n, cNodes);
			return cNodes++;
		}

        /// <inheritdoc/>
		public void AddEdge(T from, T to)
		{
			int iFrom = NodeIndex(from);
			if (iFrom <  0)
				iFrom = AddNodeInternal(from);
			int iTo = NodeIndex(to);
			if (iTo < 0)
				iTo = AddNodeInternal(to);

			if (edges is null)
			{
				edges = new Edge[8];
				cEdges = 0;
			}
			if (cEdges == edges.Length)
			{
				Edge [] newEdges = new Edge[cEdges * 2];
				edges.CopyTo(newEdges, 0);
				edges = newEdges;
			}

			int iEdgePrev = nodes![iFrom].firstSucc;
			if (iEdgePrev == -1)			// This node has no successors.
				nodes[iFrom].firstSucc = cEdges;
			else
			{
				while (edges[iEdgePrev].nextSucc != -1)
				{
					iEdgePrev = edges[iEdgePrev].nextSucc;
				}
				edges[iEdgePrev].nextSucc = cEdges;
			}

			iEdgePrev = nodes[iTo].firstPred;
			if (iEdgePrev == -1)			// this node has no predecessors.
				nodes[iTo].firstPred = cEdges;
			else
			{
				while (edges[iEdgePrev].nextPred != -1)
				{
					iEdgePrev = edges[iEdgePrev].nextPred;
				}
				edges[iEdgePrev].nextPred = cEdges;
			}

			edges[cEdges].from = iFrom;
			edges[cEdges].to = iTo;
			edges[cEdges].nextSucc = -1;
			edges[cEdges].nextPred = -1;
			++cEdges;
			++nodes[iFrom].cSucc;
			++nodes[iTo].cPred;
		}

        /// <inheritdoc/>
        public bool ContainsEdge(T from, T to)
        {
            if (edges is null)
                return false;
            int iFrom = NodeIndex(from);
            if (iFrom < 0)
                return false;
            int iTo = NodeIndex(to);
            if (iTo < 0)
                return false;

            int iEdge = nodes![iFrom].firstSucc;
            while (iEdge >= 0)
            {
                if (edges[iEdge].to == iTo)
                    return true;
                iEdge = edges[iEdge].nextSucc;
            }
            return false;
        }

		private ICollection<T> CreateEdgeCollectionCore(T o, bool fSuccessors)
		{
            int iNode;
            if (!mpobjectNode.TryGetValue(o, out iNode))
                throw new ArgumentException(string.Format("Unknown node {0}.", o));
                //return new EdgeCollection(this, -1, fSuccessors);
			return new EdgeCollection(this, iNode, fSuccessors);
		}

		private int NodeIndex(T o)
		{
            int idx;
            if (mpobjectNode.TryGetValue(o, out idx))
                return idx;
            else
				return -1;
		}

        /// <inheritdoc/>
		public ICollection<T> Nodes
		{
			get { return nodeCollection; }
		}

        /// <inheritdoc/>
		public ICollection<T> Predecessors(T o)
		{
			return CreateEdgeCollectionCore(o, false);
		}

        /// <inheritdoc/>
		public void RemoveEdge(T from, T to)
		{
			int iFrom = NodeIndex(from);
			int iTo = NodeIndex(to);
			if (nodes is null || edges is null || nodes[iFrom].firstSucc == -1 || nodes[iTo].firstPred == -1)
				throw new ArgumentException("No such edge.");

			// Remove the edge in the from and to list.

			int iEdgeFPrev= -1;
			int iEdgeF;
			for (iEdgeF = nodes[iFrom].firstSucc; iEdgeF != -1; iEdgeF = edges[iEdgeF].nextSucc)
			{
				if (edges[iEdgeF].to == iTo)
					break;
				iEdgeFPrev = iEdgeF;
			}

			int iEdgeTPrev = -1;
			int iEdgeT;
			for (iEdgeT = nodes[iTo].firstPred; iEdgeT != -1; iEdgeT = edges[iEdgeT].nextPred)
			{
				if (edges[iEdgeT].from == iFrom)
					break;
				iEdgeTPrev = iEdgeT;
			}

			if (iEdgeT != iEdgeF)
				throw new ArgumentException("No such edge");

			if (iEdgeFPrev == -1)
				nodes[iFrom].firstSucc = edges[iEdgeF].nextSucc;
			else
				edges[iEdgeFPrev].nextSucc = edges[iEdgeF].nextSucc;
			
			if (iEdgeTPrev == -1)
				nodes[iTo].firstPred = edges[iEdgeT].nextPred;
			else
				edges[iEdgeTPrev].nextPred = edges[iEdgeT].nextPred;

			edges[iEdgeT].from = -1;
			edges[iEdgeT].to = -1;
			edges[iEdgeT].nextSucc = -1;
			edges[iEdgeT].nextPred = -1;
			--nodes[iFrom].cSucc;
			--nodes[iTo].cPred;
		}

        /// <inheritdoc/>
		public ICollection<T> Successors(T node)
		{
			return CreateEdgeCollectionCore(node, true);
		}

		private struct Node
		{
			public T Item;
			public int firstPred;
			public int firstSucc;
			public int cSucc;
			public int cPred;

            public override string ToString()
            {
                return "Node: " + Item is null ? "(null)" : Item.ToString()!;
            }
		}

		private struct Edge
		{
			public int from;
			public int to;
			public int nextPred;
			public int nextSucc;
		}

		private class EdgeCollection : ICollection<T>
		{
			private DirectedGraphImpl<T> graph;
			private int iNode;
			private bool fSuccessor;

			public EdgeCollection(DirectedGraphImpl<T> graph, int iNode, bool fSuccessor)
			{
                if (iNode < 0)
                    throw new ArgumentException("Invalid node.");
				this.graph = graph;
				this.iNode = iNode;
				this.fSuccessor = fSuccessor;
			}

			#region IEnumerable Members
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

			public IEnumerator<T> GetEnumerator()
			{
                if (fSuccessor)
                    return GetSuccessorEnumerator();
                else
                    return GetPredecessorEnumerator();
			}

            private IEnumerator<T> GetSuccessorEnumerator()
            {
                if (graph.nodes is null || graph.edges is null)
                    yield break;
                int iEdge = graph.nodes[iNode].firstSucc;
                while (iEdge >= 0)
                {
                    yield return graph.nodes[graph.edges[iEdge].to].Item;
                    iEdge = graph.edges[iEdge].nextSucc;
                }
            }

            private IEnumerator<T> GetPredecessorEnumerator()
            {
                if (graph.nodes is null || graph.edges is null || iNode == -1)
                    yield break;
                int iEdge = graph.nodes[iNode].firstPred;
                while (iEdge >= 0)
                {
                    yield return graph.nodes[graph.edges[iEdge].from].Item;
                    iEdge = graph.edges[iEdge].nextPred;
                }
            }

            #endregion


            #region ICollection<T> Members

            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                var e = GetEnumerator();
                int i = arrayIndex;
                while (e.MoveNext() && i < array.Length)
                {
                    array[i++] = e.Current;
                }
            }

            public int Count
            {
                get
                {
                    if (fSuccessor)
                    {
                        if (graph.nodes is not null)
                            return graph.nodes[iNode].cSucc;
                        else
                            return 0;
                    }
                    else
                    {
                        if (graph.nodes is not null)
                            return graph.nodes[iNode].cPred;
                        else
                            return 0;
                    }
                }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

		private class NodeCollection : ICollection<T>
		{
			private DirectedGraphImpl<T> graph;

			public NodeCollection(DirectedGraphImpl<T> graph)
			{
				this.graph = graph;
			}

            public void Add(T node)
            {
                graph.AddNode(node);
            }

			public bool IsSynchronized
			{
				get { return false; }
			}

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T node)
            {
                return graph.mpobjectNode.ContainsKey(node);
            }

			public int Count
			{
				get { return graph.cNodes; }
			}

			public void CopyTo(T [] array, int index)
			{
                if (graph.nodes is null)
                    return;
                for (int i = 0; i + index < array.Length; ++i)
                {
                    array[i + index] = graph.nodes[i].Item;
                }
			}

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T node)
            {
                throw new NotImplementedException("Remove");
            }

			public object? SyncRoot
			{
				get { return null; }
			}

			#region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new NodeEnumerator(graph);
            }

			public IEnumerator<T> GetEnumerator()
			{
				return new NodeEnumerator(graph);
			}
			#endregion
		}

		private class NodeEnumerator : IEnumerator<T>
		{
			private DirectedGraphImpl<T> graph;
			private int iNode = -1;

			public NodeEnumerator(DirectedGraphImpl<T> graph)
			{
				this.graph = graph;
			}

			public void Reset()
			{
				iNode = -1;
			}

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

			public T Current
			{
				get 
				{
					if (iNode == -1 || iNode >= graph.cNodes || graph.nodes is null)
						throw new InvalidOperationException("Enumerator must be positioned at a valid location.");
					return graph.nodes[iNode].Item;
				}
			}

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

			public bool MoveNext()
			{
				++iNode;
				return iNode < graph.cNodes;
			}
		}
	}
}
