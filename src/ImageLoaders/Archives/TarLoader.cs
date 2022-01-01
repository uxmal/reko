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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.ImageLoaders.Archives.Tar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Archives
{
    /// <summary>
    /// This class knows how to parse tar files (tarballs).
    /// </summary>
    public class TarLoader : ImageLoader
    {
        private const int TarBlockSize = 512;

        public TarLoader(IServiceProvider services, ImageLocation location, byte[]imgRaw)
            : base(services, location, imgRaw)
        {
        }


        public override ILoadedImage Load(Address? addrLoad)
        {
            var rdr = new ByteImageReader(base.RawImage);
            var archive = new TarArchive(this.ImageLocation);

            for (; ; )
            {
                var tarHeader = rdr.ReadStruct<tar_header>();
                if (tarHeader.filename.All(b => b == 0))
                    break;
                if (PeekString(rdr, "ustar"))
                {
                    var ustarHeader = rdr.ReadStruct<ustar_header>();
                    Align(rdr, TarBlockSize);

                    var filename = TarFile.GetString(tarHeader.filename);
                    var file = archive.AddFile(filename, (a, p, n) => TarFile.Load(a, p, n, tarHeader, ustarHeader, rdr));
                    
                    rdr.Offset += file.Length;
                    Align(rdr, TarBlockSize);
                }
            }
            return archive;
        }

        private void Align(ByteImageReader rdr, int alignment)
        {
            var blockOffset = (rdr.Offset + alignment - 1) / alignment;
            rdr.Offset = blockOffset * alignment;
        }

        private bool PeekString(ByteImageReader rdr, string v)
        {
            for (int i = 0; i < v.Length; ++i)
            {
                if (!rdr.TryPeekByte(i, out byte b) || (char) b != v[i])
                    return false;
            }
            return true;
        }
    }
}
