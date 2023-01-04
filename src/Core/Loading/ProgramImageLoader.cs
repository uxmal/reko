#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Core.Loading
{
    /// <summary>
    /// Abstract base class for image loaders that load <see cref="Program"/>s.
    /// These examine a raw image, and generate a Program instance after carrying out
    /// relocations, resolving external symbols etc.
    /// </summary>
    public abstract class ProgramImageLoader : ImageLoader
    {
        public ProgramImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
            : base(services, imageLocation, imgRaw)
        {
        }

        /// <summary>
        /// If nothing else is specified, this is the address at which the image will be loaded.
        /// </summary>
        public abstract Address PreferredBaseAddress { get; set; }

        public override ILoadedImage Load(Address? addrLoad)
        {
            return LoadProgram(addrLoad);
        }

        /// <summary>
        /// Loads a program image into memory.
        /// </summary>
        /// <param name="addrLoad">Optional base address of the program image. If not specified,
        /// use the image format's default loading address.</param>
        /// <returns>An object implementing the <see cref="ILoadedImage>" /> interface.</returns>
        public abstract Program LoadProgram(Address? address);
        
        /// <summary>
        /// Loads the image into memory at the specified address, using the 
        /// provided IProcessorArchitecture and IPlatform. Used when loading
        /// raw files; not all image loaders can support this.
        /// </summary>
        /// <param name="addrLoad"></param>
        /// <param name="arch"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public virtual Program LoadProgram(
            //$REVIEW: this is just LoadDetails all over again
            Address addrLoad,
            IProcessorArchitecture arch,
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            throw new NotSupportedException(
                $"Image loader {GetType().FullName} doesn't support overriding the processor architecture or platform.");
        }

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
        public virtual ImageSegment? AddSegmentReference(Address addr, ushort seg)
        {
            return null;
        }
    }
}
