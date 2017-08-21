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

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Seralizes and deserializes MIPS signatures on Windows. 
    /// </summary>
    public class MipsCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private static string[] iregs = { "r4", "r5", "r6", "r7" };
        private static string[] fregs = { };

        public MipsCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            Storage  ret = null;

            if (dtRet != null)
            {
                ret = this.GetReturnRegister(dtRet);
            }

            int ir = 0;
            int fr = 0;
            var args = new List<Storage>();
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                Storage arg;
                var dtArg = dtParams[iArg];
                var prim = dtArg as PrimitiveType;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr >= fregs.Length)
                    {
                        arg = new StackArgumentStorage(-1, dtArg);
                    }
                    else
                    {
                        arg = arch.GetRegister(fregs[fr]);
                    }
                    ++fr;
                }
                else if (dtArg.Size <= 4)
                {
                    if (ir >= iregs.Length)
                    {
                        arg = new StackArgumentStorage(-1, dtArg);
                    }
                    else
                    {
                        arg = arch.GetRegister(iregs[ir]);
                    }
                    ++ir;
                }
                else
                {
                    int regsNeeded = (dtArg.Size + 3) / 4;
                    if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
                    {
                        arg = new StackArgumentStorage(-1, dtArg);
                    }
                    if (regsNeeded == 2)
                    {
                        arg = new SequenceStorage(
                                arch.GetRegister(iregs[ir]),
                                arch.GetRegister(iregs[ir + 1]));
                        ir += 2;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                args.Add(arg);
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null, //$TODO
                Parameters = args,
                StackDelta = 0,
                FpuStackDelta = 0,
            };
        }

        public Storage GetReturnRegister(DataType dtArg)
        {
            int bitSize = dtArg.BitSize;
            var pt = dtArg as PrimitiveType;
            if (pt != null && pt.Domain == Domain.Real)
            {
                var f0 = arch.GetRegister("f0");
                if (bitSize <= 64)
                    return f0;
                throw new NotImplementedException();
            }
            var v0 = arch.GetRegister("r2");
            if (bitSize <= 32)
                return v0;
            if (bitSize <= 64)
            {
                var v1 = arch.GetRegister("r3");
                return new SequenceStorage(v1, v0);
            }
            throw new NotImplementedException();
        }
    }
}
