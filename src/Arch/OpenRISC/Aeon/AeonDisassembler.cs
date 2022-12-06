#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Aeon
{
    using Decoder = Decoder<AeonDisassembler, Mnemonic, AeonInstruction>;
    using Mutator = Mutator<AeonDisassembler>;

    // http://linux-chenxing.org/cpu/aeon.html
    public class AeonDisassembler : DisassemblerBase<AeonInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly AeonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public AeonDisassembler(AeonArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }


        public override AeonInstruction CreateInvalidInstruction()
        {
            return new AeonInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid
            };
        }

        public override AeonInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            // Instructions are 16, 24 or 32 bits.
            if (!rdr.TryReadBeUInt16(out ushort us))
                return null;
            var instr = rootDecoder.Decode(us, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override AeonInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new AeonInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
        }

        public override AeonInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("AeonDis", addr, rdr, message);
            return new AeonInstruction
            {
                //$TODO: while still experimental, assume NYI instructions are valid.
                InstructionClass = InstrClass.Linear,
                //InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
            };
        }

        private static Mutator RegisterFromField(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                return true;
            };
        }
        private static readonly Mutator R0 = RegisterFromField(0);
        private static readonly Mutator R3 = RegisterFromField(3);
        private static readonly Mutator R5 = RegisterFromField(5);
        private static readonly Mutator R8 = RegisterFromField(8);
        private static readonly Mutator R10 = RegisterFromField(10);
        private static readonly Mutator R13 = RegisterFromField(13);
        private static readonly Mutator R15 = RegisterFromField(15);
        private static readonly Mutator R16 = RegisterFromField(16);
        private static readonly Mutator R21 = RegisterFromField(21);

        private static Mutator UnsignedImmediate(int bitPos, int bitlength)
        {
            var field = new Bitfield(bitPos, bitlength);
            return (u, d) =>
            {
                var uImm = field.Read(u);
                d.ops.Add(ImmediateOperand.Word32(uImm));
                return true;
            };
        }
        private static readonly Mutator uimm0_5 = UnsignedImmediate(0, 5);
        private static readonly Mutator uimm0_8 = UnsignedImmediate(0, 8);
        private static readonly Mutator uimm0_10 = UnsignedImmediate(0, 10);
        private static readonly Mutator uimm0_13 = UnsignedImmediate(0, 13);
        private static readonly Mutator uimm0_16 = UnsignedImmediate(0, 16);
        private static readonly Mutator uimm3_5 = UnsignedImmediate(3, 5);
        private static readonly Mutator uimm4_12 = UnsignedImmediate(4, 12);
        private static readonly Mutator uimm5_8 = UnsignedImmediate(5, 8);
        private static readonly Mutator uimm5_16 = UnsignedImmediate(5, 16);
        private static readonly Mutator uimm8_5 = UnsignedImmediate(8, 5);
        private static readonly Mutator uimm10_3 = UnsignedImmediate(10, 3);


        /// <summary>
        /// Memory access with signed offset
        /// </summary>
        /// <returns></returns>
        private static Mutator Ms(
            int bitposBaseReg, 
            int bitposOffset,
            int lengthOffset,
            int offsetScale,
            PrimitiveType dt)
        {
            var baseField = new Bitfield(bitposBaseReg, 5);
            var offsetField = new Bitfield(bitposOffset, lengthOffset);
            return (u, d) =>
            {
                var ireg = baseField.Read(u);
                var baseReg = Registers.GpRegisters[ireg];
                var offset = offsetField.ReadSigned(u) << offsetScale;
                d.ops.Add(new MemoryOperand(dt) { Base = baseReg, Offset = offset });
                return true;
            };
        }

        private static Mutator DisplacementFromPc(int bitpos, int length)
        {
            var displacementField = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var displacement = displacementField.ReadSigned(u);
                var target = d.addr + displacement; //$REVIEW: and a +4?
                d.ops.Add(AddressOperand.Create(target));
                return true;
            };
        }
        private static readonly Mutator disp0_26 = DisplacementFromPc(0, 26);
        private static readonly Mutator disp2_8 = DisplacementFromPc(2, 8);
        private static readonly Mutator disp2_16 = DisplacementFromPc(2, 16);
        private static readonly Mutator disp2_24 = DisplacementFromPc(2, 24);

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<AeonDisassembler, Mnemonic, AeonInstruction>(message);
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator[] mutators)
        {
            return new InstrDecoder<AeonDisassembler, Mnemonic, AeonInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator[] mutators)
        {
            return new InstrDecoder<AeonDisassembler, Mnemonic, AeonInstruction>(iclass, mnemonic, mutators);
        }

        private class D26BitDecoder : Decoder
        {
            private readonly Decoder decoder;

            public D26BitDecoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override AeonInstruction Decode(uint uInstr16, AeonDisassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                uint uInstr32 = (uInstr16 << 8) | b;
                return decoder.Decode(uInstr32, dasm);
            }
        }

        private class D32BitDecoder : Decoder
        {
            private readonly Decoder decoder;

            public D32BitDecoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override AeonInstruction Decode(uint uInstr16, AeonDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort us))
                    return dasm.CreateInvalidInstruction();
                uint uInstr32 = (uInstr16 << 16) | us;
                return decoder.Decode(uInstr32, dasm);
            }
        }

        static AeonDisassembler()
        {
            var decoder24 = Create24bitInstructionDecoder();
            var decoder16 = Create16bitInstructionDecoder();
            var decoder32 = Create32bitInstructionDecoder();
            rootDecoder =  Mask(13, 3, "Aeon architectures",
                decoder24,
                decoder24,
                decoder24,
                decoder24,

                decoder16,
                decoder32,
                decoder32,
                decoder32);
        }

        private static Decoder Create32bitInstructionDecoder()
        {
            var decoder0 = Sparse(0, 4, "  opc=0", Nyi("0"),
                (0x1, Instr(Mnemonic.l_movhi, R21, uimm5_16)),               // chenxing(mod), disasm
                (0xD, Instr(Mnemonic.l_mtspr, R16, R21, uimm4_12)),          // chenxing
                (0xF, Instr(Mnemonic.l_mfspr, R21, R16, uimm4_12)));         // chenxing
            var decoderA = Mask(0, 2, "  A",
                Nyi("0b00"),
                Nyi("0b01"),
                Nyi("0b10"),
                Instr(Mnemonic.l_j, InstrClass.Transfer, disp2_24));            // chenxing
            var decoderB = Mask(0, 2, "  B",
                Instr(Mnemonic.l_sw, Ms(16, 2, 14, 0, PrimitiveType.Word32), R21),          // chenxing, backtrace
                Instr(Mnemonic.l_sw__, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),        // guess
                Instr(Mnemonic.l_lwz__, R21, Ms(16, 2, 14, 2, PrimitiveType.Word32)),  // guess
                Nyi("0b11"));
            var decoderD = Select(Bf((5, 11), (21, 5)), u => u == 0,
                Sparse(0, 3, "  opc=D", Nyi("D"),
                    (0b001, Instr(Mnemonic.l_invalidate_line, Ms(16, 6, 4, 0, PrimitiveType.Word32))), //$REVIEW j?  // chenxing
                    (0b111, Instr(Mnemonic.l_invalidate_line, Ms(16, 6, 4, 0, PrimitiveType.Word32))), //$REVIEW j?  // disasm
                    (0b101, Instr(Mnemonic.l_syncwritebuffer))),                                                     // disasm
                Nyi("D, non-zero bits"));
            var decoder = Mask(26, 4, "  32-bit instr",
                decoder0,
                Instr(Mnemonic.l_andi, R15, R10, uimm0_5),           // chenxing
                Instr(Mnemonic.l_ori, R15, R10, uimm0_5),           // chenxing
                Nyi("0b0011"),

                Nyi("0b0100"),
                // XXX: n is probably wrong
                Instr(Mnemonic.l_bf, disp0_26),                    // disasm, guess
                Nyi("0b0110"),
                Nyi("0b0111"),

                Nyi("0b1000"),
                // XXX: n is probably wrong
                Instr(Mnemonic.l_jal__, InstrClass.Transfer | InstrClass.Call, disp0_26),       // guess
                decoderA,
                decoderB,

                Nyi("0b1100"),
                decoderD,
                Nyi("0b1110"),
                //$REVIEW: signed or unsigned immediate?
                Instr(Mnemonic.l_addi, R21,R16,uimm0_16));           // chenxing, backtrace


            return new D32BitDecoder(decoder);
        }

        /*
        */
        // Instr(Mnemonic.l_movhi", 4, "110000dddddkkkkkkkkkkkkkkkk00001", "r%d, %k"),

        /*

        // 24-bit / 3-byte / "BN"?
        Instr(Mnemonic.l_nop",   3, "000000000000000000000000"),                    # chenxing
        // XXX: top bits of k may be a register, and this is an or?
        Instr(Mnemonic.l_movhi?",3, "000001dddddkkkkkkkkkkkkk", "r%d, %k"),
        Instr(Mnemonic.l_lhz",   3, "000010dddddaaaaa00000001", "r%d, 0(r%a)"),     # chenxing
        // XXX: i might be left shifted by 2 bits
        Instr(Mnemonic.l_sw",    3, "000011bbbbbaaaaaiiiiii00", "%i(r%a), r%b", signed={'i'}),# chenxing, backtrace
        // XXX: assuming this is l.lwz and not l.lws
        Instr(Mnemonic.l_lwz?",  3, "000011dddddaaaaaiiiiii10", "r%d, %i(r%a)", signed={'i'}),# guess
        // XXX: assuming this is l.lwz and not l.lws
        Instr(Mnemonic.l_lwz?",  3, "000100dddddaaaaaiiiiii00", "r%d, %i(r%a)", signed={'i'}),# guess
        Instr(Mnemonic.l_sw?",   3, "000110bbbbbaaaaaiiiiii00", "%i(r%a), r%b", signed={'i'}),# guess
        Instr(Mnemonic.l_addi",  3, "000111dddddaaaaakkkkkkkk", "r%d, r%a, %k", signed={'k'}),# chenxing, backtrace
        Instr(Mnemonic.l_bf",    3, "001000nnnnnnnnnnnnnnnn01", "%n", signed={'n'}),# chenxing(mod), disasm

        // branch if reg == imm ? XXX: signed/unsigned?
        Instr('?beqi?",  3, "001000aaaaaiiinnnnnnnn00", "r%a, %i, %n", signed={'n'}),# wild guess
        // branch if reg <= imm XXX: signed/unsigned?
        Instr('?ble?i?", 3, "001001aaaaaiiinnnnnnnn01", "r%a, %i, %n", signed={'n'}),# wild guess
        Instr(Mnemonic.l_movhi", 3, "001101100000000000000001", "r1, ???"),         # chenxing
        Instr(Mnemonic.l_mul",   3, "010000dddddaaaaabbbbb011", "r%d, r%a, r%b"),   # disasm
        Instr(Mnemonic.l_and",   3, "010001dddddaaaaabbbbb100", "r%d, r%a, r%b"),   # chenxing
        Instr(Mnemonic.l_or?",   3, "010001dddddaaaaabbbbb101", "r%d, r%a, r%b"),   # guess
        Instr(Mnemonic.l_srli?", 3, "010011dddddaaaaalllll001", "r%d, r%a, %l"),    # guess
        Instr(Mnemonic.l_ori",   3, "010100dddddaaaaakkkkkkkk", "r%d, r%a, %k"),    # chenxing
        Instr(Mnemonic.l_andi",  3, "010101dddddaaaaakkkkkkkk", "r%d, r%a, %k"),    # guess
        Instr(Mnemonic.l_sfgtui",3, "010111aaaaaiiiiiiii11011", "r%a, %i", signed={'i'}), # disasm
        Instr('?entri?", 3, "010111xxxxyyyyyyyyy11000", "??? %x, %y"),      # backtrace
        Instr(Mnemonic.l_sfeqi", 3, "010111aaaaaiiiii00000001", "r%a, %i"),         # chenxing
        // XXX: might only move (low) 16 bits?
        Instr('?mov?",   3, "010111dddddaaaaa00000100", "r%d, r%a"),        # guess
        Instr(Mnemonic.l_sfne",  3, "010111aaaaabbbbb00001101", "r%a, r%b"),        # chenxing
        Instr(Mnemonic.l_sfgeu", 3, "010111bbbbbaaaaa00010111", "r%a, r%b"),        # chenxing

        // 32-bit / 4-byte / "BG"?
        Instr(Mnemonic.l_movhi", 4, "110000dddddkkkkkkkkkkkkkkkk00001", "r%d, %k"),                # chenxing(mod), disasm
        Instr(Mnemonic.l_mtspr", 4, "110000bbbbbaaaaakkkkkkkkkkkk1101", "r%a, r%b, %k"),           # chenxing
        Instr(Mnemonic.l_mfspr", 4, "110000dddddaaaaakkkkkkkkkkkk1111", "r%d, r%a, %k"),           # chenxing
        Instr(Mnemonic.l_andi",  4, "110001dddddaaaaakkkkkkkkkkkkkkkk", "r%d, r%a, %k"),           # chenxing
        Instr(Mnemonic.l_ori",   4, "110010dddddaaaaakkkkkkkkkkkkkkkk", "r%d, r%a, %k"),           # chenxing
        // XXX: n is probably wrong
        Instr(Mnemonic.l_bf",    4, "11010100nnnnnnnnnnnnnnnnnnnnnnnn", "%n", signed={'n'}),       # disasm, guess
        // XXX: n is probably wrong
        Instr(Mnemonic.l_jal?",  4, "111001nnnnnnnnnnnnnnnnnnnnnnnnnn", "%n", signed={'n'}),       # guess
        Instr(Mnemonic.l_j",     4, "111010nnnnnnnnnnnnnnnnnnnnnnnn11", "%n", signed={'n'}),       # chenxing
        Instr(Mnemonic.l_sw",    4, "111011bbbbbaaaaaiiiiiiiiiiiiii00", "%i(r%a), r%b", signed={'i'}), # chenxing, backtrace
        Instr(Mnemonic.l_sw?",   4, "111011bbbbbaaaaaiiiiiiiiiiiiii01", "(%i << 2)(r%a), r%b", signed={'i'}), # guess
        Instr(Mnemonic.l_lwz?",  4, "111011dddddaaaaaiiiiiiiiiiiiii10", "r%d, %i(r%a)", signed={'i'}), # guess
        Instr(Mnemonic.l_addi",  4, "111111dddddaaaaakkkkkkkkkkkkkkkk", "r%d, r%a, %k"),           # chenxing, backtrace
        
        Instr(Mnemonic.l_invalidate_line", 4, "11110100000aaaaa00000000000j0001", "0(r%a), %j"),   # chenxing
        Instr(Mnemonic.l_invalidate_line", 4, "11110100000aaaaa00000000001j0111", "0(r%a), %j"),   # disasm
        Instr(Mnemonic.l_syncwritebuffer", 4, "11110100000000000000000000000101"),                 # disasm         
         
         */

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create16bitInstructionDecoder()
        {
            var decoder0_0 = Sparse(0, 10, "  decoder 0_0",
                    Instr(Mnemonic.Nyi, uimm0_10),
                (0, Instr(Mnemonic.l_nop)),
                (1, Instr(Mnemonic.bt_trap)));  //$REVIEW: don't know how to decode // disasm

            return Mask(10, 3, "  16-bit",
                decoder0_0,
                Instr(Mnemonic.l_jr, InstrClass.Transfer, R5, R0),   // disasm
                Instr(Mnemonic.mov__, R5, R0),                       // wild guess
                Instr(Mnemonic.l_add__, R5, R0),                     // guess

                Instr(Mnemonic.l_j, uimm0_10),                        // chenxing
                Instr(Mnemonic.Nyi, R5, R0),
                Instr(Mnemonic.l_andi__, R5, uimm0_5),                // diasm, guess, may be movi
                Instr(Mnemonic.l_addi__, R5, uimm0_5));               // backtrace, guess
        }

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create24bitInstructionDecoder()
        {
            var nyi = Nyi("24-bit instr");
            var decode00 = Select(u => u == 0,
                Instr(Mnemonic.l_nop, InstrClass.Linear | InstrClass.Zero | InstrClass.Padding),
                nyi);
            var decode03 = Mask(0, 2, "  3",
                // XXX: i might be left shifted by 2 bits
                Instr(Mnemonic.l_sw, Ms(8, 2, 6, 2, PrimitiveType.Word32), R13),    // 000 011 bbbbbaaaaaiiiiii00   // chenxing, backtrace
                Nyi("0b01"),
                // XXX: assuming this is l.lwz and not l.lws
                Instr(Mnemonic.l_lwz__, R13, Ms(8, 2, 6, 2, PrimitiveType.Word32)), // 000 011 dddddaaaaaiiiiii10   // guess
                Nyi("0b11"));
            var decode08 = Mask(0, 2, "  8",
                // branch if reg == imm ? XXX: signed/unsigned?
                Instr(Mnemonic.beqi__, R13, uimm10_3, disp2_8),              // wild guess
                Instr(Mnemonic.l_bf, disp2_16),                             // chenxing(mod), disasm
                Nyi("10"),
                Nyi("11"));
            var decode09 = Mask(0, 2, "  9",
                Nyi("00"),
                Instr(Mnemonic.ble__i__,  R13, uimm10_3, disp2_8), // wild guess
                Nyi("10"),
                Nyi("11"));

            var decode10 = Sparse(0, 3, "  10", Nyi("10"),
                (0b011, Instr(Mnemonic.l_mul, R13, R8, R3)));                   // disasm

            var decode11 = Sparse(0, 3, "  11", Nyi("11"),
                (0b100, Instr(Mnemonic.l_and, R13, R8, R3)),   // chenxing
                (0b101, Instr(Mnemonic.l_or__, R13, R8, R3)));   // guess

            var decode13 = Sparse(0, 3, "  13", Nyi("13"),
                (0b001, Instr(Mnemonic.l_srli__, R13, R8, uimm3_5)));       // guess

            var decode17 = Sparse(0, 5, "  17", Nyi("17"),
                (0b11011, Instr(Mnemonic.l_sfgtui, R13, uimm5_8)),          // disasm
                (0b11000, Instr(Mnemonic.entri__, R13, uimm5_8)),           // backtrace
                (0b00001, Instr(Mnemonic.l_sfeqi, R13, uimm8_5)),           // chenxing
                // XXX: might only move (low) 16 bits?
                (0b00100, Instr(Mnemonic.mov__, R13,R8)),                   // guess
                (0b01101, Instr(Mnemonic.l_sfne,  R13,R8)),                 // chenxing
                (0b10111, Instr(Mnemonic.l_sfgeu, R13,R8)));                // chenxing

            return new D26BitDecoder(Mask(18, 5, "  24-bit instr",  // bit 23 is always 0
                decode00,
                // XXX: top bits of k may be a register, and this is an or?
                Instr(Mnemonic.l_movhi__, R13, uimm0_13),
                Instr(Mnemonic.l_lhz, R13, Ms(8, 2, 6, 1, PrimitiveType.UInt16)), // chenxing
                decode03,

                // XXX: assuming this is l.lwz and not l.lws
                Instr(Mnemonic.l_lwz__, R13, Ms(8, 2, 6, 2, PrimitiveType.Word32)),    // guess
                Nyi("0b00101"),
                Instr(Mnemonic.l_sw__, Ms(8, 2, 6, 2, PrimitiveType.Word32), R13), // guess
                Instr(Mnemonic.l_addi, R13, R8, uimm0_8), //$REVIEW: signed?           // chenxing, backtrace

                decode08,
                // branch if reg <= imm XXX: signed/unsigned?
                decode09,
                Nyi("0b01010"),
                Nyi("0b01011"),

                Nyi("0b01100"),
                Instr(Mnemonic.l_movhi__, UnsignedImmediate(0, 18)),         // chenxing
                Nyi("0b01110"),
                Nyi("0b01111"),

                decode10,
                decode11,
                Nyi("0b10010"),
                decode13,

                Instr(Mnemonic.l_ori, R13, R8, uimm0_8),    // chenxing
                Instr(Mnemonic.l_andi, R13, R8, uimm0_8),    // guess
                Nyi("0b10110"),
                decode17,

                Nyi("0b11000"),
                Nyi("0b11001"),
                Nyi("0b11010"),
                Nyi("0b11011"),

                Nyi("0b11100"),
                Nyi("0b11101"),
                Nyi("0b11110"),
                Nyi("0b11111")));
        }
    }
}
