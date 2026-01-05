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

using Reko.Core.Configuration;
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.Core.Services
{
    /// <summary>
    /// Provides a service for loading symbols from a symbol source.
    /// </summary>
    public interface ISymbolLoadingService
    {
        /// <summary>
        /// Given a filename, returns the symbol source that is responsible for
        /// extracting symbols from that file.
        /// </summary>
        /// <param name="filename">Path to a file that may be associated with symbols.
        /// </param>
        /// <returns>A <see cref="ISymbolSource"/> implementation if symbols were foind;
        /// otherwise null.</returns>
        ISymbolSource? GetSymbolSource(string filename);

        /// <summary>
        /// Gets a list of available symbol sources.
        /// </summary>
        /// <returns>A list of available symbol sources.
        /// </returns>
        List<SymbolSourceDefinition> GetSymbolSources();
    }
}
