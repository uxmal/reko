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
using System;

namespace Reko.Arch.WE32100
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(PrimitiveType width) : base(width)
        {
        }

        public RegisterStorage? Base { get; set; }
        public int Offset { get; set; }
        public bool Deferred { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Deferred)
                renderer.WriteChar('*');
            if (Base is null)
            {
                renderer.WriteChar('$');
                renderer.WriteFormat("0x{0:X}", (uint) Offset);
            }
            else
            {
                renderer.WriteFormat("{0}(%{1})", Offset, Base);
            }
        }
    }
}