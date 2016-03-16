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
using System.Text;

namespace Reko.Analysis
{
	/// <summary>
	/// Transforms a <see cref="Reko.Core.Procedure"/> to Static Single Assignment form.
	/// </summary>
    /// <remarks>
    /// This class implements the SSA algorithm from Appel's "Modern compiler 
    /// implementatation in [language of your choice]."
    /// </remarks>
    [Obsolete("", false)]
	public class SsaTransform
	{
        private ProgramDataFlow programFlow;
		private Procedure proc;
        private IImportResolver importResolver;

        private const byte BitDefined = 1;
		private const byte BitDeadIn = 2;
		private const byte BitHasPhi = 4;
        private Dictionary<Expression, byte>[] AOrig;

		/// <summary>
		/// Constructs an SsaTransform, and in the process generates the SsaState for the procedure <paramref>proc</paramref>.
		/// </summary>
		/// <param name="proc"></param>
		/// <param name="gr"></param>
		public SsaTransform(ProgramDataFlow programFlow, Procedure proc, IImportResolver importResolver, DominatorGraph<Block> gr)
		{
            this.programFlow = programFlow;
			this.proc = proc;
            this.importResolver = importResolver;
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
            private IImportResolver importResolver;
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
                this.importResolver = ssaXform.importResolver;
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
                    .Where(id => !existing.Contains(id) &&
                                 !(id.Storage is StackArgumentStorage))
                    .OrderBy(id => id.Name)     // Sort them for stability; unit test are sensitive to shifting order 
                    .Select(id => new Statement(0, new UseInstruction(id), block)));
            }

            private void AddDefInstructions(CallInstruction ci, ProcedureFlow2 flow)
            {
                var existing = ci.Definitions.Select(d => ssa.Identifiers[(Identifier)d.Expression].OriginalIdentifier).ToHashSet();
                var ab = new FrameApplicationBuilder(null, proc.Frame, null, null, true);
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
                var ea = access.EffectiveAddress.Accept(this);
                BinaryExpression bin;
                Identifier id;
                Constant c = null;
                if (ea.As(out bin) &&
                    bin.Left.As(out id) &&
                    bin.Right.As(out c) &&
                    rename.ContainsKey(id))
                {
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
                    access.EffectiveAddress = c;
                    var e = importResolver.ResolveToImportedProcedureConstant(stmCur, c);
                    if (e != null)
                        return e;
                }
                access.MemoryId = (MemoryIdentifier)access.MemoryId.Accept(this);
                access.EffectiveAddress = ea;
                return access;
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
        private IImportResolver importResolver;
        private Block block;
        private Statement stmCur;
        private Dictionary<Block, SsaBlockState> blockstates;
        private SsaState ssa;
        private TransformerFactory factory;
        public readonly HashSet<SsaIdentifier> incompletePhis;

        public SsaTransform2(IProcessorArchitecture arch, Procedure proc, IImportResolver importResolver, DataFlow2 programFlow)
        {
            this.arch = arch;
            this.programFlow = programFlow;
            this.importResolver = importResolver;
            this.ssa = new SsaState(proc, null);
            this.blockstates = ssa.Procedure.ControlGraph.Blocks.ToDictionary(k => k, v => new SsaBlockState(v));
            this.factory = new TransformerFactory(this);
            this.incompletePhis = new HashSet<SsaIdentifier>();
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
                //Debug.Print("*** {0}:", b.Name);
                foreach (var s in b.Statements.ToList())
                {
                    this.stmCur = s;
                    //Debug.Print("***  {0}", s.Instruction);
                    s.Instruction = s.Instruction.Accept(this);
                }
                blockstates[b].Visited = true;
            }
            ProcessIncompletePhis();

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
            //$TODO: flag groups need to be grouped on exit
            // We don't do them yet.
            this.block = ssa.Procedure.ExitBlock;
            var existing = block.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .Select(u => u.Expression)
                .ToHashSet();
            var reachingIds = ssa.Identifiers
                .Where(sid => sid.Identifier.Name != sid.OriginalIdentifier.Name &&
                              !(sid.Identifier.Storage is MemoryStorage) &&
                              !(sid.Identifier.Storage is StackStorage) &&
                              !existing.Contains(sid.Identifier))
                .Select(sid => sid.OriginalIdentifier);
            reachingIds = SeparateSequences(reachingIds);
            reachingIds = GroupFlags(reachingIds);
            var sortedIds = ResolveOverlaps(reachingIds)
                .Distinct()
                .OrderBy(id => id.Name);    // Sort them for stability; unit test are sensitive to shifting order 

            var stms = sortedIds.Select(id => new Statement(0, new UseInstruction(id), block)).ToList();
            block.Statements.AddRange(stms);
            stms.ForEach(u =>
            {
                var use = (UseInstruction)u.Instruction;
                use.Expression = NewUse((Identifier)use.Expression, u, true);
            });
        }

