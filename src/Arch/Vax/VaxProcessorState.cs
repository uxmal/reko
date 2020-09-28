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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Arch.Vax
{
    public class VaxProcessorState : ProcessorState
    {
        private VaxArchitecture arch;
        private uint [] regs = new uint[32];
        private bool [] isValid = new bool [32];

        public VaxProcessorState(VaxArchitecture arch)
        {
            Debug.Assert(arch != null);
            this.arch = arch;
        }

        public VaxProcessorState(VaxProcessorState that) : base(that)
        {
            this.arch = that.arch;
            this.regs = (uint[])that.regs.Clone();
            this.isValid = (bool[])that.isValid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            return new VaxProcessorState(this);
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (reg != null && isValid[(int)reg.Domain])
                return Constant.Create(reg.DataType, regs[(int)reg.Domain]);
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
        }

        public override void SetRegister(RegisterStorage reg, Constant v)
        {
            if (reg != null && v != null && v.IsValid)
            {
                isValid[(int)reg.Domain] = true;
                regs[(int)reg.Domain] = v.ToByte();
            }
            else
            {
                isValid[(int)reg.Domain] = false;
            }
        }
    }
}
