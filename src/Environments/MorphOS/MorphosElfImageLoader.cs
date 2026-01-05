#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.ImageLoaders.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.MorphOS
{
    /// <summary>
    /// For some reason, the MorphOS project chose to wrap the ELF image file format
    /// rather than to use it directly.
    /// </summary>
    public class MorphosElfImageLoader : ProgramImageLoader
    {
        private ElfImageLoader elfLdr;

        public MorphosElfImageLoader(IServiceProvider services, ImageLocation imageUri, byte[] rawBytes) 
            : base(services, imageUri, rawBytes)
        {
            this.elfLdr = null!;
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x0010_0000);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Program LoadProgram(Address? addrLoad, string? sPlatformOverride)
        {
            var sr = new StructureReader<MorphosHeader>(new BeImageReader(this.RawImage));
            var hdr = sr.Read();
            var embeddedElfImage = new byte[this.RawImage.Length - hdr.ElfOffset];
            //$PERF: this is a prime candidate for Span<T>
            Array.Copy(this.RawImage, hdr.ElfOffset, embeddedElfImage, 0, embeddedElfImage.Length);
            this.elfLdr = new ElfImageLoader(this.Services, this.ImageLocation, embeddedElfImage);
            var program = elfLdr.LoadProgram(addrLoad, sPlatformOverride);
            return program;
        }

        private MorphosHeader LoadMorphosElfHeader()
        {
            throw new NotImplementedException();
        }
    }
}
