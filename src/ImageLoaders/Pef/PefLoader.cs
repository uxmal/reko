using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PefLoader : ImageLoader
    {
        private const uint ArchPpc = 0x70777063;    // 'pwpc'
        private const uint ArchM68k = 0x6D36386B;   // 'm68k'

        public PefLoader(IServiceProvider services, string filename, byte[]rawImage) : base(services, filename, rawImage)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x0010_1000);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override Program Load(Address? addrLoad)
        {
            var rdr = new BeImageReader(RawImage, 0);
            var hdr = LoadHeader(rdr);
            var sechdrs = LoadSectionHeaders(rdr, hdr.sectionCount);
            var secnames = LoadSectionNameTable(rdr, sechdrs);
            var program = BuildProgram(hdr, sechdrs, secnames, hdr.OSTypearchitecture, addrLoad);
            return program;
        }



        private Program BuildProgram(PEFContainerHeader hdr, PEFSectionHeader[] sechdrs, string[] secnames, uint uArch, Address? addrLoad)
        {
            var sections = new List<ImageSegment>();
            for (int i = 0; i < sechdrs.Length; ++i)
            {
                switch (sechdrs[i].sectionKind)
                {
                case SectionKind.Code:
                    sections.Add(LoadSection(sechdrs[i], secnames[i]));
                    break;
                case SectionKind.Unpacked:
                    sections.Add(LoadSection(sechdrs[i], secnames[i]));
                    break;
                case SectionKind.Pattern:
                case SectionKind.Constant:
                    throw new NotImplementedException();
                case SectionKind.Loader: LoadLoaderSection(sechdrs[i]);
                    break;
                case SectionKind.Executable:
                    throw new NotImplementedException();
                }
            }
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = GetArchitecture(cfgSvc, uArch);
            var platform = cfgSvc.GetEnvironment("macOS").Load(Services, arch);
            return new Program(
                new SegmentMap(
                    sections.Min(s => s.Address),
                    sections.ToArray()),
                arch,
                platform);
        }

        private ImageSegment LoadSection(PEFSectionHeader header, string  name)
        {
            var rdr = new BeImageReader(RawImage, header.containerOffset);
            var bytes = rdr.ReadBytes(header.packedSize);
            var mode = ComputeAccessMode(header.sectionKind);
            var addr = Address.Ptr32(header.defaultAddress);
            return new ImageSegment(name, new MemoryArea(addr, bytes), mode);
        }

        private AccessMode ComputeAccessMode(byte sectionKind)
        {
            switch (sectionKind)
            {
            case SectionKind.Code: return AccessMode.ReadExecute; 
            case SectionKind.Executable: return AccessMode.ReadExecute; 
            case SectionKind.Unpacked: return AccessMode.ReadWrite;
            case SectionKind.Constant: return AccessMode.Read;
            }
            throw new NotImplementedException();
        }

        private void LoadLoaderSection(PEFSectionHeader pEFSectionHeader)
        {
        }

        private string[] LoadSectionNameTable(BeImageReader rdr, PEFSectionHeader[] sechdrs)
        {
            var result = new string[sechdrs.Length];
            var iStartNameNable = rdr.Offset;
            for (int i = 0; i < result.Length; ++i)
            {
                rdr.Offset = iStartNameNable + sechdrs[i].nameOffset;
                var c = rdr.ReadCString(PrimitiveType.Char, Encoding.ASCII);
                result[i] = c.ToString();
            }
            return result;
        }

        private PEFSectionHeader[] LoadSectionHeaders(BeImageReader rdr, int sectionCount)
        {
            var result = new PEFSectionHeader[sectionCount];
            var srdr = new StructureReader<PEFSectionHeader>(rdr);
            for (int i = 0; i < result.Length; ++i)
            {
                result[0] = srdr.Read();
            }
            return result;
        }

        private PEFContainerHeader LoadHeader(BeImageReader rdr)
        {
            var srdr = new StructureReader<PEFContainerHeader>(rdr);
            return srdr.Read();
        }

        private IProcessorArchitecture GetArchitecture(IConfigurationService cfgSvc, uint uArch)
        {
            string sArch = uArch switch
            {
                ArchPpc => "ppc-be-32",
                ArchM68k => "m68k",
                _ => throw new NotSupportedException($"Architecture '{uArch:X8} is not supported."),
            };
            var arch = cfgSvc.GetArchitecture(sArch);
            if (arch is null)
                throw new InvalidOperationException($"Unable to load architecture '{sArch}'.");
            return arch;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }

        [StructLayout(LayoutKind.Sequential)]
        [Endian(Endianness.BigEndian)]
        struct PEFContainerHeader
        {
            public uint OSTypetag1;
            public uint OSTypetag2;
            public uint OSTypearchitecture;
            public uint UInt32formatVersion;
            public uint UInt32dateTimeStamp;
            public uint UInt32oldDefVersion;
            public uint UInt32oldImpVersion;
            public uint UInt32currentVersion;
            public ushort sectionCount;
            public ushort UInt16instSectionCount;
            public uint UInt32reservedA;
        }

        [StructLayout(LayoutKind.Sequential)]
        [Endian(Endianness.BigEndian)]
        public struct PEFSectionHeader
        {
            public int nameOffset;  
            public uint defaultAddress; 
            public uint totalSize;   
            public uint unpackedSize; 
            public uint packedSize;  
            public uint containerOffset; 
            public byte sectionKind;
            public byte shareKind;
            public byte alignment;
            public byte reservedA;
        }

        static class SectionKind
        {
            public const byte Code = 0;         // Contains read-only executable code in an uncompressed binary format.A container can have any number of code sections.
                                                // Code sections are always shared.
            public const byte Unpacked = 1;     // Contains uncompressed, initialized, read/write data followed by zero-initialized read/write data.
                                                // A container can have any number of data sections, each with a different sharing option.

            public const byte Pattern = 2;      // Contains read/write data initialized by a pattern specification contained in the section's contents. The contents essentially contain a small program that tells the Code Fragment Manager how to initialize the raw data in memory.
                                                // A container can have any number of pattern-initialized data sections, each with its own sharing option.

            public const byte Constant = 3;     // Contains uncompressed, initialized, read-only data.
                                                // A container can have any number of constant sections, and they are implicitly shared.

            public const byte Loader = 4;       // Loader No  Contains information about imports, exports, and entry points.See "The Loader Section" (page 8-15) for more details.
                                                // A container can have only one loader section.

            public const byte Executable = 6;   // Contains information that is both executable and modifiable. For example, this section can store code that contains embedded data.
                                                // A container can have any number of executable data sections, each with a different sharing option.
        }

    }
}
