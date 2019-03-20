#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.HpSom
{
    public class HpSomLoader : ImageLoader
    {
        public HpSomLoader(IServiceProvider services, string filename, byte[] imgRaw) :
            base(services, filename, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0x00100000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var rdr = new BeImageReader(RawImage);
            var ext = new StructureReader<SOM_Header>(rdr);
            var somHeader = ext.Read();

            if (somHeader.aux_header_location == 0)
                throw new BadImageFormatException();

            rdr = new BeImageReader(RawImage, somHeader.aux_header_location);
            var auxReader = new StructureReader<aux_id>(rdr);
            var aux = auxReader.Read();
            switch (aux.type)
            {
            case aux_id_type.exec_aux_header:
                return LoadExecSegments(rdr);
            default:
                throw new BadImageFormatException();
            }
        }

        private Program LoadExecSegments(BeImageReader rdr)
        {
            var segments = new List<ImageSegment>();
            var execAuxRdr = new StructureReader<SOM_Exec_aux_hdr>(rdr);
            var execAux = execAuxRdr.Read();

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
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("paRisc");
            var platform = cfgSvc.GetEnvironment("hpux").Load(Services, arch);
            return new Program(segmap, arch, platform);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                new SortedList<Address, ImageSymbol>());
        }
    }
}
