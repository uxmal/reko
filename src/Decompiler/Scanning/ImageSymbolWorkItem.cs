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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Scanning
{
    public class ImageSymbolWorkItem : WorkItem
    {
        private IScanner scanner;
        private Program program;
        private ImageSymbol sym;
        private bool isEntryPoint;

        public ImageSymbolWorkItem(IScanner scanner, Program program, ImageSymbol sym, bool isEntryPoint) : base(sym.Address)
        {
            this.scanner = scanner;
            this.program = program;
            this.sym = sym;
            this.isEntryPoint = isEntryPoint;
        }

        public override void Process()
        {
            scanner.ScanImageSymbol(sym, isEntryPoint);
        }

        public override string ToString()
        {
            return string.Format("Symbol: {0}{1}",
                sym.Name ?? sym.Address.ToString(),
                isEntryPoint ? " entry" : "");
        }
    }
}
