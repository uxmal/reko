#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.HpSom
{
    // http://webcache.googleusercontent.com/search?q=cache:xSnpsogDtIkJ:nixdoc.net/man-pages/HP-UX/man4/a.out.4.html+&cd=1&hl=en&ct=clnk&gl=se
    public class HpSomLoader : ProgramImageLoader
    {
        private static TraceSwitch trace = new TraceSwitch(nameof(HpSomLoader), "Trace HP SOM loader")
        {
            Level = TraceLevel.Verbose
        };
        private List<ImageSymbol> pltEntries;
        private SortedList<Address, ImageSymbol> symbols;

        public HpSomLoader(IServiceProvider services, ImageLocation imageUri, byte[] imgRaw) :
            base(services, imageUri, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0x00100000);
            pltEntries = null!;
            symbols = default!;
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {

            var somHeader = MakeReader(0).ReadStruct<SOM_Header>();

            if (somHeader.aux_header_location == 0)
                throw new BadImageFormatException();

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = MakeArchitecture(somHeader, cfgSvc);
            var platform = cfgSvc.GetEnvironment("hpux").Load(Services, arch);

            this.symbols = ReadSymbols(arch, somHeader.symbol_location, somHeader.symbol_total, somHeader.symbol_strings_location);
            var spaces = ReadSpaces(somHeader.space_location, somHeader.space_total, somHeader.space_strings_location);
            var subspaces = ReadSubspaces(somHeader.subspace_location, somHeader.subspace_total, somHeader.space_strings_location);


            var rdr = MakeReader(somHeader.aux_header_location);
            var aux = rdr.ReadStruct<aux_id>();

            switch (aux.type)
            {
            case aux_id_type.exec_aux_header:
                var program = LoadExecSegments(somHeader, arch, platform, rdr);
                return program;
            default:
                throw new BadImageFormatException();
            }
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

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                symbols);
        }

        private object? ReadDynamicLibraryInfo(
            SOM_Exec_aux_hdr exeAuxHdr,
            IProcessorArchitecture arch)
        {
            // According to HP's spec, the shlib info is at offset 0 of the $TEXT$ space.
            var dlHeader = MakeReader(exeAuxHdr.exec_tfile).ReadStruct<DlHeader>();
            uint uStrTable = exeAuxHdr.exec_tfile + dlHeader.string_table_loc;
            var imports = ReadImports(exeAuxHdr.exec_tfile + dlHeader.import_list_loc, dlHeader.import_list_count, uStrTable);
            var dltEntries = ReadDltEntries(dlHeader, exeAuxHdr.exec_dfile, imports);
            this.pltEntries = ReadPltEntries(dlHeader, exeAuxHdr, imports, arch);
            return null;
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
            }
            return dlts;
        }

        private object ReadDltEntries(DlHeader dlhdr, uint uData, List<string> names)
        {
            var rdr = MakeReader(uData + dlhdr.dlt_loc);
            var dlts = new List<(string, uint)>();
            for (int i = 0; i < dlhdr.dlt_count; ++i)
            {
                var addr = Address.Ptr32((uint) rdr.Offset);
                uint n = rdr.ReadUInt32();
                dlts.Add((names[i], n));
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
            var spaces = new List<(string?, SpaceDictionaryRecord)>();
            var rdr = new StructureReader<SpaceDictionaryRecord>(MakeReader(space_location));
            for (; count > 0; --count)
            {
                var space = rdr.Read();
                var name = ReadString(space.name, uStrings);
                spaces.Add((name, space));
            }
            return spaces;
        }

        private List<(string?, SubspaceDictionaryRecord)> ReadSubspaces(uint subspace_location, uint count, uint uStrings)
        {
            var subspaces = new List<(string?, SubspaceDictionaryRecord)>();
            var rdr = new StructureReader<SubspaceDictionaryRecord>(MakeReader(subspace_location));
            for (; count > 0; --count)
            {
                var subspace = rdr.Read();
                var name = ReadString(subspace.name, uStrings);
                var access = subspace.attributes >> 25;
                subspaces.Add((name, subspace));
            }
            return subspaces;
        }

        private SortedList<Address,ImageSymbol> ReadSymbols(IProcessorArchitecture arch, uint sym_location, uint sym_count, uint uStrings)
        {
            trace.Inform("HpSom: reading symbols from {0:X8}, string table {1:X8}", sym_location, uStrings);
            var symbols = new Dictionary<string, symbol_dictionary_record>();
            var imageSymbols = new SortedList<Address, ImageSymbol>();
            var rdr = new StructureReader<symbol_dictionary_record>(MakeReader(sym_location));
            for (uint i = 0; i < sym_count; ++i)
            {
                var symbol = rdr.Read();
                var name = ReadString(symbol.name, uStrings) ?? "";
                trace.Verbose("  {0,-10} {1:X8} {2:X8} {3:X8} {4:X8} {5}",
                    symbol.type, symbol.name, symbol.qualifier_name, symbol.info, symbol.symbol_value, name);
                // Ignore symbols starting with '$' but don't ignore symbols starting
                // with '$$'
                if (name.Length >= 2 && name[0] == '$' && name[1] != '$')
                    continue;
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
            return imageSymbols;
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

        private Program LoadExecSegments(in SOM_Header somHeader, IProcessorArchitecture arch, IPlatform platform, BeImageReader rdr)
        {
            var segments = new List<ImageSegment>();
            var execAux = rdr.ReadStruct<SOM_Exec_aux_hdr>();
            var dlHeaderRdr = ReadDynamicLibraryInfo(execAux, arch);

            var textBytes = new byte[execAux.exec_tsize];
            var textAddr = Address.Ptr32(execAux.exec_tmem);
            Array.Copy(RawImage, (int) execAux.exec_tfile, textBytes, 0, textBytes.Length);
            var textSeg = new ImageSegment(
                ".text",
                new ByteMemoryArea(textAddr, textBytes),
                AccessMode.ReadExecute);
            segments.Add(textSeg);

            var dataBytes = new byte[execAux.exec_dsize];
            var dataAddr = Address.Ptr32(execAux.exec_tmem);
            Array.Copy(RawImage, (int) execAux.exec_dfile, dataBytes, 0, dataBytes.Length);
            var dataSeg = new ImageSegment(
                ".data",
                new ByteMemoryArea(dataAddr, dataBytes),
                AccessMode.ReadWrite);
            segments.Add(dataSeg);

            var segmap = new SegmentMap(
                segments.Min(s => s.Address),
                segments.ToArray());
            return new Program(segmap, arch, platform);
        }

    }
}
