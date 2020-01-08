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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Services
{
    public interface IUnpackerService
    {
        /// <summary>
        /// Given an original <see cref="ImageLoader"/> and the offset into
        /// the image where the main execution point is, searches the signature database
        /// to find an unpacker that matches.
        /// </summary>
        /// <param name="imageLoader">The image loader that contains the raw bytes of the image.</param>
        /// <param name="entryPointOffset">The offset from the beginning of the image</param>
        /// <returns>An image loader that can unpack the image, or the original
        /// ImageLoader if no unpacker could be found.</returns>
        ImageLoader FindUnpackerBySignature(ImageLoader imageLoader, uint entryPointOffset);
    }
}
