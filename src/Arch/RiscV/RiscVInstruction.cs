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
using Reko.Core.Machine;

using System;
using System.Collections.Generic;

namespace Reko.Arch.RiscV
{
    public class RiscVInstruction : MachineInstruction
    {
        private static Dictionary<Mnemonic, string> opcodeNames;

        public Mnemonic Mnemonic;

        static RiscVInstruction()
        {
            opcodeNames = new Dictionary<Mnemonic, string>
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

        public override int OpcodeAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RenderMnemonic(writer);
            RenderOperands(writer, options);
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            if (!opcodeNames.TryGetValue(Mnemonic, out string name))
            {
                name = Mnemonic.ToString();
                name = name.Replace('_', '.');
            }
            writer.WriteOpcode(name);
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case RegisterOperand rop:
                writer.WriteString(rop.Register.Name);
                return;
            case ImmediateOperand immop:
                immop.Write(writer, options);
                return;
            case AddressOperand addrop:
                //$TODO: 32-bit?
                writer.WriteAddress(string.Format("{0:X16}", addrop.Address.ToLinear()), addrop.Address);
                return;
            case MemoryOperand memop:
                memop.Write(writer, options);
                return;
            }
            throw new NotImplementedException($"Risc-V operand type {op.GetType().Name} not implemented yet.");
        }
    }
}