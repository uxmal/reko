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
	/// A directed graph implementation.
	/// $TODO: recover lost space from deleted nodes and edges if this proves necessary.
	/// </summary>
	public class DirectedGraph
	{
		private Hashtable mpobjectNode = new Hashtable();		// Maps object to integer index into nodes array.
		private Node [] nodes;
		private int cNodes;
		private Edge [] edges;
		private int cEdges;
		private NodeCollection nodeCollection;

		public DirectedGraph()
		{
			this.nodeCollection = new NodeCollection(this);
		}

		public void AddNode(object n)
		{
			if (mpobjectNode.Contains(n))
				return;

			AddNodeInternal(n);
		}

		private int AddNodeInternal(object n)
		{
			if (nodes == null)
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

		public void AddEdge(object from, object to)
		{
			int iFrom = NodeIndex(from);
			if (iFrom <  0)
				iFrom = AddNodeInternal(from);
			int iTo = NodeIndex(to);
			if (iTo < 0)
				iTo = AddNodeInternal(to);

			if (edges == null)
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

			int iEdgePrev = nodes[iFrom].firstSucc;
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

		private ICollection CreateEdgeCollectionCore(object o, bool fSuccessors)
		{
			object oIdx = mpobjectNode[o];
			if (oIdx == null)
				return new EdgeCollection(this, -1, true);
			int iNode = (int) oIdx;
			return new EdgeCollection(this, iNode, fSuccessors);
		}

		private int NodeIndex(object o)
		{
			object oIdx = mpobjectNode[o];
			if (oIdx == null)
				return -1;
			return (int) oIdx;
		}

		public ICollection Nodes
		{
			get { return nodeCollection; }
		}

		public ICollection Predecessors(object o)
		{
			return CreateEdgeCollectionCore(o, false);
		}

		public void RemoveEdge(object from, object to)
		{
			int iFrom = NodeIndex(from);
			int iTo = NodeIndex(to);
			if (nodes[iFrom].firstSucc == -1 || nodes[iTo].firstPred == -1)
				throw new ArgumentException("No such edge");

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

		public ICollection Successors(object o)
		{
			return CreateEdgeCollectionCore(o, true);
		}


		internal struct Node
		{
			public object Item;
			public int firstPred;
			public int firstSucc;
			public int cSucc;
			public int cPred;
		}

		internal struct Edge
		{
			public int from;
			public int to;
			public int nextPred;
			public int nextSucc;
		}

		private class EdgeCollection :
			ICollection
		{
			private DirectedGraph graph;
			private int iNode;
			private bool fSuccessor;

			public EdgeCollection(DirectedGraph graph, int iNode, bool fSuccessor)
			{
				this.graph = graph;
				this.iNode = iNode;
				this.fSuccessor = fSuccessor;
			}

			#region ICollection Members
			public bool IsSynchronized
			{
				get { return false; }
			}

			public int Count
			{
				get 
				{
					return fSuccessor
						  ? graph.nodes[iNode].cSucc
						  : graph.nodes[iNode].cPred; }
			}

			public void CopyTo(Array array, int index)
			{
				if (fSuccessor)
				{
					int iEdge = graph.nodes[iNode].firstSucc;
					int i = 0;
					while (iEdge != -1)
					{
						array.SetValue(graph.nodes[graph.edges[iEdge].to].Item, index + i);
						iEdge = graph.edges[iEdge].nextSucc;
						++i;
					}
				}
				else
				{
					int iEdge = graph.nodes[iNode].firstPred;
					int i = 0;
					while (iEdge != -1)
					{
						array.SetValue(graph.nodes[graph.edges[iEdge].from].Item, index + i);
						++i;
						iEdge = graph.edges[iEdge].nextPred;
					}
				}
			}

			public object SyncRoot
			{
				get { return null; }
			}
			#endregion

			#region IEnumerable Members
			public IEnumerator GetEnumerator()
			{
				return new EdgeEnumerator(graph, iNode, fSuccessor);
			}
			#endregion

		}


		private class EdgeEnumerator : 
			IEnumerator
		{
			private DirectedGraph graph;
			private int iEdge = -1;
			private int iNode;
			private bool fSuccessor;

			public EdgeEnumerator(DirectedGraph graph, int iNode, bool fSuccessor)
			{
				this.graph = graph;
				this.iNode = iNode;
				this.fSuccessor = fSuccessor;
			}

			public void Reset()
			{
				iEdge = -1;
			}

			public int Count
			{
				get { return fSuccessor ? graph.nodes[iNode].cSucc : graph.nodes[iNode].cPred; }
			}

			public object Current
			{
				get
				{
					if (iEdge == -1)
						throw new InvalidOperationException("MoveNext must be called before Current");

					if (fSuccessor)
					{
						return graph.nodes[graph.edges[iEdge].to].Item;
					}
					else
					{
						return graph.nodes[graph.edges[iEdge].from].Item;
					}
				}
			}

			public IEnumerator GetEnumerator()
			{
				return this;
			}

			public bool MoveNext()
			{
				if (iNode == -1)
					return false;
				
				if (iEdge == -1)
				{
					
					iEdge = fSuccessor
						? graph.nodes[iNode].firstSucc
						: graph.nodes[iNode].firstPred;
				}
				else
				{
					iEdge = fSuccessor 
						? graph.edges[iEdge].nextSucc
						: graph.edges[iEdge].nextPred;
				}
				return iEdge != -1;
			}
		}

		private class NodeCollection : ICollection
		{
			private DirectedGraph graph;

			public NodeCollection(DirectedGraph graph)
			{
				this.graph = graph;
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public int Count
			{
				get { return graph.cNodes; }
			}

			public void CopyTo(Array array, int index)
			{
				for (int i = 0; i < graph.cNodes; ++i)
				{
					array.SetValue(graph.nodes[i], index + i);
				}
			}

			public object SyncRoot
			{
				get { return null; }
			}


			#region IEnumerable Members
			public IEnumerator GetEnumerator()
			{
				return new NodeEnumerator(graph);
			}
			#endregion

		}

		private class NodeEnumerator : IEnumerator
		{
			private DirectedGraph graph;
			private int iNode = -1;

			public NodeEnumerator(DirectedGraph graph)
			{
				this.graph = graph;
			}

			public void Reset()
			{
				iNode = -1;
			}

			public object Current
			{
				get 
				{
					if (iNode == -1 || iNode >= graph.cNodes)
						throw new InvalidOperationException("Enumerator must be positioned at a valid location.");
					return graph.nodes[iNode].Item;
				}
			}

			public bool MoveNext()
			{
				++iNode;
				return iNode < graph.cNodes;
			}
		}
	}
}
