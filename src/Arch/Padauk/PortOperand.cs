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

using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Padauk
{
    public class PortOperand : AbstractMachineOperand
    {

        private PortOperand(DataType dt, uint port, int bit)
            : base(dt)
        {
            this.Port = port;
            this.Bit = bit < 0 ? null : bit;
        }

        public static PortOperand Create(uint port)
        {
            return new PortOperand(PrimitiveType.Byte, port, -1);
        }

        public static PortOperand CreateBitAccess(uint port, int bit)
        {
            return new PortOperand(PrimitiveType.Bool, port, bit);
        }

        public uint Port { get; }
        public int? Bit { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteFormat("IO(0x{0:X})", Port);
            if (Bit.HasValue)
            {
                renderer.WriteFormat(".{0}", Bit.Value);
            }
        }
    }
}