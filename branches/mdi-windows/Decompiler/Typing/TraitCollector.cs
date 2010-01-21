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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;

namespace Decompiler.Typing
{
	/// <summary>
	/// Gathers the traits for each expression in a program.
	/// </summary>
	/// <remarks>
	/// Assumes that a prior pass has assigned unique type identifiers to each expression in the program.
	/// <para>
	/// Much of the type inference code in this namespace was inspired by the master's thesis
	/// "Entwicklung eines Typanalysesystem für einen Decompiler", 2004, by Raimar Falke.
	/// </para>
	/// </remarks>
	public class TraitCollector : InstructionVisitorBase
	{
		private Program prog;
        private Procedure proc;
		private TypeFactory factory;
		private TypeStore store;
		private ITraitHandler handler;
		private LinearInductionVariable ivCur;
		private ArrayExpressionMatcher aem;
		private AddressTraitCollector atrco;

		private static TraceSwitch trace = new TraceSwitch("TraitCollector", "Traces the work of the Trait Collector");

		public TraitCollector(TypeFactory factory, TypeStore store, ITraitHandler handler, Program prog)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
            this.prog = prog;
			this.aem = new ArrayExpressionMatcher(prog.Architecture.PointerType);
			this.atrco = new AddressTraitCollector(factory, store, handler, prog);
		}

		/// <summary>
		/// Add the traits of the procedure's signature.
		/// </summary>
		private void AddProcedureTraits(Procedure proc)
		{
			ProcedureSignature sig = proc.Signature;
            if (sig.ReturnValue != null)
            {
                handler.DataTypeTrait(sig.ReturnValue.TypeVariable, sig.ReturnValue.DataType);
            }
		}

