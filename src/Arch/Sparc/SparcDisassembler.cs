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
    public class SparcDisassembler : DisassemblerBase<SparcInstruction>
    {
        private const InstrClass Transfer = InstrClass.Delay | InstrClass.Transfer;
        private const InstrClass CondTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Conditional;
        private const InstrClass LinkTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Call;

        private SparcInstruction instrCur;
        private EndianImageReader imageReader;
        private readonly List<MachineOperand> ops;

        public SparcDisassembler(SparcArchitecture arch, EndianImageReader imageReader)
        {
            this.imageReader = imageReader;
            this.ops = new List<MachineOperand>();
        }


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
        public override SparcInstruction DisassembleInstruction()
        {
            if (!imageReader.IsValid)
                return null;
            ops.Clear();
            var addr = imageReader.Address;
            uint wInstr = imageReader.ReadBeUInt32();
            switch (wInstr >> 30)
            {
            default: Debug.Assert(false, "Impossible!"); break;

            case 0:
                instrCur = decoders_0[(wInstr >> 22) & 7].Decode(this, wInstr);
                break;
            case 1:
                instrCur = new SparcInstruction
                {
                    Opcode = Opcode.call,
                    InstructionClass = LinkTransfer,
                    Op1 = new AddressOperand((imageReader.Address - 4) + ((int) wInstr << 2)),
                };
                break;
            case 2:
                instrCur = decoders_2[(wInstr >> 19) & 0x3F].Decode(this, wInstr);
                break;
            case 3:
                instrCur = decoders_3[(wInstr >> 19) & 0x3F].Decode(this, wInstr);
                break;
            }
            instrCur.Address = addr;
            instrCur.Length = 4;
            instrCur.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            return instrCur;
        }

        private class Decoder
        {
            public Opcode code;
            public InstrClass iclass = InstrClass.Linear;
            public Mutator<SparcDisassembler>[] mutators;

            public virtual SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return invalid.Decode(dasm, wInstr);
                }
                return new SparcInstruction
                {
                    Opcode = code,
                    InstructionClass = iclass,
                    Op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    Op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    Op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
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
            Instr(Opcode.unimp, InstrClass.Invalid),
            Instr(Opcode.illegal, InstrClass.Invalid),
            new BranchDecoder { offset = 0x00 },
            Instr(Opcode.illegal, InstrClass.Invalid),
            Instr(Opcode.sethi, I,r25),
            Instr(Opcode.illegal, InstrClass.Invalid),
            new BranchDecoder { offset = 0x10 },
            new BranchDecoder { offset = 0x20 },
        };

        private class BranchDecoder : Decoder
        {
            public uint offset;

            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                uint i = ((wInstr >> 25) & 0xF) + offset;
                SparcInstruction instr = branchOps[i].Decode(dasm, wInstr);
                instr.InstructionClass |= ((wInstr & (1u << 29)) != 0) ? InstrClass.Annul : 0;
                return instr;
            }
        }

        private static Decoder Instr(Opcode opcode, params Mutator<SparcDisassembler>[] mutators)
        {
            return new Decoder { code = opcode, iclass = InstrClass.Linear, mutators = mutators };
        }

        private static Decoder Instr(Opcode opcode, InstrClass iclass, params Mutator<SparcDisassembler>[] mutators)
        {
            return new Decoder { code = opcode, iclass = iclass, mutators = mutators };
        }

        private static Decoder invalid = Instr(Opcode.illegal, InstrClass.Invalid);

        private static Decoder[] branchOps = new Decoder[]
        {
            // 00
            Instr(Opcode.bn, CondTransfer, J),
            Instr(Opcode.be, CondTransfer, J),
            Instr(Opcode.ble, CondTransfer, J),
            Instr(Opcode.bl, CondTransfer, J),
            Instr(Opcode.bleu, CondTransfer, J),
            Instr(Opcode.bcs, CondTransfer, J),
            Instr(Opcode.bneg, CondTransfer, J),
            Instr(Opcode.bvs, CondTransfer, J),

            Instr(Opcode.ba, CondTransfer, J),
            Instr(Opcode.bne, CondTransfer, J),
            Instr(Opcode.bg, CondTransfer, J),
            Instr(Opcode.bge, CondTransfer, J),
            Instr(Opcode.bgu, CondTransfer, J),
            Instr(Opcode.bcc, CondTransfer, J),
            Instr(Opcode.bpos, CondTransfer, J),
            Instr(Opcode.bvc, CondTransfer, J),

            // 10
            Instr(Opcode.fbn, CondTransfer, J),
            Instr(Opcode.fbne, CondTransfer, J),
            Instr(Opcode.fblg, CondTransfer, J),
            Instr(Opcode.fbul, CondTransfer, J),
            Instr(Opcode.fbug, CondTransfer, J),
            Instr(Opcode.fbg, CondTransfer, J),
            Instr(Opcode.fbu, CondTransfer, J),
            Instr(Opcode.fbug, CondTransfer, J),

            Instr(Opcode.fba, CondTransfer, J),
            Instr(Opcode.fbe, CondTransfer, J),
            Instr(Opcode.fbue, CondTransfer, J),
            Instr(Opcode.fbge, CondTransfer, J),
            Instr(Opcode.fbuge, CondTransfer, J),
            Instr(Opcode.fble, CondTransfer, J),
            Instr(Opcode.fbule, CondTransfer, J),
            Instr(Opcode.fbo, CondTransfer, J),

            // 20
            Instr(Opcode.cbn, J),
            Instr(Opcode.cb123, J),
            Instr(Opcode.cb12, J),
            Instr(Opcode.cb13, J),
            Instr(Opcode.cb1, J),
            Instr(Opcode.cb23, J),
            Instr(Opcode.cb2, J),
            Instr(Opcode.cb3, J),

            Instr(Opcode.cba, J),
            Instr(Opcode.cb0, J),
            Instr(Opcode.cb03, J),
            Instr(Opcode.cb02, J),
            Instr(Opcode.cb023, J),
            Instr(Opcode.cb01, J),
            Instr(Opcode.cb013, J),
            Instr(Opcode.cb012, J),

            // 30
            Instr(Opcode.tn, r14,T),
            Instr(Opcode.te, r14,T),
            Instr(Opcode.tle, r14,T),
            Instr(Opcode.tl, r14,T),
            Instr(Opcode.tleu, r14,T),
            Instr(Opcode.tcs, r14,T),
            Instr(Opcode.tneg, r14,T),
            Instr(Opcode.tvs, r14,T),

            Instr(Opcode.ta, r14,T),
            Instr(Opcode.tne, r14,T),
            Instr(Opcode.tg, r14,T),
            Instr(Opcode.tge, r14,T),
            Instr(Opcode.tgu, r14,T),
            Instr(Opcode.tcc, r14,T),
            Instr(Opcode.tpos, r14,T),
            Instr(Opcode.tvc, r14,T),
        };

        private static Decoder[] decoders_2 = new Decoder[]
        {
            // 00
            Instr(Opcode.add, r14,R0,r25),
            Instr(Opcode.and, r14,R0,r25),
            Instr(Opcode.or, r14,R0,r25),
            Instr(Opcode.xor, r14,R0,r25),
            Instr(Opcode.sub, r14,R0,r25),
            Instr(Opcode.andn, r14,R0,r25),
            Instr(Opcode.orn, r14,R0,r25),
            Instr(Opcode.xnor, r14,R0,r25),

            Instr(Opcode.addx, r14,R0,r25),
            invalid,
            Instr(Opcode.umul, r14,R0,r25),
            Instr(Opcode.smul, r14,R0,r25),
            Instr(Opcode.subx, r14,R0,r25),
            invalid,
            Instr(Opcode.udiv, r14,R0,r25),
            Instr(Opcode.sdiv, r14,R0,r25),

            // 10
            Instr(Opcode.addcc, r14,R0,r25),
            Instr(Opcode.andcc, r14,R0,r25),
            Instr(Opcode.orcc, r14,R0,r25),
            Instr(Opcode.xorcc, r14,R0,r25),
            Instr(Opcode.subcc, r14,R0,r25),
            Instr(Opcode.andncc, r14,R0,r25),
            Instr(Opcode.orncc, r14,R0,r25),
            Instr(Opcode.xnorcc, r14,R0,r25),

            Instr(Opcode.addxcc, r14,R0,r25),
            invalid,
            Instr(Opcode.umulcc, r14,R0,r25),
            Instr(Opcode.smulcc, r14,R0,r25),
            Instr(Opcode.subxcc, r14,R0,r25),
            invalid,
            Instr(Opcode.udivcc, r14,R0,r25),
            Instr(Opcode.sdivcc, r14,R0,r25),

            // 20
            Instr(Opcode.taddcc, r14,R0,r25),
            Instr(Opcode.tsubcc, r14,R0,r25),
            Instr(Opcode.taddcctv, r14,R0,r25),
            Instr(Opcode.tsubcctv, r14,R0,r25),
            Instr(Opcode.mulscc, r14,R0,r25),
            Instr(Opcode.sll, r14,S,r25),
            Instr(Opcode.srl, r14,S,r25),
            Instr(Opcode.sra, r14,S,r25),

            Instr(Opcode.rd, ry,r25),
            new Decoder { code=Opcode.rdpsr, },
            new Decoder { code=Opcode.rdtbr, },
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 30
            new Decoder { code=Opcode.wrasr, },
            new Decoder { code=Opcode.wrpsr, },
            new Decoder { code=Opcode.wrwim, },
            new Decoder { code=Opcode.wrtbr, },
            new FPop1Decoder { },
            new FPop2Decoder { },
            new CPop1 {  },
            new CPop2 {  },

            Instr(Opcode.jmpl, r14,Rs,r25),
            Instr(Opcode.rett, r14,Rs ),
            new BranchDecoder { offset= 0x30, },
            Instr(Opcode.flush),
            Instr(Opcode.save, r14,R0,r25),
            Instr(Opcode.restore, r14,R0,r25),
            invalid,
            invalid,
        };

        private static Decoder[] decoders_3 = new Decoder[]
        {
            // 00
            Instr(Opcode.ld, Mw,r25),
            Instr(Opcode.ldub, Mb,r25),
            Instr(Opcode.lduh, Mh,r25),
            Instr(Opcode.ldd, Md,r25),
            Instr(Opcode.st, r25,Mw),
            Instr(Opcode.stb, r25,Mb),
            Instr(Opcode.sth, r25,Mh),
            Instr(Opcode.std, r25,Md),

            invalid,
            Instr(Opcode.ldsb, Msb,r25),
            Instr(Opcode.ldsh, Msh,r25),
            invalid,
            invalid,
            new Decoder { code=Opcode.ldstub,  iclass=InstrClass.Invalid },
            invalid,
            Instr(Opcode.swap, Mw,r25),

            // 10
            Instr(Opcode.lda, Aw,r25),
            Instr(Opcode.lduba, Ab,r25),
            Instr(Opcode.lduha, Ah,r25),
            Instr(Opcode.ldda, Ad,r25),
            Instr(Opcode.sta, r25,Aw),
            Instr(Opcode.stba, r25,Ab),
            Instr(Opcode.stha, r25,Ah),
            Instr(Opcode.stda, r25,Ad),

            invalid,
            Instr(Opcode.ldsba, r25,Ab),
            Instr(Opcode.ldsha, r25,Ah),
            invalid,
            invalid,
            Instr(Opcode.ldstuba, Ab,r25),
            invalid,
            Instr(Opcode.swapa, Aw,r25),

            // 20
            Instr(Opcode.ldf,   Mw,f24),
            Instr(Opcode.ldfsr, Mw,rfsr),
            invalid,
            Instr(Opcode.lddf, Md,f24),
            Instr(Opcode.stf, f24,Mw),
            Instr(Opcode.stfsr, rfsr,Mw),
            Instr(Opcode.stdfq),
            Instr(Opcode.stdf, f24,Md),

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 30
            new Decoder { code=Opcode.ldc, },
            new Decoder { code=Opcode.ldcsr, },
            invalid,
            new Decoder { code=Opcode.lddc, },
            new Decoder { code=Opcode.stc, },
            new Decoder { code=Opcode.stcsr, },
            new Decoder { code=Opcode.stdcq, },
            new Decoder { code=Opcode.stdc, },

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
        };

        private class FPop1Decoder : Decoder
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpDecoders[(wInstr >> 5) & 0x1FF].Decode(dasm, wInstr);
            }
        }

        private class FPop2Decoder : Decoder
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpDecoders[(wInstr >> 5) & 0x1FF].Decode(dasm, wInstr);
            }
        }

        private class CPop1 : Decoder
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpDecoders[(wInstr >> 4) & 0x1FF].Decode(dasm, wInstr);
            }
        }
        private class CPop2 : Decoder
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpDecoders[(wInstr >> 4) & 0xFF].Decode(dasm, wInstr);
            }
        }

        private static Dictionary<uint, Decoder> fpDecoders = new Dictionary<uint, Decoder>
        {
            // 00 
            { 0x01, Instr(Opcode.fmovs, f0,f25) },
            { 0x05, Instr(Opcode.fnegs, f0,f25) },
            { 0x09, Instr(Opcode.fabss, f0,f25) },
            { 0x29, Instr(Opcode.fsqrts, f0,f25) },
            { 0x2A, Instr(Opcode.fsqrtd, d0,d25) },
            { 0x2B, Instr(Opcode.fsqrtq, q0,q25) },

            { 0x41, Instr(Opcode.fadds, f14,f0,f25) },
            { 0x42, Instr(Opcode.faddd, d14,d0,d25) },
            { 0x43, Instr(Opcode.faddq, q14,q0,q25) },
            { 0x45, Instr(Opcode.fsubs, f14,f0,f25) },
            { 0x46, Instr(Opcode.fsubd, d14,d0,d25) },
            { 0x47, Instr(Opcode.fsubq, q14,q0,q25) },

            { 0xC4, Instr(Opcode.fitos, f0,f25) },
            { 0xC6, Instr(Opcode.fdtos, d0,f25) },
            { 0xC7, Instr(Opcode.fqtos, q0,f25) },
            { 0xC8, Instr(Opcode.fitod, f0,d25) },
            { 0xC9, Instr(Opcode.fstod, f0,d25) },
            { 0xCB, Instr(Opcode.fqtod, q0,d25) },
            { 0xCC, Instr(Opcode.fitoq, f0,q25) },
            { 0xCD, Instr(Opcode.fstoq, f0,q25) },
            { 0xCE, Instr(Opcode.fdtoq, d0,q25) },
            { 0xD1, Instr(Opcode.fstoi, f0,f25) },
            { 0xD2, Instr(Opcode.fdtoi, d0,f25) },
            { 0xD3, Instr(Opcode.fqtoi, q0,f25) },

            { 0x49, Instr(Opcode.fmuls, f14,f0,f25) },
            { 0x4A, Instr(Opcode.fmuld, d14,d0,d25) },
            { 0x4B, Instr(Opcode.fmulq, q14,q0,q25) },
            { 0x4D, Instr(Opcode.fdivs, f14,f0,f25) },
            { 0x4E, Instr(Opcode.fdivd, d14,d0,d25) },
            { 0x4F, Instr(Opcode.fdivq, q14,q0,q25) },

            { 0x69, Instr(Opcode.fsmuld, f14,f0,d25) },
            { 0x6E, Instr(Opcode.fdmulq, d14,d0,q25) },

            { 0x51, Instr(Opcode.fcmps, f14,f0) },
            { 0x52, Instr(Opcode.fcmpd, d14,d0) },
            { 0x53, Instr(Opcode.fcmpq, f14,f0) },
            { 0x55, Instr(Opcode.fcmpes, f14,f0) },
            { 0x56, Instr(Opcode.fcmped, d14,d0) },
            { 0x57, Instr(Opcode.fcmpeq, q14,q0) },
        };
    }
}