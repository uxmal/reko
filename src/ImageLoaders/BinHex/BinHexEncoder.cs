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
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.BinHex
{
    public class BinHexEncoder
    {
        // '!', '"', '#', '$', '%', '&', '\'', '(',  
        // ')', '*', '+', ',', '-', '0', '1', '2', 
        // '3', '4', '5', '6', '8', '9', '@', 'A', 
        // 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',

        // 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R',
        // 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '[', 
        // '`', 'a', 'b', 'c', 'd', 'e', 'f', 'h',
        // 'i', 'j', 'k', 'l', 'm', 'p', 'q', 'r' 

        public const string BitEncodings = "!\"#$%&'()*+,-012345689@ABCDEFGHIJKLMNPQRSTUVXYZ[`abcdefhijklmpqr";
        public const string binhex4header = "(This file must be converted with BinHex 4.0)";

        private TextWriter writer;
        private uint leftchar = 0;
        private int leftbits = 0;

        public BinHexEncoder(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Encode(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                Encode(bytes[i]);
            }
        }

        public void Encode(byte b)
        {
            // Shift into our buffer, and output any 6 bits ready 
            leftchar = (leftchar << 8) | b;
            leftbits += 8;
            while (leftbits >= 6)
            {
                uint this_ch = (leftchar >> (leftbits - 6)) & 0x3f;
                leftbits -= 6;
                writer.Write(BitEncodings[(int) this_ch]);
            }
        }

        public void Flush()
        {
            // Output a possible runt byte
            if (leftbits != 0)
            {
                leftchar <<= (6 - leftbits);
                writer.Write(BitEncodings[(int) (leftchar & 0x3F)]);
            }
        }

        public void WriteHeader()
        {
            writer.Write(binhex4header);
            writer.Write(":");
        }
    }
}
