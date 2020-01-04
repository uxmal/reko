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
using System.Text;

namespace Reko.Arch.M68k
{
    public interface M68kOperand
    {
        T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public interface M68kOperandVisitor<T>
    {
        T Visit(M68kImmediateOperand imm);
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

    public abstract class M68kOperandImpl : MachineOperand, M68kOperand
    {
        public M68kOperandImpl(PrimitiveType dataWidth)
            : base(dataWidth)
        {
        }

        public abstract T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public class M68kImmediateOperand : M68kOperandImpl
    {
        public M68kImmediateOperand(Constant cons) : base((PrimitiveType)cons.DataType)
        {
            Constant = cons;
        }

        public Constant Constant { get; private set; }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString("#");
            writer.WriteString(MachineOperand.FormatValue(Constant, false, M68kDisassembler.HexStringFormat));
        }
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

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteFormat("{0},{1}", Register1.Name, Register2.Name);
        }
    }

    public class MemoryOperand : M68kOperandImpl
    {
        public const string HexStringFormat = "{0}${1}";

        public AddressRegister Base;
        public Constant Offset;

        public MemoryOperand(PrimitiveType width, AddressRegister baseReg)
            : base(width)
        {
            this.Base = baseReg;
        }

        public MemoryOperand(PrimitiveType width, AddressRegister baseReg, Constant offset)
            : base(width)
        {
            this.Base = baseReg;
            this.Offset = offset;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public static MemoryOperand Indirect(PrimitiveType width, AddressRegister baseReg)
        {
            return new MemoryOperand(width, baseReg);
        }

        public static MachineOperand Indirect(PrimitiveType width, AddressRegister baseReg, Constant offset)
        {
            return new MemoryOperand(width, baseReg, offset);
        }

        public static MachineOperand PreDecrement(PrimitiveType dataWidth, AddressRegister baseReg)
        {
            return new PredecrementMemoryOperand(dataWidth, baseReg);
        }

        public static MachineOperand PostIncrement(PrimitiveType dataWidth, AddressRegister baseReg)
        {
            return new PostIncrementMemoryOperand(dataWidth, baseReg);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Offset != null)
            {
                writer.WriteString(MachineOperand.FormatValue(Offset, false, M68kDisassembler.HexStringFormat));
            }
            writer.WriteString("(");
            writer.WriteString(Base.Name);
            writer.WriteString(")");
        }
    }

    public class PredecrementMemoryOperand : M68kOperandImpl
    {
        public readonly AddressRegister Register;

        public PredecrementMemoryOperand(PrimitiveType dataWidth, AddressRegister areg)
            : base(dataWidth)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString("-(");
            writer.WriteString(Register.Name);
            writer.WriteString(")");
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

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString("(");
            writer.WriteString(Register.Name);
            writer.WriteString(")+");
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

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString("(");
            if (Imm8 < 0)
            {
                writer.WriteFormat("-${0:X2},", -Imm8);
            }
            else if (Imm8 > 0)
            {
                writer.WriteFormat("${0:X2},", Imm8);
            }
            writer.WriteString(ARegister.Name);
            if (XRegister != null)
            {
                writer.WriteString(",");
                writer.WriteString(XRegister.Name);
                if (XWidth.Size == 2)
                    writer.WriteString(".w");
                if (Scale > 1)
                    writer.WriteFormat("*{0}", Scale);
            }
            writer.WriteString(")");
        }
    }
}
