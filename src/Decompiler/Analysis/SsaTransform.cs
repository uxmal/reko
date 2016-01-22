#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        public Procedure Procedure { get { return proc; } }

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
                dict[eDef] = (byte)(bits | (BitDefined | BitDeadIn));
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
                var access = (MemoryAccess)store.Dst;
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
                    if (ci.Uses.Count > 0 || ci.Definitions.Count > 0)
                        return;
                    foreach (Identifier id in proc.Frame.Identifiers)
                    {
                        if ((id.Storage is RegisterStorage && !(id.Storage is TemporaryStorage))
                            || id.Storage is FlagGroupStorage)
                        {
                            ci.Definitions.Add(new DefInstruction(id));
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
            private Dictionary<Identifier, Identifier> rename;      // most recently used name for var x.
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

                bool[] visited = new bool[proc.ControlGraph.Blocks.Count];
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
                                PhiAssignment phi = (PhiAssignment)stmCur.Instruction;
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
                    var idNew = NewDef((Identifier)def.Expression, null, false);
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
                // Hell node implementation - use all register variables.

                var alreadyExistingUses = ci.Uses.Select(u => ssa.Identifiers[(Identifier)u.Expression].OriginalIdentifier).ToHashSet();
                foreach (Identifier id in ssa.Identifiers.Select(s => s.OriginalIdentifier).Distinct().ToList())
                {
                    if (id.Storage is RegisterStorage || id.Storage is FlagGroupStorage ||
                        id.Storage is StackLocalStorage)
                    {
                        if (!alreadyExistingUses.Contains(id))
                        {
                            alreadyExistingUses.Add(id);
                            var newId = NewUse(id, stmCur);
                            ci.Uses.Add(new UseInstruction(newId));
                        }
                    }
                }
                foreach (DefInstruction def in ci.Definitions)
                {
                    var id = (Identifier)def.Expression;
                    if (id.Storage is RegisterStorage || id.Storage is FlagGroupStorage ||
                        id.Storage is StackLocalStorage)
                    {
                        def.Expression = NewDef(id, null, false);
                    }
                }
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
                    acc.MemoryId = (MemoryIdentifier)NewDef(acc.MemoryId, store.Src, false);
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

    /// <summary>
    /// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment
    /// form.
    /// </summary>
    /// <remarks>
    /// EXPERIMENTAL - consult uxmal before using
    /// 
    /// This class implements an SSA algorithm that doesn't require 
    /// calculation of the dominator graph. It is based on the algorithm
    /// described in "Simple and Efficient Construction of Static Single
    /// Assignment Form" by Matthias Braun, Sebastian Buchwald, Sebastian 
    /// Hack, Roland Leißa, Christoph Mallon, and Andreas Zwinkau. It is
    /// expected that when it is fully implemented, it will take over from 
    /// SsaTransform above.
    /// </remarks>
    public class SsaTransform2 : InstructionTransformer
    {
        private IProcessorArchitecture arch;
        private DataFlow2 programFlow;
        private Block block;
        private Statement stmCur;
        private Dictionary<Block, SsaBlockState> blockstates;
        private SsaState ssa;

        public SsaTransform2(IProcessorArchitecture arch, Procedure proc, DataFlow2 programFlow)
        {
            this.arch = arch;
            this.programFlow = programFlow;
            this.ssa = new SsaState(proc, null);
            this.blockstates = ssa.Procedure.ControlGraph.Blocks.ToDictionary(k => k, v => new SsaBlockState(v));
        }

        public bool AddUseInstructions { get; set; }
        /// <summary>
        /// If set, only renames frame accesses.
        /// </summary>
        public bool RenameFrameAccesses { get; set; }
        public SsaState SsaState { get { return ssa; } }

        /// <summary>
        /// Transforms <paramref name="proc"/> into Static Single
        /// Assignment form.
        /// </summary>
        /// <remarks>
        /// The resulting SSA identifiers are conventiently kept in the
        /// SsaState property.
        /// </remarks>
        /// <param name="proc"></param>
        public void Transform()
        {
            // Visit blocks in RPO order so that we are guaranteed that a 
            // block with predecessors is always visited after them.
            foreach (Block b in new DfsIterator<Block>(ssa.Procedure.ControlGraph).ReversePostOrder())
            {
                this.block = b;
                foreach (var s in b.Statements.ToList())
                {
                    this.stmCur = s;
                    s.Instruction = s.Instruction.Accept(this);
                }
            }

            // Optionally, add Use instructions in the exit block.
            if (this.AddUseInstructions)
                AddUsesToExitBlock();
        }

        /// <summary>
        /// Adds a UseInstruction for each SsaIdentifier.
        /// </summary>
        /// <remarks>
        /// Doing this will allow us to detect what definitions reach the end of the function.
        /// //$TODO: what about functions that don't terminate, or have branches that don't terminate? In such cases,
        /// the identifiers should be removed.</remarks>
        private void AddUsesToExitBlock()
        {
            var block = ssa.Procedure.ExitBlock;
            var existing = block.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .Select(u => u.Expression)
                .ToHashSet();
            var reachingIds = ssa.Identifiers
                .Where(sid => sid.Identifier.Name != sid.OriginalIdentifier.Name &&
                              !(sid.Identifier.Storage is MemoryStorage) &&
                              !existing.Contains(sid.Identifier))
                .Select(sid => sid.OriginalIdentifier)
                .Distinct()
                .OrderBy(id => id.Name);    // Sort them for stability; unit test are sensitive to shifting order 

            var stms = reachingIds.Select(id => new Statement(0, new UseInstruction(id), block)).ToList();
            block.Statements.AddRange(stms);
            stms.ForEach(u =>
            {
                var use = (UseInstruction)u.Instruction;
                use.Expression = NewUse((Identifier)use.Expression, u, true);
            });
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            if (a is AliasAssignment)
                return a;
            var src = a.Src.Accept(this);
            Identifier idNew = NewDef(a.Dst, src, false);
            return new Assignment(idNew, src);
        }

        /// <summary>
        /// Unresolved calls can be "hell nodes". A hell node is an indirect calls or indirect
        /// jump that prior passes of the decompiler have been unable to resolve.
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            Procedure callee = GetUserProcedure(ci);
            ProcedureFlow2 flow;
            if (callee != null && programFlow.ProcedureFlows.TryGetValue(callee, out flow))
            {
                GenerateUseDefsForKnownCallee(ci, callee, flow);
            }
            else
            {
                GenerateUseDefsForUnknownCallee(ci);
            }
            return ci;
        }

        private void GenerateUseDefsForKnownCallee(CallInstruction ci, Procedure callee, ProcedureFlow2 flow)
        {
            var ab = new ApplicationBuilder(arch, ssa.Procedure.Frame, ci.CallSite, ci.Callee, null, true);
            foreach (var use in callee.EntryBlock.Statements
                .Select(s => (Identifier)((DefInstruction)s.Instruction).Expression))
            {
                var u = ab.Bind(use);
                ci.Uses.Add(new UseInstruction((Identifier)u.Accept(this)));
            }
            foreach (var def in flow.Trashed)
            {
                var d = ssa.Procedure.Frame.EnsureIdentifier(def);
                ci.Definitions.Add(
                    new DefInstruction(
                        NewDef(d, ci.Callee, false)));
            }
        }

        private void GenerateUseDefsForUnknownCallee(CallInstruction ci)
        {
            //$TODO special case for flags; unify them all into an überflag.
            var existingUses = ci.Uses
                .Select(u => ssa.Identifiers[(Identifier)u.Expression].OriginalIdentifier)
                .ToHashSet();
            var existingDefs = ci.Definitions
                .Select(d => ssa.Identifiers[(Identifier)d.Expression].OriginalIdentifier)
                .ToHashSet();

            // Hell node implementation - use and define all variables.
            foreach (Identifier id in ssa.Procedure.Frame.Identifiers)
            {
                if (!existingUses.Contains(id) &&
                    (id.Storage is RegisterStorage && !(id.Storage is TemporaryStorage))
                        || id.Storage is StackStorage)
                {
                    ci.Uses.Add(
                        new UseInstruction((Identifier)NewUse(id, stmCur, false)));
                }
                if (!existingDefs.Contains(id) &&
                    (id.Storage is RegisterStorage && !(id.Storage is TemporaryStorage))
                    || id.Storage is FlagGroupStorage)
                {
                    ci.Definitions.Add(
                        new DefInstruction(
                            NewDef(id, ci.Callee, false)));
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

        public override Instruction TransformDefInstruction(DefInstruction def)
        {
            return def;
        }

        public override Instruction TransformStore(Store store)
        {
            store.Src = store.Src.Accept(this);
            var acc = store.Dst as MemoryAccess;
            if (acc != null)
            {
                if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, acc.EffectiveAddress))
                {
                    var idFrame = EnsureStackVariable(ssa.Procedure, acc.EffectiveAddress, acc.DataType);
                    var idDst = NewDef(idFrame, store.Src, false);
                    return new Assignment(idDst, store.Src);
                }
                else
                {
                    SegmentedAccess sa = acc as SegmentedAccess;
                    if (sa != null)
                        sa.BasePointer = sa.BasePointer.Accept(this);
                    acc.EffectiveAddress = acc.EffectiveAddress.Accept(this);
                    UpdateMemoryIdentifier(acc, true);
                }
            }
            else
            {
                store.Dst = store.Dst.Accept(this);
            }
            return store;
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            return NewUse(id, stmCur, false);
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

        private Identifier NewDef(Identifier idOld, Expression src, bool isSideEffect)
        {
            SsaIdentifier sidOld;
            if (idOld != null && ssa.Identifiers.TryGetValue(idOld, out sidOld))
            {
                if (sidOld.OriginalIdentifier != sidOld.Identifier)
                {
                    // Already renamed by a previous pass.
                    return sidOld.Identifier;
                }
            }
            var sid = ssa.Identifiers.Add(idOld, stmCur, src, isSideEffect);
            var flagGroup = idOld.Storage as FlagGroupStorage;
            Identifier idNew;
            var bs = blockstates[block];
            if (flagGroup != null && !RenameFrameAccesses)
            {
                foreach (uint flagBitMask in flagGroup.GetFlagBitMasks())
                {
                    var ss = new SsaFlagTransformer(idOld, flagBitMask, ssa.Identifiers, stmCur, blockstates);
                    ss.WriteVariable(bs, sid, true);
                }
                idNew = sid.Identifier;
                return idNew;
            }
            var stack = idOld.Storage as StackStorage;
            if (stack != null)
            {
                var ss = new SsaStackTransformer(idOld, stack.StackOffset, ssa.Identifiers, stmCur, blockstates);
                return ss.WriteVariable(bs, sid, true);
            }
            var seq = idOld.Storage as SequenceStorage;
            if (seq != null)
            {
                var ss = new SsaSequenceTransformer(idOld, seq, seq.Head, ssa.Identifiers, stmCur, blockstates);
                var head = ss.WriteVariable(bs, sid, true);
                ss = new SsaSequenceTransformer(idOld, seq, seq.Tail, ssa.Identifiers, stmCur, blockstates);
                var tail = ss.WriteVariable(bs, sid, true);
                return sid.Identifier;
            }
            else if (!RenameFrameAccesses)
            {
                var ss = new SsaIdentifierTransformer(idOld, ssa.Identifiers, stmCur, blockstates);
                return ss.WriteVariable(bs, sid, true);
            }
            return idOld;
        }

        private Expression NewUse(Identifier id, Statement stm, bool force)
        {
            var bs = blockstates[block];
            var flagGroup = id.Storage as FlagGroupStorage;
            if (flagGroup != null && (!RenameFrameAccesses || force))
            {
                // Analyze each flag in the flag group separately.
                var ids = new Dictionary<Identifier, SsaIdentifier>();
                foreach (uint flagBitMask in flagGroup.GetFlagBitMasks())
                {
                    var ss = new SsaFlagTransformer(id, flagBitMask, ssa.Identifiers, stm, blockstates);
                    var sid = ss.ReadVariable(bs, false);
                    ids[sid.Identifier] = sid;
                }
                if (ids.Count == 1)
                {
                    var de = ids.First();
                    de.Value.Uses.Add(stm);
                    return de.Key;
                }
                else
                {
                    return OrTogether(ids.Values, stm);
                }
            }
            var stack = id.Storage as StackStorage;
            if (stack != null && (RenameFrameAccesses || force))
            {
                var ss = new SsaStackTransformer(id, stack.StackOffset, ssa.Identifiers, stm, blockstates);
                var sid = ss.ReadVariable(bs, false);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }
            var seq = id.Storage as SequenceStorage;
            if (seq != null)
            {
                var ss = new SsaIdentifierTransformer(seq.Head, ssa.Identifiers, stmCur, blockstates);
                var head = ss.ReadVariable(bs, false);
                ss = new SsaIdentifierTransformer(seq.Tail, ssa.Identifiers, stmCur, blockstates);
                var tail = ss.ReadVariable(bs, false);
                var sqs = new SsaSequenceTransformer(id, seq, null, ssa.Identifiers, stm, blockstates);
                return sqs.Fuse(head, tail);
            }
            else if (!RenameFrameAccesses || force)
            {
                var ss = new SsaIdentifierTransformer(id, ssa.Identifiers, stm, blockstates);
                var sid = ss.ReadVariable(bs, false);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }
            return id;
        }

        private Expression OrTogether(IEnumerable<SsaIdentifier> sids, Statement stm)
        {
            Expression e = null;
            foreach (var sid in sids.OrderBy(id => id.Identifier.Name))
            {
                sid.Uses.Add(stm);
                if (e == null)
                    e = sid.Identifier;
                else
                    e = new BinaryExpression(Operator.Or, PrimitiveType.Byte, e, sid.Identifier);
            }
            return e;
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, access.EffectiveAddress))
            {
                var idFrame = EnsureStackVariable(ssa.Procedure, access.EffectiveAddress, access.DataType);
                var idNew = NewUse(idFrame, stmCur, false);
                return idNew;
            }
            else
            {
                access.EffectiveAddress = access.EffectiveAddress.Accept(this);
                access.MemoryId = (MemoryIdentifier)NewUse(access.MemoryId, stmCur, false);
                return access;
            }
        }

        public override Expression VisitSegmentedAccess(SegmentedAccess access)
        {
            if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, access.EffectiveAddress))
            {
                var idFrame = EnsureStackVariable(ssa.Procedure, access.EffectiveAddress, access.DataType);
                var idNew = NewUse(idFrame, stmCur, false);
                return idNew;
            }
            else
            {
                access.BasePointer = access.BasePointer.Accept(this);
                access.EffectiveAddress = access.EffectiveAddress.Accept(this);
                access.MemoryId = (MemoryIdentifier) NewUse(access.MemoryId, stmCur, false);
                return access;
            }
        }

        private void UpdateMemoryIdentifier(MemoryAccess access, bool storing)
        {
            if (storing)
            {
                var sid = ssa.Identifiers.Add(access.MemoryId, this.stmCur, null, false);
                var ss = new SsaIdentifierTransformer(access.MemoryId, ssa.Identifiers, stmCur, blockstates);
                access.MemoryId = (MemoryIdentifier)ss.WriteVariable(blockstates[block], sid, false);
            }
            else
            {
                access.MemoryId = (MemoryIdentifier)access.MemoryId.Accept(this);
            }
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
    }

    public class SsaBlockState
    {
        public readonly Block Block;
        public readonly Dictionary<StorageDomain, SsaIdentifier> currentDef;
        public readonly Dictionary<uint, SsaIdentifier> currentFlagDef;
        public readonly Dictionary<int, SsaIdentifier> currentStackDef;
        public readonly Dictionary<StorageDomain, SsaIdentifier> incompletePhis;

        public SsaBlockState(Block block)
        {
            this.Block = block;
            this.currentDef = new Dictionary<StorageDomain, SsaIdentifier>();
            this.currentFlagDef = new Dictionary<uint, SsaIdentifier>();
            this.currentStackDef = new Dictionary<int, SsaIdentifier>();
            this.incompletePhis = new Dictionary<StorageDomain, SsaIdentifier>();
        }
    }

    public class SsaIdentifierTransformer
    {
        private Identifier id;
        private SsaIdentifierCollection ssaIds;
        private Statement stm;
        private IDictionary<Block, SsaBlockState> blockstates;

        public SsaIdentifierTransformer(Identifier id, SsaIdentifierCollection ssaIds, Statement stm, IDictionary<Block, SsaBlockState> blockstates)
        {
            this.id = id;
            this.ssaIds = ssaIds;
            this.stm = stm;
            this.blockstates = blockstates;
        }

        /// <summary>
        /// Registers the fact that identifier <paramref name="id"/> is
        /// modified in the block <paramref name="b" />. 
        /// </summary>
        /// <param name="id">The identifier before being SSA transformed</param>
        /// <param name="b">The block in which the identifier was changed</param>
        /// <param name="sid">The identifier after being SSA transformed.</param>
        /// <param name="performProbe">if true, looks "backwards" to see
        ///   if <paramref name="id"/> overlaps with another identifier</param>
        /// <returns></returns>
        public virtual Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
        {
            SsaIdentifier alias = sid;
            if (performProbe)
            {
                // Did a previous SSA id modify the same storage as id?
                alias = ReadVariable(bs, performProbe);
                if (alias != null)
                {
                    // Was the previous modification larger than this modification?
                    if (alias.Identifier.Storage.Exceeds(id.Storage))
                    {
                        // Generate a DPB so the previous modification "shines
                        // through".
                        var dpb = new DepositBits(alias.Identifier, sid.Identifier, (int)id.Storage.BitAddress);
                        var ass = new AliasAssignment(alias.OriginalIdentifier, dpb);
                        alias = InsertAfterDefinition(sid.DefStatement, ass);
                    }
                    else
                    {
                        alias = sid;
                    }
                }
                else
                {
                    alias = sid;
                }
            }
            bs.currentDef[id.Storage.Domain] = alias;
            return sid.Identifier;
        }

        /// <summary>
        /// Reaches "backwards" to locate the SSA identifier that defines
        /// the identifier <paramref name="id"/>, starting in block <paramref name="b"/>.
        /// </summary>
        /// If no definition of <paramref name="id"/> is found, a new 
        /// DefStatement is created in the entry block of the procedure,
        /// </summary>
        /// <param name="id"></param>
        /// <param name="b"></param>
        /// <param name="aliasProbe"></param>
        /// <returns></returns>
        public virtual SsaIdentifier ReadVariable(SsaBlockState bs, bool aliasProbe)
        {
            SsaIdentifier ssaId;
            if (bs.currentDef.TryGetValue(id.Storage.Domain, out ssaId))
            {
                // Defined locally in this block.
                // Does ssaId intersect the probed value?
                if (ssaId.Identifier.Storage.OverlapsWith(id.Storage))
                {
                    return MaybeGenerateAliasStatement(ssaId, aliasProbe);
                }
            }
            // Keep probin'.
            return ReadVariableRecursive(bs, aliasProbe);
        }

        public SsaIdentifier ReadVariableRecursive(SsaBlockState bs, bool aliasProbe)
        {
            SsaIdentifier val;
            if (false)  // !sealedBlocks.Contains(b))
            {
                // Incomplete CFG
                //val = newPhi(id, b);
                //incompletePhis[b][id.Storage] = val;
            }
            else if (bs.Block.Pred.Count == 0)
            {
                // Undef'ined or unreachable parameter; assume it's a def.
                if (!aliasProbe)
                    val = NewDefInstruction(id, bs.Block);
                else
                    val = null;
            }
            else if (bs.Block.Pred.Count == 1)
            {
                val = ReadVariable(blockstates[bs.Block.Pred[0]], aliasProbe);
            }
            else
            {
                // Break potential cycles with operandless phi
                val = NewPhi(id, bs.Block);
                WriteVariable(bs, val, false);
                val = AddPhiOperands(id, val, aliasProbe);
            }
            if (val != null && !aliasProbe)
                WriteVariable(bs, val, false);
            return val;
        }

        /// <summary>
        /// If <paramref name="idTo"/> is smaller than <paramref name="sidFrom" />, then
        /// it doesn't cover it completely. Therefore, we must generate a SLICE / cast 
        /// statement.
        /// </summary>
        /// <param name="idTo"></param>
        /// <param name="sidFrom"></param>
        /// <returns></returns>
        private SsaIdentifier MaybeGenerateAliasStatement(SsaIdentifier sidFrom, bool aliasProbe)
        {
            var stgFrom = sidFrom.Identifier.Storage;
            Debug.Assert(!(id.Storage is FlagGroupStorage), "Should never be called on a flag group");
            var stgTo = id.Storage;
            if (stgFrom == stgTo ||
                (aliasProbe && stgFrom.Exceeds(stgTo)))
            {
                return sidFrom;
            }
            Expression e = null;
            if (stgFrom.Covers(stgTo))
            {
                if (stgTo.BitAddress != 0)
                    e = new Slice(id.DataType, sidFrom.Identifier, (uint)stgTo.BitAddress);
                else
                    e = new Cast(id.DataType, sidFrom.Identifier);
            }
            else
            {
                var sidTo = ReadVariable(blockstates[sidFrom.DefStatement.Block], false);
                e = new DepositBits(id, sidFrom.Identifier, (int)stgFrom.BitAddress);
            }
            var ass = new AliasAssignment(id, e);
            return InsertAfterDefinition(sidFrom.DefStatement, ass);
        }

        /// <summary>
        /// Inserts the statement <paramref name="ass"/> after the statement
        /// <paramref name="stmBefore"/>, skipping any AliasAssignments that
        /// statements that may have been added after 
        /// <paramref name="stmBefore"/>.
        /// </summary>
        /// <param name="stmBefore"></param>
        /// <param name="ass"></param>
        /// <returns></returns>
        public SsaIdentifier InsertAfterDefinition(Statement stmBefore, AliasAssignment ass)
        {
            var b = stmBefore.Block;
            int i = b.Statements.IndexOf(stmBefore);
            // Skip alias statements
            while (i < b.Statements.Count - 1 && b.Statements[i].Instruction is AliasAssignment)
                ++i;
            stmBefore.Block.Statements.Insert(i + 1, stmBefore.LinearAddress, ass);

            var sidTo = ssaIds.Add(ass.Dst, this.stm, ass.Src, false);
            ass.Dst = sidTo.Identifier;
            return sidTo;
        }

        /// <summary>
        /// Creates a phi statement with no slots for the predecessor blocks, then
        /// inserts the phi statement as the first statement of the block.
        /// </summary>
        /// <param name="b">Block into which the phi statement is inserted</param>
        /// <param name="v">Destination variable for the phi assignment</param>
        /// <returns>The inserted phi Assignment</returns>
        private SsaIdentifier NewPhi(Identifier id, Block b)
        {
            var phiAss = new PhiAssignment(id, 0);
            var stm = new Statement(0, phiAss, b);
            b.Statements.Insert(0, stm);

            var sid = ssaIds.Add(phiAss.Dst, stm, phiAss.Src, false);
            phiAss.Dst = sid.Identifier;
            return sid;
        }

        private SsaIdentifier AddPhiOperands(Identifier id, SsaIdentifier phi, bool aliasProbe)
        {
            // Determine operands from predecessors.
            var preds = phi.DefStatement.Block.Pred;
            var sids = preds.Select(p => ReadVariable(blockstates[p], aliasProbe)).ToArray();
            if (aliasProbe && sids.Any(s => s != null))
            {
                for (int i = 0; i < sids.Length; ++i)
                {
                    if (sids[i] == null)
                        sids[i] = ReadVariable(blockstates[preds[i]], false);
                }
            }
            if (aliasProbe && sids.Any(s => s == null))
                return null;
            ((PhiAssignment)phi.DefStatement.Instruction).Src =
                new PhiFunction(
                    id.DataType,
                    sids.Select(s => s.Identifier).ToArray());
            return TryRemoveTrivial(phi, aliasProbe);
        }

        /// <summary>
        /// If the phi function is trivial, remove it.
        /// </summary>
        /// <param name="phi"></param>
        /// <returns></returns>
        private SsaIdentifier TryRemoveTrivial(SsaIdentifier phi, bool aliasProbe)
        {
            bool firstTime = true;
            Identifier same = null;
            foreach (Identifier op in ((PhiAssignment)phi.DefStatement.Instruction).Src.Arguments)
            {
                if (op == same || op == phi.Identifier)
                {
                    // Unique value or self-reference
                    continue;
                }
                if (!firstTime)
                {
                    // The phi merges at least two values; not trivial
                    return phi;
                }
                same = op;
                firstTime = false;
            }
            SsaIdentifier sid;
            if (same == null)
            {
                // Undef'ined or unreachable parameter; assume it's a def.
                sid = NewDefInstruction(phi.OriginalIdentifier, phi.DefStatement.Block);
            }
            else
            {
                sid = ssaIds[same];
            }

            // Remember all users except for phi
            var users = phi.Uses.Where(u => u != phi.DefStatement).ToList();

            // Reroute all uses of phi to use same. Remove phi.
            ReplaceBy(phi, same);

            // Remove all phi uses which may have become trivial now.
            foreach (var use in users)
            {
                var phiAss = use.Instruction as PhiAssignment;
                if (phiAss != null)
                {
                    TryRemoveTrivial(ssaIds[phiAss.Dst], aliasProbe);
                }
            }
            phi.DefStatement.Block.Statements.Remove(phi.DefStatement);
            ssaIds.Remove(phi);
            return sid;
        }

        private SsaIdentifier NewDefInstruction(Identifier id, Block b)
        {
            var sid = ssaIds.Add(id, null, null, false);
            sid.DefStatement = new Statement(0, new DefInstruction(id), b);
            b.Statements.Add(sid.DefStatement);
            return sid;
        }

        private void ReplaceBy(SsaIdentifier sidOld, Identifier idNew)
        {
            foreach (var use in sidOld.Uses)
            {
                use.Instruction.Accept(new IdentifierReplacer(this.ssaIds, use, sidOld.Identifier, idNew));
            }
        }
    }

    public class SsaFlagTransformer : SsaIdentifierTransformer
    {
        private uint flagMask;

        public SsaFlagTransformer(Identifier id, uint flagMask, SsaIdentifierCollection ssaIds, Statement stm, IDictionary<Block, SsaBlockState> blockstates)
            : base(id, ssaIds, stm, blockstates)
        {
            this.flagMask = flagMask;
        }

        public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
        {
            bs.currentFlagDef[flagMask] = sid;
            return sid.Identifier;
        }

        public override SsaIdentifier ReadVariable(SsaBlockState bs, bool aliasProbe)
        {
            SsaIdentifier ssaId;
            if (bs.currentFlagDef.TryGetValue(flagMask, out ssaId))
            {
                // Defined locally in this block.
                return ssaId;
            }
            // Keep probin'.
            return ReadVariableRecursive(bs, aliasProbe);
        }
    }

    public class SsaStackTransformer : SsaIdentifierTransformer
    {
        private int stackOffset;

        public SsaStackTransformer(
            Identifier id,
            int stackOffset, 
            SsaIdentifierCollection ssaIds,
            Statement stm,
            IDictionary<Block,SsaBlockState> blockstates)
            : base(id, ssaIds, stm, blockstates)
        {
            this.stackOffset = stackOffset;
        }

        public override SsaIdentifier ReadVariable(SsaBlockState bs, bool aliasProbe)
        {
            SsaIdentifier ssaId;
            if (bs.currentStackDef.TryGetValue(stackOffset, out ssaId))
            {
                // Defined locally in this block.
                return ssaId;
            }
            // Keep probin'.
            return ReadVariableRecursive(bs, aliasProbe);
        }

        public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
        {
            bs.currentStackDef[stackOffset] = sid;
            return sid.Identifier;
        }
    }

    public class SsaSequenceTransformer : SsaIdentifierTransformer
    {
        private Identifier idSub;
        private SequenceStorage seq;

        public SsaSequenceTransformer(
            Identifier id, 
            SequenceStorage seq, 
            Identifier idSub,
            SsaIdentifierCollection ssaIds, 
            Statement stm,
            IDictionary<Block, SsaBlockState> blockstates)
            : base(id, ssaIds, stm, blockstates)
        {
            this.seq = seq;
            this.idSub = idSub;
        }

        public Identifier Fuse(SsaIdentifier head, SsaIdentifier tail)
        {
            AliasAssignment assHead, assTail;
            if (head.DefStatement.Instruction.As(out assHead) &&
                tail.DefStatement.Instruction.As(out assTail))
            {
                Slice eHead;
                Cast eTail;
                if (assHead.Src.As(out eHead) && assTail.Src.As(out eTail))
                {
                    return (Identifier)eHead.Expression;
                }
            }
            throw new NotImplementedException();
        }

        public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
        {
            bs.currentDef[idSub.Storage.Domain] = sid;
            return sid.Identifier;
        }
    }
}
