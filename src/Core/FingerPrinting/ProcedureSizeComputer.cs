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

using System;
using System.Linq;

namespace Reko.Core.FingerPrinting;

/// <summary>
/// Computes the size of a procedure in number of machine code instructions.
/// </summary>
public class ProcedureSizeComputer : IFeatureComputer
{
    /// <inheritdoc/>
    public string Type => "procsize";

    /// <summary>
    /// Computes the size of the procedure in number of instructions.
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="program"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IFeature ComputeFeature(Procedure proc, Program program)
    {
        var size = proc.ControlGraph.Blocks
            .Select(b => CountMachineInstructions(b, program))
            .Sum();
        return new ProcedureSizeFeature(size);
    }

    private int CountMachineInstructions(Block block, Program program)
    {
        if (block.Statements.Count == 0)
            return 0;
        var addrMin = block.Statements[0].Address;
        var addrMax = block.Statements
            .Select(s => s.Address)
            .Aggregate((a, b) => Address.Max(a, b));
        int cInstr = 0;
        var dasm = program.CreateDisassembler(block.Procedure.Architecture, addrMin);
        foreach (var instr in dasm)
        {
            if (instr.Address >= addrMax)
                break;
            ++cInstr;
        }
        return cInstr;
    }
}
