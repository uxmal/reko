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

        internal MachineOperand op1;
        internal MachineOperand op2;
        internal MachineOperand op3;

        /// <summary>
        /// Instantiates a new <see cref="PIC18Instruction"/> with given <see cref="Opcode"/>, execution mode and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 2 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC18 opcode.</param>
        /// <param name="isExecExtend">True if this PIC18 instruction is for extended execution mode.</param>
        /// <param name="ops">Zero, one or two instuction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 2 operands provided.</exception>
        public PIC18Instruction (Opcode opc, PICExecMode execMode, params MachineOperand[] ops)
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
                    {
                        op3 = ops[2];
                        if (ops.Length >= 4)
                            throw new ArgumentException("Too many operands.", nameof(ops));
                    }
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
        /// Gets a value indicating whether this PIC18 instruction is used in extended-mode instruction set only.
        /// </summary>
        /// <value>
        /// True if this PIC18 is in extended-mode, false if not.
        /// </value>
        public bool IsExecExtended => ExecMode == PICExecMode.Extended;

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
            get { return Opcode == Opcode.invalid; }
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
                case 2: return op3;
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
                if (op3 == null)
                    return 2;
                return 3;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 == null)
                return;
            writer.Tab();
            op1.Write(writer, options);
            if (op2 == null)
                return;
            writer.Write(",");
            op2.Write(writer, options);
            if (op3 == null)
                return;
            writer.Write(",");
            op3.Write(writer, options);
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
                case Opcode.daw:
                    return FlagM.C;

                case Opcode.clrf:
                    return FlagM.Z;

                case Opcode.andlw:
                case Opcode.andwf:
                case Opcode.comf:
                case Opcode.iorlw:
                case Opcode.iorwf:
                case Opcode.movf:
                case Opcode.rlncf:
                case Opcode.rrncf:
                case Opcode.xorlw:
                case Opcode.xorwf:
                    return FlagM.Z | FlagM.N;

                case Opcode.rlcf:
                case Opcode.rrcf:
                    return FlagM.C | FlagM.Z | FlagM.N;

                case Opcode.addlw:
                case Opcode.addwf:
                case Opcode.addwfc:
                case Opcode.decf:
                case Opcode.incf:
                case Opcode.negf:
                case Opcode.reset:
                case Opcode.subfwb:
                case Opcode.sublw:
                case Opcode.subwf:
                case Opcode.subwfb:
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
                case Opcode.addwfc:
                case Opcode.bc:
                case Opcode.bnc:
                case Opcode.rlcf:
                case Opcode.rrcf:
                case Opcode.subfwb:
                case Opcode.subwfb:
                    return FlagM.C;

                case Opcode.daw:
                    return FlagM.C | FlagM.DC;

                case Opcode.bn:
                case Opcode.bnn:
                    return FlagM.N;

                case Opcode.bnz:
                case Opcode.bz:
                    return FlagM.Z;

                case Opcode.bnov:
                case Opcode.bov:
                    return FlagM.OV;

                case Opcode.@return:
                case Opcode.retfie:
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
                { Opcode.call,    LinkTransfer },
                { Opcode.rcall,   LinkTransfer },
                { Opcode.reset,   Transfer },
                { Opcode.callw,   LinkTransfer },
                { Opcode.@return, Transfer },
                { Opcode.retlw,   Transfer },
                { Opcode.retfie,  Transfer },
                { Opcode.bra,     Transfer },
                { Opcode.@goto,   Transfer },
                { Opcode.decfsz,  CondLinear },
                { Opcode.incfsz,  CondLinear },
                { Opcode.infsnz,  CondLinear },
                { Opcode.dcfsnz,  CondLinear },
                { Opcode.cpfslt,  CondLinear },
                { Opcode.cpfseq,  CondLinear },
                { Opcode.cpfsgt,  CondLinear },
                { Opcode.tstfsz,  CondLinear },
                { Opcode.btfss,   CondTransfer },
                { Opcode.btfsc,   CondTransfer },
                { Opcode.bz,      CondTransfer },
                { Opcode.bnz,     CondTransfer },
                { Opcode.bc,      CondTransfer },
                { Opcode.bnc,     CondTransfer },
                { Opcode.bov,     CondTransfer },
                { Opcode.bnov,    CondTransfer },
                { Opcode.bn,      CondTransfer },
                { Opcode.bnn,     CondTransfer },
                { Opcode.addulnk, Transfer },
                { Opcode.subulnk, Transfer },
            };

        }

    }

}
