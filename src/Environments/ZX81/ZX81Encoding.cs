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
using System.Linq;
using System.Text;

namespace Reko.Environments.ZX81
{
    /// <summary>
    /// Implements the Sinclair-defined text encoding, which has no relationship
    /// at all to ASCII.
    /// </summary>
    public class ZX81Encoding : Encoding
    {
        /*
+---------------------------------------------------------------------------+
|   |   0    |   1    |   2    |   3    |   4    |   5    |   6    |   7    |
|   +-----------------------------------------------------------------------+
| 0 |space   |        |        |        |        |        |        |        |
| 1 |(       |)       |>       |<       |=       |+       |-       |*       |
| 2 |4       |5       |6       |7       |8       |9       |A       |B       |
| 3 |K       |L       |M       |N       |O       |P       |Q       |R       |
| 4 |RND     |PI      |INKEY$  |~~      |~~      |~~      |~~      |~~      |
| 5 |~~      |~~      |~~      |~~      |~~      |~~      |~~      |~~      |
| 6 |~~      |~~      |~~      |~~      |~~      |~~      |~~      |~~      |
| 7 |up      |down    |left    |right   |GRAPHICS|EDIT    |NEWLINE |RUBOUT  |
| 8 |        |        |        |        |        |        |        |        |
| 9 |[(]     |[)]     |[>]     |[<]     |[=]     |[+]     |[-]     |[*]     |
| A |[4]     |[5]     |[6]     |[7]     |[8]     |[9]     |[A]     |[B]     |
| B |[K]     |[L]     |[M]     |[N]     |[O]     |[P]     |[Q]     |[R]     |
| C |""      |AT      |TAB     |~~      |CODE    |VAL     |LEN     |SIN     |
| D |SQR     |SGN     |ABS     |PEEK    |USR     |STR$    |CHR$    |NOT     |
| E |STEP    |LPRINT  |LLIST   |STOP    |SLOW    |FAST    |NEW     |SCROLL  |
| F |LIST    |LET     |PAUSE   |NEXT    |POKE    |PRINT   |PLOT    |RUN     |
+---+-----------------------------------------------------------------------+
|   |   8    |   9    |   A    |   B    |   C    |   D    |   E    |   F    |
|   +-----------------------------------------------------------------------+
| 0 |        |        |        |"       |£       |$       |:       |?       |
| 1 |/       |;       |,       |.       |0       |1       |2       |3       |
| 2 |C       |D       |E       |F       |G       |H       |I       |J       |
| 3 |S       |T       |U       |V       |W       |X       |Y       |Z       |
| 4 |~~      |~~      |~~      |~~      |~~      |~~      |~~      |~~      |
| 5 |~~      |~~      |~~      |~~      |~~      |~~      |~~      |~~      |
| 6 |~~      |~~      |~~      |~~      |~~      |~~      |~~      |~~      |
| 7 |[K]/[L] |FUNCTION|~~      |~~      |~~      |~~      |number  |cursor  |
| 8 |        |        |        |["]     |[£]     |[$]     |[:]     |[?]     |
| 9 |[/]     |[;]     |[,]     |[.]     |[0]     |[1]     |[2]     |[3]     |
| A |[C]     |[D]     |[E]     |[F]     |[G]     |[H]     |[I]     |[J]     |
| B |[S]     |[T]     |[U]     |[V]     |[W]     |[X]     |[Y]     |[Z]     |
| C |COS     |TAN     |ASN     |ACS     |ATN     |LN      |EXP     |INT     |
| D |**      |OR      |AND     |<=      |>=      |<>      |THEN    |TO      |
| E |CONT    |DIM     |REM     |FOR     |GOTO    |GOSUB   |INPUT   |LOAD    |
| F |SAVE    |RAND    |IF      |CLS     |UNPLOT  |CLEAR   |RETURN  |COPY    |
+---+-----------------------------------------------------------------------+
 */
        private static char[] asciiToZx;

        // '@' used as 'illegal' value since ZX81 apparently had no '@' in its character set.
        private static char[] zxToUnicode = new char[]
        {
            ' ', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '"',  '£', '$', ':', '?',  
            '(', ')', '>', '<',  '=', '+', '-', '*',  '/', ';', ',', '.',  '0', '1', '2', '3',  
            '4', '5', '6', '7',  '8', '9', 'A', 'B',  'C', 'D', 'E', 'F',  'G', 'H', 'I', 'J',  
            'K', 'L', 'M', 'N',  'O', 'P', 'Q', 'R',  'S', 'T', 'U', 'V',  'W', 'X', 'Y', 'Z',  

            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '\n', '\x7F','@', '@', '@', '@',  '@', '@', '@', '@',  

            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  

            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
            '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  '@', '@', '@', '@',  
        };

        static ZX81Encoding()
        {
            asciiToZx = Enumerable.Repeat('@', 256).ToArray();
            for (int iZx = 0; iZx < zxToUnicode.Length; ++iZx)
            {
                char zxChar = zxToUnicode[iZx];
                if (zxChar != '@')
                    asciiToZx[zxChar] = (char) iZx;
            }
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            int iEnd = charIndex + charCount;
            int iDst = byteIndex;
            for (int iSrc = charIndex; iSrc < iEnd; ++iSrc, ++iDst)
            {
                byte b;
                char c = chars[iSrc];
                if (c >= 0x256)
                    b = (byte) '@';
                else
                {
                    b = (byte) asciiToZx[c];
                }
                if (b == (byte) '@')
                    b = 0x0F;
                bytes[iDst] = b;
            }
            return iDst - byteIndex;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int iEnd = byteIndex + byteCount;
            int iDst = charIndex;
            for (int iSrc = byteIndex; iSrc < iEnd; ++iSrc, ++iDst)
            {
                char c = zxToUnicode[bytes[iSrc]];
                if (c == '@')
                    c = '?';
                chars[iDst] = c;
            }
            return iDst - charIndex;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}
