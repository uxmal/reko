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

using Reko.Arch.Microchip.Common;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC18
{

    public class PIC18Rewriter : IEnumerable<RtlInstructionCluster>
    {

        #region Locals

        private PIC18Architecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private PIC18Disassembler disasm;
        private IEnumerator<PIC18Instruction> dasm;
        private PIC18State state;
        private PIC18Instruction instrCurr;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;
        private Identifier Wreg;    // cached WREG register identifier
        private Identifier Fsr2;    // cached FSR2 register identifier

        #endregion

        #region Constructors

        public PIC18Rewriter(PIC18Architecture arch, EndianImageReader rdr, PIC18State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            disasm = new PIC18Disassembler(arch, rdr);
            dasm = disasm.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCurr = dasm.Current;
                var addr = instrCurr.Address;
                var len = instrCurr.Length;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                Wreg = binder.EnsureRegister(PIC18Registers.WREG);
                Fsr2 = binder.EnsureRegister(PIC18Registers.FSR2);

                switch (instrCurr.Opcode)
                {
                    default:
                        throw new AddressCorrelatedException(addr, $"Rewriting of PIC18 instruction '{instrCurr.Opcode}' is not implemented yet.");

                    case Opcode.invalid:
                    case Opcode.unaligned:
                        m.Invalid();
                        break;
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
                    case Opcode.MOVFFL: RewriteMOVFF(); break;
                    case Opcode.MOVLB: RewriteMOVLB(); break;
                    case Opcode.MOVLW: RewriteMOVLW(); break;
                    case Opcode.MOVSF: RewriteMOVSF(); break;
                    case Opcode.MOVSFL: RewriteMOVSF(); break;
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
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break; ;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); 

        #endregion

        #region Helpers

        private Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(PIC18Registers.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        private ArrayAccess PushToHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            var slot = m.ARef(PrimitiveType.Ptr32, PIC18Registers.GlobalStack, stkptr);
            m.Assign(stkptr, m.IAdd(stkptr, Constant.Byte(1)));
            return slot;
        }

        private ArrayAccess PopFromHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, m.ISub(stkptr, Constant.Byte(1)));
            var slot = m.ARef(PrimitiveType.Ptr32, PIC18Registers.GlobalStack, stkptr);
            return slot;
        }

        private static MemoryAccess DataMem8(Expression ea)
            => new MemoryAccess(PIC18Registers.GlobalData, ea, PrimitiveType.Byte);

        private Expression GetFSRNum(MachineOperand op)
        {
            switch (op)
            {
                case PIC18FSROperand fsrnum:
                    switch (fsrnum.FSRNum.ToByte())
                    {
                        case 0:
                            return binder.EnsureRegister(PIC18Registers.FSR0);
                        case 1:
                            return binder.EnsureRegister(PIC18Registers.FSR1);
                        case 2:
                            return binder.EnsureRegister(PIC18Registers.FSR2);
                        default:
                            throw new InvalidOperationException($"Invalid FSR number: {fsrnum.FSRNum.ToByte()}");
                    }

                default:
                    throw new InvalidOperationException($"Invalid FSR operand.");
            }
        }

        private Expression GetImmediateValue(MachineOperand op)
        {
            switch (op)
            {
                case PIC18ImmediateOperand imm:
                    return imm.ImmediateValue;

                default:
                    throw new InvalidOperationException($"Invalid immediate operand.");
            }
        }

        private Expression GetProgramAddress(MachineOperand op)
        {
            switch (op)
            {
                case PIC18ProgAddrOperand paddr:
                    return PICProgAddress.Ptr(paddr.CodeTarget);

                default:
                    throw new InvalidOperationException($"Invalid program address operand.");
            }
        }

        private Expression GetTBLRWMode(MachineOperand op)
        {
            switch (op)
            {
                case PIC18TableReadWriteOperand tblincrmod:
                    return tblincrmod.TBLIncrMode;

                default:
                    throw new InvalidOperationException($"Invalid table read/write operand.");
            }
        }

        private Constant GetShadow(MachineOperand op)
        {
            switch (op)
            {
                case PIC18ShadowOperand shadow:
                    return shadow.IsShadow;

                default:
                    throw new InvalidOperationException($"Invalid shadow operand.");
            }
        }

        private Expression GetFSR2IdxAddress(MachineOperand op)
        {
            switch (op)
            {
                case PIC18FSR2IdxOperand fsr2idx:
                    return DataMem8(m.IAdd(Fsr2, fsr2idx.Offset));

                default:
                    throw new InvalidOperationException($"Invalid FSR2 indexed address operand.");
            }
        }

        private Constant GetBitMask(MachineOperand op, bool revert)
        {
            switch (op)
            {
                case PIC18DataBitAccessOperand bitaddr:
                    int mask = (1 << bitaddr.BitNumber.ToByte());
                    if (revert) mask = ~mask;
                    return Constant.Byte((byte)mask);

                default:
                    throw new InvalidOperationException("Invalid bit number operand.");
            }
        }

        #endregion

        #region Rewrite methods

        #region Instructions Rewriter Helpers

        /// <summary>
        /// Gets the source/destination direct address (SFR register or memory), addressing mode and actual source memory access
        /// of a single operand instruction. (e.g. "SETF f,a").
        /// </summary>
        /// <param name="op">The instruction operand.</param>
        /// <param name="ptr">[out] The source/destination pointer expression to access memory.</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        private (IndirectRegOp, Expression) GetUnaryPtrs(MachineOperand op, out Expression ptr)
        {
            (IndirectRegOp indop, Expression adr) GetMemoryBankAccess()
            {

                switch (op)
                {
                    case PIC18BankedAccessOperand bmem:
                        var offset = bmem.BankAddr;

                        // Check if access is a Direct Addressing one (bank designated by BSR).
                        // 
                        if (!bmem.IsAccessRAM.ToBoolean())
                        {
                            // Address is BSR direct addressing.
                            Identifier bsr = binder.EnsureRegister(PIC18Registers.BSR);
                            return (IndirectRegOp.None, DataMem8(m.IAdd(m.Shl(bsr, 8), offset)));
                        }

                        // We have some sort of Access Bank RAM type of access; either Lower or Upper area.
                        //
                        if (PIC18MemoryDescriptor.BelongsToAccessRAMLow(offset))
                        {
                            if (bmem.ExecMode == PICExecMode.Traditional)
                            {
                                return (IndirectRegOp.None, DataMem8(offset));
                            }
                            // Address is in the form [FSR2]+offset ("à la" Extended Execution mode).
                            return (IndirectRegOp.None, DataMem8(m.IAdd(Fsr2, offset)));
                        }

                        // Address is Upper ACCESS Bank addressing. Try to get any "known" SFR for this PIC.
                        // 
                        var accAddr = PIC18MemoryDescriptor.TranslateAccessAddress(offset);
                        var sfr = PIC18Registers.GetRegisterBySizedAddr(accAddr, 8) as PICRegisterStorage;
                        if (sfr != PICRegisterStorage.None)
                        {
                            var iop = PIC18Registers.IndirectOpMode(sfr, out PICRegisterStorage fsr);
                            if (iop != IndirectRegOp.None)
                                return (iop, binder.EnsureRegister(fsr));
                            return (iop, binder.EnsureRegister(sfr));
                        }
                        return (IndirectRegOp.None, DataMem8(accAddr));

                    default:
                        throw new InvalidOperationException("Invalid direct addressing operand.");
                }
            }

            var (indop, adr) = GetMemoryBankAccess();
            switch (indop)
            {
                case IndirectRegOp.None:    // Direct mode
                    ptr = adr;
                    break;

                case IndirectRegOp.INDF:    // Indirect modes
                case IndirectRegOp.POSTDEC:
                case IndirectRegOp.POSTINC:
                case IndirectRegOp.PREINC:
                    ptr = DataMem8(adr);
                    break;

                case IndirectRegOp.PLUSW:   // Indirect-indexed mode
                    ptr = DataMem8(m.IAdd(adr, Wreg));
                    break;

                default:
                    throw new InvalidOperationException("Unable to adjust indirect pointer.");
            }

            return (indop, adr);
        }

        /// <summary>
        /// Gets the source address (SFR register or memory), addressing mode, actual source memory access
        /// and actual destination access of a dual operand instruction (e.g. "ADDWF f,d,a").
        /// </summary>
        /// <param name="op">The instruction operand.</param>
        /// <param name="derefptr">[out] The source pointer expression to access memory.</param>
        /// <param name="dst">[out] Destination access expression (WREG or memory access).</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        private (IndirectRegOp, Expression) GetBinaryPtrs(MachineOperand op, out Expression derefptr, out Expression dst)
        {

            bool DestIsWreg()
            {
                if (op is PIC18DataByteAccessWithDestOperand bytedst)
                    return bytedst.WregIsDest.ToBoolean();
                return false;
            }

            var (indop, adr) = GetUnaryPtrs(op, out derefptr);
            dst = (DestIsWreg() ? Wreg : derefptr);
            return (indop, adr);
        }

        /// <summary>
        /// Gets the source/destination absolute address (SFR register or memory), addressing mode and actual memory access
        /// of an absolute address instruction. (e.g. the <code>"fd"</code> of <code>"MOVSF zs,fd"</code>).
        /// </summary>
        /// <param name="op">The instruction operand.</param>
        /// <param name="derefptr">[out] The source/destination pointer expression to access memory.</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the instruction operand is invalid.</exception>
        private (IndirectRegOp, Expression) GetUnaryAbsPtrs(MachineOperand op, out Expression derefptr)
        {

            (IndirectRegOp indop, Expression adr) GetDataAbsAddress()
            {
                switch (op)
                {
                    case PIC18DataAbsAddrOperand memabsaddr:
                        var reg = PIC18Registers.GetRegisterBySizedAddr(memabsaddr.DataTarget, 8);
                        if (reg is PICRegisterStorage sfr)
                        {
                            if (sfr != PICRegisterStorage.None)
                            {
                                var indmode = PIC18Registers.IndirectOpMode(sfr, out PICRegisterStorage fsr);
                                if (indmode != IndirectRegOp.None)
                                    return (indmode, binder.EnsureRegister(fsr));
                                return (indmode, binder.EnsureRegister(sfr));
                            }
                        }
                        return (IndirectRegOp.None, DataMem8(PICDataAddress.Ptr(memabsaddr.DataTarget)));

                    default:
                        throw new InvalidOperationException($"Invalid data absolute address operand.");
                }
            }

            var (indop, adr) = GetDataAbsAddress();
            switch (indop)
            {
                case IndirectRegOp.None:    // Direct mode
                    derefptr = adr;
                    break;

                case IndirectRegOp.INDF:    // Indirect modes
                case IndirectRegOp.POSTDEC:
                case IndirectRegOp.POSTINC:
                case IndirectRegOp.PREINC:
                    derefptr = DataMem8(adr);
                    break;

                case IndirectRegOp.PLUSW:   // Indirect-indexed mode
                    derefptr = DataMem8(m.IAdd(adr, Wreg));
                    break;

                default:
                    throw new InvalidOperationException("Unable to adjust indirect pointer.");
            }

            return (indop, adr);
        }

        private void RewriteCondBranch(TestCondition test)
        {
            rtlc = RtlClass.ConditionalTransfer;
            if (instrCurr.op1 is PIC18ProgRel8AddrOperand brop)
            {
                m.Branch(test, PICProgAddress.Ptr(brop.CodeTarget), rtlc);
                return;
            }
            throw new InvalidOperationException("Wrong 8-bit relative PIC address");
        }

        private void SetStatusFlags(Expression dst)
        {
            FlagM flags = PIC18CC.Defined(instrCurr.Opcode);
            if (flags != 0)
                m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        private PICProgAddress SkipToAddr()
            => PICProgAddress.Ptr(instrCurr.Address + instrCurr.Length + 2);

        #endregion

        private void RewriteADDFSR()
        {
            var fsr = GetFSRNum(instrCurr.op1);
            var k = GetImmediateValue(instrCurr.op2);
            m.Assign(fsr, m.IAdd(fsr, k));
        }

        private void RewriteADDLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.IAdd(Wreg, k));
            SetStatusFlags(Wreg);
        }

        private void RewriteADDULNK()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Fsr2, m.IAdd(Fsr2, k));
            SetStatusFlags(Fsr2);
            rtlc = RtlClass.Transfer;
            m.Return(0, 0);
        }

        private void RewriteADDWF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
                
            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.IAdd(Wreg, derefptr));
                    break;


                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.IAdd(Wreg, derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.IAdd(Wreg, derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.IAdd(Wreg, derefptr));
                    break;

            }
            SetStatusFlags(dst);
        }

        private void RewriteADDWFC()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
            var carry = FlagGroup(FlagM.C);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.IAdd(m.IAdd(Wreg, derefptr), carry));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.IAdd(m.IAdd(Wreg, derefptr), carry));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.IAdd(m.IAdd(Wreg, derefptr), carry));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(m.IAdd(adr, 1), carry));
                    m.Assign(dst, m.IAdd(Wreg, derefptr));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteANDLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            var res = m.And(Wreg, k);
            m.Assign(Wreg, res);
            SetStatusFlags(Wreg);
        }

        private void RewriteANDWF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.And(Wreg, derefptr));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.And(Wreg, derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.And(Wreg, derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.And(Wreg, derefptr));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteBC()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.ULT, FlagGroup(FlagM.C)));
        }

        private void RewriteBCF()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            var mask = GetBitMask(instrCurr.op1, true);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, m.And(derefptr, mask));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, m.And(derefptr, mask));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, m.And(derefptr, mask));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, m.And(derefptr, mask));
                    break;

            }
        }

        private void RewriteBN()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.LT, FlagGroup(FlagM.N)));
        }

        private void RewriteBNC()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.UGE, FlagGroup(FlagM.C)));
        }

        private void RewriteBNN()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.GE, FlagGroup(FlagM.N)));
        }

        private void RewriteBNOV()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.NO, FlagGroup(FlagM.OV)));
        }

        private void RewriteBOV()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.OV, FlagGroup(FlagM.OV)));
        }

        private void RewriteBNZ()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.NE, FlagGroup(FlagM.Z)));
        }

        private void RewriteBRA()
        {
            rtlc = RtlClass.Transfer;
            if (instrCurr.op1 is PIC18ProgRel11AddrOperand brop)
            {
                m.Goto(PICProgAddress.Ptr(brop.CodeTarget));
                return;
            }
            throw new InvalidOperationException("Wrong 11-bit relative PIC address");
        }

        private void RewriteBSF()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            var mask = GetBitMask(instrCurr.op1, false);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, m.Or(derefptr, mask));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, m.Or(derefptr, mask));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, m.Or(derefptr, mask));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, m.Or(derefptr, mask));
                    break;
            }
        }

        private void RewriteBTFSC()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            var mask = GetBitMask(instrCurr.op1, false);
            Expression res = null;

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    res = m.And(derefptr, mask);
                    break;

                case IndirectRegOp.POSTDEC:
                    res = m.And(derefptr, mask);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    res = m.And(derefptr, mask);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    res = m.And(derefptr, mask);
                    break;
            }
            m.Branch(m.Eq0(res), SkipToAddr(), rtlc);
        }

        private void RewriteBTFSS()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            var mask = GetBitMask(instrCurr.op1, false);
            Expression res = null;

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    res = m.And(derefptr, mask);
                    break;

                case IndirectRegOp.POSTDEC:
                    res = m.And(derefptr, mask);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    res = m.And(derefptr, mask);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    res = m.And(derefptr, mask);
                    break;

            }
            m.Branch(m.Ne0(res), SkipToAddr(), rtlc);
        }

        private void RewriteBTG()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            var mask = GetBitMask(instrCurr.op1, false);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, m.Xor(derefptr, mask));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, m.Xor(derefptr, mask));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, m.Xor(derefptr, mask));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, m.Xor(derefptr, mask));
                    break;

            }
        }

        private void RewriteBZ()
        {
            // TODO: review RTL to use for flag test.
            RewriteCondBranch(m.Test(ConditionCode.EQ, FlagGroup(FlagM.Z)));
        }

        private void RewriteCALL()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;

            var target = GetProgramAddress(instrCurr.op1);
            Constant shad = GetShadow(instrCurr.op2);
            Address retaddr = instrCurr.Address + instrCurr.Length;
            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Identifier statuss = binder.EnsureRegister(PIC18Registers.STATUS_CSHAD);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            if (shad.ToBoolean() && !(statuss is null))
            {
                Identifier wregs = binder.EnsureRegister(PIC18Registers.WREG_CSHAD);
                Identifier bsrs = binder.EnsureRegister(PIC18Registers.BSR_CSHAD);
                m.Assign(statuss, binder.EnsureRegister(PIC18Registers.STATUS));
                m.Assign(wregs, Wreg);
                m.Assign(bsrs, binder.EnsureRegister(PIC18Registers.BSR));
            }
            m.Call(target, 0);
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
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, 0);
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, 0);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, 0);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, 0);
                    break;
            }
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
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Comp(derefptr));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Comp(derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Comp(derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Comp(derefptr));
                    break;
            }

            SetStatusFlags(dst);
        }

        private void RewriteCPFSEQ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Branch(m.Eq(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Branch(m.Eq(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.POSTINC:
                    m.Branch(m.Eq(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Branch(m.Eq(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }
        }

        private void RewriteCPFSGT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Branch(m.Ugt(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Branch(m.Ugt(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.POSTINC:
                    m.Branch(m.Ugt(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Branch(m.Ugt(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

        }

        private void RewriteCPFSLT()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Branch(m.Ult(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Branch(m.Ult(derefptr, Wreg), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.POSTINC:
                    m.Branch(m.Ult(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Branch(m.Ult(derefptr, Wreg), SkipToAddr(), rtlc);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }
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
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteDECFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;
            }
            m.Branch(m.Eq0(dst), SkipToAddr(), rtlc);
        }

        private void RewriteDCFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(derefptr, 1));
                    break;
            }
            m.Branch(m.Ne0(dst), SkipToAddr(), rtlc);
        }

        private void RewriteGOTO()
        {

            rtlc = RtlClass.Transfer;
            var target = GetProgramAddress(instrCurr.op1);
            m.Goto(target);
        }

        private void RewriteINCF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteINCFSZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;
            }
            m.Branch(m.Eq0(dst), SkipToAddr(), rtlc);
        }

        private void RewriteINFSNZ()
        {
            rtlc = RtlClass.ConditionalTransfer;
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.IAdd(derefptr, 1));
                    break;
            }
            m.Branch(m.Ne0(dst), SkipToAddr(), rtlc);
        }

        private void RewriteIORLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.Or(Wreg, k));
            SetStatusFlags(Wreg);
        }

        private void RewriteIORWF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Or(Wreg, derefptr));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Or(Wreg, derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Or(Wreg, derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Or(Wreg, derefptr));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteLFSR()
        {
            var sfrN = GetFSRNum(instrCurr.op1);
            var k = GetImmediateValue(instrCurr.op2);
            m.Assign(sfrN, k);
            SetStatusFlags(sfrN);
        }

        private void RewriteMOVF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, derefptr);
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, derefptr);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, derefptr);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, derefptr);
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteMOVFF()
        {
            var (indops, adrfs) = GetUnaryAbsPtrs(instrCurr.op1, out Expression derefptrs);
            var (indopd, adrfd) = GetUnaryAbsPtrs(instrCurr.op2, out Expression derefptrd);

             switch (indops)
             {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    switch (indopd)
                    {
                        case IndirectRegOp.None:
                        case IndirectRegOp.INDF:
                        case IndirectRegOp.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case IndirectRegOp.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case IndirectRegOp.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case IndirectRegOp.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    break;

                case IndirectRegOp.POSTDEC:
                    switch (indopd)
                    {
                        case IndirectRegOp.None:
                        case IndirectRegOp.INDF:
                        case IndirectRegOp.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case IndirectRegOp.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case IndirectRegOp.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case IndirectRegOp.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    m.Assign(adrfs, m.ISub(adrfs, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    switch (indopd)
                    {
                        case IndirectRegOp.None:
                        case IndirectRegOp.INDF:
                        case IndirectRegOp.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case IndirectRegOp.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case IndirectRegOp.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case IndirectRegOp.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    m.Assign(adrfs, m.IAdd(adrfs, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adrfs, m.IAdd(adrfs, 1));
                    switch (indopd)
                    {
                        case IndirectRegOp.None:
                        case IndirectRegOp.INDF:
                        case IndirectRegOp.PLUSW:
                            m.Assign(derefptrd, derefptrs);
                            break;

                        case IndirectRegOp.POSTDEC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.ISub(adrfd, 1));
                            break;

                        case IndirectRegOp.POSTINC:
                            m.Assign(derefptrd, derefptrs);
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            break;

                        case IndirectRegOp.PREINC:
                            m.Assign(adrfd, m.IAdd(adrfd, 1));
                            m.Assign(derefptrd, derefptrs);
                            break;
                    }
                    break;
            }

        }

        private void RewriteMOVLB()
        {
            var bsr = binder.EnsureRegister(PIC18Registers.BSR);
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(bsr, k);
        }

        private void RewriteMOVLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, k);
        }

        private void RewriteMOVSF()
        {
            var zs = GetFSR2IdxAddress(instrCurr.op1);
            var (indop, adr) = GetUnaryAbsPtrs(instrCurr.op2, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, zs);
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, zs);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, zs);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, zs);
                    break;
            }
        }

        private void RewriteMOVSS()
        {
            var zs = GetFSR2IdxAddress(instrCurr.op1);
            var zd = GetFSR2IdxAddress(instrCurr.op2);
            m.Assign(zd, zs);
        }

        private void RewriteMOVWF()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, Wreg);
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, Wreg);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, Wreg);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, Wreg);
                    break;
            }
        }

        private void RewriteMULLW()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(prod, m.UMul(Wreg, k));
        }

        private void RewriteMULWF()
        {
            var prod = binder.EnsureRegister(PIC18Registers.PROD);
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(prod, m.UMul(derefptr, Wreg));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(prod, m.UMul(derefptr, Wreg));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(prod, m.UMul(derefptr, Wreg));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(prod, m.UMul(derefptr, Wreg));
                    break;
            }
        }

        private void RewriteNEGF()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, m.Neg(derefptr));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, m.Neg(derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, m.Neg(derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, m.Neg(derefptr));
                    break;
            }
            SetStatusFlags(derefptr);
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
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(DataMem8(Fsr2), k);
            m.Assign(Fsr2, m.IAdd(Fsr2, 1));
            SetStatusFlags(Fsr2);
        }

        private void RewriteRCALL()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;

            var target = GetProgramAddress(instrCurr.op1);
            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target, 0);
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

            var k = GetImmediateValue(instrCurr.op1);
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Wreg, k);
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteRETURN()
        {
            rtlc = RtlClass.Transfer;

            Identifier tos = binder.EnsureRegister(PIC18Registers.TOS);
            Constant shad = GetShadow(instrCurr.op1);
            Identifier statuss = binder.EnsureRegister(PIC18Registers.STATUS_CSHAD);

            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            if (shad.ToBoolean() && !(statuss is null))
            {
                Identifier wregs = binder.EnsureRegister(PIC18Registers.WREG_CSHAD);
                Identifier bsrs = binder.EnsureRegister(PIC18Registers.BSR_CSHAD);
                m.Assign(binder.EnsureRegister(PIC18Registers.BSR), bsrs);
                m.Assign(Wreg, wregs);
                m.Assign(binder.EnsureRegister(PIC18Registers.STATUS), statuss);
            }
            m.Return(0, 0);
        }

        private void RewriteRLCF()
        {
            //TODO:  PseudoProcedure(__rlcf) ?

            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
            var carry = FlagGroup(FlagM.C);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlcf", PrimitiveType.Byte, derefptr, carry)));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlcf", PrimitiveType.Byte, derefptr, carry)));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlcf", PrimitiveType.Byte, derefptr, carry)));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlcf", PrimitiveType.Byte, derefptr, carry)));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

            SetStatusFlags(dst);
        }

        private void RewriteRLNCF()
        {
            //TODO:  PseudoProcedure(__rlncf) ?

            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlncf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlncf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlncf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rlncf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

            SetStatusFlags(dst);
        }

        private void RewriteRRCF()
        {
            //TODO:  PseudoProcedure(__rrcf) ?

            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
            var carry = FlagGroup(FlagM.C);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrcf", PrimitiveType.Byte, derefptr, carry)));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrcf", PrimitiveType.Byte, derefptr, carry)));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrcf", PrimitiveType.Byte, derefptr, carry)));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrcf", PrimitiveType.Byte, derefptr, carry)));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteRRNCF()
        {
            //TODO:  PseudoProcedure(__rrncf) ?

            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrncf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrncf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrncf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__rrncf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

            SetStatusFlags(dst);
        }

        private void RewriteSETF()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(derefptr, 255);
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(derefptr, 255);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(derefptr, 255);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(derefptr, 255);
                    break;
            }
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
            var fsr = GetFSRNum(instrCurr.op1);
            var k = GetImmediateValue(instrCurr.op2);
            m.Assign(fsr, m.ISub(fsr, k));
            SetStatusFlags(fsr);
        }

        private void RewriteSUBFWB()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
            var borrow = m.Not(FlagGroup(FlagM.C));

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(m.ISub(Wreg, derefptr), borrow));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(m.ISub(Wreg, derefptr), borrow));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(m.ISub(Wreg, derefptr), borrow));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(m.ISub(Wreg, derefptr), borrow));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

            }
            SetStatusFlags(dst);
        }

        private void RewriteSUBLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.ISub(k, Wreg));
            SetStatusFlags(Wreg);
        }

        private void RewriteSUBULNK()
        {
            rtlc = RtlClass.Transfer;

            var k = GetImmediateValue(instrCurr.op1);
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Fsr2, m.ISub(Fsr2, k));
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            m.Return(0, 0);
        }

        private void RewriteSUBWF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(derefptr, Wreg));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(derefptr, Wreg));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(derefptr, Wreg));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(derefptr, Wreg));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteSUBWFB()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);
            var borrow = m.Not(FlagGroup(FlagM.C));

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.ISub(m.ISub(derefptr, Wreg), borrow));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.ISub(m.ISub(derefptr, Wreg), borrow));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.ISub(m.ISub(derefptr, Wreg), borrow));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.ISub(m.ISub(derefptr, Wreg), borrow));
                    break;
            }
            SetStatusFlags(dst);
        }

        private void RewriteSWAPF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, derefptr)));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, derefptr)));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

        }

        private void RewriteTBLRD()
        {
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            var tblmode = GetTBLRWMode(instrCurr.op1);
            m.SideEffect(host.PseudoProcedure("__tblrd", VoidType.Instance, tblptr, tblmode));
        }

        private void RewriteTBLWT()
        {
            var tblptr = binder.EnsureRegister(PIC18Registers.TBLPTR);
            var tblmode = GetTBLRWMode(instrCurr.op1);
            m.SideEffect(host.PseudoProcedure("__tblwt", VoidType.Instance, tblptr, tblmode));
        }

        private void RewriteTSTFSZ()
        {
            var (indop, adr) = GetUnaryPtrs(instrCurr.op1, out Expression derefptr);
            rtlc = RtlClass.ConditionalTransfer;

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Branch(m.Eq0(derefptr), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Branch(m.Eq0(derefptr), SkipToAddr(), rtlc);
                    break;

                case IndirectRegOp.POSTINC:
                    m.Branch(m.Eq0(derefptr), SkipToAddr(), rtlc);
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Branch(m.Eq0(derefptr), SkipToAddr(), rtlc);
                    m.Assign(adr, m.ISub(adr, 1));
                    break;
            }

        }

        private void RewriteXORLW()
        {
            var src = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.Xor(Wreg, src));
            SetStatusFlags(Wreg);
        }

        private void RewriteXORWF()
        {
            var (indop, adr) = GetBinaryPtrs(instrCurr.op1, out Expression derefptr, out Expression dst);

            switch (indop)
            {
                case IndirectRegOp.None:
                case IndirectRegOp.INDF:
                case IndirectRegOp.PLUSW:
                    m.Assign(dst, m.Xor(Wreg, derefptr));
                    break;

                case IndirectRegOp.POSTDEC:
                    m.Assign(dst, m.Xor(Wreg, derefptr));
                    m.Assign(adr, m.ISub(adr, 1));
                    break;

                case IndirectRegOp.POSTINC:
                    m.Assign(dst, m.Xor(Wreg, derefptr));
                    m.Assign(adr, m.IAdd(adr, 1));
                    break;

                case IndirectRegOp.PREINC:
                    m.Assign(adr, m.IAdd(adr, 1));
                    m.Assign(dst, m.Xor(Wreg, derefptr));
                    break;
            }
            SetStatusFlags(dst);
        }

        #endregion

    }

}
