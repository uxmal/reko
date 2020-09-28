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

namespace Reko.Arch.M6800.M6812
{
    public class M6812State : ProcessorState
    {
        public M6812State(M6812Architecture arch)
        {
            this.Architecture = arch;
        }

        public M6812State(M6812State that)
        {
            this.Architecture = that.Architecture;
        }

        public override IProcessorArchitecture Architecture { get; }

        public override ProcessorState Clone()
        {
            return new M6812State(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            return Constant.Invalid;
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
            throw new NotImplementedException();
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            throw new NotImplementedException();
        }

        public override void OnProcedureEntered()
        {
            throw new NotImplementedException();
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
            throw new NotImplementedException();
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
