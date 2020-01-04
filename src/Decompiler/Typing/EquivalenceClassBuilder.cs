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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Typing
{
	/// <summary>
	/// Assigns a type to each expression node in a program and builds equivalence classes.
	/// </summary>
	public class EquivalenceClassBuilder : InstructionVisitorBase
	{
        private static TraceSwitch trace = new TraceSwitch(nameof(EquivalenceClassBuilder), "Trace EquivalenceClassBuilder") { Level = TraceLevel.Warning };

		private TypeFactory factory;
		private TypeStore store;
        private DecompilerEventListener listener;
		private FunctionType signature;
        private Dictionary<ushort, TypeVariable> segTypevars;
        private Dictionary<string, EquivalenceClass> typeReferenceClasses;
        private Statement stmCur;

		public EquivalenceClassBuilder(TypeFactory factory, TypeStore store, DecompilerEventListener listener)
        {
			this.factory = factory;
			this.store = store;
            this.listener = listener;
			this.signature = null;
            this.segTypevars = new Dictionary<ushort, TypeVariable>();
            this.typeReferenceClasses = new Dictionary<string, EquivalenceClass>();
		}

		public void Build(Program program)
        {
            // Special case for the global variables. In essence,
            // a memory map of all the globals.
			var tvGlobals = store.EnsureExpressionTypeVariable(factory, program.Globals, "globals_t");
            tvGlobals.OriginalDataType = program.Globals.DataType;

            EnsureSegmentTypeVariables(program.SegmentMap.Segments.Values);
            int cProc = program.Procedures.Count;
            int i = 0;
            foreach (Procedure proc in program.Procedures.Values)
            {
                if (listener.IsCanceled())
                    return;
                listener.ShowProgress("Gathering primitive datatypes from instructions.", i++, cProc);
                this.signature = proc.Signature;
                EnsureSignatureTypeVariables(this.signature);
                
                foreach (Statement stm in proc.Statements)
                {
                    stmCur = stm;
                    stm.Instruction.Accept(this);
                }
            }
		}

        public void EnsureSegmentTypeVariables(IEnumerable<ImageSegment> segments)
        {
            foreach (var segment in segments.Where(s => s.Identifier != null))
            {
                var selector = segment.Address.Selector;
                if (selector.HasValue)
                {
                    segment.Identifier.TypeVariable = null;
                    var tvSeg = store.EnsureExpressionTypeVariable(factory, segment.Identifier, segment.Identifier.Name + "_t");
                    tvSeg.OriginalDataType = segment.Identifier.DataType;
                    this.segTypevars[selector.Value] = tvSeg;
                }
            }
        }

        public void EnsureSignatureTypeVariables(FunctionType signature)
        {
            if (signature == null || !signature.ParametersValid)
                return;
            if (!signature.HasVoidReturn)
            {
                signature.ReturnValue.Accept(this);
            }
            foreach (var param in signature.Parameters)
            {
                param.Accept(this);
            }
        }

        public TypeVariable EnsureTypeVariable(Expression e)
		{
            var tv = store.EnsureExpressionTypeVariable(factory, e);
            var typeref = e.DataType.ResolveAs<TypeReference>();
            if (typeref != null)
            {
                if (this.typeReferenceClasses.TryGetValue(typeref.Name, out var eq))
                {
                    store.MergeClasses(tv, eq.Representative);
                }
                else
                {
                    this.typeReferenceClasses.Add(typeref.Name, tv.Class);
                }
            }
            return tv;
		}

		public override void VisitApplication(Application appl)
		{
            var oldSig = signature;
			signature = null;
			appl.Procedure.Accept(this);
			FunctionType sig = signature;

			if (sig != null)
			{
				if (sig.Parameters.Length != appl.Arguments.Length)
					throw new InvalidOperationException("Parameter count must match.");
			}

			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
				if (sig != null)
				{
					EnsureTypeVariable(sig.Parameters[i]);
					store.MergeClasses(appl.Arguments[i].TypeVariable, sig.Parameters[i].TypeVariable);
				}
			}
			EnsureTypeVariable(appl);
            signature = oldSig;
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

        public override void VisitAddress(Address addr)
        {
            EnsureTypeVariable(addr);
        }

		public override void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
			if (binExp.Operator is ConditionalOperator)
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
                if (segTypevars.TryGetValue(c.ToUInt16(), out var tv))
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

        public override void VisitConditionalExpression(ConditionalExpression cond)
        {
            base.VisitConditionalExpression(cond);
            EnsureTypeVariable(cond);
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
            foreach (var e in seq.Expressions)
            {
                e.Accept(this);
            }
			EnsureTypeVariable(seq);
		}

        public override void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
            EnsureTypeVariable(outArg);
        }

        public override void VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression == null)
                return;
            ret.Expression.Accept(this);
            if (!signature.HasVoidReturn)
            {
                if (signature.ReturnValue.TypeVariable == null)
                {
                    DebugEx.Warn(trace, "Eqb: {0:X}: Type variable for return value of signature of {1} is missing", stmCur.LinearAddress, stmCur.Block.Procedure.Name);
                    return;
                }
                store.MergeClasses(
                    signature.ReturnValue.TypeVariable,
                    ret.Expression.TypeVariable);
            }
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
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
				store.MergeClasses(tPhi, arg.Value.TypeVariable);
			}
		}

		public override void VisitProcedureConstant(ProcedureConstant pc)
		{
			EnsureTypeVariable(pc);
			VisitProcedure(pc.Procedure);
			if (pc.Procedure.Signature.ParametersValid)
			{
				store.MergeClasses(pc.TypeVariable, pc.Procedure.Signature.TypeVariable);
				signature = pc.Procedure.Signature;
			}
		}

		public void VisitProcedure(ProcedureBase proc)
		{
			if (proc.Signature.TypeVariable == null)
			{
				proc.Signature.TypeVariable = store.EnsureExpressionTypeVariable(
					factory,
                    new Identifier("signature of " + proc.Name, VoidType.Instance, null),
					null);
			}
			if (proc.Signature.Parameters != null)
			{
				foreach (Identifier id in proc.Signature.Parameters)
				{
					id.Accept(this);
				}
			}
			//$REVIEW: return type?
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
