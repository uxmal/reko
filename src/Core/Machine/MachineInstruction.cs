#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

        private Address? addr;      //$REFACTOR: in C# 9, there will be 'init' properties.

        public MachineInstruction()
        {
            this.Operands = NoOperands;
        }

        //$TODO: make MachineInstruction have a ctor with (Address, Length, Operands)
        //$REFACTOR: in C# 9, there will be 'init' properties, then we won't need a nullable backing field.
        /// <summary>
        /// The address at which the instruction begins.
        /// </summary>
        public Address Address { get { return addr!; } set { addr = value; } }

        /// <summary>
        /// The length of the entire instruction measured in storage units.
        /// Some architectures, e.g. M68k, x86, and most 8-bit microprocessors, have
        /// variable length instructions.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The kind of instruction.
        /// </summary>
        public InstrClass InstructionClass { get; set; }

        /// <summary>
        /// 0 or more operands of the instruction.
        /// </summary>
        public MachineOperand[] Operands { get; set; }

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

        /// <summary>
        /// Renders this instruction to the provided <see cref="MachineInstructionRenderer"/>.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="options"></param>
        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginInstruction(Address);
            DoRender(renderer, options);
            renderer.EndInstruction();
        }

        protected virtual void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
        }

        /// <summary>
        /// Each different supported mnemonic should have a different numerical value, exposed here.
        /// </summary>
        public abstract int MnemonicAsInteger { get; }

        /// <summary>
        /// Generate a string for the mnemonic; this is only used for generation of unit tests, so use only
        /// characters [a-z0-9_]
        /// </summary>
        public abstract string MnemonicAsString { get; }
        
        public sealed override string ToString()
        {
            var renderer = new StringRenderer();
            this.DoRender(renderer, MachineInstructionRendererOptions.Default);
            return renderer.ToString();
        }

        public string ToString(MachineInstructionRendererOptions options)
        {
            var renderer = new StringRenderer();
            this.DoRender(renderer, options);
            return renderer.ToString();
        }

        /// <summary>
        /// Utility function to render the operands, separated by commas.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="options"></param>
        protected void RenderOperands(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Operands!.Length == 0)
                return;
            renderer.Tab();
            RenderOperand(Operands[0], renderer, options);
            for (int i = 1; i < Operands.Length; ++i)
            {
                renderer.WriteString(options.OperandSeparator ?? ",");
                RenderOperand(Operands[i], renderer, options);
            }
        }

        protected virtual void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            operand.Render(renderer, options);
        }
    }
}
