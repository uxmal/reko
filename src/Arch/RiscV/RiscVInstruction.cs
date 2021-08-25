#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Machine;

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.RiscV
{
    public class RiscVInstruction : MachineInstruction
    {
        private static readonly Dictionary<Mnemonic, string> mnemonicNames;
         
        public Mnemonic Mnemonic { get; set; }

        static RiscVInstruction()
        {
            mnemonicNames = new Dictionary<Mnemonic, string>
            {
                { Mnemonic.c_add, "c.add" },
                { Mnemonic.c_addi, "c.addi" },
                { Mnemonic.c_addiw, "c.addiw" },
                { Mnemonic.c_addw, "c.addw" },
                { Mnemonic.c_and, "c.and" },
                { Mnemonic.c_andi, "c.andi" },
                { Mnemonic.c_beqz,  "c.beqz" },
                { Mnemonic.c_bnez,  "c.bnez" },
                { Mnemonic.c_addi16sp, "c.addi16sp" },
                { Mnemonic.c_addi4spn, "c.addi4spn" },
                { Mnemonic.c_fld, "c.fld" },
                { Mnemonic.c_fldsp, "c.fldsp" },
                { Mnemonic.c_flw, "c.flw" },
                { Mnemonic.c_flwsp, "c.flwsp" },
                { Mnemonic.c_fsd, "c.fsd" },
                { Mnemonic.c_fsdsp, "c.fsdsp" },
                { Mnemonic.c_j, "c.j" },
                { Mnemonic.c_jr, "c.jr" },
                { Mnemonic.c_jalr, "c.jalr" },
                { Mnemonic.c_ld, "c.ld" },
                { Mnemonic.c_ldsp, "c.ldsp" },
                { Mnemonic.c_lwsp, "c.lwsp" },
                { Mnemonic.c_li, "c.li" },
                { Mnemonic.c_lui, "c.lui" },
                { Mnemonic.c_lw, "c.lw" },
                { Mnemonic.c_mv, "c.mv" },
                { Mnemonic.c_or, "c.or" },
                { Mnemonic.c_sdsp, "c.sdsp" },
                { Mnemonic.c_swsp, "c.swsp" },
                { Mnemonic.c_slli, "c.slli" },
                { Mnemonic.c_sd, "c.sd" },
                { Mnemonic.c_srai, "c.srai" },
                { Mnemonic.c_srli, "c.srli" },
                { Mnemonic.c_sub, "c.sub" },
                { Mnemonic.c_subw, "c.subw" },
                { Mnemonic.c_sw, "c.sw" },
                { Mnemonic.c_xor, "c.xor" },
                { Mnemonic.fadd_s, "fadd.s" },
                { Mnemonic.fadd_d, "fadd.d" },
                { Mnemonic.fcvt_d_s, "fcvt.d.s" },
                { Mnemonic.fcvt_s_d, "fcvt.s.d" },
                { Mnemonic.feq_s, "feq.s" },
                { Mnemonic.fmadd_s, "fmadd.s" },
                { Mnemonic.fmsub_s,  "fmsub.s" },
                { Mnemonic.fnmsub_s, "fnmsub.s" },
                { Mnemonic.fnmadd_s, "fnmadd.s" },
                { Mnemonic.fmv_d_x, "fmv.d.x" },
            };
        }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        public bool Acquire { get; set; }
        public bool Release { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder();
            if (mnemonicNames.TryGetValue(Mnemonic, out string name))
            {
                sb.Append(name);
            }
            else
            {
                sb.Append(Mnemonic.ToString());
                sb.Replace('_', '.');
            }
            if (Acquire)
                sb.Append(".aq");
            if (Release)
                sb.Append(".rl");
            renderer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case RegisterStorage rop:
                renderer.WriteString(rop.Name);
                return;
            case ImmediateOperand immop:
                immop.Render(renderer, options);
                return;
            case AddressOperand addrop:
                //$TODO: 32-bit?
                if (addrop.Width.BitSize == 32)
                {
                    renderer.WriteAddress(string.Format("{0:X8}", addrop.Address.ToLinear()), addrop.Address);
                } else {
                    renderer.WriteAddress(string.Format("{0:X16}", addrop.Address.ToLinear()), addrop.Address);
                }
                    return;
            case MemoryOperand memop:
                memop.Render(renderer, options);
                return;
            }
            throw new NotImplementedException($"Risc-V operand type {op.GetType().Name} not implemented yet.");
        }
    }
}