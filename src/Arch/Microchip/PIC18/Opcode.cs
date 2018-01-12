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
namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// Values that represent opcodes of the PIC18 Legacy/Extended/Enhanced family.
    // Note: Opcodes are defined in uppercase to conform to Microchip MASM syntax.
    /// </summary>
    public enum Opcode
    {
        invalid,
        ///<summary>Add literal and WREG</summary>
        ADDLW,
        ///<summary>Add WREG and f</summary>
        ADDWF,
        ///<summary>Add WREG and Carry to f</summary>
        ADDWFC,
        ///<summary>AND literal with WREG</summary>
        ANDLW,
        ///<summary>AND WREG with f</summary>
        ANDWF,
        ///<summary>Branch if carry</summary>
        BC,
        ///<summary>Bit clear f</summary>
        BCF,
        ///<summary>Branch if negative</summary>
        BN,
        ///<summary>Branch if not carry</summary>
        BNC,
        ///<summary>Branch if not negative</summary>
        BNN,
        ///<summary>Branch if not overflow</summary>
        BNOV,
        ///<summary>Branch if not zero</summary>
        BNZ,
        ///<summary>Branch if overflow</summary>
        BOV,
        ///<summary>Branch unconditionally</summary>
        BRA,
        ///<summary>Bit set f</summary>
        BSF,
        ///<summary>Bit test f, skip if clear</summary>
        BTFSC,
        ///<summary>Bit test f, skip if set</summary>
        BTFSS,
        ///<summary>Bit toggle f</summary>
        BTG,
        ///<summary>Branch if zero</summary>
        BZ,
        ///<summary>CAll subroutine</summary>
        CALL,
        ///<summary>Clear f</summary>
        CLRF,
        ///<summary>Clear Watchdog Timer</summary>
        CLRWDT,
        ///<summary>Complement f</summary>
        COMF,
        ///<summary>Compare f with WREG, skip =</summary>
        CPFSEQ,
        ///<summary>Compare f with WREG, skip ></summary>
        CPFSGT,
        ///<summary>Compare f with WREG,  skip <</summary>
        CPFSLT,
        ///<summary>Decimal adjust W register</summary>
        DAW,
        ///<summary>Decrement f, skip if not 0</summary>
        DCFSNZ,
        ///<summary>Decrement f</summary>
        DECF,
        ///<summary>Decrement f,  skip if 0</summary>
        DECFSZ,
        ///<summary>Go to address</summary>
        GOTO,
        ///<summary>Increment f</summary>
        INCF,
        ///<summary>Increment f,  skip if 0</summary>
        INCFSZ,
        ///<summary>Increment f, skip if not 0</summary>
        INFSNZ,
        ///<summary>Inclusive OR literal with WREG</summary>
        IORLW,
        ///<summary>Inclusive OR WREG with f</summary>
        IORWF,
        ///<summary>Move literal to FSRn</summary>
        LFSR,
        ///<summary>Move f</summary>
        MOVF,
        ///<summary>Move source to destination</summary>
        MOVFF,
        ///<summary>Move source to destination (long range - enhanced PIC18 only)</summary>
        MOVFFL,
        ///<summary>Move literal to low nibble in BSR</summary>
        MOVLB,
        ///<summary>Move literal to WREG</summary>
        MOVLW,
        ///<summary>Move WREG to f</summary>
        MOVWF,
        ///<summary>Multiply literal with WREG</summary>
        MULLW,
        ///<summary>Multiply WREG with f</summary>
        MULWF,
        ///<summary>Negate f</summary>
        NEGF,
        ///<summary>No operation</summary>
        NOP,
        ///<summary>Pop top of return stack</summary>
        POP,
        ///<summary>Push top of return stack</summary>
        PUSH,
        ///<summary>Relative call</summary>
        RCALL,
        ///<summary>Reset</summary>
        RESET,
        ///<summary>Return from interrupt</summary>
        RETFIE,
        ///<summary>Return with literal in WREG</summary>
        RETLW,
        ///<summary>Return from subroutine</summary>
        RETURN,
        ///<summary>Rotate left f through Carry</summary>
        RLCF,
        ///<summary>Rotate left f (No Carry)</summary>
        RLNCF,
        ///<summary>Rotate right f through Carry</summary>
        RRCF,
        ///<summary>Rotate right f (No Carry)</summary>
        RRNCF,
        ///<summary>Set f</summary>
        SETF,
        ///<summary>Enter sleep mode</summary>
        SLEEP,
        ///<summary>Substract f from WREG with Borrow</summary>
        SUBFWB,
        ///<summary>Substract WREG from literal</summary>
        SUBLW,
        ///<summary>Substract WREG from f</summary>
        SUBWF,
        ///<summary>Substract WREG from f with Borrow</summary>
        SUBWFB,
        ///<summary>Swap nibbles in f</summary>
        SWAPF,
        ///<summary>Table read</summary>
        TBLRD,
        ///<summary>Table write</summary>
        TBLWT,
        ///<summary>Test f,  skip if 0</summary>
        TSTFSZ,
        ///<summary>Exclusive OR literal with WREG</summary>
        XORLW,
        ///<summary>Exclusive OR WREG with f</summary>
        XORWF,

        // PIC18 Extended Execution mode

        ///<summary>Add literal to FSR</summary>
        ADDFSR,
        ///<summary>Add literal to FSR2 and return</summary>
        ADDULNK,
        ///<summary>Call subroutine using WREG</summary>
        CALLW,
        ///<summary>Move indexed to f</summary>
        MOVSF,
        ///<summary>Move indexed to indexed</summary>
        MOVSS,
        ///<summary>Push literal at FSR2, decrement FSR2</summary>
        PUSHL,
        ///<summary>Substract literal from FSR</summary>
        SUBFSR,
        ///<summary>Substract literal from FSR2 and return</summary>
        SUBULNK,
        ///<summary>Move indexed to f (Long range - enhanced PIC18 only)</summary>
        MOVSFL
    }

}
