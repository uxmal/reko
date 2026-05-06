#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.X86.Rewriter
{
    /// <summary>
    /// Rewriter methods for NEC V20/V30-specific instructions.
    /// </summary>
    public partial class X86Rewriter
    {
        // ----------------------------------------------------------------
        // Bit-manipulation instructions: TEST1, CLR1, SET1, NOT1
        //
        //   test1 r/m, CL|imm  — test bit N of r/m; result in flags
        //   clr1  r/m, CL|imm  — clear bit N of r/m
        //   set1  r/m, CL|imm  — set bit N of r/m
        //   not1  r/m, CL|imm  — complement bit N of r/m
        //
        // All follow the same pattern; the specific operation is passed in
        // via the intrinsic argument.
        // ----------------------------------------------------------------

        private void RewriteTest1()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(arch.Registers.C),
                m.Fn(test1_intrinsic.MakeInstance(src0.DataType), src0, src1));
        }

        private void RewriteBitManip1(IntrinsicProcedure intrinsic)
        {
            var dst = SrcOp(0);
            var bitIdx = SrcOp(1);
            m.Assign(
                binder.EnsureFlagGroup(arch.Registers.C),
                m.Fn(intrinsic.MakeInstance(dst.DataType), dst, bitIdx,
                    m.Out(dst.DataType, SrcOp(0))));
        }

        // ----------------------------------------------------------------
        // Nybble rotate instructions: ROL4, ROR4
        //
        //   rol4 r/m8  — AL:[r/m8] nybble pair rotate left
        //   ror4 r/m8  — AL:[r/m8] nybble pair rotate right
        // ----------------------------------------------------------------

        private void RewriteRol4()
        {
            var op = SrcOp(0);
            var al = binder.EnsureRegister(Registers.al);
            m.Assign(al, m.Fn(rol4_intrinsic, al, op, m.Out(PrimitiveType.Byte, op)));
        }

        private void RewriteRor4()
        {
            var op = SrcOp(0);
            var al = binder.EnsureRegister(Registers.al);
            m.Assign(al, m.Fn(ror4_intrinsic, al, op, m.Out(PrimitiveType.Byte, op)));
        }

        // ----------------------------------------------------------------
        // 4-bit BCD string instructions: ADD4S, SUB4S, CMP4S
        //
        //   add4s  — DS:SI += ES:DI (BCD nybble strings, length in CX)
        //   sub4s  — DS:SI -= ES:DI
        //   cmp4s  — compare BCD nybble strings
        //
        // All use implicit pointer operands; we model them as side-effect
        // intrinsics.
        // ----------------------------------------------------------------

        private void RewriteStringBcd(IntrinsicProcedure intrinsic)
        {
            var cx = orw.AluRegister(Registers.cx);
            var si = orw.AluRegister(Registers.si);
            var di = orw.AluRegister(Registers.di);
            m.SideEffect(m.Fn(intrinsic, cx, si, di));
        }

        // ----------------------------------------------------------------
        // BRKEM ib — break to emulator
        // Modelled as an indirect call to the emulator via the imm8 vector.
        // ----------------------------------------------------------------

        private void RewriteBrkem()
        {
            var vector = SrcOp(0);
            m.SideEffect(m.Fn(brkem_intrinsic, vector));
        }

        // ----------------------------------------------------------------
        // Static fields: intrinsic procedure objects
        // ----------------------------------------------------------------

        private static readonly IntrinsicProcedure add4s_intrinsic;
        private static readonly IntrinsicProcedure brkem_intrinsic;
        private static readonly IntrinsicProcedure clr1_intrinsic;
        private static readonly IntrinsicProcedure cmp4s_intrinsic;
        private static readonly IntrinsicProcedure not1_intrinsic;
        private static readonly IntrinsicProcedure rol4_intrinsic;
        private static readonly IntrinsicProcedure ror4_intrinsic;
        private static readonly IntrinsicProcedure set1_intrinsic;
        private static readonly IntrinsicProcedure sub4s_intrinsic;
        private static readonly IntrinsicProcedure test1_intrinsic;
    }
}
