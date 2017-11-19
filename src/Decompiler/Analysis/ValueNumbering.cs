#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TextWriter = System.IO.TextWriter;
using StringWriter = System.IO.StringWriter;
using Reko.Core.Services;

namespace Reko.Analysis
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
        private Dictionary<Expression, Expression> optimistic;	// maps <valnum> -> <node>
        private Dictionary<Expression, Expression> valid;
		private Stack<Node> stack;
		private int iDFS;
		private Dictionary<Identifier,Node> nodes;
        private DecompilerEventListener listener;

		private static Constant zero;
		private static TraceSwitch trace = new TraceSwitch("ValueNumbering", "Follows the flow of value numbering");

        public class AnyValueNumber : Identifier
		{
			public static AnyValueNumber Instance = new AnyValueNumber();

			private AnyValueNumber() : base("any", null, null)
			{
			}
		}

		public ValueNumbering(SsaIdentifierCollection ssaIds, DecompilerEventListener listener)
		{
			this.ssaIds = ssaIds;
            optimistic = new Dictionary<Expression, Expression>();
			valid = new Dictionary<Expression,Expression>();
			stack = new Stack<Node>();
            this.listener = listener;

			// Set initial value numbers for all nodes (SSA identifiers). 
			// Value numbers for the original values at procedure entry are just the
			// SSA identifiers, while any other values are undefined.
			
			AssignInitialValueNumbers();

			// Walk the SCC's of the node graph using Tarjan's algorithm.

			iDFS = 0;
			foreach (var id in nodes.Keys)
			{
				DFS(id);
			}
		}

		private void AddNodeInOrder(List<Node> al, Node n)
		{
			for (int i = 0; i < al.Count; ++i)
			{
				if (n.DfsNumber < al[i].DfsNumber)
				{
					al.Insert(i, n);
					return;
				}
			}
			al.Add(n);
		}

		public void AssignInitialValueNumbers()
		{
            nodes = new Dictionary<Identifier, Node>();
			foreach (var id in ssaIds)
			{
				Node n = new Node(id);
				if (id.IsOriginal)
				{
					n.vn = Lookup(n.info.Identifier, valid, n.info.Identifier);
				}
				else
				{
					n.vn = AnyValueNumber.Instance;
				}
				nodes[id.Identifier] = n;
			}
		}

        public class ValueNumberingContext : EvaluationContext
        {
            private Dictionary<Expression,Expression> table;

            public ValueNumberingContext(Dictionary<Expression , Expression> table)
            {
                this.table = table;
            }
            
            public Expression GetValue(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(MemoryAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(SegmentedAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Application appl)
            {
                throw new NotImplementedException();
            }

            public Expression GetDefiningExpression(Identifier id)
            {
                throw new NotImplementedException();
            }

            public void RemoveIdentifierUse(Identifier id)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
                throw new NotImplementedException();
            }

            public void RemoveExpressionUse(Expression exp)
            {
                throw new NotImplementedException();
            }

            public void SetValue(Identifier id, Expression value)
            {
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public bool IsUsedInPhi(Identifier id)
            {
                return false;
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                throw new NotImplementedException();
            }
        }

		/// <summary>
		/// Assigns a value number to the variable identified by the node 'n'.
		/// If the node changes value, we return true.
		/// </summary>
		/// <param name="n"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		private bool AssignValueNumber(Node n, Dictionary<Expression,Expression> table)
		{
            var  simp = new ExpressionSimplifier(new ValueNumberingContext(table), listener);
			Expression expr;
			if (n.definingExpr == null)
			{
				if (trace.TraceVerbose) Debug.Write("Can't simplify node: " + n.lvalue);
				expr = n.lvalue;
			}
			else
			{
				if (trace.TraceVerbose) Debug.Write("Simplified: " + n.definingExpr);
				expr = n.definingExpr.Accept(simp);
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


		private void ClassifyInductionVariable(List<Node> scc)
		{
			RegionConstantFinder rc = new RegionConstantFinder(scc, ssaIds);
			Block header = null;
			foreach (Node n in scc)
			{
				if (header == null) // || header.RpoNumber > n.info.DefStatement.Block.RpoNumber)
					header = n.info.DefStatement.Block;
			}

			bool fInductionVariable = true;
			foreach (Node n in scc)
			{
				if (!rc.IsInductiveOperation(n.info.DefStatement))
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

		private Node DFS(Identifier id)
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
					List<Node> scc = new List<Node>();
					Node x;
					do
					{
						x = stack.Pop();
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
			return nodes[id].definingExpr;
		}

		public Expression GetValueNumber(Identifier id)
		{
			return nodes[id].vn;
		}

		/// <summary>
		/// Looks up the value number of an expression. If it can't be
		/// found in the table, the SSA name <paramref>lvalue</paramref> is used as a value number.
		/// in the table.
		/// </summary>
		/// <param name="expr">Expression to look for</param>
		/// <returns>Value number of the expression</returns>
		public Expression Lookup(Expression expr, Dictionary<Expression,Expression> table, Expression lvalue)
		{
			Expression e;
            if (table.TryGetValue(expr, out e))
			{
                return e;
			}
			else
			{
				table.Add(expr, lvalue);
				return lvalue;
			}
		}

		/// <summary>
		/// Process a strongly connected component of identifiers and phi-
		/// functions. A singleton is easy, we just give it a value number.
		/// Loops have to be iterated until they stabilize.
		/// </summary>
		/// <param name="scc"></param>
		private void ProcessScc(List<Node> scc)
		{
			if (scc.Count == 1)
			{
				AssignValueNumber(scc[0], valid);
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
			SortedList<string,string> vals = new SortedList<string,string>();
			writer.WriteLine("Values:");
			foreach (KeyValuePair<Expression,Expression> de in valid)
			{
				string s = string.Format("{0}: <{1}>", de.Key, de.Value);
				vals.Add(s, s);
			}
			foreach (string s in vals.Values)
			{
				writer.WriteLine("\t{0}", s);
			}
			writer.WriteLine("Identifiers:");
            throw new NotImplementedException("Fix this once we decided to use ValueNumbering again.");
            //for (int i = 0; i != nodes.Count; ++i)
            //{
            //    throw new NotImplementedException("Use hashtable for ssaIds //				SsaIdentifier info = ssaIds[i];");
            //    //writer.WriteLine("\t{0}: <{1}>", info.Identifier, nodes[i].vn);
            //}
		}

		private static Constant Zero
		{
			get 
			{ 
				if (zero == null)
				{
					zero = Constant.Byte(0);
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
				if (info.DefStatement != null)
				{
					Assignment ass = info.DefStatement.Instruction as Assignment;
					if (ass != null)
					{
						this.lvalue = ass.Dst;
						return;
					}
					PhiAssignment phi = info.DefStatement.Instruction as PhiAssignment;
					if (phi != null)
					{
						this.lvalue = phi.Dst;
						return;
					}
				}
				this.lvalue = info.Identifier;
			}
		}

		private class NodeVisitor : InstructionVisitorBase
		{
			private SsaIdentifierCollection ssaIds;
			private ValueNumbering vn;
			private Dictionary<Identifier,Node> nodes;
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
				if (node.info.DefStatement != null && !(n.info.Identifier is MemoryIdentifier))
					node.info.DefStatement.Instruction.Accept(this);
				else
					node.definingExpr = n.info.Identifier;
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
				Node o = nodes[id];
				if (!o.Visited)
				{
					o = vn.DFS(id);
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
			private List<Node> scc;
			private bool inductive = true;
			private Constant stride;

			public RegionConstantFinder(List<Node> scc, SsaIdentifierCollection ssaIds)
			{
				this.ssaIds = ssaIds;
				this.scc = scc;
				this.stride = Constant.Invalid;
			}

			public bool IsInductiveOperation(Statement stm)
			{
				stm.Instruction.Accept(this);
				return inductive;
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
				stride = c;
			}

			public override void VisitBinaryExpression(BinaryExpression binExp)
			{
				binExp.Left.Accept(this);
				if (inductive)
					binExp.Right.Accept(this);
			}

			public override void VisitIdentifier(Identifier id)
			{
                throw new NotImplementedException();
                //inductive = scc.Contains(id);
                //if (inductive)
                //    identifier = id;
                //else
                //    identifier = null;
			}
		}
	}
}
