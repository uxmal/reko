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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning;

/// <summary>
/// A preliminary form of a procedure consisting of RTL instructions.
/// </summary>
public class RtlProcedure : IAddressable
{
    /// <summary>
    /// Constructs an RTL procedure.
    /// </summary>
    /// <param name="arch"><see cref="IProcessorArchitecture"/> used by the procedure.</param>
    /// <param name="addr">Address of the procedure.</param>
    /// <param name="name">Name of the procedure.</param>
    /// <param name="provenance">The <see cref="ProvenanceType">procenance</see> of the
    /// procedure.</param>
    /// <param name="blocks">The basic blocks that constitute the code of the procedure.
    /// </param>
    public RtlProcedure(
        IProcessorArchitecture arch,
        Address addr,
        string name,
        ProvenanceType provenance,
        ISet<RtlBlock> blocks)
    {
        this.Architecture = arch;
        this.Address = addr;
        this.Name = name;
        this.Provenance = provenance;
        this.Blocks = blocks;
    }

    /// <summary>
    /// <see cref="IProcessorArchitecture"/> used by this procedure.
    /// </summary>
    public IProcessorArchitecture Architecture { get; }

    /// <summary>
    /// Entry address of the procedure.
    /// </summary>
    public Address Address { get; }
    
    /// <summary>
    /// The name of the procedure.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// How this procedure was discovered.
    /// </summary>
    public ProvenanceType Provenance { get; }

    /// <summary>
    /// The blocks the procedure consists of.
    /// </summary>
    public ISet<RtlBlock> Blocks { get; }


    /// <inheritdoc/>
    public override string ToString()
    {
        return $"RtlProcedure(entry: {Address}, blocks: {Blocks.Count})";
    }

    /// <summary>
    /// Dumps a textual representation to the debugging console.
    /// </summary>

    [Conditional("DEBUG")]
    public void Dump()
    {
        Debug.Print("    {0}",
            string.Join(",\r\n    ", this.Blocks
                .OrderBy(b => b.Address.ToLinear())
                .Select(b => b.Address)));
    }
}
