#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.SegaGenesis
{
    public class RomLoader : ImageLoader
    {
        public RomLoader(IServiceProvider services, string filename, byte[] imgRaw) 
            : base(services, filename, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            if (RawImage.Length <= 200)
                throw new BadImageFormatException("The file is too small for a Sega Genesis ROM image.");
            var image = new LoadedImage(addrLoad, RawImage);
            var cfgService = Services.RequireService<IConfigurationService>();
            var arch = cfgService.GetArchitecture("m68k");
            var env = cfgService.GetEnvironment("sega-genesis");
            var platform = env.Load(Services, arch);

            return new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = arch,
                Platform = platform,
            };
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            // Get the Reset address from offset $0004 of the interrupt vector.
            var addrReset = Address.Ptr32(program.Image.ReadLeUInt32(4));
            var eps = new List<EntryPoint>();
            if (program.Image.IsValidAddress(addrReset))
            {
                eps.Add(new EntryPoint(addrReset, "Reset", program.Architecture.CreateProcessorState()));
            }
            return new RelocationResults(eps, new RelocationDictionary(), new List<Address>());
        }
    }
}
