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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public class ShiftOperand : MachineOperand
    {
        public ShiftOperand(Opcode2 opcode, MachineOperand op) : base(op.Width)
        {
            this.Opcode = opcode;
            this.Shift = op;
        }

        public ShiftOperand(Opcode2 opcode, PrimitiveType width) : base(width)
        {
            this.Opcode = opcode;
        }

        public ShiftOperand(MachineOperand op, Opcode2 opcode, int shAmt)
            : this(op, opcode, ArmImmediateOperand.Byte((byte)shAmt))
        {
        }

        public ShiftOperand(MachineOperand op, Opcode2 opcode, MachineOperand shAmt)
            : base(op.Width)
        {
            this.Operand = op;
            this.Opcode = opcode;
            this.Shift = shAmt;
        }
        public MachineOperand Operand { get; set; }
        public Opcode2 Opcode { get; set; }
        public MachineOperand Shift { get; set; }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            Operand.Write(fExplicit, writer);
            writer.Write(",");
            writer.WriteOpcode(Opcode.ToString());
            writer.Write(' ');
            Shift.Write(fExplicit, writer);
        }
    }
}
