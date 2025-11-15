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
using Reko.Core.Types;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core.Lib;
using Reko.Core.Intrinsics;
using Reko.Core.Serialization;

namespace Reko.Arch.MicrochipPIC.Common
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
        protected InstrClass iclass;
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
            this.instrCurr = null!;
            this.m = null!;
            this.rtlInstructions = null!;
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
                iclass = instrCurr.InstructionClass;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);

                RewriteInstr();

                yield return m.MakeCluster(instrCurr.Address, instrCurr.Length, iclass);
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
            => binder.EnsureFlagGroup(
                new (
                    PICRegisters.STATUS, 
                    (uint)flags, 
                    arch.GrfToString(PICRegisters.STATUS, 
                    "",
                    (uint)flags)));

        protected MemoryAccess DataMem8(Expression ea)
            => m.Mem(PICRegisters.GlobalData, PrimitiveType.Byte, ea);

        protected MemoryAccess DataBankMem8(Expression bsr, Expression ea)
            => m.Mem(
                PICRegisters.GlobalData,
                PrimitiveType.Byte,
                SegmentedPointer.Create(bsr, ea));

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
            iclass = InstrClass.ConditionalTransfer;
            switch (indMode)
            {
                case FSRIndexedMode.None:
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), iclass);
                    break;

                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), iclass);
                    break;

                case FSRIndexedMode.POSTDEC:
                    m.Assign(dst, src);
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), iclass);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTINC:
                    m.Assign(dst, src);
                    m.BranchInMiddleOfInstruction(cond, SkipToAddr(), iclass);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    m.Assign(dst, src);
                    m.Branch(cond, SkipToAddr(), iclass);
                    break;
            }
        }

        protected Address SkipToAddr()
            => instrCurr.Address + instrCurr.Length + 2;

        protected void CondBranch(TestCondition test)
        {
            iclass = InstrClass.ConditionalTransfer;
            if (instrCurr.Operands[0] is PICOperandProgMemoryAddress brop)
            {
                m.Branch(test, PICProgAddress.Ptr(brop.CodeTarget.ToUInt32()), iclass);
                return;
            }
            throw new InvalidOperationException($"Wrong PIC program relative address: op1={instrCurr.Operands[0]}.");
        }

        protected void CondSkipIndirect(Expression cond, FSRIndexedMode indMode, Expression memPtr)
        {
            iclass = InstrClass.ConditionalTransfer;
            switch (indMode)
            {
                case FSRIndexedMode.None:
                case FSRIndexedMode.INDF:
                case FSRIndexedMode.PLUSW:
                    m.Branch(cond, SkipToAddr(), iclass);
                    break;

                case FSRIndexedMode.POSTINC:
                    m.Branch(cond, SkipToAddr(), iclass);
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    break;

                case FSRIndexedMode.POSTDEC:
                    m.Branch(cond, SkipToAddr(), iclass);
                    m.Assign(memPtr, m.ISub(memPtr, 1));
                    break;

                case FSRIndexedMode.PREINC:
                    m.Assign(memPtr, m.IAdd(memPtr, 1));
                    m.Branch(cond, SkipToAddr(), iclass);
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
                        var srciopr = PICRegisters.IndirectOpMode(regsrc, out PICRegisterStorage indsrcreg);
                        if (srciopr != FSRIndexedMode.None)
                            return (srciopr, binder.EnsureRegister(indsrcreg));
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
                    if (absmem.DataTarget == PICDataAddress.Invalid)
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
                    return (FSRIndexedMode.None, DataMem8(absmem.DataTarget.ToAddress()));

                default:
                    throw new InvalidOperationException($"Invalid data absolute address operand.");
            }
        }

        /// <summary>
        /// Gets the source/destination direct address (SFR register or memory), addressing mode and actual source memory access
        /// of a single operand instruction. (e.g. "SETF f,a" or "CLRF f").
        /// </summary>
        /// <param name="opernd">The instruction memory operand.</param>
        /// <param name="memExpr">[out] The source/destination pointer expression to access memory.</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        protected (FSRIndexedMode indMode, Expression memPtr) GetUnaryPtrs(MachineOperand opernd, out Expression memExpr)
        {
            var (indMode, memPtr) = GetMemFileAccess(opernd);
            switch (indMode)
            {
                case FSRIndexedMode.None:    // Direct mode
                    memExpr = memPtr;
                    break;

                case FSRIndexedMode.INDF:    // Indirect modes
                case FSRIndexedMode.POSTDEC:
                case FSRIndexedMode.POSTINC:
                case FSRIndexedMode.PREINC:
                    memExpr = DataMem8(memPtr);
                    break;

                case FSRIndexedMode.PLUSW:   // Indirect-indexed mode
                    memExpr = DataMem8(m.IAdd(memPtr, Wreg));
                    break;

                default:
                    throw new InvalidOperationException("Unable to create indirect pointer expression.");
            }

            return (indMode, memPtr);
        }

        /// <summary>
        /// Gets the source address (SFR register or memory), addressing mode, actual source memory access
        /// and actual destination access of a dual operand instruction (e.g. "ADDWF f,d,a").
        /// </summary>
        /// <param name="opernd">The instruction operand.</param>
        /// <param name="memExpr">[out] The source pointer expression to access memory.</param>
        /// <param name="dst">[out] Destination access expression (WREG or memory access).</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        protected (FSRIndexedMode indMode, Expression memPtr) GetBinaryPtrs(out Expression memExpr, out Expression dst)
        {
            bool DestIsWreg(MachineOperand opernd)
            {
                switch (opernd)
                {
                    case PICOperandMemWRegDest wreg:
                        return wreg.WRegIsDest;
                }
                return false;
            }

            var (indMode, memPtr) = GetUnaryPtrs(instrCurr.Operands[0], out memExpr);
            dst = (DestIsWreg(instrCurr.Operands[1]) ? Wreg : memExpr);
            return (indMode, memPtr);
        }

        /// <summary>
        /// Gets the source/destination absolute address (SFR register or memory), addressing mode and actual memory access
        /// of an absolute address instruction. (e.g. the <code>"fd"</code> of <code>"MOVSF zs,fd"</code>).
        /// </summary>
        /// <param name="opernd">The instruction operand.</param>
        /// <param name="memExpr">[out] The source/destination pointer expression to access memory.</param>
        /// <returns>
        /// Addressing mode and source memory/register.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the instruction operand is invalid.</exception>
        protected (FSRIndexedMode indMode, Expression adr) GetUnaryAbsPtrs(MachineOperand opernd, out Expression memExpr)
        {
            var (indMode, memPtr) = GetDataAbsAddress(opernd);
            switch (indMode)
            {
                case FSRIndexedMode.None:    // Direct mode
                    memExpr = memPtr;
                    break;

                case FSRIndexedMode.INDF:    // Indirect modes
                case FSRIndexedMode.POSTDEC:
                case FSRIndexedMode.POSTINC:
                case FSRIndexedMode.PREINC:
                    memExpr = DataMem8(memPtr);
                    break;

                case FSRIndexedMode.PLUSW:   // Indirect-indexed mode
                    memExpr = DataMem8(m.IAdd(memPtr, Wreg));
                    break;

                default:
                    throw new InvalidOperationException($"Unable to adjust indirect pointer. Indexation mode: {indMode}");
            }

            return (indMode, memPtr);
        }

        protected static readonly IntrinsicProcedure asrf_intrinsic = IntrinsicBuilder.Pure("__asrf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure callw_intrinsic = IntrinsicBuilder.Pure("__callw")
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Word16);
        protected static readonly IntrinsicProcedure daw_intrinsic = IntrinsicBuilder.Pure("__daw")
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Bool)
            .Param(PrimitiveType.Bool)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure lslf_intrinsic = IntrinsicBuilder.Pure("__lslf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure lsrf_intrinsic = IntrinsicBuilder.Pure("__lsrf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure reset_intrinsic = new IntrinsicBuilder("__reset", true, new()
            {
                Terminates = true,
            })
            .Void();
        protected static readonly IntrinsicProcedure rlf_intrinsic = IntrinsicBuilder.Pure("__rlf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure rrf_intrinsic = IntrinsicBuilder.Pure("__rrf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure rlcf_intrinsic = IntrinsicBuilder.Pure("__rlcf")
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Bool)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure rlncf_intrinsic = IntrinsicBuilder.Pure("__rlncf")
             .Param(PrimitiveType.Byte)
             .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure rrcf_intrinsic = IntrinsicBuilder.Pure("__rrcf")
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Bool)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure rrncf_intrinsic = IntrinsicBuilder.Pure("__rrncf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        protected static readonly IntrinsicProcedure swapf_intrinsic = IntrinsicBuilder.Pure("__swapf")
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
    }
}
