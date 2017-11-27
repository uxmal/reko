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

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// Values that represent opcodes of the PIC18 Legacy/Extended family.
    /// </summary>
    public enum Opcode
    {
        invalid,
        ///<summary>Add literal and WREG</summary>
        addlw,
        ///<summary>Add WREG and f</summary>
        addwf,
        ///<summary>Add WREG and Carry to f</summary>
        addwfc,
        ///<summary>AND literal with WREG</summary>
        andlw,
        ///<summary>AND WREG with f</summary>
        andwf,
        ///<summary>Branch if carry</summary>
        bc,
        ///<summary>Bit clear f</summary>
        bcf,
        ///<summary>Branch if negative</summary>
        bn,
        ///<summary>Branch if not carry</summary>
        bnc,
        ///<summary>Branch if not negative</summary>
        bnn,
        ///<summary>Branch if not overflow</summary>
        bnov,
        ///<summary>Branch if not zero</summary>
        bnz,
        ///<summary>Branch if overflow</summary>
        bov,
        ///<summary>Branch unconditionally</summary>
        bra,
        ///<summary>Bit set f</summary>
        bsf,
        ///<summary>Bit test f, skip if clear</summary>
        btfsc,
        ///<summary>Bit test f, skip if set</summary>
        btfss,
        ///<summary>Bit toggle f</summary>
        btg,
        ///<summary>Branch if zero</summary>
        bz,
        ///<summary>CAll subroutine</summary>
        call,
        ///<summary>Clear f</summary>
        clrf,
        ///<summary>Clear Watchdog Timer</summary>
        clrwdt,
        ///<summary>Complement f</summary>
        comf,
        ///<summary>Compare f with WREG, skip =</summary>
        cpfseq,
        ///<summary>Compare f with WREG, skip ></summary>
        cpfsgt,
        ///<summary>Compare f with WREG,  skip <</summary>
        cpfslt,
        ///<summary>Decimal adjust W register</summary>
        daw,
        ///<summary>Decrement f, skip if not 0</summary>
        dcfsnz,
        ///<summary>Decrement f</summary>
        decf,
        ///<summary>Decrement f,  skip if 0</summary>
        decfsz,
        ///<summary>Go to address</summary>
        @goto,
        ///<summary>Increment f</summary>
        incf,
        ///<summary>Increment f,  skip if 0</summary>
        incfsz,
        ///<summary>Increment f, skip if not 0</summary>
        infsnz,
        ///<summary>Inclusive OR literal with WREG</summary>
        iorlw,
        ///<summary>Inclusive OR WREG with f</summary>
        iorwf,
        ///<summary>Move literal to FSRn</summary>
        lfsr,
        ///<summary>Move f</summary>
        movf,
        ///<summary>Move source to destination</summary>
        movff,
        ///<summary>Move source to destination (long range)</summary>
        movffl,
        ///<summary>Move literal to low nibble in BSR</summary>
        movlb,
        ///<summary>Move literal to WREG</summary>
        movlw,
        ///<summary>Move WREG to f</summary>
        movwf,
        ///<summary>Multiply literal with WREG</summary>
        mullw,
        ///<summary>Multiply WREG with f</summary>
        mulwf,
        ///<summary>Negate f</summary>
        negf,
        ///<summary>No operation</summary>
        nop,
        ///<summary>Pop top of return stack</summary>
        pop,
        ///<summary>Push top of return stack</summary>
        push,
        ///<summary>Relative call</summary>
        rcall,
        ///<summary>Reset</summary>
        reset,
        ///<summary>Return from interrupt</summary>
        retfie,
        ///<summary>Return with literal in WREG</summary>
        retlw,
        ///<summary>Return from subroutine</summary>
        @return,
        ///<summary>Rotate left f through Carry</summary>
        rlcf,
        ///<summary>Rotate left f (No Carry)</summary>
        rlncf,
        ///<summary>Rotate right f through Carry</summary>
        rrcf,
        ///<summary>Rotate right f (No Carry)</summary>
        rrncf,
        ///<summary>Set f</summary>
        setf,
        ///<summary>Enter sleep mode</summary>
        sleep,
        ///<summary>Substract f from WREG with Borrow</summary>
        subfwb,
        ///<summary>Substract WREG from literal</summary>
        sublw,
        ///<summary>Substract WREG from f</summary>
        subwf,
        ///<summary>Substract WREG from f with Borrow</summary>
        subwfb,
        ///<summary>Swap nibbles in f</summary>
        swapf,
        ///<summary>Table read</summary>
        tblrd,
        ///<summary>Table write</summary>
        tblwt,
        ///<summary>Test f,  skip if 0</summary>
        tstfsz,
        ///<summary>Exclusive OR literal with WREG</summary>
        xorlw,
        ///<summary>Exclusive OR WREG with f</summary>
        xorwf,

        // PIC18 Extended Execution mode

        ///<summary>Add literal to FSR</summary>
        addfsr,
        ///<summary>Add literal to FSR2 and return</summary>
        addulnk,
        ///<summary>Call subroutine using WREG</summary>
        callw,
        ///<summary>Move indexed to f</summary>
        movsf,
        ///<summary>Move indexed to indexed</summary>
        movss,
        ///<summary>Push literal at FSR2, decrement FSR2</summary>
        pushl,
        ///<summary>Substract literal from FSR</summary>
        subfsr,
        ///<summary>Substract literal from FSR2 and return</summary>
        subulnk,
        ///<summary>Move indexed to f (Long range)</summary>
        movsfl,
    }

}
