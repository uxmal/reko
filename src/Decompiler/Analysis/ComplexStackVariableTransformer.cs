#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Rewrites expressions like <code>fp +/-offset</code> if offset is
    /// inside of one of the specified intervals. In particular it rewrites
    /// <code>fp - offset</code> to <code>&amp;tLoc_offset1 + offset2</code>
    /// where <code>offset1 - offset2 = offset</code>
    /// </summary>
    public class ComplexStackVariableTransformer : InstructionTransformer
    {
        private readonly SsaState ssa;
        private readonly IntervalTree<int, DataType> escapedFrameIntervals;
        private readonly IDecompilerEventListener eventListener;
        private readonly Dictionary<int, SsaIdentifier> frameIds;
        private Statement stmCur;
        private readonly ExpressionEmitter m;

        public ComplexStackVariableTransformer(
            SsaState ssa,
            IntervalTree<int, DataType> escapedFrameIntervals,
            IDecompilerEventListener eventListener)
        {
            this.ssa = ssa;
            this.escapedFrameIntervals = escapedFrameIntervals;
            this.eventListener = eventListener;
            this.frameIds = new();
            this.m = new();
            this.stmCur = default!;
        }

        public void Transform()
        {
            var proc = ssa.Procedure;
            foreach (var (interval, dt) in escapedFrameIntervals)
            {
                var id = proc.Frame.EnsureStackVariable(interval.Start, dt);
                var sid = ssa.EnsureDefInstruction(id, proc.EntryBlock);
                frameIds[interval.Start] = sid;
            }
            foreach (var stm in proc.Statements)
            {
                if (eventListener.IsCanceled())
                    return;
                this.stmCur = stm;
                stm.Instruction = stm.Instruction.Accept(this);
            }
        }

        #region IExpressionVisitor Members

        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            foreach (var use in ci.Uses.ToArray())
            {
                var e = use.Expression.Accept(this);
                ci.Uses.Remove(use);
                ci.Uses.Add(new CallBinding(use.Storage, e));
            }
            return base.TransformCallInstruction(ci);
        }

        public override Expression VisitBinaryExpression(BinaryExpression bin)
        {
            if (IsFrameAccess(bin, out var offset))
            {
                var bitSize = bin.DataType.BitSize;
                if (TryRewriteFrameOffset(offset, bitSize, out var e))
                    return e;
                return bin;
            }
            var left = bin.Left.Accept(this);
            var right = bin.Right.Accept(this);
            return new BinaryExpression(bin.Operator, bin.DataType, left, right);
        }

        #endregion

        private bool TryRewriteFrameOffset(
            int offset,
            int ptrBitSize,
            [MaybeNullWhen(false)] out Expression e)
        {
            var ints = escapedFrameIntervals.GetIntervalsOverlappingWith(
                Interval.Create(offset, offset + 1));
            if (!ints.Any())
            {
                e = null;
                return false;
            }
            var (i, _) = ints.First();
            var id = frameIds[i.Start].Identifier;
            var fp = ssa.Procedure.Frame.FramePointer;
            ssa.Identifiers[fp].Uses.Remove(stmCur);
            ssa.Identifiers[id].Uses.Add(stmCur);
            var ptr = new Pointer(id.DataType, ptrBitSize);
            e = m.AddSubSignedInt(m.AddrOf(ptr, id), offset - i.Start);
            return true;
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
    }
}
