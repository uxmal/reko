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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Types;
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

        public OmfLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawImage)
            : base(services, imageLocation, rawImage)
        {
            this.rawImage = rawImage;
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var loader = new TypeLibraryDeserializer(platform, true, dstLib);
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
            for (; ;)
            {
                byte[]? data;
                (type, data) = ReadRecord(rdr);
                if (data is null)
                    break;
                switch (type)
                {
                default: throw new NotImplementedException($"OMF record type {type} ({(int) type:X} has not been implemented yet.");
                case RecordType.THEADR:
                    // Can't seem to do anything useful with THEADRs
                    break;
                case RecordType.COMENT:
                    var rdrComent = new LeImageReader(data);
                    if (!rdrComent.TryReadByte(out byte _)) // Ignore the comment type
                        break;
                    if (!rdrComent.TryReadByte(out byte cmtClass))
                        break;
                    if ((CommentClass) cmtClass != CommentClass.Extensions)
                        break;
                    if (!rdrComent.TryReadByte(out byte cmtExt))
                        break;
                    if ((CommentExtension) cmtExt == CommentExtension.IMPDEF)
                    {
                        ReadImpdef(rdrComent, loader);
                    }
                    else
                    {
                        throw new NotImplementedException($"OMF COMENT extension {(CommentExtension) cmtExt} (0x{cmtExt:X}) is not implemented yet.");
                    }
                    break;
                case RecordType.MODEND:
                    // Modend'Name seem to be followed by padding to a 16-byte boundary.
                    while ((rdr.Offset & 0xF) != 0 && rdr.TryReadByte(out _))
                        ;
                    break;
                case RecordType.LibraryEnd:
                    return dstLib;
                }
            }
            return dstLib;
        }

        private SystemService? ReadImpdef(LeImageReader rdr, TypeLibraryDeserializer loader)
        {
            if (!rdr.TryReadByte(out byte useOrdinal))
                return null;
            var internalName = ReadString(rdr);
            if (internalName is null)
                return null;
            var moduleName = ReadString(rdr);
            if (moduleName is null)
                return null;
            if (useOrdinal != 0)
            {
                if (!rdr.TryReadLeInt16(out var ordinal))
                    return null;
                var svc = new SystemService
                {
                    ModuleName = moduleName,
                    Name = internalName,
                    SyscallInfo = new SyscallInfo
                    {
                        Vector = ordinal
                    }
                };
                loader.LoadService(ordinal, svc);
                return svc;
            }
            else
            {
                throw new NotImplementedException("non-ordinals");
            }
        }

        private string ReadString(LeImageReader rdr)
        {
            var cStr = rdr.ReadLengthPrefixedString(
                PrimitiveType.Byte,
                PrimitiveType.Char,
                Encoding.ASCII);
            return cStr!.ToString();
        }

        private (RecordType, byte[]?) ReadRecord(LeImageReader rdr)
        {
            if (!rdr.TryReadByte(out var type))
                return (0, null);
            if (!rdr.TryReadUInt16(out var length))
                throw new BadImageFormatException();
            //$PERF: use Span<T>
            var bytes = rdr.ReadBytes(length);
            return ((RecordType)type, bytes);
        }
    }
}