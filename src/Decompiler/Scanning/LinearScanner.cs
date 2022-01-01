#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class LinearScanner
    {
        private readonly Program program;
        private readonly ISet<Address> symbols;

        public LinearScanner(Program program, ISet<Address> symbols)
        {
            this.program = program;
            this.symbols = symbols;
        }

        //$TODO: islands.
        public void Scan()
        {
            var items = (program.ImageMap.Items.Values.Where(i => i.DataType != null && i.DataType is UnknownType));
            {

            }
        }
    }
}
