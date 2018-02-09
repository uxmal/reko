#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        private SparcInstruction instrCur;
        private EndianImageReader imageReader;

        public SparcDisassembler(SparcArchitecture arch, EndianImageReader imageReader)
        {
            this.imageReader = imageReader;
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
            var addr = imageReader.Address;
            uint wInstr = imageReader.ReadBeUInt32();
            switch (wInstr >> 30)
            {
            default: Debug.Assert(false, "Impossible!"); break;

            case 0:
                instrCur = opRecs_0[(wInstr >> 22) & 7].Decode(this, wInstr);
                break;
            case 1:
                instrCur = new SparcInstruction
                {
                    Opcode = Opcode.call,
                    Op1 = new AddressOperand((imageReader.Address - 4) + ((int)wInstr << 2)),
                };
                break;
            case 2:
                instrCur = opRecs_2[(wInstr >> 19) & 0x3F].Decode(this, wInstr);
                break;
            case 3:
                instrCur = opRecs_3[(wInstr >> 19) & 0x3F].Decode(this, wInstr);
                break;
            }
            instrCur.Address = addr;
            instrCur.Length = 4;
            return instrCur;
        }

        private class OpRec
        {
            public Opcode code;
            public string fmt;

            public virtual SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                if (fmt == null)
                    return new SparcInstruction { Opcode = code };
                var ops = new List<MachineOperand>();
                int i = 0;
                while (i != fmt.Length)
                {
                    char c = fmt[i++];
                    if (c == ',')
                        continue;
                    switch (c)
                    {
                    default: throw new NotSupportedException(string.Format("Format {0} not supported.", c));
                    case 'r':       // Register reference
                        ops.Add(GetRegisterOperand(wInstr, ref i));
                        break;
                    case 'f':       // FPU register
                        ops.Add(GetFpuRegisterOperand(wInstr, ref i));
                        break;
                    case 'A':
                        ops.Add(GetAlternateSpaceOperand(wInstr, GetOperandSize(ref i)));
                        break;
                    case 'I':       // 22-bit immediate value
                        ops.Add(GetImmOperand(wInstr, 22));
                        break;
                    case 'J':
                        ops.Add(GetAddressOperand(dasm.imageReader.Address, wInstr));
                        break;
                    case 'M':
                        ops.Add(GetMemoryOperand(wInstr, GetOperandSize(ref i)));
                        break;
                    case 'R':       // Register or simm13.
                                    // if 's', return a signed immediate operand where relevant.
                        ops.Add(GetRegImmOperand(wInstr, fmt[i++] == 's', 13));
                        break;
                    case 'S':       // Register or uimm5
                        ops.Add(GetRegImmOperand(wInstr, false, 6));
                        break;
                    case 'T':       // trap number
                        ops.Add(GetRegImmOperand(wInstr, false, 7));
                        break;
                    }
                }
                return new SparcInstruction
                {
                    Opcode = code,
                    Op1 = ops.Count > 0 ? ops[0] : null,
                    Op2 = ops.Count > 1 ? ops[1] : null,
                    Op3 = ops.Count > 2 ? ops[2] : null,
                };
            }

            private PrimitiveType GetOperandSize(ref int i)
            {
                int size = fmt[i++];
                bool signed = (i < fmt.Length -1 && fmt[i] == '+');
                if (signed)
                    ++i;
                {
                    signed = true;
                }
                switch (size)
                {
                case 'b': return signed ? PrimitiveType.SByte : PrimitiveType.Byte;
                case 'h': return signed ? PrimitiveType.Int16 : PrimitiveType.UInt16;
                case 'w': return signed ? PrimitiveType.Int32 : PrimitiveType.UInt32;
                case 'd': return signed ? PrimitiveType.Int64 : PrimitiveType.UInt64;
                }
                throw new NotImplementedException(string.Format("Unknown format character {0}.", fmt[i-1]));
            }

            private int SignExtend(uint word, int bits)
            {
                int imm = (int) word & ((1 << bits) - 1);
                int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
                return imm | mask;
            }

            private AddressOperand GetAddressOperand(Address addr, uint wInstr)
            {
                int offset = SignExtend(wInstr, 22) << 2;
                return new AddressOperand(addr + (offset - 4));
            }

            private MachineOperand GetAlternateSpaceOperand(uint wInstr, PrimitiveType type)
            {
                RegisterStorage b = Registers.GetRegister(wInstr >> 14);
                RegisterStorage idx = Registers.GetRegister(wInstr);
                var asi = (wInstr >> 4) & 0xFF;
                return new MemoryOperand(b, Constant.Int32((int)asi), type);
            }

            private MachineOperand GetMemoryOperand(uint wInstr, PrimitiveType type)
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

            private RegisterOperand GetRegisterOperand(uint wInstr, ref int i)
            {
                return new RegisterOperand(GetRegister(wInstr, ref i));
            }

            private RegisterOperand GetFpuRegisterOperand(uint wInstr, ref int i)
            {
                // Register operand are followed by their bit offset within the instruction,
                // expressed as decimal digits.
                int offset = 0;
                while (i < fmt.Length && Char.IsDigit(fmt[i]))
                {
                    offset = offset * 10 + (fmt[i++] - '0');
                }
                return new RegisterOperand(Registers.GetFpuRegister((wInstr >> offset) & 0x1F));
            }

            private RegisterStorage GetRegister(uint wInstr, ref int i)
            {
                if (fmt[i] == 'y')
                {
                    ++i;
                    return Registers.y;
                }

                // Register operand are followed by their bit offset within the instruction,
                // expressed as decimal digits.
                int offset = 0;
                while (i < fmt.Length && Char.IsDigit(fmt[i]))
                {
                    offset = offset * 10 + (fmt[i++] - '0');
                }
                return Registers.GetRegister((wInstr >> offset) & 0x1F);
            }

            private MachineOperand GetRegImmOperand(uint wInstr, bool signed, int bits)
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

            private ImmediateOperand GetImmOperand(uint wInstr, int bits)
            {
                uint imm = wInstr & ((1u << bits) - 1);
                return new ImmediateOperand(Constant.Word32(imm));
            }
        }

        private OpRec[] opRecs_0 = new OpRec[]
        {
            new OpRec { code=Opcode.unimp, },
            new OpRec { code=Opcode.illegal, },
            new BrachOpRec { offset = 0x00 },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.sethi, fmt="I,r25" }, 
            new OpRec { code=Opcode.illegal, },
            new BrachOpRec { offset = 0x10 },
            new BrachOpRec { offset = 0x20 },
        };

        private class BrachOpRec : OpRec
        {
            public uint offset;

            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                uint i = ((wInstr >> 25) & 0xF) + offset;
                SparcInstruction instr = branchOps[i].Decode(dasm, wInstr);
                if ((wInstr & (1u << 29)) != 0)
                    instr.Annul = true;
                return instr;
            }
        }

        private static OpRec[] branchOps = new OpRec[] 
        {
            // 00
            new OpRec { code=Opcode.bn, fmt="J"},
            new OpRec { code=Opcode.be, fmt="J"},
            new OpRec { code=Opcode.ble,fmt="J" },
            new OpRec { code=Opcode.bl, fmt="J" },
            new OpRec { code=Opcode.bleu, fmt="J" },
            new OpRec { code=Opcode.bcs, fmt="J" },
            new OpRec { code=Opcode.bneg, fmt="J" },
            new OpRec { code=Opcode.bvs, fmt="J" },

            new OpRec { code=Opcode.ba, fmt="J" },
            new OpRec { code=Opcode.bne, fmt="J" },
            new OpRec { code=Opcode.bg, fmt="J" },
            new OpRec { code=Opcode.bge, fmt="J" },
            new OpRec { code=Opcode.bgu, fmt="J" },
            new OpRec { code=Opcode.bcc, fmt="J" },
            new OpRec { code=Opcode.bpos, fmt="J" },
            new OpRec { code=Opcode.bvc, fmt="J" },

            // 10
            new OpRec { code=Opcode.fbn, fmt="J" },
            new OpRec { code=Opcode.fbne, fmt="J" },
            new OpRec { code=Opcode.fblg, fmt="J" },
            new OpRec { code=Opcode.fbul, fmt="J" },
            new OpRec { code=Opcode.fbug, fmt="J" },
            new OpRec { code=Opcode.fbg, fmt="J" }, 
            new OpRec { code=Opcode.fbu, fmt="J" },
            new OpRec { code=Opcode.fbug, fmt="J" },

            new OpRec { code=Opcode.fba, fmt="J" },
            new OpRec { code=Opcode.fbe, fmt="J" },
            new OpRec { code=Opcode.fbue, fmt="J" },
            new OpRec { code=Opcode.fbge, fmt="J" },
            new OpRec { code=Opcode.fbuge, fmt="J" },
            new OpRec { code=Opcode.fble, fmt="J" },
            new OpRec { code=Opcode.fbule, fmt="J" },
            new OpRec { code=Opcode.fbo, fmt="J" },

            // 20
            new OpRec { code=Opcode.cbn, fmt="J" },
            new OpRec { code=Opcode.cb123, fmt="J" },
            new OpRec { code=Opcode.cb12, fmt="J" },
            new OpRec { code=Opcode.cb13, fmt="J" },
            new OpRec { code=Opcode.cb1, fmt="J" },
            new OpRec { code=Opcode.cb23, fmt="J" },
            new OpRec { code=Opcode.cb2, fmt="J" },
            new OpRec { code=Opcode.cb3, fmt="J" },

            new OpRec { code=Opcode.cba, fmt="J" },
            new OpRec { code=Opcode.cb0, fmt="J" },
            new OpRec { code=Opcode.cb03, fmt="J" },
            new OpRec { code=Opcode.cb02, fmt="J" },
            new OpRec { code=Opcode.cb023, fmt="J" },
            new OpRec { code=Opcode.cb01, fmt="J" },
            new OpRec { code=Opcode.cb013, fmt="J" },
            new OpRec { code=Opcode.cb012, fmt="J" },

            // 30
            new OpRec { code=Opcode.tn, fmt="r14,T" },
            new OpRec { code=Opcode.te, fmt="r14,T" },
            new OpRec { code=Opcode.tle, fmt="r14,T" },
            new OpRec { code=Opcode.tl, fmt="r14,T" },
            new OpRec { code=Opcode.tleu, fmt="r14,T" },
            new OpRec { code=Opcode.tcs, fmt="r14,T" },
            new OpRec { code=Opcode.tneg, fmt="r14,T" },
            new OpRec { code=Opcode.tvs, fmt="r14,T" },
                                    
            new OpRec { code=Opcode.ta, fmt="r14,T" },
            new OpRec { code=Opcode.tne, fmt="r14,T" },
            new OpRec { code=Opcode.tg, fmt="r14,T" },
            new OpRec { code=Opcode.tge, fmt="r14,T" },
            new OpRec { code=Opcode.tgu, fmt="r14,T" },
            new OpRec { code=Opcode.tcc, fmt="r14,T" },
            new OpRec { code=Opcode.tpos, fmt="r14,T" },
            new OpRec { code=Opcode.tvc, fmt="r14,T" },
        };

        private static OpRec[] opRecs_2 = new OpRec[] 
        {
            // 00
            new OpRec { code=Opcode.add, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.and, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.or,  fmt="r14,R0,r25" },
            new OpRec { code=Opcode.xor, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.sub, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.andn,fmt="r14,R0,r25" },
            new OpRec { code=Opcode.orn, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.xnor,fmt="r14,R0,r25" },

            new OpRec { code=Opcode.addx, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.illegal,  },
            new OpRec { code=Opcode.umul, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.smul, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.subx, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.udiv, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.sdiv, fmt="r14,R0,r25" },

            // 10
            new OpRec { code=Opcode.addcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.andcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.orcc,  fmt="r14,R0,r25" },
            new OpRec { code=Opcode.xorcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.subcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.andncc,fmt="r14,R0,r25" },
            new OpRec { code=Opcode.orncc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.xnorcc,fmt="r14,R0,r25" },

            new OpRec { code=Opcode.addxcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.illegal,  },
            new OpRec { code=Opcode.umulcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.smulcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.subxcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.udivcc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.sdivcc, fmt="r14,R0,r25" },

            // 20
            new OpRec { code=Opcode.taddcc, fmt="r14,R0,r25"},
            new OpRec { code=Opcode.tsubcc, fmt="r14,R0,r25"},
            new OpRec { code=Opcode.taddcctv, fmt="r14,R0,r25"},
            new OpRec { code=Opcode.tsubcctv, fmt="r14,R0,r25"},
            new OpRec { code=Opcode.mulscc, fmt="r14,R0,r25" },
            new OpRec { code=Opcode.sll, fmt="r14,S,r25" },
            new OpRec { code=Opcode.srl, fmt="r14,S,r25" },
            new OpRec { code=Opcode.sra, fmt="r14,S,r25" },

            new OpRec { code=Opcode.rd, fmt="ry,r25" },
            new OpRec { code=Opcode.rdpsr, },
            new OpRec { code=Opcode.rdtbr, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },

            // 30
            new OpRec { code=Opcode.wrasr, },
            new OpRec { code=Opcode.wrpsr, },
            new OpRec { code=Opcode.wrwim, },
            new OpRec { code=Opcode.wrtbr, },
            new FPop1Rec { },
            new FPop2Rec { },
            new CPop1 {  },
            new CPop2 {  },

            new OpRec { code=Opcode.jmpl, fmt = "r14,Rs,r25"},
            new OpRec { code=Opcode.rett, fmt = "r14,Rs" },
            new BrachOpRec { offset= 0x30, },
            new OpRec { code=Opcode.flush, },
            new OpRec { code=Opcode.save,    fmt ="r14,R0,r25" },
            new OpRec { code=Opcode.restore, fmt= "r14,R0,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
        };

        private static OpRec[] opRecs_3 = new OpRec[]
        {
            // 00
            new OpRec { code=Opcode.ld,   fmt="Mw,r25" },
            new OpRec { code=Opcode.ldub, fmt="Mb,r25" },
            new OpRec { code=Opcode.lduh, fmt="Mh,r25" },
            new OpRec { code=Opcode.ldd,  fmt="Md,r25" },
            new OpRec { code=Opcode.st,   fmt="r25,Mw"},
            new OpRec { code=Opcode.stb,  fmt="r25,Mb"},
            new OpRec { code=Opcode.sth,  fmt="r25,Mh"},
            new OpRec { code=Opcode.std,  fmt="r25,Md"},

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldsb,    fmt="Mb+,r25" },
            new OpRec { code=Opcode.ldsh,    fmt="Mh+,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldstub,  },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.swap,    fmt="Mw,r25" },

            // 10
            new OpRec { code=Opcode.lda,  fmt="Aw,r25" },
            new OpRec { code=Opcode.lduba,fmt="Ab,r25" }, 
            new OpRec { code=Opcode.lduha,fmt="Ah,r25" }, 
            new OpRec { code=Opcode.ldda, fmt="Ad,r25" },
            new OpRec { code=Opcode.sta,  fmt="r25,Aw" },
            new OpRec { code=Opcode.stba, fmt="r25,Ab" },
            new OpRec { code=Opcode.stha, fmt="r25,Ah" },
            new OpRec { code=Opcode.stda, fmt="r25,Ad" },

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldsba,   fmt="r25,Ab" },
            new OpRec { code=Opcode.ldsha,   fmt="r25,Ah" },
            new OpRec { code=Opcode.illegal  },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldstuba, fmt="Ab,r25"},
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.swapa,  fmt="Aw,r25" },

            // 20
            new OpRec { code=Opcode.ldf,   fmt="Mw,f24", },
            new OpRec { code=Opcode.ldfsr, fmt="Mw,%fsr" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.lddf,  fmt="Md,f24" },
            new OpRec { code=Opcode.stf,   fmt ="f24,Mw" },
            new OpRec { code=Opcode.stfsr, fmt="%fsr,Mw" },
            new OpRec { code=Opcode.stdfq, },
            new OpRec { code=Opcode.stdf, fmt= "f24,Md" },

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },

            // 30
            new OpRec { code=Opcode.ldc, },
            new OpRec { code=Opcode.ldcsr, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.lddc, },
            new OpRec { code=Opcode.stc, },
            new OpRec { code=Opcode.stcsr, },
            new OpRec { code=Opcode.stdcq, },
            new OpRec { code=Opcode.stdc, },

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
        };

        private class FPop1Rec : OpRec
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpOprecs[(wInstr >> 5) & 0x1FF].Decode(dasm, wInstr);
            }
        }

        private class FPop2Rec : OpRec
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpOprecs[(wInstr >> 5) & 0x1FF].Decode(dasm, wInstr);
            }
        }

        private class CPop1 : OpRec
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0x1FF].Decode(dasm, wInstr);
            }
        }
        private class CPop2 : OpRec
        {
            public override SparcInstruction Decode(SparcDisassembler dasm, uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0xFF].Decode(dasm, wInstr);
            }
        }

        private static Dictionary<uint, OpRec> fpOprecs = new Dictionary<uint,OpRec>
        {
            // 00 
            { 0x01, new OpRec { code=Opcode.fmovs, fmt="f0,f25" } },
            { 0x05, new OpRec { code=Opcode.fnegs, fmt="f0,f25" } },
            { 0x09, new OpRec { code=Opcode.fabss, fmt="f0,f25" } },
            { 0x29, new OpRec { code=Opcode.fsqrts, fmt="f0,f25" } },
            { 0x2A, new OpRec { code=Opcode.fsqrtd, fmt="f0,f25" } },
            { 0x2B, new OpRec { code=Opcode.fsqrtq, fmt="f0,f25" } },

            { 0x41, new OpRec { code=Opcode.fadds, fmt="f14,f0,f25" } },
            { 0x42, new OpRec { code=Opcode.faddd, fmt="f14,f0,f25" } },
            { 0x43, new OpRec { code=Opcode.faddq, fmt="f14,f0,f25" } },
            { 0x45, new OpRec { code=Opcode.fsubs, fmt="f14,f0,f25" } },
            { 0x46, new OpRec { code=Opcode.fsubd, fmt="f14,f0,f25" } },
            { 0x47, new OpRec { code=Opcode.fsubq, fmt="f14,f0,f25" } },

            { 0xC4, new OpRec { code=Opcode.fitos, fmt="f0,f25" } },
            { 0xC6, new OpRec { code=Opcode.fdtos, fmt="f0,f25" } },
            { 0xC7, new OpRec { code=Opcode.fqtos, fmt="f0,f25" } },
            { 0xC8, new OpRec { code=Opcode.fitod, fmt="f0,f25" } },
            { 0xC9, new OpRec { code=Opcode.fstod, fmt="f0,f25" } },
            { 0xCB, new OpRec { code=Opcode.fqtod, fmt="f0,f25" } },
            { 0xCC, new OpRec { code=Opcode.fitoq, fmt="f0,f25" } },
            { 0xCD, new OpRec { code=Opcode.fstoq, fmt="f0,f25" } },
            { 0xCE, new OpRec { code=Opcode.fdtoq, fmt="f0,f25" } },
            { 0xD1, new OpRec { code=Opcode.fstoi, fmt="f0,f25" } },
            { 0xD2, new OpRec { code=Opcode.fdtoi, fmt="f0,f25" } },
            { 0xD3, new OpRec { code=Opcode.fqtoi, fmt="f0,f25" } },

            { 0x49, new OpRec { code=Opcode.fmuls, fmt="f14,f0,f25" } },
            { 0x4A, new OpRec { code=Opcode.fmuld, fmt="f14,f0,f25" } },
            { 0x4B, new OpRec { code=Opcode.fmulq, fmt="f14,f0,f25" } },
            { 0x4D, new OpRec { code=Opcode.fdivs, fmt="f14,f0,f25" } },
            { 0x4E, new OpRec { code=Opcode.fdivd, fmt="f14,f0,f25" } },
            { 0x4F, new OpRec { code=Opcode.fdivq, fmt="f14,f0,f25" } },

            { 0x69, new OpRec { code=Opcode.fsmuld, fmt="f14,f0,f25" } },
            { 0x6E, new OpRec { code=Opcode.fdmulq, fmt="f14,f0,f25" } },

            { 0x51, new OpRec { code=Opcode.fcmps, fmt="f14,f0" } },
            { 0x52, new OpRec { code=Opcode.fcmpd, fmt="f14,f0" } },
            { 0x53, new OpRec { code=Opcode.fcmpq, fmt="f14,f0" } },
            { 0x55, new OpRec { code=Opcode.fcmpes, fmt="f14,f0" } },
            { 0x56, new OpRec { code=Opcode.fcmped, fmt="f14,f0" } },
            { 0x57, new OpRec { code=Opcode.fcmpeq, fmt="f14,f0" } },
        };
    }
}
