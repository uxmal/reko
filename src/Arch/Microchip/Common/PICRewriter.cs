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
using Reko.Core.Types;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// A PIC IL rewriter abstract/helper class.
    /// </summary>
    public abstract class PICRewriter : IEnumerable<RtlInstructionCluster>
    {
        protected PICArchitecture arch;
        protected PICDisassemblerBase disasm;
        protected PICProcessorState state;
        protected IStorageBinder binder;
        protected IRewriterHost host;

        protected IEnumerator<PICInstruction> dasm;
        protected PICInstruction instrCurr;
        protected RtlClass rtlc;
        protected List<RtlInstruction> rtlInstructions;
        protected RtlEmitter m;
        protected Identifier Wreg;    // cached WREG register identifier
        protected Identifier Bsr;     // cached BSR register identifier

        protected PICRewriter(PICArchitecture arch, PICDisassemblerBase disasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.disasm = disasm;
            dasm = disasm.GetEnumerator();
            Wreg = GetWReg;
            Bsr = GetBsr;
        }

        /// <summary>
        /// Gets the working register (WREG) which is often used implicitly by PIC instructions.
        /// </summary>
        protected Identifier GetWReg => binder.EnsureRegister(PICRegisters.WREG);

        /// <summary>
        /// Gets the bank select register (BSR) which is often used for PIC data addresses.
        /// </summary>
        protected Identifier GetBsr => binder.EnsureRegister(PICRegisters.BSR);

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCurr = dasm.Current;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);

                RewriteInstr();

                yield return new RtlInstructionCluster(instrCurr.Address, instrCurr.Length, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Actual instruction rewriter method must be implemented by derived classes.
        /// </summary>
        protected abstract void RewriteInstr();

        protected abstract void SetStatusFlags(Expression dst);

        protected Identifier FlagGroup(FlagM flags)
            => binder.EnsureFlagGroup(PICRegisters.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);

        protected static MemoryAccess DataMem8(Expression ea)
            => new MemoryAccess(PICRegisters.GlobalData, ea, PrimitiveType.Byte);

        protected static SegmentedAccess DataBankMem8(Expression bsr, Expression ea)
            => new SegmentedAccess(PICRegisters.GlobalData, bsr, ea, PrimitiveType.Byte);

        protected ArrayAccess PushToHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            var slot = m.ARef(PrimitiveType.Ptr32, PICRegisters.GlobalStack, stkptr);
            m.Assign(stkptr, m.IAdd(stkptr, Constant.Byte(1)));
            return slot;
        }

        protected ArrayAccess PopFromHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, m.ISub(stkptr, Constant.Byte(1)));
            var slot = m.ARef(PrimitiveType.Ptr32, PICRegisters.GlobalStack, stkptr);
            return slot;
        }

        protected Constant GetBitMask(MachineOperand op, bool revert)
        {
            switch (op)
            {
                case PICOperandMemBitNo bitno:
                    int mask = (1 << bitno.BitNo);
                    if (revert)
                        mask = ~mask;
                    return Constant.Byte((byte)mask);

                default:
                    throw new InvalidOperationException($"Invalid bit number operand: {op}.");
            }
        }

        protected void ArithAssign(Expression dst, Expression src)
        {
            m.Assign(dst, src);
            SetStatusFlags(dst);
        }

        protected void ArithAssignIndirect(Expression dst, Expression src, FSRIndexedMode indMode, Expression memPtr)
        {
            switch (indMode)
            {
                case FSRIndexedMode.None:
                    ArithAssign(dst, src);
                    break;

                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    ArithAssign(dst, src);
                    break;

                case FSRIndexedMode.POSTDEC:
                    ArithAssign(dst, src);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    ArithAssign(dst, src);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    ArithAssign(dst, src);
                    break;
            }
        }

        protected void ArithCondSkip(Expression dst, Expression src, Expression cond, FSRIndexedMode indMode, Expression memPtr)
        {
            rtlc = RtlClass.ConditionalTransfer;
            switch (indMode)
            {
                case FSRIndexedMode.None:
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), rtlc);
                    break;

                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), rtlc);
                    break;

                case FSRIndexedMode.POSTDEC:
                    m.Assign(dst, src);
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), rtlc);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    m.Assign(dst, src);
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), rtlc);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), rtlc);
                    break;
            }
        }

        protected PICProgAddress SkipToAddr()
            => PICProgAddress.Ptr(instrCurr.Address + instrCurr.Length + 2);

        protected void CondBranch(TestCondition test)
        {
            rtlc = RtlClass.ConditionalTransfer;
            if (instrCurr.op1 is PICOperandProgMemoryAddress brop)
            {
                m.Branch(test, PICProgAddress.Ptr(brop.CodeTarget.ToUInt32()), rtlc);
                return;
            }
            throw new InvalidOperationException($"Wrong PIC program relative address: op1={instrCurr.op1}.");
        }

        protected void CondSkipIndirect(Expression cond, FSRIndexedMode indMode, Expression memPtr)
        {
            rtlc = RtlClass.ConditionalTransfer;
            switch (indMode)
            {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    m.Branch(cond, SkipToAddr(), rtlc);
                    break;

                case FSRIndexedMode.POSTINC:
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), rtlc);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTDEC:
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), rtlc);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    m.Branch(cond, SkipToAddr(), rtlc);
                    break;
            }
        }

        protected virtual (FSRIndexedMode indMode, Expression memPtr) GetMemFileAccess(MachineOperand opernd)
        {
            switch (opernd)
            {
                case PICOperandBankedMemory bnkmem:
                    var offset = bnkmem.Offset;
                    if (PICRegisters.TryGetAlwaysAccessibleRegister(offset, out var regsrc))
                    {
//                        var srciopr = PICRegisters.IndirectOpMode(regsrc, out PICRegisterStorage indsrcreg);
//                        if (srciopr != FSRIndexedMode.None)
//                            return (srciopr, binder.EnsureRegister(indsrcreg));
                        return (FSRIndexedMode.None, binder.EnsureRegister(regsrc));
                    }
                    return (FSRIndexedMode.None, DataBankMem8(Bsr, Constant.Byte(offset)));

                case PICOperandRegister reg:
                    var iopr = PICRegisters.IndirectOpMode(reg.Register, out PICRegisterStorage indreg);
                    if (iopr != FSRIndexedMode.None)
                        return (iopr, binder.EnsureRegister(indreg));
                    return (iopr, binder.EnsureRegister(reg.Register));

                default:
                    throw new InvalidOperationException($"Invalid PIC instruction's memory operand: {opernd}");

            }
        }

        protected (FSRIndexedMode indMode, Expression memPtr) GetDataAbsAddress(MachineOperand opernd)
        {
            switch (opernd)
            {
                case PICOperandDataMemoryAddress absmem:
                    if (absmem.DataTarget == PICAddress.Invalid)
                        throw new InvalidOperationException($"Invalid data absolute address operand.");

                    if (PICRegisters.TryGetRegister(absmem.DataTarget, out var sfrReg))
                    {
                        if (sfrReg != PICRegisterStorage.None)
                        {
                            var imode = PICRegisters.IndirectOpMode(sfrReg, out PICRegisterStorage fsr);
                            if (imode != FSRIndexedMode.None)
                                return (imode, binder.EnsureRegister(fsr));
                            return (imode, binder.EnsureRegister(sfrReg));
                        }
                    }
                    return (FSRIndexedMode.None, DataMem8(PICDataAddress.Ptr(absmem.DataTarget)));

                default:
                    throw new InvalidOperationException($"Invalid data absolute address operand.");
            }
        }

    }

}
