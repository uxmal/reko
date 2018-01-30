#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Scanning
{
    public class ProcedurePaddingFinder
    {
        private ScanResults sr;

        public ProcedurePaddingFinder(ScanResults sr)
        {
            this.sr = sr;
        }

        public IEnumerable<RtlBlock> FindPaddingBlocks()
        {
            return sr.ICFG.Nodes
                .Where(block =>
                {
                    var iFirst = block.Instructions[0];
                    return ((iFirst.Class & RtlClass.Padding) != 0) &&
                        sr.ICFG.Predecessors(block).Count == 0;
                });
        }

        public void Remove(object pads)
        {
        }
    }
}