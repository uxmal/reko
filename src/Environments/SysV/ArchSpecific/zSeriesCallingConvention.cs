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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class zSeriesCallingConvention : CallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;
        private readonly RegisterStorage[] fregs;

        public zSeriesCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "r2", "r3", "r4", "r5", "r6" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.fregs = new[] { "f0", "f2", "f4", "f6" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            int fr = 0;
            int gr = 0;
            ccr.LowLevelDetails(8, 160);
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dt = dtParams[i];
                switch (dt)
                {
                case PrimitiveType pt:
                    if (pt.Domain == Domain.Real)
                    {
                        if (fr < this.fregs.Length)
                        {
                            ccr.RegParam(fregs[fr]);
                            ++fr;
                        }
                        else
                        {
                            ccr.StackParam(dt);
                        }
                        break;
                    }
                    goto default;
                default:
                    if (dt.BitSize <= 64)
                    {
                        if (gr < this.iregs.Length)
                        {
                            ccr.RegParam(iregs[gr]);
                            ++gr;
                            break;
                        }
                    }
                    ccr.StackParam(dt);
                    break;
                }
            }
            if (dtRet is PrimitiveType ptRet && 
                ptRet.Domain == Domain.Real)
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
            if (stg is RegisterStorage reg)
            {
                return reg == iregs[0] || reg == fregs[0];
            }
            return false;
        }
    }
}
