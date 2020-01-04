#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.HpSom
{
    // http://webcache.googleusercontent.com/search?q=cache:xSnpsogDtIkJ:nixdoc.net/man-pages/HP-UX/man4/a.out.4.html+&cd=1&hl=en&ct=clnk&gl=se
    public class HpSomLoader : ImageLoader
    {
        private List<ImageSymbol> pltEntries;

        public HpSomLoader(IServiceProvider services, string filename, byte[] imgRaw) :
            base(services, filename, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0x00100000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var somHeader = MakeReader(0).ReadStruct<SOM_Header>();

            if (somHeader.aux_header_location == 0)
                throw new BadImageFormatException();

            var spaces = ReadSpaces(somHeader.space_location, somHeader.space_total, somHeader.space_strings_location);
            var subspaces = ReadSubspaces(somHeader.subspace_location, somHeader.subspace_total, somHeader.space_strings_location);

            var rdr = MakeReader(somHeader.aux_header_location);
            var aux = rdr.ReadStruct<aux_id>();

            switch (aux.type)
            {
            case aux_id_type.exec_aux_header:
                var program = LoadExecSegments(rdr);
                SetProgramOptions(somHeader, program);
                return program;
            default:
                throw new BadImageFormatException();
            }
        }

        private void SetProgramOptions(SOM_Header somHeader, Program program)
        {
            if (somHeader.system_id == 0x214)
            {
                program.Architecture.LoadUserOptions(new Dictionary<string, object>
                {
                    { "WordSize", 64 }
                });
            }
            else
            {
                program.Architecture.LoadUserOptions(new Dictionary<string, object>
                {
                    { "WordSize", 32 }
                });
            }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                pltEntries.ToSortedList(e => e.Address));
        }

        private object ReadDynamicLibraryInfo(
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
            var rdr = MakeReader(exeAuxHdr.exec_dfile+ dlhdr.plt_loc);
            var dlts = new List<ImageSymbol>();
            for (int i = 0; i < dlhdr.plt_count; ++i)
            {
                var addr = Address.Ptr32(exeAuxHdr.exec_dmem + ((uint) rdr.Offset - exeAuxHdr.exec_dfile));
                uint n = rdr.ReadUInt32();
                uint m = rdr.ReadUInt32();
                var name = names[i + dlhdr.dlt_count];
                var pltEntry = ImageSymbol.ExternalProcedure(arch, Address.Ptr32(n), name);
                var pltGotEntry = ImageSymbol.DataObject(arch, addr, name + "@@GOT", PrimitiveType.Ptr32);
                dlts.Add(pltEntry);
                dlts.Add(pltGotEntry);
            }
            return dlts;
        }

        private object ReadDltEntries(DlHeader dlhdr, uint uData, List<string> names)
        {
            var rdr = MakeReader(uData + dlhdr.dlt_loc);
            var dlts = new List<(string,uint)>();
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
            var result = new List<string>();
            var rdr = MakeReader(import_list_loc);
            for (; count > 0; --count)
            {
                var str = ReadString(rdr.ReadUInt32(), strTable);
                rdr.ReadUInt32();   //$TODO: do something with the flags?
                result.Add(str);
            }
            return result;
        }

        private List<(string, SpaceDictionaryRecord)> ReadSpaces(uint space_location, uint count, uint uStrings)
        {
            var spaces = new List<(string, SpaceDictionaryRecord)>();
            var rdr = new StructureReader<SpaceDictionaryRecord>(MakeReader(space_location));
            for (; count > 0; --count)
            {
                var space = rdr.Read();
                var name = ReadString(space.name, uStrings);
                spaces.Add((name, space));
            }
            return spaces;
        }

        private List<(string,SubspaceDictionaryRecord)> ReadSubspaces(uint subspace_location, uint count, uint uStrings)
        {
            var subspaces = new List<(string, SubspaceDictionaryRecord)>();
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

        private string ReadString(uint uIndex, uint uStringsOffset)
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

        private Program LoadExecSegments(BeImageReader rdr)
        {
            var segments = new List<ImageSegment>();
            var execAux = rdr.ReadStruct<SOM_Exec_aux_hdr>();

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("paRisc");

            var dlHeaderRdr = ReadDynamicLibraryInfo(execAux, arch);

            var textBytes = new byte[execAux.exec_tsize];
            var textAddr = Address.Ptr32(execAux.exec_tmem);
            Array.Copy(RawImage, (int) execAux.exec_tfile, textBytes, 0, textBytes.Length);
            var textSeg = new ImageSegment(
                ".text",
                new MemoryArea(textAddr, textBytes),
                AccessMode.ReadExecute);
            segments.Add(textSeg);

            var dataBytes = new byte[execAux.exec_dsize];
            var dataAddr = Address.Ptr32(execAux.exec_tmem);
            Array.Copy(RawImage, (int) execAux.exec_dfile, dataBytes, 0, dataBytes.Length);
            var dataSeg = new ImageSegment(
                ".data",
                new MemoryArea(dataAddr, dataBytes),
                AccessMode.ReadWrite);
            segments.Add(dataSeg);

            var segmap = new SegmentMap(
                segments.Min(s => s.Address),
                segments.ToArray());
            var platform = cfgSvc.GetEnvironment("hpux").Load(Services, arch);
            return new Program(segmap, arch, platform);
        }

    }
}
