#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcInstruction : MachineInstruction
    {
        private Opcode opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;
        public MachineOperand op4;
        public MachineOperand op5;
        public bool setsCR0;

        public PowerPcInstruction(Opcode opcode)
        {
            this.opcode = opcode;
        }

        public PowerPcInstruction(Opcode opcode, MachineOperand op1, MachineOperand op2, MachineOperand op3, bool setsCR0)
        {
            this.opcode = opcode;
            this.op1 = op1;
            this.op2 = op2;
            this.op3 = op3;
            this.setsCR0 = setsCR0;
        }

        public override int OpcodeAsInteger { get { return (int)opcode; } }

        public int Operands
        {
            get
            {
                if (op1 == null)
                    return 0;
                if (op2 == null)
                    return 1;
                if (op3 == null)
                    return 2;
                if (op4 == null)
                    return 3;
                if (op5 == null)
                    return 4;
                return 5;
            }
        }
	
        public Opcode Opcode
        {
            get { return opcode; }
        }

        public override void Render(MachineInstructionWriter writer)
        {
            var op = string.Format("{0}{1}", 
                opcode,
                setsCR0 ? "." : "");
            writer.WriteOpcode(op);
            if (op1 != null)
            {
                writer.Tab();
                op1.Write(true, writer);
                if (op2 != null)
                {
                    writer.Write(',');
                    op2.Write(true, writer);
                    if (op3 != null)
                    {
                        writer.Write(',');
                        op3.Write(true, writer);
                        if (op4 != null)
                        {
                            writer.Write(",");
                            op4.Write(true, writer);
                            if (op5 != null)
                            {
                                writer.Write(",");
                                op5.Write(true, writer);
                            }
                        }
                    }
                }
            }
        }

        public uint DefCc()
        {
            if (setsCR0)
                return 0xF;
            return 0;
        }
    }

    public class AddressOperand : MachineOperand
    {
        public Address Address;

        public AddressOperand(Address a)
            : base(PrimitiveType.Pointer32)	//$BUGBUG: 64-bit pointers?
        {
            Address = a;
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
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

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            if (condition > 3)
                writer.Write("cr{0}+", condition >> 2);
            var s = "";
            switch (condition & 3)
            {
            case 0: s = "lt"; break;
            case 1: s = "gt"; break;
            case 2: s = "eq"; break;
            case 3: s = "so"; break;
            }
            writer.Write(s);
        }
    }
}
