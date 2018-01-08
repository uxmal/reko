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

using Reko.Core.Machine;
using System.Collections.Generic;
using System;
using Microchip.Crownking;

namespace Reko.Arch.Microchip.PIC18
{
    

    /// <summary>
    /// Models an PIC18 instruction.
    /// </summary>
    public class PIC18Instruction : MachineInstruction
    {
        private const InstructionClass CondLinear = InstructionClass.Conditional | InstructionClass.Linear;
        private const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer;
        private const InstructionClass LinkTransfer = InstructionClass.Call | InstructionClass.Transfer;
        private const InstructionClass Transfer = InstructionClass.Transfer;

        private static Dictionary<Opcode, InstructionClass> classOf;

        internal PIC18OperandImpl op1;
        internal PIC18OperandImpl op2;

        /// <summary>
        /// Instantiates a new <see cref="PIC18Instruction"/> with given <see cref="Opcode"/>, execution mode and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 2 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC18 opcode.</param>
        /// <param name="isExecExtend">True if this PIC18 instruction is for extended execution mode.</param>
        /// <param name="ops">Zero, one or two instuction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 2 operands provided.</exception>
        public PIC18Instruction(Opcode opc, PICExecMode execMode, params PIC18OperandImpl[] ops)
        {
            this.Opcode = opc;
            this.ExecMode = execMode;
            if (ops.Length >= 1)
            {
                op1 = ops[0];
                if (ops.Length >= 2)
                {
                    op2 = ops[1];
                    if (ops.Length >= 3)
                        throw new ArgumentException("Too many operands.", nameof(ops));
                }
            }
        }

        public Opcode Opcode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this PIC18 instruction is used in extended-mode instruction set only.
        /// </summary>
        /// <value>
        /// True if this PIC18 is in extended-mode, false if not.
        /// </value>
        public PICExecMode ExecMode { get; private set; }

        /// <summary>
        /// The kind of instruction.
        /// </summary>
        /// <value>
        /// The instruction class.
        /// </value>
        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass il;
                if (!classOf.TryGetValue(Opcode, out il))
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
        public override bool IsValid
        {
            get { return Opcode != Opcode.invalid; }
        }

        /// <summary>
        /// Each different supported opcode should have a different numerical value, exposed here.
        /// </summary>
        /// <value>
        /// The opcode as integer.
        /// </value>
        public override int OpcodeAsInteger
        {
            get { return (int)Opcode; }
        }

        /// <summary>
        /// Retrieves the i'th operand, or null if there is none at that position.
        /// </summary>
        /// <param name="i">Operand's index..</param>
        /// <returns>
        /// The designated operand or null.
        /// </returns>
        public override MachineOperand GetOperand(int i)
        {
            switch (i)
            {
                case 0: return op1;
                case 1: return op2;
                default: return null;
            }
        }

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
                if (op1 == null)
                    return 0;
                if (op2 == null)
                    return 1;
                return 2;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 == null)
                return;
            if ((op1 as PIC18OperandImpl).IsVisible)
            {
                writer.Tab();
                op1.Write(writer, options);
                if (op2 == null)
                    return;
                if ((op2 as PIC18OperandImpl).IsVisible)
                {
                    writer.Write(",");
                    op2.Write(writer, options);
                }
            }
        }

        /// <summary>
        /// Returns the status flags modified by the instruction.
        /// </summary>
        /// <param name="opcode">The opcode of the instruction.</param>
        /// <returns>
        /// A set of status flags
        /// </returns>
        public static FlagM DefCc(Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.DAW:
                    return FlagM.C;

                case Opcode.CLRF:
                    return FlagM.Z;

                case Opcode.ANDLW:
                case Opcode.ANDWF:
                case Opcode.COMF:
                case Opcode.IORLW:
                case Opcode.IORWF:
                case Opcode.MOVF:
                case Opcode.RLNCF:
                case Opcode.RRNCF:
                case Opcode.XORLW:
                case Opcode.XORWF:
                    return FlagM.Z | FlagM.N;

                case Opcode.RLCF:
                case Opcode.RRCF:
                    return FlagM.C | FlagM.Z | FlagM.N;

                case Opcode.ADDLW:
                case Opcode.ADDWF:
                case Opcode.ADDWFC:
                case Opcode.DECF:
                case Opcode.INCF:
                case Opcode.NEGF:
                case Opcode.RESET:
                case Opcode.SUBFWB:
                case Opcode.SUBLW:
                case Opcode.SUBWF:
                case Opcode.SUBWFB:
                    return FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns the condition codes an instruction uses.
        /// </summary>
        /// <param name="opcode">The opcode of the instruction.</param>
        /// <returns>
        /// A set of status flags
        /// </returns>
        public static FlagM UseCc(Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.ADDWFC:
                case Opcode.BC:
                case Opcode.BNC:
                case Opcode.RLCF:
                case Opcode.RRCF:
                case Opcode.SUBFWB:
                case Opcode.SUBWFB:
                    return FlagM.C;

                case Opcode.DAW:
                    return FlagM.C | FlagM.DC;

                case Opcode.BN:
                case Opcode.BNN:
                    return FlagM.N;

                case Opcode.BNZ:
                case Opcode.BZ:
                    return FlagM.Z;

                case Opcode.BNOV:
                case Opcode.BOV:
                    return FlagM.OV;

                case Opcode.RETURN:
                case Opcode.RETFIE:
                    return FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static PIC18Instruction()
        {
            classOf = new Dictionary<Opcode, InstructionClass>()
            {
                { Opcode.invalid, InstructionClass.Invalid },
                { Opcode.CALL,    LinkTransfer },
                { Opcode.RCALL,   LinkTransfer },
                { Opcode.RESET,   Transfer },
                { Opcode.CALLW,   LinkTransfer },
                { Opcode.RETURN,  Transfer },
                { Opcode.RETLW,   Transfer },
                { Opcode.RETFIE,  Transfer },
                { Opcode.BRA,     Transfer },
                { Opcode.GOTO,    Transfer },
                { Opcode.DECFSZ,  CondLinear },
                { Opcode.INCFSZ,  CondLinear },
                { Opcode.INFSNZ,  CondLinear },
                { Opcode.DCFSNZ,  CondLinear },
                { Opcode.CPFSLT,  CondLinear },
                { Opcode.CPFSEQ,  CondLinear },
                { Opcode.CPFSGT,  CondLinear },
                { Opcode.TSTFSZ,  CondLinear },
                { Opcode.BTFSS,   CondLinear },
                { Opcode.BTFSC,   CondLinear },
                { Opcode.BZ,      CondTransfer },
                { Opcode.BNZ,     CondTransfer },
                { Opcode.BC,      CondTransfer },
                { Opcode.BNC,     CondTransfer },
                { Opcode.BOV,     CondTransfer },
                { Opcode.BNOV,    CondTransfer },
                { Opcode.BN,      CondTransfer },
                { Opcode.BNN,     CondTransfer },
                { Opcode.ADDULNK, Transfer },
                { Opcode.SUBULNK, Transfer },
            };

        }

    }

}
