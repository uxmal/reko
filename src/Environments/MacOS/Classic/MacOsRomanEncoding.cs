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

namespace Reko.Environments.MacOS.Classic
{
    public class MacOsRomanEncoding : Encoding
    {
        private static char[] macToUnicode = new char[]
        {
            '\x0000',
            '\x0001',
            '\x0002',
            '\x0003',
            '\x0004',
            '\x0005',
            '\x0006',
            '\x0007',
            '\x0008',
            '\x0009',
            '\x000A',
            '\x000B',
            '\x000C',
            '\x000D',
            '\x000E',
            '\x000F',

            '\x0010',
            '\x0011', // Command key ⌘: U+2318 
            '\x0012', // Shift key 
            '\x0013', // Option key
            '\x0014', // Control key 
            '\x0015',
            '\x0016',
            '\x0017',
            '\x0018',
            '\x0019',
            '\x001A',
            '\x001B',
            '\x001C',
            '\x001D',
            '\x001E',
            '\x001F',

            ' ',
            '!',
            '"',
            '#',
            '$',
            '%',
            '&',
            '\'',
            '(',
            ')',
            '*',
            '+',
            ',',
            '-',
            '.',
            '/',

            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            ':',
            ';',
            '<',
            '=',
            '>',
            '?',

            '@',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',

            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '[',
            '\\',
            ']',
            '^',
            '_',

            '`',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',

            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '{',
            '|',
            '}',
            '~',
            '\x007F',

            '\x00C4',
            '\x00C5',
            '\x00C7',
            '\x00C9',
            '\x00D1',
            '\x00D6',
            '\x00DC',
            '\x00E1',
            '\x00E0',
            '\x00E2',
            '\x00E4',
            '\x00E3',
            '\x00E5',
            '\x00E7',
            '\x00E9',
            '\x00E8',

            '\x00EA',
            '\x00EB',
            '\x00ED',
            '\x00EC',
            '\x00EE',
            '\x00EF',
            '\x00F1',
            '\x00F3',
            '\x00F2',
            '\x00F4',
            '\x00F6',
            '\x00F5',
            '\x00FA',
            '\x00F9',
            '\x00FB',
            '\x00FC',

            '\x2020',
            '\x00B0',
            '\x00A2',
            '\x00A3',
            '\x00A7',
            '\x2022',
            '\x00B6',
            '\x00DF',
            '\x00AE',
            '\x00A9',
            '\x2122',
            '\x00B4',
            '\x00A8',
            '\x2260',
            '\x00C6',
            '\x00D8',

            '\x221E',
            '\x00B1',
            '\x2264',
            '\x2265',
            '\x00A5',
            '\x00B5',
            '\x2202',
            '\x2211',
            '\x220F',
            '\x03C0',
            '\x222B',
            '\x00AA',
            '\x00BA',
            '\x03A9',
            '\x00E6',
            '\x00F8',

            '\x00BF',
            '\x00A1',
            '\x00AC',
            '\x221A',
            '\x0192',
            '\x2248',
            '\x2206',
            '\x00AB',
            '\x00BB',
            '\x2026',
            '\x00A0',
            '\x00C0',
            '\x00C3',
            '\x00D5',
            '\x0152',
            '\x0153',

            '\x2013',
            '\x2014',
            '\x201C',
            '\x201D',
            '\x2018',
            '\x2019',
            '\x00F7',
            '\x25CA',
            '\x00FF',
            '\x0178',
            '\x2044',
            '\x20AC',
            '\x2039',
            '\x203A',
            '\xFB01',
            '\xFB02',

            '\x2021',
            '\x00B7',
            '\x201A',
            '\x201E',
            '\x2030',
            '\x00C2',
            '\x00CA',
            '\x00C1',
            '\x00CB',
            '\x00C8',
            '\x00CD',
            '\x00CE',
            '\x00CF',
            '\x00CC',
            '\x00D3',
            '\x00D4',

            '\xF8FF', // The Apple logo -- Unicode private area
            '\x00D2',
            '\x00DA',
            '\x00DB',
            '\x00D9',
            '\x0131',
            '\x02C6',
            '\x02DC',
            '\x00AF',
            '\x02D8',
            '\x02D9',
            '\x02DA',
            '\x00B8',
            '\x02DD',
            '\x02DB',
            '\x02C7',
        };

        private static Dictionary<char,byte> unicodeToMac;

        static MacOsRomanEncoding()
        {
            unicodeToMac = new Dictionary<char,byte>();
            for (int i = 0; i < macToUnicode.Length; ++i)
            {
                unicodeToMac[macToUnicode[i]] = (byte)i;
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
                if (unicodeToMac.TryGetValue(chars[iSrc], out b))
                    bytes[iDst] = b;
                else 
                    bytes[iDst] = 0x0F;     //?
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
                char c = macToUnicode[bytes[iSrc]];
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
