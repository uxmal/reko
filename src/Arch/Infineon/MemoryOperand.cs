#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Infineon
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(DataType dt, RegisterStorage? baseReg, int offset)
            : base(dt)
        {
            this.Base = baseReg;
            this.Offset = offset;
        }

        public RegisterStorage? Base { get; }
        public int Offset { get; }

        public bool PostIncrement { get; init; }
        public bool PreIncrement { get; init; }

        public static MemoryOperand Absolute(DataType dt, int off18)
        {
            return new MemoryOperand(dt, null, off18);
        }

        public static MemoryOperand BaseOffset(DataType dt, RegisterStorage areg, int offset)
        {
            return new MemoryOperand(dt, areg, offset);
        }

        internal static MemoryOperand PostInc(DataType dt, RegisterStorage baseReg, int offset=0)
        {
            return new MemoryOperand(dt, baseReg, offset)
            {
                PostIncrement = true
            };
        }

        internal static MemoryOperand PreInc(DataType dt, RegisterStorage baseReg, int offset)
        {
            return new MemoryOperand(dt, baseReg, offset)
            {
                PreIncrement = true
            };
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('[');
            if (Base is null)
            {
                var addr = Address.Ptr32((uint) Offset);
                renderer.WriteAddress(addr.ToString(), addr);
                renderer.WriteChar(']');
                return;
            }
            if (PreIncrement)
            {
                renderer.WriteChar('+');
            }
            renderer.WriteString(Base.Name);
            if (PostIncrement)
            {
                renderer.WriteChar('+');
            }
            renderer.WriteChar(']');
            if (Offset != 0)
            {
                renderer.WriteString(Offset.ToString());
            }
        }
    }
}