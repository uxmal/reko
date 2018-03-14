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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    public abstract class PICInstruction<T> : MachineInstruction where T : struct 
    {

        public const InstructionClass CondLinear = InstructionClass.Conditional | InstructionClass.Linear;
        public const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer;
        public const InstructionClass LinkTransfer = InstructionClass.Call | InstructionClass.Transfer;
        public const InstructionClass Transfer = InstructionClass.Transfer;

        internal MachineOperand op1;
        internal MachineOperand op2;


        /// <summary>
        /// Instantiates a new <see cref="PICInstruction"/> with given <see cref="Opcode"/> and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 2 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC opcode.</param>
        /// <param name="ops">Zero, one or two instruction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 2 operands provided.</exception>
        public PICInstruction(T opc, params MachineOperand[] ops)
        {
            Opcode = opc;
            if (ops.Length >= 1)
            {
                op1 = ops[0];
                if (ops.Length >= 2)
                {
                    op2 = ops[1];
                    if (ops.Length >= 3)
                        throw new ArgumentException("Too many PIC instruction's operands.", nameof(ops));
                }
            }
        }

        /// <summary>
        /// Gets the opcode.
        /// </summary>
        public T Opcode { get; }

        /// <summary>
        /// Gets the number of operands of this instruction.
        /// </summary>
        /// <value>
        /// The number of operands as an integer.
        /// </value>
        public byte NumberOfOperands
        {
            get
            {
                if (op1 is null)
                    return 0;
                if (op2 is null)
                    return 1;
                return 2;
            }
        }

        /// <summary>
        /// Retrieves the nth operand, or null if there is none at that position.
        /// </summary>
        /// <param name="n">Operand's index..</param>
        /// <returns>
        /// The designated operand or null.
        /// </returns>
        public override MachineOperand GetOperand(int n)
        {
            switch (n)
            {
                case 0:
                    return op1;
                case 1:
                    return op2;
                default:
                    return null;
            }
        }

    }

}
