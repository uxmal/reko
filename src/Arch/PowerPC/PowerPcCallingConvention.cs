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

namespace Reko.Arch.PowerPC
{
    public class PowerPcCallingConvention : CallingConvention
    {
        private PowerPcArchitecture arch;

        public PowerPcCallingConvention(PowerPcArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            Storage ret = null;

            if (dtRet != null)
            {
                ret = this.GetReturnRegister(dtRet);
            }

            var parameters = new List<Storage>();
            int gr = 3;
            int fr = 1;
            int stackOffset = 0x40; //$BUG: look this up!
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtArg = dtParams[iArg];
                var prim = dtArg as PrimitiveType;
                Storage arg;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr > 8)
                    {
                        arg = new StackArgumentStorage(stackOffset, dtArg);
                    }
                    else
                    {
                        arg = arch.GetRegister("f" + fr);
                        ++fr;
                    }
                } else if (dtArg.Size <= 4)
                {
                    if (gr > 10)
                    {
                        arg = new StackArgumentStorage(stackOffset, dtArg);
                    }
                    else
                    {
                        arg = arch.GetRegister("r" + gr);
                        ++gr;
                    }
                }
                else if (dtArg.Size <= 8)
                {
                    if (gr > 9)
                    {
                        arg = new StackArgumentStorage(stackOffset, dtArg);
                    }
                    else
                    {
                        if ((gr & 1) == 0)
                            ++gr;
                        arg = new SequenceStorage(
                            arch.GetRegister("r" + gr),
                            arch.GetRegister("r" + (gr + 1)));
                        gr += 2;
                    }
                }
                else 
                    throw new NotImplementedException();

                parameters.Add(arg);
            }


            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null, //$TODO
                Parameters = parameters,
                StackDelta = 0,
                FpuStackDelta = 0
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
