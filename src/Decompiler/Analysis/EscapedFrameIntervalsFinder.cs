#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private readonly Program program;
        private readonly ProgramDataFlow flow;
        private readonly SsaState ssa;
        private readonly DecompilerEventListener eventListener;
        private Context ctx;
        private ExpressionSimplifier eval;
        private IntervalTree<int, DataType> intervals;

        public EscapedFrameIntervalsFinder(
            Program program,
            ProgramDataFlow flow,
            SsaState ssa,
            DecompilerEventListener eventListener)
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
                program.SegmentMap, ctx, eventListener);
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
                var ptr = dt.ResolveAs<Pointer>();
                if (ptr is null)
                    continue;
                var pointee = ptr.Pointee;
                if (IsFrameAccess(use.Expression, out var offset))
                {
                    AddInterval(offset, pointee);
                }
            }
        }

        public void VisitComment(CodeComment code)
        {
        }

        public void VisitDeclaration(Declaration decl)
        {
            if (decl.Expression is not null)
            {
                var (e, _) = decl.Expression.Accept(eval);
                e.Accept(this);
            }
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
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this);
                var ptr = appl.Arguments[i].DataType.ResolveAs<Pointer>();
                if (ptr is null)
                    continue;
                var pointee = ptr.Pointee;
                if (IsFrameAccess(appl.Arguments[i], out var offset))
                {
                    AddInterval(offset, pointee);
                }
            }
        }

        #endregion

        private void AddInterval(int offset, DataType dt)
        {
            var newInterval = Interval.Create(offset, offset + MeasureSize(dt));
            var ints = intervals.GetIntervalsOverlappingWith(
                newInterval).Select(de => de.Key).ToArray();
            foreach(var interval in ints)
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

        /// <summary>
        /// Compute the size of the data type <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt"></param>
        /// <remarks>
        /// We don't trust the <see cref="DataType.Size"/> property because
        /// the types may be inferred. For instance, inferred <see cref="StructureType"/>s
        /// don't have a value for their Size properties.
        /// </remarks>
        //$TODO: this should be a method on the DataType class.
        private int MeasureSize(DataType dt)
        {
            int offset = 0;
            for (; ; )
            {
                switch (dt)
                {
                case PrimitiveType _:
                case Pointer _:
                case MemberPointer _:
                case VoidType _:
                case UnknownType _:
                case EnumType _:
                    return offset + dt.Size;
                case StructureType st:
                    if (st.Size > 0)
                        return st.Size; // Trust the user/metadata
                    var field = st.Fields.LastOrDefault();
                    if (field is null)
                        return 0;
                    offset += field.Offset;
                    dt = field.DataType;
                    break;
                case UnionType ut:
                    int unionSize = 0;
                    foreach (var alt in ut.Alternatives.Values)
                    {
                        unionSize = Math.Max(unionSize, MeasureSize(alt.DataType));
                    }
                    return offset + unionSize;
                case ArrayType array:
                    var elemSize = MeasureSize(array.ElementType);
                    return offset + elemSize * array.Length;
                case ClassType cls:
                    if (cls.Size > 0)
                        return cls.Size; // Trust the user/metadata
                    var cfield = cls.Fields.LastOrDefault();
                    if (cfield is null)
                        return 0;
                    offset += cfield.Offset;
                    dt = cfield.DataType;
                    break;
                case TypeVariable tv:
                    dt = tv.DataType;
                    break;
                case EquivalenceClass eq:
                    dt = eq.DataType;
                    break;
                case TypeReference tref:
                    dt = tref.Referent;
                    break;
                case ReferenceTo refto:
                    dt = refto.Referent;
                    break;
                default:
                    throw new NotImplementedException($"MeasureSize: {dt.GetType().Name} not implemented.");
                }
            }
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
            if (bin.Operator == Operator.ISub)
            {
                offset = -c.ToInt32();
                return true;
            }
            if (bin.Operator == Operator.IAdd)
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

            public List<Statement> GetDefiningStatementClosure(Identifier id)
            {
                return new List<Statement>();
            }

            public Expression? GetValue(Identifier id)
            {
                return id;
            }

            public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
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

            public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
            {
                return access;
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
                Expression ea, out Identifier? @base, out int offset)
            {
                if (ea is Identifier id)
                {
                    @base = id;
                    offset = 0;
                    return true;
                }

                if (
                    ea is BinaryExpression bin &&
                    (
                        bin.Operator == Operator.IAdd ||
                        bin.Operator == Operator.ISub) &&
                    bin.Left is Identifier idLeft)
                {
                    if (bin.Right is Constant cOffset)
                    {
                        offset = cOffset.ToInt32();
                        if (bin.Operator == Operator.ISub)
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
