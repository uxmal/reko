#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.Zilog.Z80
{
    public class Z80ProcessorState : ProcessorState
    {
        const int RegisterFileItems = 32;

        private Z80Architecture arch;
        private ushort[] registerFile;
        private bool[] isValid;

        public Z80ProcessorState(Z80Architecture arch)
        {
            this.arch = arch;
            this.registerFile = new ushort[RegisterFileItems];
            this.isValid = new bool[RegisterFileItems];
        }

        public Z80ProcessorState(Z80ProcessorState state)
            : base(state)
        {
            this.arch = state.arch;
            this.registerFile = (ushort[])state.registerFile.Clone();
            this.isValid = (bool[])state.isValid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override ProcessorState Clone()
        {
            return new Z80ProcessorState(this);
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (isValid[(int) reg.Domain])
                return Constant.Create(reg.DataType, registerFile[(int) reg.Domain]);
            else
                return InvalidConstant.Create(reg.DataType);
        }

        public override void SetRegister(RegisterStorage reg, Constant v)
        {
            if (v is not null && v.IsValid)
            {
                isValid[(int)reg.Domain] = true;
                registerFile[(int)reg.Domain] = v.ToByte();
            }
            else
            {
                isValid[(int)reg.Domain] = false;
            }
        }

        public override void OnProcedureEntered(Address addr)
        {
        }

        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnAfterCall(FunctionType? sigCallee)
        {
        }
    }
}
