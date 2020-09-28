#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        private RegisterStorage[] fregs;
        private RegisterStorage[] iregs;

        public PowerPcCallingConvention(IProcessorArchitecture  arch)
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
            int stackOffset = 0x40; //$BUG: look this up!
            ccr.LowLevelDetails(arch.WordWidth.Size, stackOffset);
            if (dtRet != null)
            {
                SetReturnRegister(ccr, dtRet);
            }

            int fr = 0;
            int gr = 0;
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dtArg = dtParams[i];
                var prim = dtArg as PrimitiveType;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr > 8)
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
                    if (gr >= iregs.Length - 1)
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

        public void SetReturnRegister(ICallingConventionEmitter ccr, DataType dt)
        {
            var prim = dt as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                ccr.RegReturn(fregs[0]);
            }
            else
            {
                ccr.RegReturn(iregs[0]);
            }
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg) || fregs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            return iregs[0] == stg || fregs[0] == stg;
        }
    }
}
