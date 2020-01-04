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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class is used to find the padding between procedures that
    /// often appears between procedures in binaries. For instance, on
    /// x86 platforms we often see the byte 90 (nop), CC (int 3) or zeroes
    /// (which disassemble to x86 ADD instructions). We rely on the rewriters
    /// to set the RtlClass.Padding bit on instructions that might be used
    /// as padding.
    /// </summary>
    public class ProcedurePaddingFinder
    {
        private ScanResults sr;

        public ProcedurePaddingFinder(ScanResults sr)
        {
            this.sr = sr;
        }

        public List<RtlBlock> FindPaddingBlocks()
        {
            return sr.ICFG.Nodes
                .Where(block =>
                {
                    var iFirst = block.Instructions[0];
                    var nPred = sr.ICFG.Predecessors(block).Count;
                    return ((iFirst.Class & InstrClass.Padding) != 0) &&
                        nPred == 0;
                })
                .ToList();
        }

        public void Remove(List<RtlBlock> paddingBlocks)
        {
            foreach (var padding in paddingBlocks)
            {
                sr.ICFG.RemoveNode(padding);
            }
        }
    }
}