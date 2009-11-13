/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
namespace Decompiler.Core
{
	/// <summary>
	/// Rewrites code from machine-specific to machine-independent IL codes.
	/// </summary>
	public abstract class Rewriter
	{
        private IProcessorArchitecture arch;
        private IProcedureRewriter prw;

        public Rewriter(IProcessorArchitecture arch, IProcedureRewriter prw)
        {
            this.arch = arch;
            this.prw = prw;
        }

        public IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public IProcedureRewriter ProcedureRewriter
        {
            get { return prw; }
        }

        public abstract void ConvertInstructions(MachineInstruction [] instrs, Address [] addrs, uint [] deadOutFlags,  Address addrEnd, CodeEmitter emitter);

		public abstract void EmitCallAndReturn(Procedure callee);

        public virtual void RewriteInstructions(Address addr, int length, Block block)
        { 
        }
    }
}
