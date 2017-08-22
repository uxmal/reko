#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public class ThisCallConvention : CallingConvention
    {
        private RegisterStorage ecxThis;
        private int retAddressOnStack;
        private int stackAlignment;

        public ThisCallConvention(RegisterStorage ecxThis, int stackAlignment, int retAddressOnStack)
        {
            this.ecxThis = ecxThis;
            this.stackAlignment = stackAlignment;
            this.retAddressOnStack = retAddressOnStack;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            var ccr = new CallingConventionResult(stackAlignment, retAddressOnStack);
            ccr.Return = X86CallingConvention.GetReturnStorage(dtRet);
            var fpuStackDelta = ccr.Return is FpuStackStorage ? 1 : 0;

            for (int i = 0; i < dtParams.Count; ++i)
            {
                ccr.Push(dtParams[i]);
            }

            ccr.ImplicitThis = this.ecxThis;
            ccr.FpuStackDelta = fpuStackDelta;
            ccr.StackDelta = ccr.stackOffset;
            return ccr;
        }
    }
}