        private void BindActualTypesToFormalTypes(Application appl)
        {
            ProcedureConstant pc = appl.Procedure as ProcedureConstant;
            if (pc == null)
                throw new NotImplementedException("Indirect call");
            if (pc.Procedure.Signature == null)
                return;

            ProcedureSignature sig = pc.Procedure.Signature;
            if (appl.Arguments.Length != sig.FormalArguments.Length)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    pc.Procedure.Name, appl.Arguments.Length, sig.FormalArguments.Length));
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                handler.EqualTrait(appl.Arguments[i].TypeVariable, sig.FormalArguments[i].TypeVariable);
                sig.FormalArguments[i].Accept(this);
            }
            if (sig.ReturnValue != null)
                handler.EqualTrait(appl.TypeVariable, sig.ReturnValue.TypeVariable);
        }

		public void CollectEffectiveAddress(TypeVariable fieldType, Expression effectiveAddress)
		{
			atrco.Collect(null, 0, fieldType, effectiveAddress);
		}

		public void CollectEffectiveAddress(TypeVariable basePtrType, int basePtrSize, TypeVariable fieldType, Expression effectiveAddress)
		{
			atrco.Collect(basePtrType, basePtrSize, fieldType, effectiveAddress);
		}

		public void CollectProgramTraits(Program prog)
		{
			this.prog = prog;
			foreach (Procedure p in prog.Procedures.Values)
			{
                proc = p;
				AddProcedureTraits(p);
				foreach (Block block in p.RpoBlocks)
				{
					foreach (Statement stm in block.Statements)
					{
						Debug.WriteLineIf(trace.TraceVerbose, string.Format("Tracing: {0} ", stm.Instruction));
						stm.Instruction.Accept(this);
					}
				}
			} 
		}
		
		public Domain DomainOf(DataType t)
		{
			return ((PrimitiveType)t).Domain;
		}

        private DataType MakeNonPointer(DataType dataType)
        {
            PrimitiveType p = dataType as PrimitiveType;
            if (p == null)
                return null;
            return p.MaskDomain(~Domain.Pointer);
        }

		public PrimitiveType MakeNotSigned(DataType t)
		{
			PrimitiveType p = t as PrimitiveType;
			if (p == null)
				return null;
			return p.MaskDomain(~(Domain.SignedInt|Domain.Real));
		}

		public PrimitiveType MakeSigned(DataType t)
		{
			PrimitiveType p = t as PrimitiveType;
			if (p == null)
				return null;
			return p.MaskDomain(Domain.SignedInt);
		}

		public PrimitiveType MakeUnsigned(DataType t)
		{
			PrimitiveType p = t as PrimitiveType;
			if (p != null)
				return PrimitiveType.Create(Domain.UnsignedInt, p.Size);
			else
				return null;
		}

		public LinearInductionVariable MergeInductionVariableConstant(LinearInductionVariable iv, BinaryOperator op, Constant c)
		{
			if (iv == null || c == null)
				return null;
			Constant delta   = op.ApplyConstants(iv.Delta, c);
			Constant initial = (iv.Initial != null) ? op.ApplyConstants(iv.Initial, c) : null; 
			Constant final =   (iv.Final != null) ?   op.ApplyConstants(iv.Final, c) : null;
			return new LinearInductionVariable(initial, delta, final, false);
		}

		#region InstructionVisitor methods ///////////////////////////

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
			a.Dst.Accept(this);
			handler.EqualTrait(a.Dst.TypeVariable, a.Src.TypeVariable);
		}

		public override void VisitStore(Store store)
		{
			store.Src.Accept(this);
			store.Dst.Accept(this);
			handler.EqualTrait(store.Dst.TypeVariable, store.Src.TypeVariable);
		}

		public override void VisitCallInstruction(CallInstruction ci)
		{
			throw new NotImplementedException();
		}

		public override void VisitDefInstruction(DefInstruction def)
		{
			def.Expression.Accept(this);
		}

		public override void VisitIndirectCall(IndirectCall ic)
		{
			ic.Callee.Accept(this);
			handler.FunctionTrait(ic.Callee.TypeVariable, ic.Callee.DataType.Size, null, new TypeVariable[0]);
		}

		public override void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			handler.EqualTrait(phi.Dst.TypeVariable, phi.Src.TypeVariable);
		}

		public override void VisitReturnInstruction(ReturnInstruction ret)
		{
            if (ret.Expression != null)
            {
                ret.Expression.Accept(this);
                if (proc.Signature != null && proc.Signature.ReturnValue != null)
                {
                    handler.EqualTrait(proc.Signature.ReturnValue.TypeVariable, ret.Expression.TypeVariable);
                }
            }
		}

		public override void VisitSwitchInstruction(SwitchInstruction si)
		{
			si.Expression.Accept(this);
		}

		public override void VisitUseInstruction(UseInstruction u)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IExpressionVisitor methods

		public override void VisitApplication(Application appl)
		{
			appl.Procedure.Accept(this);

			TypeVariable [] paramTypes = new TypeVariable[appl.Arguments.Length];
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
				paramTypes[i] = appl.Arguments[i].TypeVariable;
			}
			handler.DataTypeTrait(appl.TypeVariable, appl.DataType as PrimitiveType); 
			handler.FunctionTrait(appl.Procedure.TypeVariable, appl.Procedure.DataType.Size, appl.TypeVariable, paramTypes);

			BindActualTypesToFormalTypes(appl);

			ivCur = null;
		}

		public class ArrayContext
		{
			public int ElementSize;
			public int Length;
		}

		public override void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
			BinaryExpression b = acc.Index as BinaryExpression;
			if (b != null && b.op == Operator.mul)
			{
				Constant c = b.Right as Constant;
				if (c != null)
				{
					atrco.CollectArray(null, acc.TypeVariable, acc.Array, c.ToInt32(), 0);
					handler.DataTypeTrait(acc.TypeVariable, acc.DataType);
					return;
				}
			}
			atrco.CollectArray(null, acc.TypeVariable, acc.Array, 1, 0);
			CollectEffectiveAddress(acc.TypeVariable, acc.Array);
		}

		public override void VisitIdentifier(Identifier id)
		{
			if (id is MemoryIdentifier)
				return;

			handler.DataTypeTrait(id.TypeVariable, id.DataType);
            if (!prog.InductionVariables.TryGetValue(id, out ivCur))
                ivCur = null;
		}

		/*
		 * We need to handle the case when seeing
		 * 1) ptr(mem(a)) = ptr(mem(a)) + C and 
		 * 2) ptr(mem(a)) = ptr(mem(b)) + C expressions.
		 * In the first instance, we may be seeing the increment of an array pointer, where the stride is C. However
		 * if mem(a) has a size > C, then this is actually an expression of the type:
		 * a = &a->fC, where C is the offset of the field fC. This is equivalent to case 2.
		 * 
		 * If we see t = x and later t = x + 4, then 
		 * [[t]] = [[x]] and [[t]] = [[x + 4]]. If [[t]] is a ptr(mem(Q)),
		 * then we have a problem! 
		 * 
		 * This analysis is probably best done after TraitCollection, since by then we have discovered max sizes of mems.
		 */
		public override void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			LinearInductionVariable ivLeft = ivCur;
			binExp.Right.Accept(this);
			LinearInductionVariable ivRight = ivCur;

			ivCur = null;
			if (ivLeft != null)
			{
				if (binExp.op == Operator.muls || binExp.op == Operator.mulu || binExp.op == Operator.mul || binExp.op == Operator.shl)
					ivCur = MergeInductionVariableConstant(ivLeft, binExp.op, binExp.Right as Constant);
			} 

			TypeVariable tvExp = binExp.TypeVariable;
			if (binExp.op == Operator.add || 
				binExp.op == Operator.sub ||
				binExp.op == Operator.and ||
				binExp.op == Operator.or  ||
				binExp.op == Operator.xor)
			{
				handler.DataTypeTrait(tvExp, binExp.DataType);
				return;
			} 
			else if (binExp.op == Operator.muls ||
				binExp.op == Operator.divs)
			{
                handler.DataTypeTrait(tvExp, MakeNonPointer(binExp.DataType));
				handler.DataTypeTrait(tvExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left.TypeVariable, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Left.DataType.Size));
				handler.DataTypeTrait(binExp.Right.TypeVariable, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Right.DataType.Size));
				return;
			}
			else if (binExp.op == Operator.mulu ||
				binExp.op == Operator.divu ||
				binExp.op == Operator.shr)
			{
                handler.DataTypeTrait(tvExp, MakeNonPointer(binExp.DataType));
                handler.DataTypeTrait(tvExp, MakeUnsigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left.TypeVariable, MakeUnsigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right.TypeVariable, MakeUnsigned(binExp.Right.DataType));
				return;
			}
			else if (binExp.op == Operator.mul)
			{
				handler.DataTypeTrait(tvExp, MakeNonPointer(binExp.DataType));
				return;
			}
			else if (binExp.op == Operator.sar)
			{
				handler.DataTypeTrait(tvExp, MakeSigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left.TypeVariable, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right.TypeVariable, MakeUnsigned(binExp.Right.DataType));
				return;
			}
			else if (binExp.op == Operator.shl)
			{
				handler.DataTypeTrait(tvExp, binExp.DataType);
				return;
			}
			else if (binExp.op == Operator.mod)
			{
				handler.DataTypeTrait(tvExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left.TypeVariable, binExp.Left.DataType);
				handler.DataTypeTrait(binExp.Right.TypeVariable, binExp.Right.DataType);
				return;
			}
			else if (binExp.op == Operator.eq ||
				binExp.op == Operator.ne)
			{
				handler.EqualTrait(binExp.Left.TypeVariable, binExp.Right.TypeVariable);
				handler.DataTypeTrait(tvExp, PrimitiveType.Bool);
				return;
			}
			else if (binExp.op == Operator.ge ||
				binExp.op == Operator.gt ||
				binExp.op == Operator.le ||
				binExp.op == Operator.lt)
			{
				handler.EqualTrait(binExp.Left.TypeVariable, binExp.Right.TypeVariable);
				handler.DataTypeTrait(tvExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left.TypeVariable, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right.TypeVariable, MakeSigned(binExp.Right.DataType));
				return;
			}
			else if (binExp.op is RealConditionalOperator)
			{
				handler.EqualTrait(binExp.Left.TypeVariable, binExp.Right.TypeVariable);
				handler.DataTypeTrait(tvExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left.TypeVariable, PrimitiveType.Create(Domain.Real, binExp.Left.DataType.Size));
				handler.DataTypeTrait(binExp.Right.TypeVariable, PrimitiveType.Create(Domain.Real, binExp.Right.DataType.Size));
				return;
			}
			else if (binExp.op == Operator.uge ||
				binExp.op == Operator.ugt ||
				binExp.op == Operator.ule ||
				binExp.op == Operator.ult)
			{
				handler.EqualTrait(binExp.Left.TypeVariable, binExp.Right.TypeVariable);
				handler.DataTypeTrait(tvExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left.TypeVariable, MakeNotSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right.TypeVariable, MakeNotSigned(binExp.Right.DataType));
				return;
			}
			throw new NotImplementedException("NYI: " + binExp.op + " in " + binExp);
		}


		public override void VisitBranch(Branch b)
		{
			b.Condition.Accept(this);
		}

		public override void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
			handler.DataTypeTrait(cast.TypeVariable, cast.DataType);
		}

		public override void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
			handler.DataTypeTrait(cof.TypeVariable, cof.DataType);
		}

		public override void VisitConstant(Constant c)
		{
			handler.DataTypeTrait(c.TypeVariable, c.DataType);
			ivCur = null;
		}

		public override void VisitDeclaration(Declaration decl)
		{
            decl.Identifier.Accept(this);
			if (decl.Expression != null) 
			{
				decl.Expression.Accept(this);
				handler.EqualTrait(decl.Identifier.TypeVariable, decl.Expression.TypeVariable);
			}
		}

		public override void VisitDepositBits(DepositBits d)
		{
			d.Source.Accept(this);
            d.InsertedBits.Accept(this);
			handler.DataTypeTrait(d.TypeVariable, d.DataType);
			ivCur = null;
		}

		public override void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
			handler.MemAccessTrait(null, deref.Expression.TypeVariable, 0, deref.TypeVariable, 0);
		}

		public override void VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

        public override void VisitMkSequence(MkSequence seq)
        {
            base.VisitMkSequence(seq);
            handler.DataTypeTrait(seq.TypeVariable, seq.DataType);
        }

        public override void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
			handler.DataTypeTrait(mps.TypeVariable, prog.Architecture.PointerType);
		}


		public override void VisitMemoryAccess(MemoryAccess access)
		{
			base.VisitMemoryAccess(access);
			TypeVariable tAccess = access.TypeVariable;
			handler.DataTypeTrait(tAccess, access.DataType);
			CollectEffectiveAddress(tAccess, access.EffectiveAddress);
		}

		public override void VisitSegmentedAccess(SegmentedAccess access)
		{
			base.VisitSegmentedAccess(access);
			TypeVariable tAccess = access.TypeVariable;
			handler.DataTypeTrait(tAccess, access.DataType);
			CollectEffectiveAddress(access.BasePointer.TypeVariable, access.BasePointer.DataType.Size, tAccess, access.EffectiveAddress);
		}

		public override void VisitPhiFunction(PhiFunction phi)
		{
			TypeVariable tPhi = phi.TypeVariable;
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				phi.Arguments[i].Accept(this);
			}
		}

		public override void VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

		public override void VisitProcedureConstant(ProcedureConstant pc)
		{
			ProcedureSignature sig = pc.Procedure.Signature;
			DataType [] argTypes = null;
			if (sig != null)
			{
				argTypes = new DataType[sig.FormalArguments.Length];
				for (int i = 0; i < argTypes.Length; ++i)
				{
					argTypes[i] = sig.FormalArguments[i].DataType;
				}
			} 
			else
			{
				PseudoProcedure ppp = pc.Procedure as PseudoProcedure;
				if (ppp != null)
				{
					argTypes = new DataType[ppp.Arity];
					for (int i = 0; i < argTypes.Length; ++i)
					{
						argTypes[i] = factory.CreateUnknown();
					}
				}
			}
		}

        private void CollectProcedureCharacteristics()
        {

        }

		public override void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
			handler.DataTypeTrait(slice.TypeVariable, slice.DataType);
		}

		public override void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
			handler.DataTypeTrait(tc.TypeVariable, tc.DataType);
		}

		public override void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
			if (unary.op == Operator.addrOf)
			{
				handler.PointerTrait(
                    unary.TypeVariable, 
                    unary.DataType.Size,
                    unary.Expression.TypeVariable);
			}
			else if (unary.op == Operator.neg)
			{
				handler.DataTypeTrait(unary.TypeVariable, MakeSigned(unary.Expression.DataType));
				handler.DataTypeTrait(unary.Expression.TypeVariable, MakeSigned(unary.Expression.DataType));
			}
			else if (unary.op == Operator.comp)
			{
				handler.DataTypeTrait(unary.TypeVariable, unary.DataType);
			}
			else if (unary.op == Operator.not)
			{
				handler.DataTypeTrait(unary.TypeVariable, PrimitiveType.Bool);
				handler.DataTypeTrait(unary.Expression.TypeVariable, PrimitiveType.Bool);
			}
			else
				throw new NotImplementedException(string.Format("TraitCollection.UnaryExpression: {0}", unary));
		}

		#endregion 
	}
}
