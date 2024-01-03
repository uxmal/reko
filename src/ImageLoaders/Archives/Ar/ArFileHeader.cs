#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Archives.Ar
{
    public class ArFileHeader
    {
        public const int Size = 60;

#nullable disable
        public string Name;                      // Member name
        public string Date;                      // Member date, seconds, decimal ASCII
        public string UserID;                    // Member User ID, decimal ASCII
        public string GroupID;                   // Member Group ID, decimal ASCII
        public string FileMode;                  // Member file mode, octal
        public string FileSize;                  // Member file size, decimal ASCII
#nullable enable

        public static ArFileHeader? Load(ByteImageReader rdr)
        {
            var header = new ArFileHeader();
            byte[] data = rdr.ReadBytes(ArFileHeader.Size);
            if (data.Length < ArFileHeader.Size)
                return null;

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
