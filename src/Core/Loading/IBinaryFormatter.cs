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

using System.IO;

namespace Reko.Core.Loading;

/// <summary>
/// Renders the contents of an <see cref="IBinaryImage"/>
/// to text.
/// </summary>
public interface IBinaryFormatter
{
    void FormatArchiveHeaders(TextWriter writer);
    void FormatPrivateHeaders(TextWriter writer);
    void FormatAllHeaders(TextWriter writer);
    void FormatSectionHeaders(TextWriter writer);
    void DisassembleExecutableSections(TextWriter writer);
    void DisassembleAllSections(TextWriter writer);
    void FormatWithSource(string prefix, TextWriter writer);
    void FormatAllSectionContents(TextWriter writer);
    void FormatDebugInformation(TextWriter writer);
    void FormatSymbols(TextWriter writer);
    void FormatDynamicSymbols(TextWriter writer);
    void FormatRelocations(TextWriter writer);
    void FormatDynamicRelocations(TextWriter writer);
}
