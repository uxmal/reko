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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    class Pdp11ProcessorState : ProcessorState
    {
        private Pdp11Architecture arch;

        public Pdp11ProcessorState(Pdp11Architecture arch)
        {
            this.arch = arch;
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }
        
        public override ProcessorState Clone()
        {
            throw new NotImplementedException();
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            throw new NotImplementedException();
        }

        public override void SetInstructionPointer(Address addr)
        {
            throw new NotImplementedException();
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            throw new NotImplementedException();
        }

        public override void OnProcedureEntered()
        {
            throw new NotImplementedException();
        }

        public override void OnProcedureLeft(ProcedureSignature procedureSignature)
        {
            throw new NotImplementedException();
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnSize)
        {
            return new CallSite(returnSize, 0);
        }

        public override void OnAfterCall(ProcedureSignature sigCallee)
        {
        }
    }
}
