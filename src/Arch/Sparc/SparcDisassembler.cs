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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    using Decoder = Decoder<SparcDisassembler, Mnemonic, SparcInstruction>;

    public class SparcDisassembler : DisassemblerBase<SparcInstruction>
    {
        private const InstrClass Transfer = InstrClass.Delay | InstrClass.Transfer;
        private const InstrClass CondTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Conditional;
        private const InstrClass LinkTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Call;

        private static readonly Decoder rootDecoder;
        private static readonly Decoder[] branchOps;

        private SparcInstruction instrCur;
        private EndianImageReader imageReader;
        private readonly List<MachineOperand> ops;

        public SparcDisassembler(SparcArchitecture arch, EndianImageReader imageReader)
        {
            this.imageReader = imageReader;
            this.ops = new List<MachineOperand>();
        }

        public override SparcInstruction DisassembleInstruction()
        {
            if (!imageReader.IsValid)
                return null;
            ops.Clear();
            var addr = imageReader.Address;
            uint wInstr = imageReader.ReadBeUInt32();
            instrCur = rootDecoder.Decode(wInstr, this);
            instrCur.Address = addr;
            instrCur.Length = 4;
            instrCur.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            return instrCur;
        }

        protected override SparcInstruction CreateInvalidInstruction()
        {
            return invalid.Decode(0, this);
        }

        private sealed class InstrDecoder : Decoder
        {
            public Mnemonic code;
            public InstrClass iclass = InstrClass.Linear;
            public Mutator<SparcDisassembler>[] mutators;

            public override SparcInstruction Decode(uint wInstr, SparcDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return invalid.Decode(wInstr, dasm);
                }
                return new SparcInstruction
                {
                    Mnemonic = code,
                    InstructionClass = iclass,
                    Operands = dasm.ops.ToArray()
                };
            }
        }

        #region Mutators

        // Register reference
        private static Mutator<SparcDisassembler> r(int pos)
        {
            return (wInstr, dasm) =>
            {
                var reg = Registers.GetRegister((wInstr >> pos) & 0x1F);
                dasm.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static Mutator<SparcDisassembler> r0 = r(0);
        private static Mutator<SparcDisassembler> r14 = r(14);
        private static Mutator<SparcDisassembler> r25 = r(25);

        private static Mutator<SparcDisassembler> r(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static Mutator<SparcDisassembler> rfsr = r(Registers.fsr);
        private static Mutator<SparcDisassembler> ry = r(Registers.y);

        // FPU register
        private static Mutator<SparcDisassembler> f(int pos)
        {
            return (u, d) =>
            {
                var freg = Registers.GetFpuRegister((int) (u >> pos) & 0x1F);
                d.ops.Add(new RegisterOperand(freg));
                return true;
            };
        }
        private static readonly Mutator<SparcDisassembler> f0 = f(0);
        private static readonly Mutator<SparcDisassembler> f14 = f(14);
        private static readonly Mutator<SparcDisassembler> f24 = f(24);
        private static readonly Mutator<SparcDisassembler> f25 = f(25);

        // double FPU register encoding
        private static Mutator<SparcDisassembler> d(int pos)
        {
            return (u, d) =>
            {
                var dreg = GetDoubleRegisterOperand(u, pos);
                if (dreg == null)
                    return false;
                d.ops.Add(dreg);
                return true;
            };
        }
        private static Mutator<SparcDisassembler> d0 = d(0);
        private static Mutator<SparcDisassembler> d14 = d(14);
        private static Mutator<SparcDisassembler> d25 = d(25);

        // quad FPU register encoding
        private static Mutator<SparcDisassembler> q(int pos)
        {
            return (u, d) =>
            {
                var qreg = GetQuadRegisterOperand(u, pos);
                if (qreg == null)
                    return false;
                d.ops.Add(qreg);
                return true;
            };
        }
        private static Mutator<SparcDisassembler> q0 = q(0);
        private static Mutator<SparcDisassembler> q14 = q(14);
        private static Mutator<SparcDisassembler> q25 = q(25);

        private static Mutator<SparcDisassembler> A(PrimitiveType size)
        {
            return (u, d) =>
            {
                d.ops.Add(GetAlternateSpaceOperand(u, size));
                return true;
            };
        }
        private static readonly Mutator<SparcDisassembler> Ab = A(PrimitiveType.Byte);
        private static readonly Mutator<SparcDisassembler> Ah = A(PrimitiveType.Word16);
        private static readonly Mutator<SparcDisassembler> Aw = A(PrimitiveType.Word32);
        private static readonly Mutator<SparcDisassembler> Ad = A(PrimitiveType.Word64);

        // 22-bit immediate value
        private static bool I(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetImmOperand(wInstr, 22));
            return true;
        }

        private static bool J(uint wInstr, SparcDisassembler dasm)
        {

            dasm.ops.Add(GetAddressOperand(dasm.imageReader.Address, wInstr));
            return true;
        }

        private static bool JJ(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(new AddressOperand((dasm.imageReader.Address - 4) + ((int) wInstr << 2)));
            return true;
        }

        private static Mutator<SparcDisassembler> M(PrimitiveType size)
        {
            return (u, d) =>
            {
                d.ops.Add(GetMemoryOperand(u, size));
                return true;
            };
        }
        private static Mutator<SparcDisassembler> Mb = M(PrimitiveType.Byte);
        private static Mutator<SparcDisassembler> Mh = M(PrimitiveType.Word16);
        private static Mutator<SparcDisassembler> Mw = M(PrimitiveType.Word32);
        private static Mutator<SparcDisassembler> Md = M(PrimitiveType.Word64);
        private static Mutator<SparcDisassembler> Msb = M(PrimitiveType.SByte);
        private static Mutator<SparcDisassembler> Msh = M(PrimitiveType.Int16);

        // Register or simm13.
        private static Mutator<SparcDisassembler> R(bool signed)
        {
            return (u, d) =>
            {
                // if 's', return a signed immediate operand where relevant.
                d.ops.Add(GetRegImmOperand(u, signed, 13));
                return true;
            };
        }
        private static Mutator<SparcDisassembler> R0 = R(false);
        private static Mutator<SparcDisassembler> Rs = R(true);

        // Register or uimm5
        private static bool S(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetRegImmOperand(wInstr, false, 6));
            return true;
        }

        // trap number
        private static bool T(uint wInstr, SparcDisassembler dasm)
        {
            dasm.ops.Add(GetRegImmOperand(wInstr, false, 7));
            return true;
        }
        
        #endregion

        private static int SignExtend(uint word, int bits)
        {
            int imm = (int) word & ((1 << bits) - 1);
            int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
            return imm | mask;
        }

        private static AddressOperand GetAddressOperand(Address addr, uint wInstr)
        {
            int offset = SignExtend(wInstr, 22) << 2;
            return new AddressOperand(addr + (offset - 4));
        }

        private static MachineOperand GetAlternateSpaceOperand(uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = Registers.GetRegister(wInstr >> 14);
            RegisterStorage idx = Registers.GetRegister(wInstr);
            var asi = (wInstr >> 4) & 0xFF;
            return new MemoryOperand(b, Constant.Int32((int) asi), type);
        }

        private static MachineOperand GetMemoryOperand(uint wInstr, PrimitiveType type)
        {
            RegisterStorage b = Registers.GetRegister(wInstr >> 14);
            if ((wInstr & (1 << 13)) != 0)
            {
                return new MemoryOperand(b, Constant.Int32(SignExtend(wInstr, 13)), type);
            }
            else
            {
                RegisterStorage idx = Registers.GetRegister(wInstr);
                return new IndexedMemoryOperand(b, idx, type);
            }
        }

        private static RegisterOperand GetDoubleRegisterOperand(uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            return new RegisterOperand(Registers.GetFpuRegister(reg));
        }

        private static RegisterOperand GetQuadRegisterOperand(uint wInstr, int offset)
        {
            int encodedReg = (int) (wInstr >> offset) & 0x1F;
            int reg = ((encodedReg & 1) << 5) | (encodedReg & ~1);
            if ((reg & 0x3) != 0)
                return null;
            return new RegisterOperand(Registers.GetFpuRegister(reg));
        }

        private static MachineOperand GetRegImmOperand(uint wInstr, bool signed, int bits)
        {
            if ((wInstr & (1 << 13)) != 0)
            {
                // Sign-extend the bastard.
                int imm = (int) wInstr & ((1 << bits) - 1);
                int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
                imm |= mask;
                return new ImmediateOperand(
                    signed
                        ? Constant.Int32(imm)
                        : Constant.Word32(imm));
            }
            else
            {
                return new RegisterOperand(Registers.GetRegister(wInstr & 0x1Fu));
            }
        }

        private static ImmediateOperand GetImmOperand(uint wInstr, int bits)
        {
            uint imm = wInstr & ((1u << bits) - 1);
            return new ImmediateOperand(Constant.Word32(imm));
        }

        private static Decoder[] decoders_0 = new Decoder[]
        {
            Instr(Mnemonic.unimp, InstrClass.Invalid),
            Instr(Mnemonic.illegal, InstrClass.Invalid),
            new BranchDecoder { offset = 0x00 },
            Instr(Mnemonic.illegal, InstrClass.Invalid),

            Instr(Mnemonic.sethi, I,r25),
            Instr(Mnemonic.illegal, InstrClass.Invalid),
            new BranchDecoder { offset = 0x10 },
            new BranchDecoder { offset = 0x20 },
        };

        private class BranchDecoder : Decoder
        {
            public uint offset;

            public override SparcInstruction Decode(uint wInstr, SparcDisassembler dasm)
            {
                uint i = ((wInstr >> 25) & 0xF) + offset;
                SparcInstruction instr = branchOps[i].Decode(wInstr, dasm);
                instr.InstructionClass |= ((wInstr & (1u << 29)) != 0) ? InstrClass.Annul : 0;
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic opcode, params Mutator<SparcDisassembler>[] mutators)
        {
            return new InstrDecoder { code = opcode, iclass = InstrClass.Linear, mutators = mutators };
        }

        private static InstrDecoder Instr(Mnemonic opcode, InstrClass iclass, params Mutator<SparcDisassembler>[] mutators)
        {
            return new InstrDecoder { code = opcode, iclass = iclass, mutators = mutators };
        }

        private static InstrDecoder invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

        static SparcDisassembler()
        {
            branchOps = new Decoder[]
            {
                // 00
                Instr(Mnemonic.bn, CondTransfer, J),
                Instr(Mnemonic.be, CondTransfer, J),
                Instr(Mnemonic.ble, CondTransfer, J),
                Instr(Mnemonic.bl, CondTransfer, J),
                Instr(Mnemonic.bleu, CondTransfer, J),
                Instr(Mnemonic.bcs, CondTransfer, J),
                Instr(Mnemonic.bneg, CondTransfer, J),
                Instr(Mnemonic.bvs, CondTransfer, J),

                Instr(Mnemonic.ba, CondTransfer, J),
                Instr(Mnemonic.bne, CondTransfer, J),
                Instr(Mnemonic.bg, CondTransfer, J),
                Instr(Mnemonic.bge, CondTransfer, J),
                Instr(Mnemonic.bgu, CondTransfer, J),
                Instr(Mnemonic.bcc, CondTransfer, J),
                Instr(Mnemonic.bpos, CondTransfer, J),
                Instr(Mnemonic.bvc, CondTransfer, J),

                // 10
                Instr(Mnemonic.fbn, CondTransfer, J),
                Instr(Mnemonic.fbne, CondTransfer, J),
                Instr(Mnemonic.fblg, CondTransfer, J),
                Instr(Mnemonic.fbul, CondTransfer, J),
                Instr(Mnemonic.fbug, CondTransfer, J),
                Instr(Mnemonic.fbg, CondTransfer, J),
                Instr(Mnemonic.fbu, CondTransfer, J),
                Instr(Mnemonic.fbug, CondTransfer, J),

                Instr(Mnemonic.fba, CondTransfer, J),
                Instr(Mnemonic.fbe, CondTransfer, J),
                Instr(Mnemonic.fbue, CondTransfer, J),
                Instr(Mnemonic.fbge, CondTransfer, J),
                Instr(Mnemonic.fbuge, CondTransfer, J),
                Instr(Mnemonic.fble, CondTransfer, J),
                Instr(Mnemonic.fbule, CondTransfer, J),
                Instr(Mnemonic.fbo, CondTransfer, J),

                // 20
                Instr(Mnemonic.cbn, J),
                Instr(Mnemonic.cb123, J),
                Instr(Mnemonic.cb12, J),
                Instr(Mnemonic.cb13, J),
                Instr(Mnemonic.cb1, J),
                Instr(Mnemonic.cb23, J),
                Instr(Mnemonic.cb2, J),
                Instr(Mnemonic.cb3, J),

                Instr(Mnemonic.cba, J),
                Instr(Mnemonic.cb0, J),
                Instr(Mnemonic.cb03, J),
                Instr(Mnemonic.cb02, J),
                Instr(Mnemonic.cb023, J),
                Instr(Mnemonic.cb01, J),
                Instr(Mnemonic.cb013, J),
                Instr(Mnemonic.cb012, J),

                // 30
                Instr(Mnemonic.tn, r14,T),
                Instr(Mnemonic.te, r14,T),
                Instr(Mnemonic.tle, r14,T),
                Instr(Mnemonic.tl, r14,T),
                Instr(Mnemonic.tleu, r14,T),
                Instr(Mnemonic.tcs, r14,T),
                Instr(Mnemonic.tneg, r14,T),
                Instr(Mnemonic.tvs, r14,T),

                Instr(Mnemonic.ta, r14,T),
                Instr(Mnemonic.tne, r14,T),
                Instr(Mnemonic.tg, r14,T),
                Instr(Mnemonic.tge, r14,T),
                Instr(Mnemonic.tgu, r14,T),
                Instr(Mnemonic.tcc, r14,T),
                Instr(Mnemonic.tpos, r14,T),
                Instr(Mnemonic.tvc, r14,T),
            };

            var fpDecoders = new (uint, Decoder)[]
            {
                // 00 
                (0x01, Instr(Mnemonic.fmovs, f0, f25)),
                (0x05, Instr(Mnemonic.fnegs, f0, f25)),
                (0x09, Instr(Mnemonic.fabss, f0, f25)),
                (0x29, Instr(Mnemonic.fsqrts, f0, f25)),
                (0x2A, Instr(Mnemonic.fsqrtd, d0, d25)),
                (0x2B, Instr(Mnemonic.fsqrtq, q0, q25)),

                (0x41, Instr(Mnemonic.fadds, f14, f0, f25)),
                (0x42, Instr(Mnemonic.faddd, d14, d0, d25)),
                (0x43, Instr(Mnemonic.faddq, q14, q0, q25)),
                (0x45, Instr(Mnemonic.fsubs, f14, f0, f25)),
                (0x46, Instr(Mnemonic.fsubd, d14, d0, d25)),
                (0x47, Instr(Mnemonic.fsubq, q14, q0, q25)),

                (0xC4, Instr(Mnemonic.fitos, f0, f25)),
                (0xC6, Instr(Mnemonic.fdtos, d0, f25)),
                (0xC7, Instr(Mnemonic.fqtos, q0, f25)),
                (0xC8, Instr(Mnemonic.fitod, f0, d25)),
                (0xC9, Instr(Mnemonic.fstod, f0, d25)),
                (0xCB, Instr(Mnemonic.fqtod, q0, d25)),
                (0xCC, Instr(Mnemonic.fitoq, f0, q25)),
                (0xCD, Instr(Mnemonic.fstoq, f0, q25)),
                (0xCE, Instr(Mnemonic.fdtoq, d0, q25)),
                (0xD1, Instr(Mnemonic.fstoi, f0, f25)),
                (0xD2, Instr(Mnemonic.fdtoi, d0, f25)),
                (0xD3, Instr(Mnemonic.fqtoi, q0, f25)),

                (0x49, Instr(Mnemonic.fmuls, f14, f0, f25)),
                (0x4A, Instr(Mnemonic.fmuld, d14, d0, d25)),
                (0x4B, Instr(Mnemonic.fmulq, q14, q0, q25)),
                (0x4D, Instr(Mnemonic.fdivs, f14, f0, f25)),
                (0x4E, Instr(Mnemonic.fdivd, d14, d0, d25)),
                (0x4F, Instr(Mnemonic.fdivq, q14, q0, q25)),

                (0x69, Instr(Mnemonic.fsmuld, f14, f0, d25)),
                (0x6E, Instr(Mnemonic.fdmulq, d14, d0, q25)),

                (0x51, Instr(Mnemonic.fcmps, f14, f0)),
                (0x52, Instr(Mnemonic.fcmpd, d14, d0)),
                (0x53, Instr(Mnemonic.fcmpq, f14, f0)),
                (0x55, Instr(Mnemonic.fcmpes, f14, f0)),
                (0x56, Instr(Mnemonic.fcmped, d14, d0)),
                (0x57, Instr(Mnemonic.fcmpeq, q14, q0))
            };

            var decoders_2 = new Decoder[]
            {
                // 00
                Instr(Mnemonic.add, r14,R0,r25),
                Instr(Mnemonic.and, r14,R0,r25),
                Instr(Mnemonic.or, r14,R0,r25),
                Instr(Mnemonic.xor, r14,R0,r25),
                Instr(Mnemonic.sub, r14,R0,r25),
                Instr(Mnemonic.andn, r14,R0,r25),
                Instr(Mnemonic.orn, r14,R0,r25),
                Instr(Mnemonic.xnor, r14,R0,r25),

                Instr(Mnemonic.addx, r14,R0,r25),
                invalid,
                Instr(Mnemonic.umul, r14,R0,r25),
                Instr(Mnemonic.smul, r14,R0,r25),
                Instr(Mnemonic.subx, r14,R0,r25),
                invalid,
                Instr(Mnemonic.udiv, r14,R0,r25),
                Instr(Mnemonic.sdiv, r14,R0,r25),

                // 10
                Instr(Mnemonic.addcc, r14,R0,r25),
                Instr(Mnemonic.andcc, r14,R0,r25),
                Instr(Mnemonic.orcc, r14,R0,r25),
                Instr(Mnemonic.xorcc, r14,R0,r25),
                Instr(Mnemonic.subcc, r14,R0,r25),
                Instr(Mnemonic.andncc, r14,R0,r25),
                Instr(Mnemonic.orncc, r14,R0,r25),
                Instr(Mnemonic.xnorcc, r14,R0,r25),

                Instr(Mnemonic.addxcc, r14,R0,r25),
                invalid,
                Instr(Mnemonic.umulcc, r14,R0,r25),
                Instr(Mnemonic.smulcc, r14,R0,r25),
                Instr(Mnemonic.subxcc, r14,R0,r25),
                invalid,
                Instr(Mnemonic.udivcc, r14,R0,r25),
                Instr(Mnemonic.sdivcc, r14,R0,r25),

                // 20
                Instr(Mnemonic.taddcc, r14,R0,r25),
                Instr(Mnemonic.tsubcc, r14,R0,r25),
                Instr(Mnemonic.taddcctv, r14,R0,r25),
                Instr(Mnemonic.tsubcctv, r14,R0,r25),
                Instr(Mnemonic.mulscc, r14,R0,r25),
                Instr(Mnemonic.sll, r14,S,r25),
                Instr(Mnemonic.srl, r14,S,r25),
                Instr(Mnemonic.sra, r14,S,r25),

                Instr(Mnemonic.rd, ry,r25),
                new InstrDecoder { code=Mnemonic.rdpsr, },
                new InstrDecoder { code=Mnemonic.rdtbr, },
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // 30
                new InstrDecoder { code=Mnemonic.wrasr, },
                new InstrDecoder { code=Mnemonic.wrpsr, },
                new InstrDecoder { code=Mnemonic.wrwim, },
                new InstrDecoder { code=Mnemonic.wrtbr, },
                Sparse(5, 9, "  FOp1", invalid, fpDecoders),
                Sparse(5, 9, "  FOp2", invalid, fpDecoders),
                Sparse(4, 9, "  CPop1", invalid, fpDecoders),
                Sparse(4, 9, "  CPop2", invalid, fpDecoders),

                Instr(Mnemonic.jmpl, r14,Rs,r25),
                Instr(Mnemonic.rett, r14,Rs ),
                new BranchDecoder { offset= 0x30, },
                Instr(Mnemonic.flush),
                Instr(Mnemonic.save, r14,R0,r25),
                Instr(Mnemonic.restore, r14,R0,r25),
                invalid,
                invalid,
            };

            var decoders_3 = new InstrDecoder[]
            {
                // 00
                Instr(Mnemonic.ld, Mw,r25),
                Instr(Mnemonic.ldub, Mb,r25),
                Instr(Mnemonic.lduh, Mh,r25),
                Instr(Mnemonic.ldd, Md,r25),
                Instr(Mnemonic.st, r25,Mw),
                Instr(Mnemonic.stb, r25,Mb),
                Instr(Mnemonic.sth, r25,Mh),
                Instr(Mnemonic.std, r25,Md),

                invalid,
                Instr(Mnemonic.ldsb, Msb,r25),
                Instr(Mnemonic.ldsh, Msh,r25),
                invalid,
                invalid,
                new InstrDecoder { code=Mnemonic.ldstub,  iclass=InstrClass.Invalid },
                invalid,
                Instr(Mnemonic.swap, Mw,r25),

                // 10
                Instr(Mnemonic.lda, Aw,r25),
                Instr(Mnemonic.lduba, Ab,r25),
                Instr(Mnemonic.lduha, Ah,r25),
                Instr(Mnemonic.ldda, Ad,r25),
                Instr(Mnemonic.sta, r25,Aw),
                Instr(Mnemonic.stba, r25,Ab),
                Instr(Mnemonic.stha, r25,Ah),
                Instr(Mnemonic.stda, r25,Ad),

                invalid,
                Instr(Mnemonic.ldsba, r25,Ab),
                Instr(Mnemonic.ldsha, r25,Ah),
                invalid,
                invalid,
                Instr(Mnemonic.ldstuba, Ab,r25),
                invalid,
                Instr(Mnemonic.swapa, Aw,r25),

                // 20
                Instr(Mnemonic.ldf,   Mw,f24),
                Instr(Mnemonic.ldfsr, Mw,rfsr),
                invalid,
                Instr(Mnemonic.lddf, Md,f24),
                Instr(Mnemonic.stf, f24,Mw),
                Instr(Mnemonic.stfsr, rfsr,Mw),
                Instr(Mnemonic.stdfq),
                Instr(Mnemonic.stdf, f24,Md),

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // 30
                new InstrDecoder { code=Mnemonic.ldc, },
                new InstrDecoder { code=Mnemonic.ldcsr, },
                invalid,
                new InstrDecoder { code=Mnemonic.lddc, },
                new InstrDecoder { code=Mnemonic.stc, },
                new InstrDecoder { code=Mnemonic.stcsr, },
                new InstrDecoder { code=Mnemonic.stdcq, },
                new InstrDecoder { code=Mnemonic.stdc, },

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
            };


            // Format 1 (op == 1)
            // +----+-------------------------------------------------------------+
            // | op | disp30                                                      |
            // +----+-------------------------------------------------------------+
            //
            // Format 2 (op == 0). SETHI and branches Bicc, FBcc, CBcc
            // +----+---+------+-----+--------------------------------------------+
            // | op | rd       | op2 | imm22                                      |
            // +----+---+------+-----+--------------------------------------------+
            // | op | a | cond | op2 | disp22                                     |
            // +----+---+------+-----+--------------------------------------------+
            // 31   29  28     24    21
            //
            // Format 3 (op = 2, 3)
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 | i=0 |        asi         |  rs2  |
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 | i=1 |        simm13              |
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 |           opf            |  rs2  |
            // +----+----------+--------+------+--------------------------+-------+
            // 31   29         24       18     13    12                   4

            rootDecoder = Mask(30, 2, "SPARC",
                Mask(22, 3, "  Format 0", decoders_0),
                Instr(Mnemonic.call, LinkTransfer, JJ),
                Mask(19, 6, "  Format 2", decoders_2),
                Mask(19, 6, "  Format 3", decoders_3));
        }
    }
}