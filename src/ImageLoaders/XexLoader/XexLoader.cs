#region License
/* 
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Xbox360;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.Xex.Enums;
using static Reko.ImageLoaders.Xex.Structures;

namespace Reko.ImageLoaders.Xex
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Ported from https://github.com/rexdex/recompiler/blob/master/dev/src/xenon_decompiler/xenonImageLoader.cpp
    /// </remarks>
    public class XexLoader : ImageLoader
    {
        private const uint IMAGE_SUBSYSTEM_XBOX = 14;
        private DecompilerEventListener decompilerEventListener;

        public XexLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
            decompilerEventListener = services.RequireService<DecompilerEventListener>();
            rdr = new BeImageReader(RawImage, 0);
        }

        private byte[] xe_xex2_retail_key = {
            0x20, 0xB1, 0x85, 0xA5, 0x9D, 0x28, 0xFD, 0xC3,
            0x40, 0x58, 0x3F, 0xBB, 0x08, 0x96, 0xBF, 0x91
        };

        private byte[] xe_xex2_devkit_key = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private BeImageReader rdr;

        private readonly List<OptionalHeader> optional_headers = new List<OptionalHeader>();
        private readonly ImageData xexData = new ImageData();

        public override Address PreferredBaseAddress
        {
            get
            {
                return new Address32(xexData.exe_address);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private readonly List<ImageSegment> segments = new List<ImageSegment>();
        private readonly Dictionary<Address32, ImportReference> imports = new Dictionary<Address32, ImportReference>();

        private void LoadHeaders()
        {
            ImageData imageData = xexData;
            xexData.header = new StructureReader<XexHeader>(rdr).Read();

            XexHeader header = xexData.header;

            if (header.xex2 != XEX2_MAGIC)
            {
                throw new BadImageFormatException("Invalid XEX Magic");
            }

            for(uint i=0; i<header.header_count; i++)
            {
                bool add = true;
                XexOptionalHeader st_optionalHeader = new StructureReader<XexOptionalHeader>(rdr).Read();
                OptionalHeader optionalHeader = new OptionalHeader(st_optionalHeader);

                switch((byte)optionalHeader.key)
                {
                    // just the data
                    case 0x00:
                    case 0x01:
                        optionalHeader.value = optionalHeader.offset;
                        optionalHeader.offset = 0;
                        break;
                    case 0xFF:
                        optionalHeader.length = rdr.ReadAt<UInt32>(optionalHeader.offset, (r) => {
                            return r.ReadUInt32();
                        });
                        optionalHeader.offset += 4;

                        if(optionalHeader.length + optionalHeader.offset > rdr.Bytes.Length) {
                            decompilerEventListener.Warn(
                                new NullCodeLocation(""),
                                $"Optional header {i} (0x{optionalHeader.key:X}) crosses file boundary. Will not be read"
                            );
                            add = false;
                        }
                        break;
                    default:
                        optionalHeader.length = ((uint)(byte)optionalHeader.key) * 4;

                        if (optionalHeader.length + optionalHeader.offset > rdr.Bytes.Length) {
                            decompilerEventListener.Warn(
                                new NullCodeLocation(""),
                                $"Optional header {i} (0x{optionalHeader.key:X}) crosses file boundary. Will not be read"
                            );
                            add = false;
                        }
                        break;
                }

                if (add) {
                    optional_headers.Add(optionalHeader);
                }
            }

            for(int i=0; i<optional_headers.Count; i++) {
                OptionalHeader opt = optional_headers[i];

                // go to the header offset
                if(opt.length > 0 && opt.offset != 0) {
                    rdr.Offset = opt.offset;
                }

                // process the optional headers
                switch (opt.key) {
                    case XEXHeaderKeys.XEX_HEADER_SYSTEM_FLAGS:
                        imageData.system_flags = (XEXSystemFlags)opt.value;
                        break;

                    case XEXHeaderKeys.XEX_HEADER_RESOURCE_INFO:
                        uint count = (opt.length - 4) / 16;
                        xexData.resources = new List<XexResourceInfo>((int)count);
                        
                        for(uint n=0; n<count; n++) {
                            xexData.resources.Insert(i, new StructureReader<XexResourceInfo>(rdr).Read());
                        }
                        break;

                    case XEXHeaderKeys.XEX_HEADER_EXECUTION_INFO:
                        imageData.execution_info = new StructureReader<XexExecutionInfo>(rdr).Read();
                        break;

                    case XEXHeaderKeys.XEX_HEADER_GAME_RATINGS:
                        break;

                    case XEXHeaderKeys.XEX_HEADER_TLS_INFO:
                        imageData.tls_info = new StructureReader<XexTlsInfo>(rdr).Read();
                        break;
                    
                    case XEXHeaderKeys.XEX_HEADER_IMAGE_BASE_ADDRESS:
                        imageData.exe_address = opt.value;
                        break;

                    case XEXHeaderKeys.XEX_HEADER_ENTRY_POINT:
                        imageData.exe_entry_point = opt.value;
                        break;

                    case XEXHeaderKeys.XEX_HEADER_DEFAULT_STACK_SIZE:
                        imageData.exe_stack_size = opt.value;
                        break;

                    case XEXHeaderKeys.XEX_HEADER_DEFAULT_HEAP_SIZE:
                        imageData.exe_heap_size = opt.value;
                        break;

                    case XEXHeaderKeys.XEX_HEADER_FILE_FORMAT_INFO:
                        XexEncryptionHeader encHeader = new StructureReader<XexEncryptionHeader>(rdr).Read();

                        imageData.file_format_info.encryption_type = encHeader.encryption_type;
                        imageData.file_format_info.compression_type = encHeader.compression_type;

                        switch (encHeader.compression_type) {
                            case XEXCompressionType.XEX_COMPRESSION_NONE:
                                break;
                            case XEXCompressionType.XEX_COMPRESSION_DELTA:
                                throw new NotImplementedException("XEX: image::Binary is using unsupported delta compression");
                            case XEXCompressionType.XEX_COMPRESSION_BASIC:
                                uint block_count = (opt.length - 8) / 8;
                                imageData.file_format_info.basic_blocks = new List<XexFileBasicCompressionBlock>((int)block_count);

                                for(int ib=0; ib<block_count; ib++) {
                                    imageData.file_format_info.basic_blocks.Insert(ib, new StructureReader<XexFileBasicCompressionBlock>(rdr).Read());
                                }
                                break;
                            case XEXCompressionType.XEX_COMPRESSION_NORMAL:
                                imageData.file_format_info.normal = new StructureReader<XexFileNormalCompressionInfo>(rdr).Read();
                                break;
                        }

                        if(encHeader.encryption_type != XEXEncryptionType.XEX_ENCRYPTION_NONE) {
                            //
                        }
                        break;
                    case XEXHeaderKeys.XEX_HEADER_IMPORT_LIBRARIES:
                        XexImportLibraryBlockHeader blockHeader = new StructureReader<XexImportLibraryBlockHeader>(rdr).Read();

                        long string_table = rdr.Offset;

                        rdr.Offset += blockHeader.string_table_size;

                        for (int m=0; m<blockHeader.count; m++) {
                            XexImportLibaryHeader imp_header = new StructureReader<XexImportLibaryHeader>(rdr).Read();

                            string name = "Unknown";
                            uint name_index = (byte)imp_header.name_index;

                            uint _i, _j;
                            for(_i=0, _j=0; _i<blockHeader.string_table_size;) {
                                if(_j > 0xFF) {
                                    throw new Exception();
                                }

                                if(_j == name_index) {
                                    name = rdr.ReadAt<string>(string_table + (_i * 4), r => r.ReadCString(
                                        PrimitiveType.Char, Encoding.ASCII
                                    ).ToString());
                                    break;
                                }

                                uint ptr = rdr.ReadAt<UInt32>(string_table + (_i * 4), r => r.ReadUInt32());
                                if(ptr == 0) {
                                    _i++;
                                    if((_i % 4) != 0) {
                                        _i += 4 - (_i % 4);
                                    }
                                    _j++;
                                } else {
                                    _i++;
                                }
                            }

                            if(name.Length > 0) {
                                imageData.libNames.Add(name);
                            }

                            for(uint ri=0; ri<imp_header.record_count; ++ri) {
                                UInt32 recordEntry = rdr.ReadUInt32();
                                xexData.import_records.Add(recordEntry);
                            }
                        }
                        break;
                }
            }

            // load the loader info
            {
                rdr.Offset = header.certificate_offset;
                xexData.loader_info = new StructureReader<XexLoaderInfo>(rdr).Read();
            }

            // load the sections
            {
                rdr.Offset = header.certificate_offset + 0x180;

                UInt32 sectionCount = rdr.ReadUInt32();
                xexData.sections = new List<XexSection>((int)sectionCount);

                for(int si=0; si<sectionCount; si++) {
                    xexData.sections.Insert(0, new StructureReader<XexSection>(rdr).Read());
                }
            }

            // decrypt the XEX key
            {
                byte[] keyToUse = xe_xex2_devkit_key;
                if(xexData.execution_info.title_id != 0) {
                    keyToUse = xe_xex2_retail_key;
                }

                Rijndael aes = new RijndaelManaged() {
                    BlockSize = 128,
                    KeySize = 128,
                    Mode = CipherMode.ECB,
                    Key = keyToUse,
                    Padding = PaddingMode.None
                };

                xexData.session_key = aes
                    .CreateDecryptor()
                    .TransformFinalBlock(xexData.loader_info.file_key, 0, 16);
                decompilerEventListener.Info(
                    new NullCodeLocation(""),
                    "XEX Session key: " + BitConverter.ToString(xexData.session_key).Replace("-", "")
                );
            }
        }

        private void DecryptBuffer(
            byte[] key, byte[] inputData, uint inputSize,
            byte[] outputData, uint outputSize
        )
        {
            if(xexData.file_format_info.encryption_type == XEXEncryptionType.XEX_ENCRYPTION_NONE) {
                if(inputSize != outputSize) {
                    throw new ArgumentException();
                }

                Array.Copy(inputData, outputData, inputSize);
                return;
            }

            byte[] ivec = new byte[16];

            Rijndael aes = new RijndaelManaged() {
                BlockSize = 128,
                KeySize = 128,
                Mode = CipherMode.CBC,
                Key = key,
                IV = ivec
            };

            var dec = aes.CreateDecryptor();

            int ct = 0;
            int pt = 0;
            for(uint n=0; n<inputSize; n += 16, ct += 16, pt += 16) {
                dec.TransformBlock(inputData, ct, 16, outputData, pt);
            }
        }

        private void LoadImageDataUncompressed()
        {
            // The EXE image memory is just the XEX memory - exe offset
            UInt32 memorySize = (uint)rdr.Bytes.Length - xexData.header.exe_offset;

            UInt32 maxImageSize = 128 << 20;
            if(memorySize >= maxImageSize) {
                throw new BadImageFormatException($"Computed image size is to big ({memorySize}), the exe offset = 0x{xexData.header.exe_offset}");
            }

            byte[] memory = new byte[memorySize];

            long sourceDataOff = rdr.Offset + xexData.header.exe_offset;
            byte[] sourceData = rdr.ReadAt<byte[]>(sourceDataOff, r => r.ReadBytes(memorySize));

            DecryptBuffer(
                xexData.session_key,
                sourceData, memorySize,
                memory, memorySize
            );
                

            xexData.memoryData = memory;
            xexData.memorySize = memorySize;
        }

        private void LoadImageDataBasic()
        {
            UInt32 memorySize = 0;
            int blockCount = xexData.file_format_info.basic_blocks.Count;
            for(int i=0; i<blockCount; i++) {
                XexFileBasicCompressionBlock block = xexData.file_format_info.basic_blocks[i];
                memorySize += block.data_size + block.zero_size;
            }

            UInt32 maxImageSize = 128 << 20;
            if(memorySize >= maxImageSize) {
                throw new BadImageFormatException($"Computed image size is to big ({memorySize}), the exe offset = 0x{xexData.header.exe_offset}");
            }

            byte[] memory = new byte[memorySize];

            byte[] ivec = new byte[16];
            Rijndael aes = new RijndaelManaged() {
                BlockSize = 128,
                KeySize = 128,
                Key = xexData.session_key,
                IV = ivec,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };
            var dec = aes.CreateDecryptor();

            int sourceOffset = (int)xexData.header.exe_offset;
            int destOffset = 0;

            for(int n=0; n<blockCount; n++) {
                XexFileBasicCompressionBlock block = xexData.file_format_info.basic_blocks[n];
                UInt32 data_size = block.data_size;
                UInt32 zero_size = block.zero_size;

                XEXEncryptionType encType = xexData.file_format_info.encryption_type;
                switch (encType) {
                    case XEXEncryptionType.XEX_ENCRYPTION_NONE:
                        Array.Copy(rdr.Bytes, sourceOffset, memory, destOffset, data_size);
                        break;
                    case XEXEncryptionType.XEX_ENCRYPTION_NORMAL:
                        byte[] pt = dec.TransformFinalBlock(rdr.Bytes, sourceOffset, (int)data_size);
                        Array.Copy(pt, 0, memory, destOffset, data_size);
                        break;
                }
                sourceOffset += (int)(data_size);
                destOffset += (int)(data_size + zero_size);
            }

            int consumed = sourceOffset;
            if(consumed > rdr.Bytes.Length) {
                throw new BadImageFormatException();
            } else if(consumed < rdr.Bytes.Length) {
                decompilerEventListener.Warn(new NullCodeLocation(""),
                    $"XEX: {rdr.Bytes.Length - consumed} bytes of data was not consumed in block decompression (out of {rdr.Bytes.Length})"
                );
            }

            int numOutputed = destOffset;
            if(numOutputed > memorySize) {
                throw new BadImageFormatException($"XEX: To much data was outputed in block decompression ({numOutputed} > {memorySize})");
            } else if(numOutputed < memorySize) {
                decompilerEventListener.Warn(new NullCodeLocation(""),
                    $"XEX: {memorySize - numOutputed} bytes of data was not outputed in block decompression (out of {memorySize})"
                );
            }

            xexData.memoryData = memory;
            xexData.memorySize = memorySize;
        }

        private void LoadImageDataNormal()
        {
            throw new NotImplementedException("XEX_COMPRESSION_NORMAL not supported yet");
        }

        private void LoadImageData()
        {
            XEXCompressionType compType = xexData.file_format_info.compression_type;
            switch (compType) {
                case XEXCompressionType.XEX_COMPRESSION_NONE:
                    LoadImageDataUncompressed();
                    return;
                case XEXCompressionType.XEX_COMPRESSION_BASIC:
                    LoadImageDataBasic();
                    return;
                case XEXCompressionType.XEX_COMPRESSION_NORMAL:
                    LoadImageDataNormal();
                    return;
            }

            throw new NotSupportedException();
        }

        private void LoadPEImage()
        {
            long fileDataSize = rdr.Bytes.Length - xexData.header.exe_offset;

            BeImageReader memRdr = new BeImageReader(xexData.memoryData);
            DOSHeader dosHeader = new StructureReader<DOSHeader>(memRdr).Read();
            dosHeader.Validate();

            memRdr.Offset = dosHeader.e_lfanew;

            UInt32 peSignature = memRdr.ReadUInt32();
            if(peSignature != 0x50450000) {
                throw new BadImageFormatException("PE: Invalid or Missing PE Signature");
            }

            COFFHeader coffHeader = new StructureReader<COFFHeader>(memRdr).Read();
            if(coffHeader.Machine != 0x1F2) {
                throw new BadImageFormatException($"PE: Machine type does not match Xbox360 (found 0x{coffHeader.Machine:X})");
            }

            if((coffHeader.Characteristics & 0x0100) == 0){
                throw new BadImageFormatException("PE: Only 32-bit images are supported");
            }

            if(coffHeader.SizeOfOptionalHeader != 224) {
                throw new BadImageFormatException($"PE: Invalid size of optional header (got {coffHeader.SizeOfOptionalHeader}");
            }

            PEOptHeader optHeader = new StructureReader<PEOptHeader>(memRdr).Read();
            if(optHeader.signature != 0x10b) {
                throw new BadImageFormatException($"PE: Invalid signature of optional header (got 0x{optHeader.signature})");
            }

            if(optHeader.Subsystem != IMAGE_SUBSYSTEM_XBOX) {
                throw new BadImageFormatException($"PE: Invalid subsystem (got {optHeader.Subsystem})");
            }

            xexData.peHeader = optHeader;

            uint extendedMemorySize = 0;
            uint numSections = coffHeader.NumberOfSections;

            List<PESection> peSections = new List<PESection>();

            for(uint i=0; i<numSections; i++) {
                COFFSection section = new StructureReader<COFFSection>(memRdr).Read();

                string sectionName = Encoding.ASCII.GetString(section.Name).Trim('\0');

                uint lastMemoryAddress = section.VirtualAddress + section.VirtualSize;
                if(lastMemoryAddress > extendedMemorySize) {
                    extendedMemorySize = lastMemoryAddress;
                }

                if (section.SizeOfRawData == 0) {
                    decompilerEventListener.Info(new NullCodeLocation(""),
                        $"Skipping empty section {sectionName}"
                    );
                    continue;
                }

                byte[] sectionData = memRdr.ReadAt<byte[]>(section.PointerToRawData, rdr => rdr.ReadBytes(section.SizeOfRawData));

                AccessMode acc = AccessMode.Read;
                if (section.Flags.HasFlag(PESectionFlags.IMAGE_SCN_MEM_WRITE)) {
                    acc |= AccessMode.Write;
                }
                if (section.Flags.HasFlag(PESectionFlags.IMAGE_SCN_MEM_EXECUTE)) {
                    acc |= AccessMode.Execute;
                }

                peSections.Add(new PESection(section));

                ImageSegment seg = new ImageSegment(sectionName, new MemoryArea(
                    new Address32(section.VirtualAddress), sectionData
                ), acc);
                segments.Add(seg);
            }

            if(extendedMemorySize > xexData.memorySize) {
                decompilerEventListener.Info(new NullCodeLocation(""),
                $"PE: Image sections extend beyond virtual memory range loaded from file ({extendedMemorySize} > {xexData.memorySize}). Extending by {extendedMemorySize - xexData.memorySize} bytes."
                );

                UInt32 oldMemorySize = xexData.memorySize;

                byte[] newMemoryData = new byte[extendedMemorySize];
                Array.Copy(xexData.memoryData, newMemoryData, xexData.memorySize);
                xexData.memorySize = extendedMemorySize;
                xexData.memoryData = newMemoryData;

                for(int i=0; i<peSections.Count; i++) {
                    PESection section = peSections[i];

                    if (section.PhysicalSize == 0)
                        continue;

                    if(section.PhysicalSize + section.PhysicalOffset > fileDataSize) {
                        decompilerEventListener.Warn(new NullCodeLocation(""),
                            $"PE: Section '{section.Name}' lies outside any phyisical data we have {section.PhysicalOffset} (size {section.PhysicalSize})"
                        );
                        continue;
                    }

                    if(section.VirtualOffset >= oldMemorySize) {
                        uint sizeToCopy = section.PhysicalSize;
                        if(section.VirtualSize < sizeToCopy) {
                            sizeToCopy = section.VirtualSize;
                        }

                        Array.Copy(
                            xexData.memoryData, section.PhysicalOffset,
                            newMemoryData, section.VirtualOffset,
                            sizeToCopy);
                    }
                }
            }
        }

        private void PopulateImports()
        {
            BeImageReader memRdr = new BeImageReader(xexData.memoryData);

            for (int i=0; i<xexData.import_records.Count; i++) {
                UInt32 tableAddress = xexData.import_records[i];

                UInt32 memOffset = tableAddress - xexData.exe_address;
                if(memOffset > xexData.memorySize) {
                    throw new BadImageFormatException($"XEX: invalid import record offset: 0x{memOffset}");
                }

                UInt32 value = memRdr.ReadAt<UInt32>(memOffset, rdr => rdr.ReadUInt32());

                byte type = (byte)((value & 0xFF000000) >> 24);
                byte libIndex = (byte)((value & 0x00FF0000) >> 16);

                if(type == 0) {
                    if(libIndex >= xexData.libNames.Count) {
                        throw new BadImageFormatException($"XEX: invalid import type 0 record lib index ({libIndex}, max:{xexData.libNames.Count})");
                    }

                    UInt32 importOrdinal = (value & 0xFFFF);
                    string importLibName = xexData.libNames[libIndex];
                    UInt32 importAddress = xexData.import_records[i];

                    var theAddress = new Address32(importAddress);
                    imports.Add(theAddress, new OrdinalImportReference(theAddress, importLibName, (int)importOrdinal));
                } else if(type == 1) {
                    if (libIndex >= xexData.libNames.Count) {
                        throw new BadImageFormatException($"XEX: invalid import type 0 record lib index ({libIndex}, max:{xexData.libNames.Count})");
                    }

                    UInt32 importOrdinal = (value & 0xFFFF);
                    string importLibName = xexData.libNames[libIndex];
                    UInt32 importAddress = xexData.import_records[i];

                    var theAddress = new Address32(importAddress);
                    imports.Add(theAddress, new OrdinalImportReference(theAddress, importLibName, (int)importOrdinal));
                }
            }
        }

        public override Program Load(Address addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc-be-64");
            var platform = cfgSvc.GetEnvironment("xbox360").Load(Services, arch);

            LoadHeaders();
            LoadImageData();
            LoadPEImage();

            PopulateImports();

            var segmentMap = new SegmentMap(addrLoad, segments.ToArray());

            var entryPointAddress = new Address32(xexData.exe_entry_point);
            var entryPoint = new ImageSymbol(entryPointAddress) { Type = SymbolType.Procedure };

            var program = new Program(
                segmentMap,
                arch,
                platform
            ) {
                ImageSymbols = { { entryPointAddress, entryPoint } },
                EntryPoints = { { entryPointAddress, entryPoint } },
            };

            foreach(var import in imports) {
                program.ImportReferences.Add(import.Key, import.Value);
            }

            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
