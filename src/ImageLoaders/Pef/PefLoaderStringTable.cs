#region License
/* 
 * Copyright (C) 2018-2021 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PefLoaderStringTable
    {
        private readonly PEFLoaderInfoHeader infoHeader;
        private readonly EndianByteImageReader rdr;

        public PefLoaderStringTable(PEFLoaderInfoHeader infoHeader, EndianByteImageReader rdr)
        {
            this.infoHeader = infoHeader;
            this.rdr = rdr;
        }

        public string ReadString(uint offset, uint length = 0)
        {
            return rdr.ReadAt(infoHeader.loaderStringsOffset + offset, (_) =>
            {
                if(length == 0) return rdr.ReadCString(PrimitiveType.Char, Encoding.ASCII).ToString();
                
                var bytes = rdr.ReadBytes(length);
                return Encoding.ASCII.GetString(bytes);
            });
        }
    }
}
