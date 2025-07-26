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
        private static readonly TraceSwitch trace = new(nameof(EquivalenceClassBuilder), "Trace EquivalenceClassBuilder") { Level = TraceLevel.Warning };

		private readonly TypeFactory factory;
		private readonly TypeStore store;
        private readonly IEventListener listener;
        private readonly Dictionary<ushort, TypeVariable> segTypevars;
        private readonly Dictionary<string, EquivalenceClass> typeReferenceClasses;
		private FunctionType? signature;
        private Statement? stmCur;

        /// <summary>
        /// Constructs an instance of the <see cref="EquivalenceClassBuilder"/> class.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="store"></param>
        /// <param name="listener"></param>
		public EquivalenceClassBuilder(TypeFactory factory, TypeStore store, IEventListener listener)
        {
			this.factory = factory;
			this.store = store;
            this.listener = listener;
			this.signature = null;
            this.segTypevars = new Dictionary<ushort, TypeVariable>();
            this.typeReferenceClasses = new Dictionary<string, EquivalenceClass>();
		}

        /// <summary>
        /// Builds the type variables for the program.
        /// </summary>
        /// <remarks>
        /// This method is called after all traits have been processed.
        /// </remarks>
        /// <param name="program">Program being analyzed.</param>
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

        /// <summary>
        /// Ensures that each <see cref="ImageSegment"/> has a type variable.
        /// </summary>
        /// <param name="segments">Sequence of <see cref="ImageSegment"/>s.</param>
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

        /// <summary>
        /// Ensure that type variables for the signature of a procedure
        /// exist.
        /// </summary>
        /// <param name="signature"></param>
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

        /// <summary>
        /// Ensures that an expression <paramref name="e"/> has a type variable.
        /// </summary>
        /// <param name="e">Expression </param>
        /// <returns></returns>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
		public override void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
			EnsureTypeVariable(acc);
		}

        /// <inheritdoc/>
		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
			a.Dst.Accept(this);
			store.MergeClasses(store.GetTypeVariable(a.Dst), store.GetTypeVariable(a.Src));
		}

        /// <inheritdoc/>
		public override void VisitStore(Store s)
		{
			s.Src.Accept(this);
			s.Dst.Accept(this);
			store.MergeClasses(store.GetTypeVariable(s.Dst), store.GetTypeVariable(s.Src));
		}

        /// <inheritdoc/>
        public override void VisitAddress(Address addr)
        {
            EnsureTypeVariable(addr);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
		public override void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
			EnsureTypeVariable(cast);
		}

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void VisitConditionalExpression(ConditionalExpression cond)
        {
            base.VisitConditionalExpression(cond);
            EnsureTypeVariable(cond);
        }

        /// <inheritdoc/>
        public override void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
			EnsureTypeVariable(cof);
		}

        /// <inheritdoc/>
        public override void VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
            EnsureTypeVariable(conversion);
        }

        /// <inheritdoc/>
		public override void VisitDefInstruction(DefInstruction def)
		{
		}

        /// <inheritdoc/>
		public override void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
			EnsureTypeVariable(deref);
		}

        /// <inheritdoc/>
		public override void VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override void VisitIdentifier(Identifier id)
		{
			EnsureTypeVariable(id);
		}

        /// <inheritdoc/>
		public override void VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
			EnsureTypeVariable(mps);
		}

        /// <inheritdoc/>
		public override void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
			EnsureTypeVariable(access);
		}

        /// <inheritdoc/>
		public override void VisitMkSequence(MkSequence seq)
		{
            foreach (var e in seq.Expressions)
            {
                e.Accept(this);
            }
			EnsureTypeVariable(seq);
		}

        /// <inheritdoc/>
        public override void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
            EnsureTypeVariable(outArg);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
		public override void VisitSegmentedAddress(SegmentedPointer address)
		{
			address.BasePointer.Accept(this);
			address.Offset.Accept(this);
			EnsureTypeVariable(address);
		}

        /// <inheritdoc/>
		public override void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
			store.MergeClasses(store.GetTypeVariable(phi.Src), store.GetTypeVariable(phi.Dst));
		}

        /// <inheritdoc/>
		public override void VisitPhiFunction(PhiFunction phi)
		{
			TypeVariable tPhi = EnsureTypeVariable(phi);
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
				store.MergeClasses(tPhi, store.GetTypeVariable(arg.Value));
			}
		}

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
		public override void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
			EnsureTypeVariable(slice);
		}

        /// <inheritdoc/>
        public override void VisitStringConstant(StringConstant str)
        {
            EnsureTypeVariable(str);
        }

        /// <inheritdoc/>
        public override void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
			EnsureTypeVariable(tc);
		}

        /// <inheritdoc/>
		public override void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
			EnsureTypeVariable(unary);
		}
	}
}
