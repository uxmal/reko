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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using static Reko.ImageLoaders.Xbe.Structures;

namespace Reko.ImageLoaders.Xbe
{
    public enum XbeBuildType
    {
        Debug,
        Release,
        SegaChihiro
    }

    internal class XbeImage
    {
        public XbeImageHeader ImageHeader;
        public XbeBuildType BuildType;

        public Address BaseAddress;
        public Address EntryPointAddress;
        public Address KernelThunkAddress;
        public Address SectionHeadersAddress;

        public Address LibraryVersionsAddress;
        public Address KernelLibraryAddress;
        public Address XapiLibraryAddress;

        /// <summary>
        /// Upper bound for the load address in Release mode
        /// </summary>
        const uint LoadAddressUpperBound = 0x1000000;

        private XbeBuildType DetectBuildType()
        {
            if ((ImageHeader.EntryPoint & 0xf0000000) == 0x40000000)
            {
                return XbeBuildType.SegaChihiro;
            }

            if ((ImageHeader.EntryPoint ^ XBE_XORKEY_ENTRYPOINT_RELEASE) > LoadAddressUpperBound)
            {
                return XbeBuildType.Debug;
            }

            return XbeBuildType.Release;
        }

        private (UInt32, UInt32) DetermineXorKeys()
        {
            UInt32 entryXorKey;
            UInt32 kernelThunkXorKey;
            switch (this.BuildType)
            {
            case XbeBuildType.Debug:
                entryXorKey = XBE_XORKEY_ENTRYPOINT_DEBUG;
                kernelThunkXorKey = XBE_XORKEY_KERNELTHUNK_DEBUG;
                break;
            case XbeBuildType.Release:
                entryXorKey = XBE_XORKEY_ENTRYPOINT_RELEASE;
                kernelThunkXorKey = XBE_XORKEY_KERNELTHUNK_RELEASE;
                break;
            case XbeBuildType.SegaChihiro:
                entryXorKey = XBE_XORKEY_ENTRYPOINT_CHIHIRO;
                kernelThunkXorKey = XBE_XORKEY_KERNELTHUNK_CHIHIRO;
                break;
            default:
                throw new BadImageFormatException("Cannot determine XBE build type.");
            }

            return (entryXorKey, kernelThunkXorKey);
        }

        public XbeImage(XbeImageHeader hdr)
        {
            this.ImageHeader = hdr;
            this.BuildType = DetectBuildType();

            UInt32 entryXorKey, kernelThunkXorKey;
            (entryXorKey, kernelThunkXorKey) = this.DetermineXorKeys();

            EntryPointAddress = Address.Ptr32(hdr.EntryPoint ^ entryXorKey);
            KernelThunkAddress = Address.Ptr32(hdr.KernelImageThunkAddress ^ kernelThunkXorKey);
            SectionHeadersAddress = Address.Ptr32(hdr.SectionHeadersAddress);
            BaseAddress = Address.Ptr32(hdr.BaseAddress);

            LibraryVersionsAddress = Address.Ptr32(hdr.LibraryVersionsAddress);
            KernelLibraryAddress = Address.Ptr32(hdr.KernelLibraryVersionAddress);
            XapiLibraryAddress = Address.Ptr32(hdr.XapiLibraryVersionAddress);
        }
    }

    internal class XbeSection
    {
        public readonly XbeSectionHeader SectionHeader;

        public readonly Address VirtualAddress;
        public readonly Address NameAddress;

        public XbeSection(XbeSectionHeader hdr)
        {
            SectionHeader = hdr;
            VirtualAddress = Address.Ptr32(SectionHeader.VirtualAddress);
            NameAddress = Address.Ptr32(SectionHeader.SectionNameAddress);
        }
    }

    internal class XbeLibrary
    {
        public readonly XbeLibraryVersion LibraryHeader;
        public readonly string LibraryName;
        
        public XbeLibrary(XbeLibraryVersion xbeLibHeader)
        {
            LibraryHeader = xbeLibHeader;
            LibraryName = Encoding.ASCII.GetString(xbeLibHeader.LibraryName);
        }
    }

    /// <summary>
    /// Loads Xbox XBE executable files.
    /// </summary>
    /// <remarks>
    /// http://www.caustik.com/cxbx/download/xbe.htm
    /// </remarks>
    public class XbeLoader : ProgramImageLoader
    {
        private readonly LeImageReader rdr;

        private const int XBE_MAX_THUNK = 378;


