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

using Reko.Core.Machine;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.FingerPrinting;

/// <summary>
/// Computes the hash of all machine instructions in a <see cref="Procedure"/>.
/// </summary>
public class MachineInstructionHashComputer : IFeatureComputer
{
    /// <inheritdoc/>
    public string Type => "mihash";

    /// <summary>
    /// Computes a hash of the machine code of a procedure.
    /// </summary>
    /// <param name="proc">Procedure whose machine code will have its hash computed.</param>
    /// <param name="program">Program context.</param>
    /// <returns>A <see cref="MachineInstructionHashFeature"/></returns>
    public IFeature ComputeFeature(Procedure proc, Program program)
    {
        var cmp = proc.Architecture.CreateInstructionComparer(Normalize.Constants)!;
        int hash = 0;
        foreach (var block in proc.ControlGraph.Blocks)
        {
            hash = hash * 17 + ComputeBlockHash(proc.Architecture, block, program, cmp);
        }
        return new MachineInstructionHashFeature(hash);
    }

    private int ComputeBlockHash(IProcessorArchitecture arch, Block block, Program program, IEqualityComparer<MachineInstruction> cmp)
    {
        if (block.Statements.Count == 0)
            return 0;
        var addrMin = block.Statements[0].Address;
        var addrMax = block.Statements
            .Select(s => s.Address)
            .Aggregate(addrMin, (a, b) => Address.Max(a, b));
        var dasm = program.CreateDisassembler(arch, block.Address);
        int hash = 0;
        foreach (var instr in dasm)
        {
            if (instr.Address > addrMax)
                break;
            var instrHash = cmp.GetHashCode(instr);
            hash = hash * 31 + instrHash;
        }
        return hash;
    }
}
