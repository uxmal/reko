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
            var reg = Reg(instr.Operands[0]);
            Expression dst;
            Expression src;
            if (reg.DataType.BitSize > dt.BitSize)
            {
                src = m.Cast(dt, reg);
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
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.IAdd(dst, src));
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteAr()
        {
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            var dt = PrimitiveType.Word32;
            var tmp = binder.CreateTemporary(dt);
            var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(src.DataType.BitSize - dt.BitSize));
            m.Assign(tmp, m.IAdd(m.Cast(dt,dst), m.Cast(dt,src)));
            m.Assign(tmpHi, m.Slice(tmpHi.DataType, dst, dt.BitSize));
            m.Assign(dst, m.Seq(tmpHi, tmp));
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(tmp));
        }

        private void RewriteChi()
        {
            var left = Reg(instr.Operands[0]);
            var imm = Const(instr.Operands[1]).ToInt16();
            var right = Constant.Create(left.DataType, imm);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClc()
        {
            var mem = (MemoryOperand)instr.Operands[0];
            var dt = PrimitiveType.CreateWord(mem.Length);
            var left = m.Mem(dt, EffectiveAddress(mem));
            var right = m.Mem(dt, EffectiveAddress(instr.Operands[1]));
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClg()
        {
            var reg = Reg(instr.Operands[0]);
            var ea = EffectiveAddress(instr.Operands[1]);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(reg, m.Mem8(ea))));
        }

        private void RewriteCli()
        {
            var ea = EffectiveAddress(instr.Operands[0]);
            var imm = Const(instr.Operands[1]);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(m.ISub(m.Mem8(ea), imm)));
        }

        private void RewriteLa()
        {
            var ea = EffectiveAddress(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, ea);
        }

        private void RewriteLarl()
        {
            Expression src = Addr(instr.Operands[1]);
            Identifier dst = Reg(instr.Operands[0]);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                src = m.Dpb(dst, src, 0);
            }
            m.Assign(dst, src);
        }

        private void RewriteL(PrimitiveType pt)
        {
            var ea = EffectiveAddress(instr.Operands[1]);
            var src = m.Mem(pt, ea);
            var dst = Reg(instr.Operands[0]);
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

        private void RewriteLgf()
        {
            var src = m.Mem(PrimitiveType.Int32, EffectiveAddress(instr.Operands[1]));
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.Cast(PrimitiveType.Int64, src));
        }

        private void RewriteLgfr()
        {
            var src = Reg(instr.Operands[1]);
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, m.Cast(tmp.DataType, src));
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.Cast(PrimitiveType.Int64, tmp));
        }

        private void RewriteLghi()
        {
            var imm = Const(instr.Operands[1]).ToInt16();
            var dst = Reg(instr.Operands[0]);
            var src = Constant.Create(dst.DataType, imm);
            m.Assign(dst, src);
        }

        private void RewriteLhi()
        {
            int imm = Const(instr.Operands[1]).ToInt16();
            var dst = Reg(instr.Operands[0]);
            var src = Constant.Create(PrimitiveType.Word32, imm);
            m.Assign(dst, src);
        }

        private void RewriteLgr()
        {
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteLmg()
        {
            var rStart = ((RegisterOperand)instr.Operands[0]).Register;
            var rEnd = ((RegisterOperand)instr.Operands[1]).Register;
            var ea = EffectiveAddress(instr.Operands[2]);
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
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
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

        private void RewriteLtgr()
        {
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, src);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteMvi()
        {
            var src = Const(instr.Operands[1]);
            var ea = EffectiveAddress(instr.Operands[0]);
            m.Assign(m.Mem8(ea), src);
        }

        private void RewriteMvz()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(instr.Operands[1]);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(instr.Operands[0]);

            m.Assign(tmp, host.PseudoProcedure("__move_zones", dt, m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
            m.Assign(m.Mem(dt, eaDst), tmp);
        }

        private void RewriteNc()
        {
            var len = ((MemoryOperand)instr.Operands[0]).Length;
            var dt = PrimitiveType.CreateWord(len);
            var eaSrc = EffectiveAddress(instr.Operands[1]);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(instr.Operands[0]);

            m.Assign(tmp, m.And(m.Mem(dt, eaDst), m.Mem(dt, eaSrc)));
            m.Assign(m.Mem(dt, eaDst), tmp);
            
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(tmp));
        }

        private void RewriteNgr()
        {
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.And(dst, src));
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(dst));
        }

        private void RewriteSgr()
        {
            var src = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.ISub(dst, src));
        }

        private void RewriteSrag()
        {
            var sh = (int)((AddressOperand)instr.Operands[2]).Address.ToLinear() & 0x3F;
            var src1 = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.Sar(src1, m.Int32(sh)));
        }

        private void RewriteSrlg()
        {
            var sh = (int)((AddressOperand)instr.Operands[2]).Address.ToLinear() & 0x3F;
            var src1 = Reg(instr.Operands[1]);
            var dst = Reg(instr.Operands[0]);
            m.Assign(dst, m.Shr(src1, m.Int32(sh)));
        }

        private void RewriteSt(PrimitiveType dt)
        {
            Expression src = Reg(instr.Operands[0]);
            if (dt.BitSize < 64)
            {
                src = m.Cast(dt, src);
            }
            var ea = EffectiveAddress(instr.Operands[1]);
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteStmg()
        {
            var rStart = ((RegisterOperand)instr.Operands[0]).Register;
            var rEnd = ((RegisterOperand)instr.Operands[1]).Register;
            var ea = EffectiveAddress(instr.Operands[2]);
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
            var eaSrc = EffectiveAddress(instr.Operands[1]);
            var tmp = binder.CreateTemporary(dt);
            var eaDst = EffectiveAddress(instr.Operands[0]);

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
