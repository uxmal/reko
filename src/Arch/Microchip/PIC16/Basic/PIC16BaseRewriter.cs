#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public class PIC16BaseRewriter : PIC16Rewriter
    {

        private PIC16BaseRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public static PIC16BaseRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            if (arch is null)
                throw new ArgumentNullException(nameof(arch));
            if (dasm is null)
                throw new ArgumentNullException(nameof(dasm));
            if (state is null)
                throw new ArgumentNullException(nameof(state));
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));
            if (host is null)
                throw new ArgumentNullException(nameof(host));
            return new PIC16BaseRewriter(arch, dasm, state, binder, host);
        }

        protected override void RewriteInstr()
        {
            base.RewriteInstr();
        }


    }

}
