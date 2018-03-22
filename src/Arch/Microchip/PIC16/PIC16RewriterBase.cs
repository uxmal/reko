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
using System;

namespace Reko.Arch.Microchip.PIC16
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

            switch (instrCurr.Opcode)
            {
                default:
                    throw new AddressCorrelatedException(addr, $"Rewriting of PIC16 instruction '{instrCurr.Opcode}' is not implemented yet.");

                case Opcode.invalid:
                case Opcode.unaligned:
                    m.Invalid();
                    break;

                case Opcode.ADDLW:
                    Rewrite_ADDLW();
                    break;
                case Opcode.ADDWF:
                    Rewrite_ADDWF();
                    break;
                case Opcode.ANDLW:
                    Rewrite_ANDLW();
                    break;
                case Opcode.ANDWF:
                    Rewrite_ANDWF();
                    break;
                case Opcode.BCF:
                    Rewrite_BCF();
                    break;
                case Opcode.BSF:
                    Rewrite_BSF();
                    break;
                case Opcode.BTFSC:
                    Rewrite_BTFSC();
                    break;
                case Opcode.BTFSS:
                    Rewrite_BTFSS();
                    break;
                case Opcode.CALL:
                    Rewrite_CALL();
                    break;
                case Opcode.CLRF:
                    Rewrite_CLRF();
                    break;
                case Opcode.CLRW:
                    Rewrite_CLRW();
                    break;
                case Opcode.CLRWDT:
                    Rewrite_CLRWDT();
                    break;
                case Opcode.COMF:
                    Rewrite_COMF();
                    break;
                case Opcode.DECF:
                    Rewrite_DECF();
                    break;
                case Opcode.DECFSZ:
                    Rewrite_DECFSZ();
                    break;
                case Opcode.GOTO:
                    Rewrite_GOTO();
                    break;
                case Opcode.INCF:
                    Rewrite_INCF();
                    break;
                case Opcode.INCFSZ:
                    Rewrite_INCFSZ();
                    break;
                case Opcode.IORLW:
                    Rewrite_IORLW();
                    break;
                case Opcode.IORWF:
                    Rewrite_IORWF();
                    break;
                case Opcode.MOVF:
                    Rewrite_MOVF();
                    break;
                case Opcode.MOVLW:
                    Rewrite_MOVLW();
                    break;
                case Opcode.MOVWF:
                    Rewrite_MOVWF();
                    break;
                case Opcode.NOP:
                    m.Nop();
                    break;
                case Opcode.RETFIE:
                    Rewrite_RETFIE();
                    break;
                case Opcode.RETLW:
                    Rewrite_RETLW();
                    break;
                case Opcode.RETURN:
                    Rewrite_RETURN();
                    break;
                case Opcode.RLF:
                    Rewrite_RLF();
                    break;
                case Opcode.RRF:
                    Rewrite_RRF();
                    break;
                case Opcode.SLEEP:
                    Rewrite_SLEEP();
                    break;
                case Opcode.SUBLW:
                    Rewrite_SUBLW();
                    break;
                case Opcode.SUBWF:
                    Rewrite_SUBWF();
                    break;
                case Opcode.SWAPF:
                    Rewrite_SWAPF();
                    break;
                case Opcode.XORLW:
                    Rewrite_XORLW();
                    break;
                case Opcode.XORWF:
                    Rewrite_XORWF();
                    break;


                // Pseudo-instructions
                case Opcode.__CONFIG:
                case Opcode.DA:
                case Opcode.DB:
                case Opcode.DE:
                case Opcode.DT:
                case Opcode.DTM:
                case Opcode.DW:
                case Opcode.__IDLOCS:
                    m.Invalid();
                    break;
            }

        }

        /// <summary>
        /// Gets the working register (WREG) which is often used implicitly by PIC instructions.
        /// </summary>
        /// <value>
        /// The WREG register.
        /// </value>
        protected override Identifier GetWReg => binder.EnsureRegister(PIC16Registers.WREG);

        protected Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(PIC16Registers.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        protected void SetStatusFlags(Expression dst)
        {
            FlagM flags = PIC16CC.Defined(instrCurr.Opcode);
            if (flags != 0)
                m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        protected ArrayAccess PushToHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            var slot = m.ARef(PrimitiveType.Ptr32, PIC16Registers.GlobalStack, stkptr);
            m.Assign(stkptr, m.IAdd(stkptr, Constant.Byte(1)));
            return slot;
        }

        protected ArrayAccess PopFromHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, m.ISub(stkptr, Constant.Byte(1)));
            var slot = m.ARef(PrimitiveType.Ptr32, PIC16Registers.GlobalStack, stkptr);
            return slot;
        }

        protected static MemoryAccess DataMem8(Expression ea)
            => new MemoryAccess(PIC16Registers.GlobalData, ea, PrimitiveType.Byte);

        protected Expression GetImmediateValue(MachineOperand op)
        {
            switch (op)
            {
                case PIC16ImmediateOperand imm:
                    return imm.ImmediateValue;

                default:
                    throw new InvalidOperationException($"Invalid immediate operand.");
            }
        }

        protected Expression GetProgramAddress(MachineOperand op)
        {
            switch (op)
            {
                case PIC16ProgAddrOperand paddr:
                    return PICProgAddress.Ptr(paddr.CodeTarget);

                default:
                    throw new InvalidOperationException($"Invalid program address operand.");
            }
        }

        protected Constant GetBitMask(MachineOperand op, bool revert)
        {
            switch (op)
            {
                case PIC16DataBitOperand bitaddr:
                    int mask = (1 << bitaddr.BitNumber.ToByte());
                    if (revert)
                        mask = ~mask;
                    return Constant.Byte((byte)mask);

                default:
                    throw new InvalidOperationException("Invalid bit number operand.");
            }
        }

        private void Rewrite_ADDLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.IAdd(Wreg, k));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_ADDWF()
        {

        }

        private void Rewrite_ANDLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.And(Wreg, k));
            SetStatusFlags(Wreg);
        }

        private void Rewrite_ANDWF()
        {

        }

        void Rewrite_BCF()
        {
        }

        void Rewrite_BSF()
        {
        }

        void Rewrite_BTFSC()
        {
        }

        void Rewrite_BTFSS()
        {
        }

        void Rewrite_CALL()
        {
        }

        void Rewrite_CLRF()
        {
        }

        void Rewrite_CLRW()
        {
        }

        void Rewrite_CLRWDT()
        {
        }

        void Rewrite_COMF()
        {
        }

        void Rewrite_DECF()
        {
        }

        void Rewrite_DECFSZ()
        {
        }

        void Rewrite_GOTO()
        {
        }

        void Rewrite_INCF()
        {
        }

        void Rewrite_INCFSZ()
        {
        }

        void Rewrite_IORLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.Or(Wreg, k));
            SetStatusFlags(Wreg);
        }

        void Rewrite_IORWF()
        {
        }

        void Rewrite_MOVF()
        {
        }

        void Rewrite_MOVLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, k);
            SetStatusFlags(Wreg);
        }

        void Rewrite_MOVWF()
        {
        }

        void Rewrite_RETFIE()
        {
        }

        void Rewrite_RETLW()
        {
        }

        void Rewrite_RETURN()
        {
        }

        void Rewrite_RLF()
        {
        }

        void Rewrite_RRF()
        {
        }

        void Rewrite_SLEEP()
        {
        }

        void Rewrite_SUBLW()
        {
        }

        void Rewrite_SUBWF()
        {
        }

        void Rewrite_SWAPF()
        {
        }

        void Rewrite_XORLW()
        {
            var k = GetImmediateValue(instrCurr.op1);
            m.Assign(Wreg, m.Xor(Wreg, k));
            SetStatusFlags(Wreg);
        }

        void Rewrite_XORWF()
        {
        }

    }

}
