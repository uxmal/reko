#region License
/* 
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
        addlw,
        ///<summary>Add W with f</summary>
        addwf,
        ///<summary>AND literal with W</summary>
        andlw,
        ///<summary>AND W with f</summary>
        andwf,
        ///<summary>Bit clear f</summary>
        bcf,
        ///<summary>Relative branch with W</summary>
        brw,
        ///<summary>Bit set f</summary>
        bsf,
        ///<summary>Bit test f, skip if clear</summary>
        btfsc,
        ///<summary>Bit test f,  skip if set</summary>
        btfss,
        ///<summary>Call subroutine</summary>
        call,
        ///<summary>Clear f</summary>
        clrf,
        ///<summary>Clear W</summary>
        clrw,
        ///<summary>Clear watchdog timer</summary>
        clrwdt,
        ///<summary>Complement f</summary>
        comf,
        ///<summary>Decrement f</summary>
        decf,
        ///<summary>Decrement f, skip if 0</summary>
        decfsz,
        ///<summary>Unconditional branch</summary>
        @goto,
        ///<summary>Increment f</summary>
        incf,
        ///<summary>Increment f, skip if 0</summary>
        incfsz,
        ///<summary>Inclusive OR literal with W</summary>
        iorlw,
        ///<summary>Inclusive OR W with f</summary>
        iorwf,
        ///<summary>Move f</summary>
        movf,
        ///<summary>Move literal to W</summary>
        movlw,
        ///<summary>Move W to f</summary>
        movwf,
        ///<summary>No operation</summary>
        nop,
        ///<summary>Load OPTION register with W</summary>
        option,
        ///<summary>Return from interrupt</summary>
        retfie,
        ///<summary>Return with literal in W</summary>
        retlw,
        ///<summary>Return from subroutine</summary>
        àreturn,
        ///<summary>Rotate left f through Carry</summary>
        rlf,
        ///<summary>Rotate right f through Carry</summary>
        rrf,
        ///<summary>Enter sleep mode</summary>
        sleep,
        ///<summary>Substract W from literal</summary>
        sublw,
        ///<summary>Substract W from f</summary>
        subwf,
        ///<summary>Swap nibbles in f</summary>
        swapf,
        ///<summary>Load TRIS register with W</summary>
        tris,
        ///<summary>Exclusive OR literal with W</summary>
        xorlw,
        ///<summary>Exclusive OR W with f</summary>
        xorwf,
        ///<summary>Add literal to FSRn</summary>

        addfsr,
        ///<summary>Add W and CARRY bit to f</summary>
        addwfc,
        ///<summary>Arithmetic right shift</summary>
        asrf,
        ///<summary>Relative branch</summary>
        bra,
        ///<summary>Subroutine call with W</summary>
        callw,
        ///<summary>Logical left shift</summary>
        lslf,
        ///<summary>Logical right shift</summary>
        lsrf,
        ///<summary>Move INDFn to W</summary>
        moviw,
        ///<summary>Move k[FSRn] to W</summary>
        moviw_idx,
        ///<summary>Move literal to BSR</summary>
        movlb,
        ///<summary>Move literal to PCLATH</summary>
        movlp,
        ///<summary>Move W to INDFn</summary>
        movwi,
        ///<summary>Move W to k[FSRn]</summary>
        movwi_idx,
        ///<summary>Reset processor</summary>
        reset,
        ///<summary>Substract W from f with Borrow</summary>
        subwfb
    }

}
