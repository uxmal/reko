#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;

namespace Reko.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment form.
	/// </summary>
    /// <remarks>
    /// This class implements the SSA algorithm from Appel's "Modern compiler 
    /// implementatation in [language of your choice]."
    /// </remarks>
	public class SsaTransform
	{
        private ProgramDataFlow programFlow;
		private Procedure proc;
        private IImportResolver importResolver;
        private HashSet<RegisterStorage> implicitRegs;

        private const byte BitDefined = 1;
		private const byte BitDeadIn = 2;
		private const byte BitHasPhi = 4;
        private Dictionary<Expression, byte>[] AOrig;

		/// <summary>
		/// Constructs an SsaTransform, and in the process generates the SsaState for the procedure <paramref>proc</paramref>.
		/// </summary>
		/// <param name="proc"></param>
		/// <param name="gr"></param>
		public SsaTransform(ProgramDataFlow programFlow, Procedure proc, IImportResolver importResolver, DominatorGraph<Block> gr, HashSet<RegisterStorage> implicitRegs)
		{
            this.programFlow = programFlow;
			this.proc = proc;
            this.importResolver = importResolver;
            this.implicitRegs = implicitRegs;
            this.SsaState = new SsaState(proc, gr);
            this.AOrig = CreateA();

			Transform();
		}

        /// <summary>
        /// Simple transform just builds the SsaGraph from all the identifiers.
        /// </summary>
        /// <param name="proc"></param>
        public SsaTransform(Procedure proc)
        {
            this.proc = proc;
            this.SsaState = new SsaState(proc, null);
            var str = new SimpleTransformer(SsaState);
            str.Transform();
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
		private Statement InsertPhiStatement(Block b, Identifier v)
		{
			var stm = new Statement(
                b.Address.ToLinear(),
				new PhiAssignment(v, b.Pred
                    .Select(p => new PhiArgument(p, v))
                    .ToArray()),
				b);
			b.Statements.Insert(0, stm);
			return stm;
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

        /// <summary>
        /// Temporary variables are never live-in, so we avoid getting phi
        /// functions all over the place by marking them explicitly as dead-in.
        /// </summary>
        /// <param name="def"></param>
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

		private HashSet<Statement> PlacePhiFunctions()
		{
            HashSet<Statement> phiStatements = new HashSet<Statement>();
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
                            var stm = InsertPhiStatement(y, a);
                            phiStatements.Add(stm);
                            if ((bits & BitDefined) == 0)
                            {
                                W.Add(y);
                            }
                        }
                    }
                }
			}
            return phiStatements;
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
			var newPhiStatements = PlacePhiFunctions();
			var rn = new VariableRenamer(this, newPhiStatements);
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
            private HashSet<RegisterStorage> implicitRegs;
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
                this.implicitRegs = ssaXform.implicitRegs;
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
                switch (store.Dst)
                {
                case MemoryAccess access:
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
                    break;
                case ArrayAccess aaccess:
                    aaccess.Array.Accept(this);
                    aaccess.Index.Accept(this);
                    store.Src.Accept(this);
                    break;
                }
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
                    if (ci.Uses.Count > 0 || ci.Definitions.Count > 0)
                        return;
                    foreach (Identifier id in proc.Frame.Identifiers)
                    {
                        if (id.Storage is TemporaryStorage)
                        {
                            continue;
                        }
                        var reg = id.Storage as RegisterStorage;
                        if (reg != null)
                        {
                            if (implicitRegs.Contains(reg))
                                continue;
                        }
                        else if (
                            !(id.Storage is FlagGroupStorage) &&
                            !(id.Storage is FpuStackStorage))
                        {
                            continue;
                        }
                        ci.Definitions.Add(new DefInstruction(id));
                        MarkDefined(id);
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
            private HashSet<RegisterStorage> implicitRegs;
            private ProgramDataFlow programFlow;
            private SsaState ssa;
            private bool renameFrameAccess;
            private bool addUseInstructions;
			private Dictionary<Identifier, Identifier> rename;		// most recently used name for var x.
			private Statement stmCur; 
			private Procedure proc;
            private IImportResolver importResolver;
            private HashSet<Identifier> existingDefs;
            private HashSet<Statement> newPhiStatements;
            private int recursionGuard;
            private int defPos;

            /// <summary>
            /// Walks the dominator tree, renaming the different definitions of variables
            /// (including phi-functions). 
            /// </summary>
            /// <param name="ssa">SSA identifiers</param>
            /// <param name="newPhiStatements">
            /// Phi statements added during current pass of SsaTransform. Used
            /// to avoid extra use of identifiers in existing phi assignments
            /// </param>
            public VariableRenamer(SsaTransform ssaXform, HashSet<Statement> newPhiStatements)
			{
                this.programFlow = ssaXform.programFlow;
				this.ssa = ssaXform.SsaState;
                this.implicitRegs = ssaXform.implicitRegs;
                this.renameFrameAccess = ssaXform.RenameFrameAccesses;
                this.addUseInstructions = ssaXform.AddUseInstructions;
                this.proc = ssaXform.proc;
                this.importResolver = ssaXform.importResolver;
				this.rename = new Dictionary<Identifier, Identifier>();
				this.stmCur = null;
                this.existingDefs = proc.EntryBlock.Statements
                    .Select(s => s.Instruction as DefInstruction)
                    .Where(d => d != null)
                    .Select(d => d.Identifier)
                    .ToHashSet();
                this.newPhiStatements = newPhiStatements;
                this.defPos = LastDefIndex(proc.EntryBlock.Statements) + 1;
			}

            private int LastDefIndex(StatementList stmts)
            {
                for (int i = stmts.Count-1; i >= 0; --i)
                {
                    if (stmts[i].Instruction is DefInstruction)
                        return i;
                }
                return -1;
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
                    sid.DefStatement = new Statement(
                        proc.EntryAddress.ToLinear(),
                        new DefInstruction(id),
                        entryBlock);
                    entryBlock.Statements.Insert(defPos, sid.DefStatement);
                    ++defPos;
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
                if (this.recursionGuard > 1000)
                {
                    Debug.Print("Stopping recursion in SsaTransform.RenameBlock");
                    return;
                }
                ++this.recursionGuard;
				var wasonentry = new Dictionary<Identifier, Identifier>(rename);

                // Rename variables in all blocks except the dummy 'def' variables in 
                // the starting block.
				foreach (Statement stm in n.Statements.ToArray())
				{
					stmCur = stm;
					stmCur.Instruction = stmCur.Instruction.Accept(this);
				}
                if (n == n.Procedure.ExitBlock && this.addUseInstructions)
                    AddUseInstructions(n);

				// Rename arguments to phi functions in successor blocks.

				foreach (Block y in n.Succ.Distinct())
				{
					for (int j = 0; j < y.Pred.Count; ++j)
					{
						if (y.Pred[j] == n)
						{
							// For each phi function in y...

							foreach (Statement stm in y.Statements.Where(s => s.Instruction is PhiAssignment))
							{
                                var newPhi = newPhiStatements.Contains(stm);
								stmCur = stm;
								PhiAssignment phi = (PhiAssignment) stmCur.Instruction;
								PhiFunction p = phi.Src;
                                // replace 'n's slot with the renamed name of the variable.
                                var value = NewUse((Identifier) p.Arguments[j].Value, stm, newPhi);
                                p.Arguments[j] = new PhiArgument(
                                    p.Arguments[j].Block,
									value);
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
                --this.recursionGuard;
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
                var stms = rename.Values
                    .Where(id => !existing.Contains(id) &&
                                 !(id.Storage is StackArgumentStorage))
                    .OrderBy(id => id.Name)     // Sort them for stability; unit test are sensitive to shifting order 
                    .Select(id => new Statement(
                        block.Address.ToLinear(),
                        new UseInstruction(id), block))
                    .ToList();
                block.Statements.AddRange(stms);
                stms.ForEach(u =>
                {
                    var use = (UseInstruction)u.Instruction;
                    use.Expression = NewUse((Identifier)use.Expression, u, true);
                });
            }

            private void AddDefInstructions(CallInstruction ci, ProcedureFlow2 flow)
            {
                var existing = ci.Definitions.Select(d => ssa.Identifiers[(Identifier)d.Identifier].OriginalIdentifier).ToHashSet();
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
                    var idNew = NewDef((Identifier) def.Identifier, null, false);
                    def.Identifier = idNew;
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

			private Identifier NewUse(Identifier idOld, Statement stm, bool force)
			{
                var idBefore = idOld;
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
                if (renameFrameAccess && !force)
                    return idBefore;
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

                if (ci.Callee is ProcedureConstant pc)
                {
                    if (pc.Procedure is Procedure procCallee &&
                        programFlow.ProcedureFlows2.TryGetValue(procCallee, out ProcedureFlow2 procFlow))
                    {
                        AddDefInstructions(ci, procFlow);
                        return ci;
                    }
                }

                RenameAllRegisterIdentifiers(ci);
                return ci;
            }

            /// <summary>
            /// Because we don't have a proper signature for the callee, we're forced to
            /// guess. Use all registers and flags in this procedure.
            /// </summary>
            /// <param name="ci"></param>
            private void RenameAllRegisterIdentifiers(CallInstruction ci)
            {
                // Hell node implementation - use all register variables that
                // aren't implicit on this platform.

                var alreadyExistingUses = ci.Uses.Select(u => ssa.Identifiers[(Identifier)u.Expression].OriginalIdentifier).ToHashSet();
                foreach (Identifier id in ssa.Identifiers.Select(s => s.OriginalIdentifier).Distinct().ToList())
                {
                    if (IsMutableRegister(id.Storage) || id.Storage is FlagGroupStorage ||
                        id.Storage is StackLocalStorage)
                    {
                        if (!alreadyExistingUses.Contains(id))
                        {
                            alreadyExistingUses.Add(id);
                            var newId = NewUse(id, stmCur, true);
                            ci.Uses.Add(new UseInstruction(newId));
                        }
                    }
                }
                foreach (DefInstruction def in ci.Definitions)
                {
                    var id = (Identifier)def.Identifier;
                    if (IsMutableRegister(id.Storage) || id.Storage is FlagGroupStorage ||
                        id.Storage is StackLocalStorage ||
                        id.Storage is FpuStackStorage)
                    {
                        def.Identifier = NewDef(id, null, false);
                    }
                }
            }

            private bool IsMutableRegister(Storage storage)
            {
                var reg = storage as RegisterStorage;
                return reg != null && !implicitRegs.Contains(reg);
            }

            public override Expression VisitApplication(Application appl)
			{
                appl.Procedure = appl.Procedure.Accept(this);
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
                return NewUse(id, stmCur, false);
			}

            public override Expression VisitMemoryAccess(MemoryAccess access)
            {
                if (this.renameFrameAccess && IsFrameAccess(proc, access.EffectiveAddress))
                {
                    ssa.Identifiers[proc.Frame.FramePointer].Uses.Remove(stmCur);
                    ssa.Identifiers[access.MemoryId].Uses.Remove(stmCur);
                    var idFrame = EnsureStackVariable(proc, access.EffectiveAddress, access.DataType);
                    var idNew = NewUse(idFrame, stmCur, true);
                    return idNew;
                }
                Constant c = null;
                var ea = access.EffectiveAddress.Accept(this);
                if (ea is BinaryExpression bin &&
                    bin.Left is Identifier id && 
                    bin.Right is Constant cRight && 
                    rename.ContainsKey(id))
                {
                    c = cRight;
                    var sid = ssa.Identifiers[rename[id]];
                    var cOther = sid.DefExpression as Constant;
                    if (cOther != null)
                    {
                        c = bin.Operator.ApplyConstants(cOther, c);
                    }
                }
                else
                {
                    c = ea as Constant;
                }

                if (c != null)
                {
                    var e = importResolver.ResolveToImportedValue(stmCur, c);
                    if (e != null)
                        return e;
                }
                var memId = (MemoryIdentifier)access.MemoryId.Accept(this);
                return new MemoryAccess(memId, ea, access.DataType);
            }

            public override Expression VisitSegmentedAccess(SegmentedAccess access)
            {
                if (this.renameFrameAccess && IsFrameAccess(proc, access.EffectiveAddress))
                {
                    var idFrame = EnsureStackVariable(proc, access.EffectiveAddress, access.DataType);
                    var idNew = NewUse(idFrame, stmCur, true);
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
                        ssa.Identifiers[proc.Frame.FramePointer].Uses.Remove(stmCur);
                        ssa.Identifiers[acc.MemoryId].DefStatement = null;
                        var idFrame = EnsureStackVariable(proc, acc.EffectiveAddress, acc.DataType);
                        var idDst = NewDef(idFrame, store.Src, false);
                        return new Assignment(idDst, store.Src);
                    }
                    var memId = (MemoryIdentifier) NewDef(acc.MemoryId, store.Src, false);
                    Expression basePtr = null;
                    if (acc is SegmentedAccess sa)
                    {
                        basePtr = sa.BasePointer.Accept(this);
                    }
                    var ea = acc.EffectiveAddress.Accept(this);
                    if (basePtr != null)
                    {
                        store.Dst = new SegmentedAccess(memId, basePtr, ea, acc.DataType);
                    }
                    else
                    {
                        store.Dst = new MemoryAccess(memId, ea, acc.DataType);
                    }
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

        class SimpleTransformer : InstructionTransformer
        {
            private SsaState ssa;
            private Statement stmCur;

            public SimpleTransformer(SsaState ssa)
            {
                this.ssa = ssa;
            }

            public void Transform()
            {
                var sig = ssa.Procedure.Signature;
                for (int i = 0; i < sig.Parameters.Length; ++i)
                {
                    sig.Parameters[i] = Def(sig.Parameters[i], null);
                }
                foreach (var stm in ssa.Procedure.Statements)
                {
                    this.stmCur = stm;
                    stm.Instruction = stm.Instruction.Accept(this);
                }
            }

            public override Instruction TransformAssignment(Assignment a)
            {
                a.Src = a.Src.Accept(this);
                a.Dst = Def(a.Dst, a.Src);
                return a;
            }

            public override Instruction TransformDeclaration(Declaration decl)
            {
                if (decl.Expression != null)
                {
                    decl.Expression = decl.Expression.Accept(this);
                }
                decl.Identifier = Def(decl.Identifier, decl.Expression);
                return decl;
            }

            public override Instruction TransformPhiAssignment(PhiAssignment phi)
            {
                phi.Src = (PhiFunction)phi.Src.Accept(this);
                phi.Dst = Def(phi.Dst, phi.Src);
                return phi;
            }

            public override Expression VisitIdentifier(Identifier id)
            {
                return Use(id);
            }

            private Identifier Use(Identifier idOld)
            {
                SsaIdentifier sid;
                if (!ssa.Identifiers.TryGetValue(idOld, out sid))
                {
                    sid = new SsaIdentifier(idOld, idOld, stmCur, null, false);
                    ssa.Identifiers.Add(idOld, sid);
                }
                ssa.Identifiers[idOld].Uses.Add(stmCur);
                return idOld;
            }

            private Identifier Def(Identifier idOld, Expression expr)
            {
                SsaIdentifier sid;
                if (ssa.Identifiers.TryGetValue(idOld, out sid))
                {
                    sid.DefExpression = expr;
                }
                else
                {
                    sid = new SsaIdentifier(idOld, idOld, stmCur, expr, false);
                    ssa.Identifiers.Add(idOld, sid);
                }
                return idOld;
            }
        }
    }

	/// <summary>
	/// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment
    /// form.
	/// </summary>
    /// <remarks>
    /// EXPERIMENTAL - consult uxmal before using
    /// 
    /// This class implements another SSA algorithm that doesn't require 
    /// calculation of the dominator graph. It is expected that when it is fully
    /// implemented, it will take over from SsaTransform above.
    /// </remarks>
    public class SsaTransform2 : InstructionTransformer 
    {
        private Block block;
        private Statement stm;
        private Dictionary<Block, Dictionary<Storage, SsaIdentifier>> currentDef;
        private Dictionary<Block, Dictionary<Storage, SsaIdentifier>> incompletePhis;
        private HashSet<Block> sealedBlocks;
        private SsaState ssa;
        private AliasState asta;

        public void Transform(Procedure proc)
        {
            this.ssa = new SsaState(proc, null);
            this.asta = new AliasState();
            this.currentDef = new Dictionary<Block, Dictionary<Storage, SsaIdentifier>>();
            this.incompletePhis = new Dictionary<Block, Dictionary<Storage, SsaIdentifier>>();
            this.sealedBlocks = new HashSet<Block>();
            foreach (Block b in new DfsIterator<Block>(proc.ControlGraph).PreOrder())
            {
                this.block = b;
                this.currentDef.Add(block, new Dictionary<Storage, SsaIdentifier>()); // new StorageEquality()));
                foreach (var s in b.Statements.ToList())
                {
                    this.stm = s;
                    stm.Instruction = stm.Instruction.Accept(this);
                }
            }
        }

        public SsaState SsaState { get { return ssa; } }

        public override Instruction TransformAssignment(Assignment a)
        {
            var src = a.Src.Accept(this);
            var sid = ssa.Identifiers.Add(a.Dst, this.stm, src, false);
            var idNew = WriteVariable(a.Dst, block, sid, null);
            return new Assignment(idNew, src);
        }

        public Identifier WriteVariable(Identifier id, Block b, SsaIdentifier sid, SsaIdentifier sidPrev)
        {
            asta.Add(id.Storage);
            currentDef[block][id.Storage] = sid;
            sid.Previous = sidPrev;
            return sid.Identifier;
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            asta.Add(id.Storage);
            return ReadVariable(id, block).Identifier;
        }

        public SsaIdentifier ReadVariable(Identifier id, Block b)
        { 
            SsaIdentifier ssaId;
            if (currentDef[b].TryGetValue(id.Storage, out ssaId))
            {
                // Defined locally in this block.
                return ssaId;
            }
            else
            {
                return ReadVariableRecursive(id, b);
            }
        }

        private SsaIdentifier ReadVariableRecursive(Identifier id, Block b)
        {
            SsaIdentifier val;
            SsaIdentifier sidPrev = null;
            if (false)  // !sealedBlocks.Contains(b))
            {
                // Incomplete CFG
                //val = newPhi(id, b);
                //incompletePhis[b][id.Storage] = val;
            }
            else if (b.Pred.Count == 0)
            {
                // Undef'ined or unreachable parameter; assume it's a def.
                val = NewDef(id, b);
            }
            else if (b.Pred.Count == 1)
            {
                val = ReadVariable(id, b.Pred[0]);
                sidPrev = val;
            }
            else
            {
                // Break potential cycles with operandless phi
                val = NewPhi(id, b);
                WriteVariable(id, b, val, null);
                val = AddPhiOperands(id, val);
            }
            WriteVariable(id, b, val, sidPrev);
            return val;
        }

        /// <summary>
        /// Creates a phi statement with no slots for the predecessor blocks, then
        /// inserts the phi statement as the first statement of the block.
        /// </summary>
        /// <param name="b">Block into which the phi statement is inserted</param>
        /// <param name="v">Destination variable for the phi assignment</param>
        /// <returns>The inserted phi Assignment</returns>
        private SsaIdentifier NewPhi( Identifier id, Block b)
        {
            var phiAss = new PhiAssignment(id);
            var stm = new Statement(
                b.Address.ToLinear(),
                phiAss,
                b);
            b.Statements.Insert(0, stm);

            var sid = ssa.Identifiers.Add(phiAss.Dst, stm, phiAss.Src, false);
            phiAss.Dst = sid.Identifier;
            return sid;
        }

        private SsaIdentifier AddPhiOperands(Identifier id, SsaIdentifier phi)
        {
            // Determine operands from predecessors.
            ((PhiAssignment)phi.DefStatement.Instruction).Src =
                new PhiFunction(
                    id.DataType,
                    phi.DefStatement.Block.Pred.
                        Select(p => new PhiArgument(
                            p,
                            ReadVariable(id, p).Identifier))
                        .ToArray());
            return TryRemoveTrivial(phi);
        }

        /// <summary>
        /// If the phi function is trivial, remove it.
        /// </summary>
        /// <param name="phi"></param>
        /// <returns></returns>
        private SsaIdentifier TryRemoveTrivial(SsaIdentifier phi)
        {
            Identifier same = null;
            foreach (var arg in ((PhiAssignment)phi.DefStatement.Instruction).Src.Arguments)
            {
                var op = (Identifier) arg.Value;
                if (op == same || op == phi.Identifier)
                {
                    // Unique value or self-reference
                    continue;
                }
                if (same != null)
                {
                    // The phi merges at least two values; not trivial
                    return phi;
                }
                same = op;
            }
            SsaIdentifier sid;
            if (same == null)
            {
                // Undef'ined or unreachable parameter; assume it's a def.
                sid = NewDef(phi.OriginalIdentifier, phi.DefStatement.Block);
            }
            else
            {
                sid = ssa.Identifiers[same];
            }

            // Remember all users except for phi
            var users = phi.Uses.Where(u => u != phi.DefStatement).ToList();

            // Reroute all uses of phi to use same. Remove phi.
            replaceBy(phi, same);

            // Remove all phi uses which may have become trivial now.
            foreach (var use in users)
            {
                var phiAss = use.Instruction as PhiAssignment;
                if (phiAss != null)
                {
                    TryRemoveTrivial(ssa.Identifiers[phiAss.Dst]);
                }
            }
            return sid;
        }

        private SsaIdentifier NewDef(Identifier id, Block b)
        {
            var sid = ssa.Identifiers.Add(id, null, null, false);
            sid.DefStatement = new Statement(
                b.Address.ToLinear(),
                new DefInstruction(id), b);
            b.Statements.Add(sid.DefStatement);
            return sid;
        }

        private void replaceBy(SsaIdentifier phi, Identifier same)
        {
        }

        private void sealBlock(Block block)
        {
            foreach (var sid in incompletePhis[block].Values)
            {
                AddPhiOperands(sid.Identifier, sid);
            }
            sealedBlocks.Add(block);
        }

        public class StorageEquality : IEqualityComparer<Storage>
        {
            public bool Equals(Storage x, Storage y)
            {
                if (x.Domain != y.Domain)
                    return false;
                var xStart = x.BitAddress;
                var xEnd = x.BitAddress + x.BitSize;
                var yStart = y.BitAddress;
                var yEnd = y.BitAddress + y.BitSize;
                return (xEnd > yStart || yEnd > xStart);
            }

            public int GetHashCode(Storage obj)
            {
                return ((int)obj.Domain).GetHashCode();
            }
        }
    }
}
