#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Memory;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        IMemory Memory { get; }
        bool NeedsSsaTransform { get; }
        IPlatform Platform { get; }
        IReadOnlySegmentMap SegmentMap { get; }
        Encoding TextEncoding { get; }
        IReadOnlyUserData User { get; }

        bool TryCreateImageReader(IProcessorArchitecture arch, Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Determines whether an <see cref="Address"/> refers to read-only memory.
        /// </summary>
        /// <param name="addr">Address to check.</param>
        /// <returns>True if the given address specifies a location in read-only
        /// memory, otherwise false;</returns>
        public bool IsPtrToReadonlySection(Address addr);


        /// <summary>
        /// Attempt to use <paramref name="expr"/> as an <see cref="Address"/>, possibly
        /// converting <see cref="Constant"/>s to addresses as necessary.
        /// </summary>
        /// <param name="expr">Expression to be interpreted as a constant.</param>
        /// <param name="interpretAsCodePtr">True if a converted pointer needs
        /// to be adjusted (Arm Thumb code pointers have their LSB set).</param>
        /// <param name="addr">The resulting <see cref="Address"/> instance.</param>
        /// <returns>True if <paramref name="expr"/> can be interpreted as an 
        /// address.
        /// </returns>
        bool TryInterpretAsAddress(Expression expr, bool interpretAsCodePtr, [MaybeNullWhen(false)] out Address addr);
    }
}
