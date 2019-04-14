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
    // https://en.wikipedia.org/wiki/Ar_(Unix)

    public class ArHeader
    {
        public string Name;                      // Member name
        public string Date;                      // Member date, seconds, decimal ASCII
        public string UserID;                    // Member User ID, decimal ASCII
        public string GroupID;                   // Member Group ID, decimal ASCII
        public string FileMode;                  // Member file mode, octal
        public string FileSize;                  // Member file size, decimal ASCII

        public const int Size = 60;


        public static ArHeader Load(LeImageReader rdr)
        {
            var header = new ArHeader();
            byte[] data = new byte[60];

            Buffer.BlockCopy(rdr.Bytes, (int) rdr.Offset, data, 0, ArHeader.Size);

            rdr.Offset += ArHeader.Size;
            header.Name = Encoding.UTF8.GetString(data, 0, 16);
            header.Date = Encoding.UTF8.GetString(data, 16, 12);
            header.UserID = Encoding.UTF8.GetString(data, 28, 6);
            header.GroupID = Encoding.UTF8.GetString(data, 34, 6);
            header.FileMode = Encoding.UTF8.GetString(data, 40, 8);
            header.FileSize = Encoding.UTF8.GetString(data, 48, 10);

            return header;
        }
    }
}
