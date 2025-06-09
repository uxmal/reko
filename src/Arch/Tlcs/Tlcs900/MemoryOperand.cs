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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Base;
        public RegisterStorage? Index;
        public Constant? Offset;
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

        public static MachineOperand Absolute(PrimitiveType size, uint uAddr)
        {
            return new MemoryOperand(size)
            {
                Offset = Constant.Word32(uAddr)
            };
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('(');
            if (Base is not null)
            {
                if (Increment < 0)
                {
                    renderer.WriteFormat("{0}:-", -Increment);
                }
                renderer.WriteString(Base.Name);
                if (Index is not null)
                {
                    renderer.WriteChar('+');
                    renderer.WriteString(Index.Name);
                }
                else if (Offset is not null)
                {
                    int off = Offset.ToInt32();
                    int absOff;
                    if (off < 0)
                    {
                        renderer.WriteChar('-');
                        absOff = -off;
                    }
                    else
                    {
                        renderer.WriteChar('+');
                        absOff = off;
                    }
                    renderer.WriteString("0x");
                    renderer.WriteFormat(OffsetFormat(off), absOff);
                }
                if (Increment > 0)
                {
                    renderer.WriteFormat("+:{0}", Increment);
                }
            }
            else
            {
                var addr = Address.Ptr32(Offset!.ToUInt32());
                renderer.WriteAddress(addr.ToString(), addr);
            }
            renderer.WriteChar(')');
        }

        private string OffsetFormat(int off)
        {
            if (-0x80 <= off && off < 0x80)
                return "{0:X2}";
            if (-0x8000 <= off && off < 0x8000)
                return "{0:X4}";
            return "{0:X8}";
        }
    }
}
