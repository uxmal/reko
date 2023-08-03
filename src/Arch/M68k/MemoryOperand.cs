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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.M68k
{
#pragma warning disable IDE1006

    public interface M68kOperand
    {
        T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public interface M68kOperandVisitor<T>
    {
        T Visit(MemoryOperand mem);
        T Visit(PredecrementMemoryOperand pre);
        T Visit(M68kAddressOperand addressOperand);
        T Visit(PostIncrementMemoryOperand post);
        T Visit(RegisterSetOperand registerSet);
        T Visit(IndexedOperand indexedOperand);
        T Visit(IndirectIndexedOperand indirectIndexedOperand);
        T Visit(DoubleRegisterOperand doubleRegisterOperand);
        T Visit(BitfieldOperand bitfield);
    }

    public abstract class M68kOperandImpl : AbstractMachineOperand, M68kOperand
    {
        public M68kOperandImpl(PrimitiveType dataWidth)
            : base(dataWidth)
        {
        }

        public abstract T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public class DoubleRegisterOperand : M68kOperandImpl
    {
        public DoubleRegisterOperand(RegisterStorage reg1, RegisterStorage reg2) : base(PrimitiveType.Word64)
        {
            this.Register1 = reg1;
            this.Register2 = reg2;
        }

        public RegisterStorage Register1 { get; private set; }
        public RegisterStorage Register2 { get; private set; }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteFormat("{0},{1}", Register1.Name, Register2.Name);
        }
    }

    public class MemoryOperand : M68kOperandImpl
    {
        public const string HexStringFormat = "{0}${1}";

        public RegisterStorage Base;
        public Constant? Offset;

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg)
            : base(width)
        {
            this.Base = baseReg;
        }

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg, Constant offset)
            : base(width)
        {
            this.Base = baseReg;
            this.Offset = offset;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
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
            if (Offset != null)
            {
                renderer.WriteString(FormatValue(Offset, false, M68kDisassembler.HexStringFormat));
            }
            renderer.WriteString("(");
            renderer.WriteString(Base.Name);
            renderer.WriteString(")");
        }
    }

    public class PredecrementMemoryOperand : M68kOperandImpl
    {
        public readonly RegisterStorage Register;

        public PredecrementMemoryOperand(PrimitiveType dataWidth, RegisterStorage areg)
            : base(dataWidth)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString("-(");
            renderer.WriteString(Register.Name);
            renderer.WriteString(")");
        }
    }

    public class PostIncrementMemoryOperand : M68kOperandImpl
    {
        public readonly RegisterStorage Register;

        public PostIncrementMemoryOperand(PrimitiveType dataWidth, RegisterStorage areg)
            : base(dataWidth)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString("(");
            renderer.WriteString(Register.Name);
            renderer.WriteString(")+");
        }
    }

    public class IndirectIndexedOperand : M68kOperandImpl
    {
        public readonly sbyte Imm8;
        public readonly RegisterStorage ARegister;
        public readonly RegisterStorage XRegister;
        public readonly PrimitiveType XWidth;
        public readonly byte Scale;

        public IndirectIndexedOperand(PrimitiveType dataWidth, sbyte imm8, RegisterStorage a, RegisterStorage x, PrimitiveType width, int scale)
            : base(dataWidth)
        {
            this.Imm8 = imm8;
            this.ARegister = a;
            this.XRegister = x;
            this.XWidth = width;
            this.Scale = (byte)scale;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
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
            if (XRegister != null)
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
