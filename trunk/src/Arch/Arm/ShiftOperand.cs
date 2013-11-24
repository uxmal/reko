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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ShiftOperand : MachineOperand
    {
        public ShiftOperand(Opcode opcode, MachineOperand op) : base(op.Width)
        {
            this.Opcode = opcode;
            this.Shift = op;
        }

        public ShiftOperand(Opcode opcode, PrimitiveType width) : base(width)
        {
            this.Opcode = opcode;
        }

        public ShiftOperand(MachineOperand op, Opcode opcode, int shAmt)
            : this(op, opcode, new ImmediateOperand(Constant.Byte((byte)shAmt)))
        {
        }

        public ShiftOperand(MachineOperand op, Opcode opcode, MachineOperand shAmt)
            : base(op.Width)
        {
            this.Operand = op;
            this.Opcode = opcode;
            this.Shift = shAmt;
        }
        public MachineOperand Operand { get; set; }
        public Opcode Opcode { get; set; }
        public MachineOperand Shift { get; set; }
    }
}
