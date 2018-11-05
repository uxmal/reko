#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// The PIC18 instructions rewriter base. Must be inherited.
    /// </summary>
    public abstract class PIC18RewriterBase : PICRewriter
    {

        protected Identifier Fsr2;    // cached FSR2 register identifier

        protected PIC18RewriterBase(PICArchitecture arch, PICDisassemblerBase disasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, disasm, state, binder, host)
        {
            Fsr2 = binder.EnsureRegister(PIC18Registers.FSR2);
        }

        protected override void RewriteInstr()
        {
            var addr = instrCurr.Address;
            var len = instrCurr.Length;

            switch (instrCurr.Opcode)
            {
                default:
                    host.Warn(
                        instrCurr.Address,
                        $"PIC18 instruction '{instrCurr.Opcode}' is not supported yet.");
                goto case Opcode.invalid;
                case Opcode.invalid:
                case Opcode.unaligned:
                    m.Invalid();
                    break;
                case Opcode.ADDLW:
                    RewriteADDLW();
                    break;
                case Opcode.ADDWF:
                    RewriteADDWF();
                    break;
                case Opcode.ADDWFC:
                    RewriteADDWFC();
                    break;
                case Opcode.ANDLW:
                    RewriteANDLW();
                    break;
                case Opcode.ANDWF:
                    RewriteANDWF();
                    break;
                case Opcode.BC:
                    RewriteBC();
                    break;
                case Opcode.BCF:
                    RewriteBCF();
                    break;
                case Opcode.BN:
                    RewriteBN();
                    break;
                case Opcode.BNC:
                    RewriteBNC();
                    break;
                case Opcode.BNN:
                    RewriteBNN();
                    break;
                case Opcode.BNOV:
                    RewriteBNOV();
                    break;
                case Opcode.BNZ:
                    RewriteBNZ();
                    break;
                case Opcode.BOV:
                    RewriteBOV();
                    break;
                case Opcode.BRA:
                    RewriteBRA();
                    break;
                case Opcode.BSF:
                    RewriteBSF();
                    break;
                case Opcode.BTFSC:
                    RewriteBTFSC();
                    break;
                case Opcode.BTFSS:
                    RewriteBTFSS();
                    break;
                case Opcode.BTG:
                    RewriteBTG();
                    break;
                case Opcode.BZ:
                    RewriteBZ();
                    break;
                case Opcode.CALL:
                    RewriteCALL();
                    break;
                case Opcode.CLRF:
                    RewriteCLRF();
                    break;
                case Opcode.CLRWDT:
                    RewriteCLRWDT();
                    break;
                case Opcode.COMF:
                    RewriteCOMF();
                    break;
                case Opcode.CPFSEQ:
                    RewriteCPFSEQ();
                    break;
                case Opcode.CPFSGT:
                    RewriteCPFSGT();
                    break;
                case Opcode.CPFSLT:
                    RewriteCPFSLT();
                    break;
                case Opcode.DAW:
                    RewriteDAW();
                    break;
                case Opcode.DCFSNZ:
                    RewriteDCFSNZ();
                    break;
                case Opcode.DECF:
                    RewriteDECF();
                    break;
                case Opcode.DECFSZ:
                    RewriteDECFSZ();
                    break;
                case Opcode.GOTO:
                    RewriteGOTO();
                    break;
                case Opcode.INCF:
                    RewriteINCF();
                    break;
                case Opcode.INCFSZ:
                    RewriteINCFSZ();
                    break;
                case Opcode.INFSNZ:
                    RewriteINFSNZ();
                    break;
                case Opcode.IORLW:
                    RewriteIORLW();
                    break;
                case Opcode.IORWF:
                    RewriteIORWF();
                    break;
                case Opcode.LFSR:
                    RewriteLFSR();
                    break;
                case Opcode.MOVF:
                    RewriteMOVF();
                    break;
                case Opcode.MOVFF:
                    RewriteMOVFF();
                    break;
                case Opcode.MOVLB:
                    RewriteMOVLB();
                    break;
                case Opcode.MOVLW:
                    RewriteMOVLW();
                    break;
                case Opcode.MOVWF:
                    RewriteMOVWF();
                    break;
                case Opcode.MULLW:
                    RewriteMULLW();
                    break;
                case Opcode.MULWF:
                    RewriteMULWF();
                    break;
                case Opcode.NEGF:
                    RewriteNEGF();
                    break;
                case Opcode.NOP:
                    m.Nop();
                    break;
                case Opcode.POP:
                    RewritePOP();
                    break;
                case Opcode.PUSH:
                    RewritePUSH();
                    break;
                case Opcode.RCALL:
                    RewriteRCALL();
                    break;
                case Opcode.RESET:
                    RewriteRESET();
                    break;
                case Opcode.RETFIE:
                    RewriteRETFIE();
                    break;
                case Opcode.RETLW:
                    RewriteRETLW();
                    break;
                case Opcode.RETURN:
                    RewriteRETURN();
                    break;
                case Opcode.RLCF:
                    RewriteRLCF();
                    break;
                case Opcode.RLNCF:
                    RewriteRLNCF();
                    break;
                case Opcode.RRCF:
                    RewriteRRCF();
                    break;
                case Opcode.RRNCF:
                    RewriteRRNCF();
                    break;
                case Opcode.SETF:
                    RewriteSETF();
                    break;
                case Opcode.SLEEP:
                    RewriteSLEEP();
                    break;
                case Opcode.SUBFWB:
                    RewriteSUBFWB();
                    break;
                case Opcode.SUBLW:
                    RewriteSUBLW();
                    break;
                case Opcode.SUBWF:
                    RewriteSUBWF();
                    break;
                case Opcode.SUBWFB:
                    RewriteSUBWFB();
                    break;
                case Opcode.SWAPF:
                    RewriteSWAPF();
                    break;
                case Opcode.TBLRD:
                    RewriteTBLRD();
                    break;
                case Opcode.TBLWT:
                    RewriteTBLWT();
                    break;
                case Opcode.TSTFSZ:
                    RewriteTSTFSZ();
                    break;
                case Opcode.XORLW:
                    RewriteXORLW();
                    break;
                case Opcode.XORWF:
                    RewriteXORWF();
                    break;

                // Pseudo-instructions
                case Opcode.CONFIG:
                case Opcode.DA:
                case Opcode.DB:
                case Opcode.DE:
                case Opcode.DW:
                case Opcode.__IDLOCS:
                    m.Invalid();
                    break;
            }

        }


        #region Rewrite methods common to all PIC18 instruction-sets.

        protected override void SetStatusFlags(Expression dst)
        {
            FlagM flags = PIC18CC.Defined(instrCurr.Opcode);
            if (flags != 0)
                m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        protected override (FSRIndexedMode indMode, Expression memPtr) GetMemFileAccess(MachineOperand opernd)
        {
            if (opernd is PICOperandBankedMemory bankmemop && bankmemop.IsAccess)
            {
                var bankmem = PICMemoryDescriptor.CreateBankedAddr(bankmemop);
                if (PICMemoryDescriptor.CanBeFSR2IndexAddress(bankmem))
                {
                    return (FSRIndexedMode.None, DataMem8(m.IAdd(Fsr2, bankmem.BankOffset))); // Address is in the form [FSR2]+offset ("à la" Extended Execution mode).
                }
                if (PICMemoryDescriptor.TryGetAbsDataAddress(bankmem, out var absAddr))
                {
                    if (PICRegisters.TryGetRegister(absAddr, out var sfr))
                    {
                        var iop = PICRegisters.IndirectOpMode(sfr, out PICRegisterStorage fsrreg);
                        if (iop != FSRIndexedMode.None)
                            return (iop, binder.EnsureRegister(fsrreg));
                        return (iop, binder.EnsureRegister(sfr));
                    }
                    return (FSRIndexedMode.None, DataMem8(absAddr));

                }
            }
            return base.GetMemFileAccess(opernd);
        }

        private Expression GetFSR2IdxAddress(MachineOperand op)
        {
            switch (op)
            {
                case PICOperandImmediate fsr2idx:
                    return DataMem8(m.IAdd(Fsr2, fsr2idx.ImmediateValue));

                default:
                    throw new InvalidOperationException($"Invalid FSR2 indexed address operand.");
            }
        }

        protected void RewriteADDFSR()
        {
            var fsrnum = instrCurr.op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {instrCurr.op1}");
            var imm = instrCurr.op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op2}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}"));
            m.Assign(fsrreg, m.IAdd(fsrreg, imm.ImmediateValue));
        }

        private void RewriteADDLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(Wreg, m.IAdd(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void RewriteADDULNK()
        {
            var fsridx = instrCurr.op1 as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR indexation operand: {instrCurr.op1}");
            m.Assign(Fsr2, m.IAdd(Fsr2, fsridx.Offset));
            SetStatusFlags(Fsr2);
            rtlc = RtlClass.Transfer;
            m.Return(0, 0);
        }

        private void RewriteADDWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression lDestValue);
            ArithAssignIndirect(lDestValue, m.IAdd(Wreg, memExpr), indMode, memPtr);
        }

        private void RewriteADDWFC()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var carry = FlagGroup(FlagM.C);
            ArithAssignIndirect(dst, m.IAdd(m.IAdd(Wreg, memExpr), carry), indMode, memPtr);
        }

        private void RewriteANDLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            ArithAssign(Wreg, m.And(Wreg, k.ImmediateValue));
        }

        private void RewriteANDWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.And(Wreg, memExpr), indMode, memPtr);
        }

        private void RewriteBC()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.ULT, FlagGroup(FlagM.C)));
        }

        private void RewriteBCF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            var mask = GetBitMask(instrCurr.op2, true);
            ArithAssignIndirect(memExpr, m.And(memExpr, mask), indMode, memPtr);
        }

        private void RewriteBN()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.LT, FlagGroup(FlagM.N)));
        }

        private void RewriteBNC()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.UGE, FlagGroup(FlagM.C)));
        }

        private void RewriteBNN()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.GE, FlagGroup(FlagM.N)));
        }

        private void RewriteBNOV()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.NO, FlagGroup(FlagM.OV)));
        }

        private void RewriteBOV()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.OV, FlagGroup(FlagM.OV)));
        }

        private void RewriteBNZ()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.NE, FlagGroup(FlagM.Z)));
        }

        private void RewriteBRA()
        {
            rtlc = RtlClass.Transfer;
            if (instrCurr.op1 is PICOperandProgMemoryAddress brop)
            {
                m.Goto(brop.CodeTarget);
                return;
            }
            throw new InvalidOperationException("Wrong program relative PIC address");
        }

        private void RewriteBSF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            var mask = GetBitMask(instrCurr.op2, false);
            ArithAssignIndirect(memExpr, m.Or(memExpr, mask), indMode, memPtr);
        }

        private void RewriteBTFSC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            var mask = GetBitMask(instrCurr.op2, false);
            Expression res = null;

            switch (indMode)
            {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    res = m.And(memExpr, mask);
                    break;

                case FSRIndexedMode.POSTDEC:
                    res = m.And(memExpr, mask);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    res = m.And(memExpr, mask);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    res = m.And(memExpr, mask);
                    break;
            }
            m.Branch(m.Eq0(res), SkipToAddr(), rtlc);
        }

        private void RewriteBTFSS()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            var mask = GetBitMask(instrCurr.op2, false);
            Expression res = null;

            switch (indMode)
            {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    res = m.And(memExpr, mask);
                    break;

                case FSRIndexedMode.POSTDEC:
                    res = m.And(memExpr, mask);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    res = m.And(memExpr, mask);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    res = m.And(memExpr, mask);
                    break;

            }
            m.Branch(m.Ne0(res), SkipToAddr(), rtlc);
        }

        private void RewriteBTG()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            var mask = GetBitMask(instrCurr.op2, false);
            ArithAssignIndirect(memExpr, m.Xor(memExpr, mask), indMode, memPtr);
        }

        private void RewriteBZ()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)));
        }

        private void RewriteCALL()
        {
            var target =  instrCurr.op1 as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid CALL target operand: {instrCurr.op1}.");
            var fast = instrCurr.op2 as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST indicator operand: {instrCurr.op2}.");
            rtlc = RtlClass.Transfer | RtlClass.Call;

            Address retaddr = instrCurr.Address + instrCurr.Length;
            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Identifier statuss = binder.EnsureRegister(PIC18Registers.STATUS_CSHAD);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            if (fast.IsFast && (statuss != null) && (statuss.Storage.Domain != StorageDomain.None))
            {
                Identifier wregs = binder.EnsureRegister(PIC18Registers.WREG_CSHAD);
                Identifier bsrs = binder.EnsureRegister(PIC18Registers.BSR_CSHAD);
                m.Assign(statuss, binder.EnsureRegister(PICRegisters.STATUS));
                m.Assign(wregs, Wreg);
                m.Assign(bsrs, Bsr);
            }
            m.Call(target.CodeTarget, 0);
        }

        private void RewriteCALLW()
        {

            rtlc = RtlClass.Transfer | RtlClass.Call;

            var pclat = binder.EnsureRegister(PIC18Registers.PCLAT);
            var target = m.Fn(host.PseudoProcedure("__callw", VoidType.Instance, Wreg, pclat));
            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target, 0);
        }

        private void RewriteCLRF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);

            switch (indMode)
            {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    m.Assign(memExpr, 0);
                    break;

                case FSRIndexedMode.POSTDEC:
                    m.Assign(memExpr, 0);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    m.Assign(memExpr, 0);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    m.Assign(memExpr, 0);
                    break;
            }
            m.Assign(binder.EnsureFlagGroup(PIC18Registers.Z), Constant.Bool(true));
        }

        private void RewriteCLRWDT()
        {
            byte mask;

            PICRegisterBitFieldStorage pd = PICRegisters.PD;
            PICRegisterBitFieldStorage to = PICRegisters.TO;
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
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Comp(memExpr), indMode, memPtr);
        }

        private void RewriteCPFSEQ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            CondSkipIndirect(m.Eq(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteCPFSGT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            CondSkipIndirect(m.Ugt(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteCPFSLT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            CondSkipIndirect(m.Ult(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteDAW()
        {
            var C = FlagGroup(FlagM.C);
            var DC = FlagGroup(FlagM.DC);
            Expression res = m.Fn(host.PseudoProcedure("__daw", PrimitiveType.Byte, Wreg, C, DC));
            m.Assign(Wreg, res);
            SetStatusFlags(Wreg);
        }

        private void RewriteDECF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.ISub(memExpr, 1), indMode, memPtr);
        }

        private void RewriteDECFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.ISub(memExpr, 1), m.Eq0(dst), indMode, memPtr);
        }

        private void RewriteDCFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.ISub(memExpr, 1), m.Ne0(dst), indMode, memPtr);
        }

        private void RewriteGOTO()
        {
            var target = instrCurr.op1 as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid GOTO target operand: {instrCurr.op1}.");

            rtlc = RtlClass.Transfer;
            m.Goto(target.CodeTarget);
        }

        private void RewriteINCF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.IAdd(memExpr, 1), indMode, memPtr);
        }

        private void RewriteINCFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.IAdd(memExpr, 1), m.Eq0(dst), indMode, memPtr);
        }

        private void RewriteINFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.IAdd(memExpr, 1), m.Ne0(dst), indMode, memPtr);
        }

        private void RewriteIORLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(Wreg, m.Or(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void RewriteIORWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Or(Wreg, memExpr), indMode, memPtr);
        }

        private void RewriteLFSR()
        {
            var fsrnum = instrCurr.op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR register number operand: {instrCurr.op1}");
            var k = instrCurr.op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op2}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}"));
            m.Assign(fsrreg, k.ImmediateValue);
        }

        private void RewriteMOVF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, memExpr, indMode, memPtr);
        }

        protected void RewriteMOVFF()
        {
            var (indops, adrfs) = GetUnaryAbsPtrs(instrCurr.op1, out Expression derefptrs);
            var (indopd, adrfd) = GetUnaryAbsPtrs(instrCurr.op2, out Expression derefptrd);

             switch (indops)
             {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    switch (indopd)
                    {
                        case FSRIndexedMode.None:
                        case FSRIndexedMode.INDF:
                        case FSRIndexedMode.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case FSRIndexedMode.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case FSRIndexedMode.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case FSRIndexedMode.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    break;

                case FSRIndexedMode.POSTDEC:
                    switch (indopd)
                    {
                        case FSRIndexedMode.None:
                        case FSRIndexedMode.INDF:
                        case FSRIndexedMode.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case FSRIndexedMode.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case FSRIndexedMode.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case FSRIndexedMode.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    m.Assign(adrfs, m.ISub(adrfs, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    switch (indopd)
                    {
                        case FSRIndexedMode.None:
                        case FSRIndexedMode.INDF:
                        case FSRIndexedMode.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case FSRIndexedMode.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case FSRIndexedMode.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case FSRIndexedMode.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    m.Assign(adrfs, m.IAdd(adrfs, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(adrfs, m.IAdd(adrfs, 1));
                    switch (indopd)
                    {
                        case FSRIndexedMode.None:
                        case FSRIndexedMode.INDF:
                        case FSRIndexedMode.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case FSRIndexedMode.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case FSRIndexedMode.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case FSRIndexedMode.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    break;
            }

        }

        private void RewriteMOVLB()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(Bsr, k.ImmediateValue);
        }

        private void RewriteMOVLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(Wreg, k.ImmediateValue);
        }

        private void RewriteMOVSF()
        {
            var zs = GetFSR2IdxAddress(instrCurr.op1);
            var (indMode, memPtr) = GetUnaryAbsPtrs(instrCurr.op2, out Expression memExpr);
            ArithAssignIndirect(memExpr, zs, indMode, memPtr);
        }

        private void RewriteMOVSS()
        {
            var zs = GetFSR2IdxAddress(instrCurr.op1);
            var zd = GetFSR2IdxAddress(instrCurr.op2);
            m.Assign(zd, zs);
        }

        private void RewriteMOVWF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            ArithAssignIndirect(memExpr, Wreg, indMode, memPtr);
        }

        private void RewriteMULLW()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(prod, m.UMul(Wreg, k.ImmediateValue));
        }

        private void RewriteMULWF()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            ArithAssignIndirect(prod, m.UMul(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteNEGF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            ArithAssignIndirect(memExpr, m.Neg(memExpr), indMode, memPtr);
        }

        private void RewritePOP()
        {
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
        }

        private void RewritePUSH()
        {
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var nextaddr = instrCurr.Address + instrCurr.Length;
            var dst = PushToHWStackAccess();
            m.Assign(dst, nextaddr);
            m.Assign(tos, nextaddr);
        }

        private void RewritePUSHL()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            m.Assign(DataMem8(Fsr2), k.ImmediateValue);
            m.Assign(Fsr2, m.IAdd(Fsr2, 1));
        }

        private void RewriteRCALL()
        {
            var target = instrCurr.op1 as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid CALL target operand: {instrCurr.op1}.");
            rtlc = RtlClass.Transfer | RtlClass.Call;

            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target.CodeTarget, 0);
        }

        private void RewriteRESET()
        {
            rtlc = RtlClass.Terminates;

            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, Constant.Byte(0));
            m.SideEffect(host.PseudoProcedure("__reset", VoidType.Instance));
        }

        private void RewriteRETFIE()
        {
            rtlc = RtlClass.Transfer;

            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteRETLW()
        {
            rtlc = RtlClass.Transfer;

            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Wreg, k.ImmediateValue);
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteRETURN()
        {
            var fast = instrCurr.op1 as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST indicator operand: {instrCurr.op1}.");
            rtlc = RtlClass.Transfer;

            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Identifier statuss = binder.EnsureRegister(PIC18Registers.STATUS_CSHAD);

            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            if (fast.IsFast && (statuss != null) && (statuss.Storage.Domain != StorageDomain.None))
            {
                Identifier wregs = binder.EnsureRegister(PIC18Registers.WREG_CSHAD);
                Identifier bsrs = binder.EnsureRegister(PIC18Registers.BSR_CSHAD);
                m.Assign(Bsr, bsrs);
                m.Assign(Wreg, wregs);
                m.Assign(binder.EnsureRegister(PICRegisters.STATUS), statuss);
            }
            m.Return(0, 0);
        }

        private void RewriteRLCF()
        {
            //TODO:  PseudoProcedure(__rlcf) ?

            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var carry = FlagGroup(FlagM.C);
            ArithAssignIndirect(dst, m.Fn(host.PseudoProcedure("__rlcf", PrimitiveType.Byte, memExpr, carry)), indMode, memPtr);
        }

        private void RewriteRLNCF()
        {
            //TODO:  PseudoProcedure(__rlncf) ?

            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Fn(host.PseudoProcedure("__rlncf", PrimitiveType.Byte, memExpr)), indMode, memPtr);
        }

        private void RewriteRRCF()
        {
            //TODO:  PseudoProcedure(__rrcf) ?

            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var carry = FlagGroup(FlagM.C);
            ArithAssignIndirect(dst, m.Fn(host.PseudoProcedure("__rrcf", PrimitiveType.Byte, memExpr, carry)), indMode, memPtr);
        }

        private void RewriteRRNCF()
        {
            //TODO:  PseudoProcedure(__rrncf) ?

            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Fn(host.PseudoProcedure("__rrncf", PrimitiveType.Byte, memExpr)), indMode, memPtr);
        }

        private void RewriteSETF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            ArithAssignIndirect(memExpr, Constant.Byte(255), indMode, memPtr);
        }

        private void RewriteSLEEP()
        {
            byte mask;

            PICRegisterBitFieldStorage pd = PICRegisters.PD;
            PICRegisterBitFieldStorage to = PICRegisters.TO;
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

        protected void RewriteSUBFSR()
        {
            var fsrnum = instrCurr.op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {instrCurr.op1}");
            var imm = instrCurr.op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op2}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}"));
            m.Assign(fsrreg, m.ISub(fsrreg, imm.ImmediateValue));
        }

        private void RewriteSUBFWB()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var borrow = m.Not(FlagGroup(FlagM.C));
            ArithAssignIndirect(dst, m.ISub(m.ISub(Wreg, memExpr), borrow), indMode, memPtr);
        }

        private void RewriteSUBLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            ArithAssign(Wreg, m.ISub(k.ImmediateValue, Wreg));
        }

        private void RewriteSUBULNK()
        {
            rtlc = RtlClass.Transfer;

            var k = instrCurr.op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op2}");
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Fsr2, m.ISub(Fsr2, k.ImmediateValue));
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteSUBWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.ISub(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteSUBWFB()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var borrow = m.Not(FlagGroup(FlagM.C));
            ArithAssignIndirect(dst, m.ISub(m.ISub(memExpr, Wreg), borrow), indMode, memPtr);
        }

        private void RewriteSWAPF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, memExpr)), indMode, memPtr);
        }

        private void RewriteTBLRD()
        {
            var tblmode = instrCurr.op1 as PICOperandTBLRW ?? throw new InvalidOperationException($"Invalid TBLRD mode operand: {instrCurr.op1}.");
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            m.SideEffect(host.PseudoProcedure("__tblrd", VoidType.Instance, tblptr, tblmode.TBLIncrMode));
        }

        private void RewriteTBLWT()
        {
            var tblmode = instrCurr.op1 as PICOperandTBLRW ?? throw new InvalidOperationException($"Invalid TBLRD mode operand: {instrCurr.op1}.");
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            m.SideEffect(host.PseudoProcedure("__tblwt", VoidType.Instance, tblptr, tblmode.TBLIncrMode));
        }

        private void RewriteTSTFSZ()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.op1, out Expression memExpr);
            rtlc = RtlClass.ConditionalTransfer;
            CondSkipIndirect(m.Eq0(memExpr), indMode, memPtr);
        }

        private void RewriteXORLW()
        {
            var k = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            ArithAssign(Wreg, m.Xor(Wreg, k.ImmediateValue));
        }

        private void RewriteXORWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Xor(Wreg, memExpr), indMode, memPtr);
        }

        #endregion

    }

}
