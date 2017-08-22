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

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Seralizes and deserializes MIPS signatures on Windows. 
    /// </summary>
    // https://msdn.microsoft.com/en-us/library/aa448706.aspx
    public class MipsCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs = { };
        private RegisterStorage iretLo;
        private RegisterStorage iretHi;
        private RegisterStorage fret;

        public MipsCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "r4", "r5", "r6", "r7" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.fregs = new[] { "f12", "f13", "f14", "f15" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.iretLo = arch.GetRegister("r2");
            this.iretHi = arch.GetRegister("r3");
            this.fret = arch.GetRegister("f0");
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            var ccr = new CallingConventionResult();
            ccr.LowLevelDetails(arch.WordWidth.Size, 0x10);
            if (dtRet != null)
            {
                ccr.Return = this.GetReturnRegister(dtRet);
            }

            int ir = 0;
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
                    if (ir >= iregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(iregs[ir]);
                        ++ir;
                    }
                }
                else
                {
                    int regsNeeded = (dtArg.Size + 3) / 4;
                    if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    if (regsNeeded == 2)
                    {
                        ccr.SequenceParam(iregs[ir], iregs[ir + 1]);
                        ir += 2;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            return ccr;
        }

        public Storage GetReturnRegister(DataType dtArg)
        {
            int bitSize = dtArg.BitSize;
            var pt = dtArg as PrimitiveType;
            if (pt != null && pt.Domain == Domain.Real)
            {
                if (bitSize <= 64)
                    return fret;
                throw new NotImplementedException();
            }
            if (bitSize <= 32)
                return iretLo;
            if (bitSize <= 64)
            {
                return new SequenceStorage(iretHi, iretLo);
            }
            throw new NotImplementedException();
        }
    }
}
