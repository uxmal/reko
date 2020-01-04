#region License
/* 
 * Copyright (C) 2018-2020 Stefano Moioli <smxdev4@gmail.com>.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.Xex.Enums;
using static Reko.ImageLoaders.Xex.Structures;

namespace Reko.ImageLoaders.Xex
{
    public class FileFormat
    {
        internal XEXEncryptionType encryption_type;
        internal XEXCompressionType compression_type;
        internal List<XexFileBasicCompressionBlock> basic_blocks;
        internal XexFileNormalCompressionInfo normal;
    }

    public class OptionalHeader
    {
        private XexOptionalHeader hdr;
        internal XEXHeaderKeys key => hdr.key;
        internal UInt32 offset {
            get {
                return hdr.offset;
            }
            set {
                hdr.offset = value;
            }
        }
        internal UInt32 value;
        internal UInt32 length;

        internal OptionalHeader(XexOptionalHeader hdr)
        {
            this.hdr = hdr;
        }
    }

    public class LoaderInfo
	{
		public byte[] aes_key;
	}

    public class ImageData
    {
        internal XexHeader header;

        internal XEXSystemFlags system_flags;
        internal XexExecutionInfo execution_info;
        internal XexGameRating game_ratings = new XexGameRating();
        internal XexTlsInfo tls_info;

        internal List<XexResourceInfo> resources;
        internal uint exe_address;
        internal uint exe_entry_point;
        internal uint exe_stack_size;
        internal uint exe_heap_size;
        internal FileFormat file_format_info = new FileFormat();

        internal List<string> libNames = new List<string>();
        internal List<UInt32> import_records = new List<uint>();
        internal LoaderInfo loader_info = new LoaderInfo();
        internal List<XexSection> sections;
        internal byte[] session_key;

        internal byte[] memoryData;
        internal uint memorySize;
        internal PEOptHeader peHeader;
    }
}
