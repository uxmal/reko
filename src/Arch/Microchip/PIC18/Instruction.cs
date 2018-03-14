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
using Reko.Libraries.Microchip;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC18
{

    using Common;

    /// <summary>
    /// Models a PIC18 instruction.
    /// </summary>
    public class PIC18Instruction : PICInstruction<Opcode>
    {

        #region Member fields

        private static Dictionary<Opcode, InstructionClass> classOf = new Dictionary<Opcode, InstructionClass>()
        {
                { Opcode.invalid,   InstructionClass.Invalid },
                { Opcode.unaligned, InstructionClass.Invalid },
                { Opcode.CALL,      LinkTransfer },
                { Opcode.RCALL,     LinkTransfer },
                { Opcode.RESET,     Transfer },
                { Opcode.CALLW,     LinkTransfer },
                { Opcode.RETURN,    Transfer },
                { Opcode.RETLW,     Transfer },
                { Opcode.RETFIE,    Transfer },
                { Opcode.BRA,       Transfer },
                { Opcode.GOTO,      Transfer },
                { Opcode.DECFSZ,    CondLinear },
                { Opcode.INCFSZ,    CondLinear },
                { Opcode.INFSNZ,    CondLinear },
                { Opcode.DCFSNZ,    CondLinear },
                { Opcode.CPFSLT,    CondLinear },
                { Opcode.CPFSEQ,    CondLinear },
                { Opcode.CPFSGT,    CondLinear },
                { Opcode.TSTFSZ,    CondLinear },
                { Opcode.BTFSS,     CondLinear },
                { Opcode.BTFSC,     CondLinear },
                { Opcode.BZ,        CondTransfer },
                { Opcode.BNZ,       CondTransfer },
                { Opcode.BC,        CondTransfer },
                { Opcode.BNC,       CondTransfer },
                { Opcode.BOV,       CondTransfer },
                { Opcode.BNOV,      CondTransfer },
                { Opcode.BN,        CondTransfer },
                { Opcode.BNN,       CondTransfer },
                { Opcode.ADDULNK,   Transfer },
                { Opcode.SUBULNK,   Transfer },
                { Opcode.CONFIG,    InstructionClass.None },
                { Opcode.DA,        InstructionClass.None },
                { Opcode.DB,        InstructionClass.None },
                { Opcode.DE,        InstructionClass.None },
                { Opcode.DW,        InstructionClass.None },
                { Opcode.__IDLOCS,  InstructionClass.None },
        };

        #endregion

        /// <summary>
        /// Instantiates a new <see cref="PIC18Instruction"/> with given <see cref="Opcode"/>, execution mode and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 2 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC18 opcode.</param>
        /// <param name="isExecExtend">True if this PIC18 instruction is for extended execution mode.</param>
        /// <param name="ops">Zero, one or two instruction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 2 operands provided.</exception>
        public PIC18Instruction(Opcode opc, PICExecMode execMode, params MachineOperand[] ops)
            : base(opc, ops)
        {
            ExecMode = execMode;
        }


        /// <summary>
        /// Gets a value indicating whether this PIC18 instruction is used in extended-mode instruction set only.
        /// </summary>
        /// <value>
        /// True if this PIC18 is in extended-mode, false if not.
        /// </value>
        public PICExecMode ExecMode { get; }

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


        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 is null)
                return;
            if (op1 is IOperandShadow opshad1)
            {
                if (opshad1.IsPresent)
                {
                    writer.WriteString(",");
                    writer.Tab();
                    op1.Write(writer, options);
                }
                return;
            }
            writer.Tab();
            op1.Write(writer, options);
            if (op2 is null)
                return;
            if (op2 is IOperandShadow opshad2)
            {
                if (opshad2.IsPresent)
                {
                    writer.WriteString(",");
                    op2.Write(writer, options);
                }
                return;
            }
            writer.WriteString(",");
            op2.Write(writer, options);
        }

    }

}
