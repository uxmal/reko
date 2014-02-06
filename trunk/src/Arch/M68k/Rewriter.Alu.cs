#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
            var opSrc = orw.RewriteSrc(di.op1);
            var opDst = orw.RewriteDst(di.op2, opSrc, binOpGen);
            AllConditions(opDst);
        }

        public void RewriteShift(Func<Expression, Expression, Expression> binOpGen)
        {
            Expression opDst;
            if (di.op2 != null)
            {
                var opSrc = orw.RewriteSrc(di.op1);
                opDst = orw.RewriteDst(di.op2, opSrc, binOpGen);
            }
            else
            {
                var opSrc = Constant.Int32(1);
                opDst = orw.RewriteDst(di.op1, PrimitiveType.Word16, opSrc, binOpGen);
            }
            AllConditions(opDst);
        }

        public void RewriteBchg()
        {
            throw new NotImplementedException("Haven't gotten this work yet.");
            var opSrc = orw.RewriteSrc(di.op1);
            var tmpMask = frame.CreateTemporary(PrimitiveType.UInt32);
            emitter.Assign(tmpMask, emitter.Shl(1, opSrc));
            var opDst = orw.RewriteDst(di.op2, tmpMask, (s, d) => emitter.Xor(d, s));
            emitter.Assign(
                orw.FlagGroup(FlagM.ZF),
                emitter.Cond(emitter.And(opDst, tmpMask)));
        }

        public void RewriteExt()
        {
            PrimitiveType dtSrc;
            PrimitiveType dtDst;
            if (di.dataWidth.Size == 4)
            {
                dtSrc = PrimitiveType.Int16;
                dtDst = PrimitiveType.Int32;
            }
            else
            {
                dtSrc = PrimitiveType.SByte;
                dtDst = PrimitiveType.Int16;
            }
            var dReg = frame.EnsureRegister(((RegisterOperand) di.op1).Register);
            emitter.Assign(
                dReg,
                emitter.Cast(dtDst, emitter.Cast(dtSrc, dReg)));
            emitter.Assign(
                orw.FlagGroup(FlagM.NF | FlagM.ZF),
                emitter.Cond(dReg));
        }

        public void RewriteExtb()
        {
            var dReg = orw.RewriteSrc(di.op1);
            emitter.Assign(
                dReg, 
                emitter.Cast(PrimitiveType.Int32,
                    emitter.Cast(PrimitiveType.SByte, dReg)));
            emitter.Assign(
                orw.FlagGroup(FlagM.NF | FlagM.ZF),
                emitter.Cond(dReg));
        }

        public void RewriteLogical(Func<Expression, Expression, Expression> binOpGen)
        {
            var width = di.dataWidth;
            var opSrc = orw.RewriteSrc(di.op1);
            var opDst = orw.RewriteDst(di.op2, opSrc, binOpGen);
            LogicalConditions(opDst);
        }

        public void RewriteMul(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.op1);
            var opDst = orw.RewriteDst(di.op2, PrimitiveType.Int32, opSrc, binOpGen);
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF), emitter.Cond(opDst));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        public void RewriteUnary(Func<Expression, Expression> unaryOpGen, Action<Expression> generateFlags)
        {
            var op = orw.RewriteUnary(di.op1, di.dataWidth, unaryOpGen);
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
            var opSrc = orw.RewriteSrc(di.op1);
            var opDst = orw.RewriteDst(di.op2, opSrc, opGen);
        }

        private void RewriteClr()
        {
            var src = Constant.Create(di.dataWidth, 0);
            var opDst = orw.RewriteMoveDst(di.op1, di.dataWidth, src);
            emitter.Assign(orw.FlagGroup(FlagM.ZF), Constant.True());
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.NF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteCmp()
        {
            var src = orw.RewriteSrc(di.op1);
            var dst = orw.RewriteSrc(di.op2);
            var tmp = frame.CreateTemporary(dst.DataType);
            emitter.Assign(tmp, emitter.ISub(dst, src));
            emitter.Assign(
                orw.FlagGroup(FlagM.CF | FlagM.NF | FlagM.VF | FlagM.ZF),
                emitter.Cond(tmp));
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
                emitter.Assign(reg, emitter.ISub(reg, di.dataWidth.Size));
                var op = emitter.Load(di.dataWidth, reg);
                m(opSrc, op);
                return op;
            }
            var postInc = mop as PostIncrementMemoryOperand;
            if (postInc != null)
            {
                var reg = frame.EnsureRegister(postInc.Register);
                var t = frame.CreateTemporary(di.dataWidth);
                if (NeedsSpilling(opSrc))
                {
                    emitter.Assign(t, opSrc);
                    opSrc = t;
                }
                m(opSrc, emitter.Load(di.dataWidth, reg));
                emitter.Assign(reg, emitter.IAdd(reg, di.dataWidth.Size));
                return t;
            }
            return orw.RewriteSrc(mop);
        }

        public void RewriteMove(bool setFlag)
        {
            var opSrc = orw.RewriteSrc(di.op1);
            var opDst = orw.RewriteDst(di.op2, opSrc, (s, d) => s);
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

        private void RewriteLink()
        {
            var aReg = orw.RewriteSrc(di.op1);
            var aSp = frame.EnsureRegister(arch.StackRegister);
            var imm = ((M68kImmediateOperand) di.op2).Constant.ToInt32();
            emitter.Assign(aSp, emitter.ISub(aSp, 4));
            emitter.Assign(emitter.LoadDw(aSp), aReg);
            emitter.Assign(aReg, aSp);
            if (imm < 0)
            {
                emitter.Assign(aSp, emitter.ISub(aSp, -imm));
            }
            else
            {
                emitter.Assign(aSp, emitter.IAdd(aSp, -imm));
            }
        }

        private void RewriteUnlk()
        {
            var aReg = orw.RewriteSrc(di.op1);
            var aSp = frame.EnsureRegister(arch.StackRegister);
            emitter.Assign(aSp, aReg);
            emitter.Assign(aReg, emitter.LoadDw(aSp));
            emitter.Assign(aSp, emitter.IAdd(aSp, 4));
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
