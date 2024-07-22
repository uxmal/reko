#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Services;
using System;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Forward analysis that propagates constants,
    /// but only within a single linear block.
    /// </summary>
    /// <remarks>
    /// This class is used to clean up code that has long chains
    /// of const->register assignments. This is needed for 
    /// copying with switch statements on RiscV processors,
    /// where the bgeu instruction is used for range checks 
    /// but only uses registers.
    /// </remarks>
    public class BlockConstantPropagator : RtlInstructionVisitor<RtlInstruction>
    {
        private readonly Dictionary<Identifier, Constant> constants;
        private readonly ExpressionEmitter m;

        public BlockConstantPropagator(IReadOnlySegmentMap memory, IDecompilerEventListener listener)
        {
            this.constants = new();
            this.m = new ExpressionEmitter();
        }

        public RtlInstruction VisitAssignment(RtlAssignment ass)
        {
            var newSrc = VisitExpression(ass.Src);
            if (ass.Dst is Identifier idDst)
            {
                if (idDst.Storage is FlagGroupStorage grf)
                {
                    return new RtlAssignment(idDst, newSrc);
                }
                if (newSrc is Constant c && c.IsValid)
                {
                    constants[idDst] = c;
                    return new RtlAssignment(idDst, c);
                }
                c = InvalidConstant.Create(ass.Dst.DataType);
                constants[idDst] = c; 
                return new RtlAssignment(idDst, newSrc);
            }
            else
            {
                return ass;
            }
        }

        public RtlInstruction VisitBranch(RtlBranch branch)
        {
            var newCond = VisitExpression(branch.Condition);
            return new RtlBranch(newCond, (Address)branch.Target, branch.Class);
        }

        public RtlInstruction VisitCall(RtlCall call)
        {
            var target = VisitExpression(call.Target);
            return new RtlCall(target, call.ReturnAddressSize, call.Class, call.Architecture);
        }

        public RtlInstruction VisitGoto(RtlGoto go)
        {
            var target = VisitExpression(go.Target);
            return new RtlGoto(target, go.Class);
        }

        public RtlInstruction VisitIf(RtlIf rtlIf)
        {
            throw new NotImplementedException();
        }

        public RtlInstruction VisitInvalid(RtlInvalid invalid)
        {
            return invalid;
        }

        public RtlInstruction VisitMicroGoto(RtlMicroGoto uGoto)
        {
            return uGoto;
        }

        public RtlInstruction VisitNop(RtlNop rtlNop)
        {
            return rtlNop;
        }

        public RtlInstruction VisitReturn(RtlReturn ret)
        {
            return ret;
        }

        public RtlInstruction VisitSideEffect(RtlSideEffect side)
        {
            return new RtlSideEffect(VisitExpression(side.Expression), side.Class);
        }

        public RtlInstruction VisitSwitch(RtlSwitch sw)
        {
            return sw;
        }

        private Expression VisitExpression(Expression e)
        {
            return e switch
            {
                Constant c => c,
                Identifier id => VisitIdentifier(id),
                BinaryExpression bin => VisitBinaryExpression(bin),
                ConditionalExpression cond => m.Conditional(
                    cond.DataType, 
                    VisitExpression(cond.Condition),
                    VisitExpression(cond.ThenExp),
                    VisitExpression(cond.FalseExp)),
                MemoryAccess mem => m.Mem(mem.MemoryId, mem.DataType, VisitExpression(mem.EffectiveAddress)),
                Address addr => addr,
                Slice sl => m.Slice(VisitExpression(sl.Expression), sl.DataType, sl.Offset),
                Conversion conv => m.Convert(VisitExpression(conv.Expression), conv.SourceDataType, conv.DataType),
                ConditionOf cof => m.Cond(VisitExpression(cof.Expression)),
                TestCondition tc => m.Test(tc.ConditionCode, VisitExpression(tc.Expression)),
                MkSequence seq => m.Seq(seq.DataType, VisitExpressions(seq.Expressions)),
                UnaryExpression u => m.Unary(u.Operator, u.DataType, VisitExpression(u.Expression)),
                SegmentedPointer sp => m.SegPtr(sp.DataType, VisitExpression(sp.BasePointer), VisitExpression(sp.Offset)),
                OutArgument o => o,
                ProcedureConstant pc => pc,
                Application app => m.Fn(VisitExpression(app.Procedure), VisitExpressions(app.Arguments)),
                ArrayAccess aref => m.ARef(aref.DataType, VisitExpression(aref.Array), VisitExpression(aref.Index)),
                _ => Fail(e)
            };
        }

        private Expression VisitIdentifier(Identifier id)
        {
            if (id.Storage is FlagGroupStorage grf)
                return id;
            return this.constants.TryGetValue(id, out var cId) && cId.IsValid
                ? cId
                : id;
        }

        private Expression Fail(Expression e)
        {
            Console.WriteLine("*** Failed propagation of {0} ({1}).", e, e.GetType().Name);
            throw new NotImplementedException(e.GetType().Name);
        }

        private Expression VisitBinaryExpression(BinaryExpression bin)
        {
            var left = VisitExpression(bin.Left);
            var right = VisitExpression(bin.Right);
            var cLeft = left as Constant;
            var cRight = right as Constant;
            if (cLeft is not null)
            {
                if (cRight is not null)
                    return bin.Operator.ApplyConstants(bin.DataType, cLeft, cRight);
                else if (bin.Operator is ConditionalOperator cop)
                    return m.Bin(cop.Mirror(), bin.DataType, right, cLeft);
            }
            return m.Bin(bin.Operator, bin.DataType, left, right);
        }

      

        private Expression[] VisitExpressions(Expression[] exprs)
        {
            int cExprs = exprs.Length;
            var newExprs = new Expression[cExprs];
            for (int i = 0; i < cExprs; i++)
            {
                newExprs[i] = VisitExpression(exprs[i]);
            }
            return newExprs;
        }

        private class EvalCtx : EvaluationContext
        {
            private readonly BlockConstantPropagator outer;

            public EvalCtx(BlockConstantPropagator outer)
            {
                this.outer = outer;
            }

            public EndianServices Endianness => throw new NotImplementedException();

            public int MemoryGranularity => throw new NotImplementedException();

            public Expression? GetDefiningExpression(Identifier id)
            {
                if (outer.constants.TryGetValue(id, out var c) && c.IsValid)
                    return c;
                else 
                    return null;
            }

            public Expression? GetValue(Identifier id)
            {
                if (outer.constants.TryGetValue(id, out var c) && c.IsValid)
                    return c;
                return id;

            }

            public Expression GetValue(MemoryAccess access, IMemory memory)
            {
                return access;
            }

            public Expression GetValue(Application appl)
            {
                return appl;
            }

            public bool IsUsedInPhi(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                throw new NotImplementedException();
            }

            public Constant ReinterpretAsFloat(Constant rawBits)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
            }
        }
    }
}
