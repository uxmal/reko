#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.HpSom
{
    // http://webcache.googleusercontent.com/search?q=cache:xSnpsogDtIkJ:nixdoc.net/man-pages/HP-UX/man4/a.out.4.html+&cd=1&hl=en&ct=clnk&gl=se
    public class HpSomLoader : ProgramImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(HpSomLoader), "Trace HP SOM loader")
        {
            Level = TraceLevel.Verbose
        };

        public HpSomLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) :
            base(services, imageLocation, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0x00100000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var somHeader = MakeReader(0).ReadStruct<SOM_Header>();
            DumpSomHeader(somHeader);
            if (somHeader.aux_header_location == 0)
                throw new BadImageFormatException();

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = MakeArchitecture(somHeader, cfgSvc);
            var platform = cfgSvc.GetEnvironment("hpux").Load(Services, arch);

            var (symbols, addrGlobal) = ReadSymbols(arch, somHeader.symbol_location, somHeader.symbol_total, somHeader.symbol_strings_location);
            var spaces = ReadSpaces(somHeader.space_location, somHeader.space_total, somHeader.space_strings_location);
            var subspaces = ReadSubspaces(somHeader.subspace_location, somHeader.subspace_total, somHeader.space_strings_location);
            var segmentMap = MakeSegmentMapFromSubspaces(spaces, subspaces);

            var rdr = MakeReader(somHeader.aux_header_location);
            var aux = rdr.ReadStruct<aux_id>();

            switch (aux.type)
            {
            case aux_id_type.exec_aux_header:
                var program = LoadExecSegments(segmentMap, arch, platform, rdr, symbols);
                program.GlobalRegister = arch.GetRegister("r27") ?? throw new InvalidOperationException("Unable to load r27.");
                program.GlobalRegisterValue = addrGlobal;
                return program;
            default:
                throw new BadImageFormatException();
            }
        }

        private SegmentMap? MakeSegmentMapFromSubspaces(List<(string?, SpaceDictionaryRecord)> spaces, List<(string?, SubspaceDictionaryRecord subspace)> subspaces)
        {
            var imgSegments = new List<ImageSegment>();
            var memAreas = new List<MemoryArea>();      // indexed by space.
            var spaceRanges = from ss in subspaces
                    group new
                    {
                        space = ss.subspace.space_index,
                        uAddrMin = ss.subspace.subspace_start,
                        uAddrMax = ss.subspace.subspace_start + ss.subspace.subspace_length
                    } by ss.subspace.space_index into g
                    select (
                        iSpace: g.Key,
                        min: g.Min(gg => gg.uAddrMin),
                        max: g.Max(gg => gg.uAddrMax));
            foreach (var spaceRange in spaceRanges)
            {
                var mem = new ByteMemoryArea(
                    Address.Ptr32(spaceRange.min), 
                    new byte[spaceRange.max - spaceRange.min]);
                var (sSpace, space) = spaces[spaceRange.iSpace];
                for (uint i = 0; i < space.subspace_quantity; ++i)
                {
                    var (sSubspace, subspace) = subspaces[space.subspace_index + (int) i];
                    var access = AccessFromSpace(subspace);
                    var imgSegment = new ImageSegment(
                        sSubspace ?? "(no name)",
                        Address.Ptr32(subspace.subspace_start),
                        mem,
                        access);
                    var cbToCopy = Math.Min(
                        subspace.initialization_length,
                        RawImage.Length - subspace.file_loc_init_value);
                    Array.Copy(
                        RawImage,
                        subspace.file_loc_init_value,
                        mem.Bytes,
                        subspace.subspace_start - spaceRange.min,
                        cbToCopy);
                    imgSegments.Add(imgSegment);
                }
            }
            return new SegmentMap(imgSegments.ToArray());
        }

        private AccessMode AccessFromSpace(SubspaceDictionaryRecord space)
        {
            // Table 11 in PA Runtime document.
            var accessControlBits = space.attributes >> 29;
            return accessControlBits switch
            {
                0 => AccessMode.Read,
                1 => AccessMode.ReadWrite,
                2 => AccessMode.ReadExecute,
                _ => AccessMode.Read,
            };
        }

        [Conditional("DEBUG")]
        private void DumpSomHeader(SOM_Header somHeader)
        {
            Debug.Print("system_id:                     {0:X4}", somHeader.system_id); // ushort
            Debug.Print("a_magic:                       {0:X4}", somHeader.a_magic); // ushort
            Debug.Print("version_id:                {0:X8}", somHeader.version_id); // uint
            Debug.Print("file_time:                 {0:X8}", somHeader.file_time); // sys_clock
            Debug.Print("entry_space:               {0:X8}", somHeader.entry_space); // uint
            Debug.Print("entry_subspace:            {0:X8}", somHeader.entry_subspace); // uint
            Debug.Print("entry_offset:              {0:X8}", somHeader.entry_offset); // uint
            Debug.Print("aux_header_location:       {0:X8}", somHeader.aux_header_location); // uint
            Debug.Print("aux_header_size:           {0:X8}", somHeader.aux_header_size); // uint
            Debug.Print("som_length:                {0:X8}", somHeader.som_length); // uint
            Debug.Print("presumed_dp:               {0:X8}", somHeader.presumed_dp); // uint
            Debug.Print("space_location:            {0:X8}", somHeader.space_location); // uint
            Debug.Print("space_total:               {0:X8}", somHeader.space_total); // uint
            Debug.Print("subspace_location:         {0:X8}", somHeader.subspace_location); // uint
            Debug.Print("subspace_total:            {0:X8}", somHeader.subspace_total); // uint
            Debug.Print("loader_fixup_location:     {0:X8}", somHeader.loader_fixup_location); // uint
            Debug.Print("loader_fixup_total:        {0:X8}", somHeader.loader_fixup_total); // uint
            Debug.Print("space_strings_location:    {0:X8}", somHeader.space_strings_location); // uint
            Debug.Print("space_strings_size:        {0:X8}", somHeader.space_strings_size); // uint
            Debug.Print("init_array_location:       {0:X8}", somHeader.init_array_location); // uint
            Debug.Print("init_array_total:          {0:X8}", somHeader.init_array_total); // uint
            Debug.Print("compiler_location:         {0:X8}", somHeader.compiler_location); // uint
            Debug.Print("compiler_total:            {0:X8}", somHeader.compiler_total); // uint
            Debug.Print("symbol_location:           {0:X8}", somHeader.symbol_location); // uint
            Debug.Print("symbol_total:              {0:X8}", somHeader.symbol_total); // uint
            Debug.Print("fixup_request_location:    {0:X8}", somHeader.fixup_request_location); // uint
            Debug.Print("fixup_request_total:       {0:X8}", somHeader.fixup_request_total); // uint
            Debug.Print("symbol_strings_location:   {0:X8}", somHeader.symbol_strings_location); // uint
            Debug.Print("symbol_strings_size:       {0:X8}", somHeader.symbol_strings_size); // uint
            Debug.Print("unloadable_sp_location:    {0:X8}", somHeader.unloadable_sp_location); // uint
            Debug.Print("unloadable_sp_size:        {0:X8}", somHeader.unloadable_sp_size); // uint
            Debug.Print("checksum:                  {0:X8}", somHeader.checksum); // uint
        }


    private IProcessorArchitecture MakeArchitecture(in SOM_Header somHeader, IConfigurationService cfgSvc)
        {
            var archOptions = this.MakeArchitectureOptions(somHeader);
            var arch = cfgSvc.GetArchitecture("paRisc", archOptions)!;
            return arch;
        }

        private Dictionary<string, object> MakeArchitectureOptions(SOM_Header somHeader)
        {
            if (somHeader.system_id == 0x214)
            {
                return new Dictionary<string, object>
                {
                    { ProcessorOption.WordSize, 64 }
                };
            }
            else
            {
                return new Dictionary<string, object>
                {
                    { ProcessorOption.WordSize, 32 }
                };
            }
        }

        private (List<ImageSymbol>, List<ImageSymbol>) ReadDynamicLibraryInfo(
            SOM_Exec_aux_hdr exeAuxHdr,
            IProcessorArchitecture arch)
        {
            // According to HP's spec, the shlib info is at offset 0 of the $TEXT$ space.
            var dlHeader = MakeReader(exeAuxHdr.exec_tfile).ReadStruct<DlHeader>();
            uint uStrTable = exeAuxHdr.exec_tfile + dlHeader.string_table_loc;
            var imports = ReadImports(exeAuxHdr.exec_tfile + dlHeader.import_list_loc, dlHeader.import_list_count, uStrTable);
            var dltEntries = ReadDltEntries(dlHeader, exeAuxHdr.exec_dfile, imports, arch);
            var pltEntries = ReadPltEntries(dlHeader, exeAuxHdr, imports, arch);
            return (dltEntries, pltEntries);
        }

        private List<ImageSymbol> ReadPltEntries(DlHeader dlhdr, SOM_Exec_aux_hdr exeAuxHdr, List<string> names, IProcessorArchitecture arch)
        {
            trace.Verbose("HpSom: PLT entries at {0:X8}", exeAuxHdr.exec_dfile + dlhdr.plt_loc);
            var rdr = MakeReader(exeAuxHdr.exec_dfile + dlhdr.plt_loc);
            var dlts = new List<ImageSymbol>();
            for (int i = 0; i < dlhdr.plt_count; ++i)
            {
                var addr = Address.Ptr32(exeAuxHdr.exec_dmem + ((uint) rdr.Offset - exeAuxHdr.exec_dfile));
                uint n = rdr.ReadUInt32(); // address of target procedure.
                uint m = rdr.ReadUInt32();

                var name = names[i + dlhdr.dlt_count];
                var pltEntry = ImageSymbol.ExternalProcedure(arch, Address.Ptr32(n), name);
                var pltGotEntry = ImageSymbol.DataObject(arch, addr, name + "@@GOT", PrimitiveType.Ptr32);
                //dlts.Add(pltEntry);
                //dlts.Add(pltGotEntry);
                trace.Verbose("  {0}: {1}", pltEntry.Address, pltEntry.Name!);
                trace.Verbose("  {0}: {1}", pltGotEntry.Address, pltGotEntry.Name!);
                trace.Verbose("    {0:X8}", m);
                dlts.Add(pltEntry);
                dlts.Add(pltGotEntry);
            }
            return dlts;
        }

        private List<ImageSymbol> ReadDltEntries(
            DlHeader dlhdr, 
            uint uData, 
            List<string> names, 
            IProcessorArchitecture arch)
        {
            var rdr = MakeReader(uData + dlhdr.dlt_loc);
            var dlts = new List<ImageSymbol>();
            for (int i = 0; i < dlhdr.dlt_count; ++i)
            {
                var addr = Address.Ptr32((uint) rdr.Offset);
                uint n = rdr.ReadUInt32();
                dlts.Add(ImageSymbol.DataObject(arch, addr, names[i]));
            }
            return dlts;
        }

        private List<string> ReadImports(uint import_list_loc, int count, uint strTable)
        {
            trace.Inform("HpSom: reading imports, strTable: {0:X8}", strTable);
            var result = new List<string>();
            var rdr = MakeReader(import_list_loc);
            for (; count > 0; --count)
            {
                var str = ReadString(rdr.ReadUInt32(), strTable) ?? "";
                var flags = rdr.ReadUInt32();   //$TODO: do something with the flags?
                result.Add(str);
                trace.Verbose("  {0:X8} {1}", flags, str);
            }
            return result;
        }

        private List<(string?, SpaceDictionaryRecord)> ReadSpaces(uint space_location, uint count, uint uStrings)
        {
            trace.Verbose("== Spaces =========");
            var spaces = new List<(string?, SpaceDictionaryRecord)>();
            var rdr = new StructureReader<SpaceDictionaryRecord>(MakeReader(space_location));
            for (; count > 0; --count)
            {
                var space = rdr.Read();
                var name = ReadString(space.name, uStrings);
                spaces.Add((name, space));
                DumpSpace(name, space);
            }
            return spaces;
        }

        [Conditional("DEBUG")]
        private void DumpSpace(string? name, SpaceDictionaryRecord space)
        {
            trace.Verbose("  {0}", name ?? "<none>");
            trace.Verbose("    name:       {0:X8}", space.name); // uint::name
            trace.Verbose("    attributes:              {0:X8}", space.attributes);
            //unsigned int is_loadable : 1; /* space is loadable */
            //unsigned int is_defined : 1; /* space is defined within file */
            //unsigned int is_private : 1; /* space is not sharable */
            //unsigned int has_intermediate_code: 1; /* contain intermediate code */
            //unsigned int is_tspecific : 1; /* is thread specific */
            //unsigned int reserved : 11; /* reserved for future expansion */
            //unsigned int sort_key : 8; /* sort key for space */
            //unsigned int reserved2 : 8; /* reserved for future expansion */
            trace.Verbose("    space_number:            {0,8}", space.space_number); 
            trace.Verbose("    subspace_index:          {0,8}", space.subspace_index);
            trace.Verbose("    subspace_quantity:       {0:X8}", space.subspace_quantity);
            trace.Verbose("    loader_fix_index:        {0,8}", space.loader_fix_index); 
            trace.Verbose("    loader_fix_quantity:     {0:X8}", space.loader_fix_quantity); 
            trace.Verbose("    init_pointer_index:      {0,8}", space.init_pointer_index); 
            trace.Verbose("    init_pointer_quantity:   {0:X8}", space.init_pointer_quantity); 
        }

        private List<(string?, SubspaceDictionaryRecord)> ReadSubspaces(uint subspace_location, uint count, uint uStrings)
        {
            trace.Verbose("== Subspaces =========");

            var subspaces = new List<(string?, SubspaceDictionaryRecord)>();
            var rdr = new StructureReader<SubspaceDictionaryRecord>(MakeReader(subspace_location));
            for (; count > 0; --count)
            {
                var subspace = rdr.Read();
                var name = ReadString(subspace.name, uStrings);
                var access = subspace.attributes >> 25;
                subspaces.Add((name, subspace));
                DumpSubspace(name, subspace);
            }
            return subspaces;
        }

        [Conditional("DEBUG")]
        private void DumpSubspace(string? name, SubspaceDictionaryRecord space)
        {
            trace.Verbose("  {0}", name ?? "<none>");
            trace.Verbose("    space_index:             {0,8}", space.space_index);
            trace.Verbose("    attributes:              {0:X8}", space.attributes);
            trace.Verbose("    file_loc_init_value:     {0:X8}", space.file_loc_init_value);
            trace.Verbose("    initialization_length:   {0:X8}", space.initialization_length);
            trace.Verbose("    subspace_start:          {0:X8}", space.subspace_start);
            trace.Verbose("    subspace_length:         {0:X8}", space.subspace_length);
            trace.Verbose("    alignment:               {0:X8}", space.alignment);
            trace.Verbose("    name:                    {0:X8}", space.name);
            trace.Verbose("    fixup_request_index:     {0,8}", space.fixup_request_index);
            trace.Verbose("    fixup_request_quantity:  {0:X8}", space.fixup_request_quantity);
        }

        private (SortedList<Address,ImageSymbol>, Constant?) ReadSymbols(IProcessorArchitecture arch, uint sym_location, uint sym_count, uint uStrings)
        {
            trace.Inform("HpSom: reading symbols from {0:X8}, string table {1:X8}", sym_location, uStrings);
            var symbols = new Dictionary<string, symbol_dictionary_record>();
            var imageSymbols = new SortedList<Address, ImageSymbol>();
            var rdr = new StructureReader<symbol_dictionary_record>(MakeReader(sym_location));
            Constant? addrGlobal = null;
            for (uint i = 0; i < sym_count; ++i)
            {
                var symbol = rdr.Read();
                var name = ReadString(symbol.name, uStrings) ?? "";
                trace.Verbose("  {0,-10} {1:X8} {2:X8} {3:X8} {4:X8} {5}",
                    symbol.type, symbol.name, symbol.qualifier_name, symbol.info, symbol.symbol_value, name);
                // Ignore symbols starting with '$' but don't ignore symbols starting
                // with '$$'
                if (name.Length >= 2 && name[0] == '$' && name[1] != '$')
                {
                    if (string.Compare(name, "$global$", StringComparison.InvariantCultureIgnoreCase) == 0)
                        addrGlobal = Constant.Create(arch.PointerType, (ulong)symbol.symbol_value);
                    continue;
                }
                // For some reason, the bottom two bits are sometimes set for code addresses.
                if (symbol.type == SymbolType.CODE)
                    symbol.symbol_value &= ~3;
                // Imports have two symbols, only accept stub value.
                if (symbols.TryGetValue(name, out var oldSymbol))
                {
                    if (oldSymbol.type == SymbolType.STUB)
                        continue;
                }
                symbols[name] = symbol;
                switch (symbol.type)
                {
                default:
                    throw new NotImplementedException();
                case SymbolType.CODE:
                case SymbolType.ENTRY:
                case SymbolType.MILLICODE:
                    var addr = Address.Ptr32((uint) (symbol.symbol_value & ~3));
                    imageSymbols[addr] = ImageSymbol.Procedure(arch, addr, name);
                    break;
                case SymbolType.STUB:
                    addr = Address.Ptr32((uint) (symbol.symbol_value & ~3));
                    imageSymbols[addr] = ImageSymbol.ExternalProcedure(arch, addr, name);
                    break;
                case SymbolType.DATA:
                    addr = Address.Ptr32((uint) (symbol.symbol_value));
                    imageSymbols[addr] = ImageSymbol.DataObject(arch, addr, name);
                    break;
                case SymbolType.ABSOLUTE:
                case SymbolType.STORAGE:
                case SymbolType.TSTORAGE:
                    break;
                }
            }
            return (imageSymbols, addrGlobal);
        }

        private string? ReadString(uint uIndex, uint uStringsOffset)
        {
            int iStrStart = (int) (uIndex + uStringsOffset);
            int i = Array.IndexOf<byte>(RawImage, 0, iStrStart);
            if (i < 0)
                return null;
            return Encoding.ASCII.GetString(RawImage, iStrStart, i - iStrStart);
        }

        private BeImageReader MakeReader(uint uFileOffset)
        {
            return new BeImageReader(RawImage, uFileOffset);
        }

        private Program LoadExecSegments(
            SegmentMap? subspaces,
            IProcessorArchitecture arch,
            IPlatform platform,
            BeImageReader rdr,
            IDictionary<Address, ImageSymbol> symbols)
        {
            var segments = new List<ImageSegment>();
            var execAux = rdr.ReadStruct<SOM_Exec_aux_hdr>();
            DumpExecAuxHeader(execAux);
            var (dltEntries, pltEntries) = ReadDynamicLibraryInfo(execAux, arch);

            var textBytes = new byte[execAux.exec_tsize];
            var textAddr = Address.Ptr32(execAux.exec_tmem);
            Array.Copy(RawImage, (int) execAux.exec_tfile, textBytes, 0, textBytes.Length);
            var textSeg = new ImageSegment(
                ".text",
                new ByteMemoryArea(textAddr, textBytes),
                AccessMode.ReadExecute);
            segments.Add(textSeg);

            if (subspaces is null)
            {
                var dataBytes = new byte[execAux.exec_dsize];
                var dataAddr = Address.Ptr32(execAux.exec_tmem);
                Array.Copy(RawImage, (int) execAux.exec_dfile, dataBytes, 0, dataBytes.Length);
                var dataSeg = new ImageSegment(
                    ".data",
                    new ByteMemoryArea(dataAddr, dataBytes),
                    AccessMode.ReadWrite);
                segments.Add(dataSeg);
                subspaces = new SegmentMap(segments.ToArray());
            }
            var program = new Program(subspaces, arch, platform);
            var addr = Address.Ptr32(execAux.exec_entry);
            program.EntryPoints.Add(addr, ImageSymbol.Procedure(arch, addr, "_start"));
            foreach (var sym in symbols.Values)
            {
                program.ImageSymbols[sym.Address] = sym;
            }
            foreach (var dlt in dltEntries)
            {
                program.ImageSymbols[dlt.Address] = dlt;
            }
            foreach (var plt in pltEntries)
            {
                program.ImageSymbols[plt.Address] = plt;
            }
            return program;
        }

        [Conditional("DEBUG")]
        private void DumpExecAuxHeader(SOM_Exec_aux_hdr execAux)
        {
            Debug.Print("== Exec_aux_hdr");
            Debug.Print("exec_tsize:    {0:X8}", execAux.exec_tsize);
            Debug.Print("exec_tmem:     {0:X8}", execAux.exec_tmem);
            Debug.Print("exec_tfile:    {0:X8}", execAux.exec_tfile);
            Debug.Print("exec_dsize:    {0:X8}", execAux.exec_dsize);
            Debug.Print("exec_dmem:     {0:X8}", execAux.exec_dmem);
            Debug.Print("exec_dfile:    {0:X8}", execAux.exec_dfile);
            Debug.Print("exec_bsize:    {0:X8}", execAux.exec_bsize);
            Debug.Print("exec_entry:    {0:X8}", execAux.exec_entry);
            Debug.Print("exec_flags:    {0:X8}", execAux.exec_flags);
            Debug.Print("exec_bfill:    {0:X8}", execAux.exec_bfill);
        }
    }
}
