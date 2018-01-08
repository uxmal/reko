#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet
 * inspired by work of
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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microchip.Crownking;

namespace Reko.Arch.Microchip.PIC18
{
    public class PIC18Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private PIC18Architecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private PIC18Disassembler disasm;
        private IEnumerator<PIC18Instruction> dasm;
        private ProcessorState state;
        private PIC18Instruction instr;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private List<RtlInstructionCluster> clusters;
        private RtlEmitter m;

        public PIC18Rewriter(PIC18Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.disasm = new PIC18Disassembler(arch, rdr);
            this.dasm = disasm.GetEnumerator();
            this.state = state;
            this.binder = binder;
            this.host = host;
        }

        /// <summary>
        /// Gets or sets the PIC18 execution mode.
        /// </summary>
        public PICExecMode ExecMode
        {
            get
            {
                return disasm.ExecMode;
            }
            set
            {
                disasm.ExecMode = value;
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = instr.Address;
                var len = instr.Length;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                switch (instr.Opcode)
                {
                    default:
                        throw new AddressCorrelatedException(addr, $"Rewriting of PIC18 instruction '{instr.Opcode}' is not implemented yet.");

                    case Opcode.invalid: m.Invalid(); break;
                    case Opcode.ADDFSR: RewriteADDFSR(); break;
                    case Opcode.ADDLW: RewriteADDLW(); break;

                    case Opcode.ADDULNK:
                        break;

                    case Opcode.ADDWF: RewriteADDWF(); break;
                    case Opcode.ADDWFC: RewriteADDWFC(); break;
                    case Opcode.ANDLW: RewriteANDLW(); break;
                    case Opcode.ANDWF: RewriteANDWF(); break;
                    case Opcode.BC: RewriteBC(); break;
                    case Opcode.BCF: RewriteBCF(); break;
                    case Opcode.BN: RewriteBN(); break;
                    case Opcode.BNC: RewriteBNC(); break;
                    case Opcode.BNN: RewriteBNN(); break;
                    case Opcode.BNOV: RewriteBNOV(); break;
                    case Opcode.BNZ: RewriteBNZ(); break;
                    case Opcode.BOV: RewriteBOV(); break;
                    case Opcode.BRA: RewriteBRA(); break;
                    case Opcode.BSF: RewriteBSF(); break;
                    case Opcode.BTFSC: RewriteBTFSC(); break;
                    case Opcode.BTFSS: RewriteBTFSC(); break;
                    case Opcode.BTG: RewriteBTG(); break;
                    case Opcode.BZ: RewriteBZ(); break;
                    case Opcode.CALL: RewriteCALL(); break;
                    case Opcode.CALLW: RewriteCALLW(); break;
                    case Opcode.CLRF: RewriteCLRF(); break;
                    case Opcode.CLRWDT: RewriteCLRWDT(); break;
                    case Opcode.COMF: RewriteCOMF(); break;
                    case Opcode.CPFSEQ: RewriteCPFSEQ(); break;
                    case Opcode.CPFSGT: RewriteCPFSGT(); break;
                    case Opcode.CPFSLT: RewriteCPFSLT(); break;

                    case Opcode.DAW:
                        break;

                    case Opcode.DCFSNZ: RewriteDCFSNZ(); break;
                    case Opcode.DECF: RewriteDECF(); break;
                    case Opcode.DECFSZ: RewriteDECFSZ(); break;
                    case Opcode.GOTO: RewriteGOTO(); break;
                    case Opcode.INCF: RewriteINCF(); break;
                    case Opcode.INCFSZ: RewriteINCFSZ(); break;
                    case Opcode.INFSNZ: RewriteINFSNZ(); break;
                    case Opcode.IORLW: RewriteIORLW(); break;
                    case Opcode.IORWF: RewriteIORWF(); break;

                    case Opcode.LFSR:
                        break;

                    case Opcode.MOVF:
                        break;

                    case Opcode.MOVFF:
                        break;

                    case Opcode.MOVFFL:
                        break;

                    case Opcode.MOVLB: RewriteMOVLB(); break;
                    case Opcode.MOVLW: RewriteMOVLW(); break;

                    case Opcode.MOVSF:
                        break;

                    case Opcode.MOVSFL:
                        break;

                    case Opcode.MOVSS:
                        break;

                    case Opcode.MOVWF: RewriteMOVWF(); break;
                    case Opcode.MULLW: RewriteMULLW(); break;
                    case Opcode.MULWF: RewriteMULWF(); break;
                    case Opcode.NEGF: RewriteNEGF(); break;
                    case Opcode.NOP: m.Nop(); break;

                    case Opcode.POP:
                        break;

                    case Opcode.PUSH:
                        break;

                    case Opcode.PUSHL:
                        break;

                    case Opcode.RCALL:
                        break;

                    case Opcode.RESET:
                        break;

                    case Opcode.RETFIE:
                        break;

                    case Opcode.RETLW:
                        break;

                    case Opcode.RETURN:
                        break;

                    case Opcode.RLCF: RewriteRLCF(); break;
                    case Opcode.RLNCF: RewriteRLNCF(); break;
                    case Opcode.RRCF: RewriteRRCF(); break;
                    case Opcode.RRNCF: RewriteRRNCF(); break;
                    case Opcode.SETF: RewriteSETF(); break;
                    case Opcode.SLEEP: RewriteSLEEP(); break;
                    case Opcode.SUBFSR: RewriteSUBFSR(); break;
                    case Opcode.SUBFWB: RewriteSUBFWB(); break;
                    case Opcode.SUBLW: RewriteSUBLW(); break;

                    case Opcode.SUBULNK:
                        break;

                    case Opcode.SUBWF: RewriteSUBWF(); break;
                    case Opcode.SUBWFB: RewriteSUBWFB(); break;

                    case Opcode.SWAPF:
                        break;

                    case Opcode.TBLRD:
                        break;

                    case Opcode.TBLWT:
                        break;

                    case Opcode.TSTFSZ: RewriteTSTFSZ(); break;
                    case Opcode.XORLW: RewriteXORLW(); break;
                    case Opcode.XORWF: RewriteXORWF(); break;
                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break; ;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); ;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(Registers.status, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        private Expression RewriteOp(PIC18OperandImpl op)
        {
            var imm4Op = op as PIC18Immed4Operand;
            if (imm4Op != null)
                return imm4Op.Imm4;
            var imm6Op = op as PIC18Immed6Operand;
            if (imm6Op != null)
                return imm6Op.Imm6;
            var imm8Op = op as PIC18Immed8Operand;
            if (imm8Op != null)
                return imm8Op.Imm8;
            var imm12Op = op as PIC18Immed12Operand;
            if (imm6Op != null)
                return imm12Op.Imm12;
            var imm14Op = op as PIC18Immed14Operand;
            if (imm14Op != null)
                return imm14Op.Imm14;
            var bankaccess = op as PIC18DataBankAccessOperand;
            if (bankaccess != null)
                return getMemoryBankAccess(bankaccess);
            var off8 = op as PIC18ProgRel8AddrOperand;
            if (off8 != null)
                return off8.CodeTarget.Address;
            var off11 = op as PIC18ProgRel11AddrOperand;
            if (off11 != null)
                return off11.CodeTarget.Address;
            var tgtprog = op as PIC18ProgAbsAddrOperand;
            if (tgtprog != null)
                return tgtprog.CodeTarget.Address;
            var fsridxOp = op as PIC18FSR2IdxOperand;
            if (fsridxOp != null)
                return fsridxOp.Offset;
            var fsrnumOp = op as PIC18FSRNumOperand;
            if (fsrnumOp != null)
            {
                switch (fsrnumOp.FSRNum.ToByte())
                {
                    case 0: return binder.EnsureSequence(Registers.fsr0h, Registers.fsr0l, PrimitiveType.Ptr16);
                    case 1: return binder.EnsureSequence(Registers.fsr1h, Registers.fsr1l, PrimitiveType.Ptr16);
                    case 2: return binder.EnsureSequence(Registers.fsr2h, Registers.fsr2l, PrimitiveType.Ptr16);
                    default: throw new InvalidOperationException();
                }
            }
            var mem12bitaddr = op as PIC18Memory12bitAbsAddrOperand;
            if (mem12bitaddr != null)
                return mem12bitaddr.DataTarget.Address;
            var mem14bitaddr = op as PIC18Memory14bitAbsAddrOperand;
            if (mem14bitaddr != null)
                return mem14bitaddr.DataTarget.Address;
            var fsr2idx = op as PIC18FSR2IdxOperand;
            if (fsr2idx != null)
                return fsr2idx.Offset;
            var shadow = op as PIC18ShadowOperand;
            if (shadow != null)
                return shadow.IsShadow;
            var tblincrmod = op as PIC18TableReadWriteOperand;
            if (tblincrmod != null)
                return tblincrmod.TBLIncrMode;

            throw new NotImplementedException($"Rewriting of PIC18 operand type {op.GetType().FullName} is not implemented yet.");
        }

        private Expression RewriteDestOp(PIC18OperandImpl op)
        {
            Expression dst = null;
            var dstop = op as PIC18DataByteAccessWithDestOperand;
            if (dstop != null)
            {
                dst = (dstop.WregIsDest.ToBoolean() ? binder.EnsureRegister(Registers.wreg) : getMemoryBankAccess(dstop));
            }
            return dst;
        }

        private Expression getMemoryBankAccess(PIC18DataBankAccessOperand mem)
        {
            Address offset = mem.MemAddr.Address;
            if ((mem.ExecMode == PICExecMode.Extended) && mem.IsAccessRAM.ToBoolean() && (offset.ToUInt16() < 0x60))
            {
                // Address is in the form [FSR2]+offset.
                Identifier fsr2 = binder.EnsureSequence(Registers.fsr2h, Registers.fsr2l, PrimitiveType.Ptr16);
                return m.IAdd(fsr2, offset);
            }
            else
            {
                if (mem.IsAccessRAM.ToBoolean())
                {
                    // Address is ACCESS Bank addressing.
                    int physAddr = arch.MemoryMapper.RemapDataAddr(offset.ToUInt16());
                    return Constant.Create(PrimitiveType.Ptr16, physAddr);
                }
                else
                {
                    // Address is BSR direct addressing.
                    return m.IAdd(m.IMul(binder.EnsureRegister(Registers.bsr), 256), offset);
                }
            }
        }

        private void RewriteADDFSR()
        {
            var sfr = RewriteOp(instr.op1);
            var k = RewriteOp(instr.op2);
            m.Assign(sfr, m.IAdd(sfr, k));
        }

        private void RewriteADDLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.IAdd(w, k));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(w));
        }

