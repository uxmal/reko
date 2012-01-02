#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using System.Text;
using M68kAddressOperand = Decompiler.Arch.M68k.AddressOperand;

namespace Decompiler.Arch.M68k
{
    public class Decoder
    {
        public readonly Opcode opcode;
        public readonly string args;

        public Decoder(Opcode opcode, string args)
        {
            this.opcode = opcode;
            this.args = args;
        }

        public M68kInstruction Decode(ushort opcode, ImageReader rdr)
        {
            M68kInstruction instr = new M68kInstruction();
            instr.code = this.opcode;
            int i = 0;
            if (args[0] == 's')
            {
                instr.dataWidth = OperandFormatDecoder.GetSizeType(opcode, args[1], null);
                i = 3;
            }

            OperandFormatDecoder opTranslator = new OperandFormatDecoder(opcode, args, i);
            instr.op1 = opTranslator.GetOperand(rdr, instr.dataWidth);
            instr.op2 = opTranslator.GetOperand(rdr, instr.dataWidth);
            instr.op3 = opTranslator.GetOperand(rdr, instr.dataWidth);
            return instr;
        }
    }
}
