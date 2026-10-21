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
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Linc;

public class LincInstructionComparer : InstructionComparer
{
    public LincInstructionComparer(Normalize norm) : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        switch (op1)
        {
        case Constant c1:
            var c2 = (Constant) op2;
            return base.CompareConstants(c1, c2);
        case MemoryOperand m1:
            var m2 = (MemoryOperand) op2;
            return base.NormalizeConstants ? true : m1.Offset == m2.Offset;
        case TapeBlockOperator t1:
            var t2 = (TapeBlockOperator) op2;
            return base.NormalizeConstants ? true :t1.Mblk == t2.Mblk && t1.Tblk == t2.Tblk;
        }
        throw new NotImplementedException();
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            Constant c => base.GetConstantHash(c),
            MemoryOperand m => m.Offset.GetHashCode(),
            TapeBlockOperator t => HashCode.Combine(t.Mblk, t.Tblk),
            _ => throw new NotImplementedException()
        };
        throw new NotImplementedException();
    }
}