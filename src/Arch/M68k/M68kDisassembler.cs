/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kDisassembler : Disassembler
    {
        private ImageReader rdr;

        public M68kDisassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override Address Address
        {
            get { throw new NotImplementedException(); }
        }

        public override MachineInstruction DisassembleInstruction()
        {
            return Disassemble();
        }

        public M68kInstruction Disassemble()
        {
            M68kInstruction instr = new M68kInstruction();

            ushort opcode = rdr.ReadBeUint16();
            switch (opcode >> 12)
            {
                case 0x05:
                    instr.code = Opcode.addq;
                    instr.dataWidth = SizeField(opcode, 6); ;
                    instr.op1 = SignedImmediateByte(opcode, 9, 0x07);
                    instr.op2 = ParseOperand(opcode, 0, instr.dataWidth);
                    break;
                case 0x7:
                    instr.code = Opcode.moveq;
                    instr.op1 = SignedImmediateByte(opcode, 0, 0xFF);
                    instr.op2 = DataRegister(opcode, 9);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown 680x0 opcode {0:X4}.", opcode));
            }

            return instr;
        }

        private PrimitiveType SizeField(ushort opcode, int bitOffset)
        {
            switch ((opcode >> bitOffset) & 3)
            {
                case 0: return PrimitiveType.Byte;
                case 1: return PrimitiveType.Word16;
                case 2: return PrimitiveType.Word32;
                default: throw new InvalidOperationException(string.Format("Illegal size field in opcode {0:X4}.", opcode));
            }
        }
        private MachineOperand ParseOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth)
        {
            int operandBits = opcode >> bitOffset;
            int addressMode = (operandBits >> 3 )& 0x07;
            switch (addressMode)
            {
                case 2:  // Address register indirect
                    return MemoryOperand.Indirect(dataWidth, AddressRegister(opcode, bitOffset));
                default: throw new NotImplementedException();
            }

        }

        private static MachineRegister AddressRegister(ushort opcode, int bitOffset)
        {
            return Registers.GetRegister(8 + ((opcode >> bitOffset) & 0x7));
        }

        private static RegisterOperand DataRegister(ushort opcode, int bitOffset)
        {
            return new RegisterOperand(Registers.GetRegister((opcode >> bitOffset) & 0x7));
        }

        private static ImmediateOperand SignedImmediateByte(ushort opcode, int bitOffset, int mask)
        {
            return new ImmediateOperand(new Constant(PrimitiveType.SByte, (opcode >> bitOffset) & mask));
        }
    }
}
