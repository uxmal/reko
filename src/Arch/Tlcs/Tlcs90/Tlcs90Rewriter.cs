#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Reko.Core;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public class Tlcs90Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private Frame frame;
        private IRewriterHost host;
        private EndianImageReader rdr;
        private ProcessorState state;
        private Tlcs90Architecture tlcs90Architecture;

        public Tlcs90Rewriter(Tlcs90Architecture tlcs90Architecture, EndianImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.tlcs90Architecture = tlcs90Architecture;
            this.rdr = rdr;
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
