#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Disassembler : Disassembler
    {
        private ImageReader rdr;
        private Pdp11Architecture arch;
        private Pdp11Instruction instr;

        public Pdp11Disassembler(ImageReader rdr, Pdp11Architecture arch)
        {
            this.rdr = rdr;
            this.arch = arch;
        }

        public Address Address
        {
            get { throw new NotImplementedException(); }
        }

        public MachineInstruction DisassembleInstruction()
        {
            ushort opcode = rdr.ReadLeUInt16();
            switch ((opcode >> 0x0C) & 7)
            {
            case 0: return NonDoubleOperandInstruction(opcode);
            case 1:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.mov,
                    DataWidth = DataWidthFromSizeBit(opcode & 0x8000u),
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6)
                };
            }
            throw new NotImplementedException();
        }

        private PrimitiveType DataWidthFromSizeBit(uint p)
        {
            throw new NotImplementedException();
        }

        private MachineInstruction NonDoubleOperandInstruction(int opcode)
        {
            throw new NotImplementedException();
        }

        private MachineOperand DecodeOperand(int operandBits)
        {
            MachineRegister reg = arch.GetRegister(operandBits & 7);
            if (reg == Registers.pc)
            {
                throw new NotImplementedException();
            }
            else
            {
                switch ((operandBits >> 3) & 7)
                {
                //case 0: return new RegisterOperand(reg);    //   Reg           Direct addressing of the register
                //case 1: return new MemoryOperand(reg);      //   Reg Def       Contents of Reg is the address
                //case 2:    //   AutoIncr      Contents of Reg is the address, then Reg incremented
                //case 3:    //   AutoIncrDef   Content of Reg is addr of addr, then Reg Incremented
                //case 4:    //   AutoDecr      Reg is decremented then contents is address
                //case 5:    //   AutoDecrDef   Reg is decremented then contents is addr of addr
                //case 6: return new MemoryOperand(reg, rdr.ReadLeUInt16());   //   Index         Contents of Reg + Following word is address
                //case 7:   //   IndexDef      Contents of Reg + Following word is addr of addr
                default: throw new NotSupportedException();
                }
            }
        }
    }
}
