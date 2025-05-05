#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Types
{
    /// <summary>
    /// Type qualifiers.
    /// </summary>
    [Flags]
    public enum Qualifier
    {
        /// <summary>
        /// No type qualifier
        /// </summary>
        None = 0,

        /// <summary>
        /// Type is a constant value.
        /// </summary>
        Const = 1,

        /// <summary>
        /// Type is a volatile value.
        /// </summary>
        Volatile = 2,

        /// <summary>
        /// Type is a restrict value, and doesn't alias any other value.
        /// </summary>
        Restricted = 4, // C99
    }
}
