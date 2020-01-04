#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Typing
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
	public class TraitCollector : InstructionVisitor<DataType>, ExpressionVisitor<DataType>
	{
		private Program program;
        private Procedure proc;
		private TypeFactory factory;
		private ITypeStore store;
		private ITraitHandler handler;
		private LinearInductionVariable ivCur;
		private ArrayExpressionMatcher aem;
		private AddressTraitCollector atrco;

		private static TraceSwitch trace = new TraceSwitch("TraitCollector", "Traces the work of the Trait Collector");

		public TraitCollector(TypeFactory factory, ITypeStore store, ITraitHandler handler, Program program)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
            this.program = program;
			this.aem = new ArrayExpressionMatcher(program.Platform.PointerType);
			this.atrco = new AddressTraitCollector(factory, store, handler, program);
		}

		/// <summary>
		/// Add the traits of the procedure's signature.
		/// </summary>
		private void AddProcedureTraits(Procedure proc)
		{
			FunctionType sig = proc.Signature;
            if (!sig.HasVoidReturn)
            {
                handler.DataTypeTrait(sig.ReturnValue, sig.ReturnValue.DataType);
            }
		}

        private void BindActualTypesToFormalTypes(Application appl)
        {
            if (!(appl.Procedure is ProcedureConstant pc))
                throw new NotImplementedException("Indirect call.");
            if (pc.Procedure.Signature == null)
                return;

            FunctionType sig = pc.Procedure.Signature;
            if (appl.Arguments.Length != sig.Parameters.Length)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    pc.Procedure.Name, appl.Arguments.Length, sig.Parameters.Length));
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                handler.EqualTrait(appl.Arguments[i], sig.Parameters[i]);
                sig.Parameters[i].Accept(this);
            }
            if (!sig.HasVoidReturn)
                handler.EqualTrait(appl, sig.ReturnValue);
        }

		public void CollectEffectiveAddress(Expression field, Expression effectiveAddress)
		{
			atrco.Collect(null, 0, field, effectiveAddress);
		}

		public void CollectEffectiveAddress(Expression basePtr, int basePtrSize, Expression field, Expression effectiveAddress)
		{
			atrco.Collect(basePtr, basePtrSize, field, effectiveAddress);
		}

		public void CollectProgramTraits(Program program)
		{
			this.program = program;
            handler.DataTypeTrait(program.Globals, program.Globals.DataType);
            foreach (Procedure p in program.Procedures.Values)
            {
                proc = p;
                AddProcedureTraits(p);
                foreach (Statement stm in p.Statements)
                {
                    Debug.WriteLineIf(trace.TraceVerbose, string.Format("Tracing: {0} ", stm.Instruction));
                    stm.Instruction.Accept(this);
                }
            }
		}
		
		public Domain DomainOf(DataType t)
		{
			return ((PrimitiveType)t).Domain;
		}

        private DataType MakeNonPointer(DataType dataType)
        {
            if (!(dataType is PrimitiveType p))
                return null;
            return p.MaskDomain(~Domain.Pointer);
        }

        private DataType MakeIntegral(DataType dataType)
        {
            if (!(dataType is PrimitiveType p))
                return null;
            return p.MaskDomain(Domain.Integer);
        }

		public PrimitiveType MakeNotSigned(DataType t)
		{
            if (!(t is PrimitiveType p))
                return null;
            return p.MaskDomain(~(Domain.SignedInt | Domain.Real));
		}

		public PrimitiveType MakeSigned(DataType t)
		{
            if (!(t is PrimitiveType p))
                return null;
            return p.MaskDomain(Domain.SignedInt);
		}

		public PrimitiveType MakeUnsigned(DataType t)
		{
            if (t is PrimitiveType p)
                return PrimitiveType.Create(Domain.UnsignedInt, p.BitSize);
            else
                return null;
        }

		public LinearInductionVariable MergeInductionVariableConstant(LinearInductionVariable iv, Operator op, Constant c)
		{
			if (iv == null || c == null)
				return null;
			Constant delta   = op.ApplyConstants(iv.Delta, c);
			Constant initial = (iv.Initial != null) ? op.ApplyConstants(iv.Initial, c) : null; 
			Constant final =   (iv.Final != null) ?   op.ApplyConstants(iv.Final, c) : null;
			return new LinearInductionVariable(initial, delta, final, false);
		}

		#region InstructionVisitor methods ///////////////////////////

		public DataType VisitAssignment(Assignment a)
		{
			var dtSrc = a.Src.Accept(this);
			var dtDst = a.Dst.Accept(this);
            handler.DataTypeTrait(a.Dst, dtSrc);
			return handler.EqualTrait(a.Dst, a.Src);
		}

		public DataType VisitStore(Store store)
		{
			store.Src.Accept(this);
			store.Dst.Accept(this);
			return handler.EqualTrait(store.Dst, store.Src);
		}

		public DataType VisitCallInstruction(CallInstruction call)
		{
            handler.DataTypeTrait(
                call.Callee, 
                new Pointer(
                    new CodeType(), 
                    program.Platform.PointerType.BitSize));
            return call.Callee.Accept(this);
        }

        public DataType VisitGotoInstruction(GotoInstruction g)
        {
            throw new NotImplementedException();
        }

        public DataType VisitComment(CodeComment comment)
        {
            return VoidType.Instance;
        }

		public DataType VisitDefInstruction(DefInstruction def)
		{
			return def.Identifier.Accept(this);
		}

		public DataType VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			return handler.EqualTrait(phi.Dst, phi.Src);
		}

        public DataType VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression == null)
                return VoidType.Instance;

            var dt = ret.Expression.Accept(this);
            if (!proc.Signature.HasVoidReturn)
            {
                dt = handler.EqualTrait(proc.Signature.ReturnValue, ret.Expression);
            }
            return dt;
        }

        public DataType VisitSideEffect(SideEffect sideEffect)
        {
            sideEffect.Expression.Accept(this);
            return VoidType.Instance;
        }

        public DataType VisitSwitchInstruction(SwitchInstruction si)
		{
			si.Expression.Accept(this);
            return VoidType.Instance;
		}

		public DataType VisitUseInstruction(UseInstruction u)
		{
            u.Expression.Accept(this);
            return VoidType.Instance;
		}

		#endregion

		#region IExpressionVisitor methods

		public DataType VisitApplication(Application appl)
		{
			appl.Procedure.Accept(this);

			TypeVariable [] paramTypes = new TypeVariable[appl.Arguments.Length];
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
				paramTypes[i] = appl.Arguments[i].TypeVariable;
			}
			var dt = handler.DataTypeTrait(appl, appl.DataType); 
			handler.FunctionTrait(appl.Procedure, appl.Procedure.DataType.Size, appl.TypeVariable, paramTypes);

			BindActualTypesToFormalTypes(appl);

			ivCur = null;
            return dt;
		}

		public DataType VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
            if (acc.Index is BinaryExpression b && (b.Operator == Operator.IMul || b.Operator == Operator.SMul || b.Operator == Operator.UMul))
            {
                if (b.Right is Constant c)
                {
                    atrco.CollectArray(null, acc, acc.Array, c.ToInt32(), 0);
                    return handler.DataTypeTrait(acc, acc.DataType);
                }
            }
            atrco.CollectArray(null, acc, acc.Array, 1, 0);
			CollectEffectiveAddress(acc, acc.Array);
            return handler.DataTypeTrait(acc, acc.DataType);
		}

		public DataType VisitIdentifier(Identifier id)
		{
			if (id is MemoryIdentifier)
                return id.DataType ;

			var dt = handler.DataTypeTrait(id, id.DataType);
            if (!program.InductionVariables.TryGetValue(id, out ivCur))
                ivCur = null;
            return dt;
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
		public DataType VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			var ivLeft = ivCur;
			binExp.Right.Accept(this);

			ivCur = null;
			if (ivLeft != null)
			{
				if (binExp.Operator == Operator.SMul || binExp.Operator == Operator.UMul || binExp.Operator == Operator.IMul || binExp.Operator == Operator.Shl)
					ivCur = MergeInductionVariableConstant(ivLeft, binExp.Operator, binExp.Right as Constant);
			} 

			TypeVariable tvExp = binExp.TypeVariable;
            //$BUGBUG: This needs to be redone because the domain of the operation is now in the OPERATOR, not the operands.
			if (binExp.Operator == Operator.IAdd || 
				binExp.Operator == Operator.ISub ||
				binExp.Operator == Operator.And ||
				binExp.Operator == Operator.Or  ||
				binExp.Operator == Operator.Xor)
			{
                return handler.DataTypeTrait(binExp, binExp.DataType);
			} 
			else if (binExp.Operator == Operator.SMul ||
				binExp.Operator == Operator.SDiv)
			{
                handler.DataTypeTrait(binExp, MakeNonPointer(binExp.DataType));
                var dt = handler.DataTypeTrait(binExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Left.DataType.BitSize));
				handler.DataTypeTrait(binExp.Right, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Right.DataType.BitSize));
                return dt;
			}
			else if (binExp.Operator == Operator.UMul ||
				binExp.Operator == Operator.UDiv ||
				binExp.Operator == Operator.Shr)
			{
                handler.DataTypeTrait(binExp, MakeNonPointer(binExp.DataType));
                var dt = handler.DataTypeTrait(binExp, MakeUnsigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left, MakeUnsigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeUnsigned(binExp.Right.DataType));
                return dt;
			}
			else if (binExp.Operator == Operator.IMul)
			{
                handler.DataTypeTrait(binExp.Left, MakeIntegral(binExp.Left.DataType));
                handler.DataTypeTrait(binExp.Right, MakeIntegral(binExp.Right.DataType));
                return handler.DataTypeTrait(binExp, MakeIntegral(binExp.DataType));
			}
			else if (binExp.Operator == Operator.Sar)
			{
				var dt = handler.DataTypeTrait(binExp, MakeSigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeUnsigned(binExp.Right.DataType));
                return dt;
			}
			else if (binExp.Operator == Operator.Shl)
			{
                return handler.DataTypeTrait(binExp, MakeIntegral(binExp.DataType));
			}
			else if (binExp.Operator == Operator.IMod)
			{
				var dt = handler.DataTypeTrait(binExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left, binExp.Left.DataType);
				handler.DataTypeTrait(binExp.Right, binExp.Right.DataType);
                return dt;
			}
			else if (binExp.Operator == Operator.Eq ||
				binExp.Operator == Operator.Ne)
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				return handler.DataTypeTrait(binExp, PrimitiveType.Bool);
			}
			else if (binExp.Operator == Operator.Ge ||
				binExp.Operator == Operator.Gt ||
				binExp.Operator == Operator.Le ||
				binExp.Operator == Operator.Lt)
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeSigned(binExp.Right.DataType));
                return dt;
			}
			else if (binExp.Operator is RealConditionalOperator)
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, PrimitiveType.Create(Domain.Real, binExp.Left.DataType.BitSize));
				handler.DataTypeTrait(binExp.Right, PrimitiveType.Create(Domain.Real, binExp.Right.DataType.BitSize));
                return dt;
			}
			else if (binExp.Operator == Operator.Uge ||
				binExp.Operator == Operator.Ugt ||
				binExp.Operator == Operator.Ule ||
				binExp.Operator == Operator.Ult)
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, MakeNotSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeNotSigned(binExp.Right.DataType));
                return dt;
			}
            else if (binExp.Operator == Operator.FAdd ||
                binExp.Operator == Operator.FSub ||
                binExp.Operator == Operator.FMul ||
                binExp.Operator == Operator.FDiv)
            {
                var dt = PrimitiveType.Create(Domain.Real, binExp.DataType.BitSize);
                handler.DataTypeTrait(binExp, dt);
				handler.DataTypeTrait(binExp.Left, dt);
				handler.DataTypeTrait(binExp.Right, dt);
                return dt;
            }
            else if (binExp.Operator == Operator.Cand ||
                binExp.Operator == Operator.Cor)
            {
                var dt = PrimitiveType.Bool;
                handler.DataTypeTrait(binExp, dt);
                handler.DataTypeTrait(binExp.Left, dt);
                handler.DataTypeTrait(binExp.Right, dt);
                return dt;
            }
            throw new NotImplementedException("NYI: " + binExp.Operator + " in " + binExp);
		}

		public DataType VisitBranch(Branch b)
		{
			return b.Condition.Accept(this);
		}

		public DataType VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
			return handler.DataTypeTrait(cast, cast.DataType);
		}

        public DataType VisitConditionalExpression(ConditionalExpression c)
        {
            throw new NotImplementedException();
        }

        public DataType VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
			return handler.DataTypeTrait(cof, cof.DataType);
		}

        public DataType VisitAddress(Address addr)
        {
            return handler.DataTypeTrait(addr, addr.DataType);
        }

		public DataType VisitConstant(Constant c)
		{
			var dt = handler.DataTypeTrait(c, c.DataType);
			ivCur = null;
            if (c.DataType == PrimitiveType.SegmentSelector)
            {
                handler.MemAccessTrait(
                    null, 
                    program.Globals,
                    program.Platform.PointerType.Size,
                    c,
                    c.ToInt32() * 0x10);   //$REVIEW Platform-dependent
            }
            return dt;
		}

        public DataType VisitDeclaration(Declaration decl)
        {
            var dtId = decl.Identifier.Accept(this);
            if (decl.Expression == null)
                return dtId;

            decl.Expression.Accept(this);
            return handler.EqualTrait(decl.Identifier, decl.Expression);
        }

        public DataType VisitDepositBits(DepositBits d)
        {
            d.Source.Accept(this);
            d.InsertedBits.Accept(this);
            var dt = handler.DataTypeTrait(d, d.DataType);
            ivCur = null;
            return dt;
        }

		public DataType VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
            return handler.MemAccessTrait(null, deref.Expression, deref.Expression.DataType.Size, deref, 0);
		}

		public DataType VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

        public DataType VisitMkSequence(MkSequence seq)
        {
            foreach (var e in seq.Expressions)
            {
                var dt = e.Accept(this);
            }
            return handler.DataTypeTrait(seq, seq.DataType);
        }

        public DataType VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
			return handler.DataTypeTrait(mps, program.Platform.PointerType);
		}

		public DataType VisitMemoryAccess(MemoryAccess access)
		{
            access.EffectiveAddress.Accept(this);
			var dt = handler.DataTypeTrait(access, access.DataType);
			CollectEffectiveAddress(access, access.EffectiveAddress);
            return dt;
		}

		public DataType VisitSegmentedAccess(SegmentedAccess access)
		{
            access.BasePointer.Accept(this);
            access.EffectiveAddress.Accept(this);
			TypeVariable tAccess = access.TypeVariable;
			var dt = handler.DataTypeTrait(access, access.DataType);
			CollectEffectiveAddress(access.BasePointer, access.BasePointer.DataType.Size, access, access.EffectiveAddress);
            return dt;
		}

        public DataType VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
            return handler.PointerTrait(
                outArg,
                outArg.DataType.Size,
                outArg.Expression);
        }

		public DataType VisitPhiFunction(PhiFunction phi)
		{
			TypeVariable tPhi = phi.TypeVariable;
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
			}
            return handler.DataTypeTrait(phi, phi.DataType);
        }

		public DataType VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

		public DataType VisitProcedureConstant(ProcedureConstant pc)
		{
			FunctionType sig = pc.Procedure.Signature;
			DataType [] argTypes = null;
			if (sig != null && sig.Parameters != null)
			{
				argTypes = new DataType[sig.Parameters.Length];
				for (int i = 0; i < argTypes.Length; ++i)
				{
					argTypes[i] = sig.Parameters[i].TypeVariable;
				}
			} 
			else
			{
                if (pc.Procedure is PseudoProcedure ppp)
                {
                    argTypes = new DataType[ppp.Arity];
                    for (int i = 0; i < argTypes.Length; ++i)
                    {
                        argTypes[i] = factory.CreateUnknown();
                    }
                }
            }
            return sig != null && !sig.HasVoidReturn ? sig.ReturnValue.DataType : null;
		}

        private void CollectProcedureCharacteristics()
        {

        }

        public DataType VisitScopeResolution(ScopeResolution sr)
        {
            throw new NotImplementedException();
        }

		public DataType VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
			return handler.DataTypeTrait(slice, slice.DataType);
		}

		public DataType VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
            return handler.DataTypeTrait(tc, tc.DataType);
		}

		public DataType VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
			if (unary.Operator == Operator.AddrOf)
			{
				return handler.PointerTrait(
                    unary, 
                    unary.DataType.Size,
                    unary.Expression);
			}
			else if (unary.Operator == Operator.Neg)
			{
				handler.DataTypeTrait(unary.Expression, MakeSigned(unary.Expression.DataType));
                return handler.DataTypeTrait(unary, MakeSigned(unary.Expression.DataType));
            }
			else if (unary.Operator == Operator.Comp)
			{
				return handler.DataTypeTrait(unary, unary.DataType);
			}
			else if (unary.Operator == Operator.Not)
			{
                handler.DataTypeTrait(unary.Expression, PrimitiveType.Bool);
                return handler.DataTypeTrait(unary, PrimitiveType.Bool);
			}
			else
				throw new NotImplementedException(string.Format("TraitCollection.UnaryExpression: {0}", unary));
		}

		#endregion 
	}
}
