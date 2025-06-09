#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Finds linear induction variables and annotates the identifiers with that information.
    /// </summary>
    public class LinearInductionVariableFinder : InstructionVisitorBase
	{
        private readonly SsaState ssa;
        private readonly List<LinearInductionVariable> ivs;
        private readonly Dictionary<LinearInductionVariable, LinearInductionVariableContext> contexts;
        private readonly BlockDominatorGraph doms;
        private LinearInductionVariableContext ctx;
        private ICollection<SsaIdentifier>? operands;

        public LinearInductionVariableFinder(SsaState ssa, BlockDominatorGraph doms)
		{
			this.ssa = ssa;
			this.doms = doms;
            this.ctx = new LinearInductionVariableContext();
            this.ivs = new List<LinearInductionVariable>();
            this.contexts = new Dictionary<LinearInductionVariable, LinearInductionVariableContext>();
        }

#if OSCAR_CAN_CODE
        tf6556i yjmuki7        
#endif
        public Dictionary<LinearInductionVariable, LinearInductionVariableContext> Contexts
        {
            get { return contexts; }
        }

        public LinearInductionVariableContext Context
        {
            get { return ctx; }
        }

		public LinearInductionVariable? CreateInductionVariable()
		{
			if (ctx.PhiStatement is null) return null;
			if (ctx.PhiIdentifier is null) return null;
			if (ctx.DeltaValue is null) return null;

			if (ctx.TestStatement is null && ctx.InitialValue is null)
			{
				return new LinearInductionVariable(null, ctx.DeltaValue, null, false);
			}
			if (ctx.InitialValue is not null)
			{
				if (IsIdUsedOnlyBy(ctx.PhiIdentifier, ctx.TestStatement, ctx.DeltaStatement))
				{
					// The only use inside the loop is the increment, so we never see the initial value.
					ctx.InitialValue = Operator.IAdd.ApplyConstants(ctx.InitialValue.DataType, ctx.InitialValue, ctx.DeltaValue);
				}
			}

            ctx.TestValue = AdjustTestValue(ctx.TestValue);

            return ctx.CreateInductionVariable();
		}

        public Constant? AdjustTestValue(Constant? testValue)
        {
            if (testValue is null)
                return null; 

            // <= or >= operators imply an extra spin around the loop.

            if (RelEq(ctx.TestOperator) &&
                DominatesAllUses(ctx.TestStatement, ctx.PhiIdentifier!) &&
                BranchTrueIntoLoop())
            {
                testValue = Operator.IAdd.ApplyConstants(testValue.DataType, testValue, ctx.DeltaValue!);
            }
            Identifier idNew = ((Assignment) ctx.DeltaStatement!.Instruction).Dst;
            if (!IsSingleUsingStatement(ctx.TestStatement!, idNew))
            {
                if (!(IsSingleUsingStatement(ctx.PhiStatement!, idNew) &&
                    DominatesAllUses(ctx.TestStatement, ctx.PhiIdentifier!)))
                {
                    // A use is made of the variable between increment and test.
                    testValue = Operator.IAdd.ApplyConstants(testValue.DataType, testValue, ctx.DeltaValue!);
                }
            }
            return testValue;
        }

        private bool BranchTrueIntoLoop()
        {
            return 
                ctx.TestStatement!.Block.ThenBlock ==
                ctx.PhiStatement!.Block;
        }

        /// <summary>
        /// Operator is a relation-equals operator.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private static bool RelEq(Operator? op)
        {
            if (op is null)
                return false;
            var opType = op.Type;
            return opType == OperatorType.Le || opType == OperatorType.Ge ||
                   opType == OperatorType.Ule || opType == OperatorType.Uge;
        }

		public bool DominatesAllUses(Statement? stm, Identifier id)
		{
			SsaIdentifier sid = ssa.Identifiers[id];
			foreach (Statement u in sid.Uses)
			{
				if (u != stm)
				{
					if (!doms.DominatesStrictly(stm!, u))
						return false;
				}
			}
			return true;
		}

        /// <summary>
        /// Find all linear induction variables in this procedure.
        /// </summary>
        public void Find()
        {
            var sccs = SccFinder.FindAll(new SsaGraph(ssa.Identifiers));
            foreach (var scc in sccs)
            {
                ProcessScc(scc);
            }
        }

		public Constant? FindFinalValue(ICollection<SsaIdentifier> scc)
		{
			foreach (SsaIdentifier sid in scc)
			{
				foreach (Statement u in sid.Uses)
				{
                    if (u.Instruction is not Branch b)
                        continue;
                    if (b.Condition is BinaryExpression bin && 
                        bin.Left is Identifier && 
                        bin.Operator is ConditionalOperator)
					{
						ctx.TestOperator = bin.Operator;
						ctx.TestStatement = u;
						ctx.TestValue = bin.Right as Constant;
						return ctx.TestValue;
					}
				}
			}
			return null;
		}

		public Constant? FindInitialValue(PhiFunction phi)
		{
			if (phi.Arguments.Length != 2)
				return null;
            var sid0 = ssa.Identifiers[(Identifier)phi.Arguments[0].Value];
            var sid1 = ssa.Identifiers[(Identifier)phi.Arguments[1].Value];
            if (sid0.DefStatement is null || sid1.DefStatement is null)
                return null;
            var sid = doms.DominatesStrictly(sid1.DefStatement, sid0.DefStatement)
                ? sid1 : sid0;
            if (sid.DefStatement.Instruction is not Assignment ass)
                return null;

            if (ass.Dst != sid.Identifier)
				return null;

            ctx.InitialStatement = sid.DefStatement;
			ctx.InitialValue = ass.Src as Constant;
			return ctx.InitialValue;
		}

		public Constant? FindLinearIncrement(ICollection<SsaIdentifier> sids)
		{
            foreach (SsaIdentifier sid in sids)
            {
                if (sid.DefStatement is null)
                    continue;
                if (sid.DefStatement.Instruction is not Assignment ass)
                    continue;
                if (ass.Src is BinaryExpression bin && 
                    bin.Operator.Type.IsAddOrSub())
                {
                    if (bin.Left is Identifier idLeft && IsSccMember(idLeft, sids))
                    {
                        if (bin.Right is Constant c)
                        {
                            ctx.DeltaStatement = sid.DefStatement;
                            ctx.DeltaValue = (bin.Operator.Type == OperatorType.ISub)
                                ? c.Negate()
                                : c;
                            return ctx.DeltaValue;
                        }
                    }
                }
            }
			return null;
		}

		public PhiFunction? FindPhiFunction(ICollection<SsaIdentifier> sids)
		{
            foreach (SsaIdentifier sid in sids)
            {
                if (sid.DefStatement is null)
                    continue;
                if (sid.DefStatement.Instruction is PhiAssignment phi)
                {
                    ctx.PhiStatement = sid.DefStatement;
                    ctx.PhiIdentifier = phi.Dst;
                    return phi.Src;
                }
            }
            return null;
		}

        public List<LinearInductionVariable> InductionVariables
		{
			get { return ivs; }
		}

		public bool IsSingleUsingStatement(Statement stm, Identifier id)
		{
			SsaIdentifier sid = ssa.Identifiers[id];
			return sid.Uses.Count == 1 && sid.Uses[0] == stm;
		}


		public bool IsIdUsedOnlyBy(Identifier id, Statement? stm1, Statement? stm2)
		{
			SsaIdentifier sid = ssa.Identifiers[id];
			foreach (Statement u in sid.Uses)
			{
				if (u != stm1 && u != stm2)
					return false;
			}
			return true;
		}

		public static bool IsSccMember(Identifier id, ICollection<SsaIdentifier> sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.Identifier == id)
					return true;
			}
			return false;
		}

        public IEnumerable<SsaIdentifier> GetSuccessors(SsaIdentifier sid)
        {
            this.operands = new List<SsaIdentifier>();
            if (sid.DefStatement is not null)
            {
                sid.DefStatement.Instruction.Accept(this);
            }
            return operands;
        }

        public class SsaGraph : InstructionVisitorBase, DirectedGraph<SsaIdentifier>
        {
            private readonly SsaIdentifierCollection ssaIds;
            private ICollection<SsaIdentifier>? operands;

            public SsaGraph(SsaIdentifierCollection ssaIds)
            {
                this.ssaIds = ssaIds;
            }

            #region DirectedGraph<SsaIdentifier> Members

            public ICollection<SsaIdentifier> Predecessors(SsaIdentifier node)
            {
                throw new NotImplementedException();
            }

            public ICollection<SsaIdentifier> Successors(SsaIdentifier sid)
            {
                this.operands = new List<SsaIdentifier>();
                if (sid.DefStatement is not null)
                {
                    sid.DefStatement.Instruction.Accept(this);
                }
                return operands;
            }

            public ICollection<SsaIdentifier> Nodes => ssaIds;

            public void AddEdge(SsaIdentifier nodeFrom, SsaIdentifier nodeTo)
            {
                throw new NotImplementedException();
            }

            public void RemoveEdge(SsaIdentifier nodeFrom, SsaIdentifier nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(SsaIdentifier nodeFrom, SsaIdentifier nodeTo)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region InstructionVisitor members //////////////////////

            public override void VisitAssignment(Assignment a)
            {
                a.Src.Accept(this);
            }

            public override void VisitIdentifier(Identifier id)
            {
                operands!.Add(ssaIds[id]);
            }

            public override void VisitSideEffect(SideEffect side)
            {
                side.Expression.Accept(this);
            }

            public override void VisitMemoryAccess(MemoryAccess access)
            {
                access.EffectiveAddress.Accept(this);
            }

            #endregion 


        }

		#region InstructionVisitor members //////////////////////

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

		public override void VisitIdentifier(Identifier id)
		{
			operands!.Add(ssa.Identifiers[id]);
		}

		public override void VisitSideEffect(SideEffect side)
		{
			side.Expression.Accept(this);
		}

		public override void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
		}

		#endregion 

		public virtual void ProcessScc(IList<SsaIdentifier> scc)
		{
			if (scc.Count <= 1)
				return;

            ctx = new LinearInductionVariableContext();
			PhiFunction? phi = FindPhiFunction(scc);
			if (phi is null)
				return;
			ctx.DeltaValue = FindLinearIncrement(scc);
			if (ctx.DeltaValue is null)
				return;
			ctx.InitialValue = FindInitialValue(phi);
			ctx.TestValue = FindFinalValue(scc);
			LinearInductionVariable? iv = CreateInductionVariable();
			if (iv is not null)
			{
				foreach (SsaIdentifier sid in scc)
				{
					sid.InductionVariable = iv;
				}
				ivs.Add(iv);
                contexts.Add(iv, ctx);
			}
		}
	}
}
