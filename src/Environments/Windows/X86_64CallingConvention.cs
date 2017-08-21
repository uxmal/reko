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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Types;
using Reko.Arch.X86;
using System.Diagnostics;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Windows calling convention for x86-64.
    /// </summary>
    public class X86_64CallingConvention : CallingConvention
    {
        private static readonly RegisterStorage[] iRegs =
        {
            Registers.rcx,
            Registers.rdx,
            Registers.r8,
            Registers.r9,
        };

        private static readonly RegisterStorage[] fRegs =
        {
            Registers.xmm0,
            Registers.xmm1,
            Registers.xmm2,
            Registers.xmm3,
        };

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            Storage ret = null;
            if (dtRet != null)
            {
                if (dtRet.Size > 8)
                    throw new NotImplementedException();
                var pt = dtRet as PrimitiveType;
                if (pt != null && pt.Domain == Domain.Real)
                {
                    ret = Registers.xmm0;
                }
                else
                {
                    ret = Registers.rax;
                }
            }
            var parameters = new List<Storage>();
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dt = dtParams[i];
                if (dt.Size > 8)
                    throw new NotImplementedException();
                var pt = dt as PrimitiveType;
                if (pt != null && pt.Domain == Domain.Real && i < fRegs.Length)
                {
                    parameters.Add(fRegs[i]);
                }
                else if (i < iRegs.Length)
                {
                    parameters.Add(iRegs[i]);
                }
                else
                {
                    var stg = new StackArgumentStorage(8 * (i + 1), dt);
                    parameters.Add(stg);
                }
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null,
                Parameters = parameters,
                StackDelta = 8,
                FpuStackDelta = 0,
            };
        }
    }
}