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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    public abstract class PIC16RewriterBase : PICRewriter
    {

        protected PIC16RewriterBase(PICArchitecture arch, PICDisassemblerBase disasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, disasm, state, binder, host)
        {
        }

        /// <summary>
        /// Actual instruction rewriter method for all PIC16 families.
        /// </summary>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        protected override void RewriteInstr()
        {
            var addr = instrCurr.Address;
            var len = instrCurr.Length;

            switch (instrCurr.Mnemonic)
            {
            default:
                host.Warn(
                    instrCurr.Address,
                    $"PIC16 instruction {instrCurr.Mnemonic}' is not supported yet.");
                goto case Mnemonic.invalid;
            case Mnemonic.invalid:
            case Mnemonic.unaligned:
                m.Invalid();
                break;

                case Mnemonic.ADDLW:
                    Rewrite_ADDLW();
                    break;
                case Mnemonic.ADDWF:
                    Rewrite_ADDWF();
                    break;
                case Mnemonic.ANDLW:
                    Rewrite_ANDLW();
                    break;
                case Mnemonic.ANDWF:
                    Rewrite_ANDWF();
                    break;
                case Mnemonic.BCF:
                    Rewrite_BCF();
                    break;
                case Mnemonic.BSF:
                    Rewrite_BSF();
                    break;
                case Mnemonic.BTFSC:
                    Rewrite_BTFSC();
                    break;
                case Mnemonic.BTFSS:
                    Rewrite_BTFSS();
                    break;
                case Mnemonic.CALL:
                    Rewrite_CALL();
                    break;
                case Mnemonic.CLRF:
                    Rewrite_CLRF();
                    break;
                case Mnemonic.CLRW:
                    Rewrite_CLRW();
                    break;
                case Mnemonic.CLRWDT:
                    Rewrite_CLRWDT();
                    break;
                case Mnemonic.COMF:
                    Rewrite_COMF();
                    break;
                case Mnemonic.DECF:
                    Rewrite_DECF();
                    break;
                case Mnemonic.DECFSZ:
                    Rewrite_DECFSZ();
                    break;
                case Mnemonic.GOTO:
                    Rewrite_GOTO();
                    break;
                case Mnemonic.INCF:
                    Rewrite_INCF();
                    break;
                case Mnemonic.INCFSZ:
                    Rewrite_INCFSZ();
                    break;
                case Mnemonic.IORLW:
                    Rewrite_IORLW();
                    break;
                case Mnemonic.IORWF:
                    Rewrite_IORWF();
                    break;
                case Mnemonic.MOVF:
                    Rewrite_MOVF();
                    break;
                case Mnemonic.MOVLW:
                    Rewrite_MOVLW();
                    break;
                case Mnemonic.MOVWF:
                    Rewrite_MOVWF();
                    break;
                case Mnemonic.NOP:
                    m.Nop();
                    break;
                case Mnemonic.RETFIE:
                    Rewrite_RETFIE();
                    break;
                case Mnemonic.RETLW:
                    Rewrite_RETLW();
                    break;
                case Mnemonic.RETURN:
                    Rewrite_RETURN();
                    break;
                case Mnemonic.RLF:
                    Rewrite_RLF();
                    break;
                case Mnemonic.RRF:
                    Rewrite_RRF();
                    break;
                case Mnemonic.SLEEP:
                    Rewrite_SLEEP();
                    break;
                case Mnemonic.SUBLW:
                    Rewrite_SUBLW();
                    break;
                case Mnemonic.SUBWF:
                    Rewrite_SUBWF();
                    break;
                case Mnemonic.SWAPF:
                    Rewrite_SWAPF();
                    break;
                case Mnemonic.XORLW:
                    Rewrite_XORLW();
                    break;
                case Mnemonic.XORWF:
                    Rewrite_XORWF();
                    break;


                // Pseudo-instructions
                case Mnemonic.__CONFIG:
                case Mnemonic.DA:
                case Mnemonic.DB:
                case Mnemonic.DE:
                case Mnemonic.DT:
                case Mnemonic.DTM:
                case Mnemonic.DW:
                case Mnemonic.__IDLOCS:
                    m.Invalid();
                    break;
            }

        }

        protected override void SetStatusFlags(Expression dst)
        {
            FlagM flags = PIC16CC.Defined(instrCurr.Mnemonic);
            if (flags != 0)
                m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        protected void GetSrc(out Expression srcMem)
        {
            var src = instrCurr.Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {instrCurr.Operands[0]}");
            GetUnaryPtrs(src, out srcMem);
        }

        protected void GetSrcAndDst(out Expression srcMem, out Expression dstMem)
        {
            GetSrc(out srcMem);
            var dst = instrCurr.Operands[1] as PICOperandMemWRegDest ?? throw new InvalidOperationException($"Invalid destination operand: {instrCurr.Operands[1]}");
            dstMem = dst.WRegIsDest ? Wreg : srcMem;
        }

        private void Rewrite_ADDLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.IAdd(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_ADDWF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.IAdd(Wreg, srcMem));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_ANDLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.And(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_ANDWF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.And(Wreg, srcMem));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_BCF()
        {
            GetSrc(out var srcMem);
            var mask = GetBitMask(instrCurr.Operands[1], true);
            m.Assign(srcMem, m.And(srcMem, mask));
        }

        private void Rewrite_BSF()
        {
            GetSrc(out var srcMem);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            m.Assign(srcMem, m.Or(srcMem, mask));
        }

        private void Rewrite_BTFSC()
        {
            rtlc = InstrClass.ConditionalTransfer;
            GetSrc(out var srcMem);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            var res = m.And(srcMem, mask);
            m.Branch(m.Eq0(res), SkipToAddr(), rtlc);
        }

        private void Rewrite_BTFSS()
        {
            rtlc = InstrClass.ConditionalTransfer;
            GetSrc(out var srcMem);
            var mask = GetBitMask(instrCurr.Operands[1], false);
            var res = m.And(srcMem, mask);
            m.Branch(m.Ne0(res), SkipToAddr(), rtlc);
        }

        private void Rewrite_CALL()
        {
            rtlc = InstrClass.Transfer | InstrClass.Call;
            var target = instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid program address operand: {instrCurr.Operands[0]}");
            Address retaddr = instrCurr.Address + instrCurr.Length;
            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Call(target.CodeTarget, 0);
        }

        private void Rewrite_CLRF()
        {
            GetSrc(out var srcMem);
            m.Assign(srcMem, Constant.Byte(0));
            m.Assign(binder.EnsureFlagGroup(PICRegisters.Z), Constant.Bool(true));
        }

        private void Rewrite_CLRW()
        {
            m.Assign(Wreg, Constant.Byte(0));
            m.Assign(binder.EnsureFlagGroup(PICRegisters.Z), Constant.Bool(true));
        }

        private void Rewrite_CLRWDT()
        {
            byte mask;

            PICRegisterBitFieldStorage pd = PICRegisters.PD;
            PICRegisterBitFieldStorage to = PICRegisters.TO;
            var status = binder.EnsureRegister(PICRegisters.STATUS);
            mask = (byte)((1 << pd.BitPos) | (1 << to.BitPos));
            m.Assign(status, m.Or(status, Constant.Byte(mask)));
        }

        private void Rewrite_COMF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Comp(srcMem));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_DECF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.ISub(srcMem, Constant.Byte(1)));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_DECFSZ()
        {
            rtlc = InstrClass.ConditionalTransfer;
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.ISub(srcMem, Constant.Byte(1)));
            m.Branch(m.Eq0(dstMem), SkipToAddr(), rtlc);
        }

        private void Rewrite_GOTO()
        {
            rtlc = InstrClass.Transfer;
            var target = instrCurr.Operands[0] as PICOperandProgMemoryAddress ?? throw new InvalidOperationException($"Invalid program address operand: {instrCurr.Operands[0]}");
            m.Goto(target.CodeTarget);
        }

        private void Rewrite_INCF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.IAdd(srcMem, Constant.Byte(1)));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_INCFSZ()
        {
            rtlc = InstrClass.ConditionalTransfer;
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.IAdd(srcMem, Constant.Byte(1)));
            m.Branch(m.Eq0(dstMem), SkipToAddr(), rtlc);
        }

        private void Rewrite_IORLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.Or(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_IORWF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Or(Wreg, srcMem));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_MOVF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, srcMem);
            SetStatusFlags(dstMem);
        }

        private void Rewrite_MOVLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, k.ImmediateValue);
            SetStatusFlags(Wreg);
        }

        private void Rewrite_MOVWF()
        {
            GetSrc(out var srcMem);
            m.Assign(srcMem, Wreg);
            SetStatusFlags(srcMem);
        }

        private void Rewrite_RETFIE()
        {
            rtlc = InstrClass.Transfer;
            PICRegisterBitFieldStorage gie = PIC16Registers.GIE;
            byte mask = (byte)(1 << gie.BitPos);
            var intcon = binder.EnsureRegister(PIC16Registers.INTCON);
            m.Assign(intcon, m.Or(intcon, Constant.Byte(mask)));
            PopFromHWStackAccess();
            m.Return(0, 0);
        }

        private void Rewrite_RETLW()
        {
            rtlc = InstrClass.Transfer;
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, k.ImmediateValue);
            PopFromHWStackAccess();
            m.Return(0, 0);
        }

        private void Rewrite_RETURN()
        {
            rtlc = InstrClass.Transfer;
            PopFromHWStackAccess();
            m.Return(0, 0);
        }

        private void Rewrite_RLF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Fn(host.PseudoProcedure("__rlf", PrimitiveType.Byte, srcMem)));
        }

        private void Rewrite_RRF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Fn(host.PseudoProcedure("__rrf", PrimitiveType.Byte, srcMem)));
        }

        private void Rewrite_SLEEP()
        {
            m.Nop();
        }

        private void Rewrite_SUBLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.ISub(k.ImmediateValue, Wreg));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_SUBWF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.ISub(srcMem, Wreg));
            SetStatusFlags(dstMem);
        }

        private void Rewrite_SWAPF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Fn(host.PseudoProcedure("__swapf", PrimitiveType.Byte, srcMem)));
        }

        private void Rewrite_XORLW()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(Wreg, m.Xor(Wreg, k.ImmediateValue));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_XORWF()
        {
            GetSrcAndDst(out var srcMem, out var dstMem);
            m.Assign(dstMem, m.Xor(Wreg, srcMem));
            SetStatusFlags(dstMem);
        }

    }

}
