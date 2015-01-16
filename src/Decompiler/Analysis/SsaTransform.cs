#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Decompiler.Core.Procedure"/> to Static Single Assignment form.
	/// </summary>
	public class SsaTransform
	{
		private Identifier [] varsOrig;
		private Procedure proc;
		private DominatorGraph<Block> domGraph;

		private const byte BitDefined = 1;
		private const byte BitDeadIn = 2;
		private const byte BitHasPhi = 4;

		/// <summary>
		/// Constructs an SsaTransform, and in the process generates the SsaState for the procedure <paramref>proc</paramref>.
		/// </summary>
		/// <param name="proc"></param>
		/// <param name="gr"></param>
		public SsaTransform(Procedure proc, DominatorGraph<Block> gr)
		{
			this.proc = proc;
			this.domGraph = gr;
			this.varsOrig = new Identifier[proc.Frame.Identifiers.Count];
			proc.Frame.Identifiers.CopyTo(varsOrig);

			Transform();
		}

        public SsaState SsaState { get; private set; }

        private int RpoNumber(Block b)
        {
            return domGraph.ReversePostOrder[b];
        }

		/// <summary>
		/// Creates a phi statement with slots for each predecessor block, then
		/// inserts the phi statement as the first statement of the block.
		/// </summary>
		/// <param name="b">Block into which the phi statement is inserted</param>
		/// <param name="v">Destination variable for the phi assignment</param>
		/// <returns>The inserted phi Assignment</returns>
		private Instruction InsertPhiStatement(Block b, Identifier v)
		{
			var stm = new Statement(
                0,
				new PhiAssignment(v, b.Pred.Count),
				b);
			b.Statements.Insert(0, stm);
			return stm.Instruction;
		}

		private void LocateAllDefinedVariables(Dictionary<Identifier, byte>[] defOrig)
		{
			var ldv = new LocateDefinedVariables(proc, this, defOrig);
			foreach (Block n in domGraph.ReversePostOrder.Keys)
			{
				ldv.LocateDefs(n);
			}
		}

		private void MarkTemporariesDeadIn(Dictionary<Identifier, byte>[] def)
		{
            foreach (var block in proc.ControlGraph.Blocks)
            {
                int iBlock = RpoNumber(block);
                foreach (Identifier id in proc.Frame.Identifiers.Where(id => id.Storage is TemporaryStorage))
                {
                    byte bits;
                    if (!def[iBlock].TryGetValue(id, out bits))
                        bits = 0;
                    def[iBlock][id] = (byte)(bits | BitDeadIn);
                }
            }
		}

		private void PlacePhiFunctions()
		{
            var AOrig = CreateA();
			LocateAllDefinedVariables(AOrig);
			MarkTemporariesDeadIn(AOrig);

			// For each defined variable in block n, collect the places where it is defined

			foreach (var a in varsOrig)
			{
				// Create a worklist W of all the blocks that define a.

				var W = new WorkList<Block>();
                foreach (Block b in domGraph.ReversePostOrder.Keys) 
				{
                    byte bits;
                    AOrig[RpoNumber(b)].TryGetValue(a, out bits);
					if ((bits & BitDefined) != 0)
						W.Add(b);
				}
                Block n;
                while (W.GetWorkItem(out n))
                {
                    foreach (Block y in domGraph.DominatorFrontier(n))
                    {
                        // Only add phi functions if there is no
                        // phi already and variable is not deadIn.

                        var dict = AOrig[RpoNumber(y)];
                        byte bits;
                        dict.TryGetValue(a, out bits);
                        if ((bits & (BitHasPhi | BitDeadIn)) == 0)
                        {
                            bits |= BitHasPhi;
                            dict[a] = bits;
                            InsertPhiStatement(y, a);
                            if ((bits & BitDefined) == 0)
                            {
                                W.Add(y);
                            }
                        }
                    }
                }
			}
		}

        private Dictionary<Identifier, byte>[] CreateA()
        {
            var a = new Dictionary<Identifier, byte>[proc.ControlGraph.Blocks.Count];
            for (int i = 0; i < a.Length; ++i)
            {
                a[i] = new Dictionary<Identifier, byte>();
            }
            return a;
        }

		public void Transform()
		{
            this.SsaState = new SsaState(proc, domGraph);
			PlacePhiFunctions();
			var rn = new VariableRenamer(this, varsOrig, proc);
			rn.RenameBlock(proc.EntryBlock);
		}

		/// <summary>
		/// Locates the variables defined in this block by examining
        /// each statement to find variables in L-value positions.
		/// In addition, the set deadIn for each block is calculated.
		/// These are all the variables that are known to be dead on
		/// entry to the function. Dead variables won't need phi code!
		/// </summary>
		private class LocateDefinedVariables : InstructionVisitorBase
		{
			private Procedure proc;
			private Block block;
            private Dictionary<Identifier, byte>[] defVars; // variables defined by a statement.
			private Statement stmCur;
            private SsaTransform ssa;

			public LocateDefinedVariables(Procedure proc, SsaTransform ssa, Dictionary<Identifier, byte>[] defOrig)
			{
				this.proc = proc;
                this.ssa = ssa;
                this.defVars = defOrig;
			}

			private void MarkDefined(Identifier id)
			{
				Debug.Assert(id.Number >= 0);
                var dict = defVars[ssa.RpoNumber(block)];
                byte bits;
                dict.TryGetValue(id, out bits);
				dict[id] = (byte) (bits | (BitDefined | BitDeadIn));
			}

			public void LocateDefs(Block b)
			{
				this.block = b;
				for (int i = block.Statements.Count - 1; i >= 0; --i)
				{
					stmCur = block.Statements[i];
					stmCur.Instruction.Accept(this);
				}
			}

			public override void VisitAssignment(Assignment ass)
			{
				Identifier id = ass.Dst;
				MarkDefined(id);
				ass.Src.Accept(this);
			}

			public override void VisitStore(Store store)
			{
				var access = (MemoryAccess) store.Dst;
                var iBlock = ssa.RpoNumber(block);
                byte grf;
                defVars[iBlock].TryGetValue(access.MemoryId, out grf);
				grf = (byte)((grf & ~BitDeadIn) | BitDefined);
				defVars[iBlock][access.MemoryId] = grf;

				store.Dst.Accept(this);
				store.Src.Accept(this);
			}

			public override void VisitApplication(Application app)
			{
				app.Procedure.Accept(this);
				for (int i = app.Arguments.Length - 1; i >= 0; --i)
				{
                    app.Arguments[i].Accept(this);
				}
			}

			/// <summary>
			/// Unresolved calls can be "hell nodes". A hell node is an indirect calls or indirect
            /// jump that prior passes of the decompiler have been unable to resolve.
			/// </summary>
			/// <param name="ci"></param>
			/// <returns></returns>
			public override void VisitCallInstruction(CallInstruction ci)
			{
				// Hell node implementation - define all register variables.
				foreach (Identifier id in proc.Frame.Identifiers)
				{
					if (id.Storage is RegisterStorage || id.Storage is FlagGroupStorage)
					{
						MarkDefined(id);
					}
				}
			}

			/// <summary>
			/// Any uses of the identifier <paramref>id</paramref> 
			/// make it liveIn, and therefore no longer deadIn.
			/// </summary>
			/// <param name="id"></param>
			/// <returns></returns>
			public override void VisitIdentifier(Identifier id)
			{
                var dict = defVars[ssa.RpoNumber(block)];
                byte bits;
                dict.TryGetValue(id, out bits);
                dict[id] = (byte)(bits & unchecked((byte)~BitDeadIn));
			}

            public override void VisitOutArgument(OutArgument outArg)
            {
                Identifier id = outArg.Expression as Identifier;
                if (id != null)
                    MarkDefined(id);
                else
                    outArg.Expression.Accept(this);
            }
		}

		public class VariableRenamer : InstructionTransformer
		{
			private SsaTransform ssa;
			private int [] rename;		// most recently used name for var x.
			private int [] wasonentry;	// the name x had on entry into the block.
			private Statement stmCur; 
			private Procedure proc;

			/// <summary>
			/// Walks the dominator tree, renaming the different definitions of variables
			/// (including phi-functions). 
			/// </summary>
			/// <param name="ssa">SSA identifiers</param>
			/// <param name="p">procedure to rename</param>
			/// <param name="useSignature">if true, uses variables from procedure's signature.</param>
			public VariableRenamer(SsaTransform ssa, Identifier [] varsOrig, Procedure p)
			{
				this.ssa = ssa;
				this.rename = new int[varsOrig.Length];
				this.wasonentry = new int[varsOrig.Length];
				this.stmCur = null;
				this.proc = p;

				Block entryBlock = p.EntryBlock;
				Debug.Assert(entryBlock.Statements.Count == 0);
				for (int a = 0; a < rename.Length; ++a)
				{
					var id = ssa.SsaState.Identifiers.Add(ssa.varsOrig[a], null, null, false);
					rename[a] = a;

					// Variables that are used before defining are "predefined" by adding a 
                    // DefInstruction in the entry block for the procedure. Any such variables 
                    // that are found to be live correspond to the input parameters of the 
                    // procedure.

					id.DefStatement = new Statement(0, new DefInstruction(proc.Frame.Identifiers[a]), entryBlock);
					entryBlock.Statements.Add(id.DefStatement);
					wasonentry[a] = -1;
				}
			}

            private int RpoNumber(Block block)
            {
                return ssa.domGraph.ReversePostOrder[block];
            }

			/// <summary>
			/// Renames all variables in a block to use their SSA names
			/// </summary>
			/// <param name="n">Block to rename</param>
			public void RenameBlock(Block n)
			{
				int [] wasonentry = new int [rename.Length];
				rename.CopyTo(wasonentry, 0);

				// Rename variables in all blocks except the starting block which
				// only contains dummy 'def' variables.

				if (n != n.Procedure.EntryBlock)
				{
					foreach (Statement stm in n.Statements)
					{
						stmCur = stm;
						stmCur.Instruction.Accept(this);
					}
				}

				// Rename arguments to phi functions in successor blocks.

				bool [] visited = new bool[proc.ControlGraph.Blocks.Count];
				foreach (Block y in n.Succ)
				{
					for (int j = 0; j < y.Pred.Count; ++j)
					{
						if (y.Pred[j] == n && !visited[RpoNumber(y)])
						{
							visited[RpoNumber(y)] = true;

							// For each phi function in y...

							foreach (Statement stm in y.Statements)
							{
								stmCur = stm;
								PhiAssignment phi = stmCur.Instruction as PhiAssignment;
								if (phi != null)
								{
									PhiFunction p = (PhiFunction) phi.Src;
									// replace 'n's slot with the renamed name of the variable.
									p.Arguments[j] = 
										NewUse((Identifier) p.Arguments[j], stm);
								}
							}
						}
					}
				}
				foreach (Block c in ssa.domGraph.ReversePostOrder.Keys)
				{
					if (c != proc.EntryBlock && ssa.domGraph.ImmediateDominator(c) == n)
						RenameBlock(c);
				}
				wasonentry.CopyTo(rename, 0);
			}

			// Record the id that v was mapped to on entry to this block;

			private void EnsureWasOnEntry(int v, int id)
			{
				int cOld = wasonentry.Length;
				if (v >= cOld)
				{
					throw new ApplicationException("Array really big!");
				}
				else if (wasonentry[v] == -1)
				{
					wasonentry[v] = id;
				}	
			}

            // A new definition of id requires a new SSA name.
			private Identifier NewDef(Identifier idOld, Expression exprDef, bool isSideEffect)
			{
				var sid = ssa.SsaState.Identifiers.Add(idOld, stmCur, exprDef, isSideEffect);
				int iNew = Rename(idOld);
				rename[idOld.Number] = sid.Identifier.Number;
				EnsureWasOnEntry(idOld.Number, iNew);
				return sid.Identifier;
			}

			private Identifier NewUse(Identifier idOld, Statement stm)
			{
                int iNew = Rename(idOld);
				var sid = ssa.SsaState.Identifiers[iNew];
				sid.Uses.Add(stm);
				return sid.Identifier;
			}

            private int Rename(Identifier idOld)
            {
                return rename[idOld.Number];
            }

			public override Instruction TransformAssignment(Assignment ass)
			{
				ass.Src = ass.Src.Accept(this);
				Identifier id = (Identifier) ass.Dst;
				ass.Dst = NewDef(id, ass.Src, false);
				return ass;
			}

			public override Instruction TransformPhiAssignment(PhiAssignment phi)
			{
				// Only rename the defined variable in phi-functions.

				Identifier id = (Identifier) phi.Dst;
                phi.Dst = NewDef(id, phi.Src, false);
				return phi;
			}

			public override Instruction TransformCallInstruction(CallInstruction ci)
			{
                ci.Callee = ci.Callee.Accept(this);
				// Hell node implementation - use all register variables.

				foreach (Identifier id in proc.Frame.Identifiers)
				{
					if (id.Storage is RegisterStorage || id.Storage is FlagGroupStorage)
					{
						NewUse(id, stmCur);
					}
				}
				return ci;
			}

			public override Expression VisitApplication(Application appl)
			{
				for (int i = 0; i < appl.Arguments.Length; ++i)
				{
                    UnaryExpression unary = appl.Arguments[i] as UnaryExpression;
                    if (unary != null && unary.Operator == Operator.AddrOf)
                    {
                        Identifier id = unary.Expression as Identifier;
                        if (id != null)
                        {
                            unary.Expression = NewDef(id, appl, true);
                            continue;
                        }
                    }
                    appl.Arguments[i] = appl.Arguments[i].Accept(this);
				}
				return appl;
			}

            public override Expression VisitOutArgument(OutArgument outArg)
            {
                var id = outArg.Expression as Identifier;
                Expression exp;
                if (id != null)
                    exp = NewDef(id, outArg, true);
                else 
                    exp = outArg.Expression.Accept(this);
                return new OutArgument(outArg.DataType, exp);
            }

			public override Expression VisitIdentifier(Identifier id)
			{
				return NewUse(id, stmCur);
			}

			public override Instruction TransformStore(Store store)
			{
				store.Src = store.Src.Accept(this);

				var acc = store.Dst as MemoryAccess;
				if (acc != null)
				{
					acc.MemoryId = (MemoryIdentifier) NewDef(acc.MemoryId, store.Src, false);
					SegmentedAccess sa = acc as SegmentedAccess;
					if (sa != null)
						sa.BasePointer = sa.BasePointer.Accept(this);
					acc.EffectiveAddress = acc.EffectiveAddress.Accept(this);
				}
				else
				{
					store.Dst = store.Dst.Accept(this);
				}
				return store;
			}

			public override Expression VisitUnaryExpression(UnaryExpression unary)
			{
				unary.Expression = unary.Expression.Accept(this);
				return unary;
			}
		}
	}
}
