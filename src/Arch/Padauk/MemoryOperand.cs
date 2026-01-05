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

using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Padauk
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        public uint Offset { get; set; }
        public int? Bit { get; set; }
        public bool Indirect { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var format = Indirect ? "[[0x{0:X}]]" : "[0x{0:X}]";
            renderer.WriteFormat(format, Offset);
            if (Bit.HasValue)
            {
                renderer.WriteFormat(".{0}", Bit.Value);
            }
        }
    }
}