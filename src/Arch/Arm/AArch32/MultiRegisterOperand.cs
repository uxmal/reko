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
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Arm.AArch32
{
    public class MultiRegisterOperand : AbstractMachineOperand
    {
        private readonly RegisterStorage[] registers;
        private readonly uint bitmask;
        private readonly int index;

        public MultiRegisterOperand(RegisterStorage[] registers, DataType width, uint bitmask, int index = -1) : base(width)
        {
            this.registers = registers;
            this.bitmask = bitmask;
            this.index = index;
        }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            int mask = 1;
            for (int i = 0; i < 32; ++i, mask <<= 1)
            {
                if ((bitmask & mask) != 0)
                    yield return registers[i];
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sep = "{";
            int mask = 1;
            int iStart = -1;
            for (int i = 0; i < 33; ++i, mask <<= 1)
            {
                if ((bitmask & mask) != 0)
                {
                    if (index >= 0)
                    {
                        renderer.WriteString(sep);
                        sep = ",";
                        renderer.WriteString(registers[i].Name);
                        renderer.WriteFormat("[{0}]", index);
                    }
                    else if (iStart < 0)
                    {
                        // Start a range
                        renderer.WriteString(sep);
                        sep = ",";
                        iStart = i;
                        renderer.WriteString(registers[i].Name);
                    }
                }
                else
                {
                    if (index < 0)
                    {
                        if (0 <= iStart && iStart < i - 1)
                        {
                            renderer.WriteChar('-');
                            renderer.WriteString(registers[i - 1].Name);
                        }
                        iStart = -1;
                    }
                }
            }
            renderer.WriteChar('}');
        }
    }
}
