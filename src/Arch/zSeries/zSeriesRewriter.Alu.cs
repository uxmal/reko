#region License
/*
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class zSeriesRewriter
    {
        private void RewriteAhi2(PrimitiveType dt)
        {
            var imm = Const(1);
            var n = imm.ToInt16();
            Expression src = Reg(0, dt);
            src = m.AddSubSignedInt(src, n);
            var dst = Assign(Reg(0), src); 
            SetCc(m.Cond(dst));
        }

        private void RewriteAhi3(PrimitiveType dt)
        {
            var imm = Const(1);
            var n = imm.ToInt16();
            Expression src = Reg(2, dt);
            src = m.AddSubSignedInt(src, n);
            var dst = Assign(Reg(0), src);
            SetCc(m.Cond(dst));
        }

        private void RewriteAgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.IAdd(dst, src));
            SetCc(m.Cond(dst));
        }

        private void RewriteA(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = m.Mem(dt, EffectiveAddress(1));
            var dst = Assign(Reg(0), m.IAdd(src1, src2));
            SetCc(m.Cond(dst));
        }

        private void RewriteAdcSbcReg(Func<Expression,Expression,Expression> fn, PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            var dst = Assign(Reg(0), fn(fn(src1, src2), cc));
            SetCc(m.Cond(dst));
        }

        private void RewriteAlugfr(Func<Expression, Expression, Expression> fn, PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var src1 = Reg(0, dtDst);
            var src2 = Reg(1, dtSrc);
            var dst = Assign(Reg(0), fn(src1, m.Convert(src2, dtSrc, dtDst)));
            SetCc(m.Cond(dst));
        }

        private void RewriteAr(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            var dst = Assign(Reg(0), m.IAdd(src1, src2));
            SetCc(m.Cond(dst));
        }

        private void RewriteAsi(PrimitiveType dt)
        {
            var ea = EffectiveAddress(0);
            var left = binder.CreateTemporary(dt);
            m.Assign(left, m.Mem(dt, ea));
            var c = Constant.Create(dt, this.Const(1).ToInt64());
            var sum = binder.CreateTemporary(dt);
            m.Assign(sum, m.IAdd(left, c));
            m.Assign(m.Mem(dt, ea), sum);
            SetCc(m.Cond(sum));
        }

        private void RewriteC(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var ea = EffectiveAddress(1);
            SetCc(m.Cond(m.ISub(left, m.Mem(dt, ea))));
        }

        private void RewriteCghi()
        {
            var left = Reg(0);
            var imm = Const(1).ToInt64();
            var right = Constant.Create(left.DataType, imm);
            SetCc(m.Cond(m.ISub(left, right)));
        }

        private void RewriteCr(Func<Expression,Expression,Expression> fn, PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            var diff = fn(src1, src2);
            diff.DataType = dt;
            SetCc(m.Cond(diff));
        }

        private void RewriteChi()
        {
            var left = Reg(0);
            var imm = Const(1).ToInt16();
            var right = Constant.Create(left.DataType, imm);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClc()
        {
            var mem = (MemoryOperand)instr.Operands[0];
            var dt = PrimitiveType.CreateWord(mem.Length);
            var left = m.Mem(dt, EffectiveAddress(0));
            var right = m.Mem(dt, EffectiveAddress(1));
            SetCc(m.Cond(m.ISub(left, right)));
        }

        private void RewriteClcl(PrimitiveType dt)
        {
            var leftHi = ((RegisterOperand) instr.Operands[0]).Register;
            var leftLo = NextGpReg(leftHi);
            var rightHi = ((RegisterOperand) instr.Operands[1]).Register;
            var rightLo = NextGpReg(rightHi);
            var left = binder.EnsureSequence(dt, leftHi, leftLo);
            var right = binder.EnsureSequence(dt, rightHi, rightLo);
            SetCc(m.Cond(m.USub(left, right)));
        }

        private void RewriteCl(PrimitiveType dt)
        {
            var reg = Reg(0, dt);
            var ea = EffectiveAddress(1);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(reg, m.Mem(dt, ea))));
        }

        private void RewriteClrl(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Rel(1, dt);
            SetCc(m.Cond(m.USub(src1, src2)));
        }

        private void RewriteClfi(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Imm(1, dt);
            SetCc(m.Cond(m.ISub(left, right)));
        }

        private void RewriteCli()
        {
            var ea = EffectiveAddress(0);
            var imm = Const(1);
            SetCc(m.Cond(m.ISub(m.Mem8(ea), imm)));
        }

        private void RewriteCmpH(PrimitiveType dtResult, PrimitiveType dtHalf)
        {
            var src1 = Reg(0, dtResult);
            var src2 = m.Convert(m.Mem(dtHalf, EffectiveAddress(1)), dtHalf, dtResult);
            var diff = m.ISub(src1, src2);
            diff.DataType = dtResult;
            SetCc(m.Cond(diff));
        }

        private void RewriteCs(PrimitiveType dt)
        {
            var arg1 = Reg(0, dt);
            var arg2 = Reg(1, dt);
            var mem = m.AddrOf(arch.PointerType, m.Mem(dt, EffectiveAddress(2)));
            var o = m.Out(dt, arg1);
            SetCc(host.Intrinsic("__compare_and_swap", false, PrimitiveType.Byte, arg1, arg2, mem, o));
        }

        private void RewriteD()
        {
            var dividend = Reg(0);
            var divisor = m.Mem(PrimitiveType.Int32, EffectiveAddress(1));
            var quo = binder.CreateTemporary(PrimitiveType.Int32);
            var rem = binder.CreateTemporary(PrimitiveType.Int32);
            m.Assign(quo, m.SDiv(dividend, divisor));
            m.Assign(rem, m.Mod(dividend, divisor));
            var quoReg = ((RegisterOperand) instr.Operands[0]).Register;
            var remReg = NextGpReg(quoReg);
            Assign(binder.EnsureRegister(quoReg), quo);
            Assign(binder.EnsureRegister(remReg), rem);
        }

        private void RewriteDlr(PrimitiveType dt)
        {
            var divhi = Reg(0, dt);
            var divlo = NextGpReg(0, dt);
            var dtDividend = PrimitiveType.Create(Domain.UnsignedInt, dt.BitSize);
            var dividend = binder.EnsureSequence(dtDividend, divhi.Storage, divlo.Storage);
            Assign(divhi, m.UDiv(dt, dividend, Reg(1, dt)));
            Assign(divlo, m.Mod(dt, dividend, Reg(1, dt)));
        }

        private void RewriteDp()
        {
            var eaLeft = (MemoryOperand) instr.Operands[0];
            var eaRight = (MemoryOperand) instr.Operands[1];
            var ptrLeft = binder.EnsureRegister(eaLeft.Base!);
            var lenLeft = Constant.Create(PrimitiveType.Int32, eaLeft.Offset);
            var ptrRight= binder.EnsureRegister(eaRight.Base!);
            var lenRight = Constant.Create(PrimitiveType.Int32, eaRight.Offset);
            SetCc(host.Intrinsic("__packed_divide", false, PrimitiveType.Byte, ptrLeft, lenLeft, ptrRight, lenRight, ptrLeft));
        }

        private void RewriteDr()
        {
            var dividend = Reg(0);
            var divisor = Reg(1, PrimitiveType.Int32);
            var quo = binder.CreateTemporary(PrimitiveType.Int32);
            var rem = binder.CreateTemporary(PrimitiveType.Int32);
            m.Assign(quo, m.SDiv(dividend, divisor));
            m.Assign(rem, m.Mod(dividend, divisor));
            var quoReg = ((RegisterOperand) instr.Operands[0]).Register;
            var remReg = NextGpReg(quoReg);
            Assign(binder.EnsureRegister(quoReg), quo);
            Assign(binder.EnsureRegister(remReg), rem);
        }

        private void RewriteDsgr(PrimitiveType dt)
        {
            var divhi = Reg(0, dt);
            var divlo = NextGpReg(0, dt);
            Assign(divhi, m.SDiv(divhi, Reg(1, dt)));
            Assign(divlo, m.Mod(dt, divhi, Reg(1, dt)));
        }

        private void RewriteIc()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(tmp, m.Mem8(EffectiveAddress(1)));
            var dst = Reg(0);
            Assign(Reg(0), m.Dpb(dst, tmp, 0));
        }

        private void RewriteLa()
        {
            var ea = EffectiveAddress(1);
            var dst = Reg(0);
            m.Assign(dst, ea);
        }

        private void RewriteLarl()
        {
            Expression src = Addr(1);
            Identifier dst = Reg(0);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Dpb(dst, src, 0);
            }
            m.Assign(dst, src);
        }

        private void RewriteL(PrimitiveType pt)
        {
            var ea = EffectiveAddress(1);
            var src = m.Mem(pt, ea);
            Assign(Reg(0), src);
        }

        private void RewriteL(PrimitiveType ptFrom, PrimitiveType ptTo)
        {
            var ea = EffectiveAddress(1);
            var src = m.Mem(ptFrom, ea);
            Assign(Reg(0), m.Convert(src, ptFrom, ptTo));
        }

        private void RewriteLaa(Func<Expression,Expression,Expression> fn, PrimitiveType dt)
        {
            var src1 = Reg(1);
            var tmp = binder.CreateTemporary(src1.DataType);
            var ea = EffectiveAddress(2);
            m.Assign(tmp, src1);
            Assign(src1, m.IAdd(src1, m.Mem(dt, ea)));
            Assign(Reg(0), tmp);
            this.SetCc(m.Cond(src1));
        }

        private void RewriteLay()
        {
            var ea = EffectiveAddress(1);
            var dst = Reg(0);
            m.Assign(dst, ea);
        }

        private void RewriteLcr(PrimitiveType dt, Func<Expression, Expression> fn)
        {
            var src = Reg(1, dt);
            var dst = Assign(Reg(0), fn(src));
            SetCc(m.Cond(dst));
        }

        private void RewriteLdgr()
        {
            var src = Reg(1);
            var dst = FReg(0);
            m.Assign(dst, src);
        }

        private void RewriteLgf()
        {
            var src = m.Mem(PrimitiveType.Int32, EffectiveAddress(1));
            var dst = Reg(0);
            m.Assign(dst, m.Convert(src, PrimitiveType.Int32, PrimitiveType.Int64));
        }

        private void RewriteLgdr()
        {
            var src = FReg(1);
            var dst = Reg(0);
            m.Assign(dst, src);
        }

        private void RewriteLgfr()
        {
            var src = Reg(1);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, m.Convert(src, src.DataType, tmp.DataType));
            var dst = Reg(0);
            m.Assign(dst, m.Convert(tmp, tmp.DataType, PrimitiveType.Int64));
        }

        private void RewriteLghi()
        {
            var imm = Const(1).ToInt16();
            var dst = Reg(0);
            var src = Constant.Create(dst.DataType, imm);
            m.Assign(dst, src);
        }

        private void RewriteLgrl()
        {
            var addr = PcRel(1);
            var dst = Reg(0);
            m.Assign(dst, m.Mem64(addr));
        }

        private void RewriteLhi()
        {
            int imm = Const(1).ToInt16();
            var dst = Reg(0);
            var src = Constant.Create(dst.DataType, imm);
            m.Assign(dst, src);
        }

        private void RewriteLl(PrimitiveType dt)
        {
            var ea = EffectiveAddress(1);
            var src = m.Mem(dt, ea);
            m.Assign(Reg(0), m.Convert(src, src.DataType, arch.WordWidth));
        }

        private void RewriteLli(PrimitiveType dt, int bitOffset)
        {
            var imm = Imm(1, dt);
            var dst = Reg(0);
            Assign(dst, m.Dpb(dst, imm, bitOffset));
        }

        private void RewriteLreg(PrimitiveType dt)
        {
            var src = Reg(1, dt);
            Assign(Reg(0), src);
        }

        private void RewriteLmg()
        {
            var rStart = ((RegisterOperand)instr.Operands[0]).Register;
            var rEnd = ((RegisterOperand)instr.Operands[1]).Register;
            var ea = EffectiveAddress(2);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            int i = rStart.Number;
            for (; ; )
            {
                var r = binder.EnsureRegister(Registers.GpRegisters[i]);
                m.Assign(r, m.Mem(r.DataType, tmp));
                if (i == rEnd.Number)
                    break;
                m.Assign(tmp, m.IAdd(tmp, Constant.Int(r.DataType, r.DataType.Size)));
                i = (i + 1) % 16;
            }
        }

        private void RewriteLnr(PrimitiveType dt)
        {
            var src = Reg(1, dt);
            var dst = Assign(Reg(0), m.Neg(src));
            SetCc(m.Cond(dst));
        }

        private void RewriteLoc(PrimitiveType dt, ConditionCode ccode)
        {
            if (ccode != ConditionCode.ALWAYS)
            {
                var cc = binder.EnsureFlagGroup(Registers.CC);
                m.Branch(m.Test(ccode.Invert(), cc), instr.Address + instr.Length);
            }
            var src = Op(2, dt);
            src.DataType = dt;
            Assign(Reg(0), src);
        }

        private void RewriteLpr(string fnName, PrimitiveType dt)
        {
            Expression src = Reg(1, dt);
            src = host.Intrinsic(fnName, true, dt, src);
            var dst = Assign(Reg(0), src);
            SetCc(m.Cond(dst));
        }


        private void RewriteLogic(PrimitiveType dt, Func<Expression, Expression, Expression> fn)
        {
            var left = Reg(0, dt);
            var right = m.Mem(dt, EffectiveAddress(1));
            var dst = Assign(Reg(0), fn(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteLogicR(PrimitiveType dt, Func<Expression, Expression, Expression> fn)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), fn(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteLr(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            Expression src = Reg(1, dtSrc);
            var excessBits = dtDst.BitSize - dtSrc.BitSize;
            if (excessBits > 0)
            {
                src = m.Convert(src, dtSrc, dtDst);
            }
            Assign(Reg(0), src);
        }

        private void RewriteLrv(PrimitiveType dt)
        {
            var src = Reg(1, dt);
            Assign(Reg(0), host.Intrinsic($"__load_reverse_{dt.BitSize}", false, dt, src));
        }

        private void RewriteLt(PrimitiveType dt)
        {
            var ea = EffectiveAddress(1);
            var dst = Assign(Reg(0), m.Mem(dt, ea));
            SetCc(m.Cond(m.ISub(dst, 0)));
        }

        private void RewriteLtr(PrimitiveType dt)
        {
            var src = Reg(1, dt);
            var dst = Assign(Reg(0), src);
            SetCc(m.Cond(m.ISub(dst, 0)));
        }

        private void RewriteM(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var left = NextGpReg(0, dtSrc);
            var right = m.Mem(dtSrc, EffectiveAddress(1));
            m.Assign(Reg(0), m.SMul(dtDst, left, right));

        }

        private void RewriteMr(Func<PrimitiveType,Expression,Expression,Expression> fn, PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var left = Reg(0, dtSrc);
            var right = Reg(1, dtSrc);
            var dstlo = NextGpReg(((RegisterOperand) instr.Operands[0]).Register);
            var dst = binder.EnsureSequence(dtDst, left.Storage, dstlo);
            m.Assign(dst, fn(dtDst, left, right));
        }

        private void RewriteMulR(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            Assign(Reg(0), m.SMul(left, right));
        }

        private void RewriteMvcle()
        {
            //$BUG: this isn't 100% correct, but we need a starting point.
            var ea = EffectiveAddress(2);
            Identifier dst = Seq(PrimitiveType.Word128, 0, 1);
            var result = Assign(dst, host.Intrinsic("__mvcle", true, dst.DataType, ea));
            SetCc(m.Cond(result));
        }

        private void RewriteMvi(PrimitiveType dt)
        {
            var src = Constant.Create(dt, Const(1).ToInt64());
            var ea = EffectiveAddress(0);
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteMvz()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(1);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(0);

            m.Assign(tmp, host.Intrinsic("__move_zones", false, dt, m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
            m.Assign(m.Mem(dt, eaDst), tmp);
        }

        private void RewriteNc()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(1);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(0);

            m.Assign(tmp, m.And(m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
            m.Assign(m.Mem(dt, eaDst), tmp);
            
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(tmp));
        }

        private void RewriteNr(PrimitiveType dt)
        {
            var src1 = Reg(1, dt);
            var src2 = Reg(1, dt);
            var dst = Assign(Reg(0), m.And(src1, src2));
            SetCc(m.Cond(dst));
        }

        private void RewriteNi()
        {
            var right = Imm(1, PrimitiveType.Byte);
            var left = m.Mem8(EffectiveAddress(0));
            var tmp = binder.CreateTemporary(left.DataType);
            m.Assign(tmp, m.And(left, right));
            var dst = m.Mem8(EffectiveAddress(0));
            m.Assign(dst, tmp);
            SetCc(m.Cond(tmp));
        }

        private void RewriteOi()
        {
            var right = Imm(1, PrimitiveType.Byte);
            var left = m.Mem8(EffectiveAddress(0));
            var tmp = binder.CreateTemporary(left.DataType);
            m.Assign(tmp, m.Or(left, right));
            m.Assign(left, tmp);
            SetCc(m.Cond(tmp));
        }

        private void RewriteRisbg(string intrinsic)
        {
            var dt = PrimitiveType.Word64;
            var e = host.Intrinsic(intrinsic, true, dt, Op(1, dt), Op(2, dt), Op(3, dt), Op(4, dt));
            var dst = Assign(Reg(0), e);
            SetCc(m.Cond(dst));
        }

        private void RewriteRll(PrimitiveType dt)
        {

        }

        private void RewriteS(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var diff = m.ISub(src1, Op(1, dt));
            diff.DataType = dt;
            var dst = Assign(Reg(0), diff);
            SetCc(m.Cond(dst));
        }

        private void RewriteSub2(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Op(1, dt);
            var diff = m.ISub(src1, src2);
            var dst = Assign(Reg(0), diff);
            SetCc(m.Cond(dst));
        }

        private void RewriteAlu3(Func<Expression, Expression, Expression> fn, PrimitiveType dtResult)
        {
            var src1 = Reg(1, dtResult);
            var src2 = Op(2, dtResult);
            var bin = fn(src1, src2);
            var dst = Assign(Reg(0), bin);
            SetCc(m.Cond(dst));
        }

        private void RewriteAluH(Func<Expression,Expression,Expression> fn, PrimitiveType dtResult, PrimitiveType dtHalf)
        {
            var src1 = Reg(0, dtResult);
            var src2 = m.Convert(m.Mem(dtHalf, EffectiveAddress(1)), dtHalf, dtResult);
            var diff = fn(src1, src2);
            diff.DataType = dtResult;
            var dst = Assign(Reg(0), diff);
            SetCc(m.Cond(dst));
        }

        private void RewriteShift2(PrimitiveType dt, Func<Expression,Expression,Expression> fn)
        {
            int sh;
            if (instr.Operands[1] is AddressOperand addr)
                sh = (int) addr.Address.ToLinear() & 0x3F;
            else 
                sh = (int)((MemoryOperand)instr.Operands[1]).Offset & 0x3F;
            var src = Reg(0, dt);
            var dst = Assign(Reg(0), fn(src, m.Int32(sh)));
            SetCc(m.Cond(dst));
        }

        private void RewriteShift3(PrimitiveType dt, Func<Expression, Expression, Expression> fn)
        {
            var sh = EffectiveAddress(2);
            if (sh is Address addr)
                sh = Constant.Int32((int) addr.ToLinear());
            var src1 = Reg(1, dt);
            var dst = Assign(Reg(0), fn(src1, sh));
            SetCc(m.Cond(dst));
        }

        private void RewriteSt(PrimitiveType dt)
        {
            Expression src = Reg(0);
            if (dt.BitSize < 64)
            {
                src = m.Slice(dt, src, 0);
            }
            var ea = EffectiveAddress(1);
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteStgrl()
        {
            var addr = PcRel(1);
            var src = Reg(0);
            m.Assign(m.Mem64(addr), src);
        }

        private void RewriteStmg()
        {
            var rStart = ((RegisterOperand)instr.Operands[0]).Register;
            var rEnd = ((RegisterOperand)instr.Operands[1]).Register;
            var ea = EffectiveAddress(2);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            int i = rStart.Number;
            for (; ; )
            {
                var r = binder.EnsureRegister(Registers.GpRegisters[i]);
                m.Assign(m.Mem(r.DataType, tmp), r);
                if (i == rEnd.Number)
                    break;
                m.Assign(tmp, m.IAdd(tmp, Constant.Int(r.DataType, r.DataType.Size)));
                i = (i + 1) % 16;
            }
        }

        private void RewriteXor2(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Op(1, dt);
            var dst = Assign(Reg(0), m.Xor(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteXc()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(1);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(0);

            if (cmp.Equals(eaSrc, eaDst))
            {
                m.Assign(tmp, Constant.Zero(dt));
                m.Assign(m.Mem(dt, eaDst), Constant.Zero(dt));
            }
            else
            {
                m.Assign(tmp, m.Xor(m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
                m.Assign(m.Mem(dt, eaDst), tmp);
            }
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(tmp));
        }
    }
}
