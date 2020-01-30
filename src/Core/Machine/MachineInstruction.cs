#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Abstract base class for low-level machine instructions.
    /// </summary>
    public abstract class MachineInstruction
    {
        public static readonly MachineOperand[] NoOperands = new MachineOperand[0];

        /// <summary>
        /// The address at which the instruction begins.
        /// </summary>
        public Address Address;

        /// <summary>
        /// The length of the entire instruction. Some architectures, e.g. M68k, x86, and most
        /// 8-bit microprocessors, have variable length instructions.
        /// </summary>
        public int Length;

        /// <summary>
        /// The kind of instruction.
        /// </summary>
        public InstrClass InstructionClass;

        /// <summary>
        /// 0 or more operands of the instruction.
        /// </summary>
        public MachineOperand[] Operands;

        /// <summary>
        /// Returns true if the instruction is valid.
        /// </summary>
        public bool IsValid => InstructionClass != InstrClass.Invalid;

        /// <summary>
        /// Returns true if <paramref name="addr"/> is contained
        /// inside the instruction.
        /// </summary>
        public bool Contains(Address addr)
        {
            ulong ulInstr = Address.ToLinear();
            ulong ulAddr = addr.ToLinear();
            return ulInstr <= ulAddr && ulAddr < ulInstr + (uint)Length;
        }

        public virtual void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
        }

        /// <summary>
        /// Each different supported mnemonic should have a different numerical value, exposed here.
        /// </summary>
        public abstract int MnemonicAsInteger { get; }

        public sealed override string ToString()
        {
            var renderer = new StringRenderer();
            renderer.Address = Address;
            this.Render(renderer, MachineInstructionWriterOptions.None);
            return renderer.ToString();
        }

        public string ToString(IPlatform platform)
        {
            var renderer = new StringRenderer(platform);
            renderer.Address = Address;
            this.Render(renderer, MachineInstructionWriterOptions.None);
            return renderer.ToString();
        }

        /// <summary>
        /// Utility function to render the operands, separated by commas.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="options"></param>
        protected void RenderOperands(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Operands.Length == 0)
                return;
            writer.Tab();
            RenderOperand(Operands[0], writer, options);
            for (int i = 1; i < Operands.Length; ++i)
            {
                writer.WriteChar(',');
                RenderOperand(Operands[i], writer, options);
            }
        }

        protected virtual void RenderOperand(MachineOperand operand, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            operand.Write(writer, options);
        }
    }
}
