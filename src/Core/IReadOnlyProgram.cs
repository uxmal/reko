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

using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Machine;
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
        /// <summary>
        /// The default <see cref="IProcessorArchitecture"/> of the program.
        /// </summary>
        IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// Read-only view of the <see cref="Graphs.CallGraph"/> of this program.
        /// </summary>
        IReadOnlyCallGraph CallGraph { get; }

        /// <summary>
        /// The "global" identifier, representing the space of all global variables.
        /// </summary>
        Identifier Globals { get; }

        /// <summary>
        /// The global variables in the program.
        /// </summary>
        StructureType GlobalFields { get; }

        //$TODO: move this to SsaState.
        
        /// <summary>
        /// A read-only view of the programs's induction variables.
        /// </summary>
        IReadOnlyDictionary<Identifier, LinearInductionVariable> InductionVariables { get; }
        
        /// <summary>
        /// The global memory of the program.
        /// </summary>
        IMemory Memory { get; }

        /// <summary>
        /// Policy to use when giving names to things.
        /// </summary>
        NamingPolicy NamingPolicy { get; }

        /// <summary>
        /// True if the program requires SSA transform.
        /// </summary>
        /// <remarks>
        /// Some program images are already in SSA state and 
        /// don't require SSA Transform.
        /// </remarks>
        bool NeedsSsaTransform { get; }

        /// <summary>
        /// The platform that this program is running on.
        /// </summary>
        IPlatform Platform { get; }

        /// <summary>
        /// Memory map.
        /// </summary>
        //$TODO: use the segment map in IMemory.
        IReadOnlySegmentMap SegmentMap { get; }

        /// <summary>
        /// Text encoding of the program. This is used to decode strings.
        /// </summary>
        Encoding TextEncoding { get; }

        /// <summary>
        /// Read-only view of the program's user annotations.
        /// </summary>
        IReadOnlyUserData User { get; }

        /// <summary>
        /// Creates a disassembler for the given <see cref="IProcessorArchitecture"/>, 
        /// starting at the given address <paramref name="address"/>.
        /// </summary>
        /// <param name="architecture">Processor architecture to use for the disassembler.</param>
        /// <param name="address">Address at which to start.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of disassembled instructions.
        /// </returns>
        IEnumerable<MachineInstruction> CreateDisassembler(IProcessorArchitecture architecture, Address address);

        /// <summary>
        /// Creates an <see cref="EndianImageReader"/> for the given address.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> used for byte order etc.</param>
        /// <param name="addr">Address at which to start.</param>
        /// <param name="rdr">Resulting image reader if the given address was valid.</param>
        /// <returns>True if the given address is a valid readable memory address; otherwise false.</returns>
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
