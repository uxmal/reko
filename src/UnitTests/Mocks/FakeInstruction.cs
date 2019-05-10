#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakeInstruction : MachineInstruction
    {
        private Operation operation;
        private MachineOperand[] ops;

        public FakeInstruction(Operation operation, params MachineOperand[] ops)
        {
            this.operation = operation;
            this.ops = ops;
            this.InstructionClass = InstrClass.Invalid;
        }

        public FakeInstruction(InstrClass iClass, Operation operation, params MachineOperand[] ops)
        {
            this.InstructionClass = iClass;
            this.operation = operation;
            this.ops = ops;
        }

        public override int OpcodeAsInteger { get { return (int)operation; } }
        public Operation Operation { get { return operation; } }
        public MachineOperand[] Operands { get { return ops; } }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(operation.ToString().ToLower());
        }
    }

    public enum Operation
    {
        Invalid = -1,
        Nop,
        Add,
        Sub,
        Mul,
        Call,
        Jump,
        Branch,
        Ret,
    }
}
