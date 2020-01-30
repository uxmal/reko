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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakeInstruction : MachineInstruction
    {

        public FakeInstruction(Mnemonic mnemonic, params MachineOperand[] ops)
        {
            this.Mnemonic = mnemonic;
            this.Operands = ops;
            this.InstructionClass = InstrClass.Invalid;
        }

        public FakeInstruction(InstrClass iClass, Mnemonic mnemonic, params MachineOperand[] ops)
        {
            this.InstructionClass = iClass;
            this.Mnemonic = mnemonic;
            this.Operands = ops;
        }

        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger { get { return (int)Mnemonic; } }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString().ToLower());
        }
    }

    public enum Mnemonic
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
