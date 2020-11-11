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
        private void RewriteAhi(PrimitiveType dt)
        {
            var imm = Const(instr.Operands[1]);
            var n = imm.ToInt16();
            var reg = Reg(0);
            Expression dst;
            Expression src;
            if (reg.DataType.BitSize > dt.BitSize)
            {
                src = m.Convert(reg, reg.DataType, dt);
                dst = binder.CreateTemporary(dt);
            }
            else
            {
                src = reg;
                dst = reg;
            }
            if (n > 0)
            {
                src = m.IAdd(src, Constant.Int(dt, n));
            }
            else if (n < 0)
            {
                src = m.ISub(src, Constant.Int(dt, -n));
            }
            m.Assign(dst, src);
            if (reg.DataType.BitSize > dt.BitSize)
            {
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(reg.DataType.BitSize - dt.BitSize));
                m.Assign(tmpHi, m.Slice(tmpHi.DataType, reg, dt.BitSize));
                m.Assign(reg, m.Seq(tmpHi, dst));
            }
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteAgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.IAdd(dst, src));
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
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
            var src2 = Reg(1);
            SetCc(m.Cond(m.ISub(src1, src2)));
        }

        private void RewriteCli()
        {
            var ea = EffectiveAddress(0);
            var imm = Const(instr.Operands[1]);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(m.Mem8(ea), imm)));
        }

        private void RewriteLa()
        {
            var ea = EffectiveAddress(1);
            var dst = Reg(0);
            m.Assign(dst, ea);
        }

        private void RewriteLarl()
        {
            Expression src = Addr(instr.Operands[1]);
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
            var dst = Reg(0);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(dst.DataType.BitSize - src.DataType.BitSize));
                m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, src.DataType.BitSize));
                m.Assign(dst, m.Seq(tmpHi, src));
            }
            else
            {
                m.Assign(dst, src);
            }
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

        private void RewriteLtg()
        {
            var ea = EffectiveAddress(1);
            var dst = Reg(0);
            m.Assign(dst, m.Mem64(ea));
            SetCc(m.Cond(m.ISub(dst, 0)));
        }

        private void RewriteLtgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, src);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteMvi()
        {
            var src = Const(instr.Operands[1]);
            var ea = EffectiveAddress(0);
            m.Assign(m.Mem8(ea), src);
        }

        private void RewriteMvz()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(1);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(0);

            m.Assign(tmp, host.PseudoProcedure("__move_zones", dt, m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
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
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteSgr()
        {
            var src = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.ISub(dst, src));
        }

        private void RewriteSll()
        {
            var sh = (int)((AddressOperand)instr.Operands[1]).Address.ToLinear() & 0x3F;
            var dst = Reg(0);
            m.Assign(dst, m.Shl(dst, m.Int32(sh)));
            SetCc(m.Cond(dst));
        }

        private void RewriteSllg()
        {
            var sh = (int) ((AddressOperand) instr.Operands[2]).Address.ToLinear() & 0x3F;
            var src1 = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.Shl(src1, m.Int32(sh)));
        }

        private void RewriteSrag()
        {
            var sh = (int)((AddressOperand)instr.Operands[2]).Address.ToLinear() & 0x3F;
            var src1 = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.Sar(src1, m.Int32(sh)));
        }

        private void RewriteSrlg()
        {
            var sh = (int)((AddressOperand)instr.Operands[2]).Address.ToLinear() & 0x3F;
            var src1 = Reg(1);
            var dst = Reg(0);
            m.Assign(dst, m.Shr(src1, m.Int32(sh)));
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
