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

namespace Reko.ImageLoaders.Elf;

public class ElfBinaryImage : IBinaryImage
{
    public ElfBinaryImage(ImageLocation location, ElfHeader header, EndianServices endianness)
    {
        Location = location;
        Header = header;
        Endianness = endianness;
        sections = new List<ElfSection>();
        iSections = new List<IBinarySection>();
        segments = new List<ElfSegment>();
        iSegments = new List<IBinarySegment>();
        Symbols = new List<IBinarySymbol>();
        dynsyms = new Dictionary<int, IBinarySymbol>();
        relocations = new Dictionary<int, List<ElfRelocation>>();
        irelocations = new Dictionary<int, IReadOnlyList<IRelocation>>();
        dynamicRelocations = new List<ElfRelocation>();
        idynamicRelocations = new List<IRelocation>();

        DynamicEntries = new Dictionary<long, ElfDynamicEntry>();
        SymbolsByFileOffset = new Dictionary<ulong, Dictionary<int, ElfSymbol>>();
        Dependencies = new List<string>();
    }

    public ImageLocation Location { get; }

    public ElfHeader Header { get; set; }

    IBinaryHeader IBinaryImage.Header => Header;

    public EndianServices Endianness { get; set; }

    public IReadOnlyList<ElfSection> Sections => sections;
    private readonly List<ElfSection> sections;
    IReadOnlyList<IBinarySection> IBinaryImage.Sections => iSections;
    private readonly List<IBinarySection> iSections;

    public IReadOnlyList<ElfSegment> Segments => segments;
    private readonly List<ElfSegment> segments;
    IReadOnlyList<IBinarySegment> IBinaryImage.Segments => iSegments;
    private readonly List<IBinarySegment> iSegments;

    public IBinaryDebugInfo? DebugInfo { get; set; }

    public IReadOnlyList<IBinarySymbol> Symbols { get; }

    public IReadOnlyDictionary<int, IBinarySymbol> DynamicSymbols => dynsyms;
    private readonly Dictionary<int, IBinarySymbol> dynsyms;



    public IReadOnlyDictionary<int, List<ElfRelocation>> Relocations => relocations;
    private readonly Dictionary<int, List<ElfRelocation>> relocations;
    IReadOnlyDictionary<int, IReadOnlyList<IRelocation>> IBinaryImage.Relocations => irelocations;

    public Dictionary<int, IReadOnlyList<IRelocation>> irelocations;

    public List<ElfRelocation> DynamicRelocations => dynamicRelocations;
    private readonly List<ElfRelocation> dynamicRelocations;

    IReadOnlyList<IRelocation> IBinaryImage.DynamicRelocations => idynamicRelocations;
    private readonly List<IRelocation> idynamicRelocations;
    
    public Dictionary<long, ElfDynamicEntry> DynamicEntries { get; }


    public Dictionary<ulong, Dictionary<int, ElfSymbol>> SymbolsByFileOffset { get; }
    public List<string> Dependencies { get; }

    public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
    {
        return visitor.VisitBinaryImage(this, context);
    }

    public void AddSection(ElfSection section)
    {
        this.sections.Add(section);
        this.iSections.Add(section);
    }

    public IBinaryDumper CreateImageDumper()
    {
        return new ElfBinaryDumper(this);
    }

    public Program Load(Address? addrBase)
    {
        throw new NotImplementedException();
    }

    public void AddSections(List<ElfSection> sections)
    {
        this.sections.AddRange(sections);
        this.iSections.AddRange(sections);
    }

    public void AddSegment(ElfSegment elfSegment)
    {
        this.segments.Add(elfSegment);
        this.iSegments.Add(elfSegment);
    }

    public void AddSegments(List<ElfSegment> segments)
    {
        this.segments.AddRange(segments);
        this.iSegments.AddRange(segments);
    }

    public void AddDynamicSymbols(Dictionary<int, ElfSymbol> symtab)
    {
        foreach (var (i, s) in symtab)
        {
            dynsyms.Add(i, s);
        }
    }

    internal void AddRelocations(int isection, IReadOnlyList<ElfRelocation> relocList)
    {
        if (!this.relocations.TryGetValue(isection, out var elfRels))
        {
            elfRels = new List<ElfRelocation>();
            this.relocations.Add(isection, elfRels);
        }
        elfRels.AddRange(relocList);
        if (!this.irelocations.TryGetValue(isection, out var irels))
        {
            irels = new List<IRelocation>();
            irelocations.Add(isection, irels);
        }
            ((List<IRelocation>) irels).AddRange(relocList);
    }

    internal void AddDynamicRelocations(IReadOnlyCollection<ElfRelocation> relocs)
    {
        DynamicRelocations.AddRange(relocs);
        idynamicRelocations.AddRange(relocs);
    }
}
