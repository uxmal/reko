#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    /// <summary>
    /// Rewrites ALU instructions.
    /// </summary>
    public partial class Rewriter
    {
        public void RewriteArithmetic(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteDst(di.Instruction.op2, opSrc, binOpGen);
            AllConditions(opDst);
        }

        public void RewriteLogical(Func<Expression, Expression, Expression> binOpGen)
        {
            var width = di.Instruction.dataWidth;
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteDst(di.Instruction.op2, opSrc, binOpGen);
            LogicalConditions(opDst);
        }

        public void RewriteMul(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteDst(di.Instruction.op2, PrimitiveType.Int32, opSrc, binOpGen);
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF), emitter.Cond(opDst));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        public void RewriteUnary(Func<Expression, Expression> unaryOpGen, Action<Expression> generateFlags)
        {
            var op = orw.RewriteUnary(di.Instruction.op1, di.Instruction.dataWidth, unaryOpGen);
            generateFlags(op);
        }

        private Expression MaybeCast(PrimitiveType width, Expression expr)
        {
            if (expr.DataType.Size == width.Size)
                return expr;
            else
                return emitter.Cast(width, expr);
        }

        private void RewriteBinOp(Func<Expression,Expression,Expression> opGen)
        {
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteDst(di.Instruction.op2, opSrc, opGen);
        }

        private Expression RewriteSrcOperand(MachineOperand mop)
        {
            return RewriteDstOperand(mop, null, (s, d) => { });
        }

        private bool NeedsSpilling(Expression op)
        {
            //$REVIEW: May not need to spill here if opSrc is immediate / register other than reg
            if (op == null)
                return false;
            if (op is Constant)
                return false;
            return true;
        }

        private Expression RewriteDstOperand(MachineOperand mop, Expression opSrc, Action<Expression, Expression> m)
        {
            var preDec = mop as PredecrementMemoryOperand;
            if (preDec != null)
            {
                var reg = frame.EnsureRegister(preDec.Register);
                var t = frame.CreateTemporary(opSrc.DataType);
                if (NeedsSpilling(opSrc))
                {
                    emitter.Assign(t, opSrc);
                    opSrc = t;
                }
                emitter.Assign(reg, emitter.ISub(reg, di.Instruction.dataWidth.Size));
                var op = emitter.Load(di.Instruction.dataWidth, reg);
                m(opSrc, op);
                return op;
            }
            var postInc = mop as PostIncrementMemoryOperand;
            if (postInc != null)
            {
                var reg = frame.EnsureRegister(postInc.Register);
                var t = frame.CreateTemporary(di.Instruction.dataWidth);
                if (NeedsSpilling(opSrc))
                {
                    emitter.Assign(t, opSrc);
                    opSrc = t;
                }
                m(opSrc, emitter.Load(di.Instruction.dataWidth, reg));
                emitter.Assign(reg, emitter.IAdd(reg, di.Instruction.dataWidth.Size));
                return t;
            }
            return orw.RewriteSrc(mop);
        }

        public void RewriteMove(bool setFlag)
        {
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteDst(di.Instruction.op2, opSrc, (s, d) => s);
            if (setFlag)
            {
                emitter.Assign(
                    frame.EnsureFlagGroup(
                        (uint)(FlagM.CF | FlagM.NF | FlagM.VF | FlagM.ZF),
                        "CVZN",
                        PrimitiveType.Byte),
                    emitter.Cond(opDst));
            }
        }

        private Expression RewriteNegx(Expression expr)
        {
            expr = emitter.Neg(expr);
            return emitter.ISub(expr, orw.FlagGroup(FlagM.XF));
        }

        private void Copy(Expression dst, Expression src, int bitSize)
        {
            if (dst is Identifier && dst.DataType.BitSize > bitSize)
            {
                var dpb = emitter.Dpb(dst, src, 0, bitSize);
                emitter.Assign(dst, dpb);
            }
            else
            {
                emitter.Assign(dst, src);
            }
        }

        private void AllConditions(Expression expr)
        {
            var f = orw.FlagGroup(FlagM.CF | FlagM.NF | FlagM.VF | FlagM.XF | FlagM.ZF);
            emitter.Assign(f, emitter.Cond(expr));
        }

        private void LogicalConditions(Expression expr)
        {
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF), emitter.Cond(expr));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }
    }
}
