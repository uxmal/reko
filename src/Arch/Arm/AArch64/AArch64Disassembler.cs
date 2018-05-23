#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public class AArch64Disassembler : DisassemblerBase<Arm64Instruction>
    {
        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;

        private Arm64Architecture arch;
        private EndianImageReader rdr;
        private Address addr;

        public AArch64Disassembler(Arm64Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Arm64Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out var wInstr))
                return null;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }

        public abstract class Decoder
        {
            public abstract Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm);

            public static uint bitmask(uint u, int shift, uint mask)
            {
                return (u >> shift) & mask;
            }

            public static bool bit(uint u, int shift)
            {
                return ((u >> shift) & 1) != 0;
            }
        }

        public class MaskDecoder : Decoder
        {
            private int shift;
            private uint mask;
            private Decoder[] decoders;

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
            {
                this.shift = shift;
                this.mask = mask;
                Debug.Assert(decoders.Length == mask + 1);
                this.decoders = decoders;
            }

            public override Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = (wInstr >> shift) & mask;
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                var shMask = this.mask << shift;
                var hibit = 0x80000000u;
                var sb = new StringBuilder();
                for (int i = 0; i < 32; ++i)
                {
                    if ((shMask & hibit) != 0)
                    {
                        sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                    }
                    else
                    {
                        sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                    }
                    shMask <<= 1;
                    wInstr <<= 1;
                }
                Debug.Print(sb.ToString());
            }
        }

        public class SparseMaskDecoder : Decoder
        {
            private int shift;
            private uint mask;
            private Dictionary<uint, Decoder> decoders;
            private Decoder @default;

            public SparseMaskDecoder(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
            {
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
                this.@default = @default;
            }

            public override Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                throw new NotImplementedException($"An AArch64 decoder for the instruction {wInstr:X} ({message}) has not been implemented.");
            }
        }

        class CustomDecoder : Decoder
        {
            private Func<uint, AArch64Disassembler, Decoder> decode;

            public CustomDecoder(Func<uint, AArch64Disassembler, Decoder> decode)
            {
                this.decode = decode;
            }

            public override Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                return decode(wInstr, dasm).Decode(wInstr, dasm);
            }
        }


        private static NyiDecoder nyi(string str)
        {
            return new NyiDecoder(str);
        }

        private class InstrDecoder : Decoder
        {
            private Opcode opcode;
            private string format;

            public InstrDecoder(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override Arm64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        static AArch64Disassembler()
        {
            invalid = new InstrDecoder(Opcode.Invalid, "");

            Decoder LoadsAndStores;
            {
                LoadsAndStores = new MaskDecoder(31, 1,
                    new MaskDecoder(28, 3,          // op0 = 0 
                        new MaskDecoder(26, 1,      // op0 = 0 op1 = 0
                            new MaskDecoder(23, 3,  // op0 = 0 op1 = 0 op2 = 0
                                nyi("LoadStoreExclusive"),
                                nyi("LoadStoreExclusive"),
                                invalid,
                                invalid),
                            new MaskDecoder(23, 3,  // op0 = 0 op1 = 0 op2 = 1
                                nyi("AdvancedSimdLdStMultiple"),
                                nyi("AdvancedSimdLdStMultiple"),
                                invalid,
                                invalid)),
                        new MaskDecoder(23, 3,      // op0 = 0, op1 = 1
                            nyi("LoadRegLit"),
                            nyi("LoadRegLit"),
                            invalid,
                            invalid),
                        new MaskDecoder(23, 3,      // op0 = 0, op1 = 2
                            nyi("LdStNoallocatePair"),
                            nyi("LdStRegPairPost"),
                            nyi("LdStRegPairOffset"),
                            nyi("LdStRegPairPre")),
                        nyi(" op0 = 0, op1 = 3")),
                    invalid);
            }

            rootDecoder = new MaskDecoder(25, 0x0F,
                invalid,
                invalid,
                invalid,
                invalid,

                LoadsAndStores,
                nyi("DataProcessingReg"),
                nyi("LoadsAndStores"),
                nyi("DataProcessingScalarFpAdvancedSimd"),
                
                nyi("DataProcessingImm"),
                nyi("DataProcessingImm"),
                nyi("BranchesExceptionsSystem"),
                nyi("BranchesExceptionsSystem"),
                
                nyi("LoadsAndStores"),
                nyi("DataProcessingReg"),
                nyi("LoadsAndStores"),
                nyi("DataProcessingScalarFpAdvancedSimd"));
        }
    }
}
