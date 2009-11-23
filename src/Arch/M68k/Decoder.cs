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
                instr.dataWidth = GetSizeType(opcode, args[1], null);
                i = 3;
            }

            Opdecoder opTranslator = new Opdecoder(opcode, args, i);
            instr.op1 = opTranslator.GetOperand(rdr, instr.dataWidth);
            instr.op2 = opTranslator.GetOperand(rdr, instr.dataWidth);
            instr.op3 = opTranslator.GetOperand(rdr, instr.dataWidth);
            return instr;
        }


        public class Opdecoder
        {
            ushort opcode;
            string args;
            int i;

            public Opdecoder(ushort opcode, string args, int i)
            {
                this.opcode = opcode;
                this.args = args;
                this.i = i;
            }

            public MachineOperand GetOperand(ImageReader rdr, PrimitiveType dataWidth)
            {
                if (i >= args.Length)
                    return null;
                if (args[i] == ',')
                    ++i;

                switch (args[i++])
                {
                case 'A':
                    return new RegisterOperand(AddressRegister(opcode, GetOpcodeOffset(args[i++])));
                case 'c':
                    return new RegisterOperand(Registers.ccr);
                case 'D':
                    return DataRegisterOperand(opcode, GetOpcodeOffset(args[i++]));
                case 'E':
                    return ParseOperand(opcode, GetOpcodeOffset(args[i++]), dataWidth, rdr);
                case 'e':
                    return ParseSwappedOperand(opcode, GetOpcodeOffset(args[i++]), dataWidth, rdr);
                case 'I':
                    return GetImmediate(rdr, GetSizeType(0, args[i++], dataWidth));
                case 'J':
                    Address addr = rdr.Address;
                    int offset = opcode & 0xFF;
                    if (offset == 0xFF)
                        offset = rdr.ReadBeInt32();
                    else if (offset == 0x00)
                        offset = rdr.ReadBeInt16();
                    return new M68kAddressOperand(addr + offset);
                case 'q':
                    return GetQuickImmediate(GetOpcodeOffset(args[i++]), 7, 8, PrimitiveType.SByte);
                case 'Q':
                    return GetQuickImmediate(GetOpcodeOffset(args[i++]), 0xFF, 0, PrimitiveType.SByte);
                case 's':
                    return new RegisterOperand(Registers.sr);

                default: throw new FormatException(string.Format("Unknown argument type {0}.", args[--i]));
                }
            }

            private MachineOperand GetQuickImmediate(int offset, int mask, int zeroValue, PrimitiveType dataWidth)
            {
                int v = ((int)opcode >> offset) & mask;
                if (v == 0)
                    v = zeroValue;
                return new ImmediateOperand(new Constant(dataWidth, v));
            }

        }

        private static PrimitiveType SizeField(ushort opcode, int bitOffset)
        {
            switch ((opcode >> bitOffset) & 3)
            {
            case 0: return PrimitiveType.Byte;
            case 1: return PrimitiveType.Word16;
            case 2: return PrimitiveType.Word32;
            default: throw new InvalidOperationException(string.Format("Illegal size field in opcode {0:X4}.", opcode));
            }
        }

        private static ImmediateOperand GetImmediate(ImageReader rdr, PrimitiveType type)
        {
            if (type.Size == 1)
            {
                rdr.ReadByte();
            }
            return new ImmediateOperand(rdr.ReadBe(type));
        }

        private static MachineOperand ParseOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            byte operandBits = (byte)(opcode >> bitOffset);
            byte addressMode = (byte)((operandBits >> 3) & 0x07u);
            return ParseOperandInner(addressMode, operandBits, dataWidth, rdr);
        }

        private static MachineOperand ParseSwappedOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            byte addressMode = (byte)((opcode >> bitOffset) & 7);
            byte operandBits = (byte)(opcode >> (bitOffset + 3));
            return ParseOperandInner(addressMode, operandBits, dataWidth, rdr);
        }

        private static int GetOpcodeOffset(char c)
        {
            int offset = c - '0';
            if (offset < 0)
                throw new FormatException("Invalid offset specification.");
            if (offset < 10)
                return offset;
            return offset - 6;
        }

        private static PrimitiveType GetSizeType(ushort opcode, char c, PrimitiveType dataWidth)
        {
            switch (c)
            {
            case 'b': return PrimitiveType.Byte;
            case 'v': return dataWidth;
            case 'w': return PrimitiveType.Word16;
            case 'l': return PrimitiveType.Word32;
            default: return SizeField(opcode, GetOpcodeOffset(c)); ;
                throw new NotImplementedException();
            }
        }


        private static MachineOperand ParseOperandInner(byte addressMode, byte operandBits, PrimitiveType dataWidth, ImageReader rdr)
        {
            Constant offset;
            switch (addressMode)
            {
            case 0: // Data register direct.
                return DataRegisterOperand(operandBits, 0);
            case 1: // Address register direct
                return new RegisterOperand(AddressRegister(operandBits, 0));
            case 2:  // Address register indirect
                return MemoryOperand.Indirect(dataWidth, AddressRegister(operandBits, 0));
            case 3:  // Address register indirect with postincrement.
                return MemoryOperand.PostIncrement(dataWidth, AddressRegister(operandBits, 0));
            case 4:  // Address register indirect with predecrement.
                return MemoryOperand.PreDecrement(dataWidth, AddressRegister(operandBits, 0));
            case 5: // Address register indirect with displacement.
                offset = new Constant(PrimitiveType.Int16, rdr.ReadBeInt16());
                return MemoryOperand.Indirect(dataWidth, AddressRegister(operandBits, 0), offset);
            default: throw new NotImplementedException(string.Format("Address mode {0:X} not implemented.", addressMode));
            }
        }

        private static MachineRegister AddressRegister(ushort opcode, int bitOffset)
        {
            return Registers.GetRegister(8 + ((opcode >> bitOffset) & 0x7));
        }

        private static RegisterOperand DataRegisterOperand(ushort opcode, int bitOffset)
        {
            return new RegisterOperand(Registers.GetRegister((opcode >> bitOffset) & 0x7));
        }

        private static ImmediateOperand SignedImmediateByte(ushort opcode, int bitOffset, int mask)
        {
            return new ImmediateOperand(new Constant(PrimitiveType.SByte, (opcode >> bitOffset) & mask));
        }
    }
}
