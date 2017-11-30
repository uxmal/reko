#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Arch.Arm
{
    public class RegisterRangeOperand : MachineOperand
    {
        private uint instr;

        public RegisterRangeOperand(uint instr): base(PrimitiveType.Word32)
        {
            this.instr = instr;
        }

        public IEnumerable<int> GetRegisters()
        {
            uint m = 1u;
            for (int i = 0; i < 16; ++i, m <<= 1)
            {
                if ((instr & m) != 0)
                    yield return i;
            }
        }

        public IEnumerable<int> GetRegistersInverted()
        {
            uint m = 0x8000u;
            for (int i = 15; i >= 0; --i, m >>= 1)
            {
                if ((instr & m) != 0)
                    yield return i;
            }
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteChar('{');
            {
                uint w = instr & 0xFFFF;
                int i = 0;
                while (w != 0)
                {
                    int j;
                    while ((w & (1u << i)) == 0)
                        ++i;
                    for (j = i + 1; (w & (1u << j)) != 0; ++j)
                        ;
                    --j;
                    // registers [i..j] 
                    writer.WriteString(A32Registers.GpRegs[i].Name);
                    if (j - i != 0)
                    {
                        writer.WriteChar((j - i > 1) ? '-' : ',');
                        writer.WriteString(A32Registers.GpRegs[j].Name);
                    }
                    i = j; w = (w >> (j + 1)) << (j + 1);
                    if (w != 0) writer.WriteChar(',');
                }
            }
            writer.WriteChar('}');
            if ((instr & (1 << 22)) != 0) writer.WriteChar('^');
        }
    }
}
