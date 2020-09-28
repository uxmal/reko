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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tms7000
{
    public class Tms7000State : ProcessorState
    {
        private Tms7000Architecture arch;
        private Dictionary<RegisterStorage, Constant> regs;

        public Tms7000State(Tms7000Architecture arch)
        {
            this.arch = arch;
            this.regs = new Dictionary<RegisterStorage, Constant>();
        }

        public Tms7000State(Tms7000State that)
        {
            this.arch = that.arch;
            this.regs = new Dictionary<RegisterStorage, Constant>(that.regs);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override ProcessorState Clone()
        {
            return new Tms7000State(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (!regs.TryGetValue(r, out var c))
                c = Constant.Invalid;
            return c;
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        public override void SetInstructionPointer(Address addr)
        {
            throw new NotImplementedException();
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            throw new NotImplementedException();
        }
    }
}
