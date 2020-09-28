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
using System.Text;

namespace Reko.Core
{
    internal class CustomDecoderFallback : DecoderFallback
    {
        public override int MaxCharCount => 4;

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new CustomFallbackBuffer();
        }


        public class CustomFallbackBuffer : DecoderFallbackBuffer
        {
            private char fallback;

            public override int Remaining
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override bool Fallback(byte[] bytesUnknown, int index)
            {
                // Awful hack. We create characters in the Private Use Area (U+E000..U+E0FF)
                // and later rely on these being decoded correctly by the string serializer.
                fallback = (char)(0xE000 + bytesUnknown[0]);
                return true;
            }

            public override char GetNextChar()
            {
                if (fallback == 0)
                    return '\0';
                var f = fallback;
                fallback = '\0';
                return f;
            }

            public override bool MovePrevious()
            {
                throw new NotImplementedException();
            }
        }
    }
}