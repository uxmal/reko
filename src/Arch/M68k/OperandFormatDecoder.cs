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
using System.Text;
using M68kAddressOperand = Decompiler.Arch.M68k.AddressOperand;

namespace Decompiler.Arch.M68k
{
    public class OperandFormatDecoder
    {
        ushort opcode;
        string args;
        int i;

        public OperandFormatDecoder(ushort opcode, string args, int i)
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
            Address addr;
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
                addr = rdr.Address;
                int offset = opcode & 0xFF;
                if (offset == 0xFF)
                    offset = rdr.ReadBeInt32();
                else if (offset == 0x00)
                    offset = rdr.ReadBeInt16();
                return new M68kAddressOperand(addr + offset);
            case 'M':
                return new RegisterSetOperand(rdr.ReadBeUInt16());
            case 'q':
                return GetQuickImmediate(GetOpcodeOffset(args[i++]), 7, 8, PrimitiveType.SByte);
            case 'Q':
                return GetQuickImmediate(GetOpcodeOffset(args[i++]), 0xFF, 0, PrimitiveType.SByte);
            case 'R': // relative 
                addr = rdr.Address;
                int relative = 0;
                switch (args[i++])
                {
                case 'w': relative = rdr.ReadBeInt16(); break;
                case 'l': relative = rdr.ReadBeInt32(); break;
                default: throw new NotImplementedException();
                }
                return new M68kAddressOperand(addr + relative);
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
            return new ImmediateOperand(Constant.Create(dataWidth, v));
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

        private MachineOperand ParseOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            opcode >>= bitOffset;
            byte operandBits = (byte)(opcode & 7);
            byte addressMode = (byte)((opcode >> 3) & 7);
            return ParseOperandInner(addressMode, operandBits, dataWidth, rdr);
        }

        private MachineOperand ParseSwappedOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            opcode >>= bitOffset;
            byte addressMode = (byte)(opcode & 7);
            byte operandBits = (byte)((opcode >> 3) & 7);
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

        public static PrimitiveType GetSizeType(ushort opcode, char c, PrimitiveType dataWidth)
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


