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

namespace Reko.Arch.CompactRisc
{
    internal class Cr16InstructionComparer : InstructionComparer
    {
        private const int MemCode = 0x10;

        public Cr16InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
        {
            return op1 switch
            {
                RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
                Constant c1 => CompareConstants(c1, (Constant) op2),
                Address a1 => CompareAddresses(a1, (Address) op2),
                MemoryOperand m1 => CompareMemoryOperands(m1, (MemoryOperand) op2),
                SequenceStorage s1 => CompareSequences(s1, (SequenceStorage) op2),
                _ => throw new NotImplementedException(op1.GetType().Name)
            };
        }

        private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
        {
            if (!CompareStorages(m1.Base, m2.Base))
                return false;
            if (!CompareStorages(m1.Index, m2.Index))
                return false;
            return CompareConstants(m1.Displacement, m2.Displacement);
        }

        public override int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
                RegisterStorage reg => GetRegisterHash(reg),
                Constant c => GetConstantHash(c),
                Address a => GetAddressHash(a),
                MemoryOperand m => GetMemoryHash(m),
                SequenceStorage s => GetSequenceHash(s),
                _ => throw new NotImplementedException(op.GetType().Name)
            };
        }

        private int GetMemoryHash(MemoryOperand m)
        {
            int hash = MemCode;
            hash = hash * 17 ^ GetStorageHash(m.Base);
            hash = hash * 17 ^ GetStorageHash(m.Index);
            hash = hash * 17 ^ GetConstantHash(m.Displacement);
            return hash;
        }
    }
}