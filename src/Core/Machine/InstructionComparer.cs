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

using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Compares instructions for equality.
    /// </summary>
    public abstract class InstructionComparer : IEqualityComparer<MachineInstruction>
    {
        private readonly Normalize norm;

        /// <summary>
        /// Constructs an instruction comparer.
        /// </summary>
        /// <param name="norm">Normalization to use.</param>
        public InstructionComparer(Normalize norm)
        {
            this.norm = norm;
        }

        /// <summary>
        /// Compares the operands of two instructions.
        /// </summary>
        /// <param name="x">First machine instruction.</param>
        /// <param name="y">Second machine instruction.</param>
        /// <returns>True if all operands compare equal; otherwise false.</returns>
        public abstract bool CompareOperands(MachineInstruction x, MachineInstruction y);

        /// <summary>
        /// Compares the two <see cref="RegisterStorage"/>s.
        /// </summary>
        /// <param name="regA">First register.</param>
        /// <param name="regB">Second register.</param>
        /// <returns>True if registers are equal; otherwise false.</returns>
        public bool CompareRegisters(RegisterStorage? regA, RegisterStorage? regB)
        {
            if (regA is null)
            {
                return regB is null;
            }
            if (regB is null)
            {
                return regA is null;
            }
            return NormalizeRegisters || regA == regB;
        }

        /// <summary>
        /// Compares the two <see cref="Constant"/>s.
        /// </summary>
        /// <param name="constA">First constant.</param>
        /// <param name="constB">Second constant.</param>
        /// <returns>True if registers are equal; otherwise false.</returns>
        public bool CompareValues(Constant? constA, Constant? constB)
        {
            if (constA is null)
            {
                return constB is null;
            }
            if (constB is null)
            {
                return constA is null;
            }
            return NormalizeConstants || constA.GetValue().Equals(constB.GetValue());
        }

        /// <inheritdoc/>
        public virtual bool Equals(MachineInstruction? x, MachineInstruction? y)
        {
            if (x is null)
                return y is null;
            if (y is null)
                return false;
            if (x.MnemonicAsInteger != y.MnemonicAsInteger)
                return false;
            return CompareOperands(x, y);
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="Constant"/>.
        /// </summary>
        /// <param name="c">Constant whose hash code is to be computed.</param>
        /// <returns>Hash code of the constant, normalized if necessary.</returns>
        public int GetConstantHash(Constant? c)
        {
            if ((norm & Normalize.Constants) != 0 || c is null)
                return 0;
            return c.GetValue().GetHashCode();
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="RegisterStorage"/>.
        /// </summary>
        /// <param name="r">Register whose hash code is to be computed.</param>
        /// <returns>Hash code of the register, normalized if necessary.</returns>
        public int GetRegisterHash(RegisterStorage? r)
        {
            if ((norm & Normalize.Registers) != 0 || r is null)
                return 0;
            return r.Number.GetHashCode();
        }

        public int GetHashCode(MachineInstruction instr)
        {
            int h = instr.MnemonicAsInteger.GetHashCode();
            return h ^ GetOperandsHash(instr);
        }

        /// <summary>
        /// Computes a hash code for the operands of the given instruction.
        /// </summary>
        /// <param name="instr">Machine instruction whose operands will have their hash compited.</param>
        /// <returns>The hash code of the operands, respecting the <see cref="norm"/>.
        /// </returns>
        public abstract int GetOperandsHash(MachineInstruction instr);

        /// <summary>
        /// If true, differences between constants will be ignored.
        /// </summary>
        public bool NormalizeConstants { get { return (norm & Normalize.Constants) != 0; } }

        /// <summary>
        /// If true, register differences will be ignored.
        /// </summary>
        public bool NormalizeRegisters { get { return (norm & Normalize.Registers) != 0; } }
    }
}
