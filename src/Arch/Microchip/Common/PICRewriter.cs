#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// A PIC IL rewriter abstract class.
    /// </summary>
    public abstract class PICRewriter : IEnumerable<RtlInstructionCluster>
    {
        protected PICArchitecture arch;
        protected PICDisassemblerBase disasm;
        protected PICProcessorState state;
        protected IStorageBinder binder;
        protected IRewriterHost host;

        protected IEnumerator<PICInstruction> dasm;
        protected PICInstruction instrCurr;
        protected RtlClass rtlc;
        protected List<RtlInstruction> rtlInstructions;
        protected RtlEmitter m;
        protected Identifier Wreg;    // cached WREG register identifier

        protected PICRewriter(PICArchitecture arch, PICDisassemblerBase disasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.disasm = disasm;
            dasm = disasm.GetEnumerator();
            Wreg = GetWReg;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCurr = dasm.Current;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);

                RewriteInstr();

                yield return new RtlInstructionCluster(instrCurr.Address, instrCurr.Length, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Actual instruction rewriter method must be implemented by derived classes.
        /// </summary>
        protected abstract void RewriteInstr();

        /// <summary>
        /// Gets the working register (WREG) which is often used implicitly by PIC instructions.
        /// </summary>
        protected abstract Identifier GetWReg { get; }

    }

}
