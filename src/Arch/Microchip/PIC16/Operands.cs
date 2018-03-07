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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Libraries.Microchip;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// The notion of PIC16 immediate operand. Must be inherited.
    /// </summary>
    public abstract class PIC16ImmediateOperand : MachineOperand, IOperand
    {
        /// <summary>
        /// The immediate value.
        /// </summary>
        public readonly Constant ImmediateValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="immValue">The immediate constant value.</param>
        /// <param name="dataWidth">The constant data width.</param>
        public PIC16ImmediateOperand(Constant immValue, PrimitiveType dataWidth) : base(dataWidth)
        {
            ImmediateValue = immValue;
        }

        /// <summary>
        /// Accepts the given visitor method.
        /// </summary>
        /// <param name="visitor">The visitor method.</param>
        public abstract void Accept(IOperandVisitor visitor);

        /// <summary>
        /// Accepts the given visitor function.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="visitor">The visitor function.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IOperandVisitor<T> visitor);

        /// <summary>
        /// Accepts the given visitor function with context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="visitor">The visitor function.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// A PIC16 5-bit unsigned immediate operand. Used by MOVLB instruction.
    /// </summary>
    public class PIC16Immed5Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 5-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed5Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm5(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm5(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm5(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC16 7-bit unsigned immediate operand. Used by MOVLP instruction.
    /// </summary>
    public class PIC16Immed7Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 7-bit unsigned immediate operand. Used by MOVLP instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed7Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm7(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm7(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm7(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC16 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
    /// </summary>
    public class PIC16Immed8Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed8Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm8(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm8(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm8(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{ImmediateValue}");
        }

    }

}
