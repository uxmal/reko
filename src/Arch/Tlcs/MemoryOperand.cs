#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public RegisterStorage Index;
        public Constant Offset;
        public int Increment;

        private MemoryOperand(PrimitiveType size) : base(size)
        {
        }

        public static MemoryOperand Indirect(PrimitiveType size, RegisterStorage reg)
        {
            return new MemoryOperand(size)
            {
                Base = reg,
            };
        }

        public static MachineOperand Indexed8(PrimitiveType size, RegisterStorage reg, sbyte offset)
        {
            return new MemoryOperand(size)
            {
                Base = reg,
                Offset = Constant.SByte(offset)
            };
        }

        public static MachineOperand Indexed16(PrimitiveType size, RegisterStorage reg, short offset)
        {
            return new MemoryOperand(size)
            {
                Base = reg,
                Offset = Constant.Int16(offset)
            };
        }

        public static MachineOperand RegisterIndexed(PrimitiveType size, RegisterStorage rBase, RegisterStorage rIdx)
        {
            return new MemoryOperand(size)
            {
                Base = rBase,
                Index = rIdx
            };
        }

        public static MachineOperand PreDecrement(PrimitiveType size, int decrement, RegisterStorage reg)
        {
            return new MemoryOperand(size)
            {
                Base = reg,
                Increment = -decrement,
            };
        }

        public static MachineOperand PostIncrement(PrimitiveType size, int increment, RegisterStorage reg)
        {
            return new MemoryOperand(size)
            {
                Base = reg,
                Increment = increment,
            };
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            writer.Write('(');
            if (Base != null)
            {
                if (Increment < 0)
                {
                    writer.Write('-');
                }
                writer.Write(Base.Name);
                if (Index != null)
                {
                    writer.Write('+');
                    writer.Write(Index.Name);
                }
                else if (Offset != null)
                {
                    writer.Write('+');
                    writer.Write(Offset.ToString());
                } 
                if (Increment > 0)
                {
                    writer.Write('+');
                }
            }
            writer.Write(')');
        }

    
    }
}
