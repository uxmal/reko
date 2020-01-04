#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public RegisterStorage Index;
        public Constant Offset;
        public int Increment;

        public MemoryOperand(PrimitiveType size) : base(size)
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

        public static MachineOperand Absolute(PrimitiveType size, ushort uAddr)
        {
            return new MemoryOperand(size)
            {
                Offset = Constant.Word16(uAddr)
            };
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteChar('(');
            if (Base != null)
            {
                if (Increment < 0)
                {
                    writer.WriteFormat("{0}:-", -Increment);
                }
                writer.WriteString(Base.Name);
                if (Index != null)
                {
                    writer.WriteChar('+');
                    writer.WriteString(Index.Name);
                }
                else if (Offset != null)
                {
                    int off = Offset.ToInt32();
                    int absOff;
                    if (off < 0)
                    {
                        writer.WriteChar('-');
                        absOff = -off;
                    }
                    else
                    {
                        writer.WriteChar('+');
                        absOff = off;
                    }
                    writer.WriteString("0x");
                    writer.WriteFormat(OffsetFormat(off), absOff);
                }
                if (Increment > 0)
                {
                    writer.WriteFormat("+:{0}", Increment);
                }
            }
            else
            {
                var addr = Address.Ptr16(Offset.ToUInt16());
                writer.WriteAddress(addr.ToString(), addr);
            }
            writer.WriteChar(')');
        }

        private string OffsetFormat(int off)
        {
            if (-0x80 <= off && off < 0x80)
                return "{0:X2}";
            return "{0:X4}";
        }
    }
}
