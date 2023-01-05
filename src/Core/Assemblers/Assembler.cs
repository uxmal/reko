#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Core.Assemblers
{
	public interface IAssembler
	{
        Address StartAddress { get; }
        ICollection<ImageSymbol> EntryPoints { get; }
        ICollection<ImageSymbol> ImageSymbols { get; }
        Dictionary<Address, ImportReference> ImportReferences { get; }

        /// <summary>
        /// Assembles the provided assembly langugage program into a new
        /// <see cref="Program"/>.
        /// </summary>
        Program Assemble(Address baseAddress, TextReader reader);
        Program AssembleFragment(Address baseAddress, string asmFragment);

        /// <summary>
        /// Assembles the assembly language program from <paramref name="reader"/> and mutates 
        /// the provided <paramref name="program"/> starting at <paramref name="address"/>. 
        /// </summary>
        /// <param name="program">The program to mutate.</param>
        /// <param name="baseAddress">Location at which to start writing machine code.</param>
        /// <param name="reader">Assembly language source code.</param>
        /// <returns>The number of machine code bytes written.</returns>
        /// <remarks>
        /// Side effects include: the memory areas of the program will be
        /// mutated, and <see cref="ImageSymbol"/>s may be added to 
        /// <see cref="Program.ImageSymbols"/>.
        /// </remarks>
        int AssembleAt(Program program, Address address, TextReader reader);

        /// <summary>
        /// Assembles the assembly language instructions in <paramref name="asmFragment"/>
        /// starting at the address <paramref name="address"/>.
        /// </summary>
        /// <param name="program">Program to mutate.</param>
        /// <param name="address">Address at which to start.</param>
        /// <param name="asmFragment">String containing assembly language 
        /// instructions.</param>
        /// <returns>The number of machine code bytes written.</returns>
        int AssembleFragmentAt(Program program, Address address, string asmFragment);
    }
}
