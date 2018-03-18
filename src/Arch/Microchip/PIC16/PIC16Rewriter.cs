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
using Reko.Core.Types;
using System;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public abstract class PIC16Rewriter : PICRewriter
    {

        protected PIC16Rewriter(PICArchitecture arch, PICDisassemblerBase disasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, disasm, state, binder, host)
        {
        }

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
                case Opcode.ADDWF:
                case Opcode.ANDLW:
                case Opcode.ANDWF:
                case Opcode.BCF:
                case Opcode.BRA:
                case Opcode.BSF:
                case Opcode.BTFSC:
                case Opcode.BTFSS:
                case Opcode.CALL:
                case Opcode.CLRF:
                case Opcode.CLRW:
                case Opcode.CLRWDT:
                case Opcode.COMF:
                case Opcode.DECF:
                case Opcode.DECFSZ:
                case Opcode.GOTO:
                case Opcode.INCF:
                case Opcode.INCFSZ:
                case Opcode.IORLW:
                case Opcode.IORWF:
                case Opcode.MOVF:
                case Opcode.MOVLW:
                case Opcode.MOVWF:
                case Opcode.NOP:
                case Opcode.RETFIE:
                case Opcode.RETLW:
                case Opcode.RETURN:
                case Opcode.RLF:
                case Opcode.RRF:
                case Opcode.SLEEP:
                case Opcode.SUBLW:
                case Opcode.SUBWF:
                case Opcode.SWAPF:
                case Opcode.XORLW:
                case Opcode.XORWF:

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

        protected override Identifier GetWReg => binder.EnsureRegister(PIC16Registers.WREG);

        #region Helpers

        protected Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(PIC16Registers.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
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

        #endregion

    }

}
