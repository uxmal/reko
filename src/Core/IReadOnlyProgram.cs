#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Read-only view of the <see cref="Program"/> class.
    /// </summary>
    /// <remarks>
    /// This avoids mutation of shared state, which makes supporting multiple
    /// concurrent tasks without having to provide lock operations.
    /// </remarks>
    public interface IReadOnlyProgram
    {
        IProcessorArchitecture Architecture { get; }
        IReadOnlyCallGraph CallGraph { get; }
        Identifier Globals { get; }
        StructureType GlobalFields { get; }
        IReadOnlyDictionary<Identifier, LinearInductionVariable> InductionVariables { get; }
        bool NeedsSsaTransform { get; }
        IPlatform Platform { get; }
        IReadOnlySegmentMap SegmentMap { get; }
        Encoding TextEncoding { get; }

        EndianImageReader CreateImageReader(IProcessorArchitecture arch, Address addr);
    }
}
