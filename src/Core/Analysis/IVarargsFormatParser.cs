#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
 
using System.Collections.Generic;
using Reko.Core.Types;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// Interface implemented by classes that parse format strings used in 
    /// variadic functions.
    /// </summary>
    public interface IVarargsFormatParser
    {
        /// <summary>
        /// The list of argument types extracted from the format string.
        /// </summary>
        List<DataType> ArgumentTypes { get; }

        /// <summary>
        /// Parses the varargs format string and populates the <see cref="ArgumentTypes"/>
        /// </summary>
        void Parse();
    }
}
