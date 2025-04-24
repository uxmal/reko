#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.IO;
using System.Collections.Generic;
using Reko.Core.Loading;

namespace Reko.Core.Assemblers
{
    /// <summary>
    /// Interface for an assembler. An assembler is responsible for converting 
    /// assembly language to machine code.
    /// </summary>
    public interface IAssembler
	{
        /// <summary>
        /// The address used as a base address for this assembler.
        /// </summary>
        Address StartAddress { get; }

        /// <summary>
        /// List of entry points in the assembly language program.
        /// </summary>
        /// <remarks>
        /// These entry points are usually denoted with <c>ORG</c>
        /// directives or the like.
        /// </remarks>
        ICollection<ImageSymbol> EntryPoints { get; }

        /// <summary>
        /// List of symbols encountered in the assembly language 
        /// program.
        /// </summary>
        ICollection<ImageSymbol> ImageSymbols { get; }

        /// <summary>
        /// Any external symbols referenced in the assembly language
        /// program.
        /// </summary>
        Dictionary<Address, ImportReference> ImportReferences { get; }

        /// <summary>
        /// Assembles the provided assembly language program file into a new
        /// <see cref="Program"/>.
        /// </summary>
        /// <param name="baseAddress">Address at which to assemble the program.</param>
        /// <param name="filename">File name of the assembly language program.</param>
        /// <param name="reader">Assembly language source code.</param>
        /// <returns>The resulting <see cref="Program"/>.</returns>
        Program Assemble(Address baseAddress, string filename, TextReader reader);

        /// <summary>
        /// Assembles the assembly language program in <paramref name="asmFragment"/> into a new
        /// </summary>
        /// <param name="baseAddress">Address at which to assemble the fragment.</param>
        /// <param name="asmFragment">Fragment of assembly language to assemble.</param>
        /// <returns>A <see cref="Program"/> containing the results of assembling.</returns>
        Program AssembleFragment(Address baseAddress, string asmFragment);

        /// <summary>
        /// Assembles the assembly language program from <paramref name="reader"/> and mutates 
        /// the provided <paramref name="program"/> starting at <paramref name="baseAddress"/>. 
        /// </summary>
        /// <param name="program">The program to mutate.</param>
        /// <param name="address">Location at which to start writing machine code.</param>
        /// <param name="reader">Assembly language source code.</param>
        /// <returns>The number of machine code bytes written.</returns>
        /// <remarks>
        /// Side effects include: the memory areas of the program will be
        /// mutated, and <see cref="ImageSymbol"/>s may be added to 
        /// <see cref="Program.ImageSymbols"/>.
        /// </remarks>
        int AssembleAt(Program program, Address baseAddress, TextReader reader);

        /// <summary>
        /// Assembles the assembly language instructions in <paramref name="asmFragment"/>
        /// starting at the address <paramref name="address"/>.
        /// </summary>
        /// <param name="program">Program to mutate.</param>
        /// <param name="address">Address at which to start the assembly.</param>
        /// <param name="asmFragment">String containing assembly language 
        /// instructions.</param>
        /// <returns>The number of machine code bytes written.</returns>
        int AssembleFragmentAt(Program program, Address address, string asmFragment);
    }
}
