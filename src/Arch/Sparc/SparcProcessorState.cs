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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public class SparcProcessorState : ProcessorState
    {
        private SparcArchitecture arch;
        private ulong[] regs;
        private bool[] valid;
        private uint flags;
        private uint validFlags;

        public SparcProcessorState(SparcArchitecture arch)
        {
            this.arch = arch;
            this.regs = new ulong[42];
            this.valid = new bool[42];
        }

        public SparcProcessorState(SparcProcessorState old) : base(old)
        {
            this.arch = old.arch;
            this.regs = old .regs.ToArray();
            this.valid = old.valid.ToArray();
            this.flags = old.flags;
            this.validFlags = old.validFlags;
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            return new SparcProcessorState(this);
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (Registers.IsGpRegister(reg) && valid[reg.Number])
                return Constant.Create(reg.DataType, regs[reg.Number]);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage reg, Constant v)
        {
            if (v.IsValid && Registers.IsGpRegister(reg))
            {
                valid[reg.Number] = true;
                regs[reg.Number] = v.ToUInt64();
            }
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
