#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.IO;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Extension methods for the <see cref="BinaryReader"/> class.
    /// </summary>
    public static class BinaryReaderExtensions
	{
        /// <summary>
        /// Reads a null-terminated ASCII string from <paramref name="rdr"/>.
        /// </summary>
        /// <param name="rdr"><see cref="BinaryReader"/> to read from.</param>
        /// <returns>A string decoded from the ASCII characters.</returns>
		public static string ReadNullTerminatedString(this BinaryReader rdr)
		{
			var sb = new StringBuilder();
            for (; ;)
            {
                int ch = rdr.ReadChar();
                if (ch <= 0)
                    return sb.ToString();
                sb.Append((char)ch);
            }
		}
	}
}
