#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Libraries.Microchip;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    public class PIC16EnhancedRewriter : PIC16BasicRewriter
    {

        private PIC16EnhancedRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public new static PICRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PIC16EnhancedRewriter(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                dasm ?? throw new ArgumentNullException(nameof(dasm)),
                state ?? throw new ArgumentNullException(nameof(state)),
                binder ?? throw new ArgumentNullException(nameof(binder)),
                host ?? throw new ArgumentNullException(nameof(host))
              );
        }

        /// <summary>
        /// Actual instruction rewriter method for Enhanced PIC16.
        /// </summary>
        protected override void RewriteInstr()
        {
            switch (instrCurr.Mnemonic)
            {
                default:
                    base.RewriteInstr();
                    break;

                case Mnemonic.ADDFSR:
                    Rewrite_ADDFSR();
                    break;
                case Mnemonic.ADDWFC:
                    Rewrite_ADDWFC();
                    break;
                case Mnemonic.ASRF:
                    Rewrite_ASRF();
                    break;
                case Mnemonic.BRA:
                    Rewrite_BRA();
                    break;
                case Mnemonic.BRW:
                    Rewrite_BRW();
                    break;
                case Mnemonic.CALLW:
                    Rewrite_CALLW();
                    break;
                case Mnemonic.LSLF:
                    Rewrite_LSLF();
                    break;
                case Mnemonic.LSRF:
                    Rewrite_LSRF();
                    break;
                case Mnemonic.MOVIW:
                    Rewrite_MOVIW();
                    break;
                case Mnemonic.MOVLB:
                    Rewrite_MOVLB();
                    break;
                case Mnemonic.MOVLP:
                    Rewrite_MOVLP();
                    break;
                case Mnemonic.MOVWI:
                    Rewrite_MOVWI();
                    break;
                case Mnemonic.RESET:
                    Rewrite_RESET();
                    break;
                case Mnemonic.SUBWFB:
                    Rewrite_SUBWFB();
                    break;
                case Mnemonic.TRIS:
                    Rewrite_TRIS();
                    break;
            }

        }


        private void Rewrite_ADDFSR()
        {
            var fsrnum = instrCurr.Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR register number operand: {instrCurr.Operands[0]}");
            var imm = instrCurr.Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[1]}");
            Identifier fsrreg;
            switch (fsrnum.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR1);
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
            GetSrcAndDst(out var srcmem, out var dstmem);
            var carry = FlagGroup(FlagM.C);
            m.Assign(dstmem, m.IAdd(m.IAdd(Wreg, srcmem), carry));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_ASRF()
        {
            GetSrcAndDst(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__asrf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_BRA()
        {
            rtlc = InstrClass.Transfer;
            var target = instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid program address operand: {instrCurr.Operands[0]}");
            m.Goto(target.CodeTarget);
        }

        private void Rewrite_BRW()
        {
            rtlc = InstrClass.Transfer;
            Address nextAddr = instrCurr.Address + instrCurr.Length;
            m.Goto(m.IAdd(nextAddr, Wreg));
        }

        private void Rewrite_CALLW()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            var pclath = binder.EnsureRegister(PICRegisters.PCLATH);
            var target = m.IAdd(m.Shl(pclath, 8), Wreg);
            Address retaddr = instrCurr.Address + instrCurr.Length;
            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Call(target, 0);
        }

        private void Rewrite_LSLF()
        {
            GetSrcAndDst(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__lslf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_LSRF()
        {
            GetSrcAndDst(out var srcmem, out var dstmem);
            m.Assign(dstmem, m.Fn(host.PseudoProcedure("__lsrf", PrimitiveType.Byte, srcmem)));
            SetStatusFlags(dstmem);
        }

        private void Rewrite_MOVIW()
        {
            var fsridx = instrCurr.Operands[0] as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR-indexed operand: {instrCurr.Operands[0]}");
            Identifier fsrreg;
            switch (fsridx.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR1);
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
                    if (fsridx.Offset.IsNegative)
                    {
                        m.Assign(Wreg, DataMem8(m.ISub(fsrreg, fsridx.Offset.Negate())));
                    }
                    else
                    {
                        m.Assign(Wreg, DataMem8(m.IAdd(fsrreg, fsridx.Offset)));
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FSR-indexed mode: {fsridx.Mode}");
            }

            SetStatusFlags(Wreg);
        }

        private void Rewrite_MOVLB()
        {
            var imm = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Bsr, imm.ImmediateValue);
        }

        private void Rewrite_MOVLP()
        {
            var imm = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            var pclath = binder.EnsureRegister(PIC16Registers.PCLATH);
            m.Assign(pclath, imm.ImmediateValue);
        }

        private void Rewrite_MOVWI()
        {
            var fsridx = instrCurr.Operands[0] as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR-indexed operand: {instrCurr.Operands[0]}");
            Identifier fsrreg;
            switch (fsridx.FSRNum)
            {
                case 0:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR0);
                    break;
                case 1:
                    fsrreg = binder.EnsureRegister(PIC16EnhancedRegisters.FSR1);
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
                    if (fsridx.Offset.IsNegative)
                    {
                        m.Assign(DataMem8(m.ISub(fsrreg, fsridx.Offset.Negate())), Wreg);
                    }
                    else
                    {
                        m.Assign(DataMem8(m.IAdd(fsrreg, fsridx.Offset)), Wreg);
                    }
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
            GetSrcAndDst(out var srcmem, out var dstmem);
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
