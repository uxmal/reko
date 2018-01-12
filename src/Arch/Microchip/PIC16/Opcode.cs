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

namespace Reko.Arch.Microchip.PIC16
{
    /// <summary>
    /// Values that represent opcodes of the PIC16 Mid-Range/Enhanced family.
    /// </summary>
    public enum Opcode
    {
        illegal,
        ///<summary>Add literal with W</summary>
        ADDLW,
        ///<summary>Add W with f</summary>
        ADDWF,
        ///<summary>AND literal with W</summary>
        ANDLW,
        ///<summary>AND W with f</summary>
        ANDWF,
        ///<summary>Bit clear f</summary>
        BCF,
        ///<summary>Relative branch with W</summary>
        BRW,
        ///<summary>Bit set f</summary>
        BSF,
        ///<summary>Bit test f, skip if clear</summary>
        BTFSC,
        ///<summary>Bit test f,  skip if set</summary>
        BTFSS,
        ///<summary>Call subroutine</summary>
        CALL,
        ///<summary>Clear f</summary>
        CLRF,
        ///<summary>Clear W</summary>
        CLRW,
        ///<summary>Clear watchdog timer</summary>
        CLRWDT,
        ///<summary>Complement f</summary>
        COMF,
        ///<summary>Decrement f</summary>
        DECF,
        ///<summary>Decrement f, skip if 0</summary>
        DECFSZ,
        ///<summary>Unconditional branch</summary>
        GOTO,
        ///<summary>Increment f</summary>
        INCF,
        ///<summary>Increment f, skip if 0</summary>
        INCFSZ,
        ///<summary>Inclusive OR literal with W</summary>
        IORLW,
        ///<summary>Inclusive OR W with f</summary>
        IORWF,
        ///<summary>Move f</summary>
        MOVF,
        ///<summary>Move literal to W</summary>
        MOVLW,
        ///<summary>Move W to f</summary>
        MOVWF,
        ///<summary>No operation</summary>
        NOP,
        ///<summary>Load OPTION register with W</summary>
        OPTION,
        ///<summary>Return from interrupt</summary>
        RETFIE,
        ///<summary>Return with literal in W</summary>
        RETLW,
        ///<summary>Return from subroutine</summary>
        RETURN,
        ///<summary>Rotate left f through Carry</summary>
        RLF,
        ///<summary>Rotate right f through Carry</summary>
        RRF,
        ///<summary>Enter sleep mode</summary>
        SLEEP,
        ///<summary>Substract W from literal</summary>
        SUBLW,
        ///<summary>Substract W from f</summary>
        SUBWF,
        ///<summary>Swap nibbles in f</summary>
        SWAPF,
        ///<summary>Load TRIS register with W</summary>
        TRIS,
        ///<summary>Exclusive OR literal with W</summary>
        XORLW,
        ///<summary>Exclusive OR W with f</summary>
        XORWF,
        ///<summary>Add literal to FSRn</summary>
        ADDFSR,
        ///<summary>Add W and CARRY bit to f</summary>
        ADDWFC,
        ///<summary>Arithmetic right shift</summary>
        ASRF,
        ///<summary>Relative branch</summary>
        BRA,
        ///<summary>Subroutine call with W</summary>
        CALLW,
        ///<summary>Logical left shift</summary>
        LSLF,
        ///<summary>Logical right shift</summary>
        LSRF,
        ///<summary>Move INDFn to W</summary>
        MOVIW,
        ///<summary>Move k[FSRn] to W</summary>
        moviw_idx,
        ///<summary>Move literal to BSR</summary>
        MOVLB,
        ///<summary>Move literal to PCLATH</summary>
        MOVLP,
        ///<summary>Move W to INDFn</summary>
        MOVWI,
        ///<summary>Move W to k[FSRn]</summary>
        MOVWI_idx,
        ///<summary>Reset processor</summary>
        RESET,
        ///<summary>Substract W from f with Borrow</summary>
        SUBWFB
    }

}
