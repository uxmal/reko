#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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
            this.state = state;
            this.binder = binder;
            this.host = host;
            disasm = new PIC18Disassembler(arch, rdr);
            dasm = disasm.GetEnumerator();
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
                    case Opcode.ADDULNK: RewriteADDULNK(); break;
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
                    case Opcode.DAW: RewriteDAW(); break;
                    case Opcode.DCFSNZ: RewriteDCFSNZ(); break;
                    case Opcode.DECF: RewriteDECF(); break;
                    case Opcode.DECFSZ: RewriteDECFSZ(); break;
                    case Opcode.GOTO: RewriteGOTO(); break;
                    case Opcode.INCF: RewriteINCF(); break;
                    case Opcode.INCFSZ: RewriteINCFSZ(); break;
                    case Opcode.INFSNZ: RewriteINFSNZ(); break;
                    case Opcode.IORLW: RewriteIORLW(); break;
                    case Opcode.IORWF: RewriteIORWF(); break;
                    case Opcode.LFSR: RewriteLFSR(); break;
                    case Opcode.MOVF: RewriteMOVF(); break;
                    case Opcode.MOVFF: RewriteMOVFF(); break;
                    case Opcode.MOVFFL: RewriteMOVFFL(); break;
                    case Opcode.MOVLB: RewriteMOVLB(); break;
                    case Opcode.MOVLW: RewriteMOVLW(); break;
                    case Opcode.MOVSF: RewriteMOVSF(); break;
                    case Opcode.MOVSFL: RewriteMOVSFL(); break;
                    case Opcode.MOVSS: RewriteMOVSS(); break;
                    case Opcode.MOVWF: RewriteMOVWF(); break;
                    case Opcode.MULLW: RewriteMULLW(); break;
                    case Opcode.MULWF: RewriteMULWF(); break;
                    case Opcode.NEGF: RewriteNEGF(); break;
                    case Opcode.NOP: m.Nop(); break;
                    case Opcode.POP: RewritePOP(); break;
                    case Opcode.PUSH: RewritePUSH(); break;
                    case Opcode.PUSHL: RewritePUSHL(); break;
                    case Opcode.RCALL: RewriteRCALL(); break;
                    case Opcode.RESET: RewriteRESET(); break;
                    case Opcode.RETFIE: RewriteRETFIE(); break;
                    case Opcode.RETLW: RewriteRETLW(); break;
                    case Opcode.RETURN: RewriteRETURN(); break;
                    case Opcode.RLCF: RewriteRLCF(); break;
                    case Opcode.RLNCF: RewriteRLNCF(); break;
                    case Opcode.RRCF: RewriteRRCF(); break;
                    case Opcode.RRNCF: RewriteRRNCF(); break;
                    case Opcode.SETF: RewriteSETF(); break;
                    case Opcode.SLEEP: RewriteSLEEP(); break;
                    case Opcode.SUBFSR: RewriteSUBFSR(); break;
                    case Opcode.SUBFWB: RewriteSUBFWB(); break;
                    case Opcode.SUBLW: RewriteSUBLW(); break;
                    case Opcode.SUBULNK: RewriteSUBULNK(); break;
                    case Opcode.SUBWF: RewriteSUBWF(); break;
                    case Opcode.SUBWFB: RewriteSUBWFB(); break;
                    case Opcode.SWAPF: RewriteSWAPF(); break;
                    case Opcode.TBLRD: RewriteTBLRD(); break;
                    case Opcode.TBLWT: RewriteTBLWT(); break;
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
            return binder.EnsureFlagGroup(PIC18Registers.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        #region Helpers

        private Expression RewriteOp(MachineOperand op)
        {
            var imm = op as PIC18ImmediateOperand;
            if (imm != null)
                return imm.ImmediateValue;

            var paddr = op as PIC18ProgAddrOperand;
            if (paddr != null)
                return paddr.CodeTarget;

            var bankaccess = op as PIC18BankedAccessOperand;
            if (bankaccess != null)
                return GetMemoryBankAccess(bankaccess);

            var fsridxOp = op as PIC18FSR2IdxOperand;
            if (fsridxOp != null)
            {
                var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.Ptr16);
                return m.Mem8(m.IAdd(fsr2, fsridxOp.Offset));
            }

            var fsrnumOp = op as PIC18FSRNumOperand;
            if (fsrnumOp != null)
            {
                Identifier fsr;
                switch (fsrnumOp.FSRNum.ToByte())
                {
                    case 0:
                        fsr = binder.EnsureSequence(PIC18Registers.FSR0H, PIC18Registers.FSR0L, PrimitiveType.Ptr16);
                        break;
                    case 1:
                        fsr = binder.EnsureSequence(PIC18Registers.FSR1H, PIC18Registers.FSR1L, PrimitiveType.Ptr16);
                        break;
                    case 2:
                        fsr = binder.EnsureSequence(PIC18Registers.FSR2H, PIC18Registers.FSR2L, PrimitiveType.Ptr16);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid FSR number: {fsrnumOp.FSRNum.ToByte()}");
                }
                return fsr;
            }

            var mem12bitaddr = op as PIC18Data12bitAbsAddrOperand;
            if (mem12bitaddr != null)
                return mem12bitaddr.DataTarget;

            var mem14bitaddr = op as PIC18Data14bitAbsAddrOperand;
            if (mem14bitaddr != null)
                return mem14bitaddr.DataTarget;

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

        private Expression RewriteDestOp(MachineOperand op)
        {
            Expression dst = null;
            var dstop = op as PIC18DataByteAccessWithDestOperand;
            if (dstop != null)
            {
                dst = (dstop.WregIsDest.ToBoolean() ? binder.EnsureRegister(PIC18Registers.WREG) : GetMemoryBankAccess(dstop));
            }
            return dst;
        }

        private Expression GetMemoryBankAccess(PIC18BankedAccessOperand mem)
        {
            var offset = mem.BankAddr;
            if (!mem.IsAccessRAM.ToBoolean())
            {
                // Address is BSR direct addressing.
                Identifier bsr = binder.EnsureRegister(PIC18Registers.BSR);
                return m.Mem8(m.IAdd(m.Shl(bsr, 8), offset));
            }

            // We have some sort of Access RAM bank access; either Lower or Upper area.
            //
            if (PIC18MemoryMapper.BelongsToAccessRAMLow(offset))
            {
                if (mem.ExecMode == PICExecMode.Traditional)
                {
                    return m.Mem8(offset);
                }
                // Address is in the form [FSR2]+offset.
                Identifier fsr2 = binder.EnsureSequence(PIC18Registers.FSR2H, PIC18Registers.FSR2L, PrimitiveType.Ptr16);
                return m.Mem8(m.IAdd(fsr2, offset));
            }

            // Address is Upper ACCESS Bank addressing. Try to get any "known" SFR for this PIC.
            // 
            var accAddr = PIC18MemoryMapper.TranslateAccessAddress(offset);
            var sfr = PIC18Registers.GetRegister(accAddr);
            if (sfr != RegisterStorage.None)
                return binder.EnsureRegister(sfr);
            return m.Mem8(accAddr);
        }

        #endregion

        #region Rewrite methods

        private void RewriteADDFSR()
        {
            var fsr = RewriteOp(instr.op1);
            var k = RewriteOp(instr.op2);
            m.Assign(fsr, m.IAdd(fsr, k));
        }

        private void RewriteADDLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.IAdd(w, k));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(w));
        }

        private void RewriteADDULNK()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var k = RewriteOp(instr.op1);
            m.Assign(fsr2, m.IAdd(fsr2, k));
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteADDWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(w, src));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteADDWFC()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(m.IAdd(w, src), FlagGroup(FlagM.C)));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteANDLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.And(w, k));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteANDWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.And(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteBRA()
        {
            rtlc = RtlClass.Transfer;
            var target = ((PIC18ProgRel11AddrOperand)instr.op1).CodeTarget;
            m.Goto(target);
        }

        private void RewriteBC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.ULT, FlagGroup(FlagM.C)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var bit = m.Slice(src, ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte(), 1);
            m.Assign(dst, m.Dpb(src, Constant.Bool(false), ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte()));
        }

        private void RewriteBN()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.LT, FlagGroup(FlagM.N)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBNC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.UGE, FlagGroup(FlagM.C)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBNN()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.GE, FlagGroup(FlagM.N)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBNOV()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.NO, FlagGroup(FlagM.OV)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBOV()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.OV, FlagGroup(FlagM.OV)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var target = ((PIC18ProgRel8AddrOperand)instr.op1).CodeTarget;
            m.Branch(m.Test(ConditionCode.NE, FlagGroup(FlagM.Z)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteBSF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var b = RewriteOp(instr.op2);
            m.Assign(dst, m.Dpb(src, Constant.Bool(true), ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte()));
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
            m.Branch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)), Address.Ptr32(target.ToUInt32()), RtlClass.ConditionalTransfer);
        }

        private void RewriteCALL()
        {
            //TODO: See TOS/stack update, shadowing of WREG,STATUS,BSR
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var target = RewriteOp(instr.op1);
            m.Call(target, 1);
        }

        private void RewriteCALLW()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var pclath = binder.EnsureRegister(PIC18Registers.PCLATH);
            var pclatu = binder.EnsureRegister(PIC18Registers.PCLATU);
            var target = m.Fn(host.PseudoProcedure("__callw", VoidType.Instance, w, pclath, pclatu));
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
            var pd = PIC18Registers.PD;
            if (pd != null)
            {
                var pdreg = binder.EnsureRegister(pd.FlagRegister);
                m.Assign(pdreg, m.Dpb(pdreg, Constant.True(), pd.BitPos));
            }
            var to = PIC18Registers.TO;
            if (to != null)
            {
                var toreg = binder.EnsureRegister(to.FlagRegister);
                m.Assign(toreg, m.Dpb(toreg, Constant.True(), to.BitPos));
            }
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
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Eq(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteCPFSGT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Ugt(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteCPFSLT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Ult(src, w), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteDAW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var C = FlagGroup(FlagM.C);
            var DC = FlagGroup(FlagM.DC);
            Expression res = m.Fn(host.PseudoProcedure("__daw", PrimitiveType.Byte, w, C, DC));
            m.Assign(w, res);
            m.Assign(FlagGroup(FlagM.C), m.Cond(res));
        }

        private void RewriteDECF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteDECFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
            m.Branch(m.Eq0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteDCFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
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
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteINCFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            m.Branch(m.Eq0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteINFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            m.Branch(m.Ne0(dst), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteIORLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.Or(w, k));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteIORWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Or(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteLFSR()
        {
            var sfr = RewriteOp(instr.op1);
            var k = RewriteOp(instr.op2);
            m.Assign(sfr, k);
        }

        private void RewriteMOVF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, src);
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteMOVFF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op2);
            m.Assign(dst, src);
        }

        private void RewriteMOVFFL()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op2);
            m.Assign(dst, src);
        }

        private void RewriteMOVLB()
        {
            var bsr = binder.EnsureRegister(PIC18Registers.BSR);
            var k = RewriteOp(instr.op1);
            m.Assign(bsr, k);
        }

        private void RewriteMOVLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, k);
        }

        private void RewriteMOVSF()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteOp(instr.op1);
            var fd = RewriteOp(instr.op2);
            m.Assign(fd, m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVSFL()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteOp(instr.op1);
            var fd = RewriteOp(instr.op2);
            m.Assign(fd, m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVSS()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteOp(instr.op1);
            var zd = RewriteOp(instr.op2);
            m.Assign(m.Deref(m.IAdd(fsr2, zd)), m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var dst = RewriteOp(instr.op1);
            m.Assign(dst, w);
        }

        private void RewriteMULLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            var prod = binder.EnsureSequence(PIC18Registers.PRODH, PIC18Registers.PRODL, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(w, k));
        }

        private void RewriteMULWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var prod = binder.EnsureSequence(PIC18Registers.PRODH, PIC18Registers.PRODL, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(src, w));
        }

        private void RewriteNEGF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Neg(src));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewritePOP()
        {
            var stkptr = binder.EnsureRegister(PIC18Registers.STKPTR);
            m.Assign(stkptr, m.ISub(stkptr, Constant.Byte(1)));

            //TODO: See TOS update
            var tos = binder.EnsureSequence(PIC18Registers.TOSL, PIC18Registers.TOSU, PrimitiveType.Word32);
        }

        private void RewritePUSH()
        {
            var stkptr = binder.EnsureRegister(PIC18Registers.STKPTR);
            m.Assign(stkptr, m.IAdd(stkptr, Constant.Byte(1)));

            //TODO: see TOS update
            var tos = binder.EnsureSequence(PIC18Registers.TOSL, PIC18Registers.TOSU, PrimitiveType.Word32);
        }

        private void RewritePUSHL()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var k = RewriteOp(instr.op1);
            m.Assign(m.Deref(fsr2), k);
            m.Assign(fsr2, m.IAdd(fsr2, Constant.UInt16(1)));
        }

        private void RewriteRCALL()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var target = RewriteOp(instr.op1);
            m.Call(target, 1);
        }

        private void RewriteRESET()
        {
            var stkptr = binder.EnsureRegister(PIC18Registers.STKPTR);
            m.Assign(stkptr, Constant.Byte(0));
            rtlc = RtlClass.Terminates;
            m.SideEffect(host.PseudoProcedure("__reset", VoidType.Instance));
        }

        private void RewriteRETFIE()
        {
            var tos = binder.EnsureSequence(PIC18Registers.TOSL, PIC18Registers.TOSU, PrimitiveType.Word32);
            //TODO: See TOS restore and stack update
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteRETLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, k);
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteRETURN()
        {
            //TODO: shadow WREG, BSR, STATUS
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteRLCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var C = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            var tmp = m.Fn(host.PseudoProcedure("__rlcf", src.DataType, src, C));
            m.Assign(dst, tmp);
            m.Assign(C, m.Cond(tmp));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteRLNCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            var tmp = m.Fn(host.PseudoProcedure("__rlncf", src.DataType, src));
            m.Assign(dst, tmp);
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(tmp));
        }

        private void RewriteRRCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            var C = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            var tmp = m.Fn(host.PseudoProcedure("__rrcf", src.DataType, src, C));
            m.Assign(dst, tmp);
            m.Assign(C, m.Cond(tmp));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteRRNCF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            var tmp = m.Fn(host.PseudoProcedure("__rrncf", src.DataType, src));
            m.Assign(dst, tmp);
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        private void RewriteSETF()
        {
            var dst = RewriteOp(instr.op1);
            m.Assign(dst, Constant.Byte(0xFF));
        }

        private void RewriteSLEEP()
        {
            var pd = PIC18Registers.PD;
            if (pd != null)
            {
                var pdreg = binder.EnsureRegister(pd.FlagRegister);
                m.Assign(pdreg, m.Dpb(pdreg, Constant.False(), pd.BitPos));
            }
            var to = PIC18Registers.TO;
            if (to != null)
            {
                var toreg = binder.EnsureRegister(to.FlagRegister);
                m.Assign(toreg, m.Dpb(toreg, Constant.True(), to.BitPos));
            }
        }

        private void RewriteSUBFSR()
        {
            var fsr = RewriteOp(instr.op1);
            var k = RewriteOp(instr.op2);
            m.Assign(fsr, m.ISub(fsr, k));
        }

        private void RewriteSUBFWB()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(w, src), m.Not(FlagGroup(FlagM.C))));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteSUBLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteOp(instr.op1);
            m.Assign(w, m.ISub(k, w));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(w));
        }

        private void RewriteSUBULNK()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var k = RewriteOp(instr.op1);
            m.Assign(fsr2, m.ISub(fsr2, k));
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteSUBWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(src, w));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteSUBWFB()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(src, w), m.Not(FlagGroup(FlagM.C))));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(dst));
        }

        private void RewriteSWAPF()
        {
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            dst = m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, src));
        }

        private void RewriteTBLRD()
        {
            var tblmode = ((PIC18TableReadWriteOperand)instr.op1).TBLIncrMode;
            m.SideEffect(host.PseudoProcedure("__tblrd", VoidType.Instance, tblmode));
        }

        private void RewriteTBLWT()
        {
            var tblmode = ((PIC18TableReadWriteOperand)instr.op1).TBLIncrMode;
            m.SideEffect(host.PseudoProcedure("__tblwt", VoidType.Instance, tblmode));
        }

        private void RewriteTSTFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.op1);
            m.Branch(m.Eq0(src), instr.Address + 2, RtlClass.ConditionalTransfer);
        }

        private void RewriteXORLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            m.Assign(w, m.Xor(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(w));
        }

        private void RewriteXORWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteOp(instr.op1);
            var dst = RewriteDestOp(instr.op1);
            m.Assign(dst, m.Xor(w, src));
            m.Assign(FlagGroup(FlagM.Z | FlagM.N), m.Cond(dst));
        }

        #endregion

    }

}
