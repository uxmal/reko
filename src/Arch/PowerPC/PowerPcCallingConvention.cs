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
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs;

        public PowerPcCallingConvention(PowerPcArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "r3", "r4", "r5", "r6", "r7", "r8", "r9", "r10" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.fregs = new[] { "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            //$TODO: verify the stack offset
            ccr.LowLevelDetails(arch.WordWidth.Size, 0x40);
            if (dtRet != null)
            {
                ccr.Return = this.GetReturnRegister(dtRet);
            }

            int gr = 0;
            int fr = 0; 
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtArg = dtParams[iArg];
                var prim = dtArg as PrimitiveType;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr >= fregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(fregs[fr]);
                        ++fr;
                    }
                }
                else if (dtArg.Size <= 4)
                {
                    if (gr >= iregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(iregs[gr]);
                        ++gr;
                    }
                }
                else if (dtArg.Size <= 8)
                {
                    if (gr >= iregs.Length-1)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        if ((gr & 1) == 1)
                            ++gr;
                        ccr.SequenceParam(iregs[gr], iregs[gr + 1]);
                        gr += 2;
                    }
                }
                else 
                    throw new NotImplementedException();
            }
        }

        public Storage GetReturnRegister(DataType dt)
        {
            var prim = dt as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                return fregs[0];
            }
            return iregs[0];
        }
    }
}
