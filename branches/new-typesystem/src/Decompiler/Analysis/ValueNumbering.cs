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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Diagnostics;
using TextWriter = System.IO.TextWriter;
using StringWriter = System.IO.StringWriter;

namespace Decompiler.Analysis
{
	/// <summary>
	/// This class is used to compute value numbers of all the SSA variables
	/// of a procedure. As a useful side effect, it also detects induction variables,
	/// determines their stride &c. This will be useful later when we need to 
	/// classify arrays.
	/// </summary>
	public class ValueNumbering
	{
		private SsaIdentifierCollection ssaIds;
		private Hashtable optimistic;	// maps <valnum> -> <node>
		private Hashtable valid;
		private Stack stack;
		private int iDFS;
		private Node [] nodes;

		private static Constant zero;
		private static TraceSwitch trace = new TraceSwitch("ValueNumbering", "Follows the flow of value numbering");

		public class AnyValueNumber : Identifier
		{
			public static AnyValueNumber Instance = new AnyValueNumber();

			private AnyValueNumber() : base("any", -1, null, null)
			{
			}
		}


		public ValueNumbering(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
			optimistic = new Hashtable();
			valid = new Hashtable();
			stack = new Stack();


			// Set initial value numbers for all nodes (SSA identifiers). 
			// Value numbers for the original values at procedure entry are just the
			// SSA identifiers, while any other values are undefined.
			
			AssignInitialValueNumbers();

			// Walk the SCC's of the node graph using Tarjan's algorithm.

			iDFS = 0;
			for (int i = 0; i < nodes.Length; ++i)
			{
				DFS(i);
			}
		}

		private void AddNodeInOrder(ArrayList al, Node n)
		{
			for (int i = 0; i < al.Count; ++i)
			{
				if (n.DfsNumber < ((Node) al[i]).DfsNumber)
				{
					al.Insert(i, n);
					return;
				}
			}
			al.Add(n);
		}

		public void AssignInitialValueNumbers()
		{
			nodes = new Node[ssaIds.Count];
			for (int i = 0; i < nodes.Length; ++i)
			{
				SsaIdentifier id = ssaIds[i];
				Node n = new Node(id);
				if (id.IsOriginal)
				{
					n.vn = Lookup(n.info.id, valid, n.info.id);
				}
				else
				{
					n.vn = AnyValueNumber.Instance;
				}
				nodes[i] = n;
			}
		}

		/// <summary>
		/// Assigns a value number to the variable identified by the node 'n'.
		/// If the node changes value, we return true.
		/// </summary>
		/// <param name="n"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		private bool AssignValueNumber(Node n, Hashtable table)
		{
			ExpressionSimplifier simp = new ExpressionSimplifier(this, table);
			Expression expr;
			if (n.definingExpr == null)
			{
				if (trace.TraceVerbose) Debug.Write("Can't simplify node: " + n.lvalue);
				expr = n.lvalue;
			}
			else
			{
				if (trace.TraceVerbose) Debug.Write("Simplified: " + n.definingExpr);
				expr = simp.Simplify(n.definingExpr);
				if (trace.TraceVerbose) Debug.Write(" to: " + expr);
			}
			Expression tmp = Lookup(expr, table, n.lvalue);
			if (tmp != n.vn)
			{
				if (trace.TraceVerbose) Debug.WriteLine(" assigned vn: " + tmp);
				n.vn = tmp;
				return true;
			}
			if (trace.TraceVerbose) Debug.WriteLine("");
			return false;
		}


		private void ClassifyInductionVariable(ArrayList scc)
		{
			RegionConstantFinder rc = new RegionConstantFinder(scc, ssaIds);
			Block header = null;
			foreach (Node n in scc)
			{
				if (header == null || header.RpoNumber > n.info.def.block.RpoNumber)
					header = n.info.def.block;
			}

			bool fInductionVariable = true;
			foreach (Node n in scc)
			{
				if (!rc.IsInductiveOperation(n.info.def))
				{
					fInductionVariable = false;
					break;
				}
			}

			if (!fInductionVariable)
			{
				header = null;
			}
/*
			int x = 0;
			foreach (Identifier id in scc)
			{
				if (fInductionVariable)
				{
					x += id.Number;
				}
			}
*/
		}


