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

using System;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Msp430
{
    public class Msp430State : ProcessorState
    {
        private Msp430Architecture arch;
        private int[] regs;
        private bool[] valid;

        public Msp430State(Msp430Architecture arch)
        {
            this.arch = arch;
            this.regs = new int[16];
            this.valid = new bool[16];
        }

        public Msp430State(Msp430State that) : base(that)
        {
            this.arch = that.arch;
        }
        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override ProcessorState Clone()
        {
            return new Msp430State(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            return Constant.Invalid;
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(2, 0);
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
            
        }
    }
}