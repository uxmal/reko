#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.Xtensa
{
    public class XtensaInstruction : MachineInstruction
    {
        private static readonly Dictionary<Opcodes, string> instrNames = new Dictionary<Opcodes, string>
        {
            { Opcodes.add_n, "add.n" },
            { Opcodes.add_s, "add.s" },
            { Opcodes.addi_n, "addi.n" },
            { Opcodes.beqz_n, "beqz.n" },
            { Opcodes.bnez_n, "bnez.n" },
            { Opcodes.floor_s, "floor.s" },
            { Opcodes.l32i_n, "l32i.n" },
            { Opcodes.mov_n, "mov.n" },
            { Opcodes.moveqz_s, "moveqz.s" },
            { Opcodes.movi_n, "movi.n" },
            { Opcodes.mul_s, "mul.s" },
            { Opcodes.ret_n, "ret.n" },
            { Opcodes.s32i_n, "s32i.n" },
            { Opcodes.sub_s, "sub.s"  },
            { Opcodes.ueq_s, "ueq.s" }
        };

        public Opcodes Opcode { get; set; }
        public MachineOperand[] Operands { get; internal set; }

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string instrName;
            if (!instrNames.TryGetValue(Opcode, out instrName))
            {
                instrName = Opcode.ToString();
            }
            writer.WriteOpcode(instrName);
            writer.Tab();
            var sep = "";
            if (this.Operands != null)
            {
                foreach (var op in this.Operands)
                {
                    writer.WriteString(sep);
                    op.Write(writer, options);
                    sep = ",";
                }
            }
        }
    }
}