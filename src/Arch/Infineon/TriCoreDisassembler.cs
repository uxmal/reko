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
using System.Collections.Generic;

namespace Reko.Arch.Infineon
{
    public class TriCoreDisassembler : DisassemblerBase<TriCoreInstruction, Mnemonic>
    {
        private static readonly Decoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> rootDecoder;

        private readonly TriCoreArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public TriCoreDisassembler(TriCoreArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = default!;
            this.ops = new List<MachineOperand>();
        }

        public override TriCoreInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            var offset = rdr.Offset;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.InstructionClass |= (uInstr == 0) ? InstrClass.Zero : 0;
            instr.Length = (int)(rdr.Offset - offset);
            instr.Address = addr;
            return instr;
        }

        public override TriCoreInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            if (mnemonic == Mnemonic.Nyi)
                return NotYetImplemented("");
            return new TriCoreInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override TriCoreInstruction CreateInvalidInstruction()
        {
            return new TriCoreInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override TriCoreInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("TriCoreDis", this.addr, this.rdr, message);
            return new TriCoreInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
            };
        }

        private class Instr32Decoder : Decoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction>
        {
            private readonly Decoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> decoder;

            public Instr32Decoder(Decoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> decoder)
            {
                this.decoder = decoder;
            }

            public override TriCoreInstruction Decode(uint uInstr16, TriCoreDisassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out ushort hiWord))
                    return dasm.CreateInvalidInstruction();
                uint uInstr = ((uint) hiWord << 16) | uInstr16;
                return decoder.Decode(uInstr, dasm);
            }
        }

        private static RegisterStorage AddrRegister(uint uInstr, int bitoffset)
        {
            var iReg = (uInstr >> bitoffset) & 0xF;
            return Registers.AddrRegisters[iReg];
        }

        private static Mutator<TriCoreDisassembler> AddrRegister(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.AddrRegisters[iReg]);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> A8 = AddrRegister(8);
        private static readonly Mutator<TriCoreDisassembler> A12 = AddrRegister(12);
        private static readonly Mutator<TriCoreDisassembler> A28 = AddrRegister(28);

        private static Mutator<TriCoreDisassembler> DataRegister(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.DataRegisters[iReg]);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> D8 = DataRegister(8);
        private static readonly Mutator<TriCoreDisassembler> D12 = DataRegister(12);
        private static readonly Mutator<TriCoreDisassembler> D24 = DataRegister(24);
        private static readonly Mutator<TriCoreDisassembler> D28 = DataRegister(28);

        /// <summary>
        /// Data register pair
        /// </summary>
        /// <param name="bitoffset"></param>
        /// <returns></returns>
        private static Mutator<TriCoreDisassembler> ExtendedRegister(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                if ((iReg & 1) == 1)
                    return false;
                d.ops.Add(Registers.ExtendedDRegisters[iReg>>1]);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> E8 = ExtendedRegister(8);
        private static readonly Mutator<TriCoreDisassembler> E24 = ExtendedRegister(24);
        private static readonly Mutator<TriCoreDisassembler> E28 = ExtendedRegister(28);

        /// <summary>
        /// Address register pair
        /// </summary>
        /// <param name="bitoffset"></param>
        /// <returns></returns>
        private static Mutator<TriCoreDisassembler> ExtendedARegister(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                if ((iReg & 1) == 1)
                    return false;
                d.ops.Add(Registers.ExtendedARegisters[iReg>>1]);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> P8 = ExtendedARegister(8);


        private static Mutator<TriCoreDisassembler> Register(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> a15 = Register(Registers.AddrRegisters[15]);
        private static readonly Mutator<TriCoreDisassembler> d15 = Register(Registers.DataRegisters[15]);


        /// <summary>
        ///  Core register id at position [12..27]
        /// </summary>
        private static bool CR(uint uInstr, TriCoreDisassembler dasm)
        {
            var cr = (uInstr >> 12) & 0xFFFF;
            if (Registers.CoreRegisters.TryGetValue(cr, out var reg))
                dasm.ops.Add(reg);
            else
                dasm.ops.Add(ImmediateOperand.UInt16((ushort) cr));
            return true;
        }

        private static bool ABS(uint uInstr, TriCoreDisassembler dasm)
        {
            var off18 = (int)Bitfield.ReadFields(abs_off18, uInstr);
            dasm.ops.Add(MemoryOperand.Absolute(PrimitiveType.Ptr32, off18));
            return true;
        }
        private static readonly Bitfield[] abs_off18 = Bf((12, 4), (22, 4), (28, 4), (16, 6));

        private static Mutator<TriCoreDisassembler> MemoryAccess(PrimitiveType dt, int bpreg)
        {
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpreg);
                var op = MemoryOperand.BaseOffset(dt, areg, 0);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mb_12 = MemoryAccess(PrimitiveType.Byte, 12);
        private static readonly Mutator<TriCoreDisassembler> Mh_12 = MemoryAccess(PrimitiveType.Word16, 12);
        private static readonly Mutator<TriCoreDisassembler> Mw_12 = MemoryAccess(PrimitiveType.Word32, 12);

        private static Mutator<TriCoreDisassembler> MemoryUOffsetShift(PrimitiveType dt, int bpReg, int bitoffset, int bitlength, int shift)
        {
            var bfOffset = new Bitfield(bitoffset, bitlength);
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpReg);
                var offset = bfOffset.Read(u) << shift;
                d.ops.Add(MemoryOperand.BaseOffset(dt, areg, (int)offset));
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mub_12_off8_4 = MemoryUOffsetShift(PrimitiveType.Byte, 12, 8, 4, 0);
        private static readonly Mutator<TriCoreDisassembler> Mh_12_off8_4 = MemoryUOffsetShift(PrimitiveType.Int16, 12, 8, 4, 1);
        private static readonly Mutator<TriCoreDisassembler> Mw_12_off8_4 = MemoryUOffsetShift(PrimitiveType.Word32, 12, 8, 4, 2);

        private static Mutator<TriCoreDisassembler> MemorySOffset(PrimitiveType dt, int bpReg, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpReg);
                var offset = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(MemoryOperand.BaseOffset(dt, areg, offset));
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mb_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Int8, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mub_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Byte, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Md_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Word64, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mh_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Int16, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mhu_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Word16, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mw_12_soff22_6_28_4_16_6 = MemorySOffset(PrimitiveType.Word32, 12, Bf((22, 6), (28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mw_12_soff28_4_16_6 = MemorySOffset(PrimitiveType.Word32, 12, Bf((28, 4), (16, 6)));
        
        private static Mutator<TriCoreDisassembler> MemorySOffsetPreincrement(PrimitiveType dt, int bpReg, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpReg);
                var offset = Bitfield.ReadSignedFields(fields, u);
                var op = MemoryOperand.PreInc(dt, areg, offset);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mw_12_soff28_4_16_6_pre = MemorySOffsetPreincrement(PrimitiveType.Word32, 12, Bf((28, 4), (16, 6)));

        private static Mutator<TriCoreDisassembler> MemorySOffsetPostincrement(PrimitiveType dt, int bpReg, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpReg);
                var offset = Bitfield.ReadSignedFields(fields, u);
                var op = MemoryOperand.PostInc(dt, areg, offset);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mb_12_soff28_4_16_6_post = MemorySOffsetPostincrement(PrimitiveType.Byte, 12, Bf((28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mh_12_soff28_4_16_6_post = MemorySOffsetPostincrement(PrimitiveType.Word16, 12, Bf((28, 4), (16, 6)));
        private static readonly Mutator<TriCoreDisassembler> Mw_12_soff28_4_16_6_post = MemorySOffsetPostincrement(PrimitiveType.Word32, 12, Bf((28, 4), (16, 6)));


        private static Mutator<TriCoreDisassembler> MemoryPostInc(PrimitiveType dt, int bpreg)
        {
            return (u, d) =>
            {
                var areg = AddrRegister(u, bpreg);
                var op = MemoryOperand.PostInc(dt, areg);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> Mu8_pi12 = MemoryPostInc(PrimitiveType.Byte, 12);
        private static readonly Mutator<TriCoreDisassembler> Mh_pi12 = MemoryPostInc(PrimitiveType.Word16, 12);
        private static readonly Mutator<TriCoreDisassembler> Mw_pi12 = MemoryPostInc(PrimitiveType.Word32, 12);
        
        private static bool Ma15_wo(uint uInstr, TriCoreDisassembler dasm)
        {
            var reg = Registers.AddrRegisters[15];
            var dt = PrimitiveType.Word32;
            var offset = (uint)(uInstr >> 10) & 0x3C;
            var op = MemoryOperand.BaseOffset(dt, reg, (int)offset);
            dasm.ops.Add(op);
            return true;
        }

        private static bool Ma15_bo(uint uInstr, TriCoreDisassembler dasm)
        {
            var reg = Registers.AddrRegisters[15];
            var dt = PrimitiveType.Byte;
            var offset = (uInstr >> 12) & 0xF;
            var op = MemoryOperand.BaseOffset(dt, reg, (int)offset);
            dasm.ops.Add(op);
            return true;
        }

        private static bool Msp_wo8(uint uInstr, TriCoreDisassembler dasm)
        {
            var reg = Registers.AddrRegisters[10];
            var dt = PrimitiveType.Byte;
            var offset = (uInstr & 0xFF00) >> 6;
            var op = MemoryOperand.BaseOffset(dt, reg, (int) offset);
            dasm.ops.Add(op);
            return true;
        }

        private static Mutator<TriCoreDisassembler> UImmediate(PrimitiveType dt, int bitoffset, int bitsize)
        {
            var bitfield = new Bitfield(bitoffset, bitsize);
            return (u, d) =>
            {
                var value = bitfield.Read(u);
                var op = new ImmediateOperand(Constant.Create(dt, value));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> uimm6_2 = UImmediate(PrimitiveType.Byte, 6, 2);
        private static readonly Mutator<TriCoreDisassembler> uimm8_8 = UImmediate(PrimitiveType.Byte, 8, 8);
        private static readonly Mutator<TriCoreDisassembler> uimm12_4 = UImmediate(PrimitiveType.Byte, 12, 4);
        private static readonly Mutator<TriCoreDisassembler> uimm12_6 = UImmediate(PrimitiveType.Word16, 12, 6);
        private static readonly Mutator<TriCoreDisassembler> uimm12_9 = UImmediate(PrimitiveType.Word16, 12, 9);
        private static readonly Mutator<TriCoreDisassembler> uimm12_16 = UImmediate(PrimitiveType.Word16, 12, 16);
        private static readonly Mutator<TriCoreDisassembler> uimm16_2 = UImmediate(PrimitiveType.Byte, 16, 2);
        private static readonly Mutator<TriCoreDisassembler> uimm16_5 = UImmediate(PrimitiveType.Byte, 16, 5);
        private static readonly Mutator<TriCoreDisassembler> uimm23_5 = UImmediate(PrimitiveType.Byte, 23, 5);

        private static Mutator<TriCoreDisassembler> SImmediate(PrimitiveType dt, int bitoffset, int bitsize)
        {
            var bitfield = new Bitfield(bitoffset, bitsize);
            return (u, d) =>
            {
                var value = bitfield.ReadSigned(u);
                var op = new ImmediateOperand(Constant.Create(dt, value));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> simm12_4 = SImmediate(PrimitiveType.Word32, 12, 4);
        private static readonly Mutator<TriCoreDisassembler> simm12_9 = SImmediate(PrimitiveType.Word16, 12, 9);
        private static readonly Mutator<TriCoreDisassembler> simm12_16 = SImmediate(PrimitiveType.Word16, 12, 16);

        private static Mutator<TriCoreDisassembler> UImmediate(PrimitiveType dt, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var value = Bitfield.ReadFields(fields, u);
                var op = new ImmediateOperand(Constant.Create(dt, value));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> u8_7_1_12_4 = UImmediate(PrimitiveType.Word16, Bf((7,1),(12,4)));
        
        private static Mutator<TriCoreDisassembler> UImmediate_shift(PrimitiveType dt, int bitoffset, int bitsize, int shift)
        {
            var bitfield = new Bitfield(bitoffset, bitsize);
            return (u, d) =>
            {
                var value = bitfield.Read(u) << shift;
                var op = new ImmediateOperand(Constant.Create(dt, value));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> uimm8_4_sh1 = UImmediate_shift(PrimitiveType.UInt32, 8, 4, 1);

        /// <summary>
        /// Branch with 24-bit displacement
        /// </summary>
        /// <param name="uInstr16"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        private static bool B(uint uInstr, TriCoreDisassembler dasm)
        {
            var uDisp24 = ((uInstr & 0xFF00) << 8) | (uInstr >> 16);
            var disp24 = Bits.SignExtend(uDisp24, 24);
            var addr = dasm.addr + disp24 * 2;
            dasm.ops.Add(addr);
            return true;
        }

        /// <summary>
        /// Branch to absolute 24-bit address
        /// </summary>
        /// <param name="uInstr16"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        private static bool Babs(uint uInstr, TriCoreDisassembler dasm)
        {
            var uDisp24 = ((uInstr & 0xFF00) << 8) | (uInstr >> 16);
            var addr = Address.Ptr32(uDisp24);
            dasm.ops.Add(addr);
            return true;
        }

        private static Mutator<TriCoreDisassembler> PcSignedDisplacement(int bitpos, int length)
        {
            var bf = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var disp = (bf.ReadSigned(u)) << 1;
                d.ops.Add(d.addr + disp);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> sdisp8_4 = PcSignedDisplacement(8, 4);
        private static readonly Mutator<TriCoreDisassembler> sdisp8_8 = PcSignedDisplacement(8, 8);
        private static readonly Mutator<TriCoreDisassembler> sdisp16_15 = PcSignedDisplacement(16, 15);

        private static Mutator<TriCoreDisassembler> PcUnsignedDisplacement(int bitpos, int length, uint offset)
        {
            var bf = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var disp = (bf.Read(u) + (uint)offset) << 1;
                d.ops.Add(d.addr + disp);
                return true;
            };
        }
        private static readonly Mutator<TriCoreDisassembler> udisp8_4 = PcUnsignedDisplacement(8, 4, 0);
        private static readonly Mutator<TriCoreDisassembler> udisp8_4_plus16 = PcUnsignedDisplacement(8, 4, 0);

        private static bool ndisp8_4(uint uInstr, TriCoreDisassembler dasm)
        {
            var disp = (int) (0xFFFF_FFE0u | ((uInstr >> 7) & 0x1E));
            dasm.ops.Add(dasm.addr + disp);
            return true;
        }

        private static InstrDecoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> Instr(Mnemonic mnemonic, params Mutator<TriCoreDisassembler>[] mutators)
            => Instr(mnemonic, InstrClass.Linear, mutators);

        private static InstrDecoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<TriCoreDisassembler>[] mutators)
        {
            return new InstrDecoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction>(iclass, mnemonic, mutators);
        }

        private static Instr32Decoder Instr32(Decoder<TriCoreDisassembler, Mnemonic, TriCoreInstruction> decoder)
        {
            return new Instr32Decoder(decoder);
        }

        static TriCoreDisassembler()
        {
            const InstrClass LinPri = InstrClass.Linear | InstrClass.Privileged;

            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi = Instr(Mnemonic.Nyi, InstrClass.Invalid);

            var addsc_a_SRRS = Instr(Mnemonic.addsc_a, A8, A12, d15, uimm6_2);
            
            rootDecoder = Sparse(0, 8, "TriCore", Instr32(nyi),
                (0x00, Sparse(12, 4, "  0x00", nyi,
                    (0x0, Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding)),
                    (0x7, Instr(Mnemonic.fret, InstrClass.Transfer | InstrClass.Return)),
                    (0x8, Instr(Mnemonic.rfe, InstrClass.Transfer | InstrClass.Return)),
                    (0x9, Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return)),
                    (0xA, Instr(Mnemonic.debug)))),
                (0x01, Instr32(Sparse(20, 8, "  0x01", invalid,
                    (0x00, Instr(Mnemonic.mov_aa, A28, A12)),
                    (0x01, Instr(Mnemonic.add_a, A28, A8, A12)),
                    (0x02, Instr(Mnemonic.sub_a, A28, A8, A12)),
                    (0x40, Instr(Mnemonic.eq_a, D28, A8, A12)),
                    (0x41, Instr(Mnemonic.ne_a, D28, A8, A12)),
                    (0x42, Instr(Mnemonic.lt_a, D28, A8, A12)),
                    (0x43, Instr(Mnemonic.ge_a, D28, A8, A12)),
                    (0x48, Instr(Mnemonic.eqz_a, D28, A8)),
                    (0x49, Instr(Mnemonic.nez_a, D28, A8)),
                    (0x4C, Instr(Mnemonic.mov_d, D28, A12)),
                    (0x60, Instr(Mnemonic.addsc_a, A28, A12, D8, uimm16_2)),
                    (0x62, nyi), // addsc.at
                    (0x63, Instr(Mnemonic.mov_a, A28, D12))
                    ))),
                (0x02, Instr(Mnemonic.mov, D8, D12)),
                (0x03, Instr32(Sparse(20, 8, "  0x03", nyi,
                    (0x02, Instr(Mnemonic.madd_u, D28, D24, D8, D12)),
                    (0x0A, Instr(Mnemonic.madd, D28, D24, D8, D12)),
                    (0x26, invalid),
                    (0x46, invalid),
                    (0x83, invalid),
                    (0xF0, invalid)
                    ))),
                (0x04, Instr(Mnemonic.ld_bu, D8, Mu8_pi12)),
                (0x05, Instr(Mnemonic.ld_b, D8, ABS)),
                (0x06, Instr(Mnemonic.sh, D8, simm12_4)),
                (0x07, Instr32(Mask(21, 2, "  0x07",
                    Instr(Mnemonic.nand_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.orn_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.xnor_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.xor_t, D28, D8, uimm16_5, D12, uimm23_5)))),

                (0x08, Instr(Mnemonic.ld_bu, D8, Ma15_bo)),
                (0x09, Instr32(Sparse(22, 6, "  0x09", nyi,
                    (0x05, Instr(Mnemonic.ld_d, E8, Mw_12_soff28_4_16_6_post)),
                    (0x14, Instr(Mnemonic.ld_w, D8, Mw_12_soff28_4_16_6_pre)),
                    (0x26, nyi)))),
                (0x0A, invalid),
                (0x0B, Instr32(Sparse(20, 8, "  0x0B", nyi,
                    (0x00, Instr(Mnemonic.add, D28, D8, D12)),
                    (0x02, Instr(Mnemonic.adds, D28, D8, D12)),
                    (0x03, Instr(Mnemonic.adds_u, D28, D8, D12)),
                    (0x04, Instr(Mnemonic.addx, D28, D8, D12)),
                    (0x05, Instr(Mnemonic.addc, D28, D8, D12)),
                    (0x08, Instr(Mnemonic.sub, D28, D8, D12)),
                    (0x0A, Instr(Mnemonic.subs, D28, D8, D12)),
                    (0x0B, Instr(Mnemonic.subs_u, D28, D8, D12)),
                    (0x0C, Instr(Mnemonic.subx, D28, D8, D12)),
                    (0x0D, Instr(Mnemonic.subc, D28, D8, D12)),
                    (0x0E, Instr(Mnemonic.absdif, D28, D8, D12)),
                    (0x0F, Instr(Mnemonic.absdifs, D28, D8, D12)),
                    (0x10, Instr(Mnemonic.eq, D28, D8, D12)),
                    (0x11, Instr(Mnemonic.ne, D28, D8, D12)),
                    (0x12, Instr(Mnemonic.lt, D28, D8, D12)),
                    (0x13, Instr(Mnemonic.lt_u, D28, D8, D12)),
                    (0x14, Instr(Mnemonic.ge, D28, D8, D12)),
                    (0x15, Instr(Mnemonic.ge_u, D28, D8, D12)),
                    (0x18, Instr(Mnemonic.min, D28, D8, D12)),
                    (0x19, Instr(Mnemonic.min_u, D28, D8, D12)),
                    (0x1A, Instr(Mnemonic.max, D28, D8, D12)),
                    (0x1B, Instr(Mnemonic.max_u, D28, D8, D12)),
                    (0x1D, Instr(Mnemonic.abss, D28, D12)),
                    (0x1F, Instr(Mnemonic.mov, D28, D12)),
                    (0x20, Instr(Mnemonic.and_eq, D28, D8, D12)),
                    (0x21, Instr(Mnemonic.and_ne, D28, D8, D12)),
                    (0x22, Instr(Mnemonic.and_lt, D28, D8, D12)),
                    (0x23, Instr(Mnemonic.and_lt_u, D28, D8, D12)),
                    (0x24, Instr(Mnemonic.and_ge, D28, D8, D12)),
                    (0x25, Instr(Mnemonic.and_ge_u, D28, D8, D12)),

                    (0x27, Instr(Mnemonic.or_eq, D28, D8, D12)),
                    (0x28, Instr(Mnemonic.or_ne, D28, D8, D12)),
                    (0x29, Instr(Mnemonic.or_lt, D28, D8, D12)),
                    (0x2A, Instr(Mnemonic.or_lt_u, D28, D8, D12)),
                    (0x2B, Instr(Mnemonic.or_ge, D28, D8, D12)),
                    (0x2C, Instr(Mnemonic.or_ge_u, D28, D8, D12)),

                    (0x2F, Instr(Mnemonic.xor_eq, D28, D8, D12)),
                    (0x30, Instr(Mnemonic.xor_ne, D28, D8, D12)),
                    (0x31, Instr(Mnemonic.xor_lt, D28, D8, D12)),
                    (0x32, Instr(Mnemonic.xor_lt_u, D28, D8, D12)),
                    (0x33, Instr(Mnemonic.xor_ge, D28, D8, D12)),
                    (0x34, Instr(Mnemonic.xor_ge_u, D28, D8, D12)),

                    (0x37, Instr(Mnemonic.sh_eq, D28, D8, D12)),
                    (0x38, Instr(Mnemonic.sh_ne, D28, D8, D12)),
                    (0x39, Instr(Mnemonic.sh_lt, D28, D8, D12)),
                    (0x3A, Instr(Mnemonic.sh_lt_u, D28, D8, D12)),
                    (0x3B, Instr(Mnemonic.sh_ge, D28, D8, D12)),
                    (0x3C, Instr(Mnemonic.sh_ge_u, D28, D8, D12)),

                    (0x40, Instr(Mnemonic.add_b, D28, D8, D12)),
                    (0x44, invalid),
                    (0x48, Instr(Mnemonic.sub_b, D28, D8, D12)),
                    (0x4B, invalid),
                    (0x4E, Instr(Mnemonic.absdif_b, D28, D8, D12)),
                    (0x50, Instr(Mnemonic.eq_b, D28, D8, D12)),
                    (0x52, Instr(Mnemonic.lt_b, D28, D8, D12)),
                    (0x53, Instr(Mnemonic.lt_bu, D28, D8, D12)),
                    (0x56, Instr(Mnemonic.eqany_b, D28, D8, D12)),
                    (0x58, Instr(Mnemonic.min_b, D28, D8, D12)),
                    (0x59, Instr(Mnemonic.min_bu, D28, D8, D12)),
                    (0x5A, Instr(Mnemonic.max_b, D28, D8, D12)),
                    (0x5B, Instr(Mnemonic.max_bu, D28, D8, D12)),
                    (0x5C, Instr(Mnemonic.abs_b, D28, D12)),
                    (0x5E, Instr(Mnemonic.sat_b, D28, D12)),
                    (0x5F, Instr(Mnemonic.sat_bu, D28, D12)),
                    (0x60, Instr(Mnemonic.add_h, D28, D8, D12)),
                    (0x62, Instr(Mnemonic.adds_h, D28, D8, D12)),
                    (0x63, Instr(Mnemonic.adds_hu, D28, D8, D12)),
                    (0x64, invalid),
                    (0x68, Instr(Mnemonic.sub_h, D28, D8, D12)),
                    (0x6A, Instr(Mnemonic.subs_h, D28, D8, D12)),
                    (0x6B, Instr(Mnemonic.subs_hu, D28, D8, D12)),
                    (0x6E, Instr(Mnemonic.absdif_h, D28, D8, D12)),
                    (0x6F, Instr(Mnemonic.absdifs_h, D28, D8, D12)),
                    (0x70, Instr(Mnemonic.eq_h, D28, D8, D12)),
                    (0x72, Instr(Mnemonic.lt_h, D28, D8, D12)),
                    (0x73, Instr(Mnemonic.lt_hu, D28, D8, D12)),
                    (0x76, Instr(Mnemonic.eqany_h, D28, D8, D12)),
                    (0x7A, Instr(Mnemonic.max_h, D28, D8, D12)),
                    (0x7B, Instr(Mnemonic.max_hu, D28, D8, D12)),
                    (0x7C, Instr(Mnemonic.abs_h, D12)),
                    (0x7D, Instr(Mnemonic.abss_h, D12)),
                    (0x7E, Instr(Mnemonic.sat_h, D28, D12)),
                    (0x7F, Instr(Mnemonic.sat_hu, D28, D12)),

                    (0x80, Instr(Mnemonic.mov, E28, D12)),
                    (0x81, Instr(Mnemonic.mov, E28, D8, D12)),
                    (0x90, Instr(Mnemonic.eq_w, D28, D8, D12)),
                    (0x92, Instr(Mnemonic.lt_w, D28, D8, D12)),
                    (0x93, Instr(Mnemonic.lt_wu, D28, D8, D12)),
                    (0xF1, invalid)))),
                (0x0C, Instr(Mnemonic.ld_bu, d15, Mub_12_off8_4)),
                (0x0D, Instr32(Sparse(22, 6, "  0x0D", nyi,
                    (0x00, Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding)),
                    (0x03, Instr(Mnemonic.fret, InstrClass.Transfer | InstrClass.Return)),
                    (0x04, Instr(Mnemonic.debug)),
                    (0x05, Instr(Mnemonic.rfm, InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged)),
                    (0x06, Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return)),
                    (0x07, Instr(Mnemonic.rfe, InstrClass.Transfer | InstrClass.Return)),
                    (0x08, Instr(Mnemonic.svlcx)),
                    (0x09, Instr(Mnemonic.rslcx)),
                    (0x0C, Instr(Mnemonic.enable, LinPri)),
                    (0x0D, Instr(Mnemonic.disable, LinPri)),
                    (0x0E, Instr(Mnemonic.restore, LinPri, D8)),
                    (0x0F, Instr(Mnemonic.disable, LinPri, D8)),
                    (0x12, Instr(Mnemonic.dsync)),
                    (0x13, Instr(Mnemonic.isync)),
                    (0x14, Instr(Mnemonic.trapv)),
                    (0x15, Instr(Mnemonic.trapsv)),
                    (0x16, Instr(Mnemonic.wait)),
                    (0x38, invalid)
                    ))),
                (0x0E, Instr(Mnemonic.jltz, InstrClass.ConditionalTransfer, D12, udisp8_4)),
                (0x0F, Instr32(Sparse(20, 8, "  0x0F", invalid,
                    (0x00, Instr(Mnemonic.sh, D28, D8, D12)),
                    (0x01, Instr(Mnemonic.sha, D28, D8, D12)),
                    (0x02, Instr(Mnemonic.shas, D28, D8, D12)),
                    (0x08, Instr(Mnemonic.and, D28, D8, D12)),
                    (0x09, Instr(Mnemonic.nand, D28, D8, D12)),
                    (0x0A, Instr(Mnemonic.or, D28, D8, D12)),
                    (0x0B, Instr(Mnemonic.nor, D28, D8, D12)),
                    (0x0C, Instr(Mnemonic.xor, D28, D8, D12)),
                    (0x0D, Instr(Mnemonic.xnor, D28, D8, D12)),
                    (0x0E, Instr(Mnemonic.andn, D28, D8, D12)),
                    (0x0F, Instr(Mnemonic.orn, D28, D8, D12)),
                    (0x1B, nyi/*CLZ*/),
                    (0x1C, nyi/*CLO*/),
                    (0x1D, nyi/*CLS*/),
                    (0x40, Instr(Mnemonic.sh_h, D28, D8, D12)),
                    (0x41, Instr(Mnemonic.sha_h, D28, D8, D12)),
                    (0x7C, nyi/*CLZ.H*/),
                    (0x7D, nyi/*CLO.H*/),
                    (0x7E, nyi/*CLS.H*/)
                    ))),

                (0x10, addsc_a_SRRS),
                (0x11, Instr32(Instr(Mnemonic.addih_a, A28, A8, uimm12_16))),
                (0x12, Instr(Mnemonic.add, D8, d15, D12)),
                (0x13, Instr32(Mask(21, 3, "  0x13",
                    invalid,
                    Instr(Mnemonic.madd, D28, D24, D8, simm12_9),
                    Instr(Mnemonic.madd_u, D28, D24, D8, simm12_9),
                    Instr(Mnemonic.madd, E28, E24, E8, simm12_9),

                    Instr(Mnemonic.madds_u, D28, D24, D8, simm12_9),
                    Instr(Mnemonic.madds, D28, D24, D8, simm12_9),
                    Instr(Mnemonic.madds_u, E28, E24, E8, simm12_9),
                    Instr(Mnemonic.madds, E28, E24, E8, simm12_9)))),
                (0x14, Instr(Mnemonic.ld_bu, D8, Mb_12)),
                (0x15, Instr32(Mask(26, 2, "  0x15",
                    Instr(Mnemonic.stlcx, ABS),
                    Instr(Mnemonic.stucx, ABS),
                    Instr(Mnemonic.ldlcx, ABS),
                    Instr(Mnemonic.lducx, ABS)))),
                (0x16, Instr(Mnemonic.and, d15, uimm8_8)),
                (0x17, Instr32(Sparse(21, 3, "  0x17", nyi,
                    (0x00, Instr(Mnemonic.insert, D28, D8, D12, E24)),
                    (0x02, Instr(Mnemonic.extr, D28, D8, D12, E24)),
                    (0x03, Instr(Mnemonic.extr_u, D28, D8, D12, E24)),
                    (0x04, Instr(Mnemonic.dextr, D28, D8, D12, D24))))),
                (0x18, invalid),
                (0x19, Instr32(Instr(Mnemonic.ld_w, D8, Mw_12_soff22_6_28_4_16_6))),
                (0x1B, Instr32(Instr(Mnemonic.addi, D28, D8, simm12_16))),
                (0x1C, invalid),
                (0x1D, Instr32(Instr(Mnemonic.j, InstrClass.Transfer, B))),
                (0x1E, invalid),
                (0x1F, invalid),

                (0x20, Instr(Mnemonic.sub_a, A8, uimm8_8)),
                (0x21, invalid),
                (0x22, Instr(Mnemonic.adds, D8, D12)),
                (0x23, Instr32(Sparse(16, 8, "  0x23", nyi,
                    (0x02, invalid),
                    (0x8A, Instr(Mnemonic.msubs, D24, D28, D8, D12)), 
                    (0xDF, invalid),
                    (0xEA, Instr(Mnemonic.msubs, E24, E28, D8, D12))))), 
                (0x24, Instr(Mnemonic.st_b, Mu8_pi12, D8)),
                (0x26, Instr(Mnemonic.and, D8, D12)),
                (0x27, Instr32(Mask(21, 2, "  0x27",
                    Instr(Mnemonic.sh_and_t, D28, D8, uimm16_5, D12, uimm23_5),
                    nyi,
                    nyi,
                    nyi))),
                (0x28, Instr(Mnemonic.st_b, Ma15_bo, D8)),
                (0x2A, Instr(Mnemonic.cmov, D8, d15, D12)),
                (0x2B, Instr32(Sparse(20, 4, "  0x2B", nyi,
                    (0x04, Instr(Mnemonic.sel, D28, D24, D8, D12)),
                    (0x05, Instr(Mnemonic.seln, D28, D24, D8, D12))))),
                (0x2C, Instr(Mnemonic.st_b, Mub_12_off8_4, d15)),
                (0x2D, Instr32(Sparse(20, 8, "  0x2D", nyi,
                    (0x00, Instr(Mnemonic.calli, InstrClass.Transfer | InstrClass.Call | InstrClass.Indirect, A8)),
                    (0x02, Instr(Mnemonic.jli, InstrClass.Transfer | InstrClass.Call | InstrClass.Indirect, A8)),
                    (0x31, invalid)))),
                (0x2E, Instr(Mnemonic.jz_t, InstrClass.ConditionalTransfer, d15, uimm12_4, udisp8_4)),

                (0x30, Instr(Mnemonic.add_a, A8, A12)),
                (0x31, invalid),
                (0x32, Sparse(12, 4, "  0x32", invalid,
                    (0x00, Instr(Mnemonic.sat_b, D8)),
                    (0x01, Instr(Mnemonic.sat_bu, D8)),
                    (0x02, Instr(Mnemonic.sat_h, D8)),
                    (0x03, Instr(Mnemonic.sat_hu, D8)),
                    (0x05, Instr(Mnemonic.rsub, D8)))),
                (0x33, Instr32(Sparse(21, 3, "  0x33", nyi,
                    (0x00, invalid),
                    (0x05, Instr(Mnemonic.msubs, D28, E24, D8, simm12_9)),
                    (0x07, Instr(Mnemonic.msubs, E28, E24, D8, simm12_9))))),
                (0x34, Instr(Mnemonic.st_b, Mb_12, D8)),
                (0x35, invalid),
                (0x37, Instr32(Instr(Mnemonic.extr, D28, D8, uimm23_5, uimm16_5))),
                (0x38, invalid),
                (0x39, Instr32(Instr(Mnemonic.ld_bu, D8, Mub_12_soff22_6_28_4_16_6))),
                (0x3A, Instr(Mnemonic.eq, d15, D8, D12)),
                (0x3B, Instr32(Instr(Mnemonic.mov, D28, simm12_16))),
                (0x3C, Instr(Mnemonic.j, InstrClass.Transfer, sdisp8_8)),
                (0x3E, invalid),
                (0x3F, invalid),

                (0x40, Instr(Mnemonic.mov_aa, A8, A12)),
                (0x41, invalid),
                (0x42, Instr(Mnemonic.add, D8, D12)),
                (0x44, Instr(Mnemonic.ld_w, D8, Mw_pi12)),
                (0x46, If(12, 4, u => u == 0, Instr(Mnemonic.not, D8))),
                (0x47, Instr32(Mask(21, 2, "  0x47",
                    Instr(Mnemonic.and_and_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.and_or_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.and_nor_t, D28, D8, uimm16_5, D12, uimm23_5),
                    Instr(Mnemonic.and_andn_t, D28, D8, uimm16_5, D12, uimm23_5)))),
                (0x48, Instr(Mnemonic.ld_w, D8, Ma15_wo)),
                (0x49, Instr32(Sparse(22, 6, "  0x49", nyi,
                    (0x00, Instr(Mnemonic.swap_w, Mw_12_soff28_4_16_6_post, D8)),
                    (0x01, Instr(Mnemonic.ldmst, Mw_12_soff28_4_16_6_post, E8)),
                    (0x03, Instr(Mnemonic.cmpswap_w, Mw_12_soff28_4_16_6, E8)),
                    (0x10, Instr(Mnemonic.swap_w, Mw_12_soff28_4_16_6_pre, D8)),
                    (0x11, Instr(Mnemonic.ldmst, Mw_12_soff28_4_16_6, E8)),
                    (0x13, Instr(Mnemonic.cmpswap_w, Mw_12_soff28_4_16_6_pre, E8)),
                    (0x20, Instr(Mnemonic.swap_w, Mw_12_soff28_4_16_6, D8)),
                    (0x21, Instr(Mnemonic.ldmst, Mw_12_soff28_4_16_6_pre, E8)),
                    (0x24, Instr(Mnemonic.ldlcx, Mw_12_soff28_4_16_6_pre)),
                    (0x25, Instr(Mnemonic.lducx, Mw_12_soff28_4_16_6)),
                    (0x26, Instr(Mnemonic.stlcx, Mw_12_soff28_4_16_6)),
                    (0x27, Instr(Mnemonic.stucx, Mw_12_soff28_4_16_6)),
                    (0x28, Instr(Mnemonic.lea, A8, Mb_12_soff28_4_16_6_post))))),
                (0x4A, invalid),
                (0x4B, Instr32(Sparse(20, 8, "  0x4B", nyi,
                    (0x00, If(16, 2, u => u == 1, Instr(Mnemonic.cmp_f, D28, D8, D12))),
                    (0x02, If(16, 2, u => u == 1, Instr(Mnemonic.add_f, D28, D8, D12))),
                    (0x04, If(16, 2, u => u == 1, Instr(Mnemonic.mul_f, D28, D8, D12))),
                    (0x05, If(16, 2, u => u == 1, Instr(Mnemonic.div_f, D28, D8, D12))),
                    (0x08, Instr(Mnemonic.unpack, E28, D8)),
                    (0x0C, If(16, 2, u => u == 1, Instr(Mnemonic.updfl, D8))),
                    (0x10, If(16, 2, u => u == 1, Instr(Mnemonic.ftoi, D28, D8))),
                    (0x11, If(16, 2, u => u == 1, Instr(Mnemonic.ftoq31, D28, D8))),
                    (0x12, If(16, 2, u => u == 1, Instr(Mnemonic.ftou, D28, D8))),
                    (0x13, If(16, 2, u => u == 1, Instr(Mnemonic.ftoiz, D28, D8))),
                    (0x14, If(16, 2, u => u == 1, Instr(Mnemonic.itof, D28, D8))),
                    (0x15, If(16, 2, u => u == 1, Instr(Mnemonic.q31tof, D28, D8, D12))),
                    (0x16, If(16, 2, u => u == 1, Instr(Mnemonic.utof, D28, D8))),
                    (0x17, If(16, 2, u => u == 1, Instr(Mnemonic.ftouz, D28, D8))),
                    (0x18, If(16, 2, u => u == 1, Instr(Mnemonic.ftoq31z, D28, D8))),
                    (0x19, If(16, 2, u => u == 1, Instr(Mnemonic.qseed_f, D28, D8))),
                    (0x20, If(16, 2, u => u == 1, Instr(Mnemonic.div, E28, D8, D12))),
                    (0x21, If(16, 2, u => u == 1, Instr(Mnemonic.div_u, E28, D8, D12))),
                    (0x28, invalid),
                    (0xF8, invalid)))),
                (0x4C, Instr(Mnemonic.ld_w, d15, Mw_12_off8_4)),
                (0x4E, Instr(Mnemonic.jgtz, InstrClass.ConditionalTransfer, D12, udisp8_4)),
                (0x4D, Instr32(Instr(Mnemonic.mfcr, D8, CR))),

                (0x50, addsc_a_SRRS),
                (0x53, Instr32(Sparse(20, 8, "  0x53", nyi,
                    (0x01, Instr(Mnemonic.mul, D28, D8, simm12_9)),
                    (0x02, Instr(Mnemonic.mul_u, E28, D8, simm12_9)),
                    (0x03, Instr(Mnemonic.mul, E28, D8, simm12_9))))),
                (0x54, Instr(Mnemonic.ld_w, D8, Mw_12)),
                (0x55, invalid),
                (0x57, Instr32(Sparse(21, 3, "  0x57", nyi,
                    (0x2, Instr(Mnemonic.extr, D28, D8, D24, uimm16_5))))),
                (0x58, Instr(Mnemonic.ld_w, d15, Msp_wo8)),
                (0x59, Instr32(Instr(Mnemonic.st_w, Mw_12_soff22_6_28_4_16_6, D8))),
                (0x5A, Instr(Mnemonic.sub, d15, D8, D12)),
                (0x5B, invalid),
                (0x5C, Instr(Mnemonic.call, InstrClass.Transfer | InstrClass.Call, sdisp8_8)),
                (0x5D, Instr32(Instr(Mnemonic.jl, InstrClass.Transfer | InstrClass.Call, B))),
                (0x5E, Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, d15, simm12_4, udisp8_4)),
                (0x5F, Instr32(Mask(31, 1, "  0x5F",
                    Instr(Mnemonic.jeq, InstrClass.ConditionalTransfer, D8, D12, sdisp16_15),
                    Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, D8, D12, sdisp16_15)))),

                (0x60, Instr(Mnemonic.mov_a, A8, D12)),
                (0x61, Instr32(Instr(Mnemonic.fcall, B))),
                (0x62, Instr(Mnemonic.adds, D8, D12)),
                (0x63, Instr32(Sparse(18, 6, "  0x63", nyi,
                    (0x01, nyi)))), // msub.q
                (0x64, Instr(Mnemonic.st_w, Mw_pi12, D8)),
                (0x66, invalid),
                (0x68, Instr(Mnemonic.st_w, Ma15_wo, D8)),
                (0x69, Instr32(Sparse(22, 6, "  0x69", nyi))),
                (0x6A, Instr(Mnemonic.cmovn, D8, d15, D12)),
                (0x6B, Instr32(Sparse(20, 4, "  0x6B", nyi,
                    (0x03, If(16, 2, u => u == 1, Instr(Mnemonic.fsub_f, D28, D24, D8))),
                    (0x06, If(16, 2, u => u == 1, Instr(Mnemonic.madd_f, D28, D24, D8, D12))),
                    (0x07, If(16, 2, u => u == 1, Instr(Mnemonic.msub_f, D28, D24, D8, D12)))))),
                (0x6C, Instr(Mnemonic.st_w, Mw_12_off8_4, d15)),
                (0x6D, Instr32(Instr(Mnemonic.call, InstrClass.Transfer | InstrClass.Call, B))),
                (0x6E, Instr(Mnemonic.jz, InstrClass.ConditionalTransfer, d15, sdisp8_8)),
                (0x6F, Instr32(Mask(31, 1, "  0x6F",
                    Instr(Mnemonic.jz_t, InstrClass.ConditionalTransfer, D8, u8_7_1_12_4, sdisp16_15),
                    Instr(Mnemonic.jnz_t, InstrClass.ConditionalTransfer, D8, u8_7_1_12_4, sdisp16_15)))),

                (0x70, invalid),
                (0x74, Instr(Mnemonic.st_w, Mw_12, D8)),
                (0x75, invalid),
                (0x76, Instr(Mnemonic.jz, InstrClass.ConditionalTransfer, D12, udisp8_4)),
                (0x77, Instr32(Mask(21, 2, "  0x77",
                    nyi,
                    invalid,
                    invalid,
                    invalid))),
                (0x78, Instr(Mnemonic.st_w, Msp_wo8, d15)),
                (0x79, Instr32(Instr(Mnemonic.ld_b, D8, Mb_12_soff22_6_28_4_16_6))),
                (0x7B, Instr32(Instr(Mnemonic.movh, D28, uimm12_16))),
                (0x7C, Instr(Mnemonic.jnz_a, InstrClass.ConditionalTransfer, A12, udisp8_4)),
                (0x7D, Instr32(Mask(31, 1, "  0x7D",
                    Instr(Mnemonic.jeq_a, InstrClass.ConditionalTransfer, A8, A12, sdisp16_15),
                    Instr(Mnemonic.jne_a, InstrClass.ConditionalTransfer, A8, A12, sdisp16_15)))),
                (0x7E, Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, d15, D8, udisp8_4)),
                (0x7F, Instr32(Mask(31, 1, "  0x7F",
                    Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, D8, D12, sdisp16_15),
                    Instr(Mnemonic.jge_u, InstrClass.ConditionalTransfer, D8, D12, sdisp16_15)))),

                (0x80, Instr(Mnemonic.mov_d, D8, A12)),
                (0x82, Instr(Mnemonic.mov, D8, simm12_4)),
                (0x83, Instr32(Sparse(18, 8, "  0x83", nyi,
                    (0x9B, invalid),
                    (0xE1, invalid)))),
                (0x84, Instr(Mnemonic.ld_h, D8, Mh_pi12)),
                (0x85, Instr32(Mask(26, 2, "  0x85",
                    Instr(Mnemonic.ld_w, D8, ABS),
                    Instr(Mnemonic.ld_d, E8, ABS),
                    Instr(Mnemonic.ld_a, A8, ABS),
                    nyi))),
                (0x86, Instr(Mnemonic.sha, D8, simm12_4)),
                (0x89, Instr32(Sparse(26, 6, "  0x89", nyi,
                    (0x00, Instr(Mnemonic.st_b, Mb_12_soff28_4_16_6_post, D8)),
                    (0x02, Instr(Mnemonic.st_h, Mh_12_soff28_4_16_6_post, D8)),
                    (0x17, Instr(Mnemonic.st_da, Md_12_soff22_6_28_4_16_6, P8)),
                    (0x2E, Instr(Mnemonic.cachea_i, LinPri, Mw_12_soff28_4_16_6))))),
                (0x8A, Instr(Mnemonic.cadd, D8, d15, simm12_4)),
                (0x8B, Instr32(Sparse(21, 7, "  0x8B", nyi,
                    (0x00, Instr(Mnemonic.add, D28, D8, simm12_9)),
                    (0x02, Instr(Mnemonic.adds, D28, D8, simm12_9)),
                    (0x03, Instr(Mnemonic.adds_u, D28, D8, simm12_9)),
                    (0x04, Instr(Mnemonic.addx, D28, D8, simm12_9)),
                    (0x05, Instr(Mnemonic.addc, D28, D8, simm12_9)),
                    (0x08, Instr(Mnemonic.rsub, D28, D8, simm12_9)),
                    (0x0A, Instr(Mnemonic.rsubs, D28, D8, simm12_9)),
                    (0x0B, Instr(Mnemonic.rsubs_u, D28, D8, simm12_9)),
                    (0x0E, Instr(Mnemonic.absdif, D28, D8, simm12_9)),
                    (0x0F, Instr(Mnemonic.absdifs, D28, D8, simm12_9)),
                    (0x10, Instr(Mnemonic.eq, D28, D8, simm12_9)),
                    (0x11, Instr(Mnemonic.ne, D28, D8, simm12_9)),
                    (0x12, Instr(Mnemonic.lt, D28, D8, simm12_9)),
                    (0x13, Instr(Mnemonic.lt_u, D28, D8, simm12_9)),
                    (0x14, Instr(Mnemonic.ge, D28, D8, simm12_9)),
                    (0x15, Instr(Mnemonic.ge_u, D28, D8, simm12_9)),
                    (0x18, Instr(Mnemonic.min, D28, D8, simm12_9)),
                    (0x19, Instr(Mnemonic.min_u, D28, D8, simm12_9)),
                    (0x1A, Instr(Mnemonic.max, D28, D8, simm12_9)),
                    (0x1B, Instr(Mnemonic.max_u, D28, D8, simm12_9)),
                    (0x20, Instr(Mnemonic.and_eq, D28, D8, simm12_9)),
                    (0x21, Instr(Mnemonic.and_ne, D28, D8, simm12_9)),
                    (0x22, Instr(Mnemonic.and_lt, D28, D8, simm12_9)),
                    (0x24, Instr(Mnemonic.and_ge, D28, D8, simm12_9)),
                    (0x25, Instr(Mnemonic.and_ge_u, D28, D8, simm12_9)),
                    (0x27, Instr(Mnemonic.or_eq, D28, D8, simm12_9)),
                    (0x28, Instr(Mnemonic.or_ne, D28, D8, simm12_9)),
                    (0x29, Instr(Mnemonic.or_lt, D28, D8, simm12_9)),
                    (0x2A, Instr(Mnemonic.or_lt_u, D28, D8, simm12_9)),
                    (0x2B, Instr(Mnemonic.or_ge, D28, D8, simm12_9)),
                    (0x2C, Instr(Mnemonic.or_ge_u, D28, D8, simm12_9)),
                    (0x2F, Instr(Mnemonic.xor_eq, D28, D8, simm12_9)),
                    (0x30, Instr(Mnemonic.xor_ne, D28, D8, simm12_9)),
                    (0x31, Instr(Mnemonic.xor_lt, D28, D8, simm12_9)),
                    (0x32, Instr(Mnemonic.xor_lt_u, D28, D8, simm12_9)),
                    (0x33, Instr(Mnemonic.xor_ge, D28, D8, simm12_9)),
                    (0x34, Instr(Mnemonic.xor_ge_u, D28, D8, simm12_9)),
                    (0x37, Instr(Mnemonic.sh_eq, D28, D8, simm12_9)),
                    (0x38, Instr(Mnemonic.sh_ne, D28, D8, simm12_9)),
                    (0x39, Instr(Mnemonic.sh_lt, D28, D8, simm12_9)),
                    (0x3A, Instr(Mnemonic.sh_lt_u, D28, D8, simm12_9)),
                    (0x3B, Instr(Mnemonic.sh_ge, D28, D8, simm12_9)),
                    (0x3C, Instr(Mnemonic.sh_ge_u, D28, D8, simm12_9)),
                    (0x56, Instr(Mnemonic.eqany_b, D28, D8, simm12_9)),
                    (0x76, Instr(Mnemonic.eqany_h, D28, D8, simm12_9))))),
                (0x8C, Instr(Mnemonic.ld_h, d15, Mh_12_off8_4)),
                (0x8F, Instr32(Sparse(21, 7, "  0x8F", nyi,
                    (0x00, Instr(Mnemonic.sh, D28, D8, uimm12_6)),
                    (0x01, Instr(Mnemonic.sha, D28, D8, uimm12_6)),
                    (0x02, Instr(Mnemonic.shas, D28, D8, uimm12_6)),
                    (0x07, Instr(Mnemonic.shuffle, D28, D8, uimm12_9)),
                    (0x08, Instr(Mnemonic.and, D28, D8, uimm12_9)),
                    (0x09, Instr(Mnemonic.nand, D28, D8, uimm12_9)),
                    (0x0A, Instr(Mnemonic.or, D28, D8, uimm12_9)),
                    (0x0B, Instr(Mnemonic.nor, D28, D8, uimm12_9)),
                    (0x0C, Instr(Mnemonic.xor, D28, D8, uimm12_9)),
                    (0x0D, Instr(Mnemonic.xnor, D28, D8, uimm12_9)),
                    (0x0E, Instr(Mnemonic.andn, D28, D8, uimm12_9)),
                    (0x0F, Instr(Mnemonic.orn, D28, D8, uimm12_9)),
                    (0x40, Instr(Mnemonic.sh_h, D28, D8, uimm12_9)),
                    (0x41, Instr(Mnemonic.sha_h, D28, D8, uimm12_9))))),

                (0x90, addsc_a_SRRS),
                (0x91, Instr32(Instr(Mnemonic.movh_a, A28, uimm12_16))),
                (0x92, Instr(Mnemonic.add, D8, d15, simm12_4)),
                (0x94, Instr(Mnemonic.ld_h, D8, Mh_12)),
                (0x96, Instr(Mnemonic.or, d15, uimm8_8)),
                (0x97, Instr32(Instr(Mnemonic.insert, D28, D8, uimm12_4, D24))),
                (0x99, Instr32(Instr(Mnemonic.ld_a, A8, Mw_12_soff22_6_28_4_16_6))),
                (0x9B, Instr32(Instr(Mnemonic.addih, D28, D8, uimm12_16))),
                (0x9E, invalid),
                (0x9F, invalid),

                (0xA0, Instr(Mnemonic.mov_a, A8, uimm12_4)),
                (0xA2, Instr(Mnemonic.sub, D8, D12)),
                (0xA3, Instr32(Sparse(18, 6, "  A3", nyi,
                    (0x1A, Instr(Mnemonic.msub_h, E28, E24, D8, D12, uimm16_2))))),
                (0xA4, Instr(Mnemonic.st_h, Mh_pi12, D8)),
                (0xA5, Instr32(Instr(Mnemonic.st_a, ABS, A8))),
                (0xA6, Instr(Mnemonic.or, D8, D12)),
                (0xAB, Instr32(Mask(21, 3, "  0xAB",
                    Instr(Mnemonic.cadd, D28, D24, D8, simm12_9),
                    Instr(Mnemonic.caddn, D28, D24, D8, simm12_9),
                    nyi,
                    nyi,

                    Instr(Mnemonic.sel, D28, D24, D8, simm12_9),
                    nyi,
                    nyi,
                    nyi))),
                (0xAC, Instr(Mnemonic.st_h, Mh_12_off8_4, d15)),
                (0xAE, Instr(Mnemonic.jnz_t, InstrClass.ConditionalTransfer, d15, uimm12_4, udisp8_4)),
                (0xAF, invalid),

                (0xB0, Instr(Mnemonic.add_a, A8, simm12_4)),
                (0xB4, Instr(Mnemonic.st_h, Mh_12, D8)),
                (0xB5, Instr32(Instr(Mnemonic.st_a, Mw_12_soff22_6_28_4_16_6, D8))),
                (0xB7, Instr32(Mask(21, 2, "  0xB7",
                    Instr(Mnemonic.insert, D28, D8, uimm12_4, uimm23_5, uimm16_5),
                    Instr(Mnemonic.imask, E28, uimm12_4, uimm23_5, uimm16_5),
                    nyi,
                    nyi))),
                (0xB9, Instr32(Instr(Mnemonic.ld_hu, D8, Mhu_12_soff22_6_28_4_16_6))),
                (0xBA, Instr(Mnemonic.eq, d15, D8, simm12_4)),
                (0xBB, Instr32(Instr(Mnemonic.mov_u, D28, uimm12_16))),
                (0xBC, Instr(Mnemonic.jz_a, InstrClass.ConditionalTransfer, A12, udisp8_4)),
                (0xBE, Instr(Mnemonic.jeq, InstrClass.ConditionalTransfer, d15, D12, udisp8_4)),
                (0xBF, Instr32(Mask(31, 1, "  0xBF",
                    Instr(Mnemonic.jlt, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15),
                    Instr(Mnemonic.jlt_u, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15)))),

                (0xC0, invalid),
                (0xC2, Instr(Mnemonic.add, D8, simm12_4)),
                (0xC4, Instr(Mnemonic.ld_a, A8, Mw_pi12)),
                (0xC5, Instr32(Instr(Mnemonic.lha, A8, ABS))),
                (0xC6, Instr(Mnemonic.xor, D8, D12)),
                (0xC9, Instr32(Instr(Mnemonic.ld_h, D8, Mh_12_soff22_6_28_4_16_6))),
                (0xC8, Instr(Mnemonic.ld_a, A8, Ma15_wo)),
                (0xCD, Instr32(Instr(Mnemonic.mtcr, LinPri, CR, D8))),
                (0xCE, Instr(Mnemonic.jgez, InstrClass.ConditionalTransfer, D12, udisp8_4)),
                (0xCF, invalid),

                (0xD0, addsc_a_SRRS),
                (0xD1, invalid),
                (0xD4, Instr(Mnemonic.ld_a, A8, Mw_12)),
                (0xD6, invalid),
                (0xD8, Instr(Mnemonic.ld_a, a15, Msp_wo8)),
                (0xD9, Instr32(Instr(Mnemonic.lea, A8, Mw_12_soff22_6_28_4_16_6))),
                (0xDA, Instr(Mnemonic.mov, d15, uimm8_8)),
                (0xDC, If(12, 4, u => u == 0,
                    Instr(Mnemonic.ji, InstrClass.Transfer | InstrClass.Indirect, A8))),
                (0xDF, Instr32(Mask(31, 1, "  0xDF",
                    Instr(Mnemonic.jeq, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15),
                    Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15)))),

                (0xE0, Instr(Mnemonic.bisr, LinPri, D8, uimm8_8)),
                (0xE1, Instr32(Instr(Mnemonic.fcalla, InstrClass.Transfer | InstrClass.Call, B))),
                (0xE2, Instr(Mnemonic.mul, D8, D12)),
                (0xE4, Instr(Mnemonic.st_a, Mw_pi12, D8)),
                (0xE5, Instr32(Instr(Mnemonic.ldmst, ABS, E8))),
                (0xE6, invalid),
                (0xE8, Instr(Mnemonic.st_a, Ma15_wo, A8)),
                (0xE9, Instr32(Instr(Mnemonic.st_b, Mub_12_soff22_6_28_4_16_6, D8))),
                (0xEA, Instr(Mnemonic.cmovn, D8, d15, simm12_4)),
                (0xEB, invalid),
                (0xED, Instr32(Instr(Mnemonic.calla, Babs))),
                (0xEE, Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, d15, sdisp8_8)),
                (0xEF, invalid),

                (0xF0, invalid),
                (0xF1, invalid),
                (0xF2, invalid),
                (0xF3, invalid),
                (0xF4, Instr(Mnemonic.st_a, Mw_12, A8)),
                (0xF6, Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, D12, udisp8_4)),
                (0xFC, Instr(Mnemonic.loop, InstrClass.ConditionalTransfer, D12, ndisp8_4)),
                (0xF9, Instr32(Instr(Mnemonic.st_h, Mh_12_soff22_6_28_4_16_6, D8))),
                (0xFB, Instr32(Instr(Mnemonic.mov, E28, simm12_16))),
                (0xFD, Instr32(Mask(31, 1, "  0xFD",
                    Instr(Mnemonic.loop, InstrClass.ConditionalTransfer, A12, sdisp16_15),
                    invalid))),
                (0xFE, Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, d15, D12, udisp8_4_plus16)),
                (0xFF, Instr32(Mask(31, 1, "  0xFF",
                    Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15),
                    Instr(Mnemonic.jge_u, InstrClass.ConditionalTransfer, D8, simm12_4, sdisp16_15)))));
        }
    }
}