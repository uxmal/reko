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

namespace Reko.Arch.Microchip.PIC16
{

    /// <summary>
    /// Interface for PIC16 Operands' visitors.
    /// </summary>
    public interface IOperand
    {
        void Accept(IOperandVisitor visitor);
        T Accept<T>(IOperandVisitor<T> visitor);
        T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// Interface defining the permitted visitor methods on PIC16 Operands.
    /// </summary>
    public interface IOperandVisitor
    {
        void VisitImm5(PIC16Immed5Operand imm5);
        void VisitImm6(PIC16Immed6Operand imm6);
        void VisitImm7(PIC16Immed7Operand imm7);
        void VisitImm8(PIC16Immed8Operand imm8);
        void VisitSigned6(PIC16Signed6Operand sgn6);
        void VisitDataBanked(PIC16BankedOperand bank);
        void VisitDataBit(PIC16DataBitOperand bit);
        void VisitDataByte(PIC16DataByteWithDestOperand byt);
        void VisitProgRel9(PIC16ProgRel9AddrOperand rel9);
        void VisitProgAbs(PIC16ProgAbsAddrOperand tgt);
        void VisitFSRArith(PIC16FSRArithOperand arith);
        void VisitFSRIndexed(PIC16FSRIndexedOperand index);
        void VisitIncDecFSR(PIC16FSRIncDecOperand mode);
        void VisitEEPROM(PIC16DataEEPROMOperand eeprom);
        void VisitASCII(PIC16DataASCIIOperand ascii);
        void VisitDB(PIC16DataByteOperand bytes);
        void VisitDW(PIC16DataWordOperand words);
        void VisitIDLocs(PIC16IDLocsOperand idlocs);
        void VisitConfig(PIC16ConfigOperand config);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    public interface IOperandVisitor<T>
    {
        T VisitImm5(PIC16Immed5Operand imm5);
        T VisitImm6(PIC16Immed6Operand imm6);
        T VisitImm7(PIC16Immed7Operand imm7);
        T VisitImm8(PIC16Immed8Operand imm8);
        T VisitSigned6(PIC16Signed6Operand sgn6);
        T VisitDataBanked(PIC16BankedOperand bank);
        T VisitDataBit(PIC16DataBitOperand bit);
        T VisitDataByte(PIC16DataByteWithDestOperand byt);
        T VisitProgRel9(PIC16ProgRel9AddrOperand rel9);
        T VisitProgAbs(PIC16ProgAbsAddrOperand tgt);
        T VisitFSRArith(PIC16FSRArithOperand arith);
        T VisitFSRIndexed(PIC16FSRIndexedOperand index);
        T VisitIncDecFSR(PIC16FSRIncDecOperand mode);
        T VisitEEPROM(PIC16DataEEPROMOperand eeprom);
        T VisitASCII(PIC16DataASCIIOperand ascii);
        T VisitDB(PIC16DataByteOperand bytes);
        T VisitDW(PIC16DataWordOperand words);
        T VisitIDLocs(PIC16IDLocsOperand idlocs);
        T VisitConfig(PIC16ConfigOperand config);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions with context on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the context.</typeparam>
    public interface IOperandVisitor<T, C>
    {
        T VisitImm5(PIC16Immed5Operand imm5, C context);
        T VisitImm6(PIC16Immed6Operand imm6, C context);
        T VisitImm7(PIC16Immed7Operand imm7, C context);
        T VisitImm8(PIC16Immed8Operand imm8, C context);
        T VisitSigned6(PIC16Signed6Operand sgn6, C context);
        T VisitDataBanked(PIC16BankedOperand bank, C context);
        T VisitDataBit(PIC16DataBitOperand bit, C context);
        T VisitDataByte(PIC16DataByteWithDestOperand byt, C context);
        T VisitProgRel9(PIC16ProgRel9AddrOperand rel9, C context);
        T VisitProgAbs(PIC16ProgAbsAddrOperand tgt, C context);
        T VisitFSRArith(PIC16FSRArithOperand arith, C context);
        T VisitFSRIndexed(PIC16FSRIndexedOperand index, C context);
        T VisitIncDecFSR(PIC16FSRIncDecOperand mode, C context);
        T VisitEEPROM(PIC16DataEEPROMOperand eeprom, C context);
        T VisitASCII(PIC16DataASCIIOperand ascii, C context);
        T VisitDB(PIC16DataByteOperand bytes, C context);
        T VisitDW(PIC16DataWordOperand words, C context);
        T VisitIDLocs(PIC16IDLocsOperand idlocs, C context);
        T VisitConfig(PIC16ConfigOperand config, C context);
    }

}
