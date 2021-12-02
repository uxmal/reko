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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Archives.Ar
{
    public class ArFile : ArchivedFile
    {
        private ArFileHeader fileHeader;
        private ByteImageReader rdr;
        private long fileDataStart;

        public ArFile(
            ArchiveDirectoryEntry? parent,
            ArFileHeader fileHeader,
            ByteImageReader rdr,
            long fileDataStart, 
            long filesize)
        {
            this.Parent = parent;
            this.fileHeader = fileHeader;
            this.rdr = rdr;
            this.fileDataStart = fileDataStart;
            this.Length = filesize;
        }

        public long Length { get; }

        public string Name => throw new NotImplementedException();

        public ArchiveDirectoryEntry? Parent { get; }

        public byte[] GetBytes()
        {
            rdr.Offset = fileDataStart;
            return rdr.ReadBytes((int)Length);
        }

        public ILoadedImage LoadImage(IServiceProvider services, Address? addr)
        {
            return new Blob(GetBytes());
        }
    }
}
