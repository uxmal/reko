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
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Environments.RT11
{
    public class Rt11CallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;

        public Rt11CallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            Storage ret = null;
            if (dtRet != null)
            {
                ret = GetReturnRegister(dtRet);
            }

            var parameters = new List<Storage>();
            int gr = 0;
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var arg = arch.GetRegister("r" + gr);
                ++gr;
                    parameters.Add(arg);
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null, //$TODO
                Parameters = parameters,
                StackDelta = 2,
            };
        }

        public Storage GetReturnRegister(DataType dtRet)
        {
            return arch.GetRegister("r0");
        }
    }
}