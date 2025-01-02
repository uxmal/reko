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
using System.Threading.Tasks;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750State : ProcessorState
    {
        private readonly MilStd1750Architecture arch;
        private readonly Dictionary<RegisterStorage, Constant> regValues;

        public MilStd1750State(MilStd1750Architecture arch, Dictionary<RegisterStorage, Constant>? regValues = null)
        {
            this.arch = arch;
            this.regValues = regValues ?? new Dictionary<RegisterStorage, Constant>();
            
        }

        public override IProcessorArchitecture Architecture => arch;

        public override ProcessorState Clone()
        {
            return new MilStd1750State(this.arch, new Dictionary<RegisterStorage, Constant>(this.regValues));
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (!regValues.TryGetValue(r, out Constant? value))
            {
                value = Constant.Zero(r.DataType);
            }
            return value;
        }

        public override void OnAfterCall(FunctionType? sigCallee)
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

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            regValues[r] = v;
        }
    }
}
