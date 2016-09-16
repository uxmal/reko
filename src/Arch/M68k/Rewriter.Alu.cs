#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core;

namespace Reko.Arch.M68k
{
    /// <summary>
    /// Rewrites ALU instructions.
    /// </summary>
    public partial class Rewriter
    {
        public void RewriteArithmetic(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address,opSrc, binOpGen);
            AllConditions(opDst);
        }

        public void RewriteRotation(string procName)
        {
            Expression opDst;
            if (di.op2 != null)
            {
                var opSrc = orw.RewriteSrc(di.op1, di.Address);
                opDst = orw.RewriteDst(di.op2, di.Address, opSrc, (s, d) =>
                    host.PseudoProcedure(procName, di.dataWidth, d, s));
            }
            else
            {
                opDst = orw.RewriteDst(di.op1, di.Address,
                    Constant.Byte(1), (s, d) =>
                        host.PseudoProcedure(procName, PrimitiveType.Word32, d, s));
            }
            emitter.Assign(
                orw.FlagGroup(FlagM.CF | FlagM.NF | FlagM.ZF),
                emitter.Cond(opDst));
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        public void RewriteRotationX(string procName)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, (s, d) =>
                host.PseudoProcedure(procName, di.dataWidth, d, s, orw.FlagGroup(FlagM.XF)));
            emitter.Assign(
                orw.FlagGroup(FlagM.CF | FlagM.NF | FlagM.XF | FlagM.ZF),
                emitter.Cond(opDst));
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        public void RewriteTst()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.FlagGroup(FlagM.NF | FlagM.ZF);
            emitter.Assign(opDst, emitter.Cond(emitter.ISub(opSrc, 0)));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        public void RewriteShift(Func<Expression, Expression, Expression> binOpGen)
        {
            Expression opDst;
            if (di.op2 != null)
            {
                var opSrc = orw.RewriteSrc(di.op1, di.Address);
                opDst = orw.RewriteDst(di.op2, di.Address, opSrc, binOpGen);
            }
            else
            {
                var opSrc = Constant.Int32(1);
                opDst = orw.RewriteDst(di.op1, di.Address, PrimitiveType.Word16, opSrc, binOpGen);
            }
            AllConditions(opDst);
        }

