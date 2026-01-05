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

using Reko.Core;
using Reko.Core.Loading;

namespace Reko.Scanning
{
    /// <summary>
    /// Work item to process an <see cref="ImageSymbol"/>.
    /// </summary>
    public class ImageSymbolWorkItem : WorkItem
    {
        private readonly IScanner scanner;
        private readonly ImageSymbol sym;
        private readonly bool isEntryPoint;

        /// <summary>
        /// Constructs an instance of <see cref="ImageSymbolWorkItem"/>.
        /// </summary>
        /// <param name="scanner"><see cref="IScanner"/> that is orchestrating this
        /// workitem.</param>
        /// <param name="sym">Image symbol to process.</param>
        /// <param name="isEntryPoint">True if the symbol is the entry point of the
        /// program image; otherwise false.
        /// </param>
        public ImageSymbolWorkItem(IScanner scanner, ImageSymbol sym, bool isEntryPoint) 
            : base(sym.Address!)
        {
            this.scanner = scanner;
            this.sym = sym;
            this.isEntryPoint = isEntryPoint;
        }

        /// <inheritdoc/>
        public override void Process()
        {
            scanner.ScanImageSymbol(sym, isEntryPoint);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("Symbol: {0}{1}",
                sym.Name ?? sym.Address!.ToString(),
                isEntryPoint ? " entry" : "");
        }
    }
}
