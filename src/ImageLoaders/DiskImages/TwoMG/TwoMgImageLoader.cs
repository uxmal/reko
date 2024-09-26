#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Runtime.InteropServices;

namespace Reko.ImageLoaders.DiskImages.TwoMG
{
    public class TwoMGLoader : ImageLoader
    {
        public TwoMGLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) : base(services, imageLocation, imgRaw)
        {

        }

        public override ILoadedImage Load(Address? addrLoad)
        {
                var rdr = new ByteImageReader(base.RawImage);
                var prefix = rdr.ReadStruct<Prefix>();
            switch (prefix.ImageFormat)
            {
            case 1:
                return Prodos.DiskImage.Load(base.RawImage, prefix.HeaderSize, base.ImageLocation);
            }
            return null!;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Prefix
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Magic;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Creator;

            public ushort HeaderSize;

            public ushort VersionNumber;

            public uint ImageFormat;
        }
    }
}
