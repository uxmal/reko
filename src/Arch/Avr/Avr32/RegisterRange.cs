#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.Avr.Avr32
{
    public class RegisterRange : AbstractMachineOperand
    {
        public RegisterStorage[] Registers { get; }
        public int RegisterIndex { get; }
        public int Count { get; } 

        public RegisterRange(RegisterStorage[] gpRegisters, int iRegStart, int cRegs)
            : base(PrimitiveType.Word32)    // Don't care.
        {
            this.Registers = gpRegisters;
            this.RegisterIndex = iRegStart;
            this.Count = cRegs;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(Registers[RegisterIndex].Name);
            if (Count <= 1)
                return;
            if (Count <= 2)
            {
                renderer.WriteString(",");
            }
            else
            {
                renderer.WriteString("-");
            }
            renderer.WriteString(Registers[RegisterIndex + Count - 1].Name);
        }

        public IEnumerable<RegisterStorage> Enumerate()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return Registers[RegisterIndex + i];
            }
        }
    }
}