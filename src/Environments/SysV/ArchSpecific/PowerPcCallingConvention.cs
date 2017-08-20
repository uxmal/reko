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
using System.Linq;
using System.Text;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class PowerPcCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;

        public PowerPcCallingConvention(IProcessorArchitecture  arch)
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
            int fr = 1;
            int gr = 3;
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dtArg = dtParams[i];
                var prim = dtArg as PrimitiveType;
                Storage param;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr > 8)
                    {
                        param = new StackArgumentStorage(-1, dtArg);
                    }
                    else
                    {
                        param = arch.GetRegister("f" + fr);
                        ++fr;
                    }
                }
                else if (dtArg.Size <= 4)
                {
                    if (gr > 10)
                    {
                        param = new StackArgumentStorage(-1, dtArg);
                    }
                    else
                    {
                        param = arch.GetRegister("r" + gr);
                        ++gr;
                    }
                }
                else if (dtArg.Size <= 8)
                {
                    if (gr > 9)
                    {
                        param = new StackArgumentStorage(-1, dtArg);
                    }
                    else
                    {
                        if ((gr & 1) == 0)
                            ++gr;
                        param = new SequenceStorage(
                            arch.GetRegister("r" + gr),
                            arch.GetRegister("r" + (gr + 1)));
                        gr += 2;
                    }
                }
                else
                    throw new NotImplementedException();
                parameters.Add(param);
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null,     //$TODO
                Parameters = parameters,
            };
        }

        public Storage GetReturnRegister(DataType dt)
        {
            var prim = dt as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                return arch.GetRegister("f1");
            }
            return arch.GetRegister("r3");
        }
    }
}
