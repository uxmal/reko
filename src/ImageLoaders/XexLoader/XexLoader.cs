#region License
/*
 * Copyright (C) 2018-2025 Stefano Moioli <smxdev4@gmail.com>.
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it 
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented;
 *    you must not claim that you wrote the original software.
 *    If you use this software in a product, an acknowledgment
 *    in the product documentation would be appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such,
 *    and must not be misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
#endregion
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Reko.ImageLoaders.Xex
{
    public class XEXPeData
    {
        public IEnumerable<ImageSegment> ImageSegments = Enumerable.Empty<ImageSegment>();
        public (Address, ImportReference)[] Thunks = Array.Empty<(Address, ImportReference)>();
    }

    /// <summary>
    /// References:
    /// https://github.com/xenia-project/xenia/blob/f540c188bf16abf3b3be4ffd00d1154fb12e5254/src/xenia/cpu/xex_module.cc
    /// https://github.com/rexdex/recompiler/blob/29c71bc26092c45508866bd7e7a0bd8a6160a898/dev/src/xenon_decompiler/xenonImageLoader.cpp
    /// </summary>
    public class XexLoader : ProgramImageLoader
    {
        private static byte[] xe_xex2_retail_key = {
            0x20, 0xB1, 0x85, 0xA5, 0x9D, 0x28, 0xFD, 0xC3,
            0x40, 0x58, 0x3F, 0xBB, 0x08, 0x96, 0xBF, 0x91};

        private static byte[] xe_xex2_devkit_key = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};


        private const string kXEX1Signature = "XEX1";
        private const string kXEX2Signature = "XEX2";

        private enum XexFormat
        {
            Xex1,
            Xex2,
        }

        private Memory<byte> mem;

        private xex2_header header;
        private xex2_security_info security_info;
        private xex2_opt_file_format_info opt_file_format_info;
        private byte[] session_key;

        private Memory<byte> peMem;

        private IEventListener eventListener;

        public XexLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) : base(services, imageLocation, imgRaw)
        {
            mem = new Memory<byte>(imgRaw);

            header = new xex2_header();
            security_info = new xex2_security_info();
            opt_file_format_info = new xex2_opt_file_format_info();
            session_key = new byte[16];
            eventListener = services.RequireService<IEventListener>();
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
        
        private Memory<byte>? GetOptHeader<T>(xex2_header_keys key) 
        {
            return GetOptHeader<T>(key, out var _);
        }

        private Memory<byte>? GetOptHeader<T>(xex2_header_keys key, out int header_offset)
        {
            var offset = GetOptHeader(key, out header_offset);
            if (!offset.HasValue) return null;
            return mem.Slice((int)offset.Value);
        }

        private uint? GetOptHeader(xex2_header_keys key)
        {
            return GetOptHeader(key, out var _);
        }

        private uint? GetOptHeader(xex2_header_keys key, out int header_offset)
        {
            header_offset = 0;

            var s = Enumerable.Range(0, (int)header.header_count)
                .Select(i => (i, header.opt_headers[i]))
                .Where(t => t.Item2.key == (uint)key);

            if (!s.Any()) return null;

            var t = s.First();
            var opt_header = t.Item2;

            header_offset = xex2_header.SIZEOF + (t.i * opt_header.SIZEOF);

            switch (opt_header.key & 0xFF)
            {
                case 0x00:
                    return opt_header.value;
                case 0x01:
                    // header holds a struct (get pointer to value)
                    var value_offset = header_offset + 4;
                    return (uint)value_offset;
                default:
                    // header holds an offset to a struct
                    return opt_header.offset;
            }
        }

        private uint GetBaseAddress()
        {
            var opt_image_base = GetOptHeader(xex2_header_keys.IMAGE_BASE_ADDRESS);
            if(opt_image_base is null) return security_info.load_address;

            var view = mem.Slice((int)opt_image_base);
            return new SpanStream(view, Endianness.BigEndian).ReadUInt32();
        }

        private uint? GetEntryPointAddress()
        {
            return GetOptHeader(xex2_header_keys.ENTRY_POINT);
        }


        private xex2_opt_import_libraries? GetImportLibraries()
        {
            Memory<byte> xex_implibs_data;
            {
                var maybe_xex_implibs_data = GetOptHeader<xex2_opt_import_libraries>(xex2_header_keys.IMPORT_LIBRARIES);
                if (maybe_xex_implibs_data is null)
                {
                    return null;
                }
                xex_implibs_data = maybe_xex_implibs_data.Value;
            }

            var xex_implibs = new xex2_opt_import_libraries(xex_implibs_data);
            return xex_implibs;
        }

        private bool IsPatch()
        {
            var flags = header.module_flags;
            return flags.HasFlag(xex2_module_flags.MODULE_PATCH)
                || flags.HasFlag(xex2_module_flags.PATCH_DELTA)
                || flags.HasFlag(xex2_module_flags.PATCH_FULL);
        }

        private static byte[] AesDecryptECB(byte[] data, byte[] key)
        {
            var aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Key = key;
            aes.Padding = PaddingMode.None;
            return aes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }

        private void ReadImageUncompressed()
        {
            int exe_length = mem.Length - (int) header.header_size;
            int uncompressed_size = exe_length;

            this.peMem = new Memory<byte>(new byte[exe_length]);
            var out_ptr = peMem;

            var ivec = new byte[16];
            var aesAlgo = Aes.Create();
            aesAlgo.BlockSize = 128;
            aesAlgo.KeySize = 128;
            aesAlgo.Mode = CipherMode.CBC;
            aesAlgo.IV = ivec;
            aesAlgo.Key = session_key;
            aesAlgo.Padding = PaddingMode.None;
            var aes = aesAlgo.CreateDecryptor();

            var p = (int)header.header_size;


            switch (opt_file_format_info.encryption_type)
            {
                case xex2_encryption_type.NONE:
                    mem.CopyTo(out_ptr);
                    break;
                case xex2_encryption_type.NORMAL:
                    var data = mem.Slice(p).ToArray();
                    for (int i = 0; i < uncompressed_size; i += 16)
                    {
                        aes.TransformBlock(data, i, 16, data, i);
                    }
                    data.CopyTo(out_ptr);
                    break;
                    
            }
            aes.Dispose();
        }

        private int PageSize()
        {
            if (GetBaseAddress() <= 0x90000000) return 64 * 1024;
            return 4 * 1024;
        }

        private void ReadImageBasicCompressed()
        {
            int exe_length = mem.Length - (int)header.header_size;
            int block_count = (int)(opt_file_format_info.info_size - 8) / 8;
            
            var comp_info = opt_file_format_info.basic_compression_info();
            var blocks = comp_info.blocks(block_count);

            var uncompressed_size = blocks.Sum(b => b.data_size + b.zero_size);
            var descrs = security_info.page_descriptors;

            var total_size = descrs.Sum(d => d.section.page_count * PageSize());

            var p = (int)header.header_size;

            var ivec = new byte[16];
            var aesAlgo = Aes.Create();
            aesAlgo.BlockSize = 128;
            aesAlgo.KeySize = 128;
            aesAlgo.Mode = CipherMode.CBC;
            aesAlgo.IV = ivec;
            aesAlgo.Key = session_key;
            aesAlgo.Padding = PaddingMode.None;
            var aes = aesAlgo.CreateDecryptor();

            this.peMem = new Memory<byte>(new byte[(int)total_size]);
            var out_ptr = peMem;

            foreach (var block in blocks)
            {
                switch (opt_file_format_info.encryption_type)
                {
                    case xex2_encryption_type.NONE:
                        mem.Slice(p, (int)block.data_size).CopyTo(out_ptr);
                        break;
                    case xex2_encryption_type.NORMAL:
                        int data_size = (int)block.data_size;
                        var data = mem.Slice(p, data_size).ToArray();
                        // in place decrypt
                        for(int i=0; i<block.data_size; i += 16)
                        {
                            aes.TransformBlock(data, i, 16, data, i);
                        }
                        data.CopyTo(out_ptr);
                        break;
                }
                p += (int)block.data_size;
                out_ptr = out_ptr.Slice((int)(block.data_size + block.zero_size));
            }

            aes.Dispose();
        }

        private (IMAGE_NT_HEADERS, IMAGE_SECTION_HEADER[]) ReadPEHeaders()
        {
            var doshdr = new IMAGE_DOS_HEADER(peMem);
            if (doshdr.e_magic != IMAGE_DOS_HEADER.IMAGE_DOS_SIGNATURE)
            {
                throw new InvalidDataException("DOS signature mismatch");
            }

            var nthdr_mem = peMem.Slice(doshdr.e_lfanew);

            var nthdr = new IMAGE_NT_HEADERS(nthdr_mem);
            if (nthdr.Signature != IMAGE_NT_HEADERS.IMAGE_NT_SIGNATURE)
            {
                throw new InvalidDataException("NT signature mismatch");
            }

            var filehdr = nthdr.FileHeader;
            if(filehdr.Machine != IMAGE_FILE_HEADER.IMAGE_FILE_MACHINE_POWERPCBE
                || (filehdr.Characteristics & IMAGE_FILE_HEADER.IMAGE_FILE_32BIT_MACHINE) != IMAGE_FILE_HEADER.IMAGE_FILE_32BIT_MACHINE)
            {
                throw new InvalidDataException("Unexpected PE Machine/Characteristics");
            }

            if(filehdr.SizeOfOptionalHeader != IMAGE_FILE_HEADER.IMAGE_SIZEOF_NT_OPTIONAL_HEADER)
            {
                throw new InvalidDataException("Unexpected SizeOfOptionalHeader");
            }

            var opthdr = nthdr.OptionalHeader;
            if(opthdr.Magic != IMAGE_OPTIONAL_HEADER.IMAGE_NT_OPTIONAL_HDR32_MAGIC)
            {
                throw new InvalidDataException("Optional Header signature mismatch");
            }

            if(opthdr.Subsystem != IMAGE_OPTIONAL_HEADER.IMAGE_SUBSYSTEM_XBOX)
            {
                throw new InvalidDataException("Unexpected Subsystem");
            }

            var section_headers = new SpanStream(nthdr_mem.Slice(nthdr.SIZEOF));
            var sections = Enumerable.Range(0, filehdr.NumberOfSections)
                .Select(_ => new IMAGE_SECTION_HEADER(section_headers))
                .ToArray();

            return (nthdr, sections);
        }

        private IEnumerable<ImageSegment> ProcessPESections(IMAGE_SECTION_HEADER[] sections)
        {
            var base_addr = GetBaseAddress();
            {
                var headersEnd = (int) sections.Min(s => s.VirtualAddress);
                var headers = peMem.Slice(0, headersEnd).ToArray();
                yield return new ImageSegment("(PE Headers)",
                    new ByteMemoryArea(Address.Ptr32(base_addr), headers), AccessMode.Read);
            }

            foreach(var s in sections)
            {
                var source = (int)s.VirtualAddress;
                byte[] sectionData;

                bool outOfBounds = (source >= peMem.Length || source + s.VirtualSize >= peMem.Length);
                if (outOfBounds)
                {
                    // section is not provided by PE, zero-fill it
                    sectionData = new byte[s.VirtualSize];
                } else
                {
                    sectionData = peMem.Slice(source, (int) s.VirtualSize).ToArray();
                }

                var prot = (AccessMode) 0;
                if (s.IsReadable) prot |= AccessMode.Read;
                if (s.IsWritable) prot |= AccessMode.Write;
                if (s.IsExecutable) prot |= AccessMode.Execute;
                yield return new ImageSegment(s.Name,
                    new ByteMemoryArea(Address.Ptr32(base_addr + s.VirtualAddress), sectionData), prot);
            }
        }

        private IEnumerable<(Address, ImportReference)> RewriteThunks(IMAGE_SECTION_HEADER[] sections)
        {
            var xex_implibs = GetImportLibraries();
            if (xex_implibs is null)
            {
                yield break;
            }

            var base_addr = GetBaseAddress();
            var peView = new SpanStream(peMem, Endianness.BigEndian);

            /**
             * we're going to process data and code thunks
             * both start with structures that describe the import (type, library index, ordinal)
             * since the structures aren't valid pointers (in case or data) or valid instructions (in case of code)
             * we will rewrite them to point to fake data
             */

            // find a place suitable for the fake data (add some arbitrary padding)
            var maxSection = sections.Aggregate((a, b) => a.VirtualAddress > b.VirtualAddress ? a : b);
            var fakeAddr = base_addr + maxSection.VirtualAddress + maxSection.VirtualSize + 0x10000;

            foreach (var lib in xex_implibs.import_libraries)
            {
                var lib_name = xex_implibs.string_table.table[lib.name_index];

                foreach (var thunk_addr in lib.import_table)
                {
                    var thunk_offset = thunk_addr - base_addr;

                    var thunk_value = peView.PerformAt(thunk_offset, () => peView.ReadUInt32());
                    var thunk = new xex2_thunk_data(thunk_value);

                    var symType = thunk.Type switch
                    {
                        xex2_thunk_type.data => SymbolType.Data,
                        xex2_thunk_type.function => SymbolType.ExternalProcedure,
                        _ => throw new BadImageFormatException("Unrecognized import type")
                    };

                    if (thunk.LibIndex >= xex_implibs.import_libraries.Length)
                    {
                        throw new BadImageFormatException("Bad thunk");
                    }

                    // rewrite the thunk
                    switch (thunk.Type)
                    {
                    case xex2_thunk_type.data:
                        // write data pointer
                        peView.PerformAt(thunk_offset, () =>
                        {
                            peView.WriteUInt32(fakeAddr);
                        });
                        break;
                    case xex2_thunk_type.function:
                        /**
                         * lis r11, user_export_addr
                         * ori r11, r11, user_export_addr
                         **/
                        var hi = (fakeAddr >> 16) & 0xFFFF;
                        var lo = fakeAddr & 0xFFFF;
                        peView.PerformAt(thunk_offset, () =>
                        {
                            peView.WriteUInt32(0x3D600000 | hi);
                            peView.WriteUInt32(0x616B0000 | lo);
                        });
                        break;
                    }
                    // arbitrary value
                    fakeAddr += 64;

                    // create and return a symbol reference (can't be done later as we just overwrote the metadata)
                    var imp_addr = Address.Ptr32(thunk_addr);
                    var imp_ref = new OrdinalImportReference(imp_addr, lib_name, thunk.Ordinal, symType);
                    yield return (imp_addr, imp_ref);
                }
            }
        }

        private XEXPeData ProcessPE()
        {
            var (nthdr, sections) = ReadPEHeaders();
            var thunks = RewriteThunks(sections).ToArray();

            var result = new XEXPeData()
            {
                ImageSegments = ProcessPESections(sections),
                Thunks = thunks
            };
            return result;
        }

        private void ReadImageCompressed()
        {
            var exe_length = mem.Length - header.header_size;
            var compress_buffer = new Memory<byte>(new byte[exe_length]);

            var ivec = new byte[16];
            var aesAlgo = Aes.Create();
            aesAlgo.BlockSize = 128;
            aesAlgo.KeySize = 128;
            aesAlgo.Mode = CipherMode.CBC;
            aesAlgo.IV = ivec;
            aesAlgo.Key = session_key;
            aesAlgo.Padding = PaddingMode.None;
            var aes = aesAlgo.CreateDecryptor();

            var input_buffer = mem.Slice((int)header.header_size);

            switch (opt_file_format_info.encryption_type)
            {
                case xex2_encryption_type.NONE:
                    break;
                case xex2_encryption_type.NORMAL:
                    input_buffer = aes.TransformFinalBlock(
                        mem.Slice((int)header.header_size, (int)exe_length).ToArray(),
                        0, (int)exe_length);
                    break;
            }

            var compression_info = opt_file_format_info.normal_compression_info();
            var cur_block = compression_info.first_block;

            int in_offset = 0;
            int out_offset = 0;
            while(cur_block.block_size > 0)
            {
                var block_data = input_buffer.Slice(in_offset, (int)cur_block.block_size).ToArray();

                var digest = SHA1.HashData(block_data);
                if(!Enumerable.SequenceEqual(digest, cur_block.block_hash))
                {
                    throw new InvalidDataException("block digest mismatch");
                }

                // skip block info
                in_offset += 4;
                in_offset += 20;

                while (true)
                {
                    var chunk_size = (input_buffer.Span[in_offset] << 8) | input_buffer.Span[in_offset + 1];
                    in_offset += 2;
                    if (chunk_size == 0) break;

                    input_buffer.Slice(in_offset, chunk_size)
                        .CopyTo(compress_buffer, out_offset);

                    in_offset += chunk_size;
                    out_offset += chunk_size;
                }

                var next_slice = input_buffer.Slice((int)cur_block.block_size);
                if(next_slice.Length == 0)
                {
                    break;
                }

                var next_block = new xex2_compressed_block_info(next_slice);
                cur_block = next_block;
            }

            var uncompressed_size = ImageSize();
            var out_data = new byte[uncompressed_size];
            this.peMem = out_data;

            var compressed_data = compress_buffer
                // d - compress_buffer
                .Slice(0, out_offset).ToArray();
            
            var window_bits = (int)Math.Log2(compression_info.window_size);

            var decoder = new LzxDecompressionMethod();
            decoder.init(window_bits, 0, compressed_data.Length, uncompressed_size, false);

            using (var in_buf = new MemoryStream(compressed_data))
            using (var out_buf = new MemoryStream(out_data))
            {
                decoder.decompress(in_buf, out_buf, uncompressed_size);
            }
            aes.Dispose();
        }

        private uint ImageSize()
        {
            uint total_size = (uint)security_info.page_descriptors.Sum(
                d => d.section.page_count * PageSize()
            );
            return total_size;
        }

        private xex2_opt_execution_info? GetExecutionInfo()
        {
            var exec_info = GetOptHeader<xex2_opt_execution_info>(xex2_header_keys.EXECUTION_INFO);
            if (exec_info is null) return null;
            return new xex2_opt_execution_info(exec_info.Value);
        }

        private xex2_opt_file_format_info? GetFileFormatInfo(out int opt_file_format_info_offset)
        {
            var file_format_info = GetOptHeader<xex2_opt_file_format_info>(
                    xex2_header_keys.FILE_FORMAT_INFO,
                    out opt_file_format_info_offset);
            
            if (file_format_info is null) return null;
            return new xex2_opt_file_format_info(file_format_info.Value);
        }

        public XEXPeData ExtractInnerPE()
        {
            header = new xex2_header(mem);
            security_info = header.security_info;

            if (IsPatch())
            {
                // $FIXME: patch
                throw new NotImplementedException("XEX2 Patches not supported yet");
            }

            var exec_info = GetExecutionInfo();

            // $FIXME?: xenia logic is to try both keys and see if the decryption is valid
            // we instead use RexDex logic to look at the title_id
            byte[] keyToUse;
            if(header.magic.AsString(Encoding.ASCII) == kXEX1Signature
                || exec_info is null
                || exec_info.title_id == 0
            ){
                keyToUse = xe_xex2_devkit_key;
            } else
            {
                keyToUse = xe_xex2_retail_key;
            }

            session_key = AesDecryptECB(security_info.aes_key, keyToUse);
            {
                var opt_file_format_info = GetFileFormatInfo(out var opt_file_format_info_offset);
                if(opt_file_format_info is null)
                {
                    throw new InvalidDataException("Missing FILE_FORMAT_INFO");
                }

                xex2_opt_header opt_header = new xex2_opt_header(mem.Slice(opt_file_format_info_offset));
                this.opt_file_format_info = opt_file_format_info;
            }


            switch (opt_file_format_info.compression_type)
            {
                case xex2_compression_type.NONE:
                    ReadImageUncompressed();
                    break;
                case xex2_compression_type.BASIC:
                    ReadImageBasicCompressed();
                    break;
                case xex2_compression_type.NORMAL:
                    ReadImageCompressed();
                    break;
            }

            return ProcessPE();
        }

        private (Address, ImageSymbol)? GetEntryPoint(IProcessorArchitecture arch)
        {
            var entry = GetEntryPointAddress();
            if (entry is null) return null;
            
            var addr = Address.Ptr32(entry.Value);
            return (addr, ImageSymbol.Procedure(arch, addr));
        }

        public override Program LoadProgram(Address? address)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc-be-64")!;
            var platform = cfgSvc.GetEnvironment("xbox360").Load(Services, arch)!;
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.Model, "xenon" }
            });

            var addrLoad = Address.Ptr32(GetBaseAddress());
            var peData = ExtractInnerPE();
            var segmentMap = new SegmentMap(addrLoad, peData.ImageSegments.ToArray());

            var ep = GetEntryPoint(arch);

            var program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform );

            if(ep is not null)
            {
                var (epAddr, epSym) = ep.Value;
                program.EntryPoints.Add(epAddr, epSym);
                program.ImageSymbols.Add(epAddr, epSym);
            }

            foreach (var imp in peData.Thunks)
            {
                program.ImportReferences.Add(imp.Item1, imp.Item2);
            }

            return program;
        }
    }
}