        public static IEnumerable<Identifier> SeparateSequences(IEnumerable<Identifier> ids)
        {
            foreach (var id in ids)
            {
                var seq = id.Storage as SequenceStorage;
                if (seq != null)
                {
                    yield return seq.Head;
                    yield return seq.Tail;
                }
                else
                {
                    yield return id;
                }
            }
                }

        public static IEnumerable<Identifier> ResolveOverlaps(IEnumerable<Identifier> ids)
        {
            var registerBag = new Dictionary<StorageDomain, HashSet<Identifier>>();
            var others = new List<Identifier>();
            foreach (var id in ids)
            {
                if (id.Storage is RegisterStorage)
                {
                    var dom = id.Storage.Domain;
                    HashSet<Identifier> aliases;
                    if (registerBag.TryGetValue(dom, out aliases))
                    {
                        aliases.RemoveWhere(a => id.Storage.Covers(a.Storage));
                        if (!aliases.Any(a => a.Storage.Covers(id.Storage)))
                            aliases.Add(id);
            }
                    else
                    {
                        aliases = new HashSet<Identifier> { id };
                        registerBag.Add(dom, aliases);
        }
                }
                else
                {
                    others.Add(id);
                }
            }
            return registerBag.Values.SelectMany(s => s).Concat(others);
        }

        public IEnumerable<Identifier> GroupFlags(IEnumerable<Identifier> ids)
        {
            var flags = new Dictionary<FlagRegister, uint>();
            var others = new List<Identifier>();
            foreach (var id in ids)
            {
                var grf = id.Storage as FlagGroupStorage;
                if (grf != null)
                {
                    uint u;
                    if (flags.TryGetValue(grf.FlagRegister, out u))
                    {
                        flags[grf.FlagRegister] = u | grf.FlagGroupBits;
                    }
                    else
                    {
                        flags.Add(grf.FlagRegister, grf.FlagGroupBits);
                    }
                }
                else
                {
                    others.Add(id);
                }
            }
            return flags.Select(de => ssa.Procedure.Frame.EnsureFlagGroup(arch.GetFlagGroup(de.Value)))
                .Concat(others);
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            if (a is AliasAssignment)
                return a;
            var src = a.Src.Accept(this);
            Identifier idNew = this.RenameFrameAccesses ? a.Dst : NewDef(a.Dst, src, false);
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
            ci.Callee = ci.Callee.Accept(this);
            ProcedureBase callee = GetCalleeProcedure(ci);
            if (callee != null && callee.Signature != null && callee.Signature.ParametersValid)
            {
                var ab = CreateApplicationBuilder(ci.Callee.DataType, callee, ci.CallSite);
                return ab.CreateInstruction(callee.Signature, callee.Characteristics);
            }
            ProcedureFlow2 flow;
            var proc = callee as Procedure;
            if (proc != null && programFlow.ProcedureFlows.TryGetValue(proc, out flow))
            {
                GenerateUseDefsForKnownCallee(ci, proc, flow);
            }
            else
            {
                GenerateUseDefsForUnknownCallee(ci);
            }
            return ci;
        }

        private ApplicationBuilder CreateApplicationBuilder(DataType dt, ProcedureBase eCallee, CallSite site)
        {
            var pc = new ProcedureConstant(dt, eCallee);
            var ab = new SsaApplicationBuilder(this, site, pc);
            return ab;
        }

        private void GenerateUseDefsForKnownCallee(CallInstruction ci, Procedure callee, ProcedureFlow2 flow)
        {
            var ab = new FrameApplicationBuilder(arch, ssa.Procedure.Frame, ci.CallSite, ci.Callee, true);
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
                        new UseInstruction((Identifier)NewUse(id, stmCur, true)));
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

        private ProcedureBase GetCalleeProcedure(CallInstruction ci)
        {
            Identifier id;
            ProcedureConstant pc;
            if (ci.Callee.As(out id))
            {
                pc = ssa.Identifiers[id].DefExpression as ProcedureConstant;
                if (pc == null)
                    return null;
            }
            else if (!ci.Callee.As(out pc))
            {
                return null;
            }
            return pc.Procedure;
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
                    ssa.Identifiers[ssa.Procedure.Frame.FramePointer].Uses.Remove(stmCur);
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
                    if (!this.RenameFrameAccesses)
                        UpdateMemoryIdentifier(acc, true);
                }
            }
            else
            {
                store.Dst = store.Dst.Accept(this);
            }
            return store;
        }

        public override Expression VisitApplication(Application appl)
        {
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                var  outArg = appl.Arguments[i] as OutArgument;
                if (outArg != null)
                {
                    var id = outArg.Expression as Identifier;
                    if (id != null)
                    {
                        appl.Arguments[i] = new OutArgument(
                            outArg.DataType,
                            NewDef(id, appl, true));
                        continue;
                    }
                }
                appl.Arguments[i] = appl.Arguments[i].Accept(this);
            }
            return appl;
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
            {
                exp = RenameFrameAccesses ? id : NewDef(id, outArg, true);
            }
            else
                exp = outArg.Expression.Accept(this);
            return new OutArgument(outArg.DataType, exp);
        }

        public Identifier NewDef(Identifier idOld, Expression src, bool isSideEffect)
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
            var bs = blockstates[block];
            var x = factory.Create(idOld, stmCur);
            return x.NewDef(bs, sid);
        }

        private Expression NewUse(Identifier id, Statement stm, bool force)
        {
            if (RenameFrameAccesses && !force)
                return id;
            var bs = blockstates[block];
            var x = factory.Create(id, stm);
            return x.NewUse(bs);
        }

        public override Expression VisitMemoryAccess(MemoryAccess access)
        {
            if (this.RenameFrameAccesses && IsFrameAccess(ssa.Procedure, access.EffectiveAddress))
            {
                ssa.Identifiers[ssa.Procedure.Frame.FramePointer].Uses.Remove(stmCur);
                var idFrame = EnsureStackVariable(ssa.Procedure, access.EffectiveAddress, access.DataType);
                var idNew = NewUse(idFrame, stmCur, true);
                return idNew;
            }

            var ea = access.EffectiveAddress.Accept(this);
            BinaryExpression bin;
            Identifier id;
            Constant c = null;
            if (ea.As(out bin) &&
                bin.Left.As(out id) &&
                bin.Right.As(out c))
            {
                var sid = ssa.Identifiers[id];
                var cOther = sid.DefExpression as Constant;
                if (cOther != null)
                {
                    c = bin.Operator.ApplyConstants(cOther, c);
                    sid.Uses.Remove(stmCur);
                }
                else
                {
                    c = null;
                }
            }
            else
            {
                c = ea as Constant;
            }

            if (c != null)
            {
                access.EffectiveAddress = c;
                var e = importResolver.ResolveToImportedProcedureConstant(stmCur, c);
                if (e != null)
                    return e;
                ea = c;
            }
            UpdateMemoryIdentifier(access, false);
            access.EffectiveAddress = ea;
            return access;
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
                access.MemoryId = (MemoryIdentifier)NewUse(access.MemoryId, stmCur, false);
                return access;
            }
        }

        private void UpdateMemoryIdentifier(MemoryAccess access, bool storing)
        {
            if (storing)
            {
                var sid = ssa.Identifiers.Add(access.MemoryId, this.stmCur, null, false);
                var ss = new SsaRegisterTransformer(access.MemoryId, stmCur, this);
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

        private void ProcessIncompletePhis()
        {
            foreach (var phi in incompletePhis)
            {
                var phiBlock = phi.DefStatement.Block;
                var x = factory.Create(phi.OriginalIdentifier, phi.DefStatement);
                x.AddPhiOperandsCore(phi, false);
            }
            incompletePhis.Clear();
        }

        public class SsaBlockState
        {
            public readonly Block Block;
            public readonly Dictionary<StorageDomain, AliasState> currentDef;
            public readonly Dictionary<uint, SsaIdentifier> currentFlagDef;
            public readonly Dictionary<int, SsaIdentifier> currentStackDef;
            public readonly Dictionary<int, SsaIdentifier> currentFpuDef;
            public bool Visited;

            public SsaBlockState(Block block)
            {
                this.Block = block;
                this.Visited = false;
                this.currentDef = new Dictionary<StorageDomain, AliasState>();
                this.currentFlagDef = new Dictionary<uint, SsaIdentifier>();
                this.currentStackDef = new Dictionary<int, SsaIdentifier>();
                this.currentFpuDef = new Dictionary<int, SsaIdentifier>();
            }

#if DEBUG
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("BlockState {0}", Block.Name);
                sb.AppendLine();
                sb.AppendFormat("    {0}",
                    string.Join(",", currentDef.Keys.Select(k => ((int)k).ToString())));
                return sb.ToString();
            }
#endif
        }

        public class AliasState
        {
            public readonly SsaIdentifier SsaId;        // The id that actually was modified.
            public readonly IDictionary<Identifier, SsaIdentifier> Aliases;     // Other ids that were affected by this stm.
            public AliasState(SsaIdentifier ssaId)
            {
                this.SsaId = ssaId;
                this.Aliases = new Dictionary<Identifier, SsaIdentifier>();
            }

#if DEBUG
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Alias: {0}", SsaId.Identifier.Name);
                if (Aliases.Count > 0)
                {
                    sb.AppendFormat(" = {0}", string.Join(", ", Aliases.Values.OrderBy(v => v.Identifier.Name)));
                }
                return sb.ToString();
            }
#endif
        }

        public class TransformerFactory : StorageVisitor<SsaIdentifierTransformer>
        {
            private SsaTransform2 transform;
            private Identifier id;
            private Statement stm;

            public TransformerFactory(SsaTransform2 transform)
            {
                this.transform = transform;
            }

            public SsaIdentifierTransformer Create(Identifier id, Statement stm)
            {
                this.id = id;
                this.stm = stm;
                return id.Storage.Accept(this);
            }

            public SsaIdentifierTransformer VisitFlagGroupStorage(FlagGroupStorage grf)
            {
                return new SsaFlagTransformer(id, grf, stm, transform);
            }

            public SsaIdentifierTransformer VisitFlagRegister(FlagRegister freg)
            {
                throw new NotImplementedException();
            }

            public SsaIdentifierTransformer VisitFpuStackStorage(FpuStackStorage fpu)
            {
                return new FpuStackTransformer(id, fpu, stm, transform);
            }

            public SsaIdentifierTransformer VisitMemoryStorage(MemoryStorage global)
            {
                return new SsaRegisterTransformer(id, stm, transform);
            }

            public SsaIdentifierTransformer VisitOutArgumentStorage(OutArgumentStorage arg)
            {
                throw new NotImplementedException();
            }

            public SsaIdentifierTransformer VisitRegisterStorage(RegisterStorage reg)
            {
                return new SsaRegisterTransformer(id, stm, transform);
            }

            public SsaIdentifierTransformer VisitSequenceStorage(SequenceStorage seq)
            {
                return new SsaSequenceTransformer(id, seq, stm, transform);
            }

            public SsaIdentifierTransformer VisitStackArgumentStorage(StackArgumentStorage stack)
            {
                return new SsaStackTransformer(id, stack.StackOffset, stm, transform);
            }

            public SsaIdentifierTransformer VisitStackLocalStorage(StackLocalStorage local)
            {
                return new SsaStackTransformer(id, local.StackOffset, stm, transform);
            }

            public SsaIdentifierTransformer VisitTemporaryStorage(TemporaryStorage temp)
            {
                return new SsaRegisterTransformer(id, stm, transform);
            }
        }

        public abstract class SsaIdentifierTransformer
        {
            protected Identifier id;
            protected readonly Statement stm;
            protected readonly SsaTransform2 outer;
            protected readonly SsaIdentifierCollection ssaIds;
            protected readonly IDictionary<Block, SsaBlockState> blockstates;

            public SsaIdentifierTransformer(Identifier id, Statement stm, SsaTransform2 outer)
            {
                this.id = id;
                this.stm = stm;
                this.ssaIds = outer.ssa.Identifiers;
                this.blockstates = outer.blockstates;
                this.outer = outer;
            }

            public virtual Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs, false);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public virtual Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            /// <summary>
            /// Registers the fact that identifier <paramref name="id"/> is
            /// modified in the block <paramref name="b" />. 
            /// </summary>
            /// <param name="bs">The block in which the identifier was changed</param>
            /// <param name="sid">The identifier after being SSA transformed.</param>
            /// <param name="performProbe">if true, looks "backwards" to see
            ///   if <paramref name="id"/> overlaps with another identifier</param>
            /// <returns></returns>
            public virtual Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                if (performProbe)
                {
                    // Did a previous SSA id modify the same storage as id?
                    SsaIdentifier sidPrev = ReadVariable(bs, performProbe);
                    if (sidPrev != null)
                    {
                        // Was the previous modification larger than this modification?
                        if (sidPrev.Identifier.Storage.Exceeds(id.Storage))
                        {
                            // Generate a DPB so the previous modification "shines
                            // through".
                            var sidPrevOld = sidPrev;
                            var dpb = new DepositBits(sidPrev.Identifier, sid.Identifier, (int)id.Storage.BitAddress);
                            var ass = new AliasAssignment(sidPrev.OriginalIdentifier, dpb);
                            sidPrev = InsertAfterDefinition(sid.DefStatement, ass);
                            sidPrevOld.Uses.Add(sidPrev.DefStatement);

                            var alias = new AliasState(sidPrev);
                            alias.Aliases.Add(id, sid);
                            //Debug.Print("--- {0}: {1}", bs.Block.Name, sid.Identifier.Name);
                            bs.currentDef[id.Storage.Domain] = alias;
                            return sid.Identifier;
                        }
                    }
                }
                if (!(sid.DefStatement.Instruction is AliasAssignment))
                {
                    //Debug.Print("--- {0}: {1}", bs.Block.Name, sid.Identifier.Name);
                    bs.currentDef[id.Storage.Domain] = new AliasState(sid);
                }
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
            public SsaIdentifier ReadVariable(SsaBlockState bs, bool aliasProbe)
            {
                if (id.Name == "bx") id.ToString(); //$DEBUG
                var sid = ReadBlockLocalVariable(bs, aliasProbe);
                if (sid != null)
                    return sid;
                // Keep probin'.
                return ReadVariableRecursive(bs, aliasProbe);
            }

            public abstract SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe);

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
                    val = AddPhiOperands(val, aliasProbe);
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
            protected SsaIdentifier MaybeGenerateAliasStatement(AliasState alias, bool aliasProbe)
            {
                var b = alias.SsaId.DefStatement.Block;
                var sidFrom = alias.SsaId;
                var stgTo = id.Storage;
                Storage stgFrom = sidFrom.Identifier.Storage;
                if (stgFrom == stgTo ||
                    (aliasProbe && stgFrom.Exceeds(stgTo)))
                {
                    alias.Aliases[id] = sidFrom;
                    return alias.SsaId;
                }

                Expression e = null;
                SsaIdentifier sidUse;
                if (stgFrom.Covers(stgTo))
                {
                    int offset = stgFrom.OffsetOf(stgTo);
                    if (offset > 0)
                        e = new Slice(id.DataType, sidFrom.Identifier, (uint)offset);
                    else
                        e = new Cast(id.DataType, sidFrom.Identifier);
                    sidUse = alias.SsaId;
                }
                else
                {
                    sidUse = ReadVariableRecursive(blockstates[alias.SsaId.DefStatement.Block], false);
                    e = new DepositBits(sidUse.Identifier, alias.SsaId.Identifier, (int)stgFrom.BitAddress);
                }
                var ass = new AliasAssignment(id, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                alias.Aliases[id] = sidAlias;
                return sidAlias;
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
                while (i < b.Statements.Count - 1 && b.Statements[i + 1].Instruction is AliasAssignment)
                    ++i;
                var stm = new Statement(stmBefore.LinearAddress, ass, stmBefore.Block);
                stmBefore.Block.Statements.Insert(i + 1, stm);

                var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
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

            private SsaIdentifier AddPhiOperands(SsaIdentifier phi, bool aliasProbe)
            {
                // Determine operands from predecessors.
                var preds = phi.DefStatement.Block.Pred;

                if (preds.Any(p => !blockstates[p].Visited))
                {
                    // Haven't visited some of the predecessors yet,
                    // so we can't backwalk... yet. 
                    ((PhiAssignment)phi.DefStatement.Instruction).Src =
                                new PhiFunction(phi.Identifier.DataType, new Expression[preds.Count]);
                    outer.incompletePhis.Add(phi);
                    return phi;
                }
                return AddPhiOperandsCore(phi, aliasProbe);
            }

            public SsaIdentifier AddPhiOperandsCore(SsaIdentifier phi, bool aliasProbe)
            {
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
                        phi.Identifier.DataType,
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
                var phiFunc = ((PhiAssignment)phi.DefStatement.Instruction).Src;
                foreach (Identifier op in phiFunc.Arguments)
            {
                    if (!firstTime && (op != same && op != phi.Identifier))
                {
                        // A real phi; use all its arguments.
                        UsePhiArguments(phi, phiFunc);
                        return phi;
                }
                    firstTime = false;
                    if (op != phi.Identifier)
                {
                        same = op;
                }
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

            private void UsePhiArguments(SsaIdentifier phi, PhiFunction phiFunc)
            {
                foreach (Identifier id in phiFunc.Arguments)
                {
                    ssaIds[id].Uses.Add(phi.DefStatement);
                }
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
                foreach (var use in sidOld.Uses.ToList())
                {
                    use.Instruction.Accept(new IdentifierReplacer(this.ssaIds, use, sidOld.Identifier, idNew));
                }
            }
        }

        public class SsaRegisterTransformer : SsaIdentifierTransformer
        {
            public SsaRegisterTransformer(Identifier id, Statement stm, SsaTransform2 outer)
                : base(id, stm, outer)
            {
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe)
            {
                AliasState alias;
                if (bs.currentDef.TryGetValue(id.Storage.Domain, out alias))
                {
                    // Defined locally in this block.
                    // Has the alias already been calculated?
                    SsaIdentifier ssaId = alias.SsaId;
                    if (alias.SsaId.OriginalIdentifier == id ||
                        alias.Aliases.TryGetValue(id, out ssaId))
                    {
                        return ssaId;
                    }

                    // Does ssaId intersect the probed value?
                    if (alias.SsaId.Identifier.Storage.OverlapsWith(id.Storage))
                    {
                        return MaybeGenerateAliasStatement(alias, aliasProbe);
                    }
                }
                return null;
            }
        }

        public class SsaFlagTransformer : SsaIdentifierTransformer
        {
            private uint flagMask;
            private FlagGroupStorage flagGroup;

            public SsaFlagTransformer(Identifier id, FlagGroupStorage flagGroup, Statement stm, SsaTransform2 outer)
                : base(id, stm, outer)
            {
                this.flagGroup = flagGroup;
            }

            public override Expression NewUse(SsaBlockState bs)
            {
                // Analyze each flag in the flag group separately.
                var ids = new Dictionary<Identifier, SsaIdentifier>();
                foreach (uint flagBitMask in flagGroup.GetFlagBitMasks())
                {
                    this.flagMask = flagBitMask;
                    this.id = outer.ssa.Procedure.Frame.EnsureFlagGroup(outer.arch.GetFlagGroup(flagMask));
                    var sid = ReadVariable(bs, false);
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

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                bs.currentFlagDef[flagMask] = sid;
                return sid.Identifier;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe)
            {
                SsaIdentifier ssaId;
                if (bs.currentFlagDef.TryGetValue(flagMask, out ssaId))
                {
                    // Defined locally in this block.
                    return ssaId;
                }
                return null;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                foreach (uint flagBitMask in flagGroup.GetFlagBitMasks())
                {
                    this.flagMask = flagBitMask;
                    WriteVariable(bs, sid, true);
                }
                return sid.Identifier;
            }
        }

        public class SsaStackTransformer : SsaIdentifierTransformer
        {
            private int stackOffset;

            public SsaStackTransformer(
                Identifier id,
                int stackOffset,
                Statement stm,
                SsaTransform2 outer)
                : base(id, stm, outer)
            {
                this.stackOffset = stackOffset;
            }

            public override Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs, false);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe)
            {
                SsaIdentifier ssaId;
                if (bs.currentStackDef.TryGetValue(stackOffset, out ssaId))
                {
                    // Defined locally in this block.
                    return ssaId;
                }
                return null;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                bs.currentStackDef[stackOffset] = sid;
                return sid.Identifier;
            }
        }

        public class SsaSequenceTransformer : SsaIdentifierTransformer
        {
            private SequenceStorage seq;

            public SsaSequenceTransformer(
                Identifier id,
                SequenceStorage seq,
                Statement stm,
                SsaTransform2 outer)
                : base(id, stm, outer)
            {
                this.seq = seq;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe)
            {
                var ss = outer.factory.Create(seq.Head, stm);
                var head = ss.ReadVariable(bs, false);
                ss = outer.factory.Create(seq.Tail, stm);
                var tail = ss.ReadVariable(bs, false);
                return Fuse(head, tail);
            }

            public SsaIdentifier Fuse(SsaIdentifier head, SsaIdentifier tail)
            {
                AliasAssignment assHead, assTail;
                if (head.DefStatement.Instruction.As(out assHead) &&
                    tail.DefStatement.Instruction.As(out assTail))
                {
                    Slice eHead;
                    Cast eTail;
                    if (assHead.Src.As(out eHead) && assTail.Src.As(out eTail))
                    {
                        return ssaIds[(Identifier)eHead.Expression];
                    }
                }
                throw new NotImplementedException();
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                var ss = outer.factory.Create(seq.Head, stm);
                ss.WriteVariable(bs, sid, performProbe);
                ss = outer.factory.Create(seq.Tail, stm);
                ss.WriteVariable(bs, sid, performProbe);
                return sid.Identifier;
            }
        }

        public class FpuStackTransformer : SsaIdentifierTransformer
        {
            private FpuStackStorage fpu;

            public FpuStackTransformer(Identifier id, FpuStackStorage fpu, Statement stm, SsaTransform2 outer) : base(id, stm, outer)
            {
                this.fpu = fpu;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                bs.currentFpuDef[fpu.FpuStackOffset] = sid;
                return base.NewDef(bs, sid);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs, bool aliasProbe)
            {
                SsaIdentifier sid;
                bs.currentFpuDef.TryGetValue(fpu.FpuStackOffset, out sid);
                return sid;
            }
        }

        public class SsaApplicationBuilder : ApplicationBuilder
        {
            private SsaTransform2 sst;

            public SsaApplicationBuilder(
                SsaTransform2 sst,
                CallSite site,
                Expression callee)
                : base(site, callee)
            {
                this.sst = sst;
            }

            public override Expression Bind(Identifier id)
            {
                return sst.VisitIdentifier(id);
            }

            public override Identifier BindReturnValue(Identifier id)
            {
                return sst.NewDef(id, callee, false);
            }

            public override OutArgument BindOutArg(Identifier id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
