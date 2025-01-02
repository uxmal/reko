#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.M68k.Rewriter
{
    public partial class M68kRewriter
    {
        private static readonly StringType labelType = StringType.NullTerminated(PrimitiveType.Byte);

        private void RewriteBkpt()
        {
            iclass = InstrClass.Invalid;
            var src = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            m.SideEffect(m.Fn(bkpt_intrinsic, src));
        }
        
        private void RewriteCinva()
        {
            m.SideEffect(m.Fn(cinva_intrinsic, Constant.String(instr.Operands[0].ToString()!, labelType)));
        }

        private void RewriteCinvp()
        {
            var src = (MemoryAccess) this.orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.SideEffect(m.Fn(
                cinvp_intrinsic,
                Constant.String(instr.Operands[0].ToString()!, labelType),
                src.EffectiveAddress));
        }

        private void RewriteCinvl()
        {
            var src = (MemoryAccess) this.orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.SideEffect(m.Fn(
                cinvl_intrinsic,
                Constant.String(instr.Operands[0].ToString()!, labelType),
                src.EffectiveAddress));
        }

        private void RewriteMovec()
        {
            var src = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, instr.DataWidth!, src, (s, d) =>
                m.Fn(movec_intrinsic, s));
        }

        private void RewriteMoves()
        {
            var src = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, instr.DataWidth!, src, (s, d) =>
                m.Fn(moves_intrinsic, s));
        }

        private void RewritePload()
        {
            var src1 = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            var src2 = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            m.SideEffect(m.Fn(pload_intrinsic, src1, src2));
        }

        private void RewritePflushr()
        {
            var src = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            m.SideEffect(m.Fn(pflushr_intrinsic, src));
        }

        private void RewritePtest()
        {
            var src1 = this.orw.RewriteSrc(instr.Operands[0], instr.Address);
            var src2 = ((MemoryAccess) this.orw.RewriteSrc(instr.Operands[1], instr.Address)).EffectiveAddress;
            m.SideEffect(m.Fn(ptest_intrinsic, src2, src1));
        }

        private void RewriteReset()
        {
            m.SideEffect(m.Fn(reset_intrinsic));
        }

        private void RewriteRte()
        {
            var sp = binder.EnsureRegister(Registers.a7);
            var sr = binder.EnsureRegister(Registers.sr);
            m.Assign(sr, m.Mem16(sp));
            m.Assign(sp, m.IAddS(sp, 2));
            m.Return(4, 0);
        }

        private void RewriteTas()
        {
            orw.DataWidth = PrimitiveType.Byte;
            var tmp = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var zn = binder.EnsureFlagGroup(Registers.ZN);
            m.Assign(zn, m.Fn(tas_instrinsic, tmp, m.Out(orw.DataWidth, tmp)));
        }
    }
}