        public XbeLoader(IServiceProvider services, ImageLocation imageUri, byte[] rawImage)
            : base(services, imageUri, rawImage)
        {
            rdr = new LeImageReader(rawImage);
            ctx = null!;
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return default; //the format is self describing
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private XbeImage ctx;
        private XbeImageHeader hdr;

        private long FileOffset(Address addr)
        {
            return addr - ctx.BaseAddress;
        }

        private List<ImageSegment> LoadPrimarySections() {
            List<ImageSegment> segments = new List<ImageSegment>((int) hdr.NumberOfSections + 1);

            int i;
            for (i = 0; i < hdr.NumberOfSections; i++)
            {
                XbeSectionHeader sectionHeader = rdr.ReadStruct<XbeSectionHeader>();
                XbeSection section = new XbeSection(sectionHeader);

                string sectionName = rdr.ReadAt<string>(FileOffset(section.NameAddress), (rdr) =>
                {
                    return rdr.ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString();
                });

                AccessMode accessFlgs = AccessMode.Read;
                if (sectionHeader.Flags.HasFlag(XbeSectionFlags.Executable))
                {
                    accessFlgs |= AccessMode.Execute;
                }
                if (sectionHeader.Flags.HasFlag(XbeSectionFlags.Writable))
                {
                    accessFlgs |= AccessMode.Write;
                }

                ImageSegment segment = new ImageSegment(
                    sectionName,
                    new ByteMemoryArea(section.VirtualAddress, rdr.ReadAt<byte[]>(sectionHeader.RawAddress, (rdr) =>
                    {
                        return rdr.ReadBytes(sectionHeader.RawSize);
                    })), accessFlgs);

                segments.Add(segment);
            }

            return segments;
        }

        private ImageSegment? LoadTlsSection()
        {
            Address tlsDirectoryAddress = Address.Ptr32(hdr.TlsAddress);
            XbeTls tls = rdr.ReadAt<XbeTls>(tlsDirectoryAddress - ctx.BaseAddress, (rdr) =>
            {
                return rdr.ReadStruct<XbeTls>();
            });

            if (tls.DataStartAddress != 0 && tls.DataEndAddress != 0 && tls.DataEndAddress > tls.DataStartAddress)
            {
                byte[] tlsData = new byte[tls.DataEndAddress - tls.DataStartAddress];

                ImageSegment tlsSegment = new ImageSegment(".tls", new ByteMemoryArea(
                    Address.Ptr32(tls.DataStartAddress), tlsData
                ), AccessMode.ReadWrite);

                return tlsSegment;
            }

            return null;
        }

        private Dictionary<Address, ImportReference> LoadImports(EndianImageReader importsReader)
        {
            Dictionary<Address, ImportReference> imports = new Dictionary<Address, ImportReference>();

            XbeLibrary kernelLibrary = new XbeLibrary(rdr.ReadAt(FileOffset(ctx.KernelLibraryAddress), (rdr) =>
            {
                return rdr.ReadStruct<XbeLibraryVersion>();
            }));

            for (uint i = 0; i<XBE_MAX_THUNK; i++)
            {
                Address ordinalAddress = (Address) ctx.KernelThunkAddress.Add(i * 4);
                if(!importsReader.TryReadUInt32(out uint dword))
                {
                    throw new BadImageFormatException("Unexpected EOF while reading import table.");
                }
                if (dword == 0)
                {
                    break;
                } else if((dword >> 31) == 0)
                {
                    throw new NotSupportedException("Named imports not expected in XBE files.");
                }
                int ordinalValue = (int) (dword & 0x7FFFFFFF);
                imports.Add(ordinalAddress, new OrdinalImportReference(ordinalAddress, kernelLibrary.LibraryName, ordinalValue, SymbolType.ExternalProcedure));
            }

            return imports;
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("x86-protected-32")!;
            var platform = cfgSvc.GetEnvironment("xbox").Load(Services, arch);

            this.hdr = rdr.ReadStruct<XbeImageHeader>();
            if (hdr.Magic != XBE_MAGIC)
            {
                throw new BadImageFormatException("Invalid XBE Magic");
            }
            this.ctx = new XbeImage(hdr);

            long sectionHeadersOffset = ctx.SectionHeadersAddress - ctx.BaseAddress;
            rdr.Seek(sectionHeadersOffset, System.IO.SeekOrigin.Begin);


            var segments = LoadPrimarySections();

            ImageSegment? tlsSegment = LoadTlsSection();
            if(tlsSegment is not null)
            {
                segments.Add(tlsSegment);
            }

            SegmentMap segmentMap = new SegmentMap(ctx.BaseAddress, segments.ToArray());
            var importsRdr = segmentMap.CreateImageReader(ctx.KernelThunkAddress, arch);
            var imports = LoadImports(importsRdr);

            // build program
            ImageSymbol entryPoint = ImageSymbol.Procedure(arch, ctx.EntryPointAddress);

            Program program = new Program(new ByteProgramMemory(segmentMap), arch, platform)
            {
                EntryPoints = { { ctx.EntryPointAddress, entryPoint } }
            };

            foreach (var import in imports)
            {
                program.ImportReferences.Add(import.Key, import.Value);
            }

            return program;
        }
    }
}
