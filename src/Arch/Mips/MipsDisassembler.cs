#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private MipsProcessorArchitecture arch;
        private MipsInstruction instrCur;
        private Address addr;
        private EndianImageReader rdr;
        internal bool isVersion6OrLater;

        public MipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader imageReader, bool isVersion6OrLater)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.isVersion6OrLater = isVersion6OrLater;
        }

        public override MipsInstruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null; 
            this.addr = rdr.Address;
            uint wInstr;
            if (!rdr.TryReadUInt32(out wInstr))
            {
                return null;
            }
            var opRec = opRecs[wInstr >> 26];
            try
            {
                if (opRec == null)
                    instrCur = new MipsInstruction { opcode = Opcode.illegal };
                else
                    instrCur = opRec.Decode(wInstr, this);
            }
            catch
            {
                instrCur = null;
            }
            if (instrCur == null)
            {
                instrCur = new MipsInstruction { opcode = Opcode.illegal };
            }
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        private static OpRec[] opRecs = new OpRec[]
        {
            new SpecialOpRec(),
            new CondOpRec(),
            new AOpRec(Opcode.j, "J"),
            new AOpRec(Opcode.jal, "J"),
            new AOpRec(Opcode.beq, "R1,R2,j"),
            new AOpRec(Opcode.bne, "R1,R2,j"),
            new AOpRec(Opcode.blez, "R1,j"),
            new AOpRec(Opcode.bgtz, "R1,j"),

            new AOpRec(Opcode.addi, "R2,R1,I"),
            new AOpRec(Opcode.addiu, "R2,R1,I"),
            new AOpRec(Opcode.slti, "R2,R1,I"),
            new AOpRec(Opcode.sltiu, "R2,R1,I"),

            new AOpRec(Opcode.andi, "R2,R1,U"),
            new AOpRec(Opcode.ori, "R2,R1,U"),
            new AOpRec(Opcode.xori, "R2,R1,U"),
            new AOpRec(Opcode.lui, "R2,i"),
            // 10
            new CoprocessorOpRec(
                new AOpRec(Opcode.mfc0, "R2,R3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.mtc0, "R2,R3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, "")),

            new CoprocessorOpRec(
                new AOpRec(Opcode.mfc1, "R2,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.cfc1, "R2,f3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.mtc1, "R2,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.ctc1, "R2,f3"),
                new AOpRec(Opcode.illegal, ""),

                new BcNRec(Opcode.bc1f, Opcode.bc1t),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new FpuOpRec(PrimitiveType.Real64,
                    // fn 00
                    new AOpRec(Opcode.add_d, "F4,F3,F2"),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    // fn 10
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    // fn 20
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.cvt_w_d, "F4,F3"),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    // fn 30
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.c_le_d, "c8,F3,F2"),
                    new AOpRec(Opcode.illegal, "")),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, "")),

           new CoprocessorOpRec(
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, "")),
            null, 
            new AOpRec(Opcode.beql, "R1,R2,j"), 
            new AOpRec(Opcode.bnel, "R1,R2,j"), 
            new AOpRec(Opcode.blezl, "R1,j"), 
            new AOpRec(Opcode.bgtzl, "R1,j"), 

            new AOpRec(Opcode.daddi, "R2,R1,I"),
            new AOpRec(Opcode.daddiu, "R2,R1,I"),
            new AOpRec(Opcode.ldl, "R2,El"),
            new AOpRec(Opcode.ldr, "R2,El"),
            null,
            null,
            null,
            new Special3OpRec(), 

            // 20
            new AOpRec(Opcode.lb, "R2,EB"),
            new AOpRec(Opcode.lh, "R2,EH"),
            new AOpRec(Opcode.lwl, "R2,Ew"),
            new AOpRec(Opcode.lw, "R2,Ew"),
                              
            new AOpRec(Opcode.lbu, "R2,Eb"),
            new AOpRec(Opcode.lhu, "R2,Eh"),
            new AOpRec(Opcode.lwr, "R2,Ew"),
            new AOpRec(Opcode.lwu, "R2,Ew"),
            
            new AOpRec(Opcode.sb, "R2,Eb"),
            new AOpRec(Opcode.sh, "R2,Eh"),
            new AOpRec(Opcode.swl, "R2,Ew"),
            new AOpRec(Opcode.sw, "R2,Ew"),

            new AOpRec(Opcode.sdl, "R2,Ew"),
            new AOpRec(Opcode.sdr, "R2,Ew"),
            new AOpRec(Opcode.swr, "R2,Ew"),
            null,
            // 30
            new Version6OpRec(
                new AOpRec(Opcode.ll, "R2,Ew"),
                new AOpRec(Opcode.illegal, "")),
            null, 
            null, 
            new AOpRec(Opcode.pref, "R2,Ew"),

            new Version6OpRec(
                new AOpRec(Opcode.lld, "R2,El"),
                new AOpRec(Opcode.illegal, "")),
            null, 
            null,
            new AOpRec(Opcode.ld, "R2,El"),

            new Version6OpRec(
                new AOpRec(Opcode.sc, "R2,El"),
                new AOpRec(Opcode.illegal, "")),
            new AOpRec(Opcode.swc1, "F2,Ew"),
            null,
            null,

            new AOpRec(Opcode.scd, "R2,El"),
            null, 
            null, 
            new AOpRec(Opcode.sd, "R2,El"),
        };

        public MipsInstruction DecodeOperands(Opcode opcode, uint wInstr, string opFmt)
        {
            var ops = new List<MachineOperand>();
            MachineOperand op = null;
            for (int i = 0; i < opFmt.Length; ++i)
            {
                switch (opFmt[i])
                {
                default: throw new NotImplementedException(string.Format("Operator format {0}", opFmt[i]));
                case ',':
                    continue;
                case 'R':
                    switch (opFmt[++i])
                    {
                    case '1': op = Reg(wInstr >> 21); break;
                    case '2': op = Reg(wInstr >> 16); break;
                    case '3': op = Reg(wInstr >> 11); break;
                    //case '4': op = MemOff(wInstr >> 6, wInstr); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'F':
                    switch (opFmt[++i])
                    {
                    case '1': op = FReg(wInstr >> 21); break;
                    case '2': op = FReg(wInstr >> 16); break;
                    case '3': op = FReg(wInstr >> 11); break;
                    case '4': op = FReg(wInstr >> 6); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'f': // FPU control register
                    RegisterOperand fcreg;
                    switch (opFmt[++i])
                    {
                    case '1': if (!TryGetFCReg(wInstr >> 21, out fcreg)) return null; op = fcreg; break;
                    case '2': if (!TryGetFCReg(wInstr >> 16, out fcreg)) return null; op = fcreg; break;
                    case '3': if (!TryGetFCReg(wInstr >> 11, out fcreg)) return null; op = fcreg; break;
                    //case '4': op = MemOff(wInstr >> 6, wInstr); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'I':
                    op = ImmediateOperand.Int32((short) wInstr);
                    break;
                case 'U':
                    op = ImmediateOperand.Word32((ushort) wInstr);
                    break;
                case 'i':
                    op = ImmediateOperand.Int16((short) wInstr);
                    break;
                case 'j':
                    op = RelativeBranch(wInstr);
                    break;
                case 'J':
                    op = LargeBranch(wInstr);
                    break;
                case 'B':
                    op = ImmediateOperand.Word32((wInstr >> 6) & 0xFFFFF);
                    break;
                case 's':   // Shift amount or sync type
                    op = ImmediateOperand.Byte((byte)((wInstr >> 6) & 0x1F));
                    break;
                case 'E':   // effective address w 16-bit offset
                    op = Ea(wInstr, opFmt[++i], 21, (short)wInstr);
                    break;
                case 'e':   // effective address w 9-bit offset
                    op = Ea(wInstr, opFmt[++i], 21, (short)(((short)wInstr) >> 7));
                    break;
                case 'T':   // trap code
                    op = ImmediateOperand.Word16((ushort)((wInstr >> 6) & 0x03FF));
                    break;
                case 'c':   // condition code
                    op = CCodeFlag(wInstr, opFmt, ref i);
                    break;
                case 'H':   // hardware register, see instruction rdhwr
                    op = ImmediateOperand.Byte((byte)((wInstr >> 11) & 0x1f));
                    break;
                }
                ops.Add(op);
            }
            return new MipsInstruction
            {
                opcode = opcode,
                Address = addr,
                Length = 4,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
                op3 = ops.Count > 2 ? ops[2] : null,
            };
        }

        private RegisterOperand Reg(uint regNumber)
        {
            return new RegisterOperand(arch.GetRegister((int) regNumber & 0x1F));
        }

        private RegisterOperand FReg(uint regNumber)
        {
            return new RegisterOperand(Registers.fpuRegs[regNumber & 0x1F]);
        }

        private bool TryGetFCReg(uint regNumber, out RegisterOperand op)
        {
            RegisterStorage fcreg;
            if (Registers.fpuCtrlRegs.TryGetValue(regNumber & 0x1F, out fcreg))
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

        private RegisterOperand CCodeFlag(uint wInstr, string fmt, ref int i)
        {
            int pos = 0;
            while (Char.IsDigit(fmt[++i]))
            {
                pos = pos * 10 + fmt[i] - '0';
            }
            var regNo = (wInstr >> pos) & 0x7;
            return new RegisterOperand(Registers.ccRegs[regNo]);
        }

        private AddressOperand RelativeBranch(uint wInstr)
        {
            int off = (short) wInstr;
            off <<= 2;
            return AddressOperand.Ptr32((uint)(off + rdr.Address.ToUInt32()));
        }

        private AddressOperand LargeBranch(uint wInstr)
        {
            var off = (wInstr & 0x03FFFFFF) << 2;
            return AddressOperand.Ptr32((rdr.Address.ToUInt32() & 0xF0000000u) | off);
        }

        private IndirectOperand Ea(uint wInstr, char wCode, int shift, short offset)
        {
            PrimitiveType dataWidth;
            switch (wCode)
            {
            default: throw new NotSupportedException(string.Format("Unknown width code '{0}'.", wCode));
            case 'b': dataWidth = PrimitiveType.Byte; break;
            case 'B': dataWidth = PrimitiveType.SByte; break;
            case 'h': dataWidth = PrimitiveType.Word16; break;
            case 'H': dataWidth = PrimitiveType.Int16; break;
            case 'w': dataWidth = PrimitiveType.Word32; break;
            case 'W': dataWidth = PrimitiveType.Int32; break;
            case 'l': dataWidth = PrimitiveType.Word64; break;
            case 'L': dataWidth = PrimitiveType.Int64; break;
            }
            var baseReg = arch.GetRegister((int)(wInstr >> shift) & 0x1F);
            return new IndirectOperand(dataWidth, offset, baseReg);
        }
    }
}
