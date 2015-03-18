#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcState : ProcessorState
    {
        private PowerPcArchitecture arch;
        private ulong[] regs;
        private bool[] valid;

        public PowerPcState(PowerPcArchitecture arch)
        {
            this.arch = arch;
            this.regs = new ulong[0x50];
            this.valid = new bool[0x50];
        }

        public PowerPcState(PowerPcState other)
        {
            this.arch = other.arch;
            this.regs = new ulong[other.regs.Length];
            this.valid = new bool[other.valid.Length];
            for (int i = 0; i < other.regs.Length; ++i)
            {
                this.regs[i] = other.regs[i];
                this.valid[i] = other.valid[i];
            }
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
            if (valid[reg.Number])
                return Constant.Create(reg.DataType, regs[reg.Number]);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage reg, Constant c)
        {
            if (c == null || !c.IsValid)
            {
                valid[reg.Number] = false;
            }
            else
            {
                reg.SetRegisterFileValues(regs, c.ToUInt32(), valid);	//$REVIEW: AsUint64 for PPC-64?
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(ProcedureSignature procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(0, 0);
        }

        public override void OnAfterCall(Identifier stackReg, ProcedureSignature sigCallee, ExpressionVisitor<Expression> eval)
        {
        }
    }
}
