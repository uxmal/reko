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
using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Core
{
	/// <summary>
	/// Describes the dominator structure of a particular graph.
	/// </summary>
	public class DominatorGraph
	{
		private Procedure proc;
		private int [] doms;
		private ArrayList [] domFrontiers;

		public DominatorGraph(Procedure proc)
		{
			this.proc = proc;
			if (proc != null)
			{
				this.doms = Build(proc);
				this.domFrontiers = BuildDominanceFrontiers(proc);
				this.doms[0] = -1;		// No-one dominates the root node.
			}
		}

		public Block CommonDominator(ICollection blocks)
		{
			if (blocks == null || blocks.Count == 0)
                return null; 

			Block dominator = null;
			foreach (Block b in blocks)
			{
				if (b == null)
					return null;
				if (dominator == null)
				{
					dominator = b;
				}
				else if (b != dominator && !DominatesStrictly(dominator.RpoNumber, b.RpoNumber))
				{
					if (DominatesStrictly(b.RpoNumber, dominator.RpoNumber))
					{
						dominator = b;
					}
					else
					{
						do
						{
							dominator = ImmediateDominator(dominator);
						} while (!DominatesStrictly(dominator.RpoNumber, b.RpoNumber));
					}
				}
			}
			return dominator;

		}

		public virtual bool DominatesStrictly(Statement stmDom, Statement stm)
		{
			if (stmDom == stm)
				return false;
			if (stmDom.block == stm.block)
			{
				foreach (Statement s in stmDom.block.Statements)
				{
					if (stmDom == s)
						return true;
					if (stm == s)
						return false;
				}
				throw new ApplicationException("Impossible: both stmDom and stm should be in stmDom's block!");
			}
			else
			{
				return DominatesStrictly(stmDom.block.RpoNumber, stm.block.RpoNumber);
			}
		}

		public bool DominatesStrictly(int dominator, int d)
		{
			while (doms[d] != -1)
			{
				if (doms[d] == dominator)
					return true;
				d = doms[d];
			}
			return false;
		}

		public Block ImmediateDominator(Block b)
		{
			int i = doms[b.RpoNumber];
			return (i < 0) ? null : proc.RpoBlocks[i];
		}

		public static int [] Build(Procedure proc)
		{
			const int Undefined = -1;
			int [] doms = new int[proc.RpoBlocks.Count];
			for (int i = 0; i != doms.Length; ++i)
				doms[i] = Undefined;

			doms[proc.EntryBlock.RpoNumber] = proc.EntryBlock.RpoNumber;
			bool fChanged;
			do
			{
				fChanged = false;
				foreach (Block bb in proc.RpoBlocks)
				{
					int c = bb.Pred.Count;
					if (c == 0)
						continue;		// Skip the start node.
					int newIdom = Undefined;
					foreach (Block pp in bb.Pred)
					{
						int p = pp.RpoNumber;
						if (doms[p] != Undefined)
						{
							if (newIdom == Undefined)
							{
								newIdom = p;
							}
							else 
							{
								newIdom = Intersect(doms, p, newIdom);
							}
						}
					}
					if (doms[bb.RpoNumber] != newIdom)
					{
						doms[bb.RpoNumber] = newIdom;
						fChanged = true;
					}
				}
			} while (fChanged);
			return doms;
		}

		private ArrayList [] BuildDominanceFrontiers(Procedure proc)
		{
			ArrayList [] fronts = new ArrayList[doms.Length];
			for (int i = 0; i != doms.Length; ++i)
			{
				fronts[i] = new ArrayList();
			}

			foreach (Block bb in proc.RpoBlocks)
			{
				if (bb.Pred.Count >= 2)
				{
					int b = bb.RpoNumber;
					foreach (Block p in bb.Pred)
					{
						int r = p.RpoNumber;
						while (r != -1 && r != doms[b])
						{
							// Add b to the dominance frontier of r.

							if (fronts[r] == null)
							{
								fronts[r] = new ArrayList();
								fronts[r].Add(bb);
							}
							else
							{
								if (!fronts[r].Contains(bb))
									fronts[r].Add(bb);
							}

							r = doms[r];
						}
					}
				}
			}
			return fronts;
		}

		public ArrayList DominatorFrontier(Block b)
		{
			return domFrontiers[b.RpoNumber];
		}

		public void Dump()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			Debug.Write(sw.ToString());
		}

		public IEnumerable GetDominatedNodes(Block b)
		{
			return new DominatedNodesEnumerator(b, this);
		}
		
		private static int Intersect(int [] doms, int b1, int b2)
		{
			int i1 = b1;
			int i2 = b2;
			while (i1 != i2)
			{
				while (i1 > i2)
					i1 = doms[i1];
				while (i2 > i1)
					i2 = doms[i2];
			}
			return i1;
		}

		public void Write(TextWriter writer)
		{
			foreach (Block b in proc.RpoBlocks)
			{
				Block idom = this.ImmediateDominator(b);
				writer.Write("{0} ({1}): idom {2}, Frontier:", b.RpoNumber, b.Name,  (idom != null ? idom.RpoNumber : -1));
				foreach (Block f in DominatorFrontier(b))
				{
					writer.Write(" {0}", f.RpoNumber);
				}
				writer.WriteLine();
			}
		}

		private class DominatedNodesEnumerator : IEnumerable, IEnumerator
		{
			private Block b;
			private DominatorGraph dom;
			private IEnumerator e;

			public DominatedNodesEnumerator(Block b, DominatorGraph dom)
			{
				this.b = b;
				this.dom = dom;
				this.e = b.Procedure.RpoBlocks.GetEnumerator();
			}
			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerator Members

			public void Reset()
			{
				e.Reset();
			}

			public object Current
			{
				get { return e.Current; }
			}

			public bool MoveNext()
			{
				while (e.MoveNext())
				{
					Block t = (Block) e.Current;
					if (dom.DominatesStrictly(b.RpoNumber, t.RpoNumber))
						return true;
				}
				return false;
			}

			#endregion
		}

	}
}
