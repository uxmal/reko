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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public class PowerPcInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic;
        public bool setsCR0;

        public PowerPcInstruction(Mnemonic mnemonic)
        {
            this.Mnemonic = mnemonic;
        }

        public PowerPcInstruction(Mnemonic mnemonic, MachineOperand op1, MachineOperand op2, MachineOperand op3, bool setsCR0)
        {
            this.Mnemonic = mnemonic;
            this.Operands = new MachineOperand[] { op1, op2, op3 };
            this.setsCR0 = setsCR0;
            this.InstructionClass = InstrClass.Linear;
        }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var op = string.Format("{0}{1}", 
                Mnemonic,
                setsCR0 ? "." : "");
            writer.WriteMnemonic(op);
            RenderOperands(writer, options);
        }
    }

    public class AddressOperand : MachineOperand
    {
        public Address Address;

        public AddressOperand(Address a)
            : base(PrimitiveType.Ptr32)	//$BUGBUG: 64-bit pointers?
        {
            Address = a;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteAddress("$" + Address.ToString(), Address);
        }
    }

    public class ConditionOperand : MachineOperand
    {
        public uint condition;

        public ConditionOperand(uint condition) : base(PrimitiveType.Byte)
        {
            this.condition = condition;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (condition > 3)
                writer.WriteFormat("cr{0}+", condition >> 2);
            var s = "";
            switch (condition & 3)
            {
            case 0: s = "lt"; break;
            case 1: s = "gt"; break;
            case 2: s = "eq"; break;
            case 3: s = "so"; break;
            }
            writer.WriteString(s);
        }
    }
}
