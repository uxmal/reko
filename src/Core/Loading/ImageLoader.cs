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
using System;
using System.Collections.Generic;

namespace Reko.Core.Loading
{
	/// <summary>
	/// Abstract base class for image loaders. These examine a raw image, and 
    /// generate a Program after carrying out relocations, resolving external
    /// symbols etc.
	/// </summary>
	public abstract class ImageLoader
	{
        public ImageLoader(IServiceProvider services, Reko.Core.RekoUri imageUri, byte[] imgRaw)
        {
            this.Services = services;
            this.ImageUri = imageUri;
            this.RawImage = imgRaw;
        }

        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// If nothing else is specified, this is the address at which the image will be loaded.
        /// </summary>
        public abstract Address PreferredBaseAddress { get; set; }

        /// <summary>
        /// Optional loader-specific argument specified in app.config.
        /// </summary>
        public string? Argument { get; set; }

        /// <summary>
        /// The image as it appears on the storage medium before being loaded.
        /// </summary>
        public byte[] RawImage { get; }

        /// <summary>
        /// The URI from which the image was loaded from.
        /// </summary>
        public RekoUri ImageUri { get; }

        /// <summary>
        /// Loads the header of the executable, so that its contents can be summarized. 
        /// </summary>
        /// <returns></returns>
        public ImageHeader LoadHeader(string argument) { throw new NotImplementedException();  }

        /// <summary>
		/// Loads the image into memory.
		/// </summary>
		/// <param name="addrLoad">Optional base address of the  image. If not specified,
        /// use the image format's default loading address. For some image types -- e.g. 
        /// archives -- the parameter is ignored.</param>
		/// <returns>An object implementing the <see cref="ILoadedImage>" /> interface.</returns>
        public abstract ILoadedImage Load(Address? addrLoad);
    }
}
