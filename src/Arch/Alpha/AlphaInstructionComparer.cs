#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Alpha
{
    internal class AlphaInstructionComparer : InstructionComparer
    {
        private readonly Normalize norm;

        /// <summary>
        /// Creates an instruction comparer for the Alpha architecture.
        /// </summary>
        /// <param name="norm">Normalization to use.</param>
        public AlphaInstructionComparer(Normalize norm)
            : base(norm)
        {
            this.norm = norm;
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            if (x.Operands.Length != y.Operands.Length)
                return false;
            for (int i = 0; i < x.Operands.Length; ++i)
            {
                if (!DoCompareOperands(x.Operands[i], y.Operands[i]))
                    return false;
            }
            return true;
        }

        public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
        {
            switch (op1)
            {
            case RegisterStorage reg1:
                var reg2 = (RegisterStorage) op2;
                return CompareRegisters(reg1, reg2);
            case Constant c1:
                var c2 = (Constant) op2;
                return NormalizeConstants || c1.Equals(c2);
            case Address addr1:
                var addr2 = (Address) op2;
                return NormalizeConstants || addr1.Equals(addr2);
            case MemoryOperand mop1:
                var mop2 = (MemoryOperand) op2;
                if (!CompareRegisters(mop1.Base, mop2.Base))
                    return false;
                return NormalizeConstants || mop1.Offset == mop2.Offset;
            default:
                throw new NotImplementedException(op1.GetType().Name);
            }
        }

        public override int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
                RegisterStorage reg => base.GetRegisterHash(reg),
                Constant c => base.GetConstantHash(c),
                Address addr => base.GetAddressHash(addr),
                MemoryOperand mop => GetMemoryHash(mop),
                _ => throw new NotImplementedException(op.GetType().Name)
            };
        }

        private int GetMemoryHash(MemoryOperand mop)
        {
            int h = GetRegisterHash(mop.Base);
            h = h * 23 ^ (NormalizeConstants ? 0 : mop.Offset);
            return h;
        }
    }
}