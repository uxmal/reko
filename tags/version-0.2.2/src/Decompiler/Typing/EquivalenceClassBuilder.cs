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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Typing
{
	/// <summary>
	/// Assigns a type to each expression node in a program and builds equivalence classes.
	/// </summary>
	public class EquivalenceClassBuilder : InstructionVisitorBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private ProcedureSignature signature;
        private Dictionary<ushort, TypeVariable> segTypevars;

		public EquivalenceClassBuilder(TypeFactory factory, TypeStore store)
		{
			this.factory = factory;
			this.store = store;
			this.signature = null;
            this.segTypevars = new Dictionary<ushort, TypeVariable>();
		}

		public void Build(Program prog)
		{
			EnsureTypeVariable(prog.Globals, "Globals");
			
			foreach (Procedure proc in prog.Procedures.Values)
			{
                ProcedureSignature signature = proc.Signature;
                if (signature != null)
                {
                    if (signature.ReturnValue != null)
                    {
                        signature.ReturnValue.Accept(this);
                    }
                }
				foreach (Block block in proc.RpoBlocks)
				{
					foreach (Statement stm in block.Statements)
					{
						stm.Instruction.Accept(this);
					}
				}
			}
		}

		public TypeVariable EnsureTypeVariable(Expression e)
		{
			return store.EnsureExpressionTypeVariable(factory, e);
		}

		public TypeVariable EnsureTypeVariable(Expression e, string name)
		{
			return store.EnsureExpressionTypeVariable(factory, e, name);
		}


		public override void VisitApplication(Application appl)
		{
			signature = null;
			appl.Procedure.Accept(this);
			ProcedureSignature sig = signature;

			if (sig != null)
			{
				if (sig.FormalArguments.Length != appl.Arguments.Length)
					throw new InvalidOperationException("Parameter count must match.");
			}

			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
				if (sig != null)
				{
					EnsureTypeVariable(sig.FormalArguments[i]);
					store.MergeClasses(appl.Arguments[i].TypeVariable, sig.FormalArguments[i].TypeVariable);
				}
			}
			EnsureTypeVariable(appl);
		}

		public override void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
			EnsureTypeVariable(acc);
		}

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
			a.Dst.Accept(this);
			store.MergeClasses(a.Dst.TypeVariable, a.Src.TypeVariable);
		}


		public override void VisitStore(Store s)
		{
			s.Src.Accept(this);
			s.Dst.Accept(this);
			store.MergeClasses(s.Dst.TypeVariable, s.Src.TypeVariable);
		}

		public override void VisitBinaryExpression(BinaryExpression binExp)
		{
			if (binExp.Left.ToString().IndexOf("ax_349") >= 0)
				binExp.ToString();

			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
			if (binExp.op is ConditionalOperator)
			{
				store.MergeClasses(binExp.Left.TypeVariable, binExp.Right.TypeVariable);
			}			
			EnsureTypeVariable(binExp);
		}


		public override void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
			EnsureTypeVariable(cast);
		}

		public override void VisitConstant(Constant c)
		{
            if (c.DataType == PrimitiveType.SegmentSelector)
            {
                TypeVariable tv;
                if (segTypevars.TryGetValue(c.ToUInt16(), out tv))
                {
                    c.TypeVariable = tv;
                }
                else
                {
                    EnsureTypeVariable(c);
                    segTypevars[c.ToUInt16()] = c.TypeVariable;
                }
                return;
            }
			EnsureTypeVariable(c);
		}

		public override void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
			EnsureTypeVariable(cof);
		}

		public override void VisitDeclaration(Declaration decl)
		{
			decl.Identifier.Accept(this);
			if (decl.Expression != null)
			{
				decl.Expression.Accept(this);
				store.MergeClasses(decl.Identifier.TypeVariable, decl.Expression.TypeVariable);
			}
		}

		public override void VisitDefInstruction(DefInstruction def)
		{
		}

		public override void VisitDepositBits(DepositBits d)
		{
			d.Source.Accept(this);
			d.InsertedBits.Accept(this);
			EnsureTypeVariable(d);
		}

		public override void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
			EnsureTypeVariable(deref);
		}

		public override void VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

		public override void VisitIdentifier(Identifier id)
		{
			EnsureTypeVariable(id);
		}

		public override void VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

		public override void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
			EnsureTypeVariable(mps);
		}


		public override void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
			EnsureTypeVariable(access);
		}

		public override void VisitMkSequence(MkSequence seq)
		{
			seq.Head.Accept(this);
			seq.Tail.Accept(this);
			EnsureTypeVariable(seq);
		}


		public override void VisitSegmentedAccess(SegmentedAccess access)
		{
			access.BasePointer.Accept(this);
			access.EffectiveAddress.Accept(this);
			EnsureTypeVariable(access);
		}

		public override void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			store.MergeClasses(phi.Src.TypeVariable, phi.Dst.TypeVariable);
		}

		public override void VisitPhiFunction(PhiFunction phi)
		{
			TypeVariable tPhi = EnsureTypeVariable(phi);
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				phi.Arguments[i].Accept(this);
				store.MergeClasses(tPhi, phi.Arguments[i].TypeVariable);
			}
		}

		public override void VisitProcedureConstant(ProcedureConstant pc)
		{
			EnsureTypeVariable(pc);
			VisitProcedure(pc.Procedure);
			if (pc.Procedure.Signature != null)
			{
				store.MergeClasses(pc.TypeVariable, pc.Procedure.Signature.TypeVariable);
				signature = pc.Procedure.Signature;
			}
		}

		public void VisitProcedure(ProcedureBase proc)
		{
			if (proc.Signature != null)
			{
				if (proc.Signature.TypeVariable == null)
				{
					proc.Signature.TypeVariable = store.EnsureExpressionTypeVariable(
						factory,
						new Identifier("signature of " + proc.Name, 0, null, null),
						null);
				}
				if (proc.Signature.FormalArguments != null)
				{
					foreach (Identifier id in proc.Signature.FormalArguments)
					{
						id.Accept(this);
					}
				}
				//$REVIEW: return type?
			}
		}

		public override void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
			EnsureTypeVariable(slice);
		}

		public override void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
			EnsureTypeVariable(tc);
		}

		public override void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
			EnsureTypeVariable(unary);
		}
	}
}
