#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public class MultiRegisterOperand : MachineOperand
    {
        private ushort bitmask;

        public MultiRegisterOperand(PrimitiveType width, ushort bitmask) : base(width)
        {
            this.bitmask = bitmask;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var sep = "{";
            int mask = 1;
            int iStart = -1;
            for (int i = 0; i < 17; ++i, mask <<= 1)
            {
                if ((bitmask & mask) != 0)
                {
                    if (iStart < 0)
                    {
                        // Start a range
                        writer.WriteString(sep);
                        sep = ",";
                        iStart = i;
                        writer.WriteString(Registers.GpRegs[i].Name);
                    }
                }
                else
                {
                    if (0 <= iStart && iStart < i - 1)
                    {
                        writer.WriteChar('-');
                        writer.WriteString(Registers.GpRegs[i-1].Name);
                    }
                    iStart = -1;
                }
            }
            writer.WriteChar('}');
        }
    }
}
