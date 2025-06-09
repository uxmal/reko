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
using Reko.Core.Code;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Services;
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
        private static readonly TraceSwitch trace = new(nameof(EquivalenceClassBuilder), "Trace EquivalenceClassBuilder") { Level = TraceLevel.Warning };

		private readonly TypeFactory factory;
		private readonly TypeStore store;
        private readonly IDecompilerEventListener listener;
        private readonly Dictionary<ushort, TypeVariable> segTypevars;
        private readonly Dictionary<string, EquivalenceClass> typeReferenceClasses;
		private FunctionType? signature;
        private Statement? stmCur;

		public EquivalenceClassBuilder(TypeFactory factory, TypeStore store, IDecompilerEventListener listener)
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
			var tvGlobals = store.EnsureExpressionTypeVariable(factory, null, program.Globals, "globals_t");
            tvGlobals.OriginalDataType = program.Globals.DataType;

            EnsureSegmentTypeVariables(program.SegmentMap.Segments.Values
                .Concat(program.SegmentMap.Selectors.Values));
            int cProc = program.Procedures.Count;
            int i = 0;
            foreach (Procedure proc in program.Procedures.Values)
            {
                if (listener.IsCanceled())
                    return;
                listener.Progress.ShowProgress("Gathering primitive datatypes from instructions.", i++, cProc);
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
            foreach (var segment in segments.Where(s => s.Identifier is not null))
            {
                var selector = segment.Address.Selector;
                if (selector.HasValue)
                {
                    store.ClearTypeVariable(segment.Identifier);
                    var tvSelector = store.EnsureExpressionTypeVariable(factory, segment.Address, segment.Identifier);
                    this.segTypevars[selector.Value] = tvSelector;
                    tvSelector.OriginalDataType = factory.CreatePointer(
                        segment.Fields,
                        segment.Identifier.DataType.BitSize);
                    store.SegmentTypes[segment] = segment.Fields;
                }
            }
        }

        public void EnsureSignatureTypeVariables(FunctionType signature)
        {
            if (signature is null || !signature.ParametersValid)
                return;
            if (!signature.HasVoidReturn)
            {
                signature.Outputs[0].Accept(this);
            }
            foreach (var param in signature.Parameters!)
            {
                param.Accept(this);
            }
        }

        public TypeVariable EnsureTypeVariable(Expression e)
		{
            var tv = store.EnsureExpressionTypeVariable(factory, stmCur?.Address, e);
            var typeref = e.DataType.ResolveAs<TypeReference>();
            if (typeref is not null)
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
			FunctionType? sig = signature;

			if (sig is not null)
			{
                var totalArgs = sig.Parameters!.Length + sig.Outputs.Length - 1;
				if (!sig.IsVariadic && totalArgs != appl.Arguments.Length)
					throw new InvalidOperationException("Parameter count must match.");
			}

            var inputs = sig?.Parameters;
            var outputs = sig?.Outputs;
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
                if (sig is null)
                    continue;
                Debug.Assert(inputs is not null);
                Debug.Assert(outputs is not null);
                if (i < inputs.Length)
				{
					EnsureTypeVariable(inputs[i]);
                    var tvArgument = store.GetTypeVariable(appl.Arguments[i]);
                    var tvParameter = store.GetTypeVariable(inputs[i]);
                    store.MergeClasses(tvArgument, tvParameter);
				}
                else
                {
                    int iOut = i - inputs.Length + 1;
                    if (iOut < outputs.Length)
                    {
                        EnsureTypeVariable(outputs[iOut]);
                        var tvArgument = store.GetTypeVariable(appl.Arguments[i]);
                        var tvParameter = store.GetTypeVariable(outputs[iOut]);
                        store.MergeClasses(tvArgument, tvParameter);
                    }
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
			store.MergeClasses(store.GetTypeVariable(a.Dst), store.GetTypeVariable(a.Src));
		}

		public override void VisitStore(Store s)
		{
			s.Src.Accept(this);
			s.Dst.Accept(this);
			store.MergeClasses(store.GetTypeVariable(s.Dst), store.GetTypeVariable(s.Src));
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
				store.MergeClasses(store.GetTypeVariable(binExp.Left), store.GetTypeVariable(binExp.Right));
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
                    store.SetTypeVariable(c, tv);
                }
                else
                {
                    tv = EnsureTypeVariable(c)!;
                    segTypevars[c.ToUInt16()] = tv;
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

        public override void VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
            EnsureTypeVariable(conversion);
        }

		public override void VisitDefInstruction(DefInstruction def)
		{
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
            if (ret.Expression is null)
                return;
            ret.Expression.Accept(this);
            if (signature is not null && !signature.HasVoidReturn)
            {
                if (!store.TryGetTypeVariable(signature.Outputs[0], out var tvReturn))
                {
                    trace.Warn("Eqb: {0:X}: Type variable for return value of signature of {1} is missing", stmCur!.Address, stmCur.Block.Procedure.Name);
                    return;
                }
                store.MergeClasses(tvReturn, store.GetTypeVariable(ret.Expression));
            }
        }

		public override void VisitSegmentedAddress(SegmentedPointer address)
		{
			address.BasePointer.Accept(this);
			address.Offset.Accept(this);
			EnsureTypeVariable(address);
		}

		public override void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			store.MergeClasses(store.GetTypeVariable(phi.Src), store.GetTypeVariable(phi.Dst));
		}

		public override void VisitPhiFunction(PhiFunction phi)
		{
			TypeVariable tPhi = EnsureTypeVariable(phi);
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
				store.MergeClasses(tPhi, store.GetTypeVariable(arg.Value));
			}
		}

		public override void VisitProcedureConstant(ProcedureConstant pc)
		{
			EnsureTypeVariable(pc);
			VisitProcedure(pc.Procedure, pc.Signature);
			if (pc.Signature.ParametersValid)
			{
				store.MergeClasses(store.GetTypeVariable(pc), pc.Signature.TypeVariable ?? pc.Procedure.Signature.TypeVariable!);
				signature = pc.Signature;
			}
		}

		public void VisitProcedure(ProcedureBase proc, FunctionType sig)
		{
            if (sig.TypeVariable is null)
			{
                Address? addr = (proc is Procedure userProc)
                    ? userProc.EntryAddress
                    : null;

                sig.TypeVariable = store.EnsureExpressionTypeVariable(
					factory,
                    addr,
                    new Identifier("signature of " + proc.Name, VoidType.Instance, null!),
					null);
			}
			if (sig.Parameters is not null)
			{
				foreach (Identifier id in sig.Parameters)
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

        public override void VisitStringConstant(StringConstant str)
        {
            EnsureTypeVariable(str);
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
