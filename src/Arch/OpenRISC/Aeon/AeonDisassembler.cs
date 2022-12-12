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
using Reko.Core.Expressions;
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
            byte[] ReadInstructionBytes(Address addrInstr)
            {
                var r2 = rdr.Clone();
                int len = (int) (r2.Address - addrInstr);
                r2.Offset -= len;
                var bytes = r2.ReadBytes(len);
                return bytes;
            }
            var bytes = ReadInstructionBytes(addr);

//            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
//            testGenSvc?.ReportMissingDecoder("AeonDis", addr, rdr, message);
            return new AeonInstruction
            {
                //$TODO: while still experimental, assume NYI instructions are valid.
                InstructionClass = InstrClass.Linear,
                //InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
                Operands = new MachineOperand[]
                {
                    ImmediateOperand.UInt32((uint)bytes[0] >> 2)
                }
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

        private static Mutator UnsignedImmediate(int bitPos, int bitlength, PrimitiveType? dt = null)
        {
            dt ??= PrimitiveType.Word32;
            var field = new Bitfield(bitPos, bitlength);
            return (u, d) =>
            {
                var uImm = field.Read(u);
                d.ops.Add(ImmediateOperand.Create(Constant.Create(dt, uImm)));
                return true;
            };
        }
        private static readonly Mutator uimm0_2 = UnsignedImmediate(0, 2);
        private static readonly Mutator uimm0_3 = UnsignedImmediate(0, 3);
        private static readonly Mutator uimm0_5 = UnsignedImmediate(0, 5);
        private static readonly Mutator uimm0_8 = UnsignedImmediate(0, 8);
        private static readonly Mutator uimm0_8_16 = UnsignedImmediate(0, 8, PrimitiveType.UInt16);
        private static readonly Mutator uimm0_10 = UnsignedImmediate(0, 10);
        private static readonly Mutator uimm0_13 = UnsignedImmediate(0, 13);
        private static readonly Mutator uimm0_16_16 = UnsignedImmediate(0, 16, PrimitiveType.UInt16);
        private static readonly Mutator uimm1_4 = UnsignedImmediate(1, 4);
        private static readonly Mutator uimm3_5 = UnsignedImmediate(3, 5);
        private static readonly Mutator uimm3_13 = UnsignedImmediate(3, 13);
        private static readonly Mutator uimm4_1 = UnsignedImmediate(4, 1);
        private static readonly Mutator uimm4_12 = UnsignedImmediate(4, 12);
        private static readonly Mutator uimm5_5 = UnsignedImmediate(5, 5);
        private static readonly Mutator uimm5_8 = UnsignedImmediate(5, 8);
        private static readonly Mutator uimm5_9 = UnsignedImmediate(5, 9);
        private static readonly Mutator uimm5_16 = UnsignedImmediate(5, 16);
        private static readonly Mutator uimm8_5 = UnsignedImmediate(8, 5);
        private static readonly Mutator uimm10_3 = UnsignedImmediate(10, 3);
        private static readonly Mutator uimm10_6 = UnsignedImmediate(10, 6);
        private static readonly Mutator uimm14_4 = UnsignedImmediate(14, 4);
        private static readonly Mutator uimm16_5 = UnsignedImmediate(16, 5);
        private static readonly Mutator uimm18_6 = UnsignedImmediate(18, 6);
        private static readonly Mutator uimm26_6 = UnsignedImmediate(26, 6);

        private static Mutator SignedImmediate(int bitPos, int bitlength, PrimitiveType dt)
        {
            var field = new Bitfield(bitPos, bitlength);
            return (u, d) =>
            {
                var sImm = field.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Create(Constant.Create(dt, sImm)));
                return true;
            };
        }
        private static readonly Mutator simm0_5 = SignedImmediate(0, 5, PrimitiveType.Int16);
        private static readonly Mutator simm0_8 = SignedImmediate(0, 8, PrimitiveType.Int16);
        private static readonly Mutator simm0_16 = SignedImmediate(0, 16, PrimitiveType.Int16);

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
                var target = d.addr + displacement;
                d.ops.Add(AddressOperand.Create(target));
                return true;
            };
        }
        private static readonly Mutator disp0_26 = DisplacementFromPc(0, 26);
        private static readonly Mutator disp0_10 = DisplacementFromPc(0, 10);
        private static readonly Mutator disp0_18 = DisplacementFromPc(0, 18);
        private static readonly Mutator disp1_25 = DisplacementFromPc(1, 25);
        private static readonly Mutator disp2_8 = DisplacementFromPc(2, 8);
        private static readonly Mutator disp2_16 = DisplacementFromPc(2, 16);
        private static readonly Mutator disp2_24 = DisplacementFromPc(2, 24);
        private static readonly Mutator disp3_13 = DisplacementFromPc(3, 13);

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

        private class D24BitDecoder : Decoder
        {
            private readonly Decoder decoder;

            public D24BitDecoder(Decoder decoder)
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

                decoder16,      // 100
                decoder32,      // 101
                decoder32,
                decoder32);
        }

        private static Decoder Create32bitInstructionDecoder()
        {
            var nyi_3 = Instr(Mnemonic.Nyi, uimm26_6, R21, uimm16_5, uimm3_13);
            var nyi_3_imm_disp = Instr(Mnemonic.Nyi, uimm26_6, R21, uimm16_5, disp3_13);
            var nyi_3_reg_disp = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, disp3_13);
            var nyi_3_reg_disp_3 = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, disp3_13, uimm0_3);
            var nyi_5 = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, uimm5_16);

            var decoder110000 = Sparse(0, 4, "  opc=110000", nyi_5, //$REVIEW: maybe the sub-opcode is 5 bits?
                (0b0000, Instr(Mnemonic.bn_sfgeui__, R21, uimm5_16)),           // guess
                (0b0001, Instr(Mnemonic.bg_movhi, R21, uimm5_16)),              // chenxing(mod), disasm
                (0b0100, Instr(Mnemonic.bg_sfnei__, R21, uimm5_16)),            // guess
                (0b1101, Instr(Mnemonic.bg_mtspr, R16, R21, uimm4_12)),         // chenxing, disasm
                (0b1110, Instr(Mnemonic.bg_sfleui__, R21, uimm5_16)),           // guess
                (0b1111, Instr(Mnemonic.bg_mfspr, R21, R16, uimm4_12)));        // chenxing, disasm

            var decoder110100 = Sparse(0, 3, "  opc=110100", nyi_3_imm_disp, 
                (0b010, Instr(Mnemonic.bg_beqi__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13)),       // guess
                (0b101, Instr(Mnemonic.bg_bgtui__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13)),      // guess
                (0b110, Instr(Mnemonic.bg_bltsi__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13)));     // guess

            var decoder110101 = Sparse(0, 3, "  opc=110101", nyi_3_reg_disp_3,
                (0b001, Instr(Mnemonic.bg_bges__, InstrClass.ConditionalTransfer, R21, R16, disp3_13)),     // guess
                (0b010, Instr(Mnemonic.bg_beq__, InstrClass.ConditionalTransfer, R21, R16, disp3_13)),      // guess
                // $REVIEW: could displacement be larger? There are 5 bits left over
                (0b011, Instr(Mnemonic.bg_bf, InstrClass.ConditionalTransfer, disp3_13)),                   // disasm
                (0b101, Instr(Mnemonic.bg_bgeu__, InstrClass.ConditionalTransfer, R21, R16, disp3_13)),     // guess
                (0b110, Instr(Mnemonic.bg_bne__, InstrClass.ConditionalTransfer, R21, R16, disp3_13)));     // guess

            var decoder111001 = Mask(0, 1, "  opc=111001",
                Instr(Mnemonic.bg_jal, InstrClass.Transfer | InstrClass.Call, disp1_25), // guess
                Instr(Mnemonic.bg_j, InstrClass.Transfer, disp1_25));                    // guess

            var decoder111010 = Mask(0, 1, "  opc=111010",
                Nyi("0"),
                Instr(Mnemonic.bg_lhz__, R21, Ms(16, 1, 15, 1, PrimitiveType.Word16)));  // guess

            var decoder111011 = Mask(0, 2, "  opc=111011",
                Instr(Mnemonic.bg_sw, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),         // chenxing, backtrace
                Instr(Mnemonic.bg_sw__, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),       // guess
                Instr(Mnemonic.bg_lwz, R21, Ms(16, 2, 14, 2, PrimitiveType.Word32)),      // guess, disasm
                Instr(Mnemonic.bg_sh__, Ms(16, 1, 15, 1, PrimitiveType.Word16), R21));      // guess

            var instr_inv_line = Instr(Mnemonic.bg_invalidate_line, Ms(16, 6, 4, 0, PrimitiveType.Word32), uimm4_1);
            var decoder111101 = Select(Bf((6, 10), (21, 5)), u => u == 0,
                Sparse(0, 3, "  opc=111101", nyi_3,
                
                    // $REVIEW: what's the difference between these? what about bit 5?
                    (0b001, instr_inv_line),                                                               // chenxing
                    (0b101, Instr(Mnemonic.bg_syncwritebuffer)),                                            // disasm
                    (0b110, Instr(Mnemonic.bg_flush_line, Ms(16, 6, 4, 0, PrimitiveType.Word32), uimm4_1)), // disasm
                    (0b111, instr_inv_line)),                                                              // disasm
                Nyi("111101, non-zero bits"));

            var decoder = Mask(26, 5, "  32-bit instr",
                Nyi("0b100000"),
                Nyi("0b100001"),
                Nyi("0b100010"),
                Nyi("0b100011"),

                Nyi("0b100100"),
                Nyi("0b100101"),
                Nyi("0b100110"),
                Nyi("0b100111"),

                Nyi("0b101000"),
                Nyi("0b101001"),
                Nyi("0b101010"),
                Nyi("0b101011"),

                Nyi("0b101100"),
                Nyi("0b101101"),
                Nyi("0b101110"),
                Nyi("0b101111"),

                decoder110000,
                // opcode 110001
                Instr(Mnemonic.bg_andi, R21, R16, uimm0_16_16),           // chenxing
                // opcode 110010
                Instr(Mnemonic.bg_ori, R21, R16, uimm0_16_16),            // chenxing
                // opcode 110011
                Nyi("0b110011"),

                decoder110100,
                decoder110101,
                // opcode 110110
                Nyi("0b110110"),
                // opcode 110111
                Nyi("0b110111"),

                // opcode 111000
                Nyi("0b111000"),
                decoder111001,
                decoder111010,
                decoder111011,
                // opcode 111100
                Instr(Mnemonic.bg_lbz__, R21, Ms(16, 0, 16, 0, PrimitiveType.Byte)),    // guess
                decoder111101,
                // opcode 111110
                Instr(Mnemonic.bg_sb__, Ms(16, 0, 16, 0, PrimitiveType.Byte), R21),     // guess
                // opcode 111111
                //$REVIEW: signed or unsigned immediate?
                Instr(Mnemonic.bg_addi, R21, R16, simm0_16));                           // chenxing, backtrace

            return new D32BitDecoder(decoder);
        }

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create16bitInstructionDecoder()
        {
            var nyi_5 = Instr(Mnemonic.Nyi, uimm10_6, uimm5_5, uimm0_5);

            // when rD is r0, the instruction has a different function
            var decoder100000_special = Mask(0, 1, "  opc=100000",
                Instr(Mnemonic.bt_trap, uimm1_4),                   // disasm
                Instr(Mnemonic.bt_nop, uimm1_4));                   // disasm
            // opcode 100000
            var decoder100000 = Select((5, 5), u => u == 0,
                decoder100000_special,
                Instr(Mnemonic.bt_sw__, uimm10_6, R5, simm0_5));    // guess

            var decoder100001_sub0 = Select((5, 5), u => u == 0,
                Instr(Mnemonic.bt_rfe, InstrClass.Transfer | InstrClass.Return), // disasm
                nyi_5);
            // opcode 100001
            var decoder100001 = Sparse(0, 5, "  opc=100001", nyi_5,
                (0b00000, decoder100001_sub0),
                (0b01000, Instr(Mnemonic.bt_jalr__, InstrClass.Transfer, R5)),  // guess
                (0b01001, Instr(Mnemonic.bt_jr, InstrClass.Transfer, R5)));     // disasm

            return Mask(10, 3, "  16-bit",
                decoder100000,
                decoder100001,
                // opcode 100010
                Instr(Mnemonic.bt_mov__, R5, R0),                    // wild guess
                // opcode 100011
                Instr(Mnemonic.bt_add__, R5, R0),                    // guess

                // opcode 100100
                Instr(Mnemonic.bt_j, InstrClass.Transfer, disp0_10), // chenxing
                // opcode 100101
                Instr(Mnemonic.bt_movhi__, R5, uimm0_5),             // guess
                // opcode 100110
                Instr(Mnemonic.bt_movi__, R5, simm0_5),              // disasm, guess
                // opcode 100111
                Instr(Mnemonic.bt_addi__, R5, simm0_5));             // backtrace, guess
        }

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create24bitInstructionDecoder()
        {
            var nyi_2 = Instr(Mnemonic.Nyi, uimm18_6, R13, uimm10_3, disp2_8, uimm0_2);
            var nyi_3 = Instr(Mnemonic.Nyi, uimm18_6, R13, R8, R3, uimm0_3);
            var nyi_5 = Instr(Mnemonic.Nyi, uimm18_6, R13, R8, uimm0_5);

            var decode000000 = Select(u => u == 0,
                Instr(Mnemonic.bn_nop, InstrClass.Linear | InstrClass.Zero | InstrClass.Padding),
                nyi_5);

            var decode000011_bn_sh = Instr(Mnemonic.bn_sh__, Ms(8, 1, 7, 1, PrimitiveType.Word16), R13);
            var decode000011 = Mask(0, 2, "  3",
                Instr(Mnemonic.bn_sw, Ms(8, 2, 6, 2, PrimitiveType.Word32), R13),    // 000 011 bbbbbaaaaaiiiiii00   // chenxing, backtrace
                decode000011_bn_sh,
                Instr(Mnemonic.bn_lwz, R13, Ms(8, 2, 6, 2, PrimitiveType.Word32)), // 000 011 dddddaaaaaiiiiii10   // guess, disasm
                decode000011_bn_sh);
            
            var decode001000 = Mask(0, 2, "  8",
                // branch if reg == imm
                Instr(Mnemonic.bn_beqi__, InstrClass.ConditionalTransfer,  R13, uimm10_3, disp2_8),  // wild guess
                Instr(Mnemonic.bn_bf, InstrClass.ConditionalTransfer, disp2_16),                     // chenxing(mod), disasm
                Instr(Mnemonic.bn_bnei__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8),
                Instr(Mnemonic.bn_bnf__, InstrClass.ConditionalTransfer, disp2_16));

            var decode001001 = Mask(0, 2, "  9",
                nyi_2,
                // branch if reg <= imm XXX: signed/unsigned?
                Instr(Mnemonic.bn_ble__i__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8), // wild guess
                nyi_2,
                Instr(Mnemonic.bn_bgt__i__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8)); // wild guess

            var decode010000 = Sparse(0, 3, "  10", nyi_3,
                (0b001, Instr(Mnemonic.bn_divu, R13, R8, R3)),              // disasm
                (0b011, Instr(Mnemonic.bn_mul, R13, R8, R3)),               // disasm
                (0b100, Instr(Mnemonic.bn_add, R13, R8, R3)),               // guess, disasm
                (0b101, Instr(Mnemonic.bn_sub, R13, R8, R3)));              // disasm

            var decode010001 = Sparse(0, 3, "  11", nyi_3,
                (0b100, Instr(Mnemonic.bn_and, R13, R8, R3)),               // chenxing
                (0b101, Instr(Mnemonic.bn_or, R13, R8, R3)),                // disasm, guess
                (0b110, Instr(Mnemonic.bn_xor__, R13, R8, R3)),             // guess
                // XXX: could also be nor
                (0b111, Instr(Mnemonic.bn_nand__, R13, R8, R3)));           // guess

            var decode010010 = Mask(0, 3, "  010010",
                Instr(Mnemonic.bn_cmov____, R13, R8, R3),                   // guess
                Instr(Mnemonic.bn_cmov____, R13, R8, R3, uimm0_3),          // not sure what the last 3 bits are
                Instr(Mnemonic.bn_cmovi____, R13, R8, uimm3_5),             // guess
                Instr(Mnemonic.bn_cmovi____, R13, R8, uimm3_5, uimm0_3),    // not sure what the last 3 bits are
                nyi_3,
                nyi_3,
                nyi_3,
                nyi_3);

            var decode010011 = Sparse(0, 3, "  010011", nyi_3,
                (0b000, Instr(Mnemonic.bn_slli__, R13, R8, uimm3_5)),       // guess
                (0b001, Instr(Mnemonic.bn_srli__, R13, R8, uimm3_5)),       // guess
                (0b010, Instr(Mnemonic.bn_srai__, R13, R8, uimm3_5)),       // guess
                (0b100, Instr(Mnemonic.bn_sll__, R13, R8, R3)),
                (0b101, Instr(Mnemonic.bn_srl__, R13, R8, R3)));            // guess

            var decode010111 = Sparse(0, 5, "  17", nyi_5,
                (0b00000, Instr(Mnemonic.bn_extbz__, R13, R8)),             // guess
                (0b00001, Instr(Mnemonic.bn_sfeqi, R13, uimm8_5)),          // chenxing
                (0b00100, Instr(Mnemonic.bn_exthz__, R13, R8)),             // guess
                (0b00101, Instr(Mnemonic.bn_sfeq__, R13, R8)),              // chenxing
                (0b01000, Instr(Mnemonic.bn_ff1__, R13, R8)),               // guess
                (0b01001, Instr(Mnemonic.bn_sfnei__, R13, uimm8_5)),        // guess
                (0b01101, Instr(Mnemonic.bn_sfne, R13, R8)),                // chenxing, disasm
                (0b10011, Instr(Mnemonic.bn_sfleui__, R13, uimm5_8)),       // guess
                // operands are swapped
                (0b10111, Instr(Mnemonic.bn_sfgeu, R8, R13)),               // chenxing, disasm
                (0b11000, Instr(Mnemonic.bn_entri__, uimm14_4, uimm5_9)),   // backtrace
                (0b11100, Instr(Mnemonic.bn_rtnei__, uimm14_4, uimm5_9)),   // backtrace
                (0b11011, Instr(Mnemonic.bn_sfgtui, R13, uimm5_8)),         // disasm
                // used for bn.sfltu with operands swapped
                (0b11111, Instr(Mnemonic.bn_sfgtu, R13, R8)));              // disasm

            return new D24BitDecoder(Mask(18, 5, "  24-bit instr",  // bit 23 is always 0
                decode000000,
                // opcode 000001
                Instr(Mnemonic.bn_movhi__, R13, uimm0_13),
                // opcode 000010
                Instr(Mnemonic.bn_lhz, R13, Ms(8, 1, 7, 1, PrimitiveType.UInt16)),  // chenxing
                decode000011,

                // opcode 000100
                // XXX: assuming this is l.lwz and not l.lws
                Instr(Mnemonic.bn_lbz__, R13, Ms(8, 0, 8, 0, PrimitiveType.Byte)),  // guess
                // opcode 000101
                Nyi("0b00101"),
                // opcode 000110
                Instr(Mnemonic.bn_sb__, Ms(8, 0, 8, 0, PrimitiveType.Byte), R13),    // guess
                // opcode 000111
                Instr(Mnemonic.bn_addi, R13, R8, simm0_8),                           // chenxing, backtrace

                decode001000,
                decode001001,
                // opcode 001010
                nyi_3,
                // opcode 001011
                Instr(Mnemonic.bn_j____, InstrClass.Transfer, disp0_18),

                // opcode 001100
                Nyi("0b01100"),
                // opcode 001101
                Instr(Mnemonic.bn_movhi__, R13, uimm0_13),           // chenxing
                // opcode 001110
                Nyi("0b01110"),
                // opcode 001111
                Nyi("0b01111"),

                decode010000,
                decode010001,
                // opcode 010010
                decode010010,
                decode010011,

                // opcode 010100
                Instr(Mnemonic.bn_ori, R13, R8, uimm0_8_16),  // chenxing
                // opcode 010101
                Instr(Mnemonic.bn_andi, R13, R8, uimm0_8),    // guess
                // opcode 010110
                Nyi("0b10110"),
                decode010111,

                // opcode 011000
                Nyi("0b11000"),
                // opcode 011001
                Nyi("0b11001"),
                // opcode 011010
                Nyi("0b11010"),
                // opcode 011011
                Nyi("0b11011"),

                // opcode 011100
                Nyi("0b11100"),
                // opcode 011101
                Nyi("0b11101"),
                // opcode 011110
                Nyi("0b11110"),
                // opcode 011111
                Nyi("0b11111")));
        }
    }
}
