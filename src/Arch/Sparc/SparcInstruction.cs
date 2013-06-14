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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcInstruction : MachineInstruction
    {
        public Opcode Opcode;
        public MachineOperand Op1;
        public MachineOperand Op2;
        public MachineOperand Op3;

        public override uint DefCc()
        {
            throw new NotImplementedException();
        }

        public override uint UseCc()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Opcode);
            //if (setsCR0)
            //    sb.Append('.');
            if (Op1 != null)
            {
                sb.Append('\t');
                Write(Op1, sb);
                if (Op2 != null)
                {
                    sb.Append(',');
                    Write(Op2, sb);
                    if (Op3 != null)
                    {
                        sb.Append(',');
                        Write(Op3, sb);
                    }
                }
            }
            return sb.ToString();
        }

        private void Write(MachineOperand op, StringBuilder sb)
        {
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                sb.AppendFormat("%{0}", reg.Register.Name);
                return;
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                sb.Append(imm.Value);
                return;
            }
            sb.Append(op);
        }
    }
}
