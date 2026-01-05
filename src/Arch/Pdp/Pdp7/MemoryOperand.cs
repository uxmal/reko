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
using System;

namespace Reko.Arch.Pdp.Pdp7
{
    internal class MemoryOperand : AbstractMachineOperand
    {
        public uint Address { get; set; }
        public bool Deferred { get; set; }

        public MemoryOperand(PrimitiveType dt, uint v, bool isDeferred) : base(dt)
        {
            this.Address = v;
            this.Deferred = isDeferred;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Deferred)
            {
                renderer.WriteString("i ");
            }
            renderer.WriteAddress(Convert.ToString(Address, 8), Address);
        }
    }
}