#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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
    using Reko.Core.Intrinsics;

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

            switch (instrCurr.Mnemonic)
            {
                default:
                    host.Warn(
                        instrCurr.Address,
                        $"PIC18 instruction '{instrCurr.Mnemonic}' is not supported yet.");
                goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                case Mnemonic.unaligned:
                    m.Invalid();
                    break;
                case Mnemonic.ADDLW:
                    RewriteADDLW();
                    break;
                case Mnemonic.ADDWF:
                    RewriteADDWF();
                    break;
                case Mnemonic.ADDWFC:
                    RewriteADDWFC();
                    break;
                case Mnemonic.ANDLW:
                    RewriteANDLW();
                    break;
                case Mnemonic.ANDWF:
                    RewriteANDWF();
                    break;
                case Mnemonic.BC:
                    RewriteBC();
                    break;
                case Mnemonic.BCF:
                    RewriteBCF();
                    break;
                case Mnemonic.BN:
                    RewriteBN();
                    break;
                case Mnemonic.BNC:
                    RewriteBNC();
                    break;
                case Mnemonic.BNN:
                    RewriteBNN();
                    break;
                case Mnemonic.BNOV:
                    RewriteBNOV();
                    break;
                case Mnemonic.BNZ:
                    RewriteBNZ();
                    break;
                case Mnemonic.BOV:
                    RewriteBOV();
                    break;
                case Mnemonic.BRA:
                    RewriteBRA();
                    break;
                case Mnemonic.BSF:
                    RewriteBSF();
                    break;
                case Mnemonic.BTFSC:
                    RewriteBTFSC();
                    break;
                case Mnemonic.BTFSS:
                    RewriteBTFSS();
                    break;
                case Mnemonic.BTG:
                    RewriteBTG();
                    break;
                case Mnemonic.BZ:
                    RewriteBZ();
                    break;
                case Mnemonic.CALL:
                    RewriteCALL();
                    break;
                case Mnemonic.CLRF:
                    RewriteCLRF();
                    break;
                case Mnemonic.CLRWDT:
                    RewriteCLRWDT();
                    break;
                case Mnemonic.COMF:
                    RewriteCOMF();
                    break;
                case Mnemonic.CPFSEQ:
                    RewriteCPFSEQ();
                    break;
                case Mnemonic.CPFSGT:
                    RewriteCPFSGT();
                    break;
                case Mnemonic.CPFSLT:
                    RewriteCPFSLT();
                    break;
                case Mnemonic.DAW:
                    RewriteDAW();
                    break;
                case Mnemonic.DCFSNZ:
                    RewriteDCFSNZ();
                    break;
                case Mnemonic.DECF:
                    RewriteDECF();
                    break;
                case Mnemonic.DECFSZ:
                    RewriteDECFSZ();
                    break;
                case Mnemonic.GOTO:
                    RewriteGOTO();
                    break;
                case Mnemonic.INCF:
                    RewriteINCF();
                    break;
                case Mnemonic.INCFSZ:
                    RewriteINCFSZ();
                    break;
                case Mnemonic.INFSNZ:
                    RewriteINFSNZ();
                    break;
                case Mnemonic.IORLW:
                    RewriteIORLW();
                    break;
                case Mnemonic.IORWF:
                    RewriteIORWF();
                    break;
                case Mnemonic.LFSR:
                    RewriteLFSR();
                    break;
                case Mnemonic.MOVF:
                    RewriteMOVF();
                    break;
                case Mnemonic.MOVFF:
                    RewriteMOVFF();
                    break;
                case Mnemonic.MOVLB:
                    RewriteMOVLB();
                    break;
                case Mnemonic.MOVLW:
                    RewriteMOVLW();
                    break;
                case Mnemonic.MOVWF:
                    RewriteMOVWF();
                    break;
                case Mnemonic.MULLW:
                    RewriteMULLW();
                    break;
                case Mnemonic.MULWF:
                    RewriteMULWF();
                    break;
                case Mnemonic.NEGF:
                    RewriteNEGF();
                    break;
                case Mnemonic.NOP:
                    m.Nop();
                    break;
                case Mnemonic.POP:
                    RewritePOP();
                    break;
                case Mnemonic.PUSH:
                    RewritePUSH();
                    break;
                case Mnemonic.RCALL:
                    RewriteRCALL();
                    break;
                case Mnemonic.RESET:
                    RewriteRESET();
                    break;
                case Mnemonic.RETFIE:
                    RewriteRETFIE();
                    break;
                case Mnemonic.RETLW:
                    RewriteRETLW();
                    break;
                case Mnemonic.RETURN:
                    RewriteRETURN();
                    break;
                case Mnemonic.RLCF:
                    RewriteRLCF();
                    break;
                case Mnemonic.RLNCF:
                    RewriteRLNCF();
                    break;
                case Mnemonic.RRCF:
                    RewriteRRCF();
                    break;
                case Mnemonic.RRNCF:
                    RewriteRRNCF();
                    break;
                case Mnemonic.SETF:
                    RewriteSETF();
                    break;
                case Mnemonic.SLEEP:
                    RewriteSLEEP();
                    break;
                case Mnemonic.SUBFWB:
                    RewriteSUBFWB();
                    break;
                case Mnemonic.SUBLW:
                    RewriteSUBLW();
                    break;
                case Mnemonic.SUBWF:
                    RewriteSUBWF();
                    break;
                case Mnemonic.SUBWFB:
                    RewriteSUBWFB();
                    break;
                case Mnemonic.SWAPF:
                    RewriteSWAPF();
                    break;
                case Mnemonic.TBLRD:
                    RewriteTBLRD();
                    break;
                case Mnemonic.TBLWT:
                    RewriteTBLWT();
                    break;
                case Mnemonic.TSTFSZ:
                    RewriteTSTFSZ();
                    break;
                case Mnemonic.XORLW:
                    RewriteXORLW();
                    break;
                case Mnemonic.XORWF:
                    RewriteXORWF();
                    break;

                // Pseudo-instructions
                case Mnemonic.CONFIG:
                case Mnemonic.DA:
                case Mnemonic.DB:
                case Mnemonic.DE:
                case Mnemonic.DW:
                case Mnemonic.__IDLOCS:
                    m.Invalid();
                    break;
            }

        }


        #region Rewrite methods common to all PIC18 instruction-sets.

        protected override void SetStatusFlags(Expression dst)
        {
            FlagM flags = PIC18CC.Defined(instrCurr.Mnemonic);
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
                    return (FSRIndexedMode.None, DataMem8(absAddr.ToAddress()));

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
            var fsrnum = instrCurr.Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {instrCurr.Operands[0]}");
            var imm = instrCurr.Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[1]}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}")!);
            m.Assign(fsrreg, m.IAdd(fsrreg, imm.ImmediateValue));
        }

        private void RewriteADDLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.IAdd(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void RewriteADDULNK()
        {
            var fsridx = instrCurr.Operands[0] as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR indexation operand: {instrCurr.Operands[0]}");
            m.Assign(Fsr2, m.IAdd(Fsr2, fsridx.Offset));
            SetStatusFlags(Fsr2);
            iclass = InstrClass.Transfer;
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
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
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
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            var mask = GetBitMask(instrCurr.Operands[1], true);
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
            iclass = InstrClass.Transfer;
            if (instrCurr.Operands[0] is PICOperandProgMemoryAddress brop)
            {
                m.Goto(brop.CodeTarget);
                return;
            }
            throw new InvalidOperationException("Wrong program relative PIC address");
        }

        private void RewriteBSF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            ArithAssignIndirect(memExpr, m.Or(memExpr, mask), indMode, memPtr);
        }

        private void RewriteBTFSC()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            Expression? res = null;

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
            m.Branch(m.Eq0(res!), SkipToAddr(), iclass);
        }

        private void RewriteBTFSS()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            Expression? res = null;

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
            m.Branch(m.Ne0(res!), SkipToAddr(), iclass);
        }

        private void RewriteBTG()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            ArithAssignIndirect(memExpr, m.Xor(memExpr, mask), indMode, memPtr);
        }

        private void RewriteBZ()
        {
            // TODO: review RTL to use for flag test.
            CondBranch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)));
        }

        private void RewriteCALL()
        {
            var target =  instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid CALL target operand: {instrCurr.Operands[0]}.");
            var fast = instrCurr.Operands[1] as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST indicator operand: {instrCurr.Operands[1]}.");
            iclass = InstrClass.Transfer | InstrClass.Call;

            Address retaddr = instrCurr.Address + instrCurr.Length;
            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Identifier? statuss = PIC18Registers.STATUS_CSHAD is not null
                ? binder.EnsureRegister(PIC18Registers.STATUS_CSHAD)
                : null;

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            if (fast.IsFast && (statuss is not null) && (statuss.Storage.Domain != StorageDomain.None))
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
            iclass = InstrClass.Transfer | InstrClass.Call;

            var pclat = binder.EnsureRegister(PIC18Registers.PCLAT);
            var target = m.Fn(callw_intrinsic, Wreg, pclat);
            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target, 0);
        }

        private void RewriteCLRF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);

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

            if (ReferenceEquals(pdreg, toreg) && pdreg is not null)
            {
                mask = (byte)((1 << pd.BitPos) | (1 << to.BitPos));
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
                return;
            }
            if (pdreg is not null)
            {
                mask = (byte)((1 << pd.BitPos));
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
            }
            if (toreg is not null)
            {
                mask = (byte)((1 << to.BitPos));
                m.Assign(pdreg!, m.Or(pdreg!, Constant.Byte(mask)));
            }
        }

        private void RewriteCOMF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Comp(memExpr), indMode, memPtr);
        }

        private void RewriteCPFSEQ()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            CondSkipIndirect(m.Eq(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteCPFSGT()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            CondSkipIndirect(m.Ugt(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteCPFSLT()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            CondSkipIndirect(m.Ult(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteDAW()
        {
            var C = FlagGroup(FlagM.C);
            var DC = FlagGroup(FlagM.DC);
            Expression res = m.Fn(daw_intrinsic, Wreg, C, DC);
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
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.ISub(memExpr, 1), m.Eq0(dst), indMode, memPtr);
        }

        private void RewriteDCFSNZ()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.ISub(memExpr, 1), m.Ne0(dst), indMode, memPtr);
        }

        private void RewriteGOTO()
        {
            var target = instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid GOTO target operand: {instrCurr.Operands[0]}.");

            iclass = InstrClass.Transfer;
            m.Goto(target.CodeTarget);
        }

        private void RewriteINCF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.IAdd(memExpr, 1), indMode, memPtr);
        }

        private void RewriteINCFSZ()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.IAdd(memExpr, 1), m.Eq0(dst), indMode, memPtr);
        }

        private void RewriteINFSNZ()
        {
            iclass = InstrClass.ConditionalTransfer;
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithCondSkip(dst, m.IAdd(memExpr, 1), m.Ne0(dst), indMode, memPtr);
        }

        private void RewriteIORLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
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
            var fsrnum = instrCurr.Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR register number operand: {instrCurr.Operands[0]}");
            var k = instrCurr.Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[1]}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}")!);
            m.Assign(fsrreg, k.ImmediateValue);
        }

        private void RewriteMOVF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, memExpr, indMode, memPtr);
        }

        protected void RewriteMOVFF()
        {
            var (indops, adrfs) = GetUnaryAbsPtrs(instrCurr.Operands[0], out Expression derefptrs);
            var (indopd, adrfd) = GetUnaryAbsPtrs(instrCurr.Operands[1], out Expression derefptrd);

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
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Bsr, k.ImmediateValue);
        }

        private void RewriteMOVLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, k.ImmediateValue);
        }

        private void RewriteMOVSF()
        {
            var zs = GetFSR2IdxAddress(instrCurr.Operands[0]);
            var (indMode, memPtr) = GetUnaryAbsPtrs(instrCurr.Operands[1], out Expression memExpr);
            ArithAssignIndirect(memExpr, zs, indMode, memPtr);
        }

        private void RewriteMOVSS()
        {
            var zs = GetFSR2IdxAddress(instrCurr.Operands[0]);
            var zd = GetFSR2IdxAddress(instrCurr.Operands[1]);
            m.Assign(zd, zs);
        }

        private void RewriteMOVWF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            ArithAssignIndirect(memExpr, Wreg, indMode, memPtr);
        }

        private void RewriteMULLW()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(prod, m.UMul(Wreg, k.ImmediateValue));
        }

        private void RewriteMULWF()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            ArithAssignIndirect(prod, m.UMul(memExpr, Wreg), indMode, memPtr);
        }

        private void RewriteNEGF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
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
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(DataMem8(Fsr2), k.ImmediateValue);
            m.Assign(Fsr2, m.IAdd(Fsr2, 1));
        }

        private void RewriteRCALL()
        {
            var target = instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid CALL target operand: {instrCurr.Operands[0]}.");
            iclass = InstrClass.Transfer | InstrClass.Call;

            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target.CodeTarget, 0);
        }

        private void RewriteRESET()
        {
            iclass = InstrClass.Terminates;

            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, Constant.Byte(0));
            m.SideEffect(m.Fn(reset_intrinsic));
        }

        private void RewriteRETFIE()
        {
            iclass = InstrClass.Transfer;

            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteRETLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Wreg, k.ImmediateValue);
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteRETURN()
        {
            var fast = instrCurr.Operands[0] as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST indicator operand: {instrCurr.Operands[0]}.");
            iclass = InstrClass.Transfer | InstrClass.Return;

            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Identifier? statuss = PIC18Registers.STATUS_CSHAD is not null
                ? binder.EnsureRegister(PIC18Registers.STATUS_CSHAD)
                : null;
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            if (fast.IsFast && (statuss is not null) && (statuss.Storage.Domain != StorageDomain.None))
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
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var carry = FlagGroup(FlagM.C);
            ArithAssignIndirect(dst, m.Fn(rlcf_intrinsic, memExpr, carry), indMode, memPtr);
        }

        private void RewriteRLNCF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Fn(rlncf_intrinsic, memExpr), indMode, memPtr);
        }

        private void RewriteRRCF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            var carry = FlagGroup(FlagM.C);
            ArithAssignIndirect(dst, m.Fn(rrcf_intrinsic, memExpr, carry), indMode, memPtr);
        }

        private void RewriteRRNCF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Fn(rrncf_intrinsic, memExpr), indMode, memPtr);
        }

        private void RewriteSETF()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            ArithAssignIndirect(memExpr, Constant.Byte(255), indMode, memPtr);
        }

        private void RewriteSLEEP()
        {
            byte mask;

            PICRegisterBitFieldStorage pd = PICRegisters.PD;
            PICRegisterBitFieldStorage to = PICRegisters.TO;
            Identifier pdreg = binder.EnsureRegister(pd.FlagRegister);
            Identifier toreg = binder.EnsureRegister(to.FlagRegister);

            if (ReferenceEquals(pdreg, toreg) && pdreg is not null)
            {
                mask = (byte)(~(1 << pd.BitPos));
                m.Assign(pdreg, m.And(pdreg, Constant.Byte(mask)));
                mask = (byte)(1 << to.BitPos);
                m.Assign(pdreg, m.Or(pdreg, Constant.Byte(mask)));
                return;
            }
            if (pd is not null)
            {
                m.Assign(pdreg!, m.Dpb(pdreg!, Constant.False(), pd.BitPos));
            }
            if (to is not null)
            {
                m.Assign(toreg, m.Dpb(toreg, Constant.True(), to.BitPos));
            }
        }

        protected void RewriteSUBFSR()
        {
            var fsrnum = instrCurr.Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {instrCurr.Operands[0]}");
            var imm = instrCurr.Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[1]}");
            var fsrreg = binder.EnsureRegister(PICRegisters.GetRegister($"FSR{fsrnum.FSRNum}")!);
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
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            ArithAssign(Wreg, m.ISub(k.ImmediateValue, Wreg));
        }

        private void RewriteSUBULNK()
        {
            var k = instrCurr.Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[1]}");
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
            ArithAssignIndirect(dst, m.Fn(swapf_intrinsic, memExpr), indMode, memPtr);
        }

        private void RewriteTBLRD()
        {
            var tblmode = instrCurr.Operands[0] as PICOperandTBLRW ?? throw new InvalidOperationException($"Invalid TBLRD mode operand: {instrCurr.Operands[0]}.");
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            m.SideEffect(m.Fn(tblrd_intrinsic, tblptr, tblmode.TBLIncrMode));
        }

        private void RewriteTBLWT()
        {
            var tblmode = instrCurr.Operands[0] as PICOperandTBLRW ?? throw new InvalidOperationException($"Invalid TBLRD mode operand: {instrCurr.Operands[0]}.");
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            m.SideEffect(m.Fn(tblwt_intrinsic, tblptr, tblmode.TBLIncrMode));
        }

        private void RewriteTSTFSZ()
        {
            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out Expression memExpr);
            iclass = InstrClass.ConditionalTransfer;
            CondSkipIndirect(m.Eq0(memExpr), indMode, memPtr);
        }

        private void RewriteXORLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            ArithAssign(Wreg, m.Xor(Wreg, k.ImmediateValue));
        }

        private void RewriteXORWF()
        {
            var (indMode, memPtr) = GetBinaryPtrs(out Expression memExpr, out Expression dst);
            ArithAssignIndirect(dst, m.Xor(Wreg, memExpr), indMode, memPtr);
        }

        #endregion

        private static readonly IntrinsicProcedure tblrd_intrinsic = IntrinsicBuilder.SideEffect("__tblrd")
            .Param(PrimitiveType.Ptr16)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure tblwt_intrinsic = IntrinsicBuilder.SideEffect("__tblwt")
            .Param(PrimitiveType.Ptr16)
            .Param(PrimitiveType.Byte)
            .Void();

    }

}