		// Analyze a node in the SSA graph. The node represents an operation
		// and any out nodes are any operands used by that operation.

		private Node DFS(int id)
		{
			Node n = nodes[id];
			if (!n.Visited)
			{
				n.Visited = true;
				n.DfsNumber = iDFS++;
				n.low = n.DfsNumber;

				if (trace.TraceVerbose) Debug.WriteLine("Visiting " + n.info.ToString());
				stack.Push(n);

				// Visit all operands used by this instruction.

				NodeVisitor nv = new NodeVisitor(this);
				nv.Visit(n);

				// Pop off a SCC and process it.

				if (n.low == n.DfsNumber)
				{
					ArrayList scc = new ArrayList();
					Node x;
					do
					{
						x = (Node) stack.Pop();
						AddNodeInOrder(scc, x);
					} while (x != n);
					ProcessScc(scc);
				}
			}
			return nodes[id];
		}

		public void Dump()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			System.Diagnostics.Debug.WriteLine(sw.ToString());
		}

		public Expression GetDefiningExpression(Identifier id)
		{
			return nodes[id.Number].definingExpr;
		}

		public Expression GetValueNumber(Identifier id)
		{
			return nodes[id.Number].vn;
		}

		/// <summary>
		/// Looks up the value number of an expression. If it can't be
		/// found in the table, the SSA name <paramref>lvalue</paramref> is used as a value number.
		/// in the table.
		/// </summary>
		/// <param name="expr">Expression to look for</param>
		/// <returns>Value number of the expression</returns>
		public Expression Lookup(Expression expr, Hashtable table, Expression lvalue)
		{
			object o = table[expr];
			if (o != null)
			{
				return (Expression) o;
			}
			else
			{
				table[expr] = lvalue;
				return lvalue;
			}
		}

		/// <summary>
		/// Process a strongly connected component of identifiers and phi-
		/// functions. A singleton is easy, we just give it a value number.
		/// Loops have to be iterated until they stabilize.
		/// </summary>
		/// <param name="scc"></param>
		private void ProcessScc(ArrayList scc)
		{
			if (scc.Count == 1)
			{
				AssignValueNumber((Node) scc[0], valid);
			}
			else
			{
				if (trace.TraceVerbose) Debug.WriteLine("adding nodes to optimistic table");
				bool changed;
				int repeatCount = 0;
				do
				{
					changed = false;
					foreach (Node n in scc)
					{
						changed |= AssignValueNumber(n, optimistic);
					}
				} while (changed && ++repeatCount < 1000);

				if (trace.TraceVerbose) Debug.WriteLine("adding nodes to valid table");
				foreach (Node n in scc)
				{
					AssignValueNumber(n, valid);
				}
				ClassifyInductionVariable(scc);
			}
		}

		public void Write(TextWriter writer)
		{
			SortedList vals = new SortedList();
			writer.WriteLine("Values:");
			foreach (DictionaryEntry de in valid)
			{
				string s = string.Format("{0}: <{1}>", de.Key, de.Value);
				vals.Add(s, s);
			}
			foreach (string s in vals.Values)
			{
				writer.WriteLine("\t{0}", s);
			}
			writer.WriteLine("Identifiers:");
			for (int i = 0; i != nodes.Length; ++i)
			{
				SsaIdentifier info = ssaIds[i];
				writer.WriteLine("\t{0}: <{1}>", info.id, nodes[i].vn);
			}
		}

		private static Constant Zero
		{
			get 
			{ 
				if (zero == null)
				{
					zero = new Constant(PrimitiveType.Byte, 0);
				}
				return zero;
			}
		}

