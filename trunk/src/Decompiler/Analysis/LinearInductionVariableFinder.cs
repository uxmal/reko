/* 
 * Copyright (C) 1999-2009 John Källén.
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
		private Statement stmInc;
		private Statement stmPhi;
		private Statement stmTest;
		private Identifier idPhi;
		private Constant valInit;
		private Constant valDelta;
		private Constant valTest;
		private Operator testOperator;

		public LinearInductionVariableFinder(Procedure proc, SsaIdentifierCollection ssaIds, DominatorGraph doms)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			this.doms = doms;
            this.ivs = new List<LinearInductionVariable>();
        }

		public Constant InitialValue
		{
			get { return valInit; }
			set { valInit = value; }
		}

		public Constant DeltaValue
		{
			get { return valDelta; }
			set { valDelta = value; }
		}

		public Statement DeltaStatement
		{
			get { return stmInc; }
			set { stmInc = value; }
		}

		public Statement PhiStatement
		{
			get { return stmPhi; }
			set { stmPhi = value; }
		}

		public Identifier PhiIdentifier
		{
			get { return idPhi; }
			set { idPhi = value; }
		}

		public Operator TestOperator
		{
			get { return testOperator; }
			set { testOperator = value; }
		}

		public Statement TestStatement
		{
			get { return stmTest; }
			set { stmTest = value; }
		}

		public Constant TestValue
		{
			get { return valTest; }
			set { valTest = value; }
		}

		public LinearInductionVariable CreateInductionVariable()
		{
			if (PhiStatement == null) return null;
			if (PhiIdentifier == null) return null;
			if (DeltaValue == null) return null;

			SsaIdentifier sidPhi = ssaIds[PhiIdentifier];
			if (TestStatement == null && InitialValue == null)
			{
				return new LinearInductionVariable(null, DeltaValue, null, false);
			}
			if (InitialValue != null)
			{
				if (IsIdUsedOnlyBy(PhiIdentifier, TestStatement, DeltaStatement))
				{
					// The only use inside the loop is the increment, so we never see the initial value.
					InitialValue = Operator.add.ApplyConstants(InitialValue, DeltaValue);
				}
			}

            TestValue = AdjustTestValue(TestValue);

			return new LinearInductionVariable(InitialValue, DeltaValue, TestValue, IsSignedOperator(testOperator));
		}

        private bool IsSignedOperator(Operator op)
        {
            return 
                op == Operator.lt || op == Operator.le ||
                op == Operator.gt || op == Operator.ge;
        }

        public Constant AdjustTestValue(Constant testValue)
        {
            if (testValue == null)
                return null; 

            // <= or >= operators imply an extra spin around the loop.

            if (RelEq(TestOperator) &&
                DominatesAllUses(TestStatement, PhiIdentifier))
            {
                testValue = Operator.add.ApplyConstants(testValue, DeltaValue);
            }
            Identifier idNew = (Identifier) ((Assignment) DeltaStatement.Instruction).Dst;
            if (!IsSingleUsingStatement(TestStatement, idNew))
            {
                if (!(IsSingleUsingStatement(PhiStatement, idNew) &&
                    DominatesAllUses(TestStatement, PhiIdentifier)))
                {
                    // A use is made of the variable between increment and test.
                    testValue = Operator.add.ApplyConstants(testValue, DeltaValue);
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

		public void Find()
		{
			SccFinder<SsaIdentifier> sccFinder = new SccFinder<SsaIdentifier>(this);
			foreach (SsaIdentifier sid in ssaIds)
			{
				sccFinder.Find(sid);
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
							TestOperator = bin.op;
							TestStatement = u;
							TestValue = bin.Right as Constant;
							return TestValue;
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
				Identifier t = id0; id0 = id1; id1 = t;
			}
			SsaIdentifier sid = ssaIds[id0];
			if (sid.DefStatement == null)
				return null;

			Assignment ass = sid.DefStatement.Instruction as Assignment;
			if (ass == null)
				return null;

			if (ass.Dst != id0)
				return null;

			InitialValue = ass.Src as Constant;
			return InitialValue;
		}

		public Constant FindLinearIncrement(ICollection<SsaIdentifier> sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.DefStatement != null)
				{
					Assignment ass = sid.DefStatement.Instruction as Assignment;
					if (ass != null)
					{
						BinaryExpression bin = ass.Src as BinaryExpression;
						if (bin != null && (bin.op == Operator.add || bin.op == Operator.sub))
						{
							Identifier idLeft = bin.Left as Identifier;
							if (idLeft != null && IsSccMember(idLeft, sids))
							{
								Constant c = bin.Right as Constant;
								if (c != null)
								{
									DeltaStatement = sid.DefStatement;
									DeltaValue = (bin.op == Operator.sub)
										? c.Negate()
										: c;
									return DeltaValue;
								}
							}
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
				if (sid.DefStatement != null)
				{
					PhiAssignment phi = sid.DefStatement.Instruction as PhiAssignment;
					if (phi != null)
					{
						PhiStatement = sid.DefStatement;
						PhiIdentifier = (Identifier) phi.Dst;
						return phi.Src;
					}
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

		public void AddSuccessors(SsaIdentifier sidDef, ICollection<SsaIdentifier> operands)
		{
			this.operands = operands;
			if (sidDef.DefStatement != null)
			{
				sidDef.DefStatement.Instruction.Accept(this);
			}
		}


		public virtual void ProcessScc(ICollection<SsaIdentifier> scc)
		{
			if (scc.Count <= 1)
				return;
			PhiFunction phi = FindPhiFunction(scc);
			if (phi == null)
				return;
			DeltaValue = FindLinearIncrement(scc);
			if (DeltaValue == null)
				return;

			InitialValue = FindInitialValue(phi);
			TestValue = FindFinalValue(scc);
			LinearInductionVariable iv = CreateInductionVariable();
			if (iv != null)
			{
				foreach (SsaIdentifier sid in scc)
				{
					sid.InductionVariable = iv;
				}
				ivs.Add(iv);
			}
		}

		#endregion
	}
}
