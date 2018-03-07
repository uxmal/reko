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
    /// Values that represent opcodes of the PIC16 family.
    /// Note: Opcodes are defined in uppercase to conform to Microchip MPASM user's guide syntax.
    /// </summary>
    public enum Opcode
    {
        invalid,
        unaligned,
        ///<summary>Add literal with FSR.</summary>
        ADDFSR,
        ///<summary>Add literal with W.</summary>
        ADDLW,
        ///<summary>Add W with f.</summary>
        ADDWF,
        ///<summary>Add W and Carry bit with f.</summary>
        ADDWFC,
        ///<summary>AND literal with W.</summary>
        ANDLW,
        ///<summary>AND W with f.</summary>
        ANDWF,
        ///<summary>Arithmetic Right shift.</summary>
        ASRF,
        ///<summary>Bit clear f.</summary>
        BCF,
        ///<summary>Relative branch.</summary>
        BRA,
        ///<summary>Relative branch with W.</summary>
        BRW,
        ///<summary>Bit set f.</summary>
        BSF,
        ///<summary>Bit test f, skip if clear.</summary>
        BTFSC,
        ///<summary>Bit test f,  skip if set.</summary>
        BTFSS,
        ///<summary>Call subroutine.</summary>
        CALL,
        ///<summary>Subroutine Call With W.</summary>
        CALLW,
        ///<summary>Clear f.</summary>
        CLRF,
        ///<summary>Clear W.</summary>
        CLRW,
        ///<summary>Clear watchdog timer.</summary>
        CLRWDT,
        ///<summary>Complement f.</summary>
        COMF,
        ///<summary>Decrement f.</summary>
        DECF,
        ///<summary>Decrement f, skip if 0</summary>
        DECFSZ,
        ///<summary>Unconditional branch</summary>
        GOTO,
        ///<summary>Increment f.</summary>
        INCF,
        ///<summary>Increment f, skip if 0.</summary>
        INCFSZ,
        ///<summary>Inclusive OR literal with W.</summary>
        IORLW,
        ///<summary>Inclusive OR W with f.</summary>
        IORWF,
        ///<summary>Logical Left Shift.</summary>
        LSLF,
        ///<summary>Logical Right Shift.</summary>
        LSRF,
        ///<summary>Move f.</summary>
        MOVF,
        ///<summary>Move INDFn to W - or - Move k[FSRn] to W.</summary>
        MOVIW,
        ///<summary>Move literal to BSR.</summary>
        MOVLB,
        ///<summary>Move literal to PCLATH.</summary>
        MOVLP,
        ///<summary>Move literal to W.</summary>
        MOVLW,
        ///<summary>Move W to f.</summary>
        MOVWF,
        ///<summary>Move W to INDFn -or - Move W to k[FSRn].</summary>
        MOVWI,
        ///<summary>No operation.</summary>
        NOP,
        ///<summary>Load OPTION register with W.</summary>
        OPTION,
        ///<summary>Software Reset.</summary>
        RESET,
        ///<summary>Return from interrupt.</summary>
        RETFIE,
        ///<summary>Return with literal in W.</summary>
        RETLW,
        ///<summary>Return from subroutine.</summary>
        RETURN,
        ///<summary>Rotate left f through Carry</summary>
        RLF,
        ///<summary>Rotate right f through Carry.</summary>
        RRF,
        ///<summary>Enter sleep mode.</summary>
        SLEEP,
        ///<summary>Substract W from literal.</summary>
        SUBLW,
        ///<summary>Substract W from f.</summary>
        SUBWF,
        ///<summary>Subtract W from f with Borrow.</summary>
        SUBWFB,
        ///<summary>Swap nibbles in f.</summary>
        SWAPF,
        ///<summary>Load TRIS register with W.</summary>
        TRIS,
        ///<summary>Exclusive OR literal with W.</summary>
        XORLW,
        ///<summary>Exclusive OR W with f.</summary>
        XORWF,

        // Pseudo-instructions

        /// <summary>Configuration bits pseudo-instruction.</summary>
        __CONFIG,
        /// <summary>Store string in program memory pseudo-instruction.</summary>
        DA,
        /// <summary>Store data byte in program memory pseudo-instruction.</summary>
        DB,
        /// <summary>Store data byte in EEPROM memory pseudo-instruction.</summary>
        DE,
        /// <summary>Define table in program memory pseudo-instruction.</summary>
        DT,
        /// <summary>Define table in program memory pseudo-instruction (Extended PIC16 only).</summary>
        DTM,
        /// <summary>Store data word in program memory pseudo-instruction.</summary>
        DW,
        /// <summary>Store word in ID locations memory pseudo-instruction.</summary>
        __IDLOCS
    }

}
