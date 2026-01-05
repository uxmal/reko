#region License
/* Copyright (C) 1999-2026 John Källén.
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

using System.IO;

namespace Reko.Core.Loading;

/// <summary>
/// Object that can dump the contents of a binary file in a human-readable format.
/// </summary>
public interface IBinaryDumper
{
    /// <summary>
    /// Write a textual representation of the binary image to the specified writer.
    /// </summary>
    /// <param name="writer"></param>
    void DumpImageFormat(TextWriter writer);

    /// <summary>
    /// Display the contents of the overall file header
    /// </summary>
    /// <param name="writer"></param>
    void DumpFileHeader(TextWriter writer);

    /// <summary>
    /// Display object format specific file header contents
    /// </summary>
    /// <param name="writer"></param>
    void DumpPrivateHeaders(TextWriter writer);

    /// <summary>
    /// Display the contents of the section headers
    /// </summary>
    /// <param name="writer"></param>
    void DumpSectionHeaders(TextWriter writer);

    /// <summary>
    /// Display the contents of all headers
    /// </summary>
    /// <param name="writer"></param>
    void DumpAllHeaders(TextWriter writer);

    /// <summary>
    /// Display assembler contents of executable sections.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="allSections">If false, only disassembles executable
    /// sections; otherwise disassembles all sections.</param>
    void DisassembleContents(TextWriter writer, bool allSections);

    /// <summary>
    /// Display the full contents of all sections requested.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="allSections"></param>
    void DumpSectionContents(TextWriter writer, bool allSections);

    /// <summary>
    /// Display the contents of the symbol table(s)
    /// </summary>
    /// <param name="image"></param>
    /// <param name="writer"></param>
    void DumpSymbols(IBinaryImage image, TextWriter writer);

    /// <summary>
    /// Display the contents of the dynamic symbol table
    /// </summary>
    /// <param name="writer"></param>
    void DumpDynamicSymbols(TextWriter writer);

    /// <summary>
    /// Display the relocation entries in the file
    /// </summary>
    /// <param name="writer"></param>
    void DumpRelocations(TextWriter writer);
    /// <summary>
    /// Display the dynamic relocation entries in the file
    /// </summary>
    /// <param name="writer"></param>
    void DumpDynamicRelocations(TextWriter writer);
}
