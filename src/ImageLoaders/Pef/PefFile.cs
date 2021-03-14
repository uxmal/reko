using Reko.Core;
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

        private PefFile(PefContainer container, PefImageSegment[] imageSegments)
        {
            this.Container = container;
            this.pefSegments = imageSegments;
        }

        private PefLoaderInfo ProcessLoaderSection(PefImageSegment loaderSegment)
        {
            var pefLoaderSegment = PefLoaderSegment.Load(loaderSegment);
            var loaderInfo = PefLoaderInfo.Load(pefLoaderSegment);
            return loaderInfo;
        }

        private Address GetAddress(int sectionIndex, uint sectionOffset)
        {
            switch (sectionIndex)
            {
            // offset is the absolute address value
            case -2: return new Address32(sectionOffset);
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

        private void Load()
        {
            var loaderSegment = pefSegments.Where(s => s.SectionHeader.sectionKind == PEFSectionType.Loader).Single();
            LoaderInfo = ProcessLoaderSection(loaderSegment);
        }

        public static PefFile Load(
            PefContainer container,
            PefImageSegment[] imageSegments
        )
        {
            var obj = new PefFile(container, imageSegments);
            obj.Load();
            return obj;
        }
    }
}
