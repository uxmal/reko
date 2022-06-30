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
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public class RegisterListOperand : AbstractMachineOperand
    {
        public RegisterListOperand(int bitmask) : base(PrimitiveType.Byte)
        {
            this.Bitmask = bitmask;
        }

        public int Bitmask { get; }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            for (int i = 0, mask = 1; i < 8; i++, mask <<= 1)
            {
                if ((Bitmask & mask) != 0)
                    yield return Registers.rw[i];
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sep = '(';
            foreach (var reg in GetRegisters())
            {
                renderer.WriteChar(sep);
                    sep = ',';
                renderer.WriteString(reg.Name);
            }
            renderer.WriteChar(')');
        }
    }
}