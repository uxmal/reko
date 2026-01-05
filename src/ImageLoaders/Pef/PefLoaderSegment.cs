#region License
/* 
 * Copyright (C) 2018-2026 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public struct PefLoaderFields
    {
        public PefLoaderStringTable StringTable;
        public PEFLoaderInfoHeader InfoHeader;
        public IList<PefImportedLibrary> ImportedLibraries;
        public IList<PefSymbol> ImportedSymbols;
        public IList<PEFLoaderRelocationHeader> RelocationHeaders;
        public IList<PefExportHash> ExportHashTable;
        public IList<PefHashWord> ExportKeyTable;
        public IList<PEFExportedSymbol> ExportSymbolTable;
    }

    public class PefLoaderSegment
    {
        public PefLoaderStringTable StringTable { get; private set; }

        public PEFLoaderInfoHeader InfoHeader { get; private set; }
        public IList<PefImportedLibrary> ImportedLibraries { get; private set; }
        public IList<PefSymbol> ImportedSymbols { get; private set; }
        public IList<PEFLoaderRelocationHeader> RelocationHeaders { get; private set; }
        public IList<PefExportHash> ExportHashTable { get; private set; }
        public IList<PefHashWord> ExportKeyTable { get; private set; }
        public IList<PEFExportedSymbol> ExportSymbolTable { get; private set; }

        public PefLoaderSegment(PefLoaderFields fields)
        {
            this.StringTable = fields.StringTable;
            this.InfoHeader = fields.InfoHeader;
            this.ImportedLibraries = fields.ImportedLibraries;
            this.ImportedSymbols = fields.ImportedSymbols;
            this.RelocationHeaders = fields.RelocationHeaders;
            this.ExportHashTable = fields.ExportHashTable;
            this.ExportKeyTable = fields.ExportKeyTable;
            this.ExportSymbolTable = fields.ExportSymbolTable;
        }
    }

    public class PefLoaderSegmentReader {

        private readonly EndianByteImageReader rdr;
        private PefLoaderFields fields;

        public PefLoaderSegmentReader(PefImageSegment loaderSegment)
        {
            fields = new PefLoaderFields();
            rdr = (EndianByteImageReader) loaderSegment.Segment.MemoryArea.CreateBeReader(0);
        }

        private PEFLoaderInfoHeader ReadInfoHeader() => PEFLoaderInfoHeader.Load(rdr);

        private IEnumerable<PefImportedLibrary> ReadImportedLibraries()
        {
            for(var i=0; i<fields.InfoHeader.importedLibraryCount; i++)
            {
                yield return PefImportedLibrary.Load(rdr, fields.StringTable);
            }
        }

        private IEnumerable<PefSymbol> ReadImportedSymbolTable()
        {
            for(var i=0; i<fields.InfoHeader.totalImportedSymbolCount; i++)
            {
                var s = rdr.ReadUInt32();
                yield return new PefSymbol(s);
            }
        }

        private IEnumerable<PEFLoaderRelocationHeader> ReadRelocationHeadersTable()
        {
            for(var i=0; i<fields.InfoHeader.relocSectionCount; i++)
            {
                yield return PEFLoaderRelocationHeader.Load(rdr);
            }
        }

        private IEnumerable<PefExportHash> ReadExportHashTable()
        {
            rdr.Seek(fields.InfoHeader.exportHashOffset, System.IO.SeekOrigin.Begin);

            var count = 1 << (int) fields.InfoHeader.exportHashTablePower;
            for(var i=0; i<count; i++)
            {
                var h = rdr.ReadUInt32();
                yield return new PefExportHash(h);
            }
        }

        private IEnumerable<PefHashWord> ReadExportKeyTable()
        {
            for(var i=0; i<fields.InfoHeader.exportedSymbolCount; i++)
            {
                var v = rdr.ReadUInt32();
                yield return new PefHashWord(v);
            }
        }

        private IEnumerable<PEFExportedSymbol> ReadExportSymbolTable()
        {
            for (var i = 0; i < fields.InfoHeader.exportedSymbolCount; i++)
            {
                yield return PEFExportedSymbol.Load(rdr);
            }
        }

        private PefLoaderFields Load()
        {
            fields.InfoHeader = ReadInfoHeader();
            fields.StringTable = new PefLoaderStringTable(fields.InfoHeader, rdr);
            fields.ImportedLibraries = ReadImportedLibraries().ToArray();
            fields.ImportedSymbols = ReadImportedSymbolTable().ToArray();
            fields.RelocationHeaders = ReadRelocationHeadersTable().ToArray();
            fields.ExportHashTable = ReadExportHashTable().ToArray();
            fields.ExportKeyTable = ReadExportKeyTable().ToArray();
            fields.ExportSymbolTable = ReadExportSymbolTable().ToArray();
            return fields;
        }

        public static PefLoaderSegment ReadLoaderSegment(PefImageSegment loaderSegment)
        {
            var obj = new PefLoaderSegmentReader(loaderSegment);
            var loaderFields = obj.Load();
            return new PefLoaderSegment(loaderFields);
        }
    }
}
