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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Mips
{
    public class MultiRegisterOperand : MachineOperand
    {
        private readonly RegisterStorage[] registers;

        public MultiRegisterOperand(RegisterStorage[] registers, PrimitiveType width, uint bitmask) : base(width)
        {
            this.registers = registers;
            this.Bitmask = bitmask;
        }

        public uint Bitmask { get; }

        private IEnumerable<RegisterStorage> GetRegisters(uint bitmask)
        {
            int mask = 1;
            for (int i = 0; i < 32; ++i, mask <<= 1)
            {
                if ((bitmask & mask) != 0)
                    yield return registers[i];
            }
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var sep = "";
            if (Bits.IsBitSet(Bitmask, 0x1F)) // ra
            {
                writer.WriteString(registers[0x1F].Name);
                sep = ",";
            }
            var bm = Bitmask & ~(1u << 0x1F);
            foreach (var reg in GetRegisters(bm))
            {
                writer.WriteString(sep);
                sep = ",";
                writer.WriteString(reg.Name);
            }
        }
    }
}
