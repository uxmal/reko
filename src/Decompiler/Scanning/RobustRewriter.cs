#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Rewriter that yields invalid instructions when encountered, rather
    /// than throwing.
    /// </summary>
    public class RobustRewriter : IEnumerable<RtlInstructionCluster>
    {
        private IEnumerable<RtlInstructionCluster> inner;
        private int granularity;

        public RobustRewriter(IEnumerable<RtlInstructionCluster> inner, int granularity)
        {
            this.inner = inner;
            this.granularity = granularity;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var e = inner.GetEnumerator();
            for (; ; )
            {
                bool cont = false;
                RtlInstructionCluster rtl = null;
                try
                {
                    cont = e.MoveNext();
                    if (e != null)
                        rtl = e.Current;
                }
                catch (AddressCorrelatedException aex)
                {
                    cont = false;
                    rtl = new RtlInstructionCluster(aex.Address, granularity,
                        new RtlInvalid());
                    rtl.Class = RtlClass.Invalid;
                }
                if (rtl != null)
                    yield return rtl;
                if (!cont)
                    yield break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
