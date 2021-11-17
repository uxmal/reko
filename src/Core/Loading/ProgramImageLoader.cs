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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Abstract base class for image loaders that load <see cref="Program"/>s.
    /// These examine a raw image, and 
    /// generate a Program after carrying out relocations, resolving external
    /// symbols etc.
    /// </summary>
    public abstract class ProgramImageLoader : ImageLoader
    {
        public ProgramImageLoader(IServiceProvider services, string filename, byte[] imgRaw) 
            : base(services, filename, imgRaw)
        {
        }

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
    }
}
