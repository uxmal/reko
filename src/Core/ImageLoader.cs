#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core
{
	/// <summary>
	/// Abstract base class for image loaders. These examine a raw image, and 
    /// generate a Program after carrying out relocations, resolving external
    /// symbols etc.
	/// </summary>
	public abstract class ImageLoader
	{
        public ImageLoader(IServiceProvider services, string filename, byte[] imgRaw)
        {
            this.Services = services;
            this.Filename = filename;
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
        public string Argument { get; set; }

        /// <summary>
        /// The image as it appears on the storage medium before being loaded.
        /// </summary>
        public byte[] RawImage { get; private set; }

        /// <summary>
        /// The name of the file the image was loaded from.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Loads the header of the executable, so that its contents can be summarized. 
        /// </summary>
        /// <returns></returns>
        public ImageHeader LoadHeader(string argument) { throw new NotImplementedException();  }

        /// <summary>
		/// Loads the image into memory starting at the specified address
		/// </summary>
		/// <param name="addrLoad">Base address of program image</param>
		/// <returns></returns>
        public abstract Program Load(Address addrLoad);

        /// <summary>
        /// Loads the image into memory at the specified address, using the 
        /// provided IProcessorArchitecture and IPlatform. Used when loading
        /// raw files; not all image loaders can support this.
        /// </summary>
        /// <param name="addrLoad"></param>
        /// <param name="arch"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public virtual Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            throw new NotSupportedException(
                string.Format(
                    "Image loader {0} doesn't support overriding the processor architecture or platform.",
                    GetType().FullName));
        }

        /// <summary>
        /// Performs fix-ups of the loaded image, adding findings to the supplied collections.
        /// </summary>
        /// <param name="addrLoad">The address at which the program image is loaded.</param>
        /// <returns></returns>
		public abstract RelocationResults Relocate(Program program, Address addrLoad);

        /// <summary>
        /// Express the fact that memory at address <paramref name="addr"/> was relocated
        /// to segment <paramref>seg</paramref>.
        /// </summary>
        /// <param name="addr">The address of the relocation.</param>
        /// <param name="seg">The relocated segment reference.</param>
        /// <remarks>
        /// This method only makes sense for image formats that support
        /// x86-style segments.
        /// </remarks>
        public virtual ImageSegment AddSegmentReference(Address addr, ushort seg)
        {
            return null;
    }
    }
}
