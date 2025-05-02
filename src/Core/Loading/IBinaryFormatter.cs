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

using System.IO;

namespace Reko.Core.Loading;

/// <summary>
/// Renders the contents of an <see cref="IBinaryImage"/>
/// to text.
/// </summary>
public interface IBinaryFormatter
{
    /// <summary>
    /// Writes out the headers of an archive.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatArchiveHeaders(TextWriter writer);

    /// <summary>
    /// Writes out the segments of a binary header.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatPrivateHeaders(TextWriter writer);

    /// <summary>
    /// Writes out all headers of a binary image.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatAllHeaders(TextWriter writer);

    /// <summary>
    /// Writes out the section headers of a binary image.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatSectionHeaders(TextWriter writer);

    /// <summary>
    /// Writes out the disassembled contents of executable sections.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void DisassembleExecutableSections(TextWriter writer);

    /// <summary>
    /// Writes out the disassembled contents of all sections.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void DisassembleAllSections(TextWriter writer);

    /// <summary>
    /// Writes out disassembly mingled with source if available.
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="writer"></param>
    void FormatWithSource(string prefix, TextWriter writer);

    /// <summary>
    /// Write out the contents of all sections.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatAllSectionContents(TextWriter writer);

    /// <summary>
    /// Write out debugging information.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatDebugInformation(TextWriter writer);

    /// <summary>
    /// Write out static image symbols.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatSymbols(TextWriter writer);

    /// <summary>
    /// Write out dynamic image symbols.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    void FormatDynamicSymbols(TextWriter writer);

    /// <summary>
    /// Write out static relocations.
    /// </summary>
    /// <param name="writer"></param>
    void FormatRelocations(TextWriter writer);

    /// <summary>
    /// Write out dynamic relocations.
    /// </summary>
    /// <param name="writer"></param>
    void FormatDynamicRelocations(TextWriter writer);
}

/// <summary>
/// A dummy binary formatter that does nothing.
/// </summary>
public class NullBinaryFormatter : IBinaryFormatter
{
    /// <inheritdoc/>
    public void DisassembleAllSections(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void DisassembleExecutableSections(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatAllHeaders(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatAllSectionContents(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatArchiveHeaders(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatDebugInformation(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatDynamicRelocations(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatDynamicSymbols(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatPrivateHeaders(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatRelocations(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatSectionHeaders(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatSymbols(TextWriter writer)
    {
    }

    /// <inheritdoc/>
    public void FormatWithSource(string prefix, TextWriter writer)
    {
    }
}