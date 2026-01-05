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

using System;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Represents an error that occurred while parsing a C source file.
    /// </summary>
    public class CParserException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="CParserException"/> with the specified message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public CParserException(string message) : base(message)
        {
        }
    }
}
