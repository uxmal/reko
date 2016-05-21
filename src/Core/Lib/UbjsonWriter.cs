#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Lib
{
    public class UbjsonWriter
    {
        private Stream stm;

        public UbjsonWriter(Stream stm)
        {
            this.stm = stm;
        }

        public void Save(object o)
        {
            if (o == null)
            {
                stm.WriteByte((byte)UbjsonMarker.Null);
                return;
            }
            var s = o as string;
            if (s != null)
            {
                stm.WriteByte((byte)UbjsonMarker.String);
                var enc = Encoding.UTF8.GetByteCount(s);
                WriteLength(enc);
                var tx = new StreamWriter(stm, Encoding.UTF8);
                tx.Write(s);
                tx.Flush();
                return;
            }
            throw new NotImplementedException();
        }

        private void WriteLength(long len)
        {
            if (len < 256)
            {
                stm.WriteByte((byte)UbjsonMarker.Int8);
                stm.WriteByte((byte)len);
                return;
            }
            if (len < 65536)
            {
                stm.WriteByte((byte)UbjsonMarker.Int16);
                stm.WriteByte((byte)(len >> 8));
                stm.WriteByte((byte)len);
                return;
            }
            if (len < (1L << 32))
            {
                stm.WriteByte((byte)UbjsonMarker.Int32);
                stm.WriteByte((byte)(len >> 24));
                stm.WriteByte((byte)(len >> 16));
                stm.WriteByte((byte)(len >> 8));
                stm.WriteByte((byte)len);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
