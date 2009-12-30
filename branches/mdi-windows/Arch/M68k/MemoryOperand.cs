/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
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
        T Visit(PredecrementMemoryOperand pre);

        T Visit(AddressOperand addressOperand);

        T Visit(PostIncrementMemoryOperand post);
    }

    public abstract class M68kOperandImpl : MachineOperand, M68kOperand
    {
        public M68kOperandImpl(PrimitiveType dataWidth)
            : base(dataWidth)
        {
        }

        public abstract T Accept<T>(M68kOperandVisitor<T> visitor);
    }

    public class MemoryOperand : MachineOperand
    {
        public MachineRegister Base;
        public Constant Offset;

        public MemoryOperand(PrimitiveType dataWidth, MachineRegister baseReg)
            : base(dataWidth)
        {
            this.Base = baseReg;
        }
        public MemoryOperand(PrimitiveType dataWidth, MachineRegister baseReg, Constant offset)
            : base(dataWidth)
        {
            this.Base = baseReg;
            this.Offset = offset;
        }

        public static MemoryOperand Indirect(PrimitiveType dataWidth, MachineRegister baseReg)
        {
            return new MemoryOperand(dataWidth, baseReg);
        }

        public static MachineOperand Indirect(PrimitiveType dataWidth, MachineRegister baseReg, Constant offset)
        {
            return new MemoryOperand(dataWidth, baseReg, offset);
        }

        public static MachineOperand PreDecrement(PrimitiveType dataWidth, MachineRegister baseReg)
        {
            return new PredecrementMemoryOperand(dataWidth, baseReg);
        }

        public static MachineOperand PostIncrement(PrimitiveType dataWidth, MachineRegister baseReg)
        {
            return new PostIncrementMemoryOperand(dataWidth, baseReg);
        }
    }


    public class PredecrementMemoryOperand : M68kOperandImpl
    {
        public readonly MachineRegister Register;

        public PredecrementMemoryOperand(PrimitiveType dataWidth, MachineRegister areg)
            : base(dataWidth)
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
        public readonly MachineRegister Register;

        public PostIncrementMemoryOperand(PrimitiveType dataWidth, MachineRegister areg)
            : base(dataWidth)
        {
            this.Register = areg;
        }

        public override T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }


}
