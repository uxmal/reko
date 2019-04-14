#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Ar
{
    /// <summary>
    /// This class knows how to load an "ar" static library.
    /// </summary>
    public class ArLoader : ImageLoader
    {
        // https://en.wikipedia.org/wiki/Ar_(Unix)

        List<ArPackage> packages;

        public ArLoader(IServiceProvider services, string filename, byte[] rawBytes)
           : base(services, filename, rawBytes)
        {
            // Create the list of Ar packages
            packages = new List<ArPackage>();

            // Create an Image reader
            LeImageReader rdr = new LeImageReader(RawImage);

            // Loop through the archive file and create Archive packages
            while (rdr.Offset + ArHeader.Size < rdr.Bytes.Length)
            {
                ArPackage pack = ArPackage.Load(rdr);
                if (pack == null)
                {
                    return;
                }
                packages.Add(pack);

                // Round up to align by 2
                rdr.Offset = rdr.Offset + (rdr.Offset % 2);
            }
        }

        public int NumberArPackages
        {
            get
            {
                return packages.Count;
            }
        }


        public ArPackage GetPackage(int index)
        {
            if (index < 0 || index >= packages.Count)
            {
                return null;
            }
            return packages[index];
        }

        public override Address PreferredBaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
