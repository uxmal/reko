#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core.Configuration;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Loading
{
	/// <summary>
	/// Loads an executable image into memory, and deduces the processor architecture 
	/// from the image contents.
	/// </summary>
	public class Loader : LoaderBase
	{
        public Loader(IServiceProvider services) : base(services)
        {
        }

        /// <summary>
        /// Loads the image into memory, unpacking it if necessary. Then, relocate the image.
        /// Relocation gives us a chance to determine the addresses of interesting items.
        /// </summary>
        /// <param name="rawBytes">Image of the executeable file.</param>
        /// <param name="addrLoad">Address into which to load the file.</param>
        public override Program Load(string fileName, byte[] image, Address addrLoad)
        {
            ImageLoader imgLoader = FindImageLoader<ImageLoader>(fileName, image, () => new NullLoader(Services, image));
            if (addrLoad == null)
			{
				addrLoad = imgLoader.PreferredBaseAddress;     //$REVIEW: Should be a configuration property.
			}

            var result = imgLoader.Load(addrLoad);
            Program program = new Program(
                result.Image,
                result.ImageMap,
                result.Architecture,
                result.Platform);
            program.Name = Path.GetFileName(fileName);
		    var relocations = imgLoader.Relocate(addrLoad);
            EntryPoints.AddRange(relocations.EntryPoints);
            CopyImportThunks(imgLoader.ImportThunks, program);
            return program;
        }
    }
}
