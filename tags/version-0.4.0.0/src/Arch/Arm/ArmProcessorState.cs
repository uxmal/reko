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
using Decompiler.Core.Types;
using Decompiler.Core.Machine;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ArmProcessorState : ProcessorState
    {
        private ArmProcessorArchitecture arch;
        private uint isValid;
        private uint[] regData;

        public ArmProcessorState(ArmProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regData = new uint[16];
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            var state = new ArmProcessorState(arch);
            state.isValid = this.isValid;
            state.regData = (uint[])regData.Clone();
            return state;
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (((isValid >> r.Number) & 1) != 0)
                return Constant.Word32(regData[r.Number]);
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
            regData[A32Registers.pc.Number] = addr.Linear;
            isValid |= 1u << A32Registers.pc.Number;
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
