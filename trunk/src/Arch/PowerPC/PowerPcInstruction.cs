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
        private bool setsCR0;

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

        public Address Address { get; set; }

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
                return 3;
            }
        }
	
        public Opcode Opcode
        {
            get { return opcode; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(opcode);
            if (setsCR0)
                sb.Append('.');
            if (op1 != null)
            {
                sb.Append('\t');
                sb.Append(op1);
                if (op2 != null)
                {
                    sb.Append(',');
                    sb.Append(op2);
                    if (op3 != null)
                    {
                        sb.Append(',');
                        sb.Append(op3);
                    }
                }
            }
            return sb.ToString();
        }

        public override uint DefCc()
        {
            if (setsCR0)
                return 0xF;
            return 0;
        }

        public override uint UseCc()
        {
            throw new NotImplementedException();
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

        public override string ToString()
        {
            return "$" + Address.ToString();
        }
    }
}
