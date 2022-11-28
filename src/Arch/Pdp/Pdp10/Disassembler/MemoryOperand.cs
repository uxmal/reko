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
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Pdp.Pdp10.Disassembler
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(RegisterStorage idxRegister, uint offset, bool indirect)
            : base(PdpTypes.Word36)
        {
            this.Index = idxRegister;
            this.Offset = offset;
            this.Indirect = indirect;
        }

        public MemoryOperand(uint offset, bool indirect)
            : base(PdpTypes.Word36)
        {
            this.Offset = offset;
            this.Indirect = indirect;
        }

        public RegisterStorage? Index { get; }

        public uint Offset { get; }

        public bool Indirect { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Indirect)
                renderer.WriteChar('@');
            renderer.WriteString(Convert.ToString(Offset, 8));
            if (Index is not null)
            {
                renderer.WriteFormat("({0})", Convert.ToString(Index.Number, 8));
            }
        }
    }
}