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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core.Types;

namespace Reko.Arch.Pdp11
{
    public class Pdp11ProcessorState : ProcessorState
    {
        private Pdp11Architecture arch;
        private ushort[] regs;
        private bool[] valid;

        public Pdp11ProcessorState(Pdp11Architecture arch)
        {
            this.arch = arch;
            this.regs = new ushort[22];
            this.valid = new bool[22];
        }

        public Pdp11ProcessorState(Pdp11ProcessorState that) : base(that)
        {
            this.arch = that.arch;
            this.regs = (ushort[])that.regs.Clone();
            this.valid = (bool[])that.valid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }
        
        public override ProcessorState Clone()
        {
            return new Pdp11ProcessorState(this);
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v.IsValid)
            {
                regs[r.Number] = v.ToUInt16();
                valid[r.Number] = true;
            }
            else
            {
                valid[r.Number] = false;
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            regs[Registers.pc.Number] = addr.ToUInt16();
            valid[Registers.pc.Number] = true;
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (valid[r.Number])
            {
                return Constant.Word16(regs[r.Number]);
            }
            else
            {
                return Constant.Invalid;
            }
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnSize)
        {
            return new CallSite(returnSize, 0);
        }

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }
    }
}
