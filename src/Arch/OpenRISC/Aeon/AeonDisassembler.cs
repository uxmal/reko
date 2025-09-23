#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("AeonDis", addr, rdr, message);
            return new AeonInstruction
            {
                //$TODO: while still experimental, assume NYI instructions are valid.
                InstructionClass = InstrClass.Linear,
                //InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
                Operands = new MachineOperand[]
                {
                    Constant.UInt32((uint)bytes[0] >> 2)
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
        private static readonly Mutator R11 = RegisterFromField(11);
        private static readonly Mutator R13 = RegisterFromField(13);
        private static readonly Mutator R15 = RegisterFromField(15);
        private static readonly Mutator R16 = RegisterFromField(16);
        private static readonly Mutator R21 = RegisterFromField(21);

        /// <summary>
        /// Some sort of architectural register?
        /// </summary>
        private static Mutator ac(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.acs[ireg]);
                return true;
            };
        }
        private static readonly Mutator ac7 = ac(7);
        private static readonly Mutator ac11 = ac(11);
        private static readonly Mutator ac16 = ac(16);

        /// <summary>
        /// Probably a vector register
        /// </summary>
        private static Mutator VectorRegister(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.VectorRegisters[ireg]);
                return true;
            };
        }
        private static readonly Mutator<AeonDisassembler> V6 = VectorRegister(6);
        private static readonly Mutator<AeonDisassembler> V11 = VectorRegister(11);
        private static readonly Mutator<AeonDisassembler> V16 = VectorRegister(16);
        private static readonly Mutator<AeonDisassembler> V21 = VectorRegister(21);

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
                d.ops.Add(Constant.Create(dt, uImm));
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
        private static readonly Mutator uimm1_5 = UnsignedImmediate(1, 5);
        private static readonly Mutator uimm1_6 = UnsignedImmediate(1, 6);
        private static readonly Mutator uimm1_10 = UnsignedImmediate(1, 10);
        private static readonly Mutator uimm2_2 = UnsignedImmediate(2, 2);
        private static readonly Mutator uimm2_6 = UnsignedImmediate(2, 6);
        private static readonly Mutator uimm3_1 = UnsignedImmediate(3, 1);
        private static readonly Mutator uimm3_5 = UnsignedImmediate(3, 5);
        private static readonly Mutator uimm3_13 = UnsignedImmediate(3, 13);
        private static readonly Mutator uimm4_1 = UnsignedImmediate(4, 1);
        private static readonly Mutator uimm4_2 = UnsignedImmediate(4, 2);
        private static readonly Mutator uimm4_4 = UnsignedImmediate(4, 4);
        private static readonly Mutator uimm4_12 = UnsignedImmediate(4, 12);
        private static readonly Mutator uimm5_5 = UnsignedImmediate(5, 5);
        private static readonly Mutator uimm5_6 = UnsignedImmediate(5, 6);
        private static readonly Mutator uimm5_8 = UnsignedImmediate(5, 8);
        private static readonly Mutator uimm5_9 = UnsignedImmediate(5, 9);
        private static readonly Mutator uimm5_16 = UnsignedImmediate(5, 16);
        private static readonly Mutator uimm6_5 = UnsignedImmediate(6, 5);
        private static readonly Mutator uimm8_1 = UnsignedImmediate(8, 1);
        private static readonly Mutator uimm8_3 = UnsignedImmediate(8, 3);
        private static readonly Mutator uimm8_5 = UnsignedImmediate(8, 5);
        private static readonly Mutator uimm8_8 = UnsignedImmediate(8, 8);
        private static readonly Mutator uimm9_2 = UnsignedImmediate(9, 2);
        private static readonly Mutator uimm10_3 = UnsignedImmediate(10, 3);
        private static readonly Mutator uimm10_6 = UnsignedImmediate(10, 6);
        private static readonly Mutator uimm11_2 = UnsignedImmediate(11, 2);
        private static readonly Mutator uimm11_5 = UnsignedImmediate(11, 5);
        private static readonly Mutator uimm11_10 = UnsignedImmediate(11, 10);
        private static readonly Mutator uimm12_10 = UnsignedImmediate(12, 10);
        private static readonly Mutator uimm14_4 = UnsignedImmediate(14, 4);
        private static readonly Mutator uimm16_2 = UnsignedImmediate(16, 2);
        private static readonly Mutator uimm16_5 = UnsignedImmediate(16, 5);
        private static readonly Mutator uimm18_2 = UnsignedImmediate(18, 2);
        private static readonly Mutator uimm18_6 = UnsignedImmediate(18, 6);
        private static readonly Mutator uimm19_1 = UnsignedImmediate(19, 1);
        private static readonly Mutator uimm20_6 = UnsignedImmediate(20, 6);
        private static readonly Mutator uimm21_2 = UnsignedImmediate(21, 2);
        private static readonly Mutator uimm23_2 = UnsignedImmediate(23, 2);
        private static readonly Mutator uimm26_6 = UnsignedImmediate(26, 6);

        private static Mutator SignedImmediate(int bitPos, int bitlength, PrimitiveType dt)
        {
            var field = new Bitfield(bitPos, bitlength);
            return (u, d) =>
            {
                var sImm = field.ReadSigned(u);
                d.ops.Add(Constant.Create(dt, sImm));
                return true;
            };
        }
        private static readonly Mutator simm0_5 = SignedImmediate(0, 5, PrimitiveType.Int16);
        private static readonly Mutator simm0_8 = SignedImmediate(0, 8, PrimitiveType.Int16);
        private static readonly Mutator simm0_16 = SignedImmediate(0, 16, PrimitiveType.Int16);
        private static readonly Mutator simm0_16_32 = SignedImmediate(0, 16, PrimitiveType.Int32);
        private static readonly Mutator simm3_5 = SignedImmediate(3, 5, PrimitiveType.Int32);
        private static readonly Mutator simm5_8 = SignedImmediate(5, 8, PrimitiveType.Int32);
        private static readonly Mutator simm8_8 = SignedImmediate(8, 8, PrimitiveType.Int32);
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
                d.ops.Add(target);
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

        private static bool Eq0(uint u) => u == 0;

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<AeonDisassembler, Mnemonic, AeonInstruction>(message);
        }

        private static Decoder Nyi(params Mutator[] mutators)
        {
            return new NyiDecoder(InstrClass.Linear, mutators);
        }

        private static Decoder Nyi(InstrClass iclass, params Mutator[] mutators)
        {
            return new NyiDecoder(iclass, mutators);
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

        private class NyiDecoder : InstrDecoder<AeonDisassembler, Mnemonic, AeonInstruction>
        {
            public NyiDecoder(InstrClass iclass, params Mutator<AeonDisassembler>[] mutators)
                : base(iclass, Mnemonic.Nyi, mutators)
            {
            }

            public override AeonInstruction Decode(uint wInstr, AeonDisassembler dasm)
            {
                var testGenSvc = dasm.arch.Services.GetService<ITestGenerationService>();
                if (testGenSvc is not null)
                {
                    int instrBitlength = 8 * (int)(dasm.rdr.Address - dasm.addr);
                    uint msb6 = wInstr >> (instrBitlength - 6);
                    string sMsb6 = Convert.ToString(msb6, 2).PadLeft(6, '0');
                    testGenSvc?.ReportMissingDecoder("AeonDis", dasm.addr, dasm.rdr, sMsb6);
                }
                return base.Decode(wInstr, dasm);
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
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi = Nyi(uimm26_6);
            var nyi_3 = Nyi(uimm26_6, R21, uimm16_5, uimm3_13);
            var nyi_3_imm_disp = Nyi(uimm26_6, R21, uimm16_5, disp3_13);
            var nyi_3_imm_disp_3 = Nyi(uimm26_6, R21, uimm16_5, disp3_13, uimm0_3);
            var nyi_3_reg_disp = Nyi(uimm26_6, R21, R16, disp3_13);
            var nyi_3_reg_disp_3 = Nyi(uimm26_6, R21, R16, disp3_13, uimm0_3);
            var nyi_4 = Nyi(uimm26_6, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2, uimm0_4);
            var nyi_5 = Nyi(uimm26_6, R21, R16, uimm5_16, uimm0_5);
            var nyi_16s = Nyi(uimm26_6, Ru21, R16, simm0_16);
            var nyi_16u = Nyi(uimm26_6, Ru21, R16, uimm0_16_16);

            var nyi_unlikely = Nyi(InstrClass.Unlikely, uimm26_6);

            var decoder101000 = Mask(0, 2,
                Instr(Mnemonic.bg_divl__, Ru21, R16, R11, uimm5_6),
                Instr(Mnemonic.bg_divlr__, Ru21, R16, R11, uimm5_6),
                Instr(Mnemonic.bg_divlu__, Ru21, R16, R11, uimm5_6),
                Instr(Mnemonic.bg_divlru__, Ru21, R16, R11, uimm5_6));

            var decoder101001 = Mask(0, 2, "  101001",
                Instr(Mnemonic.bg_ldww__, Ru21, Ms(16, 5, 11, 0, PrimitiveType.Word32), uimm2_2, uimm4_1),
                Instr(Mnemonic.bg_ldwwx__, Ru21, Ms(16, 5, 11, 0, PrimitiveType.Word32), uimm2_2, uimm4_1),
                Instr(Mnemonic.bg_ldww2__, Ru21, Ms(16, 5, 11, 0, PrimitiveType.Word32), uimm2_2, uimm4_1),
                Instr(Mnemonic.bg_ldww2x__, Ru21, Ms(16, 5, 11, 0, PrimitiveType.Word32), uimm2_2, uimm4_1));

            var bg_amacr_01 = Mask(11, 1, "  01",
                Instr(Mnemonic.bg_dummy1__, Ru21, ac7, uimm1_6, R16),
                Mask(12, 3, "  1",
                    Instr(Mnemonic.bg_amacr40_32__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40_32r__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40r_32__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40r_32r__, Ru21, ac7, uimm1_6),

                    Instr(Mnemonic.bg_amacr40_16__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40_16r__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40r_16__, Ru21, ac7, uimm1_6),
                    Instr(Mnemonic.bg_amacr40r_16r__, Ru21, ac7, uimm1_6)));

            var decoder101010 = Mask(9, 2,
                Instr(Mnemonic.bg_amacr__, Ru21, ac7, uimm11_5, uimm1_6),
                bg_amacr_01,
                Instr(Mnemonic.bg_amacr_ex__, Ru21, ac7, uimm11_5, uimm1_6, uimm19_1, uimm16_2),
                Nyi("bg.amacr"));

            var decoder101011 = Mask(0, 1, "  101011",
                Instr(Mnemonic.bg_btb__, uimm23_2, uimm21_2, uimm1_10, uimm11_10),
                Mask(11, 1, "  1",
                    Instr(Mnemonic.bg_loop__, uimm1_10, R21),
                    Instr(Mnemonic.bg_loopi__, uimm1_10, uimm12_10)));

            var decoder101100 = Sparse(0, 6, "  101100", nyi,
                (0b111100, invalid));

            var decoder101101 = Mask(0, 1, "  101101",
                Sparse(0, 6, "  101101", nyi,
                    (0b001000, If(6, 10, Eq0,
                        Instr(Mnemonic.mv_mmk__, Ru21, V16)))),
                Instr(Mnemonic.mv_opv__, V21, V16, V11, V6, uimm1_5));

            var decoder101110 = Sparse(0, 4, "  101110", Nyi("101110"),
                (0b0000, Mask(4, 2,
                    Instr(Mnemonic.bg_ext__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_extu__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_exti__, Ru21, R16, uimm6_5, uimm11_5),
                    Instr(Mnemonic.bg_extui__, Ru21, R16, uimm6_5, uimm11_5))),
                (0b0010, Mask(4, 2,
                    Instr(Mnemonic.bg_dep__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_depi__, Ru21, R16, uimm6_5, uimm11_5),
                    Instr(Mnemonic.bg_depr__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_depri__, Ru21, R16, uimm6_5, uimm11_5))),
                (0b0011, Mask(4, 2,
                    Instr(Mnemonic.bg_satdep__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_satdepu__, Ru21, R16, uimm6_5, R11),
                    Instr(Mnemonic.bg_satdepi__, Ru21, R16, uimm6_5, uimm11_5),
                    Instr(Mnemonic.bg_satdepui__, Ru21, R16, uimm6_5, uimm11_5))),
                (0b0100, Mask(4, 2,
                    Mask(6, 2, 
                        Instr(Mnemonic.bg_minu__, Ru21, R16, R11),
                        Instr(Mnemonic.bg_max__, Ru21, R16, R11),
                        Instr(Mnemonic.bg_min__, Ru21, R16, R11),
                        Instr(Mnemonic.bg_maxu__, Ru21, R16, R11)),
                    Instr(Mnemonic.bg_abs__, Ru21, R16),
                    Mask(6, 2, 
                        Instr(Mnemonic.bg_minui__, Ru21, R16, uimm8_8),
                        Instr(Mnemonic.bg_maxi__, Ru21, R16, simm8_8),
                        Instr(Mnemonic.bg_mini__, Ru21, R16, simm8_8),
                        Instr(Mnemonic.bg_maxui__, Ru21, R16, uimm8_8)),
                    Nyi("101110 ... 11 0100"))),
                (0b0101, Mask(4, 3, "  0101",
                    Instr(Mnemonic.bg_adddf__, Ru21, R16, R11),
                    Instr(Mnemonic.bg_subdf__, Ru21, R16, R11),
                    Instr(Mnemonic.bg_i2df__, Ru21, R16),
                    Instr(Mnemonic.bg_df2i__, Ru21, R16),

                    Instr(Mnemonic.bg_muldf__, Ru21, R16, R11),
                    Instr(Mnemonic.bg_madddf__, Ru21, R16, R11),
                    Instr(Mnemonic.bg_divdf__, Ru21, R16, R11),
                    Instr(Mnemonic.bg_remdf__, Ru21, R16, R11))),
                (0b0110, invalid),
                (0b1000, Instr(Mnemonic.bg_fft__, uimm4_4, R16, R11, uimm8_3)),
                (0b1001, Instr(Mnemonic.bg_fftr__, uimm4_4, R16, R11, uimm8_3)),
                (0b1010, Instr(Mnemonic.bg_ifft__, uimm4_4, R16, R11, uimm8_3)),
                (0b1011, Instr(Mnemonic.bg_ifftr__, uimm4_4, R16, R11, uimm8_3)),
                (0b1100, invalid),
                (0b1111, invalid));

            var bg_amul = Mask(23, 1, "  amul",
                Mask(21, 2, "  0",
                    Instr(Mnemonic.bg_amul__, ac7, R16, R11),
                    nyi,
                    Instr(Mnemonic.bg_amul16c__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amul16cn__, ac7, R16, R11)),
                nyi);

            var bg_amac_qq = Sparse(12, 4, "  bg.amac_qq", nyi,
                (0b0000, Instr(Mnemonic.bg_amacqq, ac7)),
                (0b0001, Instr(Mnemonic.bg_amac16qq, ac7)),
                (0b0010, Instr(Mnemonic.bg_amac16cqq, ac7)),
                (0b0011, Instr(Mnemonic.bg_amac16ncqq, ac7)),

                (0b0101, Instr(Mnemonic.bg_amac16q, ac7, R16, uimm9_2)),
                (0b0110, Instr(Mnemonic.bg_amac16cq, ac7, R16, uimm9_2)),
                (0b0111, Instr(Mnemonic.bg_amac16cnq, ac7, R16, uimm9_2)));

            var bg_amsb_qq = Sparse(12, 4, "  bg.amsb_qq", nyi,
                (0b0000, Instr(Mnemonic.bg_amsbqq, ac7)),
                (0b0001, Instr(Mnemonic.bg_amsb16qq, ac7)),
                (0b0010, Instr(Mnemonic.bg_amsb16cqq, ac7)),
                (0b0011, Instr(Mnemonic.bg_amsb16ncqq, ac7)),

                (0b0101, Instr(Mnemonic.bg_amsb16q, ac7, R16, uimm9_2)),
                (0b0110, Instr(Mnemonic.bg_amsb16cq, ac7, R16, uimm9_2)),
                (0b0111, Instr(Mnemonic.bg_amsb16cnq, ac7, R16, uimm9_2)));


            var bg_amac = Mask(21, 3, "  bg.amac*",
                Instr(Mnemonic.bg_amac__, ac7,R16, R11),
                invalid,
                Instr(Mnemonic.bg_amac16__, ac7,R16, R11),
                Instr(Mnemonic.bg_amacn__, ac7,R16, R11),
                bg_amac_qq,
                bg_amac_qq,
                bg_amac_qq,
                bg_amac_qq);
            var bg_amsb = Mask(21, 3, "bg.amsb",
                Instr(Mnemonic.bg_amsb__, ac7, R16, R11),
                invalid,
                Instr(Mnemonic.bg_amsb16cn__, ac7, R16, R11),
                Instr(Mnemonic.bg_amsb16cn__, ac7, R16, R11),
                bg_amsb_qq,
                bg_amsb_qq,
                bg_amsb_qq,
                bg_amsb_qq);

            var decoder101111 = Sparse(0, 4, "  101111", Nyi("101111"),
                (0b0000, Mask(4, 3, "  0000",
                    Instr(Mnemonic.bg_amacrq__, Ru21, uimm8_3),
                    Instr(Mnemonic.bg_amacwq__, uimm8_3, R21),
                    Instr(Mnemonic.bg_amacwqaddr__, uimm8_3, R21),
                    Instr(Mnemonic.bg_amacwqaddr_length__, uimm8_3, R21, R16),

                    invalid,
                    Instr(Mnemonic.bg_amacwq2__, uimm8_3, R21, R16),
                    Instr(Mnemonic.bg_amacwq2addr__, uimm8_3, R21, R16),
                    invalid)),
                (0b0001, Mask(4, 3, "  0001",
                    Instr(Mnemonic.bg_pamacrq__, Ru21, uimm8_3),
                    Instr(Mnemonic.bg_pamacwq__, uimm8_3, R16),
                    Instr(Mnemonic.bg_pamacwqaddr__, uimm8_3, R16),
                    Instr(Mnemonic.bg_pamacwqaddr_length__, uimm8_3, R16, R11),

                    invalid,
                    Instr(Mnemonic.bg_pamacwq2__, uimm8_3, R16, R11),
                    Instr(Mnemonic.bg_pamacwq2addr__, uimm8_3, R16, R11),
                    invalid)),
                (0b0010, Instr(Mnemonic.bg_pamac__, uimm20_6, uimm18_2, uimm5_6, uimm16_2, uimm11_2)),
                (0b0100, invalid),
                (0b1000, Mask(4, 3,
                    Nyi("101111 ... 000 1000"),
                    Nyi("101111 ... 001 1000"),
                    Nyi("101111 ... 010 1000"),
                    Nyi("101111 ... 011 1000"),

                    bg_amul,
                    bg_amac,
                    bg_amsb,
                    Instr(Mnemonic.bg_amacw__, ac7,R16, R11))),
                (0b1001, Mask(4, 3, "  1001",
                    Instr(Mnemonic.bg_amulqu__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacqu__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbqu__, ac7, R16, uimm9_2),
                    invalid,

                    Instr(Mnemonic.bg_amulu__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacu__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbu__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amadd__, ac7, ac16, ac11))),
                (0b1010, Mask(4, 3, "  1010",
                    Instr(Mnemonic.bg_amulq_ll__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacq_ll__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbq_ll__, ac7, R16, uimm9_2),
                    invalid,

                    Instr(Mnemonic.bg_amul_ll__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amac_ll__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amsb_ll__, ac7, R16, R11),
                    invalid)),
                (0b1011, Mask(4, 3, "  1011",
                      Instr(Mnemonic.bg_amulq_hl__, ac7, R16, uimm9_2),
                      Instr(Mnemonic.bg_amacq_hl__, ac7, R16, uimm9_2),
                      Instr(Mnemonic.bg_amsbq_hl__, ac7, R16, uimm9_2),
                      invalid,

                      Instr(Mnemonic.bg_amul_hl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amac_hl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amsb_hl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amsub_hl__, ac7, R16, R11))),
                (0b1100, Mask(4, 3,
                      Instr(Mnemonic.bg_amulq_wl__, ac7, R16, uimm9_2),
                      Instr(Mnemonic.bg_amacq_wl__, ac7, R16, uimm9_2),
                      Instr(Mnemonic.bg_amsbq_wl__, ac7, R16, uimm9_2),
                      nyi,

                      Instr(Mnemonic.bg_amul_wl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amac_wl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amsb_wl__, ac7, R16, R11),
                      Instr(Mnemonic.bg_amsub_wl__, ac7, R16, R11))),
                (0b1101, Mask(4, 3,
                    Instr(Mnemonic.bg_amulq_wh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacq_wh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbq_wh__, ac7, R16, uimm9_2),
                    invalid,

                    Instr(Mnemonic.bg_amul_wh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amac_wh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amsb_wh__, ac7, R16, R11),
                    invalid)),
                (0b1110, Mask(4, 3,
                    Instr(Mnemonic.bg_amulq_hh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacq_hh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbq_hh__, ac7, R16, uimm9_2),
                    invalid,

                    Instr(Mnemonic.bg_amul_hh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amac_hh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amsb_hh__, ac7, R16, R11),
                    invalid)),
                (0b1111, Mask(4, 3,
                    Instr(Mnemonic.bg_amulq_lh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amacq_lh__, ac7, R16, uimm9_2),
                    Instr(Mnemonic.bg_amsbq_lh__, ac7, R16, uimm9_2),
                    invalid,

                    Instr(Mnemonic.bg_amul_lh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amac_lh__, ac7, R16, R11),
                    Instr(Mnemonic.bg_amsb_lh__, ac7, R16, R11),
                    invalid)));

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
                Instr(Mnemonic.bg_bleui__, InstrClass.ConditionalTransfer, R21, simm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_beqi__, InstrClass.ConditionalTransfer, R21, simm16_5, disp3_13),   // guess
                Instr(Mnemonic.bg_chk_lu__, InstrClass.ConditionalTransfer, R21, Ms(16, 3, 8, 1, PrimitiveType.Byte), R11),  // objdump
                Instr(Mnemonic.bg_bgtsi__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_bgtui__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_bltui__, InstrClass.ConditionalTransfer, R21, uimm16_5, disp3_13),  // guess
                Instr(Mnemonic.bg_chk_ll__, InstrClass.ConditionalTransfer, R21, Ms(16, 3, 8, 1, PrimitiveType.Byte), R11)); // objdump

            var decoder110101 = Mask(0, 3, "  opc=110101",
                Instr(Mnemonic.bg_bleu__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bges__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_beq__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),      // guess
                // $REVIEW: could displacement be larger? There are 5 bits left over
                Instr(Mnemonic.bg_bf, InstrClass.ConditionalTransfer, disp3_13),                   // source
                Instr(Mnemonic.bg_bgts__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bgeu__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),     // guess
                Instr(Mnemonic.bg_bne__, InstrClass.ConditionalTransfer, R21, R16, disp3_13),      // guess
                Instr(Mnemonic.bg_bnf, InstrClass.ConditionalTransfer, disp3_13));                 // objdump

            var decoder111001 = Mask(0, 1, "  opc=111001",
                Instr(Mnemonic.bg_jal, InstrClass.Transfer | InstrClass.Call, disp1_25), // guess
                Instr(Mnemonic.bg_j, InstrClass.Transfer, disp1_25));                    // guess

            var decoder111010 = Mask(0, 1, "  opc=111010",
                Instr(Mnemonic.bg_lhs__, Ru21, Ms(16, 1, 15, 1, PrimitiveType.Int16)),   // guess
                Instr(Mnemonic.bg_lhz__, Ru21, Ms(16, 1, 15, 1, PrimitiveType.Word16))); // guess

            var decoder111011_word = Mask(1, 1, "  opc=111011",
                Instr(Mnemonic.bg_sw, Ms(16, 2, 14, 2, PrimitiveType.Word32), R21),
                Instr(Mnemonic.bg_lwz, Ru21, Ms(16, 2, 14, 2, PrimitiveType.Word32)));

            var decoder111011 = Mask(0, 1, "  opc=111011",
                decoder111011_word,
                Instr(Mnemonic.bg_sh, Ms(16, 1, 15, 1, PrimitiveType.Word16), R21));

            var decoder111101 = Select(Bf((21, 5)), u => u == 0,
                Sparse(0, 4, "  opc=111101", nyi_4,
                    // XXX: chenxing had 0b0001 as invalidate_line
                    // XXX: possible/necessary to ensure unused bits are 0?
                    (0b0011, Instr(Mnemonic.bg_invalidate, Ms(16, 6, 10, 0, PrimitiveType.Word32))),          // objdump
                    (0b0100, Instr(Mnemonic.bg_flush_invalidate, Ms(16, 6, 10, 0, PrimitiveType.Word32))),    // source
                    (0b0101, Instr(Mnemonic.bg_syncwritebuffer)),                                             // source
                    (0b0110, Instr(Mnemonic.bg_flush_line, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2)), // source
                    (0b0111, Instr(Mnemonic.bg_invalidate_line, Ms(16, 6, 10, 0, PrimitiveType.Word32), uimm4_2)), // source
                    (0b1010, Instr(Mnemonic.bg_dma_op__, R16, R10))), // objdump
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
                decoder101000,
                decoder101001,
                decoder101010,
                decoder101011,

                // opcode 101100
                decoder101100,
                decoder101101,
                decoder101110,
                decoder101111,

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
                invalid,
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
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi_5 = Nyi(uimm10_6, uimm5_5, uimm0_5);

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

            var decoder100001_sub0 =
                Instr(Mnemonic.bt_rfe, InstrClass.Transfer | InstrClass.Return); // source

            var decoder100001 = Sparse(0, 5, "  opc=100001", nyi_5,
                (0b00000, decoder100001_sub0),
                (0b00001, Instr(Mnemonic.bt_ei__)),
                (0b00010, Instr(Mnemonic.bt_di__)),
                (0b00011, Instr(Mnemonic.bt_sys__)),
                (0b00100, Instr(Mnemonic.bt_wait__)),
                (0b00101, Instr(Mnemonic.bt_synci__)),
                (0b00110, Instr(Mnemonic.bt_syncd__)),
                (0b00111, Instr(Mnemonic.bt_return__, InstrClass.Transfer|InstrClass.Return)),
                (0b01000, Instr(Mnemonic.bt_jalr__, InstrClass.Transfer, Ru5)), // guess
                (0b01001, Instr(Mnemonic.bt_jr, InstrClass.Transfer, Ru5)),     // source
                (0b01010, Instr(Mnemonic.bt_pop__, Ru5)),                      // objdump
                (0b01011, Instr(Mnemonic.bt_push__, Ru5)),                      // objdump
                (0b01100, Instr(Mnemonic.bt_syncp__)),
                (0b10000, Instr(Mnemonic.bt_sfgtui_minus32769__, R5)),
                (0b10001, Instr(Mnemonic.bt_sfleui_minus32769__, R5)),
                (0b10010, Instr(Mnemonic.bt_sfgtsi_minus32769__, R5)),
                (0b10011, Instr(Mnemonic.bt_sflesi_minus32769__, R5)),
                (0b10100, Instr(Mnemonic.bt_add16, Ru5)),                      // objdump
                (0b10101, Instr(Mnemonic.bt_mov16, Ru5)),                 // objdump
                (0b10110, Instr(Mnemonic.bt_dm__)),                      // objdump
                (0b10111, Instr(Mnemonic.bt_em__)),
                (0b11100, invalid));                      // objdump

            return Mask(10, 3, "  16-bit",
                decoder100000,
                decoder100001,
                // opcode 100010
                Instr(Mnemonic.bt_mov, Ru5, R0),                   // guess
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
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi = Nyi(uimm18_6);
            var nyi_2 = Nyi(uimm18_6, R13, uimm10_3, disp2_8, uimm0_2);
            var nyi_3 = Nyi(uimm18_6, R13, R8, R3, uimm0_3);
            var nyi_5 = Nyi(uimm18_6, R13, R8, uimm0_5);
            var nyi_unlikely = Nyi(InstrClass.Unlikely, uimm18_6);

            var decode000000 = Select(Bf((0, 18)), u => u == 0,
                Instr(Mnemonic.bn_nop, InstrClass.Linear | InstrClass.Padding | InstrClass.Zero), // source
                // $REVIEW: need to check what a nonzero operand does on actual hardware.
                // In the objdump code it appears to be ignored.
                Instr(Mnemonic.bn_nop, InstrClass.Linear | InstrClass.Padding, uimm18_6));

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
                (0b000, Instr(Mnemonic.bn_flb, Ru13, R8, R3)),              // objdump
                (0b001, Instr(Mnemonic.bn_andn, Ru13, R8, R3)),             // objdump
                (0b010, Instr(Mnemonic.bn_sub_s, Ru13, R8, R3)),             // objdump
                (0b011, Instr(Mnemonic.bn_add_s, Ru13, R8, R3)),             // objdump
                (0b100, Instr(Mnemonic.bn_and, Ru13, R8, R3)),              // chenxing
                (0b101, Instr(Mnemonic.bn_or, Ru13, R8, R3)),               // source, guess
                (0b110, Instr(Mnemonic.bn_xor__, Ru13, R8, R3)),            // guess
                // XXX: could also be nor
                (0b111, Instr(Mnemonic.bn_nand__, Ru13, R8, R3)));          // guess

            var decode010010 = Mask(0, 3, "  010010",
                Instr(Mnemonic.bn_cmov__, Ru13, R8, R3),                  // guess
                Instr(Mnemonic.bn_cmovir__, Ru13, simm8_5, R3),                  // guess
                Instr(Mnemonic.bn_cmovri__, Ru13, R8, simm3_5),             // guess
                Instr(Mnemonic.bn_cmovii__, Ru13, simm8_5, simm3_5),       // guess
                invalid,
                invalid,
                invalid,
                invalid);

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
                (0b01010, Mask(5, 1,
                    Instr(Mnemonic.bn_exp__, R13, R8),                      // objdump
                    Instr(Mnemonic.bn_exp16__, R13, R8))),                  // objdump
                (0b01011, invalid),
                (0b01100, Instr(Mnemonic.bn_fl1, R13, R8)),                // chenxing, source
                (0b01101, Instr(Mnemonic.bn_sfne, R13, R8)),                // chenxing, source
                (0b01110, Instr(Mnemonic.bn_clz__, R13, R8)),               // objdump
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

            var decode011000 = Mask(0, 3, "  011000",
                Instr(Mnemonic.bn_srlri__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_srari__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_srarzi__, Ru13, R8, uimm3_5),
                invalid,

                Instr(Mnemonic.bn_addcqq_s__, R13),
                Instr(Mnemonic.bn_subcqq_s__, R13),
                Instr(Mnemonic.bn_addcnqq_s__, R13),
                Instr(Mnemonic.bn_subcnqq_s__, R13));


            var decode011011 = Mask(0, 3, "  011011",
                Instr(Mnemonic.bn_muladdh__, Ru13, R8, R3),
                Instr(Mnemonic.bn_muladdhx__, Ru13, R8, R3),
                Instr(Mnemonic.bn_mulsubh__, Ru13, R8, R3),
                Instr(Mnemonic.bn_mulsubhx__, Ru13, R8, R3),

                Instr(Mnemonic.bn_addp_s__, Ru13, R8, R3),
                Instr(Mnemonic.bn_subp_s__, Ru13, R8, R3),
                Instr(Mnemonic.bn_addcn_s__, Ru13, R8, R3),
                Instr(Mnemonic.bn_subcn_s__, Ru13, R8, R3));

            var decode011100 = Mask(0, 3, "  011100",
                Instr(Mnemonic.bn_sat__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_satu__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_satsu__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_satus__, Ru13, R8, uimm3_5),

                Instr(Mnemonic.bn_sf2df__, Ru13, R8),
                Instr(Mnemonic.bn_df2sf__, Ru13, R8),
                Instr(Mnemonic.bn_satsu__, Ru13, R8, uimm3_5),
                Instr(Mnemonic.bn_satus__, Ru13, R8, uimm3_5));

            var decode011110 = Mask(0, 3, "  011110",
                Instr(Mnemonic.bn_sfeqsf__, R8, R3),
                Instr(Mnemonic.bn_sflesf__, R8, R3),
                Instr(Mnemonic.bn_sfeqdf__, R8, R3),
                Instr(Mnemonic.bn_sfledf__, R8, R3),

                Instr(Mnemonic.bn_sfnesf__, R8, R3),
                Instr(Mnemonic.bn_sfgtsf__, R8, R3),
                Instr(Mnemonic.bn_sfnedf__, R8, R3),
                Instr(Mnemonic.bn_sfgtdf__, R8, R3));

            var decode011111 = Mask(0, 3, "  011111",
                Instr(Mnemonic.bn_addc_s_lwq__, Ru13, R8, uimm4_2, uimm3_1),
                Instr(Mnemonic.bn_subc_s_lwq__, Ru13, R8, uimm4_2, uimm3_1),
                Instr(Mnemonic.bn_addcn_s_lwq__, Ru13, R8, uimm4_2, uimm3_1),
                Instr(Mnemonic.bn_subcn_s_lwq__, Ru13, R8, uimm4_2, uimm3_1),

                Instr(Mnemonic.bn_addc_s_lwqq__, Ru13, R8, uimm8_1, uimm3_1),
                Instr(Mnemonic.bn_subc_s_lwqq__, Ru13, R8, uimm8_1, uimm3_1),
                Instr(Mnemonic.bn_addcn_s_lwqq__, Ru13, R8, uimm8_1, uimm3_1),
                Instr(Mnemonic.bn_subcn_s_lwqq__, Ru13, R8, uimm8_1, uimm3_1));

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
                Instr(Mnemonic.bn_jal__, InstrClass.Transfer|InstrClass.Call, disp0_18),
                // opcode 001011
                Instr(Mnemonic.bn_j, InstrClass.Transfer, disp0_18),

                // opcode 001100
                Instr(Mnemonic.bn_mlwz__, Ru13, Ms(8, 2, 6, 2, PrimitiveType.Word32), uimm0_2),
                Instr(Mnemonic.bn_msw__, Ms(8, 2, 6, 2, PrimitiveType.Word32), R13, uimm0_2),
                invalid,
                invalid,

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
                Instr(Mnemonic.bn_xori, Ru13, R8, uimm0_8),   // objdump
                decode010111,

                decode011000,
                // opcode 011001
                Mask(0, 3, "  011001",
                    Instr(Mnemonic.bn_dsl__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_bsl__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_dsr__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_bsa__, Ru13, R8, R3),
     
                    Instr(Mnemonic.bn_crc_le__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_conjc__, Ru13, R8),
                    Instr(Mnemonic.bn_pack_s__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_bsa_s__, Ru13, R8, R3)),
                Mask(0, 2, "  011010",
                    Instr(Mnemonic.bn_dsli__, Ru13, R8, uimm2_6),
                    Instr(Mnemonic.bn_dsri__, Ru13, R8, uimm2_6),
                    invalid,
                    invalid),
                decode011011,

                // opcode 011100
                decode011100,
                // opcode 011101
                Mask(0, 3, "  011101",
                    Instr(Mnemonic.bn_addsf__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_subsf__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_i2sf__, Ru13, R8),
                    Instr(Mnemonic.bn_sf2i__, Ru13, R8),

                    Instr(Mnemonic.bn_mulsf__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_maddsf__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_divsf__, Ru13, R8, R3),
                    Instr(Mnemonic.bn_remsf__, Ru13, R8, R3)),

                decode011110,
                decode011111));
        }
    }
}
