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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Omf
{
    /// <summary>
    /// Loads OMF object files and libraries
    /// </summary>
    public class OmfLoader : MetadataLoader
    {
        // http://www.azillionmonkeys.com/qed/Omfg.pdf "OMF: Relocatable Object Module Format
        // http://www.bitsavers.org/pdf/intel/ISIS_II/121748-001_8086_Relocatable_Object_Module_Formats_Nov81.pdf 8086 Relocatable Object Module Formats (Intel ordr # 121748-001 )

        private byte[] rawImage;

        public OmfLoader(IServiceProvider services, string filename, byte[] rawImage) : base(services, filename, rawImage)
        {
            this.rawImage = rawImage;
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new LeImageReader(rawImage);
            var (type, _) = ReadRecord(rdr);
            if (type != RecordType.LibraryHeader)
            {
                return dstLib;
            }

            (type, _) = ReadRecord(rdr);
            if (type != RecordType.THEADR)
            {
                return dstLib;
            }
            var imp = ReadImpref(rdr);
            while (imp != null)
            {
                //var svc = new SystemService
                //{
                //    ModuleName = moduleName,
                //    Name = ep != null ? ep.Name : entryName,
                //    Signature = ep?.Signature,
                //};

                //mod.ServicesByName[sp.Name] = svc;    //$BUGBUG: catch dupes?

                //if (sp.Ordinal != Procedure_v1.NoOrdinal)
                //{
                //    mod.ServicesByOrdinal[sp.Ordinal] = svc;
                //}

                imp = ReadImpref(rdr);
            }
            return dstLib;
        }

        private SystemService ReadImpref(LeImageReader rdr)
        {
            var(type, data) = ReadRecord(rdr);
            if (type != RecordType.COMENT)
                return null;
            var rdrComent = new LeImageReader(data);
            return null;
        }

        private (RecordType, byte[]) ReadRecord(LeImageReader rdr)
        {
            if (!rdr.TryReadByte(out var type))
                throw new BadImageFormatException();
            if (!rdr.TryReadUInt16(out var length))
                throw new BadImageFormatException();
            //$PERF: use Span<T>
            var bytes = rdr.ReadBytes(length);
            return ((RecordType)type, bytes);
        }
    }
}