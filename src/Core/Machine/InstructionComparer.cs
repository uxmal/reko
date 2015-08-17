#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
    public abstract class InstructionComparer<T> : IEqualityComparer<T>
        where T : MachineInstruction
    {
        private  Normalize norm;

        public InstructionComparer(Normalize norm)
        {
            this.norm = norm;
        }

        public bool NormalizeConstants { get { return (norm & Normalize.Constants) != 0; } }
        public bool NormalizeRegisters { get { return (norm & Normalize.Registers) != 0; } }

        public virtual bool Equals(T x, T y)
        {
            if (x.OpcodeAsInteger != y.OpcodeAsInteger)
                return false;
            return CompareOperands(x, y);
        }

        public int GetConstantHash(Constant c)
        {
            if ((norm & Normalize.Constants) != 0)
                return 0;
            return c.GetValue().GetHashCode();
        }

        public abstract bool CompareOperands(T x, T y);

        public int GetHashCode(T instr)
        {
            int h = instr.OpcodeAsInteger.GetHashCode();
            return h ^ GetOperandsHash(instr);
        }

        public abstract int GetOperandsHash(T instr);
    }
}
