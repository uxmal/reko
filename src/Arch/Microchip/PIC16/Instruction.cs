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

namespace Reko.Arch.Microchip.PIC16
{
    /// <summary>
    /// Models a PIC16 instruction.
    /// </summary>
    public class PIC16Instruction : MachineInstruction
    {
        #region Constant fields

        private const InstructionClass CondLinear = InstructionClass.Conditional | InstructionClass.Linear;
        private const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer;
        private const InstructionClass LinkTransfer = InstructionClass.Call | InstructionClass.Transfer;
        private const InstructionClass Transfer = InstructionClass.Transfer;

        #endregion

        #region Member fields

        private static Dictionary<Opcode, InstructionClass> classOf = new Dictionary<Opcode, InstructionClass>()
        {
                { Opcode.invalid,   InstructionClass.Invalid },
                { Opcode.unaligned, InstructionClass.Invalid },
                { Opcode.CALL,      LinkTransfer },
                { Opcode.CALLW,     LinkTransfer },
                { Opcode.BRA,       Transfer },
                { Opcode.GOTO,      Transfer },
                { Opcode.RESET,     Transfer },
                { Opcode.RETURN,    Transfer },
                { Opcode.RETLW,     Transfer },
                { Opcode.RETFIE,    Transfer },
                { Opcode.BTFSS,     CondLinear },
                { Opcode.BTFSC,     CondLinear },
                { Opcode.DECFSZ,    CondLinear },
                { Opcode.INCFSZ,    CondLinear },
                { Opcode.__CONFIG,  InstructionClass.None },
                { Opcode.DA,        InstructionClass.None },
                { Opcode.DB,        InstructionClass.None },
                { Opcode.DE,        InstructionClass.None },
                { Opcode.DT,        InstructionClass.None },
                { Opcode.DTM,       InstructionClass.None },
                { Opcode.DW,        InstructionClass.None },
                { Opcode.__IDLOCS,  InstructionClass.None },
        };

        internal MachineOperand op1;
        internal MachineOperand op2;

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new <see cref="PIC16Instruction"/> with given <see cref="Opcode"/>, execution mode and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 2 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC16 opcode.</param>
        /// <param name="ops">Zero, one or two instruction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 2 operands provided.</exception>
        public PIC16Instruction(Opcode opc, params MachineOperand[] ops)
        {
            Opcode = opc;
            if (ops.Length >= 1)
            {
                op1 = ops[0];
                if (ops.Length >= 2)
                {
                    op2 = ops[1];
                    if (ops.Length >= 3)
                        throw new ArgumentException("Too many PIC16 instruction's operands.", nameof(ops));
                }
            }
        }

        #endregion

        #region Properties

        public Opcode Opcode { get; }

        /// <summary>
        /// The control-flow kind of the instruction.
        /// </summary>
        public override InstructionClass InstructionClass
        {
            get
            {
                if (!classOf.TryGetValue(Opcode, out InstructionClass il))
                    il = InstructionClass.Linear;
                return il;
            }
        }

        /// <summary>
        /// Returns true if the instruction is valid.
        /// </summary>
        /// <value>
        /// True if this instruction is valid, false if not.
        /// </value>
        public override bool IsValid => (Opcode != Opcode.invalid && Opcode != Opcode.unaligned);

        /// <summary>
        /// Each different supported opcode should have a different numerical value, exposed here.
        /// </summary>
        /// <value>
        /// The opcode as integer.
        /// </value>
        public override int OpcodeAsInteger => (int)Opcode;

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

        #endregion

        #region Methods

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

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 is null)
                return;
            writer.Tab();
            op1.Write(writer, options);
            if (op2 is null)
                return;
            writer.WriteString(",");
            op2.Write(writer, options);
        }

        #endregion

    }

}
