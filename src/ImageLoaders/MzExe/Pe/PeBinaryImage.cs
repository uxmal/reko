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

using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Reko.ImageLoaders.MzExe.Pe;

public class PeBinaryImage : IBinaryImage
{
    public PeBinaryImage(PeHeader header)
    {
        this.Header = header;
        this.sections = new();
        this.isections = new();
        this.Exports = new();
    }

    public PeHeader Header { get; }
    IBinaryHeader IBinaryImage.Header => Header;

    EndianServices IBinaryImage.Endianness => throw new System.NotImplementedException();

    public IReadOnlyList<PeImageLoader.PeSection> Sections => sections;
    private readonly List<PeImageLoader.PeSection> sections;

    IReadOnlyList<IBinarySection> IBinaryImage.Sections => isections;
    private readonly List<IBinarySection> isections;

    IReadOnlyList<IBinarySegment> IBinaryImage.Segments => throw new System.NotImplementedException();

    IBinaryDebugInfo? IBinaryImage.DebugInfo => throw new System.NotImplementedException();

    IReadOnlyList<IBinarySymbol> IBinaryImage.Symbols => throw new System.NotImplementedException();

    IReadOnlyDictionary<int, IBinarySymbol> IBinaryImage.DynamicSymbols => throw new System.NotImplementedException();

    IReadOnlyDictionary<int, IReadOnlyList<IRelocation>> IBinaryImage.Relocations => throw new System.NotImplementedException();

    IReadOnlyList<IRelocation> IBinaryImage.DynamicRelocations => throw new System.NotImplementedException();

    public List<PeExport> Exports { get; }
    public int ExportBaseOrdinal { get; internal set; }

    Program IBinaryImage.Load()
    {
        throw new System.NotImplementedException();
    }
    

    public void AddSection(PeImageLoader.PeSection section)
    {
        this.sections.Add(section);
        this.isections.Add(section);
    }

    public void AddSections(IEnumerable<PeImageLoader.PeSection> sections)
    {
        foreach (var section in sections)
        {
            AddSection(section);
        }
    }

    public PeImageLoader.PeSection? FindSection(Predicate<PeImageLoader.PeSection> predicate)
    {
        return this.sections.Find(predicate);
    }
}
