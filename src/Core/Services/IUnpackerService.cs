#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        /// Given a filename, the contents of the file it refers to, and the offset into
        /// the image where the main execution point is, searches the signature database
        /// to find an unpacker that matches.
        /// </summary>
        /// <param name="filename">The name of the file that is to be analyzed</param>
        /// <param name="image">The contents of the file as an array of bytes</param>
        /// <param name="entryPointOffset">The offset from the beginning of the image</param>
        /// <returns>An image loader that can unpack the image, or null if no
        /// unpacker could be found.</returns>
        ImageLoader FindUnpackerBySignature(string filename, byte[] image, uint entryPointOffset);
    }
}
