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

using Reko.Core;
using Reko.ImageLoaders.Hunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Hunk
{
    public class HunkMaker
    {
        private readonly static Encoding enc = Encoding.GetEncoding("ISO_8859-1");
        private readonly static Dictionary<Type, Action<object, BeImageWriter>> dispatcher =
            new Dictionary<Type, Action<object, BeImageWriter>>
        {
            { typeof(Int32), (o, w) => { w.WriteBeUInt32((uint) (Int32) o); } },
            { typeof(UInt32), (o, w) => { w.WriteBeUInt32((UInt32) o); } },
            { typeof(UInt16), (o, w) => { w.WriteBeUInt16((ushort) o); } },
            { typeof(HunkType), (o, w) => { w.WriteBeUInt32((uint) (HunkType) o); } },
            { typeof(string), (o, w) => { WriteString((string) o, w); } },
        };

        public BeImageReader MakeImageReader(params object[] data)
        {
            return new BeImageReader(MakeBytes(data), 0);
        }

        public byte[] MakeBytes(params object[] data)
        {
            var w = new BeImageWriter();
            foreach (var o in data)
            {
                dispatcher[o.GetType()](o, w);
            }
            var bytes = w.Bytes.Take((int)w.Position).ToArray();
            return bytes;
        }

        private static void WriteString(string s, BeImageWriter w)
        {
            if (s.Length <= 0)
            {
                w.WriteBeUInt32(0);
                return;
            }
            byte[] ab = enc.GetBytes(s);
            int padLength = (ab.Length + 3) & ~3;
            w.WriteBeUInt32((uint) padLength / 4);
            w.WriteBytes(ab);
            int cPad = padLength - ab.Length;
            while (--cPad >= 0)
            {
                w.WriteByte(0);
            }
        }
    }
}