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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class zSeriesRewriter
    {
        private void RewriteAhi2(PrimitiveType dt)
        {
            var imm = Const(instr.Operands[1]);
            var n = imm.ToInt16();
            Expression src = Reg(0, dt);
            src = m.AddSubSignedInt(src, n);
            var dst = Assign(Reg(0), src); 
            SetCc(m.Cond(dst));
        }

        private void RewriteAhi3(PrimitiveType dt)
        {
            var imm = Const(instr.Operands[1]);
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

        private void RewriteAl(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var dst = Assign(Reg(0), m.IAdd(src1, Op(1)));
            SetCc(m.Cond(dst));
        }

        private void RewriteAr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            var dt = PrimitiveType.Word32;
            var tmp = binder.CreateTemporary(dt);
            var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(src.DataType.BitSize - dt.BitSize));
            m.Assign(tmp, m.IAdd(m.Convert(dst, dst.DataType, dt), m.Convert(src, src.DataType, dt)));
            m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, dt.BitSize));
            m.Assign(dst, m.Seq(tmpHi, tmp));
            SetCc(m.Cond(tmp));
        }

        private void RewriteCghi()
        {
            var left = Reg(0);
            var imm = Const(instr.Operands[1]).ToInt64();
            var right = Constant.Create(left.DataType, imm);
            SetCc(m.Cond(m.ISub(left, right)));
        }

        private void RewriteCgr()
        {
            var src1 = Reg(0);
            var src2 = Reg(1);
            SetCc(m.Cond(m.ISub(src1, src2)));
        }


        private void RewriteChi()
        {
            var left = Reg(0);
            var imm = Const(instr.Operands[1]).ToInt16();
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
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClg()
        {
            var reg = Reg(0);
            var ea = EffectiveAddress(1);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(reg, m.Mem8(ea))));
        }

        private void RewriteClgr()
        {
            var src1 = Reg(0);
            var src2 = Rel(1, PrimitiveType.Word64);
            SetCc(m.Cond(m.ISub(src1, src2)));
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
            var imm = Const(instr.Operands[1]);
            SetCc(m.Cond(m.ISub(m.Mem8(ea), imm)));
        }

        private void RewriteCs(PrimitiveType dt)
        {
            var arg1 = Reg(0, dt);
            var arg2 = Reg(1, dt);
            var mem = m.AddrOf(arch.PointerType, m.Mem(dt, EffectiveAddress(2)));
            var o = m.Out(dt, arg1);
            SetCc(host.Intrinsic("__compare_and_swap", false, PrimitiveType.Byte, arg1, arg2, mem, o));
        }

        private void RewriteDp()
        {
            var eaLeft = (MemoryOperand) instr.Operands[0];
            var eaRight = (MemoryOperand) instr.Operands[1];
            var ptrLeft = binder.EnsureRegister(eaLeft.Base);
            var lenLeft = Constant.Create(PrimitiveType.Int32, eaLeft.Offset);
            var ptrRight= binder.EnsureRegister(eaRight.Base);
            var lenRight = Constant.Create(PrimitiveType.Int32, eaRight.Offset);
            SetCc(host.Intrinsic("__packed_divide", false, PrimitiveType.Byte, ptrLeft, lenLeft, ptrRight, lenRight, ptrLeft));
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

        private void RewriteLay()
        {
            var ea = EffectiveAddress(1);
            var dst = Reg(0);
            m.Assign(dst, ea);
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
            var imm = Const(instr.Operands[1]).ToInt16();
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
            int imm = Const(instr.Operands[1]).ToInt16();
            var dst = Reg(0);
            var src = Constant.Create(dst.DataType, imm);
            m.Assign(dst, src);
        }

        private void RewriteLgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, src);
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
            var src = Op(2);
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

        private void RewriteLr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            var excessBits = dst.DataType.BitSize - src.DataType.BitSize;
            if (excessBits > 0)
            {
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(excessBits));
                m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, src.DataType.BitSize));
                m.Assign(dst, m.Seq(dst, m.Slice(PrimitiveType.Word32, src, 0)));
            }
            else
            {
                m.Assign(dst, src);
            }
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
            var src = Constant.Create(dt, Const(instr.Operands[1]).ToInt64());
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

        private void RewriteNgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.And(dst, src));
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

        private void RewriteOr(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), m.Or(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteRisbg(string intrinsic)
        {
            var e = host.Intrinsic(intrinsic, true, PrimitiveType.Word64, Op(1), Op(2), Op(3), Op(4));
            var dst = Assign(Reg(0), e);
            SetCc(m.Cond(dst));
        }

        private void RewriteS(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var diff = m.ISub(src1, Op(1));
            diff.DataType = dt;
            var dst = Assign(Reg(0), diff);
            SetCc(m.Cond(dst));
        }

        private void RewriteSub2(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            var diff = m.ISub(src1, src2);
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
            int sh;
            if (instr.Operands[2] is AddressOperand addr)
                sh = (int) addr.Address.ToLinear() & 0x3F;
            else
                sh = (int) ((MemoryOperand) instr.Operands[2]).Offset & 0x3F;
            var src1 = Reg(1, dt);
            var dst = Assign(Reg(0), fn(src1, m.Int32(sh)));
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
            var right = Op(1);
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
