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

using Reko.Core;
using Reko.Core.Expressions;
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
            m.SideEffect(m.Fn(
                dcbf.MakeInstance(arch.PointerType),
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteDcbi()
        {
            m.SideEffect(m.Fn(
                dcbi.MakeInstance(arch.PointerType),
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
            m.SideEffect(m.Fn(
                dcbst.MakeInstance(arch.PointerType),
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteIcbi()
        {
            m.SideEffect(m.Fn(
                icbi.MakeInstance(arch.PointerType),
                EffectiveAddress_r0(instr.Operands[0], instr.Operands[1])));
        }

        private void RewriteIsync()
        {
            m.SideEffect(m.Fn(isync));
        }

        private void RewriteMfmsr()
        {
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Fn(mfmsr.MakeInstance(dst.DataType)));
        }

        private void RewriteMfspr()
        {
            var reg = RewriteOperand(1);
            var spr = RewriteOperand(0);
            if (spr is Identifier id)
            {
                m.Assign(reg, id);
            }
            else
            {
                m.Assign(
                    reg,
                    m.Fn(mfspr.MakeInstance(reg.DataType), spr));
            }
        }

        private void RewriteMtmsr(PrimitiveType dt)
        {
            var src = RewriteOperand(0);
            m.SideEffect(m.Fn(mtmsr.MakeInstance(dt), src));
        }

        private void RewriteMtspr()
        {
            var reg = RewriteOperand(1);
            var spr = RewriteOperand(0);
            if (spr is Identifier id)
            {
                m.Assign(id, reg);
            }
            else
            {
                m.SideEffect(m.Fn(mtspr.MakeInstance(reg.DataType), spr, reg)); // host.Intrinsic("__write_spr", true, PrimitiveType.Word32, spr, reg));
            }
        }

        private void RewriteRfi()
        {
            var srr0 = binder.EnsureRegister(arch.SpRegisters[26]);
            var srr1 = binder.EnsureRegister(arch.SpRegisters[27]);
            m.SideEffect(m.Fn(mtmsr.MakeInstance(srr1.DataType), srr1)); // host.Intrinsic("__write_msr", true, PrimitiveType.Word32, srr1));
            m.Goto(srr0);
        }

        private void RewriteTlbie()
        {
            var src = RewriteOperand(0);
            m.SideEffect(m.Fn(tlbie.MakeInstance(arch.PointerType), src));
        }

        private void RewriteTlbsync()
        {
            m.SideEffect(m.Fn(tlbsync));
        }
    }
}
