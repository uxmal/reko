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

using Reko.Arch.M68k.Disassembler;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.M68k.Machine
{
#pragma warning disable IDE1006

    public class DoubleRegisterOperand : AbstractMachineOperand
    {
        public DoubleRegisterOperand(RegisterStorage reg1, RegisterStorage reg2) : base(PrimitiveType.Word64)
        {
            Register1 = reg1;
            Register2 = reg2;
        }

        public RegisterStorage Register1 { get; private set; }
        public RegisterStorage Register2 { get; private set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteFormat("{0},{1}", Register1.Name, Register2.Name);
        }
    }

    public class MemoryOperand : AbstractMachineOperand
    {
        public const string HexStringFormat = "{0}${1}";

        public RegisterStorage Base;
        public Constant? Offset;

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg)
            : base(width)
        {
            Base = baseReg;
        }

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg, Constant offset)
            : base(width)
        {
            Base = baseReg;
            Offset = offset;
        }

        public static MemoryOperand Indirect(PrimitiveType width, RegisterStorage baseReg)
        {
            return new MemoryOperand(width, baseReg);
        }

        public static MachineOperand Indirect(PrimitiveType width, RegisterStorage baseReg, Constant offset)
        {
            return new MemoryOperand(width, baseReg, offset);
        }

        public static MachineOperand PreDecrement(PrimitiveType dataWidth, RegisterStorage baseReg)
        {
            return new PredecrementMemoryOperand(dataWidth, baseReg);
        }

        public static MachineOperand PostIncrement(PrimitiveType dataWidth, RegisterStorage baseReg)
        {
            return new PostIncrementMemoryOperand(dataWidth, baseReg);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Offset is not null)
            {
                renderer.WriteString(FormatValue(Offset, false, M68kDisassembler.HexStringFormat));
            }
            renderer.WriteString("(");
            renderer.WriteString(Base.Name);
            renderer.WriteString(")");
        }
    }

    public class PredecrementMemoryOperand : AbstractMachineOperand
    {
        public readonly RegisterStorage Register;

        public PredecrementMemoryOperand(PrimitiveType dataWidth, RegisterStorage areg)
            : base(dataWidth)
        {
            Register = areg;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString("-(");
            renderer.WriteString(Register.Name);
            renderer.WriteString(")");
        }
    }

    public class PostIncrementMemoryOperand : AbstractMachineOperand
    {
        public readonly RegisterStorage Register;

        public PostIncrementMemoryOperand(PrimitiveType dataWidth, RegisterStorage areg)
            : base(dataWidth)
        {
            Register = areg;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString("(");
            renderer.WriteString(Register.Name);
            renderer.WriteString(")+");
        }
    }

    public class IndirectIndexedOperand : AbstractMachineOperand
    {
        public readonly sbyte Imm8;
        public readonly RegisterStorage ARegister;
        public readonly RegisterStorage XRegister;
        public readonly PrimitiveType XWidth;
        public readonly byte Scale;

        public IndirectIndexedOperand(PrimitiveType dataWidth, sbyte imm8, RegisterStorage a, RegisterStorage x, PrimitiveType width, int scale)
            : base(dataWidth)
        {
            Imm8 = imm8;
            ARegister = a;
            XRegister = x;
            XWidth = width;
            Scale = (byte) scale;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString("(");
            if (Imm8 < 0)
            {
                renderer.WriteFormat("-${0:X2},", -Imm8);
            }
            else if (Imm8 > 0)
            {
                renderer.WriteFormat("${0:X2},", Imm8);
            }
            renderer.WriteString(ARegister.Name);
            if (XRegister is not null)
            {
                renderer.WriteString(",");
                renderer.WriteString(XRegister.Name);
                if (XWidth.Size == 2)
                    renderer.WriteString(".w");
                if (Scale > 1)
                    renderer.WriteFormat("*{0}", Scale);
            }
            renderer.WriteString(")");
        }
    }
}
