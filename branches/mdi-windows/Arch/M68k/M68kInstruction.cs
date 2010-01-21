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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kInstruction : MachineInstruction
    {
        public Opcode code;
        public PrimitiveType dataWidth;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;


        public override uint DefCc()
        {
            throw new NotImplementedException();
        }

        public override uint UseCc()
        {
            return 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            InstructionFormatter fmt = new InstructionFormatter(sb);
            fmt.Write(this);
            return sb.ToString();
        }
    }
}
