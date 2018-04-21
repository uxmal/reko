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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public class PIC16FullRewriter : PIC16BasicRewriter
    {

        private PIC16FullRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public new static PICRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PIC16FullRewriter(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                dasm ?? throw new ArgumentNullException(nameof(dasm)),
                state ?? throw new ArgumentNullException(nameof(state)),
                binder ?? throw new ArgumentNullException(nameof(binder)),
                host ?? throw new ArgumentNullException(nameof(host))
              );
        }

        /// <summary>
        /// Actual instruction rewriter method for Full-Featured PIC16.
        /// </summary>
        protected override void RewriteInstr()
        {
            switch (instrCurr.Opcode)
            {
                default:
                    base.RewriteInstr();
                    break;

                case Opcode.ADDFSR:
                    Rewrite_ADDFSR();
                    break;
                case Opcode.ADDWFC:
                    Rewrite_ADDWFC();
                    break;
                case Opcode.ASRF:
                    Rewrite_ASRF();
                    break;
                case Opcode.BRA:
                    Rewrite_BRA();
                    break;
                case Opcode.BRW:
                    Rewrite_BRW();
                    break;
                case Opcode.CALLW:
                    Rewrite_CALLW();
                    break;
                case Opcode.LSLF:
                    Rewrite_LSLF();
                    break;
                case Opcode.LSRF:
                    Rewrite_LSRF();
                    break;
                case Opcode.MOVIW:
                    Rewrite_MOVIW();
                    break;
                case Opcode.MOVLB:
                    Rewrite_MOVLB();
                    break;
                case Opcode.MOVLP:
                    Rewrite_MOVLP();
                    break;
                case Opcode.MOVWI:
                    Rewrite_MOVWI();
                    break;
                case Opcode.RESET:
                    Rewrite_RESET();
                    break;
                case Opcode.SUBWFB:
                    Rewrite_SUBWFB();
                    break;
                case Opcode.TRIS:
                    Rewrite_TRIS();
                    break;
            }

        }

        private void Rewrite_ADDFSR()
        {
            var fsrnum = instrCurr.op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR register number operand: {instrCurr.op1}");
            var imm = instrCurr.op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op2}");
            Identifier fsrreg;
            switch (fsrnum.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR1);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR number: {fsrnum.FSRNum}.");
            }
            if (imm.ImmediateValue.IsNegative)
            {
                m.Assign(fsrreg, m.ISub(fsrreg, imm.ImmediateValue.Negate()));
            }
            else
            {
                m.Assign(fsrreg, m.IAdd(fsrreg, imm.ImmediateValue));
            }
        }

        private void Rewrite_ADDWFC()
        {
            GetSrcAndDest(out var srcmem, out var dstmem);
            var carry = FlagGroup(FlagM.C);
            m.Assign(dstmem, m.IAdd(m.IAdd(Wreg, srcmem), carry));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_ASRF()
        {
            GetSrcAndDest(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__asrf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_BRA()
        {
            rtlc = RtlClass.Transfer;
            var target = instrCurr.op1 as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid program address operand: {instrCurr.op1}");
            m.Goto(target.CodeTarget);
        }

        private void Rewrite_BRW()
        {
            rtlc = RtlClass.Transfer;
            Address nextAddr = instrCurr.Address + instrCurr.Length;
            m.Goto(m.IAdd(nextAddr, Wreg));
        }

        private void Rewrite_CALLW()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var pclath = binder.EnsureRegister(PICRegisters.PCLATH);
            var target = m.IAdd(m.Shl(pclath, 8), Wreg);
            Address retaddr = instrCurr.Address + instrCurr.Length;
            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Call(target, 0);
        }

        private void Rewrite_LSLF()
        {
            GetSrcAndDest(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__lslf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_LSRF()
        {
            GetSrcAndDest(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__lsrf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_MOVIW()
        {
            var fsridx = instrCurr.op1 as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR-indexed operand: {instrCurr.op1}");
            Identifier fsrreg;
            switch (fsridx.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR1);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR number: {fsridx.FSRNum}.");
            }
            switch (fsridx.Mode)
            {
                case FSRIndexedMode.POSTDEC:
                    m.Assign(Wreg, DataMem8(fsrreg));
                    m.Assign(fsrreg, m.ISub(fsrreg, 1));
                    break;
                case FSRIndexedMode.POSTINC:
                    m.Assign(Wreg, DataMem8(fsrreg));
                    m.Assign(fsrreg, m.IAdd(fsrreg, 1));
                    break;
                case FSRIndexedMode.PREDEC:
                    m.Assign(fsrreg, m.ISub(fsrreg, 1));
                    m.Assign(Wreg, DataMem8(fsrreg));
                    break;
                case FSRIndexedMode.PREINC:
                    m.Assign(fsrreg, m.IAdd(fsrreg, 1));
                    m.Assign(Wreg, DataMem8(fsrreg));
                    break;
                case FSRIndexedMode.INDEXED:
                    m.Assign(Wreg, DataMem8(m.IAdd(fsrreg, fsridx.Offset)));
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR-indexed mode: {fsridx.Mode}");
            }

            SetStatusFlags(Wreg);

        }

        private void Rewrite_MOVLB()
        {
            var imm = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            var bsr = binder.EnsureRegister(PIC16FullRegisters.BSR);
            m.Assign(bsr, imm.ImmediateValue);
        }

        private void Rewrite_MOVLP()
        {
            var imm = instrCurr.op1 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.op1}");
            var pclath = binder.EnsureRegister(PIC16Registers.PCLATH);
            m.Assign(pclath, imm.ImmediateValue);
        }

        private void Rewrite_MOVWI()
        {
            var fsridx = instrCurr.op1 as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR-indexed operand: {instrCurr.op1}");
            Identifier fsrreg;
            switch (fsridx.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16FullRegisters.FSR1);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR number: {fsridx.FSRNum}.");
            }
            switch (fsridx.Mode)
            {
                case FSRIndexedMode.POSTDEC:
                    m.Assign(DataMem8(fsrreg), Wreg);
                    m.Assign(fsrreg, m.ISub(fsrreg, 1));
                    break;
                case FSRIndexedMode.POSTINC:
                    m.Assign(DataMem8(fsrreg), Wreg);
                    m.Assign(fsrreg, m.IAdd(fsrreg, 1));
                    break;
                case FSRIndexedMode.PREDEC:
                    m.Assign(fsrreg, m.ISub(fsrreg, 1));
                    m.Assign(DataMem8(fsrreg), Wreg);
                    break;
                case FSRIndexedMode.PREINC:
                    m.Assign(fsrreg, m.IAdd(fsrreg, 1));
                    m.Assign(DataMem8(fsrreg), Wreg);
                    break;
                case FSRIndexedMode.INDEXED:
                    m.Assign(DataMem8(m.IAdd(fsrreg, fsridx.Offset)), Wreg);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR-indexed mode: {fsridx.Mode}");
            }

        }

        private void Rewrite_RESET()
        {
            m.Nop();
        }

        private void Rewrite_SUBWFB()
        {
            GetSrcAndDest(out var srcmem, out var dstmem);
            var borrow = m.Not(FlagGroup(FlagM.C));
            m.Assign(dstmem, m.ISub(m.ISub(srcmem, Wreg), borrow));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_TRIS()
        {
            m.Nop();
        }

    }

}
