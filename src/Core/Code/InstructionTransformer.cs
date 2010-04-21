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

using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Base class for rebuilding instructions -- and expressions therein. Visits an instruction
	/// and replaces identifiers with new values provided by the Rename abstract method.
	/// </summary>
	public class InstructionTransformer : IExpressionTransformer
	{
		public InstructionTransformer()
		{
		}


		public Instruction Transform(Instruction instr)
		{
			return instr.Accept(this);
		}

		#region InstructionTransformer Members

		public virtual Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			a.Dst = (Identifier) a.Dst.Accept(this);
			return a;
		}

		public virtual Instruction TransformBranch(Branch b)
		{
			b.Condition = b.Condition.Accept(this);
			return b;
		}

		public virtual Instruction TransformCallInstruction(CallInstruction ci)
		{
			return ci;
		}

		public virtual Instruction TransformDeclaration(Declaration decl)
		{
			if (decl.Expression != null)
				decl.Expression = decl.Expression.Accept(this);
			return decl;
		}

		public virtual Instruction TransformDefInstruction(DefInstruction def)
		{
			def.Expression = def.Expression.Accept(this);
			return def;
		}

		public virtual Instruction TransformIndirectCall(IndirectCall ic)
		{
			ic.Callee = ic.Callee.Accept(this);
			return ic;
		}

		public virtual Instruction TransformPhiAssignment(PhiAssignment phi)
		{
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
				phi.Src.Arguments[i] = phi.Src.Arguments[i].Accept(this);
			}
			phi.Dst = (Identifier) phi.Dst.Accept(this);
			return phi;
		}

		public virtual Instruction TransformReturnInstruction(ReturnInstruction ret)
		{
			if (ret.Expression != null)
				ret.Expression = ret.Expression.Accept(this);
			return ret;
		}

		public virtual Instruction TransformSideEffect(SideEffect side)
		{
			side.Expression = side.Expression.Accept(this);
			return side;
		}

		public virtual Instruction TransformStore(Store store)
		{
			store.Src = store.Src.Accept(this);
			store.Dst = store.Dst.Accept(this);
			return store;
		}

		public virtual Instruction TransformSwitchInstruction(SwitchInstruction si)
		{
			si.Expression = si.Expression.Accept(this);
			return si;
		}

		public virtual Instruction TransformUseInstruction(UseInstruction u)
		{
			u.Expression = u.Expression.Accept(this);
			return u;
		}

		#endregion

		#region IExpressionTransformer Members

		public virtual Expression TransformApplication(Application appl)
		{
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i] = appl.Arguments[i].Accept(this);
			}
			return appl;
		}

		public virtual Expression TransformArrayAccess(ArrayAccess acc)
		{
			acc.Array = acc.Array.Accept(this);
			acc.Index = acc.Index.Accept(this);
			return acc;
		}


		public virtual Expression TransformBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left = binExp.Left.Accept(this);
			binExp.Right = binExp.Right.Accept(this);
			return binExp;
		}

		public virtual Expression TransformCast(Cast cast)
		{
			cast.Expression = cast.Expression.Accept(this);
			return cast;
		}

		public Expression TransformConditionOf(ConditionOf cof)
		{
			cof.Expression = cof.Expression.Accept(this);
			return cof;
		}

		public virtual Expression TransformConstant(Constant c)
		{
			return c;
		}

		public virtual Expression TransformDepositBits(DepositBits d)
		{
			d.Source = d.Source.Accept(this);
			d.InsertedBits = d.InsertedBits.Accept(this);
			return d;
		}

		public virtual Expression TransformDereference(Dereference deref)
		{
			deref.Expression = deref.Expression.Accept(this);
			return deref;
		}

		public virtual Expression TransformFieldAccess(FieldAccess acc)
		{
			acc.structure = acc.structure.Accept(this);
			return acc;
		}

		public virtual Expression TransformIdentifier(Identifier id)
		{
			return id;
		}

		public virtual Expression TransformMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer = mps.BasePointer.Accept(this);
			mps.MemberPointer = mps.MemberPointer.Accept(this);
			return mps;
		}
		
		public virtual Expression TransformMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			access.MemoryId = (MemoryIdentifier) access.MemoryId.Accept(this);
			return access;
		}

		public virtual Expression TransformMkSequence(MkSequence seq)
		{
			seq.Head = seq.Head.Accept(this);
			seq.Tail = seq.Tail.Accept(this);
			return seq;
		}

		public virtual Expression TransformPhiFunction(PhiFunction phi)
		{
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				phi.Arguments[i] = phi.Accept(this);
			}
			return phi;
		}

		public virtual Expression TransformPointerAddition(PointerAddition pa)
		{
			pa.Pointer = pa.Pointer.Accept(this);
			return pa;
		}

		public virtual Expression TransformProcedureConstant(ProcedureConstant pc)
		{
			return pc;
		}

		public virtual Expression TransformSegmentedAccess(SegmentedAccess access)
		{
			access.BasePointer = access.BasePointer.Accept(this);
				access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			access.MemoryId = (MemoryIdentifier) access.MemoryId.Accept(this);
			return access;
		}

		public virtual Expression TransformScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

		public virtual Expression TransformSlice(Slice slice)
		{
			slice.Expression = slice.Expression.Accept(this);
			return slice;
		}

		public virtual Expression TransformTestCondition(TestCondition tc)
		{
			tc.Expression = tc.Expression.Accept(this);
			return tc;
		}

		public virtual Expression TransformUnaryExpression(UnaryExpression unary)
		{
			unary.Expression = unary.Expression.Accept(this);
			return unary;
		}

		#endregion
	}
}
