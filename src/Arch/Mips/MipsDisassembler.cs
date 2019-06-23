#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
    public class MipsDisassembler : DisassemblerBase<MipsInstruction>
    {
        internal readonly MipsProcessorArchitecture arch;
        internal readonly bool isVersion6OrLater;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType signedWord;
        internal readonly List<MachineOperand> ops;
        private MipsInstruction instrCur;
        internal Address addr;

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
            var decoder = decoders[wInstr >> 26];
            try
            {
                if (decoder == null)
                    instrCur = new MipsInstruction { opcode = Opcode.illegal };
                else
                    instrCur = decoder.Decode(wInstr, this);
            }
            catch
            {
                instrCur = null;
            }
            if (instrCur == null)
            {
                instrCur = new MipsInstruction { opcode = Opcode.illegal };
            }
            EmitUnitTest(wInstr, instrCur);
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        [Conditional("DEBUG")]
        public void EmitUnitTest(uint wInstr, MipsInstruction instr)
        {
            if (instr.opcode != Opcode.illegal)
                return;
            var op = (wInstr >> 26);
            if (op == 0 || op == 1)
                return;
            var instrHex = $"{wInstr:X8}";
            base.EmitUnitTest("MIPS", instrHex, "", "MipsDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"0x{0:X8}\"", wInstr);
            });
        }

        internal static readonly Decoder[] decoders;

        static MipsDisassembler()
        {
            var cop1_s = new FpuDecoder(PrimitiveType.Real32,
                new InstrDecoder(Opcode.add_s, F4,F3,F2),
                new InstrDecoder(Opcode.sub_s, F4,F3,F2),
                new InstrDecoder(Opcode.mul_s, F4,F3,F2),
                new InstrDecoder(Opcode.div_s, F4,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.mov_s, F4,F3),
                new InstrDecoder(Opcode.neg_s, F4,F3),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_eq_s, c8,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_lt_s, c8,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_le_s, c8,F3,F2),
                new InstrDecoder(Opcode.illegal));

            var cop1_d = new FpuDecoder(PrimitiveType.Real64,
                // fn 00
                new InstrDecoder(Opcode.add_d, F4,F3,F2),
                new InstrDecoder(Opcode.sub_d, F4,F3,F2),
                new InstrDecoder(Opcode.mul_d, F4,F3,F2),
                new InstrDecoder(Opcode.div_d, F4,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.mov_d, F4,F3),
                new InstrDecoder(Opcode.neg_d, F4,F3),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.trunc_l_d, F4,F3),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 10
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 20
                new InstrDecoder(Opcode.cvt_s_d, F4,F3),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.cvt_w_d, F4,F3),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 30
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_eq_d, c8,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_lt_d, c8,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_le_d, c8,F3,F2),
                new InstrDecoder(Opcode.illegal));

            var cop1_w = new FpuDecoder(PrimitiveType.Int64,
                // fn 00
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 10
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 20
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 30
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.c_lt_d, c8,F3,F2),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal));

            var cop1_l = new FpuDecoder(PrimitiveType.Int64,
                // fn 00
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 10
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 20
                new InstrDecoder(Opcode.cvt_s_l, F4,F3),
                new InstrDecoder(Opcode.cvt_d_l, F4,F3),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                // fn 30
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal));

            var cop0_C0_decoder = new SparseMaskDecoder(0, 0x3F, new Dictionary<uint, Decoder>
            {
                { 0x01, new InstrDecoder(Opcode.tlbr ) },
                { 0x02, new InstrDecoder(Opcode.tlbwi) },
                { 0x06, new InstrDecoder(Opcode.tlbwr) },
                { 0x08, new InstrDecoder(Opcode.tlbp ) },
                { 0x18, new InstrDecoder(Opcode.eret ) },
                { 0x20, new InstrDecoder(Opcode.wait ) },
            });
            var cop1 = new CoprocessorDecoder(
                new InstrDecoder(Opcode.mfc1, R2,F3),
                new A64Decoder(Opcode.dmfc1, R2,F3),
                new InstrDecoder(Opcode.cfc1, R2,f3),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.mtc1, R2,F3),
                new A64Decoder(Opcode.dmtc1, R2,F3),
                new InstrDecoder(Opcode.ctc1, R2,f3),
                new InstrDecoder(Opcode.illegal),

                new BcNDecoder(
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Opcode.bc1f, c18,j),
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Opcode.bc1t, c18,j)),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                cop1_s,
                cop1_d,
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                cop1_w,
                cop1_l,
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal));

            var special2 = new SparseMaskDecoder(0, 0x3F, new Dictionary<uint, Decoder>
            {
                { 0x2, new Version6Decoder(
                    new InstrDecoder(Opcode.mul, R3,R1,R2),
                    new InstrDecoder(Opcode.illegal))
                }
            });
            decoders = new Decoder[]
            {
                new SpecialDecoder(),
                new CondDecoder(),
                new InstrDecoder(Opcode.j, J),
                new InstrDecoder(Opcode.jal, J),
                new InstrDecoder(Opcode.beq, R1,R2,j),
                new InstrDecoder(Opcode.bne, R1,R2,j),
                new InstrDecoder(Opcode.blez, R1,j),
                new InstrDecoder(Opcode.bgtz, R1,j),

                new InstrDecoder(Opcode.addi, R2,R1,I),
                new InstrDecoder(Opcode.addiu, R2,R1,I),
                new InstrDecoder(Opcode.slti, R2,R1,I),
                new InstrDecoder(Opcode.sltiu, R2,R1,I),

                new InstrDecoder(Opcode.andi, R2,R1,U),
                new InstrDecoder(Opcode.ori, R2,R1,U),
                new InstrDecoder(Opcode.xori, R2,R1,U),
                new InstrDecoder(Opcode.lui, R2,i),
                // 10
                new CoprocessorDecoder(
                    new InstrDecoder(Opcode.mfc0, R2,R3),
                    new InstrDecoder(Opcode.dmfc0, R2,R3),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.mtc0, R2,R3),
                    new InstrDecoder(Opcode.dmtc0, R2,R3),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),

                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),

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
               // 11: COP1 encodings
                cop1,

               new CoprocessorDecoder(  // 12: COP2 
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),

                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),

                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),

                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal),
                    new InstrDecoder(Opcode.illegal)),
                null,   // COP1X
                new InstrDecoder(Opcode.beql, R1,R2,j),
                new InstrDecoder(Opcode.bnel, R1,R2,j),
                new InstrDecoder(Opcode.blezl, R1,j),
                new InstrDecoder(Opcode.bgtzl, R1,j),

                new A64Decoder(Opcode.daddi, R2,R1,I),
                new A64Decoder(Opcode.daddiu, R2,R1,I),
                new A64Decoder(Opcode.ldl, R2,El),
                new A64Decoder(Opcode.ldr, R2,El),

                special2,
                null,
                null,
                new Special3Decoder(), 

                // 20
                new InstrDecoder(Opcode.lb, R2,EB),
                new InstrDecoder(Opcode.lh, R2,EH),
                new InstrDecoder(Opcode.lwl, R2,Ew),
                new InstrDecoder(Opcode.lw, R2,Ew),

                new InstrDecoder(Opcode.lbu, R2,Eb),
                new InstrDecoder(Opcode.lhu, R2,Eh),
                new InstrDecoder(Opcode.lwr, R2,Ew),
                new A64Decoder(Opcode.lwu, R2,Ew),

                new InstrDecoder(Opcode.sb, R2,Eb),
                new InstrDecoder(Opcode.sh, R2,Eh),
                new InstrDecoder(Opcode.swl, R2,Ew),
                new InstrDecoder(Opcode.sw, R2,Ew),

                new InstrDecoder(Opcode.sdl, R2,Ew),
                new InstrDecoder(Opcode.sdr, R2,Ew),
                new InstrDecoder(Opcode.swr, R2,Ew),
                null,

                // 30
                new Version6Decoder(
                    new InstrDecoder(Opcode.ll, R2,Ew),
                    new InstrDecoder(Opcode.illegal)),
                new InstrDecoder(Opcode.lwc1, F2,Ew),
                null,
                new InstrDecoder(Opcode.pref, R2,Ew),

                new Version6Decoder(
                    new A64Decoder(Opcode.lld, R2,El),
                    new InstrDecoder(Opcode.illegal)),
                new InstrDecoder(Opcode.ldc1, F2,El),
                null,
                new A64Decoder(Opcode.ld, R2,El),

                new Version6Decoder(
                    new InstrDecoder(Opcode.sc, R2,Ew),
                    new InstrDecoder(Opcode.illegal)),
                new InstrDecoder(Opcode.swc1, F2,Ew),
                null,
                null,

                new Version6Decoder(
                    new A64Decoder(Opcode.scd, R2,El),
                    new InstrDecoder(Opcode.illegal)),
                new A64Decoder(Opcode.sdc1, F2,El),
                null,
                new A64Decoder(Opcode.sd, R2,El)
            };
        }

        //public MipsInstruction DecodeOperands(uint wInstr, Opcode opcode, InstrClass iclass, string opFmt)
        //{
        //    var ops = new List<MachineOperand>();
        //    MachineOperand op = null;
        //    for (int i = 0; i < opFmt.Length; ++i)
        //    {
        //        switch (opFmt[i])
        //        {
        //        default: throw new NotImplementedException(string.Format("Operator format {0}", opFmt[i]));
        //        case ',':
        //            continue;
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
