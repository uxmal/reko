#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.M16C
{
    public class MultiRegisterOperand : AbstractMachineOperand
    {
        public MultiRegisterOperand(byte b, RegisterStorage[] regs, bool reverse)
             : base(PrimitiveType.Byte)
        {
            BitPattern = b;
            Regs = regs;
            Reverse = reverse;
        }

        public byte BitPattern { get; }
        public RegisterStorage[] Regs { get; }
        public bool Reverse { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sep = "";
            foreach (var reg in GetRegisters())
            {
                renderer.WriteString(sep);
                sep = ",";
                renderer.WriteString(reg.Name);
            }
        }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            if (Reverse)
            {
                var mask = 0x80;
                for (int i = 7; i >= 0; --i)
                {
                    if ((BitPattern & mask) != 0)
                        yield return Regs[i];
                    mask >>= 1;
                }
            }
            else
            {
                var mask = 0x1;
                for (int i = 7; i >= 0; --i)
                {
                    if ((BitPattern & mask) != 0)
                        yield return Regs[i];
                    mask <<= 1;
                }
            }
        }
    }
}
