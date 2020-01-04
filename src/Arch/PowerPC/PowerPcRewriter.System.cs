#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    partial class PowerPcRewriter
    {
        private void RewriteDcbf()
        {
            m.SideEffect(host.PseudoProcedure("__dcbf", VoidType.Instance,
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteDcbi()
        {
            m.SideEffect(host.PseudoProcedure("__dcbi", VoidType.Instance,
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteDcbt()
        {
            // This is just a hint to the cache; makes no sense to have it in
            // high-level language. Consider adding option to have cache
            // hint instructions decompiled into intrinsics
        }

        private void RewriteDcbst()
        {
            m.SideEffect(host.PseudoProcedure("__dcbst", VoidType.Instance,
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteIcbi()
        {
            m.SideEffect(host.PseudoProcedure("__icbi", VoidType.Instance,
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteIsync()
        {
            m.SideEffect(host.PseudoProcedure("__isync", VoidType.Instance));
        }

        private void RewriteMfmsr()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            m.Assign(dst, host.PseudoProcedure("__read_msr", PrimitiveType.Word32));
        }

        private void RewriteMfspr()
        {
            var spr = RewriteOperand(instr.Operands[0]);
            var reg = RewriteOperand(instr.Operands[1]);
            m.Assign(
                reg, 
                host.PseudoProcedure("__read_spr", PrimitiveType.Word32, spr));
        }

        private void RewriteMtmsr(PrimitiveType dt)
        {
            var src = RewriteOperand(instr.Operands[0]);
            m.SideEffect(host.PseudoProcedure("__write_msr", VoidType.Instance, src));
        }

        private void RewriteMtspr()
        {
            var spr = RewriteOperand(instr.Operands[0]);
            var reg = RewriteOperand(instr.Operands[1]);
            m.SideEffect(host.PseudoProcedure("__write_spr", PrimitiveType.Word32, spr, reg));
        }

        private void RewriteRfi()
        {
            var srr0 = binder.EnsureRegister(arch.SpRegisters[26]);
            var srr1 = binder.EnsureRegister(arch.SpRegisters[27]);
            m.SideEffect(host.PseudoProcedure("__write_msr", PrimitiveType.Word32, srr1));
            m.Goto(srr0);
        }
    }
}