        private void RewriteADDWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(w, src));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteADDWFC()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(m.IAdd(w, src), FlagGroup(FlagM.C)));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteANDLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.And(w, k));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteANDWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.And(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteBRA()
        {
            rtlc = RtlClass.Transfer;
            var target = ((PIC18ProgRel11AddrOperand)instr.op1).CodeTarget;
            m.Goto(target.Address);
        }

        private void RewriteBC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.ULT, FlagGroup(FlagM.C)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op2).BitNumber.ToByte(), 1);
            m.Assign(dst, m.And(src, m.Comp(bit)));
        }

        private void RewriteBN()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.LT, FlagGroup(FlagM.N)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBNC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.UGE, FlagGroup(FlagM.C)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBNN()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.GE, FlagGroup(FlagM.N)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBNOV()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.NO, FlagGroup(FlagM.OV)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBOV()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.OV, FlagGroup(FlagM.OV)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.NE, FlagGroup(FlagM.Z)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteBSF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var b = RewriteOp(instr.op2);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op2).BitNumber.ToByte(), 1);
            m.Assign(dst, m.Or(src, bit));
        }

        private void RewriteBTFSC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op2).BitNumber.ToByte(), 1);
            m.Branch(m.Eq(bit, Constant.False()), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteBTFSS()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op2).BitNumber.ToByte(), 1);
            m.Branch(m.Eq(bit, Constant.True()), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteBTG()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op2).BitNumber.ToByte(), 1);
            m.Assign(dst, m.Xor(src, bit));
        }

        private void RewriteBZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)), target.Address, RtlClass.ConditionalTransfer);
        }

        private void RewriteCALL()
        {
            //TODO: See TOS update, shadowing of WREG,STATUS,BSR
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var target = RewriteOp(instr.op1);
            m.Call(target, 3);
        }

        private void RewriteCALLW()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var w = binder.EnsureRegister(Registers.wreg);
            var target = m.IAdd(w, instr.Address);
            m.Call(target, 3);
        }

        private void RewriteCLRF()
        {
            var dst = RewriteOp(instr.op1);
            m.Assign(dst, Constant.Zero(PrimitiveType.Byte));
            m.Assign(FlagGroup(FlagM.Z), Constant.True());
        }

        private void RewriteCLRWDT()
        {
            m.Assign(FlagGroup(FlagM.TO | FlagM.PD), Constant.True());
        }

        private void RewriteCOMF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Comp(src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteCPFSEQ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var w = binder.EnsureRegister(Registers.wreg);
            m.Branch(m.Eq(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteCPFSGT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var w = binder.EnsureRegister(Registers.wreg);
            m.Branch(m.Ugt(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteCPFSLT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var w = binder.EnsureRegister(Registers.wreg);
            m.Branch(m.Ult(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteDECF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, 1));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteDECFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, 1));
            m.Branch(m.Eq0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteDCFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, 1));
            m.Branch(m.Ne0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteGOTO()
        {

            rtlc = RtlClass.Transfer;
            var target = RewriteOp(instr.op1);
            m.Goto(target);
        }

        private void RewriteINCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(src, 1));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteINCFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(src, 1));
            m.Branch(m.Eq0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteINFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(src, 1));
            m.Branch(m.Ne0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteIORLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.Or(w, k));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteIORWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Or(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteMOVLB()
        {
            var bsr = binder.EnsureRegister(Registers.bsr);
            var k = RewriteOp(instr.op1);
            m.Assign(bsr, k);
        }

        private void RewriteMOVLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            m.Assign(w, k);
        }

        private void RewriteMOVWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var dst = RewriteOp(instr.op1);
            m.Assign(dst, w);
        }

        private void RewriteMULLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            var prod = binder.EnsureSequence(Registers.prodh, Registers.prodl, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(w, k));
        }

        private void RewriteMULWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var prod = binder.EnsureSequence(Registers.prodh, Registers.prodl, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(src, w));
        }

        private void RewriteNEGF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Neg(src));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteRLCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var carry = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            m.Fn(new PseudoProcedure("__rlcf", src.DataType, 2), src, carry);
            m.Assign(dst, src);
            m.Assign(carry, m.Cond(dst));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteRLNCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            m.Fn(new PseudoProcedure("__rlncf", src.DataType, 1), src);
            m.Assign(dst, src);
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteRRCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var carry = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            m.Fn(new PseudoProcedure("__rrcf", src.DataType, 2), src, carry);
            m.Assign(dst, src);
            m.Assign(carry, m.Cond(dst));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteRRNCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            m.Fn(new PseudoProcedure("__rrncf", src.DataType, 1), src);
            m.Assign(dst, src);
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteSETF()
        {
            var dst = RewriteOp(instr.op1);
            m.Assign(dst, Constant.Byte(0xFF));
        }

        private void RewriteSLEEP()
        {
            m.Assign(FlagGroup(FlagM.TO), m.Cond(Constant.True()));
            m.Assign(FlagGroup(FlagM.PD), m.Cond(Constant.False()));
        }

        private void RewriteSUBFSR()
        {
            var fsr = RewriteOp(instr.op1);
            var k = RewriteOp(instr.op2);
            m.Assign(fsr, m.ISub(fsr, k));
        }

        private void RewriteSUBFWB()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(w, src), m.Not(FlagGroup(FlagM.C))));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteSUBLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.ISub(k, w));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(w));
        }

        private void RewriteSUBWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, w));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteSUBWFB()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(src, w), m.Not(FlagGroup(FlagM.C))));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteTSTFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            m.Branch(m.Eq0(src), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteXORLW()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(w, m.Xor(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteXORWF()
        {
            var w = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Xor(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

    }

}
