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
using System.Collections.Generic;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Finds linear induction variables and annotates the identifiers with that information.
	/// </summary>
	public class LinearInductionVariableFinder : InstructionVisitorBase, ISccFinderHost<SsaIdentifier>
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private DominatorGraph doms;
		private ICollection<SsaIdentifier> operands;
        private List<LinearInductionVariable> ivs;
        private Dictionary<LinearInductionVariable, LinearInductionVariableContext> contexts;
        private LinearInductionVariableContext ctx;

		public LinearInductionVariableFinder(Procedure proc, SsaIdentifierCollection ssaIds, DominatorGraph doms)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
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

		public LinearInductionVariable CreateInductionVariable()
		{
			if (ctx.PhiStatement == null) return null;
			if (ctx.PhiIdentifier == null) return null;
			if (ctx.DeltaValue == null) return null;

			SsaIdentifier sidPhi = ssaIds[ctx.PhiIdentifier];
			if (ctx.TestStatement == null && ctx.InitialValue == null)
			{
				return new LinearInductionVariable(null, ctx.DeltaValue, null, false);
			}
			if (ctx.InitialValue != null)
			{
				if (IsIdUsedOnlyBy(ctx.PhiIdentifier, ctx.TestStatement, ctx.DeltaStatement))
				{
					// The only use inside the loop is the increment, so we never see the initial value.
					ctx.InitialValue = Operator.add.ApplyConstants(ctx.InitialValue, ctx.DeltaValue);
				}
			}

            ctx.TestValue = AdjustTestValue(ctx.TestValue);

            return ctx.CreateInductionVariable();
		}



        public Constant AdjustTestValue(Constant testValue)
        {
            if (testValue == null)
                return null; 

            // <= or >= operators imply an extra spin around the loop.

            if (RelEq(ctx.TestOperator) &&
                DominatesAllUses(ctx.TestStatement, ctx.PhiIdentifier))
            {
                testValue = Operator.add.ApplyConstants(testValue, ctx.DeltaValue);
            }
            Identifier idNew = (Identifier) ((Assignment) ctx.DeltaStatement.Instruction).Dst;
            if (!IsSingleUsingStatement(ctx.TestStatement, idNew))
            {
                if (!(IsSingleUsingStatement(ctx.PhiStatement, idNew) &&
                    DominatesAllUses(ctx.TestStatement, ctx.PhiIdentifier)))
                {
                    // A use is made of the variable between increment and test.
                    testValue = Operator.add.ApplyConstants(testValue, ctx.DeltaValue);
                }
            }
            return testValue;
        }

        /// <summary>
        /// Operator is a relation-equals operator.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private bool RelEq(Operator op)
        {
            return op == Operator.le || op == Operator.ge ||
                   op == Operator.ule || op == Operator.uge;
        }

		public bool DominatesAllUses(Statement stm, Identifier id)
		{
			SsaIdentifier sid = ssaIds[id];
			foreach (Statement u in sid.Uses)
			{
				if (u != stm)
				{
					if (!doms.DominatesStrictly(stm, u))
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
			SccFinder<SsaIdentifier> sccFinder = new SccFinder<SsaIdentifier>(this);
			foreach (SsaIdentifier sid in ssaIds)
			{
				sccFinder.FindOld(sid);
			}
		}

		public Constant FindFinalValue(ICollection<SsaIdentifier> scc)
		{
			foreach (SsaIdentifier sid in scc)
			{
				foreach (Statement u in sid.Uses)
				{
					Branch b = u.Instruction as Branch;
					if (b != null)
					{
						BinaryExpression bin = b.Condition as BinaryExpression;
						if (bin != null && bin.op is ConditionalOperator)
						{
							ctx.TestOperator = bin.op;
							ctx.TestStatement = u;
							ctx.TestValue = bin.Right as Constant;
							return ctx.TestValue;
						}
					}
				}
			}
			return null;
		}

		public Constant FindInitialValue(PhiFunction phi)
		{
			if (phi.Arguments.Length > 2)
				return null;
			Identifier id0 = (Identifier)phi.Arguments[0];
			Identifier id1 = (Identifier)phi.Arguments[1];
			if (id0.Number > id1.Number)
			{
                id0 = id1;
			}
			SsaIdentifier sid = ssaIds[id0];
			if (sid.DefStatement == null)
				return null;

			Assignment ass = sid.DefStatement.Instruction as Assignment;
			if (ass == null)
				return null;

			if (ass.Dst != id0)
				return null;

            ctx.InitialStatement = sid.DefStatement;
			ctx.InitialValue = ass.Src as Constant;
			return ctx.InitialValue;
		}

		public Constant FindLinearIncrement(ICollection<SsaIdentifier> sids)
		{
            foreach (SsaIdentifier sid in sids)
            {
                if (sid.DefStatement == null)
                    continue;
                Assignment ass = sid.DefStatement.Instruction as Assignment;
                if (ass == null)
                    continue;
                BinaryExpression bin = ass.Src as BinaryExpression;
                if (bin != null && (bin.op == Operator.add || bin.op == Operator.sub))
                {
                    Identifier idLeft = bin.Left as Identifier;
                    if (idLeft != null && IsSccMember(idLeft, sids))
                    {
                        Constant c = bin.Right as Constant;
                        if (c != null)
                        {
                            ctx.DeltaStatement = sid.DefStatement;
                            ctx.DeltaValue = (bin.op == Operator.sub)
                                ? c.Negate()
                                : c;
                            return ctx.DeltaValue;
                        }
                    }

                }
            }
			return null;
		}

		public PhiFunction FindPhiFunction(ICollection<SsaIdentifier> sids)
		{
            foreach (SsaIdentifier sid in sids)
            {
                if (sid.DefStatement == null)
                    continue;
                PhiAssignment phi = sid.DefStatement.Instruction as PhiAssignment;
                if (phi != null)
                {
                    ctx.PhiStatement = sid.DefStatement;
                    ctx.PhiIdentifier = (Identifier) phi.Dst;
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
			SsaIdentifier sid = ssaIds[id];
			return sid.Uses.Count == 1 && sid.Uses[0] == stm;
		}


		public bool IsIdUsedOnlyBy(Identifier id, Statement stm1, Statement stm2)
		{
			SsaIdentifier sid = ssaIds[id];
			foreach (Statement u in sid.Uses)
			{
				if (u != stm1 && u != stm2)
					return false;
			}
			return true;
		}

		public bool IsSccMember(Identifier id, ICollection<SsaIdentifier> sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.Identifier == id)
					return true;
			}
			return false;
		}

		#region InstructionVisitor members //////////////////////

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

		public override void VisitIdentifier(Identifier id)
		{
			operands.Add(ssaIds[id]);
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

		#region ISccFinderHost Members //////////////////////

        public IEnumerable<SsaIdentifier> GetSuccessors(SsaIdentifier sidDef)
        {
            this.operands = new List<SsaIdentifier>();
            if (sidDef.DefStatement != null)
            {
                sidDef.DefStatement.Instruction.Accept(this);
            }
            return operands;
        }


		public virtual void ProcessScc(IList<SsaIdentifier> scc)
		{
			if (scc.Count <= 1)
				return;

            ctx = new LinearInductionVariableContext();
			PhiFunction phi = FindPhiFunction(scc);
			if (phi == null)
				return;
			ctx.DeltaValue = FindLinearIncrement(scc);
			if (ctx.DeltaValue == null)
				return;
			ctx.InitialValue = FindInitialValue(phi);
			ctx.TestValue = FindFinalValue(scc);
			LinearInductionVariable iv = CreateInductionVariable();
			if (iv != null)
			{
				foreach (SsaIdentifier sid in scc)
				{
					sid.InductionVariable = iv;
				}
				ivs.Add(iv);
                contexts.Add(iv, ctx);
			}
		}

		#endregion
	}
}
