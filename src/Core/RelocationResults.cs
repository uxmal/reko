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

namespace Reko.Core
{
    public class RelocationResults
    {
        // Known locations in the binary, along with some
        // type information and a name.
        public readonly SortedList<Address, ImageSymbol> Symbols;

        // Known callable entry points. This is a subset
        // of the list of symbols above.
        public readonly List<ImageSymbol> EntryPoints;


        public RelocationResults(
            List<ImageSymbol> entryPoints, 
            SortedList<Address, ImageSymbol> symbols)
        {
            this.EntryPoints = entryPoints;
            this.Symbols = symbols;
        }
    }
}
