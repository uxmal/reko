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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
    public class ImageWriter
    {
        private Stream stm;

        public ImageWriter() : this(new MemoryStream())
        {
        }

        public ImageWriter(Stream stm)
        {
            this.stm = stm;
        }

        public void WriteByte(byte b)
        {
            stm.WriteByte(b);
        }

        public void WriteBytes(byte b, uint count)
        {
            while (count > 0)
            {
                stm.WriteByte(b);
                --count;
            }
        }

        public void WriteBytes(byte[] bytes)
        {
            stm.Write(bytes, 0, bytes.Length);
        }

        public void WriteString(string str, Encoding enc)
        {
            WriteBytes(enc.GetBytes(str));
        }

        public void WriteLeUint16(ushort us)
        {
            stm.WriteByte((byte) us);
            stm.WriteByte((byte) (us >> 8));
        }
    }
}
