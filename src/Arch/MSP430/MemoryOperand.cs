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
using Reko.Core;
using System;
using Reko.Core.Types;

namespace Reko.Arch.Msp430
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Base;
        public short Offset;
        public bool PostIncrement;

        public MemoryOperand(PrimitiveType dataWidth) : base(dataWidth) { }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Base is null)
            {
                renderer.WriteFormat("&{0:X4}", (ushort) this.Offset);
                return;
            }
            if (Offset > 0)
            {
                if (IsPcRelative && (options.Flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
                {
                    var addr = renderer.Address + 4 + Offset;
                    renderer.WriteAddress(addr.ToString(), addr);
                }
                else
                {
                    renderer.WriteFormat("{0:X4}({1})", Offset, Base.Name);
                }
            }
            else if (Offset < 0)
            {
                if (IsPcRelative && (options.Flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
                {
                    var addr = renderer.Address + 4 + Offset;
                    renderer.WriteAddress(addr.ToString(), addr);
                }
                else
                {
                    renderer.WriteFormat("-{0:X4}({1})", -Offset, Base.Name);
                }
            }
            else
            {
                renderer.WriteChar('@');
                renderer.WriteString(Base.Name);
                if (PostIncrement)
                    renderer.WriteChar('+');
            }
        }

        public bool IsPcRelative => this.Base is not null && this.Base.Number == 0;
    }
}