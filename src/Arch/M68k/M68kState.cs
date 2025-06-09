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
using Reko.Core.Operators;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.M68k
{
    public class M68kState : ProcessorState
    {
        const int RegisterCount = 32;
        private readonly M68kArchitecture arch;
        private readonly Constant[] values;
        private readonly bool[] isValid;

        public M68kState(M68kArchitecture arch)
        {
            this.arch = arch;
            this.values = new Constant[RegisterCount];
            this.isValid = new bool[RegisterCount];
        }

        public M68kState(M68kState orig) : base(orig)
        {
            this.arch = orig.arch;
            this.values = (Constant[]) orig.values.Clone();
            this.isValid = (bool[]) orig.isValid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        #region ProcessorState Members

        public override ProcessorState Clone()
        {
            return new M68kState(this);
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v is not null && v.IsValid)
            {
                values[r.Number] = v;
                isValid[r.Number] = true;
            }
            else
            {
                isValid[r.Number] = false;
            }
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (isValid[r.Number])
            {
                return values[r.Number];
            }
            else
            {
                return InvalidConstant.Create(r.DataType);
            }
        }

        public override void OnProcedureEntered(Address addr)
        {
        }

        public override void OnProcedureLeft(FunctionType sig)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnAfterCall(FunctionType? sigCallee)
        {
        }

        #endregion
    }
}
