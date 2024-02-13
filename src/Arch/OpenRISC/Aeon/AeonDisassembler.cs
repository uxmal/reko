#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
        private InstrClass iclassModifier;

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

            // Instructions are 16, 24 or 32 bits. Read the high 16 bits first.
            if (!rdr.TryReadBeUInt16(out ushort us))
                return null;
            var instr = rootDecoder.Decode(us, this);
            ops.Clear();
            this.iclassModifier = 0;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override AeonInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new AeonInstruction
            {
                InstructionClass = iclass | iclassModifier,
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

        /// <summary>
        /// Hard-coded reference to a specific register.
        /// </summary>
        private static Mutator<AeonDisassembler> Register(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator Reg0 = Register(Registers.GpRegisters[0]);

        /// A register encoded at position <paramref name="bitpos" />. However
        /// r0 is not expected; instructions containing such references will
        /// be marked as "Unlikely".
        /// </summary>
        private static Mutator RegisterFromField_unlikely_r0(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                if (ireg == 0)
                {
                    d.iclassModifier |= InstrClass.Unlikely;
                }
                return true;
            };
        }
        private static readonly Mutator Ru5 = RegisterFromField_unlikely_r0(5);
        private static readonly Mutator Ru13 = RegisterFromField_unlikely_r0(13);
        private static readonly Mutator Ru21 = RegisterFromField_unlikely_r0(21);

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
        private static readonly Mutator uimm0_4 = UnsignedImmediate(0, 4);
        private static readonly Mutator uimm0_5 = UnsignedImmediate(0, 5);
        private static readonly Mutator uimm0_8 = UnsignedImmediate(0, 8);
        private static readonly Mutator uimm0_8_16 = UnsignedImmediate(0, 8, PrimitiveType.UInt16);
        private static readonly Mutator uimm0_10 = UnsignedImmediate(0, 10);
        private static readonly Mutator uimm0_13 = UnsignedImmediate(0, 13);
        private static readonly Mutator uimm0_16_16 = UnsignedImmediate(0, 16, PrimitiveType.UInt16);
        private static readonly Mutator uimm1_4 = UnsignedImmediate(1, 4);
        private static readonly Mutator uimm3_5 = UnsignedImmediate(3, 5);
        private static readonly Mutator uimm3_13 = UnsignedImmediate(3, 13);
        private static readonly Mutator uimm4_2 = UnsignedImmediate(4, 2);
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
        private static readonly Mutator simm0_16_32 = SignedImmediate(0, 16, PrimitiveType.Int32);
        private static readonly Mutator simm3_5 = SignedImmediate(3, 5, PrimitiveType.Int32);
        private static readonly Mutator simm5_8 = SignedImmediate(5, 8, PrimitiveType.Int32);
        private static readonly Mutator simm5_16 = SignedImmediate(5, 16, PrimitiveType.Int32);
        private static readonly Mutator simm8_5 = SignedImmediate(8, 5, PrimitiveType.Int32);
        private static readonly Mutator simm10_3 = SignedImmediate(10, 3, PrimitiveType.Int32);
        private static readonly Mutator simm16_5 = SignedImmediate(16, 5, PrimitiveType.Int32);

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

        /// <summary>
        /// Memory access with implicit r1 (stack) register base and unsigned
        /// offset access with signed offset
        /// </summary>
        /// <returns></returns>
        private static Mutator MuStack(
            int bitposOffset,
            int lengthOffset,
            int offsetScale,
            PrimitiveType dt)
        {
            var offsetField = new Bitfield(bitposOffset, lengthOffset);
            return (u, d) =>
            {
                var baseReg = Registers.GpRegisters[1];
                var offset = offsetField.Read(u) << offsetScale;
                d.ops.Add(new MemoryOperand(dt) { Base = baseReg, Offset = (int)offset });
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
            var nyi = Instr(Mnemonic.Nyi, uimm26_6);
            var nyi_3 = Instr(Mnemonic.Nyi, uimm26_6, R21, uimm16_5, uimm3_13);
            var nyi_3_imm_disp = Instr(Mnemonic.Nyi, uimm26_6, R21, uimm16_5, disp3_13);
            var nyi_3_imm_disp_3 = Instr(Mnemonic.Nyi, uimm26_6, R21, uimm16_5, disp3_13, uimm0_3);
            var nyi_3_reg_disp = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, disp3_13);
            var nyi_3_reg_disp_3 = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, disp3_13, uimm0_3);
            var nyi_4 = Instr(Mnemonic.Nyi, uimm26_6, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2, uimm0_4);
            var nyi_5 = Instr(Mnemonic.Nyi, uimm26_6, R21, R16, uimm5_16, uimm0_5);
            var nyi_16s = Instr(Mnemonic.Nyi, uimm26_6, Ru21, R16, simm0_16);
            var nyi_16u = Instr(Mnemonic.Nyi, uimm26_6, Ru21, R16, uimm0_16_16);

            var nyi_unlikely = Instr(Mnemonic.Nyi, InstrClass.Unlikely, uimm26_6);

            var decoder110000 = Sparse(0, 4, "  opc=110000", nyi_5, //$REVIEW: maybe the sub-opcode is 5 bits?
                (0b0000, Instr(Mnemonic.bg_sfgeui__, R21, uimm5_16)),           // guess
                (0b0001, Instr(Mnemonic.bg_movhi, Ru21, uimm5_16)),             // chenxing(mod), source
                (0b0100, Instr(Mnemonic.bg_sfnei__, R21, uimm5_16)),            // guess
                (0b0101, Instr(Mnemonic.bg_mtspr1__, R21, uimm5_16)),           // guess
                (0b0111, Instr(Mnemonic.bg_mfspr1__, Ru21, uimm5_16)),          // guess
                (0b1000, Instr(Mnemonic.bg_sflesi__, R21, simm5_16)),           // guess
                (0b1010, Instr(Mnemonic.bg_sfleui__, R21, uimm5_16)),           // guess
                (0b1100, Instr(Mnemonic.bg_sfgesi__, R21, simm5_16)),           // guess
                (0b1101, Instr(Mnemonic.bg_mtspr, R16, R21, uimm4_12)),         // chenxing, source
                (0b1110, Instr(Mnemonic.bg_sfgtui__, R21, uimm5_16)),           // guess
                (0b1111, Instr(Mnemonic.bg_mfspr, Ru21, R16, uimm4_12)));       // chenxing, source

            var decoder110100 = Mask(0, 3, "  opc=110100",
                Instr(Mnemonic.bg_blesi__, InstrClass.ConditionalTransfer, R21, simm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_bltsi__, InstrClass.ConditionalTransfer, R21, simm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_beqi__, InstrClass.ConditionalTransfer, R21, simm16_5, disp3_13),   // guess
                Instr(Mnemonic.bg_b011i__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_b__bitseti__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_bgtui__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_bltui__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_b111i__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13)); // guess

            var decoder110101 = Mask(0, 3, "  opc=110101",
                Instr(Mnemonic.bg_bleu__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bges__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_beq__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),      // guess
                // $REVIEW: could displacement be larger? There are 5 bits left over
                Instr(Mnemonic.bg_bf, InstrClass.ConditionalTransfer, disp3_13),                   // source
                Instr(Mnemonic.bg_bgts__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bgeu__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bne__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),      // guess
                Instr(Mnemonic.bg_b111__, InstrClass.ConditionalTransfer, R21, R16, disp3_13));    // guess

            var decoder111001 = Mask(0, 1, "  opc=111001",
                Instr(Mnemonic.bg_jal, InstrClass.Transfer | InstrClass.Call, disp1_25), // guess
                Instr(Mnemonic.bg_j, InstrClass.Transfer, disp1_25));                    // guess

            var decoder111010 = Mask(0, 1, "  opc=111010",
                Instr(Mnemonic.bg_lhs__, Ru21, Ms(16, 1, 15, 1, PrimitiveType.Int16)),   // guess
                Instr(Mnemonic.bg_lhz__, Ru21, Ms(16, 1, 15, 1, PrimitiveType.Word16))); // guess

            var decoder111011 = Mask(0, 2, "  opc=111011",
                Instr(Mnemonic.bg_sw, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),         // chenxing, backtrace
                Instr(Mnemonic.bg_sw__, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),       // guess
                Instr(Mnemonic.bg_lwz, Ru21, Ms(16, 2, 14, 2, PrimitiveType.Word32)),       // guess, source
                Instr(Mnemonic.bg_sh__, Ms(16, 1, 15, 1, PrimitiveType.Word16), R21));      // guess

            var decoder111101 = Select(Bf((21, 5)), u => u == 0,
                Sparse(0, 4, "  opc=111101", nyi_4,
                    // XXX: chenxing had 0b0001 as invalidate_line
                    // XXX: possible/necessary to ensure unused bits are 0?
                    (0b0100, Instr(Mnemonic.bg_flush_invalidate, Ms(16, 6, 10, 0, PrimitiveType.Word32))),    // source
                    (0b0101, Instr(Mnemonic.bg_syncwritebuffer)),                                             // source
                    (0b0110, Instr(Mnemonic.bg_flush_line, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2)), // source
                    (0b0111, Instr(Mnemonic.bg_invalidate_line, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2))),                                                                // source
                Instr(Mnemonic.bg_lbs__, Ru21, Ms(16, 0, 16, 0, PrimitiveType.Byte)));

            var decoder = Mask(26, 5, "  32-bit instr",
                // These should never be reached because they are actually
                // 16-bit opcodes.
                Nyi("0b100000"),
                Nyi("0b100001"),
                Nyi("0b100010"),
                Nyi("0b100011"),
                Nyi("0b100100"),
                Nyi("0b100101"),
                Nyi("0b100110"),
                Nyi("0b100111"),

                // opcode 101000
                nyi_unlikely,
                // opcode 101001
                nyi_unlikely,
                // opcode 101010
                nyi_unlikely,
                // opcode 101011
                nyi_unlikely,

                // opcode 101100
                nyi_unlikely,
                // opcode 101101
                nyi_unlikely,
                // opcode 101110
                nyi_unlikely,
                // opcode 101111
                nyi_unlikely,

                decoder110000,
                // opcode 110001
                Instr(Mnemonic.bg_andi, Ru21, R16, uimm0_16_16),          // chenxing
                // opcode 110010
                Instr(Mnemonic.bg_ori, Ru21, R16, uimm0_16_16),           // chenxing
                // opcode 110011
                Instr(Mnemonic.bg_muli__, Ru21, R16, simm0_16_32),        // guess

                decoder110100,
                decoder110101,
                // opcode 110110
                Instr(Mnemonic.bg_xori__, Ru21, R16, uimm0_16_16),        // wild guess
                // opcode 110111
                Instr(Mnemonic.bg_addci__, Ru21, R16, simm0_16),

                // opcode 111000
                nyi,
                decoder111001,
                decoder111010,
                decoder111011,

                // opcode 111100
                Instr(Mnemonic.bg_lbz__, Ru21, Ms(16, 0, 16, 0, PrimitiveType.Byte)),   // guess
                decoder111101,
                // opcode 111110
                Instr(Mnemonic.bg_sb__, Ms(16, 0, 16, 0, PrimitiveType.Byte), R21),     // guess
                // opcode 111111
                //$REVIEW: signed or unsigned immediate?
                Instr(Mnemonic.bg_addi, Ru21, R16, simm0_16));                          // chenxing, backtrace

            return new D32BitDecoder(decoder);
        }

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create16bitInstructionDecoder()
        {
            var nyi_5 = Instr(Mnemonic.Nyi, uimm10_6, uimm5_5, uimm0_5);

            // when rD is r0, the instruction has a different function
            var decoder100000_special = Mask(0, 1, "  opc=100000",
                Instr(Mnemonic.bt_trap, uimm1_4),                   // source
                Instr(Mnemonic.bt_nop, uimm1_4));                   // source

            var decoder100000_stack = Mask(0, 1, "  opc=100000",
                Instr(Mnemonic.bt_swst____, MuStack(1, 4, 2, PrimitiveType.Word32), R5),
                Instr(Mnemonic.bt_lwst____, Ru5, MuStack(1, 4, 2, PrimitiveType.Word32)));

            var decoder100000 = Select((5, 5), u => u == 0,
                decoder100000_special,
                decoder100000_stack);

            var decoder100001_sub0 = Select((5, 5), u => u == 0,
                Instr(Mnemonic.bt_rfe, InstrClass.Transfer | InstrClass.Return), // source
                nyi_5);

            var decoder100001 = Sparse(0, 5, "  opc=100001", nyi_5,
                (0b00000, decoder100001_sub0),
                (0b01000, Instr(Mnemonic.bt_jalr__, InstrClass.Transfer, Ru5)), // guess
                (0b01001, Instr(Mnemonic.bt_jr, InstrClass.Transfer, Ru5)));    // source

            return Mask(10, 3, "  16-bit",
                decoder100000,
                decoder100001,
                // opcode 100010
                Instr(Mnemonic.bt_mov__, Ru5, R0),                   // guess
                // opcode 100011
                Instr(Mnemonic.bt_add__, Ru5, R0),                   // guess

                // opcode 100100
                Instr(Mnemonic.bt_j, InstrClass.Transfer, disp0_10), // chenxing
                // opcode 100101
                Instr(Mnemonic.bt_movhi__, Ru5, uimm0_5),            // guess
                // opcode 100110
                Instr(Mnemonic.bt_movi__, Ru5, simm0_5),             // source, guess
                // opcode 100111
                Instr(Mnemonic.bt_addi__, Ru5, simm0_5));            // backtrace, guess
        }

        private static Decoder<AeonDisassembler, Mnemonic, AeonInstruction> Create24bitInstructionDecoder()
        {
            var nyi = Instr(Mnemonic.Nyi, uimm18_6);
            var nyi_2 = Instr(Mnemonic.Nyi, uimm18_6, R13, uimm10_3, disp2_8, uimm0_2);
            var nyi_3 = Instr(Mnemonic.Nyi, uimm18_6, R13, R8, R3, uimm0_3);
            var nyi_5 = Instr(Mnemonic.Nyi, uimm18_6, R13, R8, uimm0_5);
            var nyi_unlikely = Instr(Mnemonic.Nyi, InstrClass.Unlikely, uimm18_6);

            var decode000000 = Select(Bf((0, 18)), u => u == 0,
                Instr(Mnemonic.bn_nop, InstrClass.Linear | InstrClass.Padding | InstrClass.Zero), // source
                // $REVIEW: need to check what a nonzero operand does on actual hardware.
                Instr(Mnemonic.Nyi, InstrClass.Linear | InstrClass.Padding, uimm18_6));

            var decode000011_bn_sh = Instr(Mnemonic.bn_sh__, Ms(8, 1, 7, 1, PrimitiveType.Word16), R13);
            var decode000011 = Mask(0, 2, "  3",
                Instr(Mnemonic.bn_sw, Ms(8, 2, 6, 2, PrimitiveType.Word32), R13),    // 000 011 bbbbbaaaaaiiiiii00   // chenxing, backtrace
                decode000011_bn_sh,
                Instr(Mnemonic.bn_lwz, Ru13, Ms(8, 2, 6, 2, PrimitiveType.Word32)), // 000 011 dddddaaaaaiiiiii10   // guess, source
                decode000011_bn_sh);
            
            var decode001000 = Mask(0, 2, "  8",
                // branch if reg == imm
                Instr(Mnemonic.bn_beqi__, InstrClass.ConditionalTransfer, R13, simm10_3, disp2_8),   // guess
                Instr(Mnemonic.bn_bf, InstrClass.ConditionalTransfer, disp2_16),                     // chenxing(mod), source
                Instr(Mnemonic.bn_bnei__, InstrClass.ConditionalTransfer, R13, simm10_3, disp2_8),
                Instr(Mnemonic.bn_bnf__, InstrClass.ConditionalTransfer, disp2_16));

            var decode001001 = Mask(0, 2, "  9",
                Instr(Mnemonic.bn_blesi__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8),
                // branch if reg <= imm XXX: signed/unsigned?
                Instr(Mnemonic.bn_bleui__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8), // wild guess
                Instr(Mnemonic.bn_blesi____, InstrClass.ConditionalTransfer, R13, simm10_3, disp2_8), // guess
                Instr(Mnemonic.bn_bgtui__, InstrClass.ConditionalTransfer, R13, uimm10_3, disp2_8)); // wild guess

            var decode010000 = Mask(0, 3, "  10",
                //$REVIEW: divs and divu may be mixed up
                Instr(Mnemonic.bn_divs__, Ru13, R8, R3),           // guess
                Instr(Mnemonic.bn_divu, Ru13, R8, R3),             // source
                Instr(Mnemonic.bn_mulu____, Ru13, R8, R3),         // wild guess
                Instr(Mnemonic.bn_mul, Ru13, R8, R3),              // source
                Instr(Mnemonic.bn_add, Ru13, R8, R3),              // guess, source
                Instr(Mnemonic.bn_sub, Ru13, R8, R3),              // source
                Instr(Mnemonic.bn_subb__, Ru13, R8, R3),           // guess
                Instr(Mnemonic.bn_addc__, Ru13, R8, R3));

            var decode010001 = Sparse(0, 3, "  11", nyi_3,
                (0b100, Instr(Mnemonic.bn_and, Ru13, R8, R3)),              // chenxing
                (0b101, Instr(Mnemonic.bn_or, Ru13, R8, R3)),               // source, guess
                (0b110, Instr(Mnemonic.bn_xor__, Ru13, R8, R3)),            // guess
                // XXX: could also be nor
                (0b111, Instr(Mnemonic.bn_nand__, Ru13, R8, R3)));          // guess

            var decode010010 = Mask(0, 3, "  010010",
                Instr(Mnemonic.bn_cmov____, Ru13, R8, R3),                  // guess
                Instr(Mnemonic.bn_cmov____, Ru13, R8, R3),                  // guess
                Instr(Mnemonic.bn_cmovsi__, Ru13, R8, simm3_5),             // guess
                Instr(Mnemonic.bn_cmovi____, Ru13, simm8_5, simm3_5),       // guess
                nyi_3,
                nyi_3,
                nyi_3,
                nyi_3);

            var decode010011 = Mask(0, 3, "  010011",
                Instr(Mnemonic.bn_slli__, Ru13, R8, uimm3_5),       // guess
                Instr(Mnemonic.bn_srli__, Ru13, R8, uimm3_5),       // guess
                Instr(Mnemonic.bn_srai__, Ru13, R8, uimm3_5),       // guess
                Instr(Mnemonic.bn_rori__, Ru13, R8, uimm3_5),       // guess
                Instr(Mnemonic.bn_sll__, Ru13, R8, R3),
                Instr(Mnemonic.bn_srl__, Ru13, R8, R3),             // guess
                Instr(Mnemonic.bn_sra__, Ru13, R8, R3),             // guess
                Instr(Mnemonic.bn_ror__, Ru13, R8, R3));            // guess

            var decode010111 = Sparse(0, 5, "  17", nyi_5,
                (0b00000, Instr(Mnemonic.bn_extbz__, Ru13, R8)),            // guess
                (0b00010, Instr(Mnemonic.bn_extbs__, Ru13, R8)),            // guess
                (0b00001, Instr(Mnemonic.bn_sfeqi, R13, uimm5_8)),          // chenxing
                (0b00100, Instr(Mnemonic.bn_exthz__, Ru13, R8)),            // guess
                (0b00110, Instr(Mnemonic.bn_exths__, Ru13, R8)),            // guess
                (0b00101, Instr(Mnemonic.bn_sfeq__, R13, R8)),              // chenxing
                (0b01000, Instr(Mnemonic.bn_ff1__, Ru13, R8)),              // guess
                (0b01001, Instr(Mnemonic.bn_sfnei__, R13, uimm5_8)),        // guess
                (0b01101, Instr(Mnemonic.bn_sfne, R13, R8)),                // chenxing, source
                (0b01111, Instr(Mnemonic.bn_sfges____, R13, R8)),           // guess, could be sfgeu
                (0b10001, Instr(Mnemonic.bn_sflesi__, R13, simm5_8)),       // guess
                (0b10011, Instr(Mnemonic.bn_sfleui__, R13, uimm5_8)),       // guess
                // operands are swapped
                (0b10111, Instr(Mnemonic.bn_sfgeu, R8, R13)),               // chenxing, source
                (0b10101, Instr(Mnemonic.bn_sfges__, R13, R8)),             // guess
                (0b11000, Instr(Mnemonic.bn_entri__, uimm14_4, uimm5_9)),   // backtrace
                (0b11001, Instr(Mnemonic.bn_sfgesi__, R13, simm5_8)),       // guess
                (0b11011, Instr(Mnemonic.bn_sfgtui, R13, uimm5_8)),         // source
                (0b11100, Instr(Mnemonic.bn_rtnei__, uimm14_4, uimm5_9)),   // backtrace
                (0b11101, Instr(Mnemonic.bn_sflts__, R13, R8)),             // guess
                // used for bn.sfltu with operands swapped
                (0b11111, Instr(Mnemonic.bn_sfgtu, R13, R8)));              // source

            return new D24BitDecoder(Mask(18, 5, "  24-bit instr",  // bit 23 is always 0
                decode000000,
                // opcode 000001
                Instr(Mnemonic.bn_movhi__, Ru13, uimm0_13),
                // opcode 000010
                Instr(Mnemonic.bn_lhz, Ru13, Ms(8, 1, 7, 1, PrimitiveType.UInt16)), // chenxing
                decode000011,

                // opcode 000100
                Instr(Mnemonic.bn_lbz__, Ru13, Ms(8, 0, 8, 0, PrimitiveType.Byte)),  // guess
                // opcode 000101
                Instr(Mnemonic.bn_lbs__, Ru13, Ms(8, 0, 8, 0, PrimitiveType.Byte)),  // guess
                // opcode 000110
                Instr(Mnemonic.bn_sb__, Ms(8, 0, 8, 0, PrimitiveType.Byte), R13),    // guess
                // opcode 000111
                Instr(Mnemonic.bn_addi, Ru13, R8, simm0_8),                          // chenxing, backtrace

                decode001000,
                decode001001,
                // opcode 001010
                Instr(Mnemonic.bn_jal__, InstrClass.Transfer, disp0_18),
                // opcode 001011
                Instr(Mnemonic.bn_j____, InstrClass.Transfer, disp0_18),

                // opcode 001100
                nyi_5,
                // opcode 001101
                Instr(Mnemonic.bn_movhi__, Ru13, uimm0_13),          // chenxing
                // opcode 001110
                nyi,
                // opcode 001111
                nyi,
                
                decode010000,
                decode010001,
                // opcode 010010
                decode010010,
                decode010011,

                // opcode 010100
                Instr(Mnemonic.bn_ori, Ru13, R8, uimm0_8_16), // chenxing
                // opcode 010101
                Instr(Mnemonic.bn_andi, Ru13, R8, uimm0_8),   // guess
                // opcode 010110
                nyi,
                decode010111,

                // opcode 011000
                nyi_unlikely,
                // opcode 011001
                nyi_unlikely,
                // opcode 011010
                nyi_unlikely,
                // opcode 011011
                nyi_unlikely,

                // opcode 011100
                nyi_unlikely,
                // opcode 011101
                nyi_unlikely,
                // opcode 011110
                nyi_unlikely,
                // opcode 011111
                nyi_unlikely));
        }
    }
}
