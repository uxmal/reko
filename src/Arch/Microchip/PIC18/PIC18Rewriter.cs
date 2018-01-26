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
using Reko.Arch.Microchip.Common;

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
                    case Opcode.BTFSS: RewriteBTFSS(); break;
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

        private Expression RewriteSrcOp(MachineOperand op)
        {
            var imm = op as PIC18ImmediateOperand;
            if (imm != null)
                return imm.ImmediateValue;

            var paddr = op as PIC18ProgAddrOperand;
            if (paddr != null)
                return PICProgAddress.Ptr(paddr.CodeTarget);

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

            var memabsaddr = op as PIC18DataAbsAddrOperand;
            if (memabsaddr != null)
                return GetMemoryAbsAccess(memabsaddr);

            var fsr2idx = op as PIC18FSR2IdxOperand;
            if (fsr2idx != null)
                return fsr2idx.Offset;

            var shadow = op as PIC18ShadowOperand;
            if (shadow != null)
                return shadow.IsShadow;

            var tblincrmod = op as PIC18TableReadWriteOperand;
            if (tblincrmod != null)
                return tblincrmod.TBLIncrMode;

            throw new NotImplementedException($"Rewriting of PIC18 source operand type {op.GetType().FullName} is not implemented yet.");
        }

        private Expression RewriteDstOp(MachineOperand op)
        {
            Expression dst = null;
            var dstop = op as PIC18DataByteAccessWithDestOperand;
            if (dstop != null)
            {
                dst = (dstop.WregIsDest.ToBoolean() ? binder.EnsureRegister(PIC18Registers.WREG) : GetMemoryBankAccess(dstop));
                return dst;
            }
            var dstmem = op as PIC18BankedAccessOperand;
            if (dstmem != null)
                return GetMemoryBankAccess(dstmem);

            var memabsaddr = op as PIC18DataAbsAddrOperand;
            if (memabsaddr != null)
                return GetMemoryAbsAccess(memabsaddr);

            throw new NotImplementedException($"Rewriting of PIC18 destination operand type {op.GetType().FullName} is not implemented yet.");
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

        private Expression GetMemoryAbsAccess(PIC18DataAbsAddrOperand mem)
        {
            var sfr = PIC18Registers.GetRegister(mem.DataTarget);
            if (sfr != RegisterStorage.None)
                return binder.EnsureRegister(sfr);
            return m.Mem8(Address.Ptr16(mem.DataTarget));
        }

        #endregion

        #region Rewrite methods

        #region Instructions Rewriter Helpers

        private void _rewriteCondBranch(TestCondition test)
        {
            rtlc = RtlClass.ConditionalTransfer;
            var brop = instr.op1 as PIC18ProgRel8AddrOperand;
            if (brop == null)
                throw new InvalidOperationException("Wrong 8-bit relative PIC address");
            m.Branch(test, PICProgAddress.Ptr( brop.CodeTarget), rtlc);
        }

        private void _setStatusFlags(Expression dst)
        {
            FlagM flags = PIC18Instruction.DefCc(instr.Opcode);
            if (flags != 0)
                m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        private PICProgAddress _skipaddr()
            => PICProgAddress.Ptr(instr.Address + 4);

        #endregion

        private void RewriteADDFSR()
        {
            var fsr = RewriteSrcOp(instr.op1);
            var k = RewriteSrcOp(instr.op2);
            m.Assign(fsr, m.IAdd(fsr, k));
            _setStatusFlags(fsr);
        }

        private void RewriteADDLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(w, m.IAdd(w, k));
            _setStatusFlags(w);
        }

        private void RewriteADDULNK()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(fsr2, m.IAdd(fsr2, k));
            _setStatusFlags(fsr2);
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteADDWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.IAdd(w, src));
            _setStatusFlags(dst);
        }

        private void RewriteADDWFC()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.IAdd(m.IAdd(w, src), FlagGroup(FlagM.C)));
            _setStatusFlags(dst);
        }

        private void RewriteANDLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(w, m.And(w, k));
            _setStatusFlags(w);
        }

        private void RewriteANDWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.And(w, src));
            _setStatusFlags(dst);
        }

        private void RewriteBC()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.ULT, FlagGroup(FlagM.C)));
        }

        private void RewriteBCF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            int mask = ~(1 << ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte());
            m.Assign(dst, m.And(src, Constant.Byte((byte)mask)));
            _setStatusFlags(dst);
        }

        private void RewriteBN()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.LT, FlagGroup(FlagM.N)));
        }

        private void RewriteBNC()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.UGE, FlagGroup(FlagM.C)));
        }

        private void RewriteBNN()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.GE, FlagGroup(FlagM.N)));
        }

        private void RewriteBNOV()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.NO, FlagGroup(FlagM.OV)));
        }

        private void RewriteBOV()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.OV, FlagGroup(FlagM.OV)));
        }

        private void RewriteBNZ()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.NE, FlagGroup(FlagM.Z)));
        }

        private void RewriteBRA()
        {
            rtlc = RtlClass.Transfer;
            var brop = instr.op1 as PIC18ProgRel11AddrOperand;
            if (brop == null)
                throw new InvalidOperationException("Wrong 11-bit relative PIC address");
            m.Goto(PICProgAddress.Ptr(brop.CodeTarget));
        }

        private void RewriteBSF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            byte mask = (byte)(1 << ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte());
            m.Assign(dst, m.Or(src, mask));
        }

        private void RewriteBTFSC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            byte mask = (byte)(1 << ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte());
            m.Branch(m.Eq(m.And(src, mask), Constant.Zero(src.DataType)), _skipaddr(), rtlc);
        }

        private void RewriteBTFSS()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            byte mask = (byte)(1 << ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte());
            m.Branch(m.Ne(m.And(src, mask), Constant.Zero(src.DataType)), _skipaddr(), rtlc);
        }

        private void RewriteBTG()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            byte mask = (byte)(1 << ((PIC18DataBitAccessOperand)instr.op1).BitNumber.ToByte());
            m.Assign(dst, m.Xor(src, mask));
        }

        private void RewriteBZ()
        {
            // TODO: review RTL to use for flag test.
            _rewriteCondBranch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)));
        }

        private void RewriteCALL()
        {
            //TODO: See TOS/stack update, shadowing of WREG,STATUS,BSR
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var target = RewriteSrcOp(instr.op1);
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
            var dst = RewriteSrcOp(instr.op1);
            m.Assign(dst, Constant.Zero(PrimitiveType.Byte));
            m.Assign(binder.EnsureFlagGroup(PIC18Registers.Z), Constant.Bool(true));
        }

        private void RewriteCLRWDT()
        {
            byte mask;

            PICBitFieldStorage pd = PIC18Registers.PD;
            PICBitFieldStorage to = PIC18Registers.TO;
            Identifier pdreg = binder.EnsureRegister(pd.FlagRegister);
            Identifier toreg = binder.EnsureRegister(to.FlagRegister);

            if (ReferenceEquals(pdreg, toreg) && pdreg != null)
            {
                mask = (byte)((1 << pd.BitPos) | (1 << to.BitPos));
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
                return;
            }
            if (pdreg != null)
            {
                mask = (byte)((1 << pd.BitPos));
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
            }
            if (toreg != null)
            {
                mask = (byte)((1 << to.BitPos));
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
            }
        }

        private void RewriteCOMF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.Comp(src));
            _setStatusFlags(dst);
        }

        private void RewriteCPFSEQ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Eq(src, w), _skipaddr(), rtlc);
        }

        private void RewriteCPFSGT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Ugt(src, w), _skipaddr(), rtlc);
        }

        private void RewriteCPFSLT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            m.Branch(m.Ult(src, w), _skipaddr(), rtlc);
        }

        private void RewriteDAW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var C = FlagGroup(FlagM.C);
            var DC = FlagGroup(FlagM.DC);
            Expression res = m.Fn(host.PseudoProcedure("__daw", PrimitiveType.Byte, w, C, DC));
            m.Assign(w, res);
            _setStatusFlags(w);
        }

        private void RewriteDECF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
            _setStatusFlags(dst);
        }

        private void RewriteDECFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
            _setStatusFlags(dst);
            m.Branch(m.Eq0(dst), _skipaddr(), RtlClass.ConditionalTransfer);
        }

        private void RewriteDCFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(src, Constant.Byte(1)));
            _setStatusFlags(dst);
            m.Branch(m.Ne0(dst), _skipaddr(), RtlClass.ConditionalTransfer);
        }

        private void RewriteGOTO()
        {

            rtlc = RtlClass.Transfer;
            var target = RewriteSrcOp(instr.op1);
            m.Goto(target);
        }

        private void RewriteINCF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            _setStatusFlags(dst);
        }

        private void RewriteINCFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            _setStatusFlags(dst);
            m.Branch(m.Eq0(dst), _skipaddr(), RtlClass.ConditionalTransfer);
        }

        private void RewriteINFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.IAdd(src, Constant.Byte(1)));
            _setStatusFlags(dst);
            m.Branch(m.Ne0(dst), _skipaddr(), RtlClass.ConditionalTransfer);
        }

        private void RewriteIORLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(w, m.Or(w, k));
            _setStatusFlags(w);
        }

        private void RewriteIORWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.Or(w, src));
            _setStatusFlags(dst);
        }

        private void RewriteLFSR()
        {
            var sfrN = RewriteSrcOp(instr.op1);
            var k = RewriteSrcOp(instr.op2);
            m.Assign(sfrN, k);
            _setStatusFlags(sfrN);
        }

        private void RewriteMOVF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, src);
            _setStatusFlags(dst);
        }

        private void RewriteMOVFF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op2);
            m.Assign(dst, src);
            _setStatusFlags(dst);
        }

        private void RewriteMOVFFL()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op2);
            m.Assign(dst, src);
            _setStatusFlags(dst);
        }

        private void RewriteMOVLB()
        {
            var bsr = binder.EnsureRegister(PIC18Registers.BSR);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(bsr, k);
            _setStatusFlags(bsr);
        }

        private void RewriteMOVLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(w, k);
            _setStatusFlags(w);
        }

        private void RewriteMOVSF()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteSrcOp(instr.op1);
            var fd = RewriteSrcOp(instr.op2);
            m.Assign(fd, m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVSFL()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteSrcOp(instr.op1);
            var fd = RewriteSrcOp(instr.op2);
            m.Assign(fd, m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVSS()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var zs = RewriteSrcOp(instr.op1);
            var zd = RewriteSrcOp(instr.op2);
            m.Assign(m.Deref(m.IAdd(fsr2, zd)), m.Mem8(m.IAdd(fsr2, zs)));
        }

        private void RewriteMOVWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var dst = RewriteSrcOp(instr.op1);
            m.Assign(dst, w);
            _setStatusFlags(dst);
        }

        private void RewriteMULLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            var prod = binder.EnsureSequence(PIC18Registers.PRODH, PIC18Registers.PRODL, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(w, k));
        }

        private void RewriteMULWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var prod = binder.EnsureSequence(PIC18Registers.PRODH, PIC18Registers.PRODL, PrimitiveType.Word16);
            m.Assign(prod, m.UMul(src, w));
        }

        private void RewriteNEGF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.Neg(src));
            _setStatusFlags(dst);
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
            var k = RewriteSrcOp(instr.op1);
            m.Assign(m.Deref(fsr2), k);
            m.Assign(fsr2, m.IAdd(fsr2, Constant.UInt16(1)));
            _setStatusFlags(fsr2);
        }

        private void RewriteRCALL()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var target = RewriteSrcOp(instr.op1);
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
            var k = RewriteSrcOp(instr.op1);
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
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            var C = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            m.Assign(dst, m.Fn(host.PseudoProcedure("__rlcf", src.DataType, src, C)));
            _setStatusFlags(dst);
        }

        private void RewriteRLNCF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            m.Assign(dst, m.Fn(host.PseudoProcedure("__rlncf", src.DataType, src)));
            _setStatusFlags(dst);
        }

        private void RewriteRRCF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            var C = FlagGroup(FlagM.C);
            //TODO:  PseudoProcedure(?)
            m.Assign(dst, m.Fn(host.PseudoProcedure("__rrcf", src.DataType, src, C)));
            _setStatusFlags(dst);
        }

        private void RewriteRRNCF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            //TODO:  PseudoProcedure(?)
            m.Assign(dst, m.Fn(host.PseudoProcedure("__rrncf", src.DataType, src)));
            _setStatusFlags(dst);
        }

        private void RewriteSETF()
        {
            var dst = RewriteSrcOp(instr.op1);
            m.Assign(dst, Constant.Byte(0xFF));
            _setStatusFlags(dst);
        }

        private void RewriteSLEEP()
        {
            byte mask;

            PICBitFieldStorage pd = PIC18Registers.PD;
            PICBitFieldStorage to = PIC18Registers.TO;
            Identifier pdreg = binder.EnsureRegister(pd.FlagRegister);
            Identifier toreg = binder.EnsureRegister(to.FlagRegister);

            if (ReferenceEquals(pdreg, toreg) && pdreg != null)
            {
                mask = (byte)(~(1 << pd.BitPos));
                m.Assign(pdreg, m.And(pdreg, Constant.Byte(mask)));
                mask = (byte)(1 << to.BitPos);
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
                return;
            }
            if (pd != null)
            {
                m.Assign(pdreg, m.Dpb(pdreg, Constant.False(), pd.BitPos));
            }
            if (to != null)
            {
                m.Assign(toreg, m.Dpb(toreg, Constant.True(), to.BitPos));
            }
        }

        private void RewriteSUBFSR()
        {
            var fsr = RewriteSrcOp(instr.op1);
            var k = RewriteSrcOp(instr.op2);
            m.Assign(fsr, m.ISub(fsr, k));
            _setStatusFlags(fsr);
        }

        private void RewriteSUBFWB()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(w, src), m.Not(FlagGroup(FlagM.C))));
            _setStatusFlags(dst);
        }

        private void RewriteSUBLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(w, m.ISub(k, w));
            m.Assign(FlagGroup(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N), m.Cond(w));
        }

        private void RewriteSUBULNK()
        {
            var fsr2 = binder.EnsureSequence(PIC18Registers.FSR2L, PIC18Registers.FSR2H, PrimitiveType.UInt16);
            var k = RewriteSrcOp(instr.op1);
            m.Assign(fsr2, m.ISub(fsr2, k));
            _setStatusFlags(fsr2);
            rtlc = RtlClass.Transfer;
            m.Return(1, 0);
        }

        private void RewriteSUBWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(src, w));
            _setStatusFlags(dst);
        }

        private void RewriteSUBWFB()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.ISub(m.ISub(src, w), m.Not(FlagGroup(FlagM.C))));
            _setStatusFlags(dst);
        }

        private void RewriteSWAPF()
        {
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, src)));
            _setStatusFlags(dst);
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
            var src = RewriteSrcOp(instr.op1);
            m.Branch(m.Eq0(src), _skipaddr(), RtlClass.ConditionalTransfer);
        }

        private void RewriteXORLW()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            m.Assign(w, m.Xor(w, src));
            _setStatusFlags(w);
        }

        private void RewriteXORWF()
        {
            var w = binder.EnsureRegister(PIC18Registers.WREG);
            var src = RewriteSrcOp(instr.op1);
            var dst = RewriteDstOp(instr.op1);
            m.Assign(dst, m.Xor(w, src));
            _setStatusFlags(dst);
        }

        #endregion

    }

}
