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

namespace Reko.Arch.MN103
{
    public class MultipleRegistersOperand : AbstractMachineOperand
    {
        public MultipleRegistersOperand(byte encoding)
            : base(PrimitiveType.Bool)
        {
            this.Encoding = encoding;
        }

        public byte Encoding { get; }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            if ((this.Encoding & 0x80) != 0)
                yield return Registers.DRegs[2];
            if ((this.Encoding & 0x40) != 0)
                yield return Registers.DRegs[3];
            if ((this.Encoding & 0x20) != 0)
                yield return Registers.ARegs[2];
            if ((this.Encoding & 0x10) != 0)
                yield return Registers.ARegs[3];
            if ((this.Encoding & 0x8) != 0)
            {
                yield return Registers.DRegs[0];
                yield return Registers.DRegs[1];
                yield return Registers.ARegs[0];
                yield return Registers.ARegs[1];
                yield return Registers.mdr;
                yield return Registers.lir;
                yield return Registers.lar;
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('[');
            var sep = "";
            foreach (var reg in GetRegisters())
            {
                renderer.WriteString(sep);
                renderer.WriteString(reg.Name);
                sep = ",";
            }
            renderer.WriteChar(']');
        }
    }
}