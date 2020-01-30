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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Machine
{
    public abstract class InstructionComparer : IEqualityComparer<MachineInstruction>
    {
        private  Normalize norm;

        public InstructionComparer(Normalize norm)
        {
            this.norm = norm;
        }

        public abstract bool CompareOperands(MachineInstruction x, MachineInstruction y);

        public bool CompareRegisters(RegisterStorage regA, RegisterStorage regB)
        {
            if (regA == null)
            {
                return regB == null;
            }
            if (regB == null)
            {
                return regA == null;
            }
            return NormalizeRegisters || regA == regB;
        }

        public bool CompareValues(Constant constA, Constant constB)
        {
            if (constA == null)
            {
                return constB == null;
            }
            if (constB == null)
            {
                return constA == null;
            }
            return NormalizeConstants || constA.GetValue().Equals(constB.GetValue());
        }

        public virtual bool Equals(MachineInstruction x, MachineInstruction y)
        {
            if (x.MnemonicAsInteger != y.MnemonicAsInteger)
                return false;
            return CompareOperands(x, y);
        }

        public int GetConstantHash(Constant c)
        {
            if ((norm & Normalize.Constants) != 0)
                return 0;
            return c.GetValue().GetHashCode();
        }

        public int GetRegisterHash(RegisterStorage r)
        {
            if ((norm & Normalize.Registers) != 0)
                return 0;
            return r.Number.GetHashCode();
        }

        public int GetHashCode(MachineInstruction instr)
        {
            int h = instr.MnemonicAsInteger.GetHashCode();
            return h ^ GetOperandsHash(instr);
        }

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
