#region License
/* 
 * Copyright (C) 2018-2021 Stefano Moioli <smxdev4@gmail.com>.
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
    public class PefLoaderSegment
    {
        private readonly PefImageSegment loaderSegment;        
        private EndianByteImageReader rdr;

        public PefLoaderStringTable StringTable { get; private set; }

        public PEFLoaderInfoHeader InfoHeader { get; private set; }
        public IList<PefImportedLibrary> ImportedLibraries { get; private set; }
        public IList<PefSymbol> ImportedSymbols { get; private set; }
        public IList<PEFLoaderRelocationHeader> RelocationHeaders { get; private set; }
        public IList<PefExportHash> ExportHashTable { get; private set; }
        public IList<PefHashWord> ExportKeyTable { get; private set; }
        public IList<PEFExportedSymbol> ExportSymbolTable { get; private set; }

        private PefLoaderSegment(PefImageSegment loaderSegment)
        {
            this.loaderSegment = loaderSegment;
        }

        private PEFLoaderInfoHeader ReadInfoHeader() => new PEFLoaderInfoHeader(rdr);

        private IEnumerable<PefImportedLibrary> ReadImportedLibraries()
        {
            for(var i=0; i<InfoHeader.importedLibraryCount; i++)
            {
                yield return new PefImportedLibrary(rdr, StringTable);
            }
        }

        private IEnumerable<PefSymbol> ReadImportedSymbolTable()
        {
            for(var i=0; i<InfoHeader.totalImportedSymbolCount; i++)
            {
                var s = rdr.ReadUInt32();
                yield return new PefSymbol(s);
            }
        }

        private IEnumerable<PEFLoaderRelocationHeader> ReadRelocationHeadersTable()
        {
            for(var i=0; i<InfoHeader.relocSectionCount; i++)
            {
                yield return new PEFLoaderRelocationHeader(rdr);
            }
        }

        private IEnumerable<PefExportHash> ReadExportHashTable()
        {
            rdr.Seek(InfoHeader.exportHashOffset, System.IO.SeekOrigin.Begin);

            var count = 1 << (int) InfoHeader.exportHashTablePower;
            for(var i=0; i<count; i++)
            {
                var h = rdr.ReadUInt32();
                yield return new PefExportHash(h);
            }
        }

        private IEnumerable<PefHashWord> ReadExportKeyTable()
        {
            for(var i=0; i<InfoHeader.exportedSymbolCount; i++)
            {
                var v = rdr.ReadUInt32();
                yield return new PefHashWord(v);
            }
        }

        private IEnumerable<PEFExportedSymbol> ReadExportSymbolTable()
        {
            for (var i = 0; i < InfoHeader.exportedSymbolCount; i++)
            {
                yield return new PEFExportedSymbol(rdr);
            }
        }

        private void Initialize()
        {
            rdr = (EndianByteImageReader) loaderSegment.Segment.MemoryArea.CreateBeReader(0);
        }

        private void Load()
        {
            Initialize();
            InfoHeader = ReadInfoHeader();
            StringTable = new PefLoaderStringTable(InfoHeader, rdr);
            ImportedLibraries = ReadImportedLibraries().ToArray();
            ImportedSymbols = ReadImportedSymbolTable().ToArray();
            RelocationHeaders = ReadRelocationHeadersTable().ToArray();
            ExportHashTable = ReadExportHashTable().ToArray();
            ExportKeyTable = ReadExportKeyTable().ToArray();
            ExportSymbolTable = ReadExportSymbolTable().ToArray();
        }

        public static PefLoaderSegment Load(PefImageSegment loaderSegment)
        {
            var obj = new PefLoaderSegment(loaderSegment);
            obj.Load();
            return obj;
        }
    }
}
