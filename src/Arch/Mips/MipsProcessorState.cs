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

namespace Reko.Arch.Mips
{
    public class MipsProcessorState : ProcessorState
    {
        private MipsProcessorArchitecture arch;
        private Address ip;         // Instruction pointer
        private Constant[] iregs;       // integer register values.
        private bool[] valid;       // whether the regs are valid or not.

        public MipsProcessorState(MipsProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new Constant[32];
            this.valid = new bool[32];
        }

        public MipsProcessorState(MipsProcessorState that) :base(that)
        {
            this.arch = that.arch;
            this.iregs = (Constant[])that.iregs.Clone();
            this.valid = (bool[])that.valid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            return new MipsProcessorState(this);
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            int rn = r.Number;
            if (0 <= rn && rn < 32 && valid[rn])
            {
                return iregs[rn];
            }
            else
            {
                return Constant.Invalid;
            }
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            int rn = r.Number;
            if (0 <= rn && rn < 32)
            {
                // Integer register.
                if (v.IsValid)
                {
                    iregs[rn] = v;
                    valid[rn] = true;
                    return;
                }
                else
                {
                    valid[rn] = false;
                }
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            this.ip = addr;
        }

        public override void OnProcedureEntered()
        {
            // The ELF abi states that r25 must contain the address
            // of the called function when calling PIC functions. Otherwise
            // it's a TMP register that is never used to pass arguments.
            // It should be harmless to set it to a constant value at the
            // entry of the function.
            iregs[25] = ip.ToConstant();
            SetRegister(arch.GeneralRegs[25], iregs[25]);
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
