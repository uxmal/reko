#region License
/* 
 * Copyright (C) 1999-2024 Pavel Tomin.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Collect stack frame intervals which can be accessed during call to
    /// other procedures. The intervals are obtained by merging of each
    /// frame expression like <code>fp +/-offset</code> copied outside of
    /// current procedure frame.
    /// </summary>
    public class EscapedFrameIntervalsFinder : ExpressionVisitorBase, InstructionVisitor
    {
        private readonly IReadOnlyProgram program;
        private readonly ProgramDataFlow flow;
        private readonly SsaState ssa;
        private readonly IEventListener eventListener;
        private Context ctx;
        private ExpressionSimplifier eval;
        private IntervalTree<int, DataType> intervals;

        public EscapedFrameIntervalsFinder(
            IReadOnlyProgram program,
            ProgramDataFlow flow,
            SsaState ssa,
            IEventListener eventListener)
        {
            this.program = program;
            this.flow = flow;
            this.ssa = ssa;
            this.eventListener = eventListener;
            this.ctx = default!;
            this.eval = default!;
            this.intervals = default!;
        }

        private void CreateEvaluationContext()
        {
            this.ctx = new Context(ssa.Procedure.Architecture);
            this.eval = new ExpressionSimplifier(
                program.Memory, ctx, eventListener);
        }

        public IntervalTree<int, DataType> Find()
        {
            this.intervals = new IntervalTree<int, DataType>();
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                CreateEvaluationContext();
                foreach (var stm in block.Statements)
                {
                    if (eventListener.IsCanceled())
                        return intervals;
                    stm.Instruction.Accept(this);
                }
            }
            return intervals;
        }

        #region InstructionVisitor Members

        public void VisitAssignment(Assignment ass)
        {
            var (e, _) = ass.Src.Accept(eval);
            e.Accept(this);
        }

        public void VisitBranch(Branch branch)
        {
            var (e, _) = branch.Condition.Accept(eval);
            e.Accept(this);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            if (ci.Callee is not ProcedureConstant pc ||
                pc.Procedure is not Procedure proc ||
                !flow.ProcedureFlows.TryGetValue(proc, out var procFlow))
                return;
            foreach (var use in ci.Uses)
            {
                if (!procFlow.LiveInDataTypes.TryGetValue(use.Storage, out var dt))
                    continue;
                var (e, _) = use.Expression.Accept(eval);
                DataType? pointee = DeterminePointee(dt);
                if (pointee is not null && 
                    IsFrameAccess(e, out var offset))
                {
                    AddInterval(offset, pointee);
                }
            }
        }

        private DataType? DeterminePointee(DataType dt)
        {
            var ptr = dt.ResolveAs<Pointer>();
            DataType dtPointee;
            if (ptr is not null)
            {
                dtPointee = ptr.Pointee;
            }
            else
            {
                var memptr = dt.ResolveAs<MemberPointer>();
                if (memptr is null)
                    return null;
                dtPointee = memptr.Pointee;
            }
            if (dtPointee.MeasureBitSize(ssa.Procedure.Architecture.MemoryGranularity) == 0)
            {
                //$TODO: This happens when arguments are void *. (e.g. memset).
                // Other mechanisms will be needed to specify parameter sizes.
                // For now, to avoid crashes, return the size of a machine word.
                return ssa.Procedure.Architecture.WordWidth;
            }
            return dtPointee;
        }

        public void VisitComment(CodeComment code)
        {
        }

        public void VisitDefInstruction(DefInstruction def)
        {
        }

        public void VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            if (gotoInstruction.Condition is not null)
            {
                var (e, _) = gotoInstruction.Condition.Accept(eval);
                e.Accept(this);
            }
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression is not null)
            {
                var (e, _) = ret.Expression.Accept(eval);
                e.Accept(this);
            }
        }

        public void VisitSideEffect(SideEffect side)
        {
            var (e, _) = side.Expression.Accept(eval);
            e.Accept(this);
        }

        public void VisitStore(Store store)
        {
            var (value, _) = store.Src.Accept(eval);
            value.Accept(this);
            if (store.Dst is MemoryAccess mem)
            {
                ctx.SetValueEa(mem.EffectiveAddress, value);
            }
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            var (e, _) = si.Expression.Accept(eval);
            e.Accept(this);
        }

        public void VisitUseInstruction(UseInstruction use)
        {
        }

        #endregion

        #region IExpressionVisitor Members

        public override void VisitApplication(Application appl)
        {
            appl.Procedure.Accept(this);
            var sig = GetSignature(appl.Procedure);
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this);
                var dtArg = GetArgumentDataType(appl.Arguments, sig, i);
                var pointee = DeterminePointee(dtArg);
                if (pointee is not null && IsFrameAccess(appl.Arguments[i], out var offset))
                {
                    AddInterval(offset, pointee);
                }
            }
        }

        /// <summary>
        /// Get the data type of the i'th parameter, preferring the data type
        /// from the signature parameters. If the procedure is varargs or has
        /// no signature, fall back on the argument types.
        private static DataType GetArgumentDataType(Expression[] arguments, FunctionType? sig, int i)
        {
            if (sig is not null && sig.ParametersValid && i < sig.Parameters!.Length)
            {
                return sig.Parameters[i].DataType;
            }
            else
            {
                return arguments[i].DataType;
            }
        }

        private static FunctionType? GetSignature(Expression callee)
        {
            if (callee is ProcedureConstant pc &&
                pc.Procedure is ProcedureBase proc)
            {
                return proc.Signature;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void AddInterval(int offset, DataType dt)
        {
            var newInterval = Interval.Create(offset, offset + dt.MeasureSize());
            var ints = intervals.GetIntervalsOverlappingWith(
                newInterval).Select(de => de.Key).ToArray();
            foreach (var interval in ints)
            {
                if (interval.Covers(newInterval))
                {
                    return;
                }
                else if (newInterval.Covers(interval))
                {
                    intervals.Delete(interval);
                }
                else
                {
                    int start = Math.Min(interval.Start, newInterval.Start);
                    int end = Math.Max(interval.End, newInterval.End);
                    AddInterval(start, new UnknownType(end - start));
                    return;
                }
            }
            intervals.Add(newInterval, dt);
        }

        private bool IsFrameAccess(Expression e, out int offset)
        {
            offset = 0;
            if (e == ssa.Procedure.Frame.FramePointer)
            {
                offset = 0;
                return true;
            }
            if (e is not BinaryExpression bin)
                return false;
            if (bin.Left != ssa.Procedure.Frame.FramePointer)
                return false;
            if (bin.Right is not Constant c)
                return false;
            if (bin.Operator.Type == OperatorType.ISub)
            {
                offset = -c.ToInt32();
                return true;
            }
            if (bin.Operator.Type == OperatorType.IAdd)
            {
                offset = c.ToInt32();
                return true;
            }
            return false;
        }

        private class Context : EvaluationContext
        {
            private readonly IProcessorArchitecture arch;
            private Identifier? @base;
            /// <summary>
            /// Values at [base + offset] address.
            /// </summary>
            private readonly Dictionary<int, Expression> offsetValues;

            public Context(IProcessorArchitecture arch)
            {
                this.arch = arch;
                this.offsetValues = new();
            }

            public EndianServices Endianness => arch.Endianness;

            public Expression? GetDefiningExpression(Identifier id)
            {
                return id;
            }

            public Expression? GetValue(Identifier id)
            {
                return id;
            }

            public Expression GetValue(MemoryAccess access, IMemory memory)
            {
                var ea = access.EffectiveAddress;
                if (!IsIdentifierOffset(ea, out var @base, out var offset))
                    return access;
                if (this.@base != @base)
                    return access;
                if (!offsetValues.TryGetValue(offset, out var value))
                    return access;
                if (value.DataType.BitSize != access.DataType.BitSize)
                    return access;
                if (value is not Identifier)
                {
                    value = value.CloneExpression();
                    value.DataType = access.DataType;
                }
                return value;
            }

            public Expression GetValue(Application appl)
            {
                return appl;
            }

            public bool IsUsedInPhi(Identifier id)
            {
                return false;
            }

            public Expression MakeSegmentedAddress(Constant seg, Constant off)
            {
                return arch.MakeSegmentedAddress(seg, off);
            }

            public int MemoryGranularity => arch.MemoryGranularity;

            public Constant ReinterpretAsFloat(Constant rawBits)
            {
                return arch.ReinterpretAsFloat(rawBits);
            }

            public void RemoveExpressionUse(Expression expr)
            {
            }

            public void RemoveIdentifierUse(Identifier id)
            {
            }

            public void SetValue(Identifier id, Expression value)
            {
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression ea, Expression value)
            {
                if (IsIdentifierOffset(ea, out var @base, out var offset))
                {
                    if (this.@base != @base)
                    {
                        // Base identifier was changed. All stored memory data
                        // become invalid.
                        this.@base = @base;
                        offsetValues.Clear();
                    }
                    offsetValues[offset] = value;
                    return;
                }
                else if (ea is Constant)
                {
                    // Constant memory access should not affect stored data
                    return;
                }
                // Complex access can affect stored data
                this.@base = null;
                offsetValues.Clear();
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
            }

            private bool IsIdentifierOffset(
                Expression ea,
                out Identifier? @base,
                out int offset)
            {
                if (ea is Identifier id)
                {
                    @base = id;
                    offset = 0;
                    return true;
                }

                if (
                    ea is BinaryExpression bin &&
                    bin.Operator.Type.IsAddOrSub() &&
                    bin.Left is Identifier idLeft)
                {
                    if (bin.Right is Constant cOffset)
                    {
                        offset = cOffset.ToInt32();
                        if (bin.Operator.Type == OperatorType.ISub)
                            offset = -offset;
                        @base = idLeft;
                        return true;
                    }
                }
                @base = null;
                offset = 0;
                return false;
            }
        }
    }
}