        private MachineOperand ParseOperandInner(byte addressMode, byte operandBits, PrimitiveType dataWidth, ImageReader rdr)
        {
            Constant offset;
            string mode;
            switch (addressMode)
            {
            case 0: // Data register direct.
                return DataRegisterOperand(operandBits, 0);
            case 1: // Address register direct
                return new RegisterOperand(AddressRegister(operandBits, 0));
            case 2:  // Address register indirect
                return MemoryOperand.Indirect(AddressRegister(operandBits, 0));
            case 3:  // Address register indirect with postincrement.
                return MemoryOperand.PostIncrement(AddressRegister(operandBits, 0));
            case 4:  // Address register indirect with predecrement.
                return MemoryOperand.PreDecrement(AddressRegister(operandBits, 0));
            case 5: // Address register indirect with displacement.
                offset = Constant.Int16(rdr.ReadBeInt16());
                return MemoryOperand.Indirect(AddressRegister(operandBits, 0), offset);
            case 6: // Address register indirect with index
                ushort extension = rdr.ReadBeUInt16();
                if (EXT_INDEX_SCALE(extension) != 0)
                    throw new FormatException("Illegal address mode.");
                if (EXT_FULL(extension))
                {
                    if (M68kDisassembler.EXT_EFFECTIVE_ZERO(extension))
                    {
                        return new ImmediateOperand(Constant.Zero(dataWidth));
                    }

                    RegisterStorage base_reg = null;
                    RegisterStorage index_reg = null;
                    PrimitiveType index_reg_width = null;
                    int index_scale = 1;
                    var b= EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? rdr.ReadBeUInt32() : rdr.ReadBeUInt16()) : 0;
                    Constant @base = null;
                    if (b != 0)
                    {
                        if (EXT_BASE_DISPLACEMENT_LONG(extension))
                        {
                            @base = Constant.Int32((int) b);
                        }
                        else
                        {
                            @base = Constant.Int16((short) b);
                        }
                    }

                    var o = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? rdr.ReadBeUInt32() : rdr.ReadBeUInt16()) : 0;
                    Constant outer = null;
                    if (o != 0)
                        outer = Constant.Int16((short)o);
                    if (EXT_BASE_REGISTER_PRESENT(extension))
                            base_reg = Registers.AddressRegister(opcode & 7);
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg =  EXT_INDEX_AR(extension) 
                            ? Registers.AddressRegister((int)EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int)EXT_INDEX_REGISTER(extension));
                        index_reg_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Word16;
                        if (EXT_INDEX_SCALE(extension) != 0)
                            index_scale = 1 << EXT_INDEX_SCALE(extension);
                    }
                    bool preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    bool postindex = (extension & 7) > 4;
                    return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_reg_width, index_scale, preindex, postindex);
                }
                if (EXT_8BIT_DISPLACEMENT(extension) == 0)
                    mode = string.Format("(A{0},{1}{2}.{3}", opcode& 7, EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                else
                    mode = string.Format("({0},A{1},{2}{3}.{4}", M68kDisassembler.make_signed_hex_str_8(extension), opcode & 7, EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                if (EXT_INDEX_SCALE(extension) != 0)
                    mode += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                mode += ")";
                throw new NotImplementedException(string.Format("Address mode {0:X} not implemented.", addressMode));
                break;
            case 7:
                switch (operandBits)
                {
                case 0: // Absolute short address
                    return new AddressOperand(rdr.ReadBeUInt16());
                case 1: // Absolute long address
                    return new AddressOperand(rdr.ReadBeUInt32());
                case 2: // Program counter with displacement
                    return new MemoryOperand(Registers.pc, Constant.Int16(rdr.ReadBeInt16()));
                //g_helper_str = string.Format("; (${0})", (make_int_16(temp_value) + g_cpu_pc - 2) & 0xffffffff);
                //break;
                case 3:
                    throw new NotImplementedException();
                    //// Program counter with index
                    //ushort extension = rdr.ReadBeUInt16();

                    //if ((g_cpu_type & M68010_LESS) != 0 && EXT_INDEX_SCALE(extension) != 0)
                    //{
                    //    throw new NotSupportedException("Invalid address mode.");
                    //}

                    //if (EXT_FULL(extension))
                    //{
                    //    if ((g_cpu_type & M68010_LESS) != 0)
                    //    {
                    //        throw new NotSupportedException("Invalid address mode.");
                    //    }

                    //    if (EXT_EFFECTIVE_ZERO(extension))
                    //    {
                    //        mode = "0";
                    //        break;
                    //    }
                    //    @base = EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    //    outer = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    //    if (EXT_BASE_REGISTER_PRESENT(extension))
                    //        base_reg = "PC";
                    //    else
                    //        base_reg = "";
                    //    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    //    {
                    //        index_reg = string.Format("{0}{1}.{2}", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                    //        if (EXT_INDEX_SCALE(extension) != 0)
                    //            index_reg += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                    //    }
                    //    else
                    //        index_reg = "";
                    //    preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    //    postindex = (extension & 7) > 4;

                    //    mode = "(";
                    //    if (preindex || postindex)
                    //        mode += "[";
                    //    if (@base != 0)
                    //    {
                    //        mode += make_signed_hex_str_16(@base);
                    //        comma = true;
                    //    }
                    //    if (base_reg != "")
                    //    {
                    //        if (comma)
                    //            mode += ",";
                    //        mode += base_reg;
                    //        comma = true;
                    //    }
                    //    if (postindex)
                    //    {
                    //        mode += "]";
                    //        comma = true;
                    //    }
                    //    if (index_reg != "")
                    //    {
                    //        if (comma)
                    //            mode += ",";
                    //        mode += index_reg;
                    //        comma = true;
                    //    }
                    //    if (preindex)
                    //    {
                    //        mode += "]";
                    //        comma = true;
                    //    }
                    //    if (outer != 0)
                    //    {
                    //        if (comma)
                    //            mode += ",";
                    //        mode += make_signed_hex_str_16(outer);
                    //    }
                    //    mode += ")";
                    //    break;
                    //}

                    //if (EXT_8BIT_DISPLACEMENT(extension) == 0)
                    //    mode = string.Format("(PC,{0}%d.%c", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                    //else
                    //    mode = string.Format("({0},PC,%c%d.%c", make_signed_hex_str_8(extension), EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                    //if (EXT_INDEX_SCALE(extension) != 0)
                    //    mode += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                    //mode += ")";
                    //break;
                case 4:
                    /* Immediate */
                    if (dataWidth.Size == 1)        // don't want the instruction stream to get misaligned!
                        rdr.ReadByte();
                    return new ImmediateOperand(rdr.ReadBe(dataWidth));
                default:
                    throw new NotImplementedException(string.Format("Address mode {0}:{1} not implemented.", addressMode, operandBits));
                }
            default: 
                throw new NotImplementedException(string.Format("Address mode {0:X} not implemented.", addressMode));
            }
        }

                /* Extension word formats */
        private static uint EXT_8BIT_DISPLACEMENT(uint A) { return ((A) & 0xff); }
        internal static bool EXT_FULL(uint A) { return M68kDisassembler.BIT_8(A); }
        internal static bool EXT_EFFECTIVE_ZERO(uint A) { return (((A) & 0xe4) == 0xc4 || ((A) & 0xe2) == 0xc0); }
        private static bool EXT_BASE_REGISTER_PRESENT(uint A) { return !(M68kDisassembler.BIT_7(A)); }
        private static bool EXT_INDEX_REGISTER_PRESENT(uint A) { return !(M68kDisassembler.BIT_6(A)); }
        private static uint EXT_INDEX_REGISTER(uint A) { return (((A) >> 12) & 7); }
        private static bool EXT_INDEX_PRE_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && (A & 3) != 0); }
        private static bool EXT_INDEX_PRE(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) < 4 && ((A) & 7) != 0); }
        private static bool EXT_INDEX_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) > 4); }
        internal static int EXT_INDEX_SCALE(uint A) { return (int)(((A) >> 9) & 3); }
        private static bool EXT_INDEX_LONG(uint A) { return M68kDisassembler.BIT_B(A); }
        private static bool EXT_INDEX_AR(uint A) { return M68kDisassembler.BIT_F(A); }
        private static bool EXT_BASE_DISPLACEMENT_PRESENT(uint A) { return (((A) & 0x30) > 0x10); }
        private static bool EXT_BASE_DISPLACEMENT_WORD(uint A) { return (((A) & 0x30) == 0x20); }
        private static bool EXT_BASE_DISPLACEMENT_LONG(uint A) { return (((A) & 0x30) == 0x30); }
        private static bool EXT_OUTER_DISPLACEMENT_PRESENT(uint A) { return (((A) & 3) > 1 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_WORD(uint A) { return (((A) & 3) == 2 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_LONG(uint A) { return (((A) & 3) == 3 && ((A) & 0x47) < 0x44); }


        private static RegisterStorage AddressRegister(ushort opcode, int bitOffset)
        {
            return Registers.GetRegister(8 + ((opcode >> bitOffset) & 0x7));
        }

        private static RegisterOperand DataRegisterOperand(ushort opcode, int bitOffset)
        {
            return new RegisterOperand(Registers.GetRegister((opcode >> bitOffset) & 0x7));
        }

        private static ImmediateOperand SignedImmediateByte(ushort opcode, int bitOffset, int mask)
        {
            return new ImmediateOperand(Constant.Create(PrimitiveType.SByte, (opcode >> bitOffset) & mask));
        }
    }
}
