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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Analysis;
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
    public class TraitCollector : InstructionVisitor<DataType?>, ExpressionVisitor<DataType?>
	{
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(TraitCollector), "Traces the work of the Trait Collector");

		private readonly TypeFactory factory;
		private readonly ITypeStore store;
		private readonly ITraitHandler handler;
		private readonly ArrayExpressionMatcher aem;
		private readonly AddressTraitCollector atrco;
        private Program program;
        private Procedure? proc;
        private LinearInductionVariable? ivCur;

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
                handler.DataTypeTrait(sig.Outputs[0], sig.Outputs[0].DataType);
            }
		}

        private void BindActualTypesToFormalTypes(Application appl)
        {
            if (!(appl.Procedure is ProcedureConstant pc))
                throw new NotImplementedException("Indirect call.");
            FunctionType sig = pc.Signature;
            if (sig is null)
                return;

            if (appl.Arguments.Length != sig.Parameters!.Length)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    pc.Procedure.Name, appl.Arguments.Length, sig.Parameters.Length));
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                handler.EqualTrait(appl.Arguments[i], sig.Parameters[i]);
                sig.Parameters[i].Accept(this);
            }
            if (!sig.HasVoidReturn)
                handler.EqualTrait(appl, sig.Outputs[0]);
        }

		public void CollectEffectiveAddress(Expression field, Expression effectiveAddress)
		{
			atrco.Collect(null, 0, field, effectiveAddress);
		}

		public void CollectEffectiveAddress(Expression basePtr, int basePtrBitSize, Expression field, Expression effectiveAddress)
		{
			atrco.Collect(basePtr, basePtrBitSize, field, effectiveAddress);
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
			return t.Domain;
		}

        private DataType? MakeNonPointer(DataType dataType)
        {
            if (!(dataType is PrimitiveType p))
                return null;
            return p.MaskDomain(~Domain.Pointer);
        }

        private DataType? MakeIntegral(DataType dataType)
        {
            if (!(dataType is PrimitiveType p))
                return null;
            return p.MaskDomain(Domain.Integer);
        }

		public PrimitiveType? MakeNotSigned(DataType t)
		{
            if (!(t is PrimitiveType p))
                return null;
            return p.MaskDomain(~(Domain.SignedInt | Domain.Real));
		}

		public PrimitiveType? MakeSigned(DataType t)
		{
            if (!(t is PrimitiveType p))
                return null;
            return p.MaskDomain(Domain.SignedInt);
		}

		public PrimitiveType? MakeUnsigned(DataType t)
		{
            if (t is PrimitiveType p)
                return PrimitiveType.Create(Domain.UnsignedInt, p.BitSize);
            else
                return null;
        }

		public LinearInductionVariable? MergeInductionVariableConstant(LinearInductionVariable iv, Operator op, Constant? c)
		{
			if (iv is null || c is null)
				return null;
			Constant delta   = op.ApplyConstants(iv.Delta!.DataType, iv.Delta!, c);
			Constant? initial = (iv.Initial is not null) ? op.ApplyConstants(iv.Initial.DataType, iv.Initial, c) : null; 
			Constant? final =   (iv.Final is not null) ?   op.ApplyConstants(iv.Final.DataType, iv.Final, c) : null;
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

		public DataType? VisitCallInstruction(CallInstruction call)
		{
            handler.DataTypeTrait(
                call.Callee, 
                new Pointer(
                    new CodeType(), 
                    program!.Platform.PointerType.BitSize));
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

		public DataType? VisitDefInstruction(DefInstruction def)
		{
			return def.Identifier.Accept(this);
		}

		public DataType VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			return handler.EqualTrait(phi.Dst, phi.Src);
		}

        public DataType? VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression is null)
                return VoidType.Instance;

            var dt = ret.Expression.Accept(this);
            if (!proc!.Signature.HasVoidReturn)
            {
                dt = handler.EqualTrait(proc.Signature.Outputs[0], ret.Expression);
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
				paramTypes[i] = store.GetTypeVariable(appl.Arguments[i]);
			}
			var dt = handler.DataTypeTrait(appl, appl.DataType); 
			handler.FunctionTrait(appl.Procedure, appl.Procedure.DataType.Size, store.GetTypeVariable(appl), paramTypes);

			BindActualTypesToFormalTypes(appl);

			ivCur = null;
            return dt;
		}

		public DataType VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
            if (acc.Index is BinaryExpression b && b.Operator.Type.IsIntMultiplication())
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
			if (id.Storage is MemoryStorage)
                return id.DataType ;

			var dt = handler.DataTypeTrait(id, id.DataType);
            if (!program!.InductionVariables.TryGetValue(id, out ivCur))
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
			if (ivLeft is not null)
			{
				if (binExp.Operator.Type.IsIntMultiplication() || binExp.Operator.Type == OperatorType.Shl)
					ivCur = MergeInductionVariableConstant(ivLeft, binExp.Operator, binExp.Right as Constant);
			} 

			TypeVariable tvExp = store.GetTypeVariable(binExp);
            //$BUGBUG: This needs to be redone because the domain of the operation is now in the OPERATOR, not the operands.
			switch (binExp.Operator.Type)
            {
             
                case OperatorType.IAdd:
				case OperatorType.ISub:
				case OperatorType.And:
				case OperatorType.Or:
				case OperatorType.Xor:
			{
                return handler.DataTypeTrait(binExp, binExp.DataType);
			} 
			case OperatorType.SMul:
			case OperatorType.SDiv:
            case OperatorType.SMod:
			{
                handler.DataTypeTrait(binExp, MakeNonPointer(binExp.DataType)!);
                var dt = handler.DataTypeTrait(binExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Left.DataType.BitSize));
				handler.DataTypeTrait(binExp.Right, PrimitiveType.Create(DomainOf(binExp.DataType), binExp.Right.DataType.BitSize));
                return dt;
			}
			case OperatorType.UMul:
			case OperatorType.UDiv:
			case OperatorType.UMod:
            case OperatorType.Shr:
			{
                handler.DataTypeTrait(binExp, MakeNonPointer(binExp.DataType));
                var dt = handler.DataTypeTrait(binExp, MakeUnsigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left, MakeUnsigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeUnsigned(binExp.Right.DataType));
                return dt;
			}
            case OperatorType.IMul:
			{
                handler.DataTypeTrait(binExp.Left, MakeIntegral(binExp.Left.DataType));
                handler.DataTypeTrait(binExp.Right, MakeIntegral(binExp.Right.DataType));
                return handler.DataTypeTrait(binExp, MakeIntegral(binExp.DataType));
			}
            case OperatorType.Sar:
			{
				var dt = handler.DataTypeTrait(binExp, MakeSigned(binExp.DataType));
				handler.DataTypeTrait(binExp.Left, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeUnsigned(binExp.Right.DataType));
                return dt;
			}
            case OperatorType.Shl:
			{
                return handler.DataTypeTrait(binExp, MakeIntegral(binExp.DataType));
			}
            case OperatorType.IMod:
			{
				var dt = handler.DataTypeTrait(binExp, binExp.DataType);
				handler.DataTypeTrait(binExp.Left, binExp.Left.DataType);
				handler.DataTypeTrait(binExp.Right, binExp.Right.DataType);
                return dt;
			}
            case OperatorType.Eq:
            case OperatorType.Ne:
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				return handler.DataTypeTrait(binExp, PrimitiveType.Bool);
			}
			case OperatorType.Ge:
			case OperatorType.Gt:
			case OperatorType.Le:
            case OperatorType.Lt:
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, MakeSigned(binExp.Left.DataType));
				handler.DataTypeTrait(binExp.Right, MakeSigned(binExp.Right.DataType));
                return dt;
			}
            case OperatorType.Feq:
            case OperatorType.Fne:
            case OperatorType.Flt:
            case OperatorType.Fle:
            case OperatorType.Fge:
            case OperatorType.Fgt:
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, PrimitiveType.Create(Domain.Real, binExp.Left.DataType.BitSize));
				handler.DataTypeTrait(binExp.Right, PrimitiveType.Create(Domain.Real, binExp.Right.DataType.BitSize));
                return dt;
			}
			case OperatorType.Uge:
			case OperatorType.Ugt:
			case OperatorType.Ule:
            case OperatorType.Ult:
			{
				handler.EqualTrait(binExp.Left, binExp.Right);
				var dt = handler.DataTypeTrait(binExp, PrimitiveType.Bool);
				handler.DataTypeTrait(binExp.Left, MakeNotSigned(binExp.Left.DataType)!);
				handler.DataTypeTrait(binExp.Right, MakeNotSigned(binExp.Right.DataType)!);
                return dt;
			}
            case OperatorType.FAdd:
            case OperatorType.FSub:
            case OperatorType.FMul:
            case OperatorType.FDiv:
            {
                var dt = PrimitiveType.Create(Domain.Real, binExp.DataType.BitSize);
                handler.DataTypeTrait(binExp, dt);
				handler.DataTypeTrait(binExp.Left, dt);
				handler.DataTypeTrait(binExp.Right, dt);
                return dt;
            }
            case OperatorType.Cand:
            case OperatorType.Cor:
            {
                var dt = PrimitiveType.Bool;
                handler.DataTypeTrait(binExp, dt);
                handler.DataTypeTrait(binExp.Left, dt);
                handler.DataTypeTrait(binExp.Right, dt);
                return dt;
            }
            default:
                throw new NotImplementedException("NYI: " + binExp.Operator + " in " + binExp);
            }
		}

		public DataType? VisitBranch(Branch b)
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
            // There should be no more ConditionOf nodes 
            // at the time of type analysis; their DataType
            // is just a placeholder and should be
            // disregarded.
            return null!;
		}

        public DataType VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
            handler.DataTypeTrait(conversion.Expression, conversion.SourceDataType);
            return handler.DataTypeTrait(conversion, conversion.DataType);
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
                    program!.Globals,
                    program.Platform.PointerType.BitSize,
                    c,
                    c.ToInt32() * 0x10);   //$REVIEW Platform-dependent
            }
            return dt;
		}

		public DataType VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
            return handler.MemAccessTrait(null, deref.Expression, deref.Expression.DataType.BitSize, deref, 0);
		}

		public DataType VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

        public DataType? VisitMkSequence(MkSequence seq)
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
			return handler.DataTypeTrait(mps, program!.Platform.PointerType);
		}

		public DataType VisitMemoryAccess(MemoryAccess access)
		{
            access.EffectiveAddress.Accept(this);
			var dt = handler.DataTypeTrait(access, access.DataType);
            if (access.EffectiveAddress is SegmentedPointer segptr)
            {
                CollectEffectiveAddress(segptr.BasePointer, segptr.BasePointer.DataType.BitSize, access, segptr.Offset);
            }
            else
            {
                CollectEffectiveAddress(access, access.EffectiveAddress);
            }
            return dt;
		}

		public DataType VisitSegmentedAddress(SegmentedPointer addr)
		{
            addr.BasePointer.Accept(this);
            addr.Offset.Accept(this);
			var dt = handler.DataTypeTrait(addr, addr.DataType);
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
			TypeVariable tPhi = store.GetTypeVariable(phi);
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

		public DataType? VisitProcedureConstant(ProcedureConstant pc)
		{
			FunctionType sig = pc.Signature;
			DataType []? argTypes = null;
			if (sig is not null && sig.Parameters is not null)
			{
				argTypes = new DataType[sig.Parameters.Length];
				for (int i = 0; i < argTypes.Length; ++i)
				{
					argTypes[i] = store.GetTypeVariable(sig.Parameters[i]);
				}
			} 
			else
			{
                if (pc.Procedure is IntrinsicProcedure intrinsic)
                {
                    argTypes = new DataType[intrinsic.Arity];
                    for (int i = 0; i < argTypes.Length; ++i)
                    {
                        argTypes[i] = factory.CreateUnknown();
                    }
                }
            }
            return sig is not null && !sig.HasVoidReturn ? sig.Outputs[0].DataType : null;
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

        public DataType VisitStringConstant(StringConstant str)
        {
            var dt = handler.DataTypeTrait(str, str.DataType);
            return dt;
        }

        public DataType VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
            return handler.DataTypeTrait(tc, tc.DataType);
		}

		public DataType VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
			switch (unary.Operator.Type)
            {
            case OperatorType.AddrOf:
			{
				return handler.PointerTrait(
                    unary, 
                    unary.DataType.Size,
                    unary.Expression);
			}
            case OperatorType.Neg:
			{
				handler.DataTypeTrait(unary.Expression, MakeSigned(unary.Expression.DataType)!);
                return handler.DataTypeTrait(unary, MakeSigned(unary.Expression.DataType)!);
            }
            case OperatorType.Comp:
			{
				return handler.DataTypeTrait(unary, unary.DataType);
			}
            case OperatorType.Not:
			{
                handler.DataTypeTrait(unary.Expression, PrimitiveType.Bool);
                return handler.DataTypeTrait(unary, PrimitiveType.Bool);
			}
            default:
				throw new NotImplementedException(string.Format("TraitCollection.UnaryExpression: {0}", unary));
		    }
        }

		#endregion 
	}
}
