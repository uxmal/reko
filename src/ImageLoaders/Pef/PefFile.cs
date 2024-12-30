#region License
/* 
 * Copyright (C) 2018-2025 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PefFile
    {
        public readonly PefContainer Container;
        public PefLoaderInfo LoaderInfo;

        private PefImageSegment[] pefSegments;

        private PefFile(PefContainer container, PefImageSegment[] imageSegments, PefLoaderInfo loaderInfo)
        {
            this.Container = container;
            this.pefSegments = imageSegments;
            this.LoaderInfo = loaderInfo;
        }

        /// <summary>
        /// Obtains an address from a section index and an offset
        /// </summary>
        /// <param name="sectionIndex"></param>
        /// <param name="sectionOffset"></param>
        /// <returns></returns>
        private Address GetAddress(int sectionIndex, uint sectionOffset)
        {
            switch (sectionIndex)
            {
            // offset is the absolute address value
            case -2: return Address.Ptr32(sectionOffset);
            // offset is the imported symbol index
            case -3: throw new NotImplementedException();
            }

            var baseAddr = pefSegments[sectionIndex].Segment.Address;
            return baseAddr + sectionOffset;
        }

        public IEnumerable<ImageSymbol> GetEntryPoints(IProcessorArchitecture arch)
        {
            var infoHdr = LoaderInfo.Loader.InfoHeader;
            if (infoHdr.initSection > -1)
            {
                yield return ImageSymbol.Procedure(
                    arch,
                    GetAddress(infoHdr.initSection, infoHdr.initOffset),
                    "init"
                );
            }

            if(infoHdr.mainSection > -1)
            {
                yield return ImageSymbol.Procedure(
                    arch,
                    GetAddress(infoHdr.mainSection, infoHdr.mainOffset),
                    "main"
                );
            }
        }

        public IEnumerable<ImageSymbol> GetSymbols(IProcessorArchitecture arch)
        {
            var ptrCodeT = new Pointer(new CodeType(), arch.WordWidth.BitSize);

            return LoaderInfo.ExportedSymbols
                // $TODO: resolve import addresses in relocations
                .Where(s => s.sym.sectionIndex != -3)
                .Select(s =>
            {
                var symAddr = GetAddress(s.sym.sectionIndex, s.sym.symbolValue);

                return s.classAndName.SymbolClass switch
                {
                    PEFSymbolClassType.kPEFCodeSymbol => ImageSymbol.DataObject(arch, symAddr, s.Name, ptrCodeT),
                    PEFSymbolClassType.kPEFDataSymbol => ImageSymbol.DataObject(arch, symAddr, s.Name, new UnknownType()),
                    PEFSymbolClassType.kPEFTVectSymbol => ImageSymbol.Procedure(arch, symAddr, s.Name),
                    PEFSymbolClassType.kPEFTOCSymbol => ImageSymbol.DataObject(arch, symAddr, s.Name, new UnknownType()),
                    PEFSymbolClassType.kPEFGlueSymbol => ImageSymbol.DataObject(arch, symAddr, s.Name, new UnknownType()),
                    _ => throw new BadImageFormatException($"Unknown symbol class {s.classAndName.SymbolClass}"),
                };
            });
        }

        private static PefLoaderInfo ProcessLoaderSection(PefImageSegment loaderSegment)
        {
            var pefLoaderSegment = PefLoaderSegmentReader.ReadLoaderSegment(loaderSegment);
            var loaderInfo = PefLoaderInfo.Load(pefLoaderSegment);
            return loaderInfo;
        }

        private static PefLoaderInfo ProcessLoaderSection(PefImageSegment[] pefSegments)
        {
            var loaderSegment = pefSegments.Where(s => s.SectionHeader.sectionKind == PEFSectionType.Loader).Single();
            return ProcessLoaderSection(loaderSegment);
        }

        public static PefFile Load(
            PefContainer container,
            PefImageSegment[] imageSegments
        )
        {
            var loaderInfo = ProcessLoaderSection(imageSegments);
            var obj = new PefFile(container, imageSegments, loaderInfo);
            return obj;
        }
    }
}
