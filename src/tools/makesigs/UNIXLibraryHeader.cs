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

namespace makesigs
{
    internal class UNIXLibraryHeader
    {
        public string Name;                      // Member name
        public string Date;                      // Member date, seconds, decimal ASCII
        public string UserID;                    // Member User ID, decimal ASCII
        public string GroupID;                   // Member Group ID, decimal ASCII
        public string FileMode;                  // Member file mode, octal
        public string FileSize;                  // Member file size, decimal ASCII

        public const int Size = 60;


        public static UNIXLibraryHeader Load(byte[] data, int offset)
        {
            var header = new UNIXLibraryHeader();
            
            header.Name = System.Text.Encoding.UTF8.GetString(data, offset, 16);
            header.Date = System.Text.Encoding.UTF8.GetString(data, offset + 16, 12);
            header.UserID = System.Text.Encoding.UTF8.GetString(data, offset + 2, 6);
            header.GroupID = System.Text.Encoding.UTF8.GetString(data, offset + 34, 6);
            header.FileMode = System.Text.Encoding.UTF8.GetString(data, offset + 40, 8);
            header.FileSize = System.Text.Encoding.UTF8.GetString(data, offset + 48, 10);

            return header;
        }
    }
}
