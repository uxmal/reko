#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Models a contiguous range of machine registers.
    /// </summary>
    public class RegisterRange : AbstractMachineOperand
    {
        /// <summary>
        /// Underlying array of registers.
        /// </summary>
        public RegisterStorage[] Registers { get; }

        /// <summary>
        /// Starting index in the register range.
        /// </summary>
        public int RegisterIndex { get; }

        /// <summary>
        /// Number of elements contained in the range.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Constructs a register range.
        /// </summary>
        /// <param name="gpRegisters">Underlying range of registers.</param>
        /// <param name="iRegStart">Starting index of the register range.</param>
        /// <param name="cRegs">Number of regsiters in the range.</param>
        public RegisterRange(RegisterStorage[] gpRegisters, int iRegStart, int cRegs)
            : base(gpRegisters[0].DataType)    // Don't care.
        {
            this.Registers = gpRegisters;
            this.RegisterIndex = iRegStart;
            this.Count = cRegs;
        }

        /// <inheritdoc/>
        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(Registers[RegisterIndex].Name);
            if (Count <= 1)
                return;
            if (Count <= 2)
            {
                renderer.WriteString(",");
            }
            else
            {
                renderer.WriteString("-");
            }
            renderer.WriteString(Registers[RegisterIndex + Count - 1].Name);
        }

        /// <summary>
        /// Enumerates the registers in the range.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RegisterStorage> Enumerate()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return Registers[RegisterIndex + i];
            }
        }
    }
}