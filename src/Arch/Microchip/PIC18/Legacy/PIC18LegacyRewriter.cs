#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    public class PIC18LegacyRewriter : PIC18RewriterBase
    {
        private PIC18LegacyRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public static PICRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PIC18LegacyRewriter(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                dasm ?? throw new ArgumentNullException(nameof(dasm)),
                state ?? throw new ArgumentNullException(nameof(state)),
                binder ?? throw new ArgumentNullException(nameof(binder)),
                host ?? throw new ArgumentNullException(nameof(host))
              );
        }

        protected override void RewriteInstr()
        {
            base.RewriteInstr();
        }

    }

}
