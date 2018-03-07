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
    /// Interface for PIC16-Enhanced Operands' visitors.
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
        void VisitImm7(PIC16Immed7Operand imm7);
        void VisitImm8(PIC16Immed8Operand imm8);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    public interface IOperandVisitor<T>
    {
        T VisitImm5(PIC16Immed5Operand imm5);
        T VisitImm7(PIC16Immed7Operand imm7);
        T VisitImm8(PIC16Immed8Operand imm8);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions with context on PIC16 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the context.</typeparam>
    public interface IOperandVisitor<T, C>
    {
        T VisitImm5(PIC16Immed5Operand imm5, C context);
        T VisitImm7(PIC16Immed7Operand imm7, C context);
        T VisitImm8(PIC16Immed8Operand imm8, C context);
    }

}
