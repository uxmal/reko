#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;

#pragma warning disable IDE1006

namespace Reko.Arch.V850
{
    public class V850Disassembler : DisassemblerBase<V850Instruction, Mnemonic>
    {
        private static readonly MaskDecoder<V850Disassembler, Mnemonic, V850Instruction> rootDecoder;

        private readonly V850Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private ushort uInstr;      // The first 16-bit word of the instr.
        private int displacement;
        private Address addr;

        public V850Disassembler(V850Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override V850Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out this.uInstr))
                return null;
            displacement = 0;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override V850Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new V850Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray(),
            };
        }

        public override V850Instruction CreateInvalidInstruction()
        {
            return new V850Instruction
            {
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override V850Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("V850Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static Mutator<V850Disassembler> Reg(int bitpos, int bitlen)
        {
            var field = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> R = Reg(0, 5);
        private static readonly Mutator<V850Disassembler> r = Reg(11, 5);

        private static Mutator<V850Disassembler> Is(int bitpos, int bitlen)
        {
            var field = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = field.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> Is0_5 = Is(0, 5);

        private static Mutator<V850Disassembler> Iu(int bitpos, int bitlen)
        {
            var field = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = field.Read(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> Iu11_3 = Iu(11, 3);

        /// <summary>
        /// 32-bit immediate following the instruction.
        /// </summary>
        private static bool Imm32(uint uInstr, V850Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out uint imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32(imm));
            return true;
        }

        /// <summary>
        /// Signed 16-bit immediate following the instruction.
        /// </summary>
        private static bool Simm16(uint uInstr, V850Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short simm))
                return false;
            dasm.ops.Add(ImmediateOperand.Int32(simm));
            return true;
        }

        /// <summary>
        /// Signed 16-bit immediate following the instruction.
        /// </summary>
        private static bool Imm16(uint _, V850Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short simm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32(simm));
            return true;
        }

        /// <summary>
        /// Memory access with a 16-bit displacement.
        /// </summary>
        private static Mutator<V850Disassembler> Mdisp16(int bitpos, int length, PrimitiveType dt)
        {
            var baseRegField = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeInt16(out short disp))
                    return false;
                var baseReg = Registers.GpRegs[baseRegField.Read(u)];
                var mem = new MemoryOperand(dt, baseReg, disp);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> Mb = Mdisp16(0, 5, PrimitiveType.SByte);
        private static readonly Mutator<V850Disassembler> Mub = Mdisp16(0, 5, PrimitiveType.Byte);

        /// <summary>
        /// Memory access with a 16-bit displacement that was read before and stored in the <see cref="displacement"/>
        /// field.
        /// </summary>
        private static Mutator<V850Disassembler> Mdisp(PrimitiveType dt)
        {
            var baseRegField = new Bitfield(0, 5);
            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[baseRegField.Read(u)];
                var mem = new MemoryOperand(dt, baseReg, d.displacement);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> Mh_disp = Mdisp(PrimitiveType.Int16);
        private static readonly Mutator<V850Disassembler> Mw_disp = Mdisp(PrimitiveType.Word32);

        private static Mutator<V850Disassembler> Mdisp23(PrimitiveType dt)
        {
            var baseRegField = new Bitfield(0, 5);
            var dispLoField = new Bitfield(4, 7);
            return (u, d) =>
            {
                var dispLo = (ushort)dispLoField.Read(u);
                if (!d.rdr.TryReadUInt16(out ushort dispHi))
                    return false;
                var displacement = (int) ((uint) dispHi << 16) | dispLo;
                var baseReg = Registers.GpRegs[baseRegField.Read(d.uInstr)];
                var mem = new MemoryOperand(dt, baseReg, displacement);
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<V850Disassembler> Mdisp32(PrimitiveType dt)
        {
            var baseRegField = new Bitfield(0, 5);
            return (u, d) =>
            {
                if (!d.rdr.TryReadUInt16(out ushort dispLo))
                    return false;
                if (!d.rdr.TryReadUInt16(out ushort dispHi))
                    return false;
                var displacement = (int) ((uint) dispHi << 16) | dispLo;
                var baseReg = Registers.GpRegs[baseRegField.Read(u)];
                var mem = new MemoryOperand(dt, baseReg, displacement);
                d.ops.Add(mem);
                return true;
            };
        }

        private static readonly Mutator<V850Disassembler> Mw32 = Mdisp32(PrimitiveType.Word32);


        private static Mutator<V850Disassembler> Mep(int bitpos, int length, int mask, int shift, PrimitiveType dt)
        {
            var ep = Registers.ElementPtr;
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var offset =  (mask & (int) field.Read(u)) << shift;
                var mem = new MemoryOperand(dt, ep, offset);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<V850Disassembler> Mep_b = Mep(0, 7, ~0, 0, PrimitiveType.SByte);
        private static readonly Mutator<V850Disassembler> Mep_h = Mep(0, 7, ~1, 0, PrimitiveType.Int16);

        private static readonly Bitfield bits0_5 = new Bitfield(0, 5);

        private static bool disp22(uint uInstr, V850Disassembler dasm)
        {
            var dispLo = (ushort) dasm.displacement;
            var dispHi = (uint) bits0_5.ReadSigned(uInstr);
            dispHi = (dispHi << 16) | dispLo;
            dasm.ops.Add(AddressOperand.Create(dasm.addr + (int) dispHi));
            return true;
        }

        private static bool disp32(uint uInstr, V850Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort dispLo))
                return false;
            if (!dasm.rdr.TryReadUInt16(out ushort dispHi))
                return false;
            var disp = (uint) (dispHi << 16) | dispLo;
            dasm.ops.Add(AddressOperand.Create(dasm.addr + (int) disp));
            return true;
        }

        private static readonly Bitfield[] disp9_fields = Bf((11, 5), (4, 3));

        private static bool disp9(uint uInstr, V850Disassembler dasm)
        {
            var disp = Bitfield.ReadSignedFields(disp9_fields, uInstr) << 1;
            dasm.ops.Add(AddressOperand.Create(dasm.addr + disp));
            return true;
        }

        private class Mdisp16Decoder : Decoder<V850Disassembler, Mnemonic, V850Instruction>
        {
            private readonly Decoder<V850Disassembler, Mnemonic, V850Instruction> decoder;

            public Mdisp16Decoder(Decoder<V850Disassembler, Mnemonic, V850Instruction> decoder)
            {
                this.decoder = decoder;
            }

            public override V850Instruction Decode(uint wInstr, V850Disassembler dasm)
            {
                if (!dasm.rdr.TryReadLeInt16(out short disp))
                    return dasm.CreateInvalidInstruction();
                dasm.displacement = disp;
                return decoder.Decode((ushort)disp, dasm);
            }
        }

        //    return (u, d) =>
        //    {
        //        if (!d.rdr.TryReadLeInt16(out short disp))
        //            return false;
        //var(mn, dt) = instr[disp & 1];
        //        var baseReg = Registers.GpRegs[baseRegField.Read(u)];
    


    private static Decoder<V850Disassembler, Mnemonic, V850Instruction> Instr(Mnemonic mnemonic, params Mutator<V850Disassembler>[] mutators)
        {
            return new InstrDecoder<V850Disassembler, Mnemonic, V850Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder<V850Disassembler, Mnemonic, V850Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<V850Disassembler> [] mutators)
        {
            return new InstrDecoder<V850Disassembler, Mnemonic, V850Instruction>(iclass, mnemonic, mutators);
        }

        /// <summary>
        /// Memory access with a 16-bit displacement, where the LSB of the displacement selects 
        /// an instruction.
        /// </summary>
        private static Mdisp16Decoder Mdisp16_op(Decoder<V850Disassembler, Mnemonic, V850Instruction> decoder)
        {
            return new Mdisp16Decoder(decoder);
        }

        private static Decoder<V850Disassembler, Mnemonic, V850Instruction> Nyi(string message)
        {
            return new NyiDecoder<V850Disassembler, Mnemonic, V850Instruction>(message);
        }

        private static bool Ne0(uint u) => u != 0;

        static V850Disassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            var sldw_sstw = Mask(0, 1, "Sld.w/Sst.w",
                    Instr(Mnemonic.sld_w, Mep(1, 6, ~1, 0, PrimitiveType.Word32), r),
                    Instr(Mnemonic.sst_w, r, Mep(1, 6, ~1, 0, PrimitiveType.Word32)));

            var bcond_9 = Mask(0, 4, "   bcond",
                Instr(Mnemonic.bv, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bl, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bz, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bnh, InstrClass.ConditionalTransfer, disp9),

                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.br, InstrClass.Transfer, disp9),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, disp9),

                Instr(Mnemonic.bnv, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bnl, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bnz, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bh, InstrClass.ConditionalTransfer, disp9),

                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bsa, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, disp9),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, disp9));

            var dispose = Instr(Mnemonic.dispose); //$TODO operands

            var jr_disp22 = Instr(Mnemonic.jr, InstrClass.Transfer, disp22);

            var prepare2 = Nyi("prepare2");
            var prepare3 = Nyi("prepare3");

            rootDecoder = Sparse(5, 6, Nyi(""),
                (0, Select((11, 5), Ne0,
                    Instr(Mnemonic.mov, R, r),
                    Sparse(0, 5, invalid,
                        (0, Instr(Mnemonic.nop, InstrClass.Padding | InstrClass.Linear | InstrClass.Zero)),
                        (0x1D, Instr(Mnemonic.synce)),
                        (0x1E, Instr(Mnemonic.syncm)),
                        (0x1F, Instr(Mnemonic.syncp))))),
                (1, Instr(Mnemonic.not, R, r)),
                (2, Select((11, 5), Ne0,
                    Select((0, 5), Ne0,
                        Instr(Mnemonic.divh, R, r),
                        Instr(Mnemonic.fetrap, r)),
                    Select((0, 5), Ne0,
                        Instr(Mnemonic.@switch, R),
                        Instr(Mnemonic.rie)))),
                (3, Select((11, 5), Ne0,
                    Mask(4, 1, "000011",
                        Instr(Mnemonic.sld_bu, Mep(0, 4, ~0, 0, PrimitiveType.Byte), r),
                        Instr(Mnemonic.sld_hu, Mep(0, 4, ~1, 1, PrimitiveType.Word16), r)),
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Mep(11, 5, ~0, 0, PrimitiveType.Word32)))),

                (4, Select((11, 5), Ne0,
                    Instr(Mnemonic.satsubr, R, r),
                    Instr(Mnemonic.sxb, R))),
                (5, Select((11, 5), Ne0,
                    Instr(Mnemonic.satsub, R, r),
                    Instr(Mnemonic.zxb, R))),
                (6, Select((11, 5), Ne0,
                    Instr(Mnemonic.satadd, R, r),
                    Instr(Mnemonic.sxh, R))),
                (7, Select((11, 5), Ne0,
                    Instr(Mnemonic.mulh, R, r),
                    Instr(Mnemonic.zxh, R))),

                (8, Instr(Mnemonic.or, R, r)),
                (9, Instr(Mnemonic.xor, R, r)),
                (10, Instr(Mnemonic.and, R, r)),
                (11, Instr(Mnemonic.tst, R, r)),

                (12, Instr(Mnemonic.subr, R, r)),
                (13, Instr(Mnemonic.sub, R, r)),
                (14, Instr(Mnemonic.add, R, r)),
                (15, Instr(Mnemonic.cmp, R, r)),

                (16, Select((0, 5), Ne0,
                    Instr(Mnemonic.mov, Is0_5, r),
                    invalid)),
                (17, Select((11, 5), Ne0,
                    Instr(Mnemonic.satadd, Is0_5, r),
                    invalid)),
                (18, Instr(Mnemonic.add, Is0_5, r)),
                (19, Instr(Mnemonic.cmp, Is0_5, r)),

                (20, Instr(Mnemonic.shr, Is0_5, r)),
                (21, Instr(Mnemonic.sar, Is0_5, r)),
                (22, Instr(Mnemonic.shl, Is0_5, r)),
                (23, Select((11, 5), Ne0,
                    Instr(Mnemonic.mulh, Is0_5, r),
                    Select((0, 5), Ne0, 
                        Instr(Mnemonic.jarl, InstrClass.Transfer|InstrClass.Call, disp32,R),
                        Nyi("jr disp32")))),

                (24, Instr(Mnemonic.sld_b, Mep_b, r)),
                (25, Instr(Mnemonic.sld_b, Mep_b, r)),
                (26, Instr(Mnemonic.sld_b, Mep_b, r)),
                (27, Instr(Mnemonic.sld_b, Mep_b, r)),

                (28, Instr(Mnemonic.sst_b, r, Mep_b)),
                (29, Instr(Mnemonic.sst_b, r, Mep_b)),
                (30, Instr(Mnemonic.sst_b, r, Mep_b)),
                (31, Instr(Mnemonic.sst_b, r, Mep_b)),

                (32, Instr(Mnemonic.sld_h, Mep_h, r)),
                (33, Instr(Mnemonic.sld_h, Mep_h, r)),
                (34, Instr(Mnemonic.sld_h, Mep_h, r)),
                (35, Instr(Mnemonic.sld_h, Mep_h, r)),

                (36, Instr(Mnemonic.sst_h, r, Mep_h)),
                (37, Instr(Mnemonic.sst_h, r, Mep_h)),
                (38, Instr(Mnemonic.sst_h, r, Mep_h)),
                (39, Instr(Mnemonic.sst_h, r, Mep_h)),

                (40, sldw_sstw),
                (41, sldw_sstw),
                (42, sldw_sstw),
                (43, sldw_sstw),

                (44, bcond_9),
                (45, bcond_9),
                (46, bcond_9),
                (47, bcond_9),

                (48, Instr(Mnemonic.addi, Simm16, R, r)),
                (49, Select((11, 5), Ne0, "  110001",
                    Instr(Mnemonic.movea, Simm16, R, r),
                    Instr(Mnemonic.mov, Imm32, R))),
                (50, Select((11, 5), Ne0, "  110010",
                    Instr(Mnemonic.movhi, Simm16, R, r),
                    dispose)),
                (51, Select((11, 5), Ne0, "  110011",
                    Instr(Mnemonic.satsubi, Simm16, R, r),
                    dispose)),

                (52, Instr(Mnemonic.ori, Simm16, R, r)),
                (53, Instr(Mnemonic.xori, Simm16, R, r)),
                (54, Instr(Mnemonic.andi, Simm16, R, r)),
                (55, Select((11, 5), Ne0, "  110111",
                    Instr(Mnemonic.mulhi, Simm16, R, r),
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Mw32))),

                (56, Instr(Mnemonic.ld_b, Mb, r)),
                (57, Mdisp16_op(Mask(0, 1, "  111001",
                    Instr(Mnemonic.ld_h, Mh_disp, r),
                    Instr(Mnemonic.ld_w, Mw_disp, r)))),
                (58, Instr(Mnemonic.st_b, r, Mb)),
                (59, Mdisp16_op(Mask(0, 1, "  111011",
                    Instr(Mnemonic.st_h, r, Mh_disp),
                    Instr(Mnemonic.st_w, r, Mw_disp)))),

                (60, Select((11, 5), Ne0, "  111100",
                    Mdisp16_op(Mask(0, 1, "  111100 r!=0",
                        Instr(Mnemonic.jarl, InstrClass.Transfer|InstrClass.Call, disp22, r),
                        Nyi("ld.bu disp16"))),
                    Mdisp16_op(Mask(0, 4, "  111100 r=0",
                        jr_disp22,
                        prepare2,
                        jr_disp22,
                        prepare3,

                        jr_disp22,
                        Instr(Mnemonic.ld_b, Mdisp23(PrimitiveType.SByte), r),
                        jr_disp22,
                        Instr(Mnemonic.ld_h, Mdisp23(PrimitiveType.Int16), r),

                        jr_disp22,
                        Instr(Mnemonic.ld_w, Mdisp23(PrimitiveType.Word32), r),
                        jr_disp22,
                        prepare3,

                        jr_disp22,
                        Instr(Mnemonic.st_b, r, Mdisp23(PrimitiveType.Byte)),
                        jr_disp22,
                        Instr(Mnemonic.st_w, r, Mdisp23(PrimitiveType.Byte)))))),
                (61, Select((11, 5), Ne0, "  111101",
                    Mdisp16_op(Mask(0, 1, "  111101 r!=0",
                        Instr(Mnemonic.jarl, InstrClass.Transfer|InstrClass.Call, disp22, r),
                        Nyi("ld.bu disp16"))), // Who comes up with these encodings...
                    Mdisp16_op(Mask(0, 4, "  111101 r=0",
                        jr_disp22,
                        prepare2,
                        jr_disp22,
                        prepare3,

                        jr_disp22,
                        Instr(Mnemonic.ld_bu, Mdisp23(PrimitiveType.Byte), r),
                        jr_disp22,
                        Instr(Mnemonic.ld_hu, Mdisp23(PrimitiveType.UInt16), r),

                        jr_disp22,
                        Nyi("???"),
                        jr_disp22,
                        prepare3,

                        jr_disp22,
                        Nyi("st.h disp23"),
                        jr_disp22,
                        Nyi("???"))))),
                (62, Mask(14, 2, "  62",
                    Instr(Mnemonic.set1, Iu11_3, Mb),
                    Instr(Mnemonic.not1, Iu11_3, Mb),
                    Instr(Mnemonic.clr1, Iu11_3, Mb),
                    Instr(Mnemonic.tst1, Iu11_3, Mb))),
                (63, Nyi("63"))
                );
        }
    }
}