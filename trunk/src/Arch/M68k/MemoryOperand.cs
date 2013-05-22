#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public interface M68kOperand
    {
        T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public interface M68kOperandVisitor<T>
    {
        T Visit(M68kImmediateOperand imm);

        T Visit(PredecrementMemoryOperand pre);

        T Visit(AddressOperand addressOperand);

        T Visit(PostIncrementMemoryOperand post);

        T Visit(RegisterSetOperand registerSet);

        T Visit(IndexedOperand indexedOperand);
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
    }

    public class DoubleRegisterOperand : M68kOperandImpl
    {
        public DoubleRegisterOperand(RegisterStorage reg1, RegisterStorage reg2) : base(null)
        {
            this.Register1 = reg1;
            this.Register2 = reg2;
        }

        public RegisterStorage Register1 { get; private set; }
        public RegisterStorage Register2 { get; private set; }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

    }

    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public Constant Offset;

        public MemoryOperand(RegisterStorage baseReg)
            : base(null)
        {
            this.Base = baseReg;
        }

        public MemoryOperand(RegisterStorage baseReg, Constant offset)
            : base(null)
        {
            this.Base = baseReg;
            this.Offset = offset;
        }

        public static MemoryOperand Indirect(RegisterStorage baseReg)
        {
            return new MemoryOperand(baseReg);
        }

        public static MachineOperand Indirect(RegisterStorage baseReg, Constant offset)
        {
            return new MemoryOperand(baseReg, offset);
        }

        public static MachineOperand PreDecrement(RegisterStorage baseReg)
        {
            return new PredecrementMemoryOperand(baseReg);
        }

        public static MachineOperand PostIncrement(RegisterStorage baseReg)
        {
            return new PostIncrementMemoryOperand(baseReg);
        }
    }


    public class PredecrementMemoryOperand : M68kOperandImpl
    {
        public readonly RegisterStorage Register;

        public PredecrementMemoryOperand(RegisterStorage areg)
            : base(null)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class PostIncrementMemoryOperand : M68kOperandImpl
    {
        public readonly RegisterStorage Register;

        public PostIncrementMemoryOperand(RegisterStorage areg)
            : base(null)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
