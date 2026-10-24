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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Pdp.Pdp8;

public class Pdp8InstructionComparer : InstructionComparer
{
    public Pdp8InstructionComparer(Normalize norm)
        : base(norm)
    {
    }

    public override bool Equals(MachineInstruction? x, MachineInstruction? y)
    {
        if (!base.Equals(x, y))
            return false;
        if (x is Pdp8Instruction px && y is Pdp8Instruction py)
        {
            return px.Indirect == py.Indirect;
        }
        return true;
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        throw new NotImplementedException($"DoCompareOperands for {op1.GetType().Name} is not implemented yet.");
    }

    public override int GetOperandHash(MachineOperand op)
    {
        throw new NotImplementedException();
    }
}
