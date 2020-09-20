using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PEFContainer
    {
        private readonly ImageReader rdr;

        public readonly PEFContainerHeader ContainerHeader;
        private readonly PEFSectionHeader[] sectionHeaders;
        private readonly string?[] sectionNameTable;

        /////

        public IEnumerable<ImageSegment> GetImageSegments()
        {
            for(int i=0; i<ContainerHeader.sectionCount; i++)
            {
                PEFSectionHeader sectionHeader = sectionHeaders[i];
                
                byte[] containerData = rdr.ReadAt(sectionHeader.containerOffset, rdr => rdr.ReadBytes(sectionHeader.packedSize));

                if (sectionHeader.IsCompressedSection())
                {
                    // if the section is packed, the data we just read is the compressed section data we need to interpret
                    throw new NotImplementedException();
                }

                yield return new ImageSegment(
                    sectionNameTable[i] ?? $"seg{sectionHeader.defaultAddress:X8}",
                    new MemoryArea(new Address32(sectionHeader.defaultAddress), containerData),
                    sectionHeader.GetAccessMode()
                );
            }
        }

        private PEFContainerHeader ReadContainerHeader() => rdr.ReadStruct<PEFContainerHeader>();
        private IEnumerable<PEFSectionHeader> ReadSections()
        {
            for (int i = 0; i < ContainerHeader.sectionCount; i++)
            {
                yield return rdr.ReadStruct<PEFSectionHeader>();
            }       
        }

        private IEnumerable<string?> ReadSectionNameTable()
        {
            long start = rdr.Offset;

            return sectionHeaders.Select(hdr =>
            {
                return hdr.nameOffset == -1
                    ? null
                    : rdr.ReadAt(start + hdr.nameOffset, rdr => rdr.ReadCString());
            });
        }

        public PEFContainer(ImageReader rdr)
        {
            this.rdr = rdr;

            this.ContainerHeader = ReadContainerHeader();
            this.sectionHeaders = ReadSections().ToArray();
            this.sectionNameTable = ReadSectionNameTable().ToArray();
        }
    }
}
