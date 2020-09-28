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

namespace Reko.Arch.i8051
{
    public class i8051State : ProcessorState
    {
        private i8051Architecture arch;
        private Dictionary<RegisterStorage, Constant> regValues;

        public i8051State(i8051Architecture arch)
        {
            this.arch = arch;
            this.regValues = new Dictionary<RegisterStorage, Constant>();
        }

        public i8051State(i8051State that) : base(that)
        {
            this.arch = that.arch;
            this.regValues = new Dictionary<RegisterStorage, Constant>(that.regValues);
        }

        public override IProcessorArchitecture Architecture {  get { return arch; } }

        public override ProcessorState Clone()
        {
            return new i8051State(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (regValues.TryGetValue(r, out var v))
                return v;
            else
                return Constant.Invalid;
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
            regValues[Registers.PC] = Constant.Word16(addr.ToUInt16());
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v.IsValid)
                this.regValues[r] = v;
            else
                this.regValues.Remove(r);
        }
    }
}
