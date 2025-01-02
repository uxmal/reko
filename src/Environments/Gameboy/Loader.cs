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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;

namespace Reko.Environments.Gameboy
{
    /// <summary>
    /// Loads Gameboy ROM images.
    /// </summary>
    public class Loader : ProgramImageLoader
    {
        public Loader(IServiceProvider services, ImageLocation imageUri, byte[] imgRaw) 
            : base(services, imageUri, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr16(0); }
            set { throw new NotSupportedException(); }
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var configSvc = Services.RequireService<IConfigurationService>();
            var arch = configSvc.GetArchitecture("lr35902")!;
            var platformDef = configSvc.GetEnvironment("gameboy");
            var platform = platformDef.Load(Services, arch);
            var image = new byte[Math.Min(base.RawImage.Length, 0x1_0000)];
            Array.Copy(RawImage, 0, image, 0, image.Length);
            var mem = new ByteMemoryArea(PreferredBaseAddress, image);
            var imageSegment = new ImageSegment("ROM", mem, AccessMode.ReadExecute);
            var segmentMap = new SegmentMap(imageSegment);
            var program = new Program(new ByteProgramMemory(segmentMap), arch, platform);
            var start = ImageSymbol.Procedure(program.Architecture, Address.Ptr16(0x100), "Rom_init");
            program.EntryPoints[start.Address] = start;
            return program;
        }
    }
}
