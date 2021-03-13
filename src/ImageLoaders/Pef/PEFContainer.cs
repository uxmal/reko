using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PEFContainer
    {
        public readonly PEFContainerHeader ContainerHeader;
        private readonly PEFSectionHeader[] sectionHeaders;
        private readonly string?[] sectionNameTable;

        public PEFContainer(PEFContainerHeader hdr, PEFSectionHeader[] secHdrs, string[] sectionNameTable)
        {
            this.ContainerHeader = hdr;
            this.sectionHeaders = secHdrs;
            this.sectionNameTable = sectionNameTable;
        }

        /////

        public IEnumerable<ImageSegment> GetImageSegments(EndianByteImageReader rdr, Address addrLoad)
        {
            var addr = addrLoad;
            for (int i=0; i<ContainerHeader.sectionCount; i++)
            {
                PEFSectionHeader sectionHeader = sectionHeaders[i];
                
                byte[] containerData = rdr.ReadAt(sectionHeader.containerOffset, rdr => rdr.ReadBytes(sectionHeader.packedSize));

                if (sectionHeader.IsCompressedSection())
                {
                    // if the section is packed, the data we just read is the compressed section data we need to interpret
                    throw new NotImplementedException();
                }

                if (sectionHeader.defaultAddress != 0)
                    addr = Address.Ptr32(sectionHeader.defaultAddress);
                else
                    addr = addrLoad;
                var segment = new ImageSegment(
                    sectionNameTable[i] ?? $"seg{sectionHeader.defaultAddress:X8}",
                    new ByteMemoryArea(addr, containerData),
                    sectionHeader.GetAccessMode());
                yield return segment;

                addrLoad = (segment.Address + containerData.Length).Align(0x1000);
            }
        }

        private static IEnumerable<PEFSectionHeader> ReadSections(PEFContainerHeader containerHeader, EndianByteImageReader rdr)
        {
            for (int i = 0; i < containerHeader.sectionCount; i++)
            {
                yield return rdr.ReadStruct<PEFSectionHeader>();
            }       
        }

        private static IEnumerable<string> ReadSectionNameTable(IEnumerable<PEFSectionHeader> sectionHeaders, EndianByteImageReader rdr)
        {
            long start = rdr.Offset;

            return sectionHeaders.Select(hdr =>
            {
                return hdr.nameOffset == -1
                    ? "(unnamed)"
                    : rdr.ReadAt(start + hdr.nameOffset, rdr => rdr.ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString());
            });
        }

        public static PEFContainer Load(EndianByteImageReader rdr)
        {
            var hdr = rdr.ReadStruct<PEFContainerHeader>();
            if (hdr.sectionCount == 0)
                throw new BadImageFormatException($"Binary image has 0 sections.");
            var secHdrs = ReadSections(hdr, rdr).ToArray();
            var sectionNameTable = ReadSectionNameTable(secHdrs, rdr).ToArray();
            var result = new PEFContainer(hdr, secHdrs, sectionNameTable);
            return result;
        }
    }
}
