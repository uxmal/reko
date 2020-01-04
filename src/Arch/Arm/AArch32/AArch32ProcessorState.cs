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
using Reko.Core.Machine;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Arm.AArch32
{
    public class AArch32ProcessorState : ProcessorState
    {
        private IProcessorArchitecture arch;
        private Dictionary<int, ulong> regData;
        private RegisterStorage pc;

        public AArch32ProcessorState(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regData = new Dictionary<int, ulong>();
            this.pc = Registers.pc;
        }

        public AArch32ProcessorState(AArch32ProcessorState that): base(that)
        {
            this.arch = that.arch;
            this.regData = new Dictionary<int, ulong>(that.regData);
            this.pc = that.pc;
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            var state = new AArch32ProcessorState(this);
            return state;
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (regData.TryGetValue(r.Number, out ulong uVal))
                return Constant.Create(r.DataType, uVal);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v.IsValid)
            {
                regData[r.Number] = v.ToUInt64();
            }
            else
            {
                regData.Remove(r.Number);
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            regData[pc.Number] = addr.ToUInt32();
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
