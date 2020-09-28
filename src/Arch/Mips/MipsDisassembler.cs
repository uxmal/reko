#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<MipsDisassembler, Mnemonic, MipsInstruction>;

    public partial class MipsDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass CTD = InstrClass.Call | InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass DCT = InstrClass.ConditionalTransfer | InstrClass.Delay;

        private static readonly MaskDecoder<MipsDisassembler, Mnemonic, MipsInstruction> rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly bool isVersion6OrLater;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType signedWord;
        private readonly List<MachineOperand> ops;
        private MipsInstruction instrCur;
        private Address addr;

        public MipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader imageReader, bool isVersion6OrLater)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.isVersion6OrLater = isVersion6OrLater;
            this.signedWord = PrimitiveType.Create(Domain.SignedInt, arch.WordWidth.BitSize);
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
            {
                return null;
            }
            this.ops.Clear();
            instrCur = rootDecoder.Decode(wInstr, this);
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MipsInstruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Address = this.addr,
                Length = 4,
                Operands = this.ops.ToArray()
            };
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction 
            {
                Mnemonic = Mnemonic.illegal,
                InstructionClass = InstrClass.Invalid,
                Address = this.addr,
                Length = 4,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var instr = CreateInvalidInstruction();
            EmitUnitTest(wInstr, message);
            return instr;
        }

        [Conditional("DEBUG")]
        public void EmitUnitTest(uint wInstr, string message)
        {
            var op = (wInstr >> 26);
            if (op == 0 || op == 1)
                return;
            var instrHex = $"{wInstr:X8}";
            base.EmitUnitTest("MIPS", instrHex, message, "MipsDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"0x{0:X8}\");", wInstr);
            });
        }


        private static NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction> Nyi(string message)
        {
            return new NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(message);
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<MipsDisassembler> [] mutators)
        {
            return new InstrDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
        {
            return new InstrDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(iclass, mnemonic, mutators);
        }

        static MipsDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal);

            var cop1_s = Mask(0, 6, "FPU (single)",
                Instr(Mnemonic.add_s, F4,F3,F2),
                Instr(Mnemonic.sub_s, F4,F3,F2),
                Instr(Mnemonic.mul_s, F4,F3,F2),
                Instr(Mnemonic.div_s, F4,F3,F2),
                invalid,
                invalid,
                Instr(Mnemonic.mov_s, F4,F3),
                Instr(Mnemonic.neg_s, F4,F3),

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.c_eq_s, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.c_lt_s, c8,F3,F2),
                invalid,
                Instr(Mnemonic.c_le_s, c8,F3,F2),
                invalid);

            var cop1_d = Mask(0, 6, "FPU (double)",
                // fn 00
                Instr(Mnemonic.add_d, F4,F3,F2),
                Instr(Mnemonic.sub_d, F4,F3,F2),
                Instr(Mnemonic.mul_d, F4,F3,F2),
                Instr(Mnemonic.div_d, F4,F3,F2),
                invalid,
                invalid,
                Instr(Mnemonic.mov_d, F4,F3),
                Instr(Mnemonic.neg_d, F4,F3),

                invalid,
                Instr(Mnemonic.trunc_l_d, F4,F3),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                Instr(Mnemonic.cvt_s_d, F4,F3),
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.cvt_w_d, F4,F3),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                Instr(Mnemonic.c_eq_d, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.c_lt_d, c8,F3,F2),
                invalid,
                Instr(Mnemonic.c_le_d, c8,F3,F2),
                invalid);

            var cop1_w = Mask(0, 6, "FPU (word)",
                // fn 00
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.c_lt_d, c8,F3,F2),
                invalid,
                invalid,
                invalid);

            var cop1_l = Mask(0, 6, "FPU (dword)",
                // fn 00
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                Instr(Mnemonic.cvt_s_l, F4,F3),
                Instr(Mnemonic.cvt_d_l, F4,F3),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid);

            var cop0_C0_decoder = Sparse(0, 6, "COP0 C0", 
                invalid,
                ( 0x01, Instr(Mnemonic.tlbr ) ),
                ( 0x02, Instr(Mnemonic.tlbwi) ),
                ( 0x06, Instr(Mnemonic.tlbwr) ),
                ( 0x08, Instr(Mnemonic.tlbp ) ),
                ( 0x18, Instr(Mnemonic.eret ) ),
                ( 0x20, Instr(Mnemonic.wait ) ));
            var cop1 = Mask(21, 5, "COP1",
                Instr(Mnemonic.mfc1, R2,F3),
                new A64Decoder(Mnemonic.dmfc1, R2,F3),
                Instr(Mnemonic.cfc1, R2,f3),
                invalid,
                Instr(Mnemonic.mtc1, R2,F3),
                new A64Decoder(Mnemonic.dmtc1, R2,F3),
                Instr(Mnemonic.ctc1, R2,f3),
                invalid,

                Mask(16, 1,
                    Instr(InstrClass.ConditionalTransfer | InstrClass.Delay, Mnemonic.bc1f, c18,j),
                    Instr(InstrClass.ConditionalTransfer | InstrClass.Delay, Mnemonic.bc1t, c18,j)),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                cop1_s,
                cop1_d,
                invalid,
                invalid,
                cop1_w,
                cop1_l,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid);

            var cop2 = Mask(21, 5, "COP2",   // 12: COP2 
                 Nyi("mfc2"),
                 invalid,
                 Nyi("cfc2"),
                 Nyi("mfhc2"),
                 Nyi("mtc2"),
                 invalid,
                 Nyi("ctc2"),
                 Nyi("mthc2"),

                 Nyi("bc2"),
                 Nyi("bc2eqz"),
                 Instr(Mnemonic.lwc2,R2,E11w),
                 Nyi("swc2"),
                 invalid,
                 Nyi("bc2nez"),
                 Nyi("ldc2"),
                 Nyi("sdc2"),

                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,

                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid);

            var special2 = Sparse(0, 6, "Special2",
                invalid,
                (0x0, Instr(Mnemonic.madd, R1, R2)),
                (0x1, Instr(Mnemonic.maddu, R1, R2)),
                (0x2, Instr(Mnemonic.mul, R3, R1, R2)),
                (0x4, Instr(Mnemonic.msub, R1, R2)),
                (0x5, Instr(Mnemonic.msubu, R1, R2)),

                (0x20, Instr(Mnemonic.clz, R3, R1)),
                (0x21, Instr(Mnemonic.clo, R3, R1)),
                (0x3F, Instr(Mnemonic.sdbbp, Imm(PrimitiveType.UInt32, 6, 20))));

            var condDecoders = Mask(16, 5, "CondDecoders",
                Instr(DCT, Mnemonic.bltz, R1, j),
                Instr(DCT, Mnemonic.bgez, R1, j),
                Instr(DCT, Mnemonic.bltzl, R1, j),
                Instr(DCT, Mnemonic.bgezl, R1, j),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(CTD, Mnemonic.tgei, R1, I),
                Instr(CTD, Mnemonic.tgeiu, R1, I),
                Instr(CTD, Mnemonic.tlti, R1, I),
                Instr(CTD, Mnemonic.tltiu, R1, I),

                Instr(CTD, Mnemonic.teqi, R1, I),
                invalid,
                Instr(CTD, Mnemonic.tnei, R1, I),
                invalid,

                Instr(CTD, Mnemonic.bltzal, R1, j),
                Instr(CTD, Mnemonic.bgezal, R1, j),
                Instr(CTD, Mnemonic.bltzall, R1, j),
                Instr(CTD, Mnemonic.bgezall, R1, j),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);


           var bshfl = Mask(6, 5,
                invalid,
                invalid,
                Instr(Mnemonic.wsbh, x("")),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // 10
                Instr(Mnemonic.seb, R3, R2),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.seh, R3, R2),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

        var special3 = Mask(0, 6, "Special3",
            // 00
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 10
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 20
            bshfl,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 30
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            new Version6Decoder(
                invalid,
                Instr(Mnemonic.ll, R2,ew)),
            new Version6Decoder(
                invalid,
                new A64Decoder(Mnemonic.lld, R2,el)),

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.rdhwr, R2,H),
            invalid,
            invalid,
            invalid,
            invalid);

            var special = Mask(0, 6, "Special",
                Select((6, 5), n => n == 0,
                    Instr(InstrClass.Linear|InstrClass.Padding, Mnemonic.nop),
                    Instr(Mnemonic.sll, R3, R2, s)),
                Mask(16, 1,
                    Instr(Mnemonic.movf, R2, R1, C18),
                    Instr(Mnemonic.movt, R2, R1, C18)),
                Instr(Mnemonic.srl, R3, R2, s),
                Instr(Mnemonic.sra, R3, R2, s),

                Instr(Mnemonic.sllv, R3, R2, R1),
                Instr(Mnemonic.illegal),
                Instr(Mnemonic.srlv, R3, R2, R1),
                Instr(Mnemonic.srav, R3, R2, R1),

                Instr(TD, Mnemonic.jr, R1),
                Instr(CTD, Mnemonic.jalr, R3, R1),
                Instr(Mnemonic.movz, R3, R1, R2),
                Instr(Mnemonic.movn, R3, R1, R2),
                Instr(Mnemonic.syscall, B),
                Instr(Mnemonic.@break, B),
                Instr(Mnemonic.illegal),
                Instr(Mnemonic.sync, s),
                // 10
                Instr(Mnemonic.mfhi, R3),
                Instr(Mnemonic.mthi, R1),
                Instr(Mnemonic.mflo, R3),
                Instr(Mnemonic.mtlo, R1),
                new A64Decoder(Mnemonic.dsllv, R3, R2, R1),
                Instr(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrlv, R3, R2, R1),
                new A64Decoder(Mnemonic.dsrav, R3, R2, R1),

                Instr(Mnemonic.mult, R1, R2),
                Instr(Mnemonic.multu, R1, R2),
                Instr(Mnemonic.div, R1, R2),
                Instr(Mnemonic.divu, R1, R2),
                new A64Decoder(Mnemonic.dmult, R1, R2),
                new A64Decoder(Mnemonic.dmultu, R1, R2),
                new A64Decoder(Mnemonic.ddiv, R1, R2),
                new A64Decoder(Mnemonic.ddivu, R1, R2),
                // 20
                Instr(Mnemonic.add, R3, R1, R2),
                Instr(Mnemonic.addu, R3, R1, R2),
                Instr(Mnemonic.sub, R3, R1, R2),
                Instr(Mnemonic.subu, R3, R1, R2),
                Instr(Mnemonic.and, R3, R1, R2),
                Instr(Mnemonic.or, R3, R1, R2),
                Instr(Mnemonic.xor, R3, R1, R2),
                Instr(Mnemonic.nor, R3, R1, R2),

                Instr(Mnemonic.illegal),
                Instr(Mnemonic.illegal),
                Instr(Mnemonic.slt, R3, R1, R2),
                Instr(Mnemonic.sltu, R3, R1, R2),
                new A64Decoder(Mnemonic.dadd, R3, R1, R2),
                new A64Decoder(Mnemonic.daddu, R3, R1, R2),
                new A64Decoder(Mnemonic.dsub, R3, R1, R2),
                new A64Decoder(Mnemonic.dsubu, R3, R1, R2),
                // 30
                Instr(CTD, Mnemonic.tge, R1, R2, T),
                Instr(CTD, Mnemonic.tgeu, R1, R2, T),
                Instr(CTD, Mnemonic.tlt, R1, R2, T),
                Instr(CTD, Mnemonic.tltu, R1, R2, T),
                Instr(CTD, Mnemonic.teq, R1, R2, T),
                Instr(Mnemonic.illegal),
                Instr(CTD, Mnemonic.tne, R1, R2, T),
                Instr(Mnemonic.illegal),

                new A64Decoder(Mnemonic.dsll, R3, R2, s),
                Instr(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrl, R3, R2, s),
                new A64Decoder(Mnemonic.dsra, R3, R2, s),
                new A64Decoder(Mnemonic.dsll32, R3, R2, s),
                Instr(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrl32, R3, R2, s),
                new A64Decoder(Mnemonic.dsra32, R3, R2, s));

            var cop1x = Mask(0, 6,
                Instr(Mnemonic.lwxc1, F3,Mxw),
                Instr(Mnemonic.ldxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Mnemonic.luxc1, F4,Mxw),
                invalid,
                invalid,

                Instr(Mnemonic.swxc1, F3,Mxw),
                Instr(Mnemonic.sdxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Mnemonic.suxc1, F4,Mxw),
                invalid,
                Instr(Mnemonic.prefx, Imm(PrimitiveType.Byte, 11,5), Mxw),
                // 10
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.alnv_ps, F4,F3,F2,R1),
                invalid,
                // 20
                Instr(Mnemonic.madd_s, F4,F1,F3,F2),
                Instr(Mnemonic.madd_d, F4,F1,F3,F2),
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.madd_ps, F4, F1, F3, F2),
                invalid,

                Instr(Mnemonic.msub_s, F4, F1, F3, F2),
                Instr(Mnemonic.msub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.msub_ps, F4, F1, F3, F2),
                invalid,
                // 30
                Instr(Mnemonic.nmadd_s, F4, F1, F3, F2),
                Instr(Mnemonic.nmadd_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.nmadd_ps, F4, F1, F3, F2),
                invalid,

                Instr(Mnemonic.nmsub_s, F4, F1, F3, F2),
                Instr(Mnemonic.nmsub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.nmsub_ps, F4, F1, F3, F2),
                invalid);


        rootDecoder = Mask(26, 6,
                special,
                condDecoders,
                Instr(TD, Mnemonic.j, J),
                Instr(CTD, Mnemonic.jal, J),
                Instr(DCT, Mnemonic.beq, R1,R2,j),
                Instr(DCT, Mnemonic.bne, R1,R2,j),
                Instr(DCT, Mnemonic.blez, R1,j),
                Instr(DCT, Mnemonic.bgtz, R1,j),

                Instr(Mnemonic.addi, R2,R1,I),
                Instr(Mnemonic.addiu, R2,R1,I),
                Instr(Mnemonic.slti, R2,R1,I),
                Instr(Mnemonic.sltiu, R2,R1,I),

                Instr(Mnemonic.andi, R2,R1,U),
                Instr(Mnemonic.ori, R2,R1,U),
                Instr(Mnemonic.xori, R2,R1,U),
                Instr(Mnemonic.lui, R2,i),
                // 10
                Mask(21, 5, "Coprocessor",
                    Instr(Mnemonic.mfc0, R2,R3),
                    Instr(Mnemonic.dmfc0, R2,R3),
                    invalid,
                    invalid,
                    Instr(Mnemonic.mtc0, R2,R3),
                    Instr(Mnemonic.dmtc0, R2,R3),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder),
               cop1,
               new Version6Decoder(
                   invalid,
                   cop2),
               new Version6Decoder(
                    cop1x,
                    invalid),       // removed in MIPS v6
                
                Instr(DCT, Mnemonic.beql, R1,R2,j),
                Instr(DCT, Mnemonic.bnel, R1,R2,j),
                Instr(DCT, Mnemonic.blezl, R1,j),
                Instr(DCT, Mnemonic.bgtzl, R1,j),

                new A64Decoder(Mnemonic.daddi, R2,R1,I),
                new A64Decoder(Mnemonic.daddiu, R2,R1,I),
                new A64Decoder(Mnemonic.ldl, R2,El),
                new A64Decoder(Mnemonic.ldr, R2,El),

                new Version6Decoder(
                    special2,
                    invalid),
                new Version6Decoder(
                    invalid,
                    Nyi("POP6")),
                new Version6Decoder(
                    invalid, 
                    Nyi("POP7")),
                special3,

                // 20
                Instr(Mnemonic.lb, R2,EB),
                Instr(Mnemonic.lh, R2,EH),
                Instr(Mnemonic.lwl, R2,Ew),
                Instr(Mnemonic.lw, R2,Ew),

                Instr(Mnemonic.lbu, R2,Eb),
                Instr(Mnemonic.lhu, R2,Eh),
                Instr(Mnemonic.lwr, R2,Ew),
                new A64Decoder(Mnemonic.lwu, R2,Ew),

                Instr(Mnemonic.sb, R2,Eb),
                Instr(Mnemonic.sh, R2,Eh),
                Instr(Mnemonic.swl, R2,Ew),
                Instr(Mnemonic.sw, R2,Ew),

                Instr(Mnemonic.sdl, R2,Ew),
                Instr(Mnemonic.sdr, R2,Ew),
                Instr(Mnemonic.swr, R2,Ew),
                new Version6Decoder(
                    Instr(Mnemonic.cache, Imm(PrimitiveType.Byte, 16, 5), Ew),
                    invalid),

                // 30
                new Version6Decoder(
                    Instr(Mnemonic.ll, R2,Ew),
                    invalid),
                Instr(Mnemonic.lwc1, F2,Ew),
                new Version6Decoder(
                    Instr(Mnemonic.lwc2, R2, El),
                    Nyi("BC-v6")),
                Instr(Mnemonic.pref, R2,Ew),

                new Version6Decoder(
                    new A64Decoder(Mnemonic.lld, R2,El),
                    invalid),
                Instr(Mnemonic.ldc1, F2,El),
                new Version6Decoder(
                    Instr(Mnemonic.ldc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Mnemonic.ld, R2,El),

                new Version6Decoder(
                    Instr(Mnemonic.sc, R2,Ew),
                    invalid),
                Instr(Mnemonic.swc1, F2,Ew),
                new Version6Decoder(
                    Instr(Mnemonic.swc2, R2, Ew),
                    Nyi("BALC-v6")),
                new Version6Decoder(
                    invalid,
                    Nyi("PCREL-v6")),

                new Version6Decoder(
                    new A64Decoder(Mnemonic.scd, R2,El),
                    invalid),
                new A64Decoder(Mnemonic.sdc1, F2,El),
                new Version6Decoder(
                    Instr(Mnemonic.sdc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Mnemonic.sd, R2,El));
        }

        internal static Mutator<MipsDisassembler> R(int offset)
        {
            return (u, d) =>
            {
                var op = d.Reg(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> R1 = R(21);
        internal static readonly Mutator<MipsDisassembler> R2 = R(16);
        internal static readonly Mutator<MipsDisassembler> R3 = R(11);
        internal static readonly Mutator<MipsDisassembler> R4 = R(6);

        internal static Mutator<MipsDisassembler> F(int offset)
        {
            return (u, d) =>
            {
                var op = d.FReg(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> F1 = F(21);
        internal static readonly Mutator<MipsDisassembler> F2 = F(16);
        internal static readonly Mutator<MipsDisassembler> F3 = F(11);
        internal static readonly Mutator<MipsDisassembler> F4 = F(6);
        
        // FPU control register
        internal static Mutator<MipsDisassembler> Fcreg(int offset)
        {
            return (u, d) =>
            {
                if (!d.TryGetFCReg(u >> offset, out RegisterOperand fcreg))
                    return false;
                d.ops.Add(fcreg);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> f1 = Fcreg(21);
        internal static readonly Mutator<MipsDisassembler> f2 = Fcreg(16);
        internal static readonly Mutator<MipsDisassembler> f3 = Fcreg(11);

        internal static bool I(uint wInstr, MipsDisassembler dasm)
        {
            var op = new ImmediateOperand(Constant.Create(dasm.signedWord, (short) wInstr));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool U(uint wInstr, MipsDisassembler dasm)
        {
            var op = new ImmediateOperand(Constant.Create(dasm.arch.WordWidth, (ushort) wInstr));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool i(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Int16((short) wInstr);
            dasm.ops.Add(op);
            return true;
        }

        private static Mutator<MipsDisassembler> Imm(PrimitiveType dt, int bitPos, int bitlen)
        {
            var field = new Bitfield(bitPos, bitlen);
            return (u, d) =>
            {
                var n = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, n)));
                return true;
            };
        }

        internal static bool j(uint wInstr, MipsDisassembler dasm)
        {
            var op = dasm.RelativeBranch(wInstr);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool J(uint wInstr, MipsDisassembler dasm)
        {
            var op = dasm.LargeBranch(wInstr);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool B(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Word32((wInstr >> 6) & 0xFFFFF);
            dasm.ops.Add(op);
            return true;
        }

        // Shift amount or sync type
        internal static bool s(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) ((wInstr >> 6) & 0x1F));
            dasm.ops.Add(op);
            return true;
        }

        // effective address w 16-bit offset
        internal static Mutator<MipsDisassembler> E(PrimitiveType size)
        {
            return (u, d) =>
            {
                var op = d.Ea(u, size, 21, (short) u);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> Eb = E(PrimitiveType.Byte);
        internal static readonly Mutator<MipsDisassembler> EB = E(PrimitiveType.SByte);
        internal static readonly Mutator<MipsDisassembler> Eh = E(PrimitiveType.Word16);
        internal static readonly Mutator<MipsDisassembler> EH = E(PrimitiveType.Int16); 
        internal static readonly Mutator<MipsDisassembler> Ew = E(PrimitiveType.Word32);
        internal static readonly Mutator<MipsDisassembler> EW = E(PrimitiveType.Int32); 
        internal static readonly Mutator<MipsDisassembler> El = E(PrimitiveType.Word64);
        internal static readonly Mutator<MipsDisassembler> EL = E(PrimitiveType.Int64);

        // effective address w 9-bit offset
        internal static Mutator<MipsDisassembler> e(PrimitiveType size)
        {
            return (u, d) =>
            {
                var op = d.Ea(u, size, 21, (short) (((short) u) >> 7));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> ew = e(PrimitiveType.Word32);
        internal static readonly Mutator<MipsDisassembler> el = e(PrimitiveType.Word64);

        // effective address w 11-bit offset
        internal static Mutator<MipsDisassembler> E11(PrimitiveType size)
        {
            var offsetField = new Bitfield(0, 11);
            return (u, d) =>
            {
                var offset = (short)offsetField.ReadSigned(u);
                var op = d.Ea(u, size, 11, offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> E11w = E11(PrimitiveType.Word32);


        // Indexed memory address
        private static Mutator<MipsDisassembler> Mx(PrimitiveType dt, int posBase, int posIdx)
        {
            var baseField = new Bitfield(posBase, 5);
            var idxField = new Bitfield(posIdx, 5);
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var iIndex = (int) idxField.Read(u);
                var rBase = d.arch.GetRegister(iBase);
                var rIndex = d.arch.GetRegister(iIndex);
                d.ops.Add(new IndexedOperand(dt, rBase, rIndex));
                return true;
            };
        }
        private static readonly Mutator<MipsDisassembler> Mxbu = Mx(PrimitiveType.Byte, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxh = Mx(PrimitiveType.Word16, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxw = Mx(PrimitiveType.Word32, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxd = Mx(PrimitiveType.Word64, 21, 16);

        // trap code
        internal static bool T(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Word16((ushort) ((wInstr >> 6) & 0x03FF));
            dasm.ops.Add(op);
            return true;
        }

        // condition code
        internal static Mutator<MipsDisassembler> c(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.CCodeFlag(u, bitPos);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> c8 = c(8);
        internal static readonly Mutator<MipsDisassembler> c18 = c(18);

        // FPU condition code
        internal static Mutator<MipsDisassembler> C(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.FpuCCodeFlag(u, bitPos);
                d.ops.Add(op);
                return true;
            };
        }
        internal static Mutator<MipsDisassembler> C18 = C(18);

        // hardware register, see instruction rdhwr
        internal static bool H(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) ((wInstr >> 11) & 0x1f));
            dasm.ops.Add(op);
            return true;
        }

        internal static Mutator<MipsDisassembler> x(string message)
        {
            return (u, d) =>
            {
                d.NotYetImplemented(u, message);
                return false;
            };
        }

        private RegisterOperand Reg(uint regNumber)
        {
            return new RegisterOperand(arch.GetRegister((int) regNumber & 0x1F));
        }

        private RegisterOperand FReg(uint regNumber)
        {
            return new RegisterOperand(arch.fpuRegs[regNumber & 0x1F]);
        }

        private bool TryGetFCReg(uint regNumber, out RegisterOperand op)
        {
            if (arch.fpuCtrlRegs.TryGetValue(regNumber & 0x1F, out RegisterStorage fcreg))
            {
                op = new RegisterOperand(fcreg);
                return true;
            }
            else
            {
                op = null;
                return false;
            }
        }

        private RegisterOperand CCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.ccRegs[regNo]);
        }

        private RegisterOperand FpuCCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.fpuCcRegs[regNo]);
        }

        private AddressOperand RelativeBranch(uint wInstr)
        {
            int off = (short) wInstr;
            off <<= 2;
            return AddressOperand.Create(rdr.Address + off);
        }

        private AddressOperand LargeBranch(uint wInstr)
        {
            var off = (wInstr & 0x03FFFFFF) << 2;
            ulong linAddr = (rdr.Address.ToLinear() & ~0x0FFFFFFFul) | off;
            return AddressOperand.Create(
                Address.Create(arch.PointerType, linAddr));
        }

        private IndirectOperand Ea(uint wInstr, PrimitiveType dataWidth, int shift, short offset)
        {
            var baseReg = arch.GetRegister((int)(wInstr >> shift) & 0x1F);
            return new IndirectOperand(dataWidth, offset, baseReg);
        }
    }
}
