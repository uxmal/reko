/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using System;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Decompiler.Core.Procedure"/> to Static Single Assignment.
	/// </summary>
	public class SsaTransform
	{
		private SsaState ssa;
		private Identifier [] varsOrig;
		private Procedure proc;
		private DominatorGraph domGraph;

		private const byte BitDefined = 1;
		private const byte BitDeadIn = 2;
		private const byte BitHasPhi = 4;

		/// <summary>
		/// Constructs an SsaTransform, and in the process generates the SsaState for the procedure <paramref>proc</paramref>.
		/// </summary>
		/// <param name="proc"></param>
		/// <param name="gr"></param>
		/// <param name="useSignatures">if true, the signature of <paramref>proc</paramref> is used as a source of variables.</param>
		public SsaTransform(Procedure proc, DominatorGraph gr, bool useSignatures)
		{
			this.proc = proc;
			this.domGraph = gr;
			this.varsOrig = new Identifier[proc.Frame.Identifiers.Count];
			proc.Frame.Identifiers.CopyTo(varsOrig);

			Transform(useSignatures);
		}

		/// <summary>
		/// Creates a phi statement with slots for each predecessor block, then
		/// inserts the phi statement as the first statement of the block.
		/// </summary>
		/// <param name="b">Block into which the phi statement is inserted</param>
		/// <param name="v">Destination variable for the phi assignment</param>
		/// <returns>The inserted phi Assignment</returns>
		private Instruction InsertPhiStatement(Block b, int v)
		{
			Statement stm = new Statement(
				new PhiAssignment(varsOrig[v], b.Pred.Count),
				b);
			b.Statements.Insert(0, stm);
			return stm.Instruction;
		}

		
		public SsaState SsaState
		{
			get { return ssa; }
		}


		private void LocateAllDefinedVariables(byte [,] defOrig)
		{
			LocateDefinedVariables ldv = new LocateDefinedVariables(proc, defOrig);
			foreach (Block n in proc.RpoBlocks)
			{
				ldv.LocateDefs(n);
			}
		}

		private void MarkTemporariesDeadIn(byte [,] def)
		{
			foreach (Identifier id in proc.Frame.Identifiers)
			{
				if (id.Storage is TemporaryStorage)
				{
					for (int i = 0; i < proc.RpoBlocks.Count; ++i)
					{
						def[id.Number, i] |= BitDeadIn;
					}
				}
			}
		}

		private void PlacePhiFunctions()
		{
			byte [,] AOrig = new byte[varsOrig.Length, proc.RpoBlocks.Count];	
			LocateAllDefinedVariables(AOrig);
			MarkTemporariesDeadIn(AOrig);

			// For each defined variable in block n, collect the places where it is defined

			for (int a = 0; a < varsOrig.Length; ++a)
			{
				// Create a worklist W of all the blocks that define a.

				WorkList<Block> W = new WorkList<Block>();
				foreach (Block b in proc.RpoBlocks)
				{
					if ((AOrig[a, b.RpoNumber] & BitDefined) != 0)
						W.Add(b);
				}
                Block n;
                while (W.GetWorkItem(out n))
                {
                    foreach (Block y in domGraph.DominatorFrontier(n))
                    {
                        // Only add phi functions if theere is no
                        // phi already and variable is not deadIn.

                        if ((AOrig[a, y.RpoNumber] & (BitHasPhi | BitDeadIn)) == 0)
                        {
                            AOrig[a, y.RpoNumber] |= BitHasPhi;
                            InsertPhiStatement(y, a);
                            if ((AOrig[a, y.RpoNumber] & BitDefined) == 0)
                            {
                                W.Add(y);
                            }
                        }
                    }
                }
			}
		}

		public void Transform(bool useSignature)
		{
			ssa = new SsaState(proc);
			PlacePhiFunctions();
			RenameVariables rn = new RenameVariables(this, proc, useSignature);
			rn.RenameBlock(proc.EntryBlock);
		}



		/// <summary>
		/// Locates the variables defined in this block by examining each
		/// statement to find variables in L-value positions.
		/// In addition, the set deadIn for each block is calculated.
		/// These are all the variables that are known to be dead on
		/// entry to the function. Dead variables won't need phi code!
		/// </summary>
		private class LocateDefinedVariables : InstructionVisitorBase
		{
			private Procedure proc;
			private Block block;
			private byte [,] defVars;		// variables defined by a statement.
			private Statement stmCur;

			public LocateDefinedVariables(Procedure proc, byte [,] defOrig)
			{
				this.proc = proc;
				defVars = defOrig;
			}

			private void MarkDefined(Identifier id)
			{
				Debug.Assert(id.Number >= 0);
				defVars[id.Number, block.RpoNumber] |= (BitDefined | BitDeadIn);
			}

			public void LocateDefs(Block b)
			{
				block = b;
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
				MemoryAccess access = (MemoryAccess) store.Dst;
				int grf = defVars[access.MemoryId.Number, block.RpoNumber];
				grf = (grf & ~BitDeadIn) | BitDefined;
				defVars[access.MemoryId.Number, block.RpoNumber] = (byte) grf;

				store.Dst.Accept(this);
				store.Src.Accept(this);
			}

			public override void VisitApplication(Application app)
			{
				app.Procedure.Accept(this);
				for (int i = app.Arguments.Length - 1; i >= 0; --i)
				{
					Expression exp = app.Arguments[i];
					UnaryExpression u = exp as UnaryExpression;
					if (u != null && u.op == Operator.AddrOf)
					{
						Identifier id = u.Expression as Identifier;
						if (id != null)
							MarkDefined(id);
						else
							u.Expression.Accept(this);
					}
					else
					{
						exp.Accept(this);
					}
				}
			}

			/// <summary>
			/// Unresolved calls are "hell nodes"
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
				defVars[id.Number, block.RpoNumber] &= unchecked((byte)~BitDeadIn);
			}
		}

		private class RenameVariables : InstructionTransformer
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
			public RenameVariables(SsaTransform ssa, Procedure p, bool useSignature)
			{
				this.ssa = ssa;
				rename = new int[ssa.varsOrig.Length];
				wasonentry = new int[ssa.varsOrig.Length];
				stmCur = null;
				proc = p;

				Block entryBlock = p.EntryBlock;
				Debug.Assert(entryBlock.Statements.Count == 0);
				for (int a = 0; a < rename.Length; ++a)
				{
					SsaIdentifier id = ssa.SsaState.Identifiers.Add(ssa.varsOrig[a], null, null, false);
					rename[a] = a;

					// Variables that are used before defining are "predefined" in the 
					// dummy entry block.

					id.DefStatement = new Statement(new DefInstruction(proc.Frame.Identifiers[a]), entryBlock);
					entryBlock.Statements.Add(id.DefStatement);
					wasonentry[a] = -1;
				}
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

				if (n.RpoNumber != 0)
				{
					foreach (Statement stm in n.Statements)
					{
						stmCur = stm;
						stmCur.Instruction.Accept(this);
					}
				}

				// Rename arguments to phi functions in successor blocks.

				bool [] visited = new bool[proc.RpoBlocks.Count];
				foreach (Block y in n.Succ)
				{
					for (int j = 0; j < y.Pred.Count; ++j)
					{
						if (y.Pred[j] == n && !visited[y.RpoNumber])
						{
							visited[y.RpoNumber] = true;

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
				foreach (Block c in proc.RpoBlocks)
				{
					if (c.RpoNumber != 0 && ssa.domGraph.ImmediateDominator(c) == n)
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

			private Identifier NewDef(Identifier idOld, Expression exprDef, bool isSideEffect)
			{
				// A new definition of id requires a new SSA name.

				SsaIdentifier sid = ssa.SsaState.Identifiers.Add(idOld, stmCur, exprDef, isSideEffect);
				int idPrev = rename[idOld.Number];
				rename[idOld.Number] = sid.Identifier.Number;
				EnsureWasOnEntry(idOld.Number, idPrev);
				return sid.Identifier;
			}


			private Identifier NewUse(Identifier idOld, Statement stm)
			{
				int iNew = rename[idOld.Number];
				SsaIdentifier id = ssa.SsaState.Identifiers[iNew];
				id.Uses.Add(stm);
				return id.Identifier;
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


			public override Expression TransformApplication(Application appl)
			{
				for (int i = 0; i < appl.Arguments.Length; ++i)
				{
                    UnaryExpression unary = appl.Arguments[i] as UnaryExpression;
                    if (unary != null && unary.op == Operator.AddrOf)
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


			public override Expression TransformIdentifier(Identifier id)
			{
				return NewUse(id, stmCur);
			}

			public override Instruction TransformStore(Store store)
			{
				store.Src = store.Src.Accept(this);

				MemoryAccess acc = store.Dst as MemoryAccess;
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


			public override Expression TransformUnaryExpression(UnaryExpression unary)
			{
				unary.Expression = unary.Expression.Accept(this);
				return unary;
			}
		}
	}
}
