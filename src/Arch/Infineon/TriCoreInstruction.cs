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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Infineon
{
    public class TriCoreInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString
        {
            get
            {
                if (mnemonics.TryGetValue(Mnemonic, out var mnemonic))
                    return mnemonic;
                else
                    return Mnemonic.ToString();
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(MnemonicAsString);
            base.RenderOperands(renderer, options);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case ImmediateOperand imm:
                renderer.WriteString("#0x");
                renderer.WriteString(imm.Value.ToUInt32().ToString("X"));
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }

        private static readonly Dictionary<Mnemonic, string> mnemonics = new()
        {
            { Mnemonic.abs_b, "abs.b" },
            { Mnemonic.abs_h, "abs.h" },
            { Mnemonic.absdif_b, "absdif.b" },
            { Mnemonic.absdif_h, "absdif.h" },
            { Mnemonic.abss_h, "abss.h" },
            { Mnemonic.add_a, "add.a" },
            { Mnemonic.add_b, "add.b" },
            { Mnemonic.add_h, "add.h" },
            { Mnemonic.addih_a, "addih.a" },
            { Mnemonic.adds_h, "adds.h" },
            { Mnemonic.adds_hu, "adds.hu" },
            { Mnemonic.adds_u, "adds.u" },
            { Mnemonic.addsc_a, "addsc.a" },
            { Mnemonic.and_and_t, "and.and.t" },
            { Mnemonic.and_eq, "and.eq" },
            { Mnemonic.and_ge, "and.ge" },
            { Mnemonic.and_ge_u, "and.ge.u" },
            { Mnemonic.and_lt, "and.lt" },
            { Mnemonic.and_lt_u, "and.lt.u" },
            { Mnemonic.and_ne, "and.ne" },
            { Mnemonic.cachea_i, "cachea.i" },
            { Mnemonic.cmpswap_w, "cmpswap.w" },
            { Mnemonic.div_u, "div.u" },
            { Mnemonic.eq_a, "eq.a" },
            { Mnemonic.eq_b, "eq.b" },
            { Mnemonic.eq_h, "eq.h" },
            { Mnemonic.eq_w, "eq.w" },
            { Mnemonic.eqany_b, "eqany.b" },
            { Mnemonic.eqany_h, "eqany.h" },
            { Mnemonic.eqz_a, "eqz.a" },
            { Mnemonic.extr_u, "extr.u" },
            { Mnemonic.ge_a, "ge.a" },
            { Mnemonic.ge_u, "ge.u" },
            { Mnemonic.jeq_a, "jeq.a" },
            { Mnemonic.jge_u, "jge.u" },
            { Mnemonic.jlt_u, "jlt.u" },
            { Mnemonic.jne_a, "jne.a" },
            { Mnemonic.jnz_a, "jnz.a" },
            { Mnemonic.jnz_t, "jnz.t" },
            { Mnemonic.jz_a, "jz.a" },
            { Mnemonic.jz_t, "jz.t" },
            { Mnemonic.ld_a, "ld.a" },
            { Mnemonic.ld_b, "ld.b" },
            { Mnemonic.ld_bu, "ld.bu" },
            { Mnemonic.ld_d, "ld.d" },
            { Mnemonic.ld_h, "ld.h" },
            { Mnemonic.ld_hu, "ld.hu" },
            { Mnemonic.ld_w, "ld.w" },
            { Mnemonic.lt_a, "lt.a" },
            { Mnemonic.lt_b, "lt.b" },
            { Mnemonic.lt_bu, "lt.bu" },
            { Mnemonic.lt_h, "lt.h" },
            { Mnemonic.lt_hu, "lt.hu" },
            { Mnemonic.lt_u, "lt.u" },
            { Mnemonic.lt_w, "lt.w" },
            { Mnemonic.lt_wu, "lt.wu" },
            { Mnemonic.madd_u, "madd.u" },
            { Mnemonic.max_b, "max.b" },
            { Mnemonic.max_bu, "max.bu" },
            { Mnemonic.max_h, "max.h" },
            { Mnemonic.max_hu, "max.hu" },
            { Mnemonic.max_u, "max.u" },
            { Mnemonic.madds_u, "madds.u" },
            { Mnemonic.min_b, "min.b" },
            { Mnemonic.min_bu, "min.bu" },
            { Mnemonic.min_h, "min.h" },
            { Mnemonic.min_hu, "min.hu" },
            { Mnemonic.min_u, "min.u" },
            { Mnemonic.mov_a, "mov.a" },
            { Mnemonic.mov_aa, "mov.aa" },
            { Mnemonic.mov_d, "mov.d" },
            { Mnemonic.mov_u, "mov.u" },
            { Mnemonic.movh_a, "movh.a" },
            { Mnemonic.mul_u, "mul.u" },
            { Mnemonic.nand_t, "nand.t" },
            { Mnemonic.ne_a, "ne.a" },
            { Mnemonic.nez_a, "nez.a" },
            { Mnemonic.or_eq, "or.eq" },
            { Mnemonic.or_ge, "or.ge" },
            { Mnemonic.or_ge_u, "or.ge.u" },
            { Mnemonic.or_lt, "or.lt" },
            { Mnemonic.or_lt_u, "or.lt.u" },
            { Mnemonic.or_ne, "or.ne" },
            { Mnemonic.or_t, "or.t" },
            { Mnemonic.orn_t, "orn.t" },
            { Mnemonic.rsubs_u, "rsubs.u" },
            { Mnemonic.sat_b, "sat.b" },
            { Mnemonic.sat_bu, "sat.bu" },
            { Mnemonic.sat_h, "sat.h" },
            { Mnemonic.sat_hu, "sat.hu" },
            { Mnemonic.sh_and_t, "sh.and.t" },
            { Mnemonic.sh_eq, "sh.eq" },
            { Mnemonic.sh_ge, "sh.ge" },
            { Mnemonic.sh_ge_u, "sh.ge.u" },
            { Mnemonic.sh_h, "sh.h" },
            { Mnemonic.sh_lt, "sh.lt" },
            { Mnemonic.sh_lt_u, "sh.lt.u" },
            { Mnemonic.sh_ne, "sh.ne" },
            { Mnemonic.sha_h, "sha.h" },
            { Mnemonic.st_a, "st.a" },
            { Mnemonic.st_b, "st.b" },
            { Mnemonic.st_da, "st.da" },
            { Mnemonic.st_h, "st.h" },
            { Mnemonic.st_w, "st.w" },
            { Mnemonic.sub_a, "sub.a" },
            { Mnemonic.sub_b, "sub.b" },
            { Mnemonic.sub_h, "sub.h" },
            { Mnemonic.subs_h, "subs.h" },
            { Mnemonic.subs_hu, "subs.hu" },
            { Mnemonic.subs_u, "subs.u" },
            { Mnemonic.swap_w, "swap.w"},
            { Mnemonic.swapmsk_w, "swapmsk.w"},
            { Mnemonic.xnor_t, "xor.t" },
            { Mnemonic.xor_ge, "xor.ge" },
            { Mnemonic.xor_ge_u, "xor.ge.u" },
            { Mnemonic.xor_lt, "xor.lt" },
            { Mnemonic.xor_lt_u, "xor.lt.u" },
            { Mnemonic.xor_ne, "xor.ne" },
            { Mnemonic.xor_t, "xor.t" },
        };
    }
}
