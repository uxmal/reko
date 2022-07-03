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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(
            RegisterStorage? @base, 
            RegisterStorage? index,
            Constant? displacement) 
            : base(PrimitiveType.Byte)
        {
            this.Base = @base;
            this.Index = index;
            this.Displacement = displacement;
        }

        public RegisterStorage? Base { get; }
        public RegisterStorage? Index { get; }
        public Constant? Displacement { get; }
        public bool IsIoAddress { get; private set; }
        public bool PostIncrement { get; private set; }


        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Base is { })
            {
                renderer.WriteChar('@');
                renderer.WriteString(Base.Name);
                if (PostIncrement)
                {
                    renderer.WriteChar('+');
                    return;
                }
                if (Displacement is { })
                {
                    Instruction.RenderSignedConstant(Displacement, renderer);
                }
            }
            else
            {
                Debug.Assert(Displacement is not null);
                if (IsIoAddress)
                {
                    renderer.WriteString("I:");
                }
                Instruction.RenderUnsignedConstant(Displacement, renderer);
            }
        }

        public static MemoryOperand Direct(byte displacement)
        {
            return new MemoryOperand(null, null, Constant.Byte(displacement));
        }

        public static MemoryOperand Addr16(ushort addr16)
        {
            return new MemoryOperand(null, null, Constant.Word16(addr16));
        }

        public static MemoryOperand Io(byte ioRegister)
        {
            return new MemoryOperand(null, null, Constant.Byte(ioRegister))
            {
                IsIoAddress = true
            };
        }

        public static MemoryOperand IndirectA()
        {
            return new MemoryOperand(Registers.a, null, null);
        }

        public static MemoryOperand RegisterIndirect(RegisterStorage reg, Constant? disp = null)
        {
            return new MemoryOperand(reg, null, disp);
        }

        public static MemoryOperand RegisterIndirectPost(RegisterStorage reg)
        {
            return new MemoryOperand(reg, null, null)
            {
                PostIncrement = true,
            };
        }

        public static MemoryOperand RegisterIndexed(RegisterStorage @base, RegisterStorage index)
        {
            return new MemoryOperand(@base, index, null);
        }
    }
}