        public void RewriteBchg()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var tmpMask = frame.CreateTemporary(PrimitiveType.UInt32);
            emitter.Assign(tmpMask, emitter.Shl(1, opSrc));
            var opDst = orw.RewriteDst(di.op2, di.Address, tmpMask, (s, d) => emitter.Xor(d, s));
            emitter.Assign(
                orw.FlagGroup(FlagM.ZF),
                emitter.Cond(emitter.And(opDst, tmpMask)));
        }

        public void RewriteBclrBset(string name)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            PrimitiveType w = (di.op2 is RegisterOperand)
                 ? PrimitiveType.Word32 
                 : PrimitiveType.Byte;
            di.op2.Width = w;
            orw.DataWidth = w;
            var opDst = orw.RewriteSrc(di.op2, di.Address);
            emitter.Assign(
                orw.FlagGroup(FlagM.ZF),
                host.PseudoProcedure(name, PrimitiveType.Bool, opDst, opSrc, emitter.Out(PrimitiveType.Pointer32, opDst)));
        }

        public void RewriteExg()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteSrc(di.op2, di.Address);
            var tmp = frame.CreateTemporary(PrimitiveType.Word32);
            emitter.Assign(tmp, opSrc);
            emitter.Assign(opSrc, opDst);
            emitter.Assign(opDst, tmp);
            LogicalConditions(opDst);
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
            var dReg = orw.RewriteSrc(di.op1, di.Address);
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
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, binOpGen);
            LogicalConditions(opDst);
        }

        public void RewriteMul(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, PrimitiveType.Int32, opSrc, binOpGen);
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF), emitter.Cond(opDst));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        public void RewriteUnary(Func<Expression, Expression> unaryOpGen, Action<Expression> generateFlags)
        {
            var op = orw.RewriteUnary(di.op1, di.Address, di.dataWidth, unaryOpGen);
            generateFlags(op);
        }

        private Expression MaybeCast(PrimitiveType width, Expression expr)
        {
            if (expr.DataType.Size == width.Size)
                return expr;
            else
                return emitter.Cast(width, expr);
        }

        private void RewriteAddSubq(Func<Expression,Expression,Expression> opGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var regDst = di.op2 as RegisterOperand;
            if (regDst != null && regDst.Register is AddressRegister)
            {
                var opDst = frame.EnsureRegister(regDst.Register);
                emitter.Assign(opDst, opGen(opSrc, opDst));
            }
            else
            {
                var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, opGen);
                emitter.Assign(orw.FlagGroup(FlagM.CVZNX), emitter.Cond(opDst));
            }
        }

        public void RewriteAddSubx(BinaryOperator opr)
        {
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var x = orw.FlagGroup(FlagM.XF);
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteDst(di.op2, di.Address, src, (d, s) => 
                new BinaryExpression(
                    opr,
                    di.dataWidth,
                    new BinaryExpression(opr, di.dataWidth, d, s),
                    x));
            emitter.Assign(orw.FlagGroup(FlagM.CVZNX), emitter.Cond(dst));
        }

        private void RewriteScc(ConditionCode cc, FlagM flagsUsed)
        {
            orw.RewriteMoveDst(di.op1, di.Address, PrimitiveType.Byte, orw.FlagGroup(flagsUsed));
        }
        
        private void RewriteSwap()
        {
            var r = (RegisterOperand) di.op1;
            var reg = frame.EnsureRegister(r.Register);
            emitter.Assign(reg, host.PseudoProcedure("__swap", PrimitiveType.Word32, reg));
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF), emitter.Cond(reg));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteBinOp(Func<Expression,Expression,Expression> opGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, opGen);
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> opGen, FlagM flags)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, opGen);
            emitter.Assign(orw.FlagGroup(flags), emitter.Cond(opDst));
        }

        private void RewriteBtst()
        {
            orw.DataWidth = di.op1 is RegisterOperand ? PrimitiveType.Word32 : PrimitiveType.Byte;
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteSrc(di.op2, di.Address);
            emitter.Assign(
                orw.FlagGroup(FlagM.ZF),
                host.PseudoProcedure("__btst", PrimitiveType.Bool,
                    opDst, opSrc));
        }

        private void RewriteClr()
        {
            var src = Constant.Create(di.dataWidth, 0);
            var opDst = orw.RewriteMoveDst(di.op1, di.Address, di.dataWidth, src);
            emitter.Assign(orw.FlagGroup(FlagM.ZF), Constant.True());
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.NF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteCmp()
        {
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteSrc(di.op2, di.Address);
            var tmp = frame.CreateTemporary(dst.DataType);
            emitter.Assign(tmp, emitter.ISub(dst, src));
            emitter.Assign(
                orw.FlagGroup(FlagM.CF | FlagM.NF | FlagM.VF | FlagM.ZF),
                emitter.Cond(tmp));
        }

        private void RewriteDiv(Func<Expression,Expression,Expression> op, DataType dt)
        {
            Debug.Print(di.dataWidth.ToString());
            if (di.dataWidth.BitSize == 16)
            {
                di.dataWidth = PrimitiveType.UInt32;
                var src = orw.RewriteSrc(di.op1, di.Address);
                var rem = frame.CreateTemporary(dt);
                var quot = frame.CreateTemporary(dt);
                var regDst = frame.EnsureRegister(((RegisterOperand) di.op2).Register);
                emitter.Assign(rem, emitter.Cast(rem.DataType, emitter.Remainder(regDst, src)));
                emitter.Assign(quot, emitter.Cast(quot.DataType, op(regDst, src)));
                emitter.Assign(regDst, emitter.Dpb(regDst, rem, 16));
                emitter.Assign(regDst, emitter.Dpb(regDst, quot, 0));
                emitter.Assign(
                    orw.FlagGroup(FlagM.NF | FlagM.VF | FlagM.ZF),
                    emitter.Cond(quot));
                emitter.Assign(
                    orw.FlagGroup(FlagM.CF), Constant.False());
                return;
            }
            throw new NotImplementedException(di.ToString());
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
            return orw.RewriteSrc(mop, di.Address);
        }

        public Expression GetEffectiveAddress(MachineOperand op)
        {
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                if (mem.Base == null)
                {
                    return mem.Offset;
                }
                else if (mem.Base == Registers.pc)
                {
                    return di.Address + mem.Offset.ToInt32();
                }
                else if (mem.Offset == null)
                {
                    return frame.EnsureRegister(mem.Base);
                }
                else {
                    return emitter.IAdd(
                        frame.EnsureRegister(mem.Base),
                        Constant.Int32(mem.Offset.ToInt32()));
                }
            }
            var addrOp = di.op1 as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            var indIdx = di.op1 as IndirectIndexedOperand;
            if (indIdx != null)
            {
                var a = frame.EnsureRegister(indIdx.ARegister);
                var x = frame.EnsureRegister(indIdx.XRegister);
                return emitter.IAdd(a, x);        //$REVIEW: woefully incomplete...
            }
            throw new NotImplementedException(string.Format("{0} ({1})", op, op.GetType().Name));
        }

        public void RewriteLea()
        {
            var dst = orw.RewriteSrc(di.op2, di.Address);
            var src = GetEffectiveAddress(di.op1);
            emitter.Assign(dst, src);
        }

        public void RewritePea()
        {
            var sp = frame.EnsureRegister(arch.StackRegister);
            emitter.Assign(sp, emitter.ISub(sp, 4));
            var ea = GetEffectiveAddress(di.op1);
            emitter.Assign(emitter.LoadDw(sp), ea);
        }

        public void RewriteMove(bool setFlag)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, (s, d) => s);
            var isSr = GetRegister(di.op1) == Registers.sr || GetRegister(di.op2) == Registers.sr;
            if (setFlag && !isSr)
            {
                emitter.Assign(
                    orw.FlagGroup(FlagM.CVZN),
                    emitter.Cond(opDst));
            }
        }

        public void RewriteMoveq()
        {
            var opSrc = (sbyte) ((M68kImmediateOperand) di.op1).Constant.ToInt32();
            var opDst = frame.EnsureRegister(((RegisterOperand)di.op2).Register);
            emitter.Assign(opDst, Constant.Int32(opSrc));
            emitter.Assign(
                orw.FlagGroup(FlagM.CVZN),
                emitter.Cond(opDst));
        }

        public IEnumerable<Identifier> RegisterMaskIncreasing(uint bitSet)
        {
            for (int i = 0, mask = 0x8000; i < 16; ++i, mask >>= 1)
            {
                if ((bitSet & mask) != 0)
                {
                    yield return frame.EnsureRegister(arch.GetRegister(i));
                }
            }
        }

        public IEnumerable<Identifier> RegisterMaskDecreasing(uint bitSet)
        {
            for (int i = 15, mask = 1; i >= 0; --i, mask <<= 1)
            {
                if ((bitSet & mask) != 0)
                    yield return frame.EnsureRegister(arch.GetRegister(i));
            }
        }

        public void RewriteMovem()
        {
            var dstRegs = di.op2 as RegisterSetOperand;
            if (dstRegs != null)
            {
                var postInc = di.op1 as PostIncrementMemoryOperand;
                Identifier srcReg = null;
                if (postInc != null)
                {
                    srcReg = frame.EnsureRegister(postInc.Register);
                }
                else
                {
                    var src = orw.RewriteSrc(di.op1, di.Address) as MemoryAccess;
                    if (src == null)
                        throw new AddressCorrelatedException(di.Address, "Unsupported addressing mode for {0}.", di);
                    srcReg = frame.CreateTemporary(di.dataWidth);
                    emitter.Assign(srcReg, src.EffectiveAddress);
                }
                foreach (var reg in RegisterMaskIncreasing(dstRegs.BitSet))
                {
                    emitter.Assign(reg, emitter.Load(di.dataWidth, srcReg));
                    emitter.Assign(srcReg, emitter.IAdd(srcReg, di.dataWidth.Size));
                }
                return;
            }
            dstRegs = di.op1 as RegisterSetOperand;
            if (dstRegs != null)
            {
                var preDec = di.op2 as PredecrementMemoryOperand;
                if (preDec != null)
                {
                    var dstReg = frame.EnsureRegister(preDec.Register);
                    foreach (var reg in RegisterMaskDecreasing(dstRegs.BitSet))
                    {
                        emitter.Assign(dstReg, emitter.ISub(dstReg, di.dataWidth.Size));
                        emitter.Assign(emitter.Load(di.dataWidth, dstReg), reg);
                    }
                }
                else
                {
                    var src = orw.RewriteSrc(di.op2, di.Address) as MemoryAccess;
                    if (src == null)
                        throw new AddressCorrelatedException(di.Address, "Unsupported addressing mode for {0}.", di);
                    var srcReg = frame.CreateTemporary(di.dataWidth);
                    emitter.Assign(srcReg, src.EffectiveAddress);
                    foreach (var reg in RegisterMaskIncreasing(dstRegs.BitSet))
                    {
                        emitter.Assign(reg, emitter.Load(di.dataWidth, srcReg));
                        emitter.Assign(srcReg, emitter.IAdd(srcReg, di.dataWidth.Size));
                    }
                }
                return;
            }
            throw new AddressCorrelatedException(di.Address, "Unsupported addressing mode for {0}.", di);
        }

        private Expression RewriteNegx(Expression expr)
        {
            expr = emitter.Neg(expr);
            return emitter.ISub(expr, orw.FlagGroup(FlagM.XF));
        }

        private void RewriteLink()
        {
            var aReg = orw.RewriteSrc(di.op1, di.Address);
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
            var aReg = orw.RewriteSrc(di.op1, di.Address);
            var aSp = frame.EnsureRegister(arch.StackRegister);
            emitter.Assign(aSp, aReg);
            emitter.Assign(aReg, emitter.LoadDw(aSp));
            emitter.Assign(aSp, emitter.IAdd(aSp, 4));
        }

        private void Copy(Expression dst, Expression src, int bitSize)
        {
            if (dst is Identifier && dst.DataType.BitSize > bitSize)
            {
                var dpb = emitter.Dpb(dst, src, 0);
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
