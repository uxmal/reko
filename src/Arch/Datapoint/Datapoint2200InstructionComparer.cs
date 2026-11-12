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

namespace Reko.Arch.Datapoint;

public class Datapoint2200InstructionComparer : InstructionComparer
{
    public Datapoint2200InstructionComparer(Normalize norm)
        : base(norm)
    {
    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        switch (op1)
        {
        case Address addr1:
            var addr2 = (Address) op2;
            return base.CompareAddresses(addr1, addr2);
        }
        throw new System.NotImplementedException();
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            Address addr => base.GetAddressHash(addr),
            _ => throw new System.NotImplementedException()
        };
    }
}