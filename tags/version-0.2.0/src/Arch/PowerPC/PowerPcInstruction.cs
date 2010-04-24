/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcInstruction : MachineInstruction
    {
        private Opcode opcode;
        private MachineOperand op1;
        private MachineOperand op2;
        private MachineOperand op3;
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
            throw new NotImplementedException();
        }

        public override uint UseCc()
        {
            throw new NotImplementedException();
        }
    }

}