		/// <summary>
		/// Represents a node in the SSA node graph. All edges are to operands used to compute this node.
		/// </summary>
		private class Node
		{
			public int DfsNumber;
			public int low;
			public SsaIdentifier info;
			public Expression lvalue;
			public Expression vn;
			public Expression definingExpr;	// expression that defines this node. Any edges to other nodesare contained here
			public bool Visited;

			public Node(SsaIdentifier info)
			{
				this.info = info;
				if (info.def != null)
				{
					Assignment ass = info.def.Instruction as Assignment;
					if (ass != null)
					{
						this.lvalue = ass.Dst;
						return;
					}
					PhiAssignment phi = info.def.Instruction as PhiAssignment;
					if (phi != null)
					{
						this.lvalue = phi.Dst;
						return;
					}
				}
				this.lvalue = info.id;
			}
		}

		private class NodeVisitor : InstructionVisitorBase
		{
			private SsaIdentifierCollection ssaIds;
			private ValueNumbering vn;
			private Node [] nodes;
			private Node node;
			
			public NodeVisitor(ValueNumbering vn)
			{
				this.vn = vn;
				this.ssaIds = vn.ssaIds;
				this.nodes = vn.nodes;
			}

			public void Visit(Node n)
			{
				node = n;
				if (node.info.def != null && !(n.info.id is MemoryIdentifier))
					node.info.def.Instruction.Accept(this);
				else
					node.definingExpr = n.info.id;
			}

			public override void VisitAssignment(Assignment ass)
			{
				ass.Src.Accept(this);
				node.definingExpr = ass.Src;
			}

			// Pessimistic assumption is that a call to the same
			// function never gives the same answer.

			public override void VisitApplication(Application app)
			{
			}

			public override void VisitConstant(Constant c)
			{
				node.definingExpr = c;
			}

			public override void VisitIdentifier(Identifier id)
			{
				Node o = nodes[id.Number];
				if (!o.Visited)
				{
					o = vn.DFS(id.Number);
					if (o.low < node.low)
						node.low = o.low;
				}
				if (o.DfsNumber < node.DfsNumber && 
					vn.stack.Contains(o) &&
					o.DfsNumber < node.low)
				{
					node.low = o.DfsNumber;
				}
				node.definingExpr = id;
			}

			public override void VisitPhiAssignment(PhiAssignment phi)
			{
				phi.Src.Accept(this);
				node.definingExpr = phi.Src;
			}
		}

		private class RegionConstantFinder : InstructionVisitorBase
		{
			private SsaIdentifierCollection ssaIds;
			private ArrayList scc;
			private bool inductive = true;
			private Identifier identifier;
			private Value stride;

			public RegionConstantFinder(ArrayList scc, SsaIdentifierCollection ssaIds)
			{
				this.ssaIds = ssaIds;
				this.scc = scc;
				this.stride = Value.Invalid;
			}

			public bool IsInductiveOperation(Statement stm)
			{
				stm.Instruction.Accept(this);
				return inductive;
			}

			private int RegionConstant(Expression e, Block header)
			{
				Constant c = e as Constant;
				if (c != null)
				{
					return Convert.ToInt32(c.Value);
				}
				return 0;
			}

			public override void VisitApplication(Application appl)
			{
				inductive = false;
			}

			public override void VisitAssignment(Assignment a)
			{
				a.Src.Accept(this);
				if (inductive)
					a.Dst.Accept(this);
			}

			public override void VisitConstant(Constant c)
			{
				stride = new Value((PrimitiveType) c.DataType, Convert.ToInt32(c.Value));
			}

			public override void VisitBinaryExpression(BinaryExpression binExp)
			{
				binExp.Left.Accept(this);
				if (inductive)
					binExp.Right.Accept(this);
			}

			public override void VisitIdentifier(Identifier id)
			{
				inductive = scc.Contains(id);
				if (inductive)
					identifier = id;
				else
					identifier = null;
			}
		}

	}
}
