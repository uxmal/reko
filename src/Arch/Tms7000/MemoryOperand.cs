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

using System;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Tms7000
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Register;
        public Address? Address;

        public MemoryOperand() : base(PrimitiveType.Byte) { }
        public static MemoryOperand Indexed(Address address, RegisterStorage b)
        {
            return new MemoryOperand
            {
                Address = address,
                Register = b
            };
        }

        public static MemoryOperand Direct(Address address)
        {
            return new MemoryOperand
            {
                Address = address,
            };
        }

        public static MemoryOperand Indirect(RegisterStorage reg)
        {
            return new MemoryOperand
            {
                Register = reg,
            };
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Address is null)
            {
                renderer.WriteChar('*');
                renderer.WriteString(Register!.Name);
            }
            else
            {
                renderer.WriteAddress("@" + Address, Address.Value);
                if (Register is not null)
                {
                    renderer.WriteChar('(');
                    renderer.WriteString(Register.Name);
                    renderer.WriteChar(')');
                }
            }
        }
    }
}