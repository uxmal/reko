/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
	/// <summary>
	/// Abstract base class for image loaders. These examine a raw image, and generate a new, relocated image.
	/// </summary>
	public abstract class ImageLoader
	{
        private IServiceProvider services;
		private byte [] imgRaw;

        public ImageLoader(IServiceProvider services, byte[] imgRaw)
        {
            this.services = services;
            this.imgRaw = imgRaw;
        }

        public abstract IProcessorArchitecture Architecture { get; }

        public T GetService<T>()
        {
            return (T)services.GetService(typeof(T));
        }

        public virtual Dictionary<uint, PseudoProcedure> ImportThunks
        {
            get { return null; }
        }

        /// <summary>
		/// Loads the image into memory starting at the specified address
		/// </summary>
		/// <param name="addrLoad">Base address of program image</param>
		/// <returns></returns>
        public abstract ProgramImage Load(Address addrLoad);

        public abstract Platform Platform { get; }

		public abstract Address PreferredBaseAddress
		{
			get; 
		}

		public byte [] RawImage
		{
			get { return imgRaw; }
		}

		/// <summary>
		/// Performs fix-ups of the loaded image, adding findings to the supplied collections.
		/// </summary>
		/// <param name="addrLoad">The address at which the program image is loaded.</param>
		/// <param name="entryPoints">Collection into which any found entry points found should be added.</param>
		public abstract void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations);

    }
}
