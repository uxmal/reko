#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcDisassembler : Disassembler
    {
        private SparcArchitecture arch;
        private ImageReader imageReader;

        public SparcDisassembler(SparcArchitecture arch, ImageReader imageReader)
        {
            this.arch = arch;
            this.imageReader = imageReader;
        }

        public Address Address  { get { return imageReader.Address; } }

        public MachineInstruction DisassembleInstruction()
        {
            throw new NotImplementedException();
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
        public SparcInstruction Disassemble()
        {
            if (!imageReader.IsValid)
                return null;
            uint wInstr = imageReader.ReadBeUInt32();
            switch (wInstr >> 30)
            {
            case 0:
                return opRecs_0[(wInstr >> 22) & 7].Decode(wInstr);
            case 1:
                return new SparcInstruction
                {
                    Opcode = Opcode.call,
                    Op1 = new AddressOperand((imageReader.Address - 4) + (wInstr << 2)),
                };
            case 2:
                return opRecs_2[(wInstr >> 19) & 0x3F].Decode(wInstr);
            case 3:
                return opRecs_2[(wInstr >> 19) & 0x3F].Decode(wInstr);
            }
            throw new InvalidOperationException("Impossible!");
        }

        private class OpRec
        {
            public Opcode code;
            public string op;

            public virtual SparcInstruction Decode(uint wInstr)
            {
                if (op == null)
                    return new SparcInstruction { Opcode = code };
                var ops = new List<MachineOperand>();
                int i = 0;
                while (i != op.Length)
                {
                    char c = op[i++];
                    if (c == ',')
                        continue;
                    switch (c)
                    {
                    default: throw new NotSupportedException(string.Format("Format {0} not supported.", c));
                    case 'r':       // Register reference
                        ops.Add(GetRegisterOperand(wInstr, ref i));
                        break;
                    case 'I':       // 22-bit immediate value
                        ops.Add(GetImmOperand(wInstr, 22));
                        break;
                    case 'R':       // Register or simm13.
                        ++i;        // skip unused 0
                        ops.Add(GetRegImmOperand(wInstr, 13));
                        break;
                    case 'S':       // Register or uimm5
                        ops.Add(GetRegImmOperand(wInstr, 6));
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

            private RegisterOperand GetRegisterOperand(uint wInstr, ref int i)
            {
                return new RegisterOperand(GetRegister(wInstr, ref i));
            }

            private RegisterStorage GetRegister(uint wInstr, ref int i)
            {
                // Register operand are followed by their bit offset within the instruction,
                // expressed as decimal digits.
                int offset = 0;
                while (i < op.Length && Char.IsDigit(op[i]))
                {
                    offset = offset * 10 + (op[i++] - '0');
                }
                return Registers.GetRegister((wInstr >> offset) & 0x1F);
            }

            private MachineOperand GetRegImmOperand(uint wInstr, int bits)
            {
                if ((wInstr & (1 << 13)) != 0)
                {
                    // Sign-extend the bastard.
                    int imm = (int) wInstr & ((1 << bits) - 1);
                    int mask = (0 - (imm & (1 << (bits - 1)))) << 1;
                    return new ImmediateOperand(
                        Constant.Word32(imm | mask));
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

        private OpRec [] opRecs_0 = new OpRec[] {
            new OpRec {code= Opcode.unimp, },
            new OpRec { code=Opcode.illegal, },
            new BrachOpRec { offset = 0x00 },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code= Opcode.sethi, op="I,r25" }, 
            new OpRec { code=Opcode.illegal, },
            new BrachOpRec { offset = 0x10 },
            new BrachOpRec { offset = 0x20 },
        };

        private class BrachOpRec : OpRec
        {
            public uint offset;

            public override SparcInstruction Decode(uint wInstr)
            {
                uint i = ((wInstr >> 24) & 0xF) + offset;
                return branchOps[i].Decode(wInstr);
            }
        }

        private static OpRec[] branchOps = new OpRec[] {
            // 00
            new OpRec { code=Opcode.bn, },
            new OpRec { code=Opcode.be, },
            new OpRec { code=Opcode.ble, },
            new OpRec { code=Opcode.bl, },
            new OpRec { code=Opcode.bleu, },
            new OpRec { code=Opcode.bcs, },
            new OpRec { code=Opcode.bneg, },
            new OpRec { code=Opcode.bvs, },

            new OpRec { code=Opcode.ba, },
            new OpRec { code=Opcode.bne, },
            new OpRec { code=Opcode.bg, },
            new OpRec { code=Opcode.bge, },
            new OpRec { code=Opcode.bgu, },
            new OpRec { code=Opcode.bcc, },
            new OpRec { code=Opcode.bpos, },
            new OpRec { code=Opcode.bvc, },

            // 10
            new OpRec { code=Opcode.fbn, },
            new OpRec { code=Opcode.fbne, },
            new OpRec { code=Opcode.fblg, },
            new OpRec { code=Opcode.fbul, },
            new OpRec { code=Opcode.fbug, },
            new OpRec { code=Opcode.fbg, },
            new OpRec { code=Opcode.fbu, },
            new OpRec { code=Opcode.fbug, },

            new OpRec { code=Opcode.fba, },
            new OpRec { code=Opcode.fbe, },
            new OpRec { code=Opcode.fbue, },
            new OpRec { code=Opcode.fbge, },
            new OpRec { code=Opcode.fbuge, },
            new OpRec { code=Opcode.fble, },
            new OpRec { code=Opcode.fbule, },
            new OpRec { code=Opcode.fbo, },

            // 20
            new OpRec { code=Opcode.cbn, },
            new OpRec { code=Opcode.cb123, },
            new OpRec { code=Opcode.cb12, },
            new OpRec { code=Opcode.cb13, },
            new OpRec { code=Opcode.cb1, },
            new OpRec { code=Opcode.cb23, },
            new OpRec { code=Opcode.cb2, },
            new OpRec { code=Opcode.cb3, },

            new OpRec { code=Opcode.cba, },
            new OpRec { code=Opcode.cb0, },
            new OpRec { code=Opcode.cb03, },
            new OpRec { code=Opcode.cb02, },
            new OpRec { code=Opcode.cb023, },
            new OpRec { code=Opcode.cb01, },
            new OpRec { code=Opcode.cb013, },
            new OpRec { code=Opcode.cb012, },

            // 30
            new OpRec { code=Opcode.tn, },
            new OpRec { code=Opcode.te, },
            new OpRec { code=Opcode.tle, },
            new OpRec { code=Opcode.tl, },
            new OpRec { code=Opcode.tleu, },
            new OpRec { code=Opcode.tcs, },
            new OpRec { code=Opcode.tneg, },
            new OpRec { code=Opcode.tvs, },
                                    
            new OpRec { code=Opcode.ta, },
            new OpRec { code=Opcode.tne, },
            new OpRec { code=Opcode.tg, },
            new OpRec { code=Opcode.tge, },
            new OpRec { code=Opcode.tgu, },
            new OpRec { code=Opcode.tcc, },
            new OpRec { code=Opcode.tpos, },
            new OpRec { code=Opcode.tvc, },
        };

        private static OpRec[] opRecs_2 = new OpRec[] {
            // 00
            new OpRec { code=Opcode.add, op="r14,R0,r25" },
            new OpRec { code=Opcode.and, op="r14,R0,r25" },
            new OpRec { code=Opcode.or,  op="r14,R0,r25" },
            new OpRec { code=Opcode.xor, op="r14,R0,r25" },
            new OpRec { code=Opcode.sub, op="r14,R0,r25" },
            new OpRec { code=Opcode.andn,op="r14,R0,r25" },
            new OpRec { code=Opcode.orn, op="r14,R0,r25" },
            new OpRec { code=Opcode.xnor,op="r14,R0,r25" },

            new OpRec { code=Opcode.addx, op="r14,R0,r25" },
            new OpRec { code=Opcode.illegal,  },
            new OpRec { code=Opcode.umul, op="r14,R0,r25" },
            new OpRec { code=Opcode.smul, op="r14,R0,r25" },
            new OpRec { code=Opcode.subx, op="r14,R0,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.udiv, op="r14,R0,r25" },
            new OpRec { code=Opcode.sdiv, op="r14,R0,r25" },

            // 10
            new OpRec { code=Opcode.addcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.andcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.orcc,  op="r14,R0,r25" },
            new OpRec { code=Opcode.xorcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.subcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.andncc,op="r14,R0,r25" },
            new OpRec { code=Opcode.orncc, op="r14,R0,r25" },
            new OpRec { code=Opcode.xnorcc,op="r14,R0,r25" },

            new OpRec { code=Opcode.addxcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.illegal,  },
            new OpRec { code=Opcode.umulcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.smulcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.subxcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.udivcc, op="r14,R0,r25" },
            new OpRec { code=Opcode.sdivcc, op="r14,R0,r25" },

            // 20
            new OpRec { code=Opcode.taddcc, op="r14,R0,r25"},
            new OpRec { code=Opcode.tsubcc, op="r14,R0,r25"},
            new OpRec { code=Opcode.taddcctv, op="r14,R0,r25"},
            new OpRec { code=Opcode.tsubcctv, op="r14,R0,r25"},
            new OpRec { code=Opcode.mulscc, op="r14,R0,r25" },
            new OpRec { code=Opcode.sll, op="r14,S,r25" },
            new OpRec { code=Opcode.srl, op="r14,S,r25" },
            new OpRec { code=Opcode.sra, op="r14,S,r25" },

            new OpRec { code=Opcode.addx, },
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
            new FPop2Rec{  },
            new CPop1 {  },
            new CPop2 {  },

            new OpRec { code=Opcode.jmpl, },
            new OpRec { code=Opcode.rett, },
            new BrachOpRec { offset= 0x30, },
            new OpRec { code=Opcode.flush, },
            new OpRec { code=Opcode.save, },
            new OpRec { code=Opcode.restore, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
        };

        private static OpRec[] opRecs_3 = new OpRec[] {
            // 00
            new OpRec { code=Opcode.ld,   op="r25,Ew" },
            new OpRec { code=Opcode.ldub, op="r25,Eb" },
            new OpRec { code=Opcode.lduh, op="r25,Eh" },
            new OpRec { code=Opcode.ldd,  op="r25,Ed" },
            new OpRec { code=Opcode.st,   op="r25,Ew"},
            new OpRec { code=Opcode.stb,  op="r25,Eb"},
            new OpRec { code=Opcode.sth,  op="r25,Eh"},
            new OpRec { code=Opcode.std,  op="r25,Ed"},

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldsb,    op="r25,Eb" },
            new OpRec { code=Opcode.ldsh,    op="r25,Eh" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldstub,  },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.swap,    op="Ew,r25" },

            // 10
            new OpRec { code=Opcode.lda,  op="r25,Aw" },
            new OpRec { code=Opcode.lduba,op="r25,Ab" }, 
            new OpRec { code=Opcode.lduha,op="r25,Ah" }, 
            new OpRec { code=Opcode.ldda, op="r25,Ad" },
            new OpRec { code=Opcode.sta,  op="r25,Aw"},
            new OpRec { code=Opcode.stba, op="r25,Ab"},
            new OpRec { code=Opcode.stha, op="r25,Ah"},
            new OpRec { code=Opcode.stda, op="r25,Ad"},

            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldsba,   op="r25,Eb" },
            new OpRec { code=Opcode.ldsha,   op="r25,Eh" },
            new OpRec { code=Opcode.illegal  },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.ldstuba, op="Eb,r25"},
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.swapa,  op="Ew,r25" },

            // 20
            new OpRec { code=Opcode.ldf,   op="Fw,f24", },
            new OpRec { code=Opcode.ldfsr, op="Ew,%fsr" },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.lddf,  op="Fd,f24" },
            new OpRec { code=Opcode.stf,   op ="f24,Fw" },
            new OpRec { code=Opcode.stfsr ,op="%fsr,Ew" },
            new OpRec { code=Opcode.stdfq, },
            new OpRec { code=Opcode.stdf, },

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
            public override SparcInstruction Decode(uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0xFF].Decode(wInstr);
            }
        }

        private class FPop2Rec : OpRec
        {
            public override SparcInstruction Decode(uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0xFF].Decode(wInstr);
            }
        }

        private class CPop1 : OpRec
        {
            public override SparcInstruction Decode(uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0xFF].Decode(wInstr);
            }
        }
        private class CPop2 : OpRec
        {
            public override SparcInstruction Decode(uint wInstr)
            {
                return fpOprecs[(wInstr >> 4) & 0xFF].Decode(wInstr);
            }
        }


        private static OpRec[] fpOprecs = 
        {
            // 00 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.fmovs, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.fnegs, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            // 10 
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },
            new OpRec { code=Opcode.illegal, },

        };

    }
}
