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
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Loongson
{
    using Decoder = Decoder<LoongArchDisassembler, Mnemonic, LoongArchInstruction>;

    public class LoongArchDisassembler : DisassemblerBase<LoongArchInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly LoongArch arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public LoongArchDisassembler(LoongArch arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override LoongArchInstruction? DisassembleInstruction()
        {
            var offset = rdr.Offset;
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            ops.Clear();
            instr.Address = addr;
            instr.InstructionClass |= (uInstr == 0 ? InstrClass.Zero : 0);
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }

        public override LoongArchInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new LoongArchInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
            return instr;
        }

        public override LoongArchInstruction CreateInvalidInstruction()
        {
            return MakeInstruction(InstrClass.Invalid, Mnemonic.Invalid);
        }


        public override LoongArchInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("LoongarchDis", this.addr, this.rdr, message);
            return new LoongArchInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.nyi };
        }

        #region Mutators

        private static Mutator<LoongArchDisassembler> RegisterField(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = bitfield.Read(u);
                d.ops.Add(d.arch.GpRegisters[ireg]);
                return true;
            };
        }

        private static readonly Mutator<LoongArchDisassembler> r0 = RegisterField(0);
        private static readonly Mutator<LoongArchDisassembler> r5 = RegisterField(5);
        private static readonly Mutator<LoongArchDisassembler> r10 = RegisterField(10);



        private static Mutator<LoongArchDisassembler> FpuRegisterField(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = bitfield.Read(u);
                d.ops.Add(d.arch.FpRegisters[ireg]);
                return true;
            };
        }

        private static readonly Mutator<LoongArchDisassembler> f0 = FpuRegisterField(0);
        private static readonly Mutator<LoongArchDisassembler> f5 = FpuRegisterField(5);
        private static readonly Mutator<LoongArchDisassembler> f10 = FpuRegisterField(10);



        private static Mutator<LoongArchDisassembler> ConditionFlagRegisterField(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var ireg = bitfield.Read(u);
                d.ops.Add(d.arch.CfRegisters[ireg]);
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> c0 = FpuRegisterField(0);
        private static readonly Mutator<LoongArchDisassembler> c5 = FpuRegisterField(5);

        private static Mutator<LoongArchDisassembler> simm(int bitpos, int bitLength)
        {
            var field = new Bitfield(bitpos, bitLength);
            return (u, d) =>
            {
                var imm = ImmediateOperand.Int32(field.ReadSigned(u));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> si12 = simm(10, 12);
        private static readonly Mutator<LoongArchDisassembler> si14 = simm(10, 14);
        private static readonly Mutator<LoongArchDisassembler> si20 = simm(5, 20);

        private static Mutator<LoongArchDisassembler> uimm(int bitpos, int bitLength)
        {
            var field = new Bitfield(bitpos, bitLength);
            return (u, d) =>
            {
                var imm = ImmediateOperand.UInt32(field.Read(u));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> ui0 = uimm(0, 5);
        private static readonly Mutator<LoongArchDisassembler> ui5 = uimm(10, 5);
        private static readonly Mutator<LoongArchDisassembler> ui6 = uimm(10, 6);
        private static readonly Mutator<LoongArchDisassembler> ui12 = uimm(10, 12);
        private static readonly Mutator<LoongArchDisassembler> ui15 = uimm(0, 15);
        private static readonly Mutator<LoongArchDisassembler> msbd = uimm(16, 6);
        private static readonly Mutator<LoongArchDisassembler> lsbd = uimm(10, 6);
        private static readonly Mutator<LoongArchDisassembler> msbw = uimm(16, 5);
        private static readonly Mutator<LoongArchDisassembler> lsbw = uimm(10, 5);
        private static readonly Mutator<LoongArchDisassembler> csr = uimm(10, 14);
        private static readonly Mutator<LoongArchDisassembler> seq = uimm(10, 8);
        private static readonly Mutator<LoongArchDisassembler> level = uimm(10, 8);

        private static bool sa2(uint uInstr, LoongArchDisassembler dasm)
        {
            var imm = ImmediateOperand.Int32((int) ((uInstr >> 15) & 3));
            dasm.ops.Add(imm);
            return true;
        }

        private static bool sa3(uint uInstr, LoongArchDisassembler dasm)
        {
            var imm = ImmediateOperand.Int32((int) ((uInstr >> 15) & 7));
            dasm.ops.Add(imm);
            return true;
        }

        private static Mutator<LoongArchDisassembler> j(int bitOffset, int bitLength)
        {
            var bitfield = new Bitfield(bitOffset, bitLength);
            return (u, d) =>
            {
                var displacement = bitfield.ReadSigned(u);
                var addr = d.addr + displacement * 4;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> j16 = j(10, 16);


        private static Mutator<LoongArchDisassembler> jr(int bitOffset, int bitLength)
        {
            var bitfield = new Bitfield(bitOffset, bitLength);
            return (u, d) =>
            {
                long displacement = bitfield.ReadSigned(u) * 4;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.SignedWord, displacement)));
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> jr16 = jr(10, 16);


        private static Mutator<LoongArchDisassembler> Jump(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var displacement = Bitfield.ReadSignedFields(bitfields, u);
                var addr = d.addr + displacement * 4;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<LoongArchDisassembler> J = Jump(Bf((0, 10), (10, 16)));
        private static readonly Mutator<LoongArchDisassembler> j21 = Jump(Bf((0, 5), (10, 16)));

        #endregion

        private static bool Eq0(uint u) => u == 0;

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<LoongArchDisassembler>[] mutators)
        {
            return new InstrDecoder<LoongArchDisassembler, Mnemonic, LoongArchInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<LoongArchDisassembler>[] mutators)
        {
            return new InstrDecoder<LoongArchDisassembler, Mnemonic, LoongArchInstruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<LoongArchDisassembler, Mnemonic, LoongArchInstruction>(message);
        }

        static LoongArchDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi = new NyiDecoder<LoongArchDisassembler, Mnemonic, LoongArchInstruction>("nyi");
            /*
            ##CLO.W rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 0 1 0 0 rj rd
            ##CLZ.W rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 0 1 0 1 rj rd
            ##CTO.W rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 0 1 1 0 rj rd
            ##CTZ.W rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 0 1 1 1 rj rd
            ##CLO.D rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 0 0 0 rj rd
            ##CLZ.D rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 0 0 1 rj rd
            ##CTO.D rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 0 1 0 rj rd
            ##CTZ.D rd, rj#                  <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 0 1 1 rj rd
            ##REVB.2H rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 1 0 0 rj rd
            ##REVB.4H rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 1 0 1 rj rd
            ##REVB.2W rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 1 1 0 rj rd
            ##REVB.D rd, rj#                 <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   0 1 1 1 1 rj rd
            ##REVH.2W rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 0 0 0 rj rd
            ##REVH.D rd, rj#                 <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 0 0 1 rj rd
            ##BITREV.4B rd, rj#              <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 0 1 0 rj rd
            ##BITREV.8B rd, rj#              <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 0 1 1 rj rd
            ##BITREV.W rd, rj#               <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 1 0 0 rj rd
            ##BITREV.D rd, rj#               <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 1 0 1 rj rd
            ##EXT.W.H rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 1 1 0 rj rd
            ##EXT.W.B rd, rj#                <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 0 1 1 1 rj rd
            ##RDTIMEL.W rd, rj#              <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 1 0 0 0 rj rd
            ##RDTIMEH.W rd, rj#              <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 1 0 0 1 rj rd
            ##RDTIME.D rd, rj#               <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 1 0 1 0 rj rd
            ##CPUCFG rd, rj#                 <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 0 0   1 1 0 1 1 rj rd
            ##ASRTLE.D rj, rk#               <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 1 0   rk rj 0 0 0 0 0
            ##ASRTGT.D rj, rk#               <code>0 0 0 0 0 0   0 0 0 0   0 0 0 0   0 1 1   rk rj 0 0 0 0 0
            */
            var opcode_00_00_00 = Mask(15, 3, "  00",
                Mask(10, 5, "  00",
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.clo_w, r0, r5),
                    Instr(Mnemonic.clz_w, r0, r5),
                    Instr(Mnemonic.cto_w, r0, r5),
                    Instr(Mnemonic.ctz_w, r0, r5),

                    Instr(Mnemonic.clo_d, r0, r5),
                    Instr(Mnemonic.clz_d, r0, r5),
                    Instr(Mnemonic.cto_d, r0, r5),
                    Instr(Mnemonic.ctz_d, r0, r5),

                    Instr(Mnemonic.revb_2h, r0, r5),
                    Instr(Mnemonic.revb_4h, r0, r5),
                    Instr(Mnemonic.revb_2w, r0, r5),
                    Instr(Mnemonic.revb_d, r0, r5), 

                    Instr(Mnemonic.revh_2w, r0, r5),   
                    Instr(Mnemonic.revh_d, r0, r5),    
                    Instr(Mnemonic.bitrev_4b, r0, r5), 
                    Instr(Mnemonic.bitrev_8b, r0, r5), 

                    Instr(Mnemonic.bitrev_w, r0, r5),  
                    Instr(Mnemonic.bitrev_d, r0, r5),  
                    Instr(Mnemonic.ext_w_h, r0, r5),   
                    Instr(Mnemonic.ext_w_b, r0, r5),

                    Instr(Mnemonic.rdtimel_w, r0, r5), 
                    Instr(Mnemonic.rdtimeh_w, r0, r5), 
                    Instr(Mnemonic.rdtime_d, r0, r5),  
                    Instr(Mnemonic.cpucfg, r0, r5),    

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,
                If(0, 5, Eq0, Instr(Mnemonic.asrtle_d, r5, r10)),
                If(0, 5, Eq0, Instr(Mnemonic.asrtgt_d, r5, r10)),

                invalid,
                invalid,
                invalid,
                invalid);

            /*
            ##ALSL.W rd, rj, rk, sa2#        <code>0 0 0 0 0 0   0 0 0 0   0 0 0 1   0 sa2 rk rj rd
            ##ALSL.WU rd, rj, rk, sa2#       <code>0 0 0 0 0 0   0 0 0 0   0 0 0 1   1 sa2 rk rj rd
            */
            var opcode_00_00_01 = Mask(17, 1, "  00:00:01",
                Instr(Mnemonic.alsl_w, r0, r5, r10, sa2),
                Instr(Mnemonic.alsl_wu, r0, r5, r10, sa2));
            /*

            ##ADD.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   0 0 0 rk rj rd
            ##ADD.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   0 0 1 rk rj rd
            ##SUB.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   0 1 0 rk rj rd
            ##SUB.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   0 1 1 rk rj rd
            ##SLT rd, rj, rk#                <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   1 0 0 rk rj rd
            ##SLTU rd, rj, rk#               <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   1 0 1 rk rj rd
            ##MASKEQZ rd, rj, rk#            <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   1 1 0 rk rj rd
            ##MASKNEZ rd, rj, rk#            <code>0 0 0 0 0 0   0 0 0 0   0 1 0 0   1 1 1 rk rj rd
            */
            var opcode_00_00_04 = Mask(15, 3, "  opcode 00:00:4",
                Instr(Mnemonic.add_w, r0, r5, r10),
                Instr(Mnemonic.add_d, r0, r5, r10),
                Instr(Mnemonic.sub_w, r0, r5, r10),
                Instr(Mnemonic.sub_d, r0, r5, r10),
                Instr(Mnemonic.slt, r0, r5, r10),
                Instr(Mnemonic.sltu, r0, r5, r10),
                Instr(Mnemonic.maskeqz, r0, r5, r10),
                Instr(Mnemonic.masknez, r0, r5, r10));

            /*

            ##NOR rd, rj, rk#                <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   0 0 0 rk rj rd
            ##AND rd, rj, rk#                <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   0 0 1 rk rj rd
            ##OR rd, rj, rk#                 <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   0 1 0 rk rj rd
            ##XOR rd, rj, rk#                <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   0 1 1 rk rj rd
            ##ORN rd, rj, rk#                <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   1 0 0 rk rj rd
            ##ANDN rd, rj, rk#               <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   1 0 1 rk rj rd
            ##SLL.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   1 1 0 rk rj rd
            ##SRL.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 0 1   1 1 1 rk rj rd
            */
            var opcode_00_00_05 = Mask(15, 3, "  opcode 00:00:5",
                Instr(Mnemonic.nor, r0, r5, r10),
                Instr(Mnemonic.and, r0, r5, r10),
                Instr(Mnemonic.or, r0, r5, r10),
                Instr(Mnemonic.xor, r0, r5, r10),
                Instr(Mnemonic.orn, r0, r5, r10),
                Instr(Mnemonic.andn, r0, r5, r10),
                Instr(Mnemonic.sll_w, r0, r5, r10),
                Instr(Mnemonic.srl_w, r0, r5, r10));

            /*
            ##SRA.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   0 0 0 rk rj rd
            ##SLL.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   0 0 1 rk rj rd
            ##SRL.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   0 1 0 rk rj rd
            ##SRA.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   0 1 1 rk rj rd
            ##ROTR.W rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   1 1 0 rk rj rd
            ##ROTR.D rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   0 1 1 0   1 1 1 rk rj rd
            */
            var opcode_00_00_06 = Mask(15, 3, "  6",
                Instr(Mnemonic.sra_w, r0, r5, r10),
                Instr(Mnemonic.sll_d, r0, r5, r10),
                Instr(Mnemonic.srl_d, r0, r5, r10),
                Instr(Mnemonic.sra_d, r0, r5, r10),

                invalid,
                invalid,
                Instr(Mnemonic.rotr_w, r0, r5, r10),
                Instr(Mnemonic.rotr_d, r0, r5, r10));

            /*
            ##MUL.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   0 0 0 rk rj rd
            ##MULH.W rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   0 0 1 rk rj rd
            ##MULH.WU rd, rj, rk#            <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   0 1 0 rk rj rd
            ##MUL.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   0 1 1 rk rj rd
            ##MULH.D rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   1 0 0 rk rj rd
            ##MULH.DU rd, rj, rk#            <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   1 0 1 rk rj rd
            ##MULW.D.W rd, rj, rk#           <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   1 1 0 rk rj rd
            ##MULW.D.WU rd, rj, rk#          <code>0 0 0 0 0 0   0 0 0 0   0 1 1 1   1 1 1 rk rj rd
            */
            var opcode_00_00_07 = Mask(15, 3, "  7",
                Instr(Mnemonic.mul_w, r0, r5, r10),
                Instr(Mnemonic.mulh_w, r0, r5, r10),
                Instr(Mnemonic.mulh_wu, r0, r5, r10),
                Instr(Mnemonic.mul_d, r0, r5, r10),
                Instr(Mnemonic.mulh_d, r0, r5, r10),
                Instr(Mnemonic.mulh_du, r0, r5, r10),
                Instr(Mnemonic.mulw_d_w, r0, r5, r10),
                Instr(Mnemonic.mulw_d_wu, r0, r5, r10));

            /*
            ##DIV.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   0 0 0 rk rj rd
            ##MOD.W rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   0 0 1 rk rj rd
            ##DIV.WU rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   0 1 0 rk rj rd
            ##MOD.WU rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   0 1 1 rk rj rd
            ##DIV.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   1 0 0 rk rj rd
            ##MOD.D rd, rj, rk#              <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   1 0 1 rk rj rd
            ##DIV.DU rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   1 1 0 rk rj rd
            ##MOD.DU rd, rj, rk#             <code>0 0 0 0 0 0   0 0 0 0   1 0 0 0   1 1 1 rk rj rd
            */
            var opcode_00_00_08 = Mask(15, 3, "  opcode 00:00:8",
                Instr(Mnemonic.div_w, r0, r5, r10),
                Instr(Mnemonic.mod_w, r0, r5, r10),
                Instr(Mnemonic.div_wu, r0, r5, r10),
                Instr(Mnemonic.mod_wu, r0, r5, r10),

                Instr(Mnemonic.div_d, r0, r5, r10),
                Instr(Mnemonic.mod_d, r0, r5, r10),
                Instr(Mnemonic.div_du, r0, r5, r10),
                Instr(Mnemonic.mod_du, r0, r5, r10));
            /*
            ##crc.w.b.w rd, rj, rk#          <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   0 0 0 rk rj rd
            ##crc.w.h.w rd, rj, rk#          <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   0 0 1 rk rj rd
            ##crc.w.w.w rd, rj, rk#          <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   0 1 0 rk rj rd
            ##crc.w.d.w rd, rj, rk#          <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   0 1 1 rk rj rd
            ##crcc.w.b.w rd, rj, rk#         <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   1 0 0 rk rj rd
            ##crcc.w.h.w rd, rj, rk#         <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   1 0 1 rk rj rd
            ##crcc.w.w.w rd, rj, rk#         <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   1 1 0 rk rj rd
            ##crcc.w.d.w rd, rj, rk#         <code>0 0 0 0 0 0   0 0 0 0   1 0 0 1   1 1 1 rk rj rd
            */
            var opcode_00_00_09 = Mask(15, 3, "00:00:09",
                Instr(Mnemonic.crc_w_b_w,  r0, r5, r10  ),
                Instr(Mnemonic.crc_w_h_w,  r0, r5, r10  ),
                Instr(Mnemonic.crc_w_w_w,  r0, r5, r10  ),
                Instr(Mnemonic.crc_w_d_w,  r0, r5, r10  ),
                Instr(Mnemonic.crcc_w_b_w, r0, r5, r10 ),
                Instr(Mnemonic.crcc_w_h_w, r0, r5, r10 ),
                Instr(Mnemonic.crcc_w_w_w, r0, r5, r10 ),
                Instr(Mnemonic.crcc_w_d_w, r0, r5, r10 ));

            /*
        ##BREAK code#                    <code>0 0 0 0 0 0   0 0 0 0   1 0 1 0   1 0 0 code
        ##DBCL code#                     <code>0 0 0 0 0 0   0 0 0 0   1 0 1 0   1 0 1 code
        ##SYSCALL code#                  <code>0 0 0 0 0 0   0 0 0 0   1 0 1 0   1 1 0 code
        */
            var opcode_00_00_0A = Sparse(15, 3, "00:00:0A", invalid,
                (4, Instr(Mnemonic.@break, ui15)),
                (5, Instr(Mnemonic.dbcl, ui15)),
                (6, Instr(Mnemonic.syscall, ui15)));

            /*
        ##ALSL.D rd, rj, rk, sa2#        <code>0 0 0 0 0 0   0 0 0 0   1 0 1 1   0 sa2 rk rj rd
        */
            var opcode_00_00 = Mask(18, 4, "  opcode 00:00",
                opcode_00_00_00,
                opcode_00_00_01,
                Mask(17, 1, "  00:00:02",
                    Instr(Mnemonic.bytepick_w, r0, r5, r10, sa2),
                    invalid),
                Instr(Mnemonic.bytepick_d, r0, r5, r10, sa3),

                opcode_00_00_04,
                opcode_00_00_05,
                opcode_00_00_06,
                opcode_00_00_07,

                opcode_00_00_08,
                opcode_00_00_09,
                opcode_00_00_0A,
                Mask(17, 1, "  00:00:0B",
                    Instr(Mnemonic.alsl_d, r0, r5, r10, sa2),
                    invalid),

                invalid,
                invalid,
                invalid,
                invalid);
            /*
            ##SLLI.W rd, rj, ui5#            <code>0 0 0 0 0 0   0 0 0 1   0 0 0 0   0 0 1  ui5 rj rd
            ##SLLI.D rd, rj, ui6#            <code>0 0 0 0 0 0   0 0 0 1   0 0 0 0   0 1    ui6 rj rd
            ##SRLI.W rd, rj, ui5#            <code>0 0 0 0 0 0   0 0 0 1   0 0 0 1   0 0 1  ui5 rj rd
            ##SRLI.D rd, rj, ui6#            <code>0 0 0 0 0 0   0 0 0 1   0 0 0 1   0 1    ui6 rj rd
            ##SRAI.W rd, rj, ui5#            <code>0 0 0 0 0 0   0 0 0 1   0 0 1 0   0 0 1  ui5 rj rd
            ##SRAI.D rd, rj, ui6#            <code>0 0 0 0 0 0   0 0 0 1   0 0 1 0   0 1    ui6 rj rd
            ##ROTRI.W rd, rj, ui5#           <code>0 0 0 0 0 0   0 0 0 1   0 0 1 1   0 0 1  ui5 rj rd
            ##ROTRI.D rd, rj, ui6#           <code>0 0 0 0 0 0   0 0 0 1   0 0 1 1   0 1    ui6 rj rd
            ##BSTRINS.W rd, rj, msbw, lsbw#  <code>0 0 0 0 0 0   0 0 0 1   1 msbw    0 lsbw     rj rd
            ##BSTRPICK.W rd, rj, msbw, lsbw# <code>0 0 0 0 0 0   0 0 0 1   1 msbw    1 lsbw     rj rd
            */
            var bstrins_w = nyi;
            var bstrpick_w = nyi;
            var opcode_00_01 = Mask(21, 1, "  opcode 00:1",
                Mask(18, 3, "  opcode 00:1:0",
                    Mask(15, 3, "  opcode 00:1:0:0",
                        invalid,
                        Instr(Mnemonic.slli_w, r0, r5, ui5),
                        Instr(Mnemonic.slli_d, r0, r5, ui6),
                        Instr(Mnemonic.slli_d, r0, r5, ui6),
                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(15, 3, "  opcode 00:1:0:1",
                        invalid,
                        Instr(Mnemonic.srli_w, r0, r5, ui5),
                        Instr(Mnemonic.srli_d, r0, r5, ui6),
                        Instr(Mnemonic.srli_d, r0, r5, ui6),
                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(15, 3, "  opcode 00:1:0:2",
                        invalid,
                        Instr(Mnemonic.srai_w, r0, r5, ui5),
                        Instr(Mnemonic.srai_d, r0, r5, ui6),
                        Instr(Mnemonic.srai_d, r0, r5, ui6),
                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(15, 3, "  opcode 00:1:0:2",
                        invalid,
                        Instr(Mnemonic.rotri_w, r0, r5, ui5),
                        Instr(Mnemonic.rotri_d, r0, r5, ui6),
                        Instr(Mnemonic.rotri_d, r0, r5, ui6),
                        invalid,
                        invalid,
                        invalid,
                        invalid),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(15, 1, "  opcode 00:1:1",
                    Instr(Mnemonic.bstrins_w, r0, r5, msbw, lsbw),
                    Instr(Mnemonic.bstrpick_w, r0, r5, msbw, lsbw)));

            /*
            ##BSTRINS.D rd, rj, msbd, lsbd#  <code>0 0 0 0 0 0   0 0 1 0   msbd lsbd rj rd

            ##BSTRPICK.D rd, rj, msbd, lsbd# <code>0 0 0 0 0 0   0 0 1 1   msbd lsbd rj rd

            ##FADD.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 0 0 0 1 fk fj fd
            ##FADD.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 0 0 1 0 fk fj fd
            ##FSUB.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 0 1 0 1 fk fj fd
            ##FSUB.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 0 1 1 0 fk fj fd
            ##FMUL.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 1 0 0 1 fk fj fd
            ##FMUL.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 1 0 1 0 fk fj fd
            ##FDIV.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 1 1 0 1 fk fj fd
            ##FDIV.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 0 1 1 1 0 fk fj fd
            ##FMAX.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 1 0 0 0 1 fk fj fd
            ##FMAX.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 1 0 0 1 0 fk fj fd
            ##FMIN.S fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 1 0 1 0 1 fk fj fd
            ##FMIN.D fd, fj, fk#             <code>0 0 0 0 0 0   0 1 0 0   0 0 1 0 1 1 0 fk fj fd
            ##FMAXA.S fd, fj, fk#            <code>0 0 0 0 0 0   0 1 0 0   0 0 1 1 0 0 1 fk fj fd
            ##FMAXA.D fd, fj, fk#            <code>0 0 0 0 0 0   0 1 0 0   0 0 1 1 0 1 0 fk fj fd
            ##FMINA.S fd, fj, fk#            <code>0 0 0 0 0 0   0 1 0 0   0 0 1 1 1 0 1 fk fj fd
            ##FMINA.D fd, fj, fk#            <code>0 0 0 0 0 0   0 1 0 0   0 0 1 1 1 1 0 fk fj fd
            ##FSCALEB.S fd, fj, fk#          <code>0 0 0 0 0 0   0 1 0 0   0 1 0 0 0 0 1 fk fj fd
            ##FSCALEB.D fd, fj, fk#          <code>0 0 0 0 0 0   0 1 0 0   0 1 0 0 0 1 0 fk fj fd
            ##FCOPYSIGN.S fd, fj, fk#        <code>0 0 0 0 0 0   0 1 0 0   0 1 0 0 1 0 1 fk fj fd
            ##FCOPYSIGN.D fd, fj, fk#        <code>0 0 0 0 0 0   0 1 0 0   0 1 0 0 1 1 0 fk fj fd

            ##FABS.S fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 0 0 0 1 fj fd
            ##FABS.D fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 0 0 1 0 fj fd
            ##FNEG.S fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 0 1 0 1 fj fd
            ##FNEG.D fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 0 1 1 0 fj fd
            ##FLOGB.S fd, fj#                <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 1 0 0 1 fj fd
            ##FLOGB.D fd, fj#                <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 1 0 1 0 fj fd
            ##FCLASS.S fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 1 1 0 1 fj fd
            ##FCLASS.D fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   0 1 1 1 0 fj fd
            ##FSQRT.S fd, fj#                <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 0 0 0 1 fj fd
            ##FSQRT.D fd, fj#                <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 0 0 1 0 fj fd
            ##FRECIP.S fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 0 1 0 1 fj fd
            ##FRECIP.D fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 0 1 1 0 fj fd
            ##FRSQRT.S fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 1 0 0 1 fj fd
            ##FRSQRT.D fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 0   1 1 0 1 0 fj fd

            ##FMOV.S fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 0 1 0 1 fj fd
            ##FMOV.D fd, fj#                 <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 0 1 1 0 fj fd
            ##MOVGR2FR.W fd, rj#             <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 0 0 1 rj fd
            ##MOVGR2FR.D fd, rj#             <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 0 1 0 rj fd
            ##MOVGR2FRH.W fd, rj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 0 1 1 rj fd
            ##MOVFR2GR.S rd, fj#             <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 1 0 1 fj rd
            ##MOVFR2GR.D rd, fj#             <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 1 1 0 fj rd
            ##MOVFRH2GR.S rd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   0 1 1 1 1 fj rd
            ##MOVGR2FCSR fcsr, rj#           <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 0 0 0 rj fcsr
            ##MOVFCSR2GR rd, fcsr#           <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 0 1 0 fcsr rd
            ##MOVFR2CF cd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 1 0 0 fj 0 0 cd
            ##MOVCF2FR fd, cj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 1 0 1 0 0 cj fd
            ##MOVGR2CF cd, rj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 1 1 0 rj 0 0 cd
            ##MOVCF2GR rd, cj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 0 1 0 0 1   1 0 1 1 1 0 0 cj rd

            ##FCVT.S.D fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 0 1 0   0 0 1 1 0 fj fd
            ##FCVT.D.S fd, fj#               <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 0 1 0   0 1 0 0 1 fj fd

            ##FTINTRM.W.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   0 0 0 0 1 fj fd
            ##FTINTRM.W.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   0 0 0 1 0 fj fd
            ##FTINTRM.L.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   0 1 0 0 1 fj fd
            ##FTINTRM.L.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   0 1 0 1 0 fj fd
            ##FTINTRP.W.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   1 0 0 0 1 fj fd
            ##FTINTRP.W.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   1 0 0 1 0 fj fd
            ##FTINTRP.L.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   1 1 0 0 1 fj fd
            ##FTINTRP.L.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 0   1 1 0 1 0 fj fd

            ##FTINTRZ.W.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   0 0 0 0 1 fj fd
            ##FTINTRZ.W.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   0 0 0 1 0 fj fd
            ##FTINTRZ.L.S fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   0 1 0 0 1 fj fd
            ##FTINTRZ.L.D fd, fj#            <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   0 1 0 1 0 fj fd
            ##FTINTRNE.W.S fd, fj#           <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   1 0 0 0 1 fj fd
            ##FTINTRNE.W.D fd, fj#           <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   1 0 0 1 0 fj fd
            ##FTINTRNE.L.S fd, fj#           <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   1 1 0 0 1 fj fd
            ##FTINTRNE.L.D fd, fj#           <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 0 1   1 1 0 1 0 fj fd

            ##FTINT.W.S fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 1 0   0 0 0 0 1 fj fd
            ##FTINT.W.D fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 1 0   0 0 0 1 0 fj fd
            ##FTINT.L.S fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 1 0   0 1 0 0 1 fj fd
            ##FTINT.L.D fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 0 1 1 0   0 1 0 1 0 fj fd

            ##FFINT.S.W fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 1 0 1 0   0 0 1 0 0 fj fd
            ##FFINT.S.L fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 1 0 1 0   0 0 1 1 0 fj fd
            ##FFINT.D.W fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 1 0 1 0   0 1 0 0 0 fj fd
            ##FFINT.D.L fd, fj#              <code>0 0 0 0 0 0   0 1 0 0   0 1 1 1 0 1 0   0 1 0 1 0 fj fd

            */
            var opcode_00_04 = Sparse(15, 7, "  000000:0100", invalid,
                (0b0000001, Instr(Mnemonic.fadd_s, f0, f5, f10)),
                (0b0000010, Instr(Mnemonic.fadd_d, f0, f5, f10)),
                (0b0000101, Instr(Mnemonic.fsub_s, f0, f5, f10)),
                (0b0000110, Instr(Mnemonic.fsub_d, f0, f5, f10)),
                (0b0001001, Instr(Mnemonic.fmul_s, f0, f5, f10)),
                (0b0001010, Instr(Mnemonic.fmul_d, f0, f5, f10)),
                (0b0001101, Instr(Mnemonic.fdiv_s, f0, f5, f10)),
                (0b0001110, Instr(Mnemonic.fdiv_d, f0, f5, f10)),
                (0b0010001, Instr(Mnemonic.fmax_s, f0, f5, f10)),
                (0b0010010, Instr(Mnemonic.fmax_d, f0, f5, f10)),
                (0b0010101, Instr(Mnemonic.fmin_s, f0, f5, f10)),
                (0b0010110, Instr(Mnemonic.fmin_d, f0, f5, f10)),
                (0b0011001, Instr(Mnemonic.fmaxa_s, f0, f5, f10)),
                (0b0011010, Instr(Mnemonic.fmaxa_d, f0, f5, f10)),
                (0b0011101, Instr(Mnemonic.fmina_s, f0, f5, f10)),
                (0b0011110, Instr(Mnemonic.fmina_d, f0, f5, f10)),
                (0b0100001, Instr(Mnemonic.fscaleb_s, f0, f5, f10)),
                (0b0100010, Instr(Mnemonic.fscaleb_d, f0, f5, f10)),
                (0b0100101, Instr(Mnemonic.fcopysign_s, f0, f5, f10)),
                (0b0100110, Instr(Mnemonic.fcopysign_d, f0, f5, f10)),
                (0b0101000, Sparse(10, 5, "  0:4:0101000", invalid,
                    (0b00001, Instr(Mnemonic.fabs_s, f0, f5)),
                    (0b00010, Instr(Mnemonic.fabs_d, f0, f5)),
                    (0b00101, Instr(Mnemonic.fneg_s, f0, f5)),
                    (0b00110, Instr(Mnemonic.fneg_d, f0, f5)),
                    (0b01001, Instr(Mnemonic.flogb_s, f0, f5)),
                    (0b01010, Instr(Mnemonic.flogb_d, f0, f5)),
                    (0b01101, Instr(Mnemonic.fclass_s, f0, f5)),
                    (0b01110, Instr(Mnemonic.fclass_d, f0, f5)),
                    (0b10001, Instr(Mnemonic.fsqrt_s, f0, f5)),
                    (0b10010, Instr(Mnemonic.fsqrt_d, f0, f5)),
                    (0b10101, Instr(Mnemonic.frecip_s, f0, f5)),
                    (0b10110, Instr(Mnemonic.frecip_d, f0, f5)),
                    (0b11001, Instr(Mnemonic.frsqrt_s, f0, f5)),
                    (0b11010, Instr(Mnemonic.frsqrt_d, f0, f5)))),
                (0b0101001, Sparse(10, 5, "  0:4:0101001", invalid,
                    (0b00101, Instr(Mnemonic.fmov_s, f0, f5)),
                    (0b00110, Instr(Mnemonic.fmov_d, f0, f5)),
                    (0b01001, Instr(Mnemonic.movgr2fr_w, f0, r5)),
                    (0b01010, Instr(Mnemonic.movgr2fr_d, f0, r5)),
                    (0b01011, Instr(Mnemonic.movgr2frh_w, f0, r5)),
                    (0b01101, Instr(Mnemonic.movfr2gr_s, r0, f5)),
                    (0b01110, Instr(Mnemonic.movfr2gr_d, r0, f5)),
                    (0b01111, Instr(Mnemonic.movfrh2gr_s, r0, f5)),
                    (0b10000, Instr(Mnemonic.movgr2fcsr, ui0, r5)),
                    (0b10010, Instr(Mnemonic.movfcsr2gr, r0, ui5)),
                    (0b10100, Instr(Mnemonic.movfr2cf, c0, f5)),
                    (0b10101, Instr(Mnemonic.movcf2fr, f0, c5)),
                    (0b10110, Instr(Mnemonic.movgr2cf, c0, r5)),
                    (0b10111, Instr(Mnemonic.movcf2gr, r0, c5)))),
                (0b0110010, Sparse(10, 5, "  0:4:0110010", invalid,
                    (6, Instr(Mnemonic.fcvt_s_d, f0, f5)),
                    (9, Instr(Mnemonic.fcvt_d_s, f0, f5)))),
                (0b0110100, Sparse(10, 5, "  0:4:0110100", invalid,
                    (0b00001, Instr(Mnemonic.ftintrm_w_s, f0, f5)),
                    (0b00010, Instr(Mnemonic.ftintrm_w_d, f0, f5)),
                    (0b01001, Instr(Mnemonic.ftintrm_l_s, f0, f5)),
                    (0b01010, Instr(Mnemonic.ftintrm_l_d, f0, f5)),
                    (0b10001, Instr(Mnemonic.ftintrp_w_s, f0, f5)),
                    (0b10010, Instr(Mnemonic.ftintrp_w_d, f0, f5)),
                    (0b11001, Instr(Mnemonic.ftintrp_l_s, f0, f5)),
                    (0b11010, Instr(Mnemonic.ftintrp_l_d, f0, f5)))),
                (0b0110101, Sparse(10, 5, "  0:4:0110101", invalid,
                    (0b00001, Instr(Mnemonic.ftintrz_w_s, f0, f5)),
                    (0b00010, Instr(Mnemonic.ftintrz_w_d, f0, f5)),
                    (0b01001, Instr(Mnemonic.ftintrz_l_s, f0, f5)),
                    (0b01010, Instr(Mnemonic.ftintrz_l_d, f0, f5)),
                    (0b10001, Instr(Mnemonic.ftintrne_w_s, f0, f5)),
                    (0b10010, Instr(Mnemonic.ftintrne_w_d, f0, f5)),
                    (0b11001, Instr(Mnemonic.ftintrne_l_s, f0, f5)),
                    (0b11010, Instr(Mnemonic.ftintrne_l_d, f0, f5)))),
                (0b0110110, Sparse(10, 5, "  0:4:0110110", invalid,
                    (0b00001, Instr(Mnemonic.ftint_w_s, f0, f5)),
                    (0b00010, Instr(Mnemonic.ftint_w_d, f0, f5)),
                    (0b01001, Instr(Mnemonic.ftint_l_s, f0, f5)),
                    (0b01010, Instr(Mnemonic.ftint_l_d, f0, f5)))),
                (0b0111010, Sparse(10, 5, "  0:4:0111010", invalid,
                    (0b00100, Instr(Mnemonic.ffint_s_w, f0, f5)),
                    (0b00110, Instr(Mnemonic.ffint_s_l, f0, f5)),
                    (0b01000, Instr(Mnemonic.ffint_d_w, f0, f5)),
                    (0b01010, Instr(Mnemonic.ffint_d_l, f0, f5)))),
                (0b0111100, Sparse(10, 5, "  0:4:0111100", invalid,
                    (0b10001, Instr(Mnemonic.frint_s, f0, f5)))));

            /*
            ##SLTI rd, rj, si12#             <code>0 0 0 0 0 0   1 0 0 0   si12 rj rd
            ##SLTUI rd, rj, si12#            <code>0 0 0 0 0 0   1 0 0 1   si12 rj rd
            ##ADDI.W rd, rj, si12#           <code>0 0 0 0 0 0   1 0 1 0   si12 rj rd
            ##ADDI.D rd, rj, si12#           <code>0 0 0 0 0 0   1 0 1 1   si12 rj rd
            ##LU52I.D rd, rj, si12#          <code>0 0 0 0 0 0   1 1 0 0   si12 rj rd
            ##ANDI rd, rj, ui12#             <code>0 0 0 0 0 0   1 1 0 1   ui12 rj rd
            ##ORI rd, rj, ui12#              <code>0 0 0 0 0 0   1 1 1 0   ui12 rj rd
            ##XORI rd, rj, ui12#             <code>0 0 0 0 0 0   1 1 1 1   ui12 rj rd
            */
            var opcode_00 = Mask(22, 4, "  opcode 0",
                opcode_00_00,
                opcode_00_01,
                Instr(Mnemonic.bstrins_d, r0, r5, msbd, lsbd),
                Instr(Mnemonic.bstrpick_d, r0, r5, msbd, lsbd),

                opcode_00_04,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.slti, r0, r5, si12),
                Instr(Mnemonic.sltui, r0, r5, si12),
                Instr(Mnemonic.addi_w, r0, r5, si12),
                Instr(Mnemonic.addi_d, r0, r5, si12),

                Instr(Mnemonic.lu52i_d, r0, r5, si12),
                Instr(Mnemonic.andi, r0, r5, ui12),
                Instr(Mnemonic.ori, r0, r5, ui12),
                Instr(Mnemonic.xori, r0, r5, ui12)
            );
            /*
            ##CSRRD rd, csr#          <code>0 0 0 0 0 1   0 0 csr 0 0 0 0 0 rd
            ##CSRWR rd, csr#          <code>0 0 0 0 0 1   0 0 csr 0 0 0 0 1 rd
            ##CSRXCHG rd, rj, csr#    <code>0 0 0 0 0 1   0 0 csr rj!=0,1 rd
            ##CACOP code, rj, si12#   <code>0 0 0 0 0 1   1 0   0 0 si12 rj code
            ##LDDIR rd, rj, level#    <code>0 0 0 0 0 1   1 0   0 1  0 0 0 0  level rj rd
            ##LDPTE rj, seq#          <code>0 0 0 0 0 1   1 0   0 1  0 0 0 1  seq rj 0 0 0 0 0
            ##IOCSRRD.B rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 0 0 0 rj rd
            ##IOCSRRD.H rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 0 0 1 rj rd
            ##IOCSRRD.W rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 0 1 0 rj rd
            ##IOCSRRD.D rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 0 1 1 rj rd
            ##IOCSRWR.B rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 1 0 0 rj rd
            ##IOCSRWR.H rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 1 0 1 rj rd
            ##IOCSRWR.W rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 1 1 0 rj rd
            ##IOCSRWR.D rd, rj#       <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 0 1 1 1 rj rd
            ##TLBCLR                  <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 0 0 0 0 0 0 0 0 0 0 0 0 0
            ##TLBFLUSH#               <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 0 0 1 0 0 0 0 0 0 0 0 0 0
            ##TLBSRCH#                <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 0 1 0 0 0 0 0 0 0 0 0 0 0
            ##TLBRD#                  <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 0 1 1 0 0 0 0 0 0 0 0 0 0
            ##TLBWR#                  <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 1 0 0 0 0 0 0 0 0 0 0 0 0
            ##TLBFILL#                <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 1 0 1 0 0 0 0 0 0 0 0 0 0
            ##ERTN#                   <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 0  0 1 1 1 0 0 0 0 0 0 0 0 0 0 0
            ##IDLE level#             <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 0 1  level
            ##INVTLB op, rj, rk#      <code>0 0 0 0 0 1   1 0   0 1  0 0 1 0  0 1 0  rk rj op
            */
            var opcode_01 = Mask(24, 2, "  opcode 01",
                Sparse(5, 5, "  0000001:00", 
                    Instr(Mnemonic.csrxchg, InstrClass.Linear|InstrClass.Privileged, r0, csr),
                    (0, Instr(Mnemonic.csrrd, InstrClass.Linear | InstrClass.Privileged, r0, csr)),
                    (1, Instr(Mnemonic.csrwr, InstrClass.Linear | InstrClass.Privileged, r0, csr))),
                invalid,
                Mask(22, 2, "  000001:10",
                    Instr(Mnemonic.cacop, ui0, r5, si12),
                    Sparse(18, 4, "  000001:10:01", invalid,
                        (0, Instr(Mnemonic.lddir, r0, r5, level)),
                        (1, Instr(Mnemonic.ldpte, r5, seq)),
                        (2, Sparse(15, 3, "  000001:10:010:0010", invalid,
                            (0, Sparse(10, 5, "  000001:10:010:0010:000", invalid,
                                (0x0, Instr(Mnemonic.iocsrrd_b, r0, r5)),
                                (0x1, Instr(Mnemonic.iocsrrd_h, r0, r5)),
                                (0x2, Instr(Mnemonic.iocsrrd_w, r0, r5)),
                                (0x3, Instr(Mnemonic.iocsrrd_d, r0, r5)),
                                (0x4, Instr(Mnemonic.iocsrwr_b, r0, r5)),
                                (0x5, Instr(Mnemonic.iocsrwr_h, r0, r5)),
                                (0x6, Instr(Mnemonic.iocsrwr_w, r0, r5)),
                                (0x7, Instr(Mnemonic.iocsrwr_d, r0, r5)),
                                (0x8, If(0, 10, Eq0, Instr(Mnemonic.tlbclr))),
                                (0x9, If(0, 10, Eq0, Instr(Mnemonic.tlbflush))),
                                (0xA, If(0, 10, Eq0, Instr(Mnemonic.tlbsrch ))),
                                (0xB, If(0, 10, Eq0, Instr(Mnemonic.tlbrd   ))),
                                (0xC, If(0, 10, Eq0, Instr(Mnemonic.tlbwr   ))),
                                (0xD, If(0, 10, Eq0, Instr(Mnemonic.tlbfill ))),
                                (0xE, If(0, 10, Eq0, Instr(Mnemonic.ertn))))),
                            (1, Instr(Mnemonic.idle, ui15)),
                            (2, Instr(Mnemonic.invtlb, ui5, r5, r10))))),
                    invalid,
                    invalid),
                invalid);
            /*
            ##FMADD.S fd, fj, fk, fa# <code>0 0 0 0 1 0   0 0 0 0 0 1   fa fk fj fd
            ##FMADD.D fd, fj, fk, fa# <code>0 0 0 0 1 0   0 0 0 0 1 0   fa fk fj fd
            ##FMSUB.S fd, fj, fk, fa# <code>0 0 0 0 1 0   0 0 0 1 0 1   fa fk fj fd
            ##FMSUB.D fd, fj, fk, fa# <code>0 0 0 0 1 0   0 0 0 1 1 0   fa fk fj fd
            ##FNMADD.S fd, fj, fk, fa#<code>0 0 0 0 1 0   0 0 1 0 0 1   fa fk fj fd
            ##FNMADD.D fd, fj, fk, fa#<code>0 0 0 0 1 0   0 0 1 0 1 0   fa fk fj fd
            ##FNMSUB.S fd, fj, fk, fa#<code>0 0 0 0 1 0   0 0 1 1 0 1   fa fk fj fd
            ##FNMSUB.D fd, fj, fk, fa#<code>0 0 0 0 1 0   0 0 1 1 1 0   fa fk fj fd
            ##FCMP.cond.S cd, fj, fk# <code>0 0 0 0 1 1   0 0 0 0 0 1   cond fk fj 0 0 cd
            ##FCMP.cond.D cd, fj, fk# <code>0 0 0 0 1 1   0 0 0 0 1 0   cond fk fj 0 0 cd
            ##FSEL fd, fj, fk, ca#    <code>0 0 0 0 1 1   0 1 0 0 0 0   0 0 ca fk fj fd
            
            ##ADDU16I.D rd, rj, si16# <code>0 0 0 1 0 0   si16 rj rd
            
            ##LU12I.W rd, si20#       <code>0 0 0 1 0 1   0 si20 rd
            ##LU32I.D rd, si20#       <code>0 0 0 1 0 1   1 si20 rd
            */
            var opcode_05 = Mask(25, 1, "  opcode 05",
                Instr(Mnemonic.lu12i_w, r0, si20),
                Instr(Mnemonic.lu32i_d, r0, si20));
            /*
            ##PCADDI rd, si20#        <code>0 0 0 1 1 0   0 si20 rd
            ##PCALAU12I rd, si20#     <code>0 0 0 1 1 0   1 si20 rd

            ##PCADDU12I rd, si20#     <code>0 0 0 1 1 1   0 si20 rd
            ##PCADDU18I rd, si20#     <code>0 0 0 1 1 1   1 si20 rd
            
            ##LL.W rd, rj, si14#      <code>0 0 1 0 0 0   0 0 si14 rj rd
            ##SC.W rd, rj, si14#      <code>0 0 1 0 0 0   0 1 si14 rj rd
            ##LL.D rd, rj, si14#      <code>0 0 1 0 0 0   1 0 si14 rj rd
            ##SC.D rd, rj, si14#      <code>0 0 1 0 0 0   1 1 si14 rj rd

            ##LDPTR.W rd, rj, si14#   <code>0 0 1 0 0 1   0 0 si14 rj rd
            ##STPTR.W rd, rj, si14#   <code>0 0 1 0 0 1   0 1 si14 rj rd
            ##LDPTR.D rd, rj, si14#   <code>0 0 1 0 0 1   1 0 si14 rj rd
            ##STPTR.D rd, rj, si14#   <code>0 0 1 0 0 1   1 1 si14 rj rd
            */
            var opcode_09 = Mask(24, 2, "  opcode 09",
                Instr(Mnemonic.ldptr_w, r0, r5, si14),
                Instr(Mnemonic.stptr_w, r0, r5, si14),
                Instr(Mnemonic.ldptr_d, r0, r5, si14),
                Instr(Mnemonic.stptr_d, r0, r5, si14));
            /*
            ##LD.B rd, rj, si12#      <code>0 0 1 0 1 0   0 0 0 0 si12 rj rd
            ##LD.H rd, rj, si12#      <code>0 0 1 0 1 0   0 0 0 1 si12 rj rd
            ##LD.W rd, rj, si12#      <code>0 0 1 0 1 0   0 0 1 0 si12 rj rd
            ##LD.D rd, rj, si12#      <code>0 0 1 0 1 0   0 0 1 1 si12 rj rd
            ##ST.B rd, rj, si12#      <code>0 0 1 0 1 0   0 1 0 0 si12 rj rd
            ##ST.H rd, rj, si12#      <code>0 0 1 0 1 0   0 1 0 1 si12 rj rd
            ##ST.W rd, rj, si12#      <code>0 0 1 0 1 0   0 1 1 0 si12 rj rd
            ##ST.D rd, rj, si12#      <code>0 0 1 0 1 0   0 1 1 1 si12 rj rd
            ##LD.BU rd, rj, si12#     <code>0 0 1 0 1 0   1 0 0 0 si12 rj rd
            ##LD.HU rd, rj, si12#     <code>0 0 1 0 1 0   1 0 0 1 si12 rj rd
            ##LD.WU rd, rj, si12#     <code>0 0 1 0 1 0   1 0 1 0 si12 rj rd
            ##PRELD hint, rj, si12#   <code>0 0 1 0 1 0   1 0 1 1 si12 rj hint
            ##FLD.S fd, rj, si12#     <code>0 0 1 0 1 0   1 1 0 0 si12 rj fd
            ##FST.S fd, rj, si12#     <code>0 0 1 0 1 0   1 1 0 1 si12 rj fd
            ##FLD.D fd, rj, si12#     <code>0 0 1 0 1 0   1 1 1 0 si12 rj fd
            ##FST.D fd, rj, si12#     <code>0 0 1 0 1 0   1 1 1 1 si12 rj fd
            */
            var opcode_0A = Mask(22, 4, "  opcode 0A",
                Instr(Mnemonic.ld_b, r0, r5, si12),
                Instr(Mnemonic.ld_h, r0, r5, si12),
                Instr(Mnemonic.ld_w, r0, r5, si12),
                Instr(Mnemonic.ld_d, r0, r5, si12),
                Instr(Mnemonic.st_b, r0, r5, si12),
                Instr(Mnemonic.st_h, r0, r5, si12),
                Instr(Mnemonic.st_w, r0, r5, si12),
                Instr(Mnemonic.st_d, r0, r5, si12),
                Instr(Mnemonic.ld_bu, r0, r5, si12),
                Instr(Mnemonic.ld_hu, r0, r5, si12),
                Instr(Mnemonic.ld_wu, r0, r5, si12),
                Instr(Mnemonic.preld, ui0, r5, si12),
                Instr(Mnemonic.fld_s, f0, r5, si12),
                Instr(Mnemonic.fst_s, f0, r5, si12),
                Instr(Mnemonic.fld_d, f0, r5, si12),
                Instr(Mnemonic.fst_d, f0, r5, si12));
            /*
            ##LDX.B rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 0 0 0 0 0 0 rk rj rd
            ##LDX.H rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 0 0 1 0 0 0 rk rj rd
            ##LDX.W rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 0 1 0 0 0 0 rk rj rd
            ##LDX.D rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 0 1 1 0 0 0 rk rj rd
            ##STX.B rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 1 0 0 0 0 0 rk rj rd
            ##STX.H rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 1 0 1 0 0 0 rk rj rd
            ##STX.W rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 1 1 0 0 0 0 rk rj rd
            ##STX.D rd, rj, rk#       <code>0 0 1 1 1 0   0 0 0 0 0 1 1 1 0 0 0 rk rj rd
            ##LDX.BU rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 0 0 0 0 0 0 rk rj rd
            ##LDX.HU rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 0 0 1 0 0 0 rk rj rd
            ##LDX.WU rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 0 1 0 0 0 0 rk rj rd
            ##PRELDX hint, rj, rk#    <code>0 0 1 1 1 0   0 0 0 0 1 0 1 1 0 0 0 rk rj hint
            ##FLDX.S fd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 1 0 0 0 0 0 rk rj fd
            ##FLDX.D fd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 1 0 1 0 0 0 rk rj fd
            ##FSTX.S fd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 1 1 0 0 0 0 rk rj fd
            ##FSTX.D fd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 0 1 1 1 1 0 0 0 rk rj fd
            ##AMSWAP.W rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 0 0 0 rk rj rd
            ##AMSWAP.D rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 0 0 1 rk rj rd
            ##AMADD.W rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 0 1 0 rk rj rd
            ##AMADD.D rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 0 1 1 rk rj rd
            ##AMAND.W rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 1 0 0 rk rj rd
            ##AMAND.D rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 1 0 1 rk rj rd
            ##AMOR.W rd, rk, rj#      <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 1 1 0 rk rj rd
            ##AMOR.D rd, rk, rj#      <code>0 0 1 1 1 0   0 0 0 1 1 0 0 0 1 1 1 rk rj rd
            ##AMXOR.W rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 0 0 0 rk rj rd
            ##AMXOR.D rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 0 0 1 rk rj rd
            ##AMMAX.W rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 0 1 0 rk rj rd
            ##AMMAX.D rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 0 1 1 rk rj rd
            ##AMMIN.W rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 1 0 0 rk rj rd
            ##AMMIN.D rd, rk, rj#     <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 1 0 1 rk rj rd
            ##AMMAX.WU rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 1 1 0 rk rj rd
            ##AMMAX.DU rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 0 1 1 1 1 rk rj rd
            ##AMMIN.WU rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 0 0 0 rk rj rd
            ##AMMIN.DU rd, rk, rj#    <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 0 0 1 rk rj rd
            ##AMSWAP_DB.W rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 0 1 0 rk rj rd
            ##AMSWAP_DB.D rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 0 1 1 rk rj rd
            ##AMADD_DB.W rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 1 0 0 rk rj rd
            ##AMADD_DB.D rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 1 0 1 rk rj rd
            ##AMAND_DB.W rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 1 1 0 rk rj rd
            ##AMAND_DB.D rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 0 1 1 1 rk rj rd
            ##AMOR_DB.W rd, rk, rj#   <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 0 0 0 rk rj rd
            ##AMOR_DB.D rd, rk, rj#   <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 0 0 1 rk rj rd
            ##AMXOR_DB.W rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 0 1 0 rk rj rd
            ##AMXOR_DB.D rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 0 1 1 rk rj rd
            ##AMMAX_DB.W rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 1 0 0 rk rj rd
            ##AMMAX_DB.D rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 1 0 1 rk rj rd
            ##AMMIN_DB.W rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 1 1 0 rk rj rd
            ##AMMIN_DB.D rd, rk, rj#  <code>0 0 1 1 1 0   0 0 0 1 1 0 1 1 1 1 1 rk rj rd
            ##AMMAX_DB.WU rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 0 0 0 rk rj rd
            ##AMMAX_DB.DU rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 0 0 1 rk rj rd
            ##AMMIN_DB.WU rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 0 1 0 rk rj rd
            ##AMMIN_DB.DU rd, rk, rj# <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 0 1 1 rk rj rd
            ##DBAR hint#              <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 1 0 0 hint
            ##IBAR hint#              <code>0 0 1 1 1 0   0 0 0 1 1 1 0 0 1 0 1 hint
            ##FLDGT.S fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 0 0 0 rk rj fd
            ##FLDGT.D fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 0 0 1 rk rj fd
            ##FLDLE.S fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 0 1 0 rk rj fd
            ##FLDLE.D fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 0 1 1 rk rj fd
            ##FSTGT.S fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 1 0 0 rk rj fd
            ##FSTGT.D fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 1 0 1 rk rj fd
            ##FSTLE.S fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 1 1 0 rk rj fd
            ##FSTLE.D fd, rj, rk#     <code>0 0 1 1 1 0   0 0 0 1 1 1 0 1 1 1 1 rk rj fd
            ##LDGT.B rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 0 0 0 rk rj rd
            ##LDGT.H rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 0 0 1 rk rj rd
            ##LDGT.W rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 0 1 0 rk rj rd
            ##LDGT.D rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 0 1 1 rk rj rd
            ##LDLE.B rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 1 0 0 rk rj rd
            ##LDLE.H rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 1 0 1 rk rj rd
            ##LDLE.W rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 1 1 0 rk rj rd
            ##LDLE.D rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 0 1 1 1 rk rj rd
            ##STGT.B rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 0 0 0 rk rj rd
            ##STGT.H rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 0 0 1 rk rj rd
            ##STGT.W rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 0 1 0 rk rj rd
            ##STGT.D rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 0 1 1 rk rj rd
            ##STLE.B rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 1 0 0 rk rj rd
            ##STLE.H rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 1 0 1 rk rj rd
            ##STLE.W rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 1 1 0 rk rj rd
            ##STLE.D rd, rj, rk#      <code>0 0 1 1 1 0   0 0 0 1 1 1 1 1 1 1 1 rk rj rd
            */
            var opcode_0E = Sparse(15, 11, "  0E", invalid,
                (0b00000000000, Instr(Mnemonic.ldx_b, r0, r5, r10)),
                (0b00000001000, Instr(Mnemonic.ldx_h, r0, r5, r10)),
                (0b00000010000, Instr(Mnemonic.ldx_w, r0, r5, r10)),
                (0b00000011000, Instr(Mnemonic.ldx_d, r0, r5, r10)),
                (0b00000100000, Instr(Mnemonic.stx_b, r0, r5, r10)),
                (0b00000101000, Instr(Mnemonic.stx_h, r0, r5, r10)),
                (0b00000110000, Instr(Mnemonic.stx_w, r0, r5, r10)),
                (0b00000111000, Instr(Mnemonic.stx_d, r0, r5, r10)),
                (0b00001000000, Instr(Mnemonic.ldx_bu, r0, r5, r10)),
                (0b00001001000, Instr(Mnemonic.ldx_hu, r0, r5, r10)),
                (0b00001010000, Instr(Mnemonic.ldx_wu, r0, r5, r10)),
                (0b00001011000, Instr(Mnemonic.preldx, ui0, r5, r10)),
                (0b00001100000, Instr(Mnemonic.fldx_s, f0, r5, r10)),
                (0b00001101000, Instr(Mnemonic.fldx_d, f0, r5, r10)),
                (0b00001110000, Instr(Mnemonic.fstx_s, f0, r5, r10)),
                (0b00001111000, Instr(Mnemonic.fstx_d, f0, r5, r10)),
                (0b00011000000, Instr(Mnemonic.amswap_w, r0, r10, r5)),
                (0b00011000001, Instr(Mnemonic.amswap_d, r0, r10, r5)),
                (0b00011000010, Instr(Mnemonic.amadd_w, r0, r10, r5)),
                (0b00011000011, Instr(Mnemonic.amadd_d, r0, r10, r5)),
                (0b00011000100, Instr(Mnemonic.amand_w, r0, r10, r5)),
                (0b00011000101, Instr(Mnemonic.amand_d, r0, r10, r5)),
                (0b00011000110, Instr(Mnemonic.amor_w, r0, r10, r5)),
                (0b00011000111, Instr(Mnemonic.amor_d, r0, r10, r5)),
                (0b00011001000, Instr(Mnemonic.amxor_w, r0, r10, r5)),
                (0b00011001001, Instr(Mnemonic.amxor_d, r0, r10, r5)),
                (0b00011001010, Instr(Mnemonic.ammax_w, r0, r10, r5)),
                (0b00011001011, Instr(Mnemonic.ammax_d, r0, r10, r5)),
                (0b00011001100, Instr(Mnemonic.ammin_w, r0, r10, r5)),
                (0b00011001101, Instr(Mnemonic.ammin_d, r0, r10, r5)),
                (0b00011001110, Instr(Mnemonic.ammax_wu, r0, r10, r5)),
                (0b00011001111, Instr(Mnemonic.ammax_du, r0, r10, r5)),
                (0b00011010000, Instr(Mnemonic.ammin_wu, r0, r10, r5)),
                (0b00011010001, Instr(Mnemonic.ammin_du, r0, r10, r5)),
                (0b00011010010, Instr(Mnemonic.amswap_db_w, r0, r10, r5)),
                (0b00011010011, Instr(Mnemonic.amswap_db_d, r0, r10, r5)),
                (0b00011010100, Instr(Mnemonic.amadd_db_w, r0, r10, r5)),
                (0b00011010101, Instr(Mnemonic.amadd_db_d, r0, r10, r5)),
                (0b00011010110, Instr(Mnemonic.amand_db_w, r0, r10, r5)),
                (0b00011010111, Instr(Mnemonic.amand_db_d, r0, r10, r5)),
                (0b00011011000, Instr(Mnemonic.amor_db_w, r0, r10, r5)),
                (0b00011011001, Instr(Mnemonic.amor_db_d, r0, r10, r5)),
                (0b00011011010, Instr(Mnemonic.amxor_db_w, r0, r10, r5)),
                (0b00011011011, Instr(Mnemonic.amxor_db_d, r0, r10, r5)),
                (0b00011011100, Instr(Mnemonic.ammax_db_w, r0, r10, r5)),
                (0b00011011101, Instr(Mnemonic.ammax_db_d, r0, r10, r5)),
                (0b00011011110, Instr(Mnemonic.ammin_db_w, r0, r10, r5)),
                (0b00011011111, Instr(Mnemonic.ammin_db_d, r0, r10, r5)),
                (0b00011100000, Instr(Mnemonic.ammax_db_wu, r0, r10, r5)),
                (0b00011100001, Instr(Mnemonic.ammax_db_du, r0, r10, r5)),
                (0b00011100010, Instr(Mnemonic.ammin_db_wu, r0, r10, r5)),
                (0b00011100011, Instr(Mnemonic.ammin_db_du, r0, r10, r5)),
                (0b00011100100, Instr(Mnemonic.dbar, ui15)),
                (0b00011100101, Instr(Mnemonic.ibar, ui15)),
                (0b00011101000, Instr(Mnemonic.fldgt_s, f0, r5, r10)),
                (0b00011101001, Instr(Mnemonic.fldgt_d, f0, r5, r10)),
                (0b00011101010, Instr(Mnemonic.fldle_s, f0, r5, r10)),
                (0b00011101011, Instr(Mnemonic.fldle_d, f0, r5, r10)),
                (0b00011101100, Instr(Mnemonic.fstgt_s, f0, r5, r10)),
                (0b00011101101, Instr(Mnemonic.fstgt_d, f0, r5, r10)),
                (0b00011101110, Instr(Mnemonic.fstle_s, f0, r5, r10)),
                (0b00011101111, Instr(Mnemonic.fstle_d, f0, r5, r10)),
                (0b00011110000, Instr(Mnemonic.ldgt_b, r0, r5, r10)),
                (0b00011110001, Instr(Mnemonic.ldgt_h, r0, r5, r10)),
                (0b00011110010, Instr(Mnemonic.ldgt_w, r0, r5, r10)),
                (0b00011110011, Instr(Mnemonic.ldgt_d, r0, r5, r10)),
                (0b00011110100, Instr(Mnemonic.ldle_b, r0, r5, r10)),
                (0b00011110101, Instr(Mnemonic.ldle_h, r0, r5, r10)),
                (0b00011110110, Instr(Mnemonic.ldle_w, r0, r5, r10)),
                (0b00011110111, Instr(Mnemonic.ldle_d, r0, r5, r10)),
                (0b00011111000, Instr(Mnemonic.stgt_b, r0, r5, r10)),
                (0b00011111001, Instr(Mnemonic.stgt_h, r0, r5, r10)),
                (0b00011111010, Instr(Mnemonic.stgt_w, r0, r5, r10)),
                (0b00011111011, Instr(Mnemonic.stgt_d, r0, r5, r10)),
                (0b00011111100, Instr(Mnemonic.stle_b, r0, r5, r10)),
                (0b00011111101, Instr(Mnemonic.stle_h, r0, r5, r10)),
                (0b00011111110, Instr(Mnemonic.stle_w, r0, r5, r10)),
                (0b00011111111, Instr(Mnemonic.stle_d, r0, r5, r10)));

            /*
            ##BEQZ rj, offs#          <code>0 1 0 0 0 0   offs[15:0] rj offs[20:16]

            ##BNEZ rj, offs#          <code>0 1 0 0 0 1   offs[15:0] rj offs[20:16]

            ##BCEQZ cj, offs#         <code>0 1 0 0 1 0   offs[15:0] 0 0 cj offs[20:16]

            ##BCNEZ cj, offs#         <code>0 1 0 0 1 0   offs[15:0] 0 1 cj offs[20:16]

            ##JIRL rd, rj, offs#      <code>0 1 0 0 1 1   offs[15:0] rj rd

            ##B offs#                 <code>0 1 0 1 0 0   offs[15:0] offs[25:16]

            ##BL offs#                <code>0 1 0 1 0 1   offs[15:0] offs[25:16]
         */

            rootDecoder = Sparse(26, 6, "LoongArch", invalid,
                (0b000000, opcode_00),
                (0b000001, opcode_01),
                (0b000101, opcode_05),
                (0b001001, opcode_09),
                (0b001010, opcode_0A),
                (0b001110, opcode_0E),

                (0b010000, Instr(Mnemonic.beqz, InstrClass.ConditionalTransfer, r5,j21)),
                (0b010001, Instr(Mnemonic.bnez, InstrClass.ConditionalTransfer, r5,j21)),
                (0b010011, Instr(Mnemonic.jirl, InstrClass.Transfer|InstrClass.Call, r5,r0,jr16)),
                (0b010100, Instr(Mnemonic.b, InstrClass.Transfer, J)),
                (0b010101, Instr(Mnemonic.bl, InstrClass.Transfer|InstrClass.Call, J)),
                (0b010110, Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, r5,r0,j16)),
                (0b010111, Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, r5,r0,j16)),
                (0b011000, Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, r5,r0,j16)),
                (0b011001, Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, r5,r0,j16)),
                (0b011010, Instr(Mnemonic.bltu, InstrClass.ConditionalTransfer, r5,r0,j16)),
                (0b011011, Instr(Mnemonic.bgeu, InstrClass.ConditionalTransfer, r5,r0,j16))
                );
        }
    }
}