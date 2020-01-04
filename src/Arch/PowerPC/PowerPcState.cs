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

namespace Reko.Arch.PowerPC
{
    //$TODO: for @uxmal: just implement this as a 
    // dictionary [RegisterStorage -> Constant]
    public class PowerPcState : ProcessorState
    {
        private PowerPcArchitecture arch;
        private Dictionary<RegisterStorage, Constant> regs;

        public PowerPcState(PowerPcArchitecture arch)
        {
            this.arch = arch;
            this.regs = new Dictionary<RegisterStorage, Constant>();
        }

        public PowerPcState(PowerPcState other) : base(other)
        {
            this.arch = other.arch;
            this.regs = new Dictionary<RegisterStorage, Constant>(other.regs);
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override ProcessorState Clone()
        {
            return new PowerPcState(this);
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (regs.TryGetValue(reg, out Constant value))
            {
                return value;
            }
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage reg, Constant c)
        {
            c = c ?? Constant.Invalid;
            regs[reg] = c;
        }

        public override void SetInstructionPointer(Address addr)
        {
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(0, 0);
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }
    }
}
