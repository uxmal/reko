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

using System;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.Xtensa
{
    public class XtensaInstruction : MachineInstruction
    {
        private static readonly Dictionary<Mnemonic, string> instrNames = new Dictionary<Mnemonic, string>
        {
            { Mnemonic.add_n, "add.n" },
            { Mnemonic.add_s, "add.s" },
            { Mnemonic.addi_n, "addi.n" },
            { Mnemonic.beqz_n, "beqz.n" },
            { Mnemonic.bnez_n, "bnez.n" },
            { Mnemonic.floor_s, "floor.s" },
            { Mnemonic.l32i_n, "l32i.n" },
            { Mnemonic.mov_n, "mov.n" },
            { Mnemonic.moveqz_s, "moveqz.s" },
            { Mnemonic.movi_n, "movi.n" },
            { Mnemonic.mul_s, "mul.s" },
            { Mnemonic.ret_n, "ret.n" },
            { Mnemonic.s32i_n, "s32i.n" },
            { Mnemonic.sub_s, "sub.s"  },
            { Mnemonic.ueq_s, "ueq.s" }
        };

        public Mnemonic Mnemonic { get; set; }

        public override int OpcodeAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!instrNames.TryGetValue(Mnemonic, out string instrName))
            {
                instrName = Mnemonic.ToString();
            }
            writer.WriteOpcode(instrName);
            RenderOperands(writer, options);
        }
    }
}