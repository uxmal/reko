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
using Decompiler.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Decompiler.Core.Procedure"/> to Static Single Assignment form.
	/// </summary>
	public class SsaTransform
	{
        private ProgramDataFlow programFlow;
		private Procedure proc;

		private const byte BitDefined = 1;
		private const byte BitDeadIn = 2;
		private const byte BitHasPhi = 4;
        private Dictionary<Expression, byte>[] AOrig;

		/// <summary>
		/// Constructs an SsaTransform, and in the process generates the SsaState for the procedure <paramref>proc</paramref>.
		/// </summary>
		/// <param name="proc"></param>
		/// <param name="gr"></param>
		public SsaTransform(ProgramDataFlow programFlow, Procedure proc, DominatorGraph<Block> gr)
		{
            this.programFlow = programFlow;
			this.proc = proc;
            this.SsaState = new SsaState(proc, gr);
            this.AOrig = CreateA();

			Transform();
		}

        public SsaState SsaState { get; private set; }
        public bool RenameFrameAccesses { get; set; }
        public bool AddUseInstructions { get; set; }

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

		private IEnumerable<Identifier> LocateAllDefinedVariables(Dictionary<Expression, byte>[] defOrig)
		{
			var ldv = new LocateDefinedVariables(this, defOrig);
			foreach (Block n in SsaState.DomGraph.ReversePostOrder.Keys)
			{
				ldv.LocateDefs(n);
			}
            return ldv.Definitions;
		}

		private void MarkTemporariesDeadIn(Dictionary<Expression, byte>[] def)
		{
            foreach (var block in proc.ControlGraph.Blocks)
            {
                int iBlock = SsaState.RpoNumber(block);
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
			var defVars = LocateAllDefinedVariables(AOrig);
			MarkTemporariesDeadIn(AOrig);

			// For each defined variable in block n, collect the places where it is defined

			foreach (var a in defVars)
			{
				// Create a worklist W of all the blocks that define a.

				var W = new WorkList<Block>();
                foreach (Block b in SsaState.DomGraph.ReversePostOrder.Keys) 
				{
                    byte bits;
                    AOrig[SsaState.RpoNumber(b)].TryGetValue(a, out bits);
					if ((bits & BitDefined) != 0)
						W.Add(b);
				}
                Block n;
                while (W.GetWorkItem(out n))
                {
                    foreach (Block y in SsaState.DomGraph.DominatorFrontier(n))
                    {
                        // Only add phi functions if there is no
                        // phi already and variable is not deadIn.

                        var dict = AOrig[SsaState.RpoNumber(y)];
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

        private Dictionary<Expression, byte>[] CreateA()
        {
            var a = new Dictionary<Expression, byte>[proc.ControlGraph.Blocks.Count];
            for (int i = 0; i < a.Length; ++i)
            {
                a[i] = new Dictionary<Expression, byte>();
            }
            return a;
        }

        private static Identifier EnsureStackVariable(Procedure proc, Expression effectiveAddress, DataType dt)
        {
            if (effectiveAddress == proc.Frame.FramePointer)
                return proc.Frame.EnsureStackVariable(0, dt);
            var bin = (BinaryExpression)effectiveAddress;
            var offset = ((Constant)bin.Right).ToInt32();
            if (bin.Operator == Operator.ISub)
                offset = -offset;
            var idFrame = proc.Frame.EnsureStackVariable(offset, dt);
            return idFrame;
        }

        private static bool IsFrameAccess(Procedure proc, Expression e)
        {
            if (e == proc.Frame.FramePointer)
            return true;
            var bin = e as BinaryExpression;
            if (bin == null)
                return false;
            if (bin.Left != proc.Frame.FramePointer)
                return false;
            return bin.Right is Constant;
        }

        public SsaState Transform()
		{
			PlacePhiFunctions();
			var rn = new VariableRenamer(this);
			rn.RenameBlock(proc.EntryBlock);
            return SsaState;
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
            private ProgramDataFlow programFlow;
            private Procedure proc;
			private Block block;
            private bool frameVariables;
            private Dictionary<Expression, byte>[] defVars; // variables defined by a statement.
			private Statement stmCur;
            private SsaState ssa;
            private List<Identifier> definitions;
            private HashSet<Identifier> inDefinitions;

			public LocateDefinedVariables(SsaTransform ssaXform, Dictionary<Expression, byte>[] defOrig)
			{
                this.programFlow = ssaXform.programFlow;
				this.proc = ssaXform.proc;
                this.ssa = ssaXform.SsaState;
                this.frameVariables = ssaXform.RenameFrameAccesses;
                this.defVars = defOrig;
                this.definitions = new List<Identifier>();
                this.inDefinitions = new HashSet<Identifier>();
			}

            public IEnumerable<Identifier> Definitions { get { return definitions; } }

			private void MarkDefined(Identifier eDef)
			{
                SsaIdentifier sid;
                var idDef = eDef as Identifier;
                if (idDef != null && ssa.Identifiers.TryGetValue(idDef, out sid))
                {
                    // If we've seen this identifier before, use its
                    // original name.
                    eDef = sid.OriginalIdentifier;
                }
                var dict = defVars[ssa.RpoNumber(block)];
                byte bits;
                dict.TryGetValue(eDef, out bits);
				dict[eDef] = (byte) (bits | (BitDefined | BitDeadIn));
                if (!inDefinitions.Contains(eDef))
                {
                    inDefinitions.Add(eDef);
                    definitions.Add(eDef);
                }
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

                if (this.frameVariables && IsFrameAccess(proc, access.EffectiveAddress))
                {
                    var idFrame = EnsureStackVariable(proc, access.EffectiveAddress, access.DataType);
                    MarkDefined(idFrame);
                }
                else
                {
                    store.Dst.Accept(this);
                }
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
                Procedure callee = GetUserProcedure(ci);
                ProcedureFlow2 flow;
                if (callee != null && programFlow.ProcedureFlows2.TryGetValue(callee, out flow))
                {
                    foreach (var def in flow.Trashed)
                    {
                        var idDef = callee.Frame.EnsureIdentifier(def);
                        MarkDefined(idDef);
                    }
                }
                else
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
			}

            private Procedure GetUserProcedure(CallInstruction ci)
            {
                var pc = ci.Callee as ProcedureConstant;
                if (pc == null)
                    return null;
                return pc.Procedure as Procedure;
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
            private ProgramDataFlow programFlow;
            private SsaState ssa;
            private bool renameFrameAccess;
            private bool addUseInstructions;
			private Dictionary<Identifier, Identifier> rename;		// most recently used name for var x.
			private Statement stmCur; 
			private Procedure proc;
            private HashSet<Expression> existingDefs;

			/// <summary>
			/// Walks the dominator tree, renaming the different definitions of variables
			/// (including phi-functions). 
			/// </summary>
			/// <param name="ssa">SSA identifiers</param>
			/// <param name="p">procedure to rename</param>
			public VariableRenamer(SsaTransform ssaXform)
			{
                this.programFlow = ssaXform.programFlow;
				this.ssa = ssaXform.SsaState;
                this.renameFrameAccess = ssaXform.RenameFrameAccesses;
                this.addUseInstructions = ssaXform.AddUseInstructions;
                this.proc = ssaXform.proc;
				this.rename = new Dictionary<Identifier, Identifier>();
				this.stmCur = null;
                this.existingDefs = proc.EntryBlock.Statements
                    .Select(s => s.Instruction as DefInstruction)
                    .Where(d => d != null)
                    .Select(d => d.Expression)
                    .ToHashSet();
			}

            /// <summary>
            /// Variables that are used before defining are "predefined" by adding a 
            /// DefInstruction in the entry block for the procedure. Any such variables 
            /// that are found to be live correspond to the input parameters of the 
            /// procedure.
            /// </summary>
            /// <param name="id"></param>
            private SsaIdentifier UsedBeforeDefined(Identifier id)
            {
                Block entryBlock = proc.EntryBlock;
                rename[id] = id;
                if (!existingDefs.Contains(id))
                {
                    var sid = this.ssa.Identifiers.Add(id, null, null, false);
                    sid.DefStatement = new Statement(0, new DefInstruction(id), entryBlock);
                    entryBlock.Statements.Add(sid.DefStatement);
                    existingDefs.Add(id);
                    return sid;
                }
                else
                {
                    return ssa.Identifiers[id];
                }
            }

			/// <summary>
			/// Renames all variables in a block to use their SSA names
			/// </summary>
			/// <param name="n">Block to rename</param>
			public void RenameBlock(Block n)
			{
				var wasonentry = new Dictionary<Identifier, Identifier>(rename);

				// Rename variables in all blocks except the starting block which
				// only contains dummy 'def' variables.

				if (n != n.Procedure.EntryBlock)
				{
					foreach (Statement stm in n.Statements)
					{
						stmCur = stm;
						stmCur.Instruction = stmCur.Instruction.Accept(this);
					}
                    if (n == n.Procedure.ExitBlock && this.addUseInstructions)
                        AddUseInstructions(n);
				}

				// Rename arguments to phi functions in successor blocks.

				bool [] visited = new bool[proc.ControlGraph.Blocks.Count];
				foreach (Block y in n.Succ)
				{
					for (int j = 0; j < y.Pred.Count; ++j)
					{
						if (y.Pred[j] == n && !visited[ssa.RpoNumber(y)])
						{
							visited[ssa.RpoNumber(y)] = true;

							// For each phi function in y...

							foreach (Statement stm in y.Statements.Where(s => s.Instruction is PhiAssignment))
							{
								stmCur = stm;
								PhiAssignment phi = (PhiAssignment) stmCur.Instruction;
								PhiFunction p = phi.Src;
								// replace 'n's slot with the renamed name of the variable.
								p.Arguments[j] = 
									NewUse((Identifier)p.Arguments[j], stm);
							}
						}
					}
				}
				foreach (Block c in ssa.DomGraph.ReversePostOrder.Keys)
				{
					if (c != proc.EntryBlock && ssa.DomGraph.ImmediateDominator(c) == n)
						RenameBlock(c);
				}
				rename = wasonentry;
			}

            /// <summary>
            /// Adds a UseInstruction for each SsaIdentifier.
            /// </summary>
            /// <remarks>
            /// Doing this will allow us to detect what definitions reach the end of the function.
            /// //$TODO: what about functions that don't terminate, or have branches that don't terminate? In such cases,
            /// the identifiers should be removed.</remarks>
            /// <param name="block"></param>
            private void AddUseInstructions(Block block)
            {
                var existing = block.Statements
                    .Select(s => s.Instruction as UseInstruction)
                    .Where(u => u != null)
                    .Select(u => u.Expression)
                    .ToHashSet();
                block.Statements.AddRange(rename.Values
                    .Where(id => !existing.Contains(id))
                    .OrderBy(id => id.Name)     // Sort them for stability; unit test are sensitive to shifting order 
                    .Select(id => new Statement(0, new UseInstruction(id), block)));
            }

            private void AddUseInstructions(CallInstruction ci)
            {
                var existing = ci.Uses.Select(u => u.Expression).ToHashSet();
                ci.Uses.UnionWith(rename.Values
                    .Where(id => !existing.Contains(id))
                    .Select(id => new UseInstruction(id)));
            }

            private void AddDefInstructions(CallInstruction ci, ProcedureFlow2 flow)
            {
                var existing = ci.Definitions.Select(d => ssa.Identifiers[(Identifier)d.Expression].OriginalIdentifier).ToHashSet();
                var ab = new ApplicationBuilder(null, proc.Frame, null, null, null, true);
                foreach (var idDef in flow.Trashed)
                {
                    var idLocal = proc.Frame.EnsureIdentifier(idDef);
                    if (!existing.Contains(idLocal))
                    { 
                        ci.Definitions.Add(new DefInstruction(idLocal));
                    }
                }
                foreach (var def in ci.Definitions)
                {
                    var idNew = NewDef((Identifier) def.Expression, null, false);
                    def.Expression = idNew;
                }
            }

            /// <summary>
            /// Called when a new definition of a location is encountered.
            /// A new definition of id requires a new SSA name.
            /// </summary>
            /// <param name="idOld">The expression we wish to replace with an SSA name.</param>
            /// <param name="exprDef">The defining expression of idOld</param>
            /// <param name="isSideEffect">False if this is a traditional assignment, true if it is an
            /// out parameter of a function call.</param>
            /// <returns>The identifier of the new SSA identifier</returns>
			private Identifier NewDef(Identifier idOld, Expression exprDef, bool isSideEffect)
			{
                SsaIdentifier sidOld;
                if (idOld != null && ssa.Identifiers.TryGetValue(idOld, out sidOld))
                {
                    if (sidOld.OriginalIdentifier != sidOld.Identifier)
                    {
                        rename[sidOld.OriginalIdentifier] = sidOld.Identifier;
                        return sidOld.Identifier;
                    }
                }
				var sid = ssa.Identifiers.Add(idOld, stmCur, exprDef, isSideEffect);
				rename[idOld] = sid.Identifier;
				return sid.Identifier;
			}

			private Identifier NewUse(Identifier idOld, Statement stm)
			{
                SsaIdentifier sid;
                if (ssa.Identifiers.TryGetValue(idOld, out sid))
                {
                    idOld = sid.OriginalIdentifier;
                }
                Identifier idNew;
                if (!this.rename.TryGetValue(idOld, out idNew))
                {
                    // A use before a definition! This identifier
                    // must be live-in to the procedure.
                    sid = UsedBeforeDefined(idOld);
                }
                else
                {
                    // Seen it before, use the most recent
                    // renamed version of the identifier.
                    sid = ssa.Identifiers[idNew];
                }
				sid.Uses.Add(stm);
				return sid.Identifier;
			}

			public override Instruction TransformAssignment(Assignment ass)
			{
				ass.Src = ass.Src.Accept(this);
				Identifier id = ass.Dst;
				ass.Dst = NewDef(id, ass.Src, false);
				return ass;
			}

			public override Instruction TransformPhiAssignment(PhiAssignment phi)
			{
				// Only rename the defined variable in phi-functions.
                // The arguments of the phi-function will be renamed 
                // elsewhere.

				Identifier id = phi.Dst;
                phi.Dst = NewDef(id, phi.Src, false);
				return phi;
			}

			public override Instruction TransformCallInstruction(CallInstruction ci)
			{
                ci.Callee = ci.Callee.Accept(this);
                ProcedureConstant pc;
                if (ci.Callee.As(out pc))
                {
                    var procCallee = pc.Procedure as Procedure;
                    ProcedureFlow2 procFlow;
                    if (procCallee != null && programFlow.ProcedureFlows2.TryGetValue(procCallee, out procFlow))
                    {
                        AddUseInstructions(ci);
                        AddDefInstructions(ci, procFlow);
                    }
                    return ci;
                }

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

            public override Expression VisitMemoryAccess(MemoryAccess access)
            {
                if (this.renameFrameAccess && IsFrameAccess(proc, access.EffectiveAddress))
                {
                    var idFrame = EnsureStackVariable(proc, access.EffectiveAddress, access.DataType);
                    var idNew = NewUse(idFrame, stmCur);
                    return idNew;
                }
                return base.VisitMemoryAccess(access);
            }

            public override Expression VisitSegmentedAccess(SegmentedAccess access)
            {
                if (this.renameFrameAccess && IsFrameAccess(proc, access.EffectiveAddress))
                {
                    var idFrame = EnsureStackVariable(proc, access.EffectiveAddress, access.DataType);
                    var idNew = NewUse(idFrame, stmCur);
                    return idNew;
                }
                return base.VisitSegmentedAccess(access);
            }

			public override Instruction TransformStore(Store store)
			{
				store.Src = store.Src.Accept(this);

				var acc = store.Dst as MemoryAccess;
				if (acc != null)
				{
                    if (this.renameFrameAccess && IsFrameAccess(proc, acc.EffectiveAddress))
                    {
                        var idFrame = EnsureStackVariable(proc, acc.EffectiveAddress, acc.DataType);
                        var idDst = NewDef(idFrame, store.Src, false);
                        return new Assignment(idDst, store.Src);
                    }
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

            public override Instruction TransformUseInstruction(UseInstruction u)
            {
                if (u.OutArgument != null)
                    u.OutArgument = UsedBeforeDefined(u.OutArgument).Identifier;
                return base.TransformUseInstruction(u);
            }
		}
    }
}
