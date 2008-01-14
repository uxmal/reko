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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using System;
using System.Collections;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Finds linear induction variables and annotates the identifiers with that information.
	/// </summary>
	public class LinearInductionVariableFinder : InstructionVisitorBase, ISccFinderHost
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private DominatorGraph doms;
		private ArrayList operands;
		private ArrayList ivs;
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
			this.ivs = new ArrayList();
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
				return new LinearInductionVariable(null, DeltaValue, null);
			}
			if (InitialValue != null)
			{
				if (IsIdUsedOnlyBy(PhiIdentifier, TestStatement, DeltaStatement))
				{
					// The only use inside the loop is the increment, so we never see the initial value.
					InitialValue = Operator.add.ApplyConstants(InitialValue, DeltaValue);
				}
			}

			if (TestValue != null)
			{
				// <= or >= operators imply an extra spin around the loop.

				if (TestOperator == Operator.le || TestOperator == Operator.ge)
				{
					TestValue = Operator.add.ApplyConstants(TestValue, DeltaValue);
				}
				Identifier idNew = (Identifier) ((Assignment) DeltaStatement.Instruction).Dst;
				if (!IsSingleUsingStatement(TestStatement, idNew))
				{
					if (!(IsSingleUsingStatement(PhiStatement, idNew) && 
						DominatesAllUses(TestStatement, PhiIdentifier)))
					{
						// A use is made of the variable between increment and test.
						TestValue = Operator.add.ApplyConstants(TestValue, DeltaValue);
					}
				}
			}
			return new LinearInductionVariable(InitialValue, DeltaValue, TestValue);
		}

		public bool DominatesAllUses(Statement stm, Identifier id)
		{
			SsaIdentifier sid = ssaIds[id];
			foreach (Statement u in sid.uses)
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
			SccFinder sccFinder = new SccFinder(this);
			foreach (SsaIdentifier sid in ssaIds)
			{
				sccFinder.Find(sid);
			}
		}

		public Constant FindFinalValue(ArrayList scc)
		{
			foreach (SsaIdentifier sid in scc)
			{
				foreach (Statement u in sid.uses)
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
			if (sid.def == null)
				return null;

			Assignment ass = sid.def.Instruction as Assignment;
			if (ass == null)
				return null;

			if (ass.Dst != id0)
				return null;

			InitialValue = ass.Src as Constant;
			return InitialValue;
		}

		public Constant FindLinearIncrement(ArrayList sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.def != null)
				{
					Assignment ass = sid.def.Instruction as Assignment;
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
									DeltaStatement = sid.def;
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

		public PhiFunction FindPhiFunction(ArrayList sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.def != null)
				{
					PhiAssignment phi = sid.def.Instruction as PhiAssignment;
					if (phi != null)
					{
						PhiStatement = sid.def;
						PhiIdentifier = (Identifier) phi.Dst;
						return phi.Src;
					}
				}
			}
			return null;
		}

		public ArrayList InductionVariables
		{
			get { return ivs; }
		}

		public bool IsSingleUsingStatement(Statement stm, Identifier id)
		{
			SsaIdentifier sid = ssaIds[id];
			return sid.uses.Count == 1 && sid.uses[0] == stm;
		}


		public bool IsIdUsedOnlyBy(Identifier id, Statement stm1, Statement stm2)
		{
			SsaIdentifier sid = ssaIds[id];
			foreach (Statement u in sid.uses)
			{
				if (u != stm1 && u != stm2)
					return false;
			}
			return true;
		}

		public bool IsSccMember(Identifier id, ArrayList sids)
		{
			foreach (SsaIdentifier sid in sids)
			{
				if (sid.id == id)
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

		public void AddSuccessors(object o, ArrayList operands)
		{
			this.operands = operands;
			SsaIdentifier sidDef = (SsaIdentifier) o;
			if (sidDef.def != null)
			{
				sidDef.def.Instruction.Accept(this);
			}
		}


		public virtual void ProcessScc(ArrayList scc)
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
					sid.iv = iv;
				}
				ivs.Add(iv);
			}
		}

		#endregion
	}
}
