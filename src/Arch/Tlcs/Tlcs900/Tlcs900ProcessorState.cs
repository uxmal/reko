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

namespace Reko.Arch.Tlcs
{
    public class Tlcs900ProcessorState : ProcessorState
    {
        private Tlcs900Architecture arch;
        private uint[] regs;
        private bool[] valid;

        public Tlcs900ProcessorState(Tlcs900Architecture arch)
        {
            this.arch = arch;
            this.regs = new uint[32];
            this.valid = new bool[32];
        }

        public Tlcs900ProcessorState(Tlcs900ProcessorState that) : base(that)
        {
            this.arch = that.arch;
            this.regs = (uint[])that.regs.Clone();
            this.valid = (bool[])that.valid.Clone();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override ProcessorState Clone()
        {
            var that = new Tlcs900ProcessorState(this);
            return that;
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (valid[r.Number])
                return Constant.Create(r.DataType, regs[r.Number]);
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

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v.IsValid)
            {
                valid[r.Number] = true;
                regs[r.Number] = v.ToUInt32();
            }
            else
            {
                valid[(int)r.Number] = false;
                regs[r.Number] = 0xDDDDDDDD;
            }
        }
    }
}
