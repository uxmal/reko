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
        void Accept(IPIC16OperandVisitor visitor);
        T Accept<T>(IOperandVisitor<T> visitor);
        T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// Interface defining the permitted visitor methods on PIC16 Operands.
    /// </summary>
    public interface IPIC16OperandVisitor
    {
        void VisitDataBanked(PIC16BankedOperand bank);
        void VisitDataBit(PIC16DataBitOperand bit);
        void VisitDataByte(PIC16DataByteWithDestOperand byt);
        void VisitFSRArith(PIC16FSRArithOperand arith);
        void VisitFSRIndexed(PIC16FSRIndexedOperand index);
        void VisitIncDecFSR(PIC16FSRIncDecOperand mode);
        void VisitTrisNum(PIC16TrisNumOperand tris);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    public interface IOperandVisitor<T>
    {
        T VisitDataBanked(PIC16BankedOperand bank);
        T VisitDataBit(PIC16DataBitOperand bit);
        T VisitDataByte(PIC16DataByteWithDestOperand byt);
        T VisitFSRArith(PIC16FSRArithOperand arith);
        T VisitFSRIndexed(PIC16FSRIndexedOperand index);
        T VisitIncDecFSR(PIC16FSRIncDecOperand mode);
        T VisitTrisNum(PIC16TrisNumOperand tris);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions with context on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the context.</typeparam>
    public interface IOperandVisitor<T, C>
    {
        T VisitDataBanked(PIC16BankedOperand bank, C context);
        T VisitDataBit(PIC16DataBitOperand bit, C context);
        T VisitDataByte(PIC16DataByteWithDestOperand byt, C context);
        T VisitFSRArith(PIC16FSRArithOperand arith, C context);
        T VisitFSRIndexed(PIC16FSRIndexedOperand index, C context);
        T VisitIncDecFSR(PIC16FSRIncDecOperand mode, C context);
        T VisitTrisNum(PIC16TrisNumOperand tris, C context);
    }

}
