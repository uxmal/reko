#region License
/* 
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Machine;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Arm
{
    public class ArmProcessorState : ProcessorState
    {
        private IProcessorArchitecture arch;
        private uint isValid;
        private ulong[] regData;
        private RegisterStorage pc;

        public ArmProcessorState(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regData = new ulong[48];
            this.pc = arch.GetRegister("pc");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            var state = new ArmProcessorState(arch);
            state.isValid = this.isValid;
            state.regData = (ulong[])regData.Clone();
            return state;
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (((isValid >> r.Number) & 1) != 0)
                return Constant.Create(r.DataType, regData[r.Number]);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v.IsValid)
            {
                isValid |= 1u << r.Number;
                regData[r.Number] = v.ToUInt32();
            }
            else
            {
                isValid &= ~(1u << r.Number);
                regData[r.Number] = 0xCCCCCCCC;
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            regData[pc.Number] = addr.ToUInt32();
            isValid |= 1u << pc.Number;
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
