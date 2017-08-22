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
    public class X86_64CallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs;
        private RegisterStorage al;
        private RegisterStorage ax;
        private RegisterStorage eax;
        private RegisterStorage rax;
        private RegisterStorage rdx;

        public X86_64CallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "rdi", "rsi", "rdx", "rcx", "r8", "r9" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.fregs = new []{ "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.al = arch.GetRegister("al");
            this.ax = arch.GetRegister("ax");
            this.eax = arch.GetRegister("eax");
            this.rax = arch.GetRegister("rax");
            this.rdx = arch.GetRegister("rdx");
        }

        public override ICallingConventionEmitter Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.PointerType.Size, 0x0008);
            if (dtRet != null)
            {
                ccr.Return = this.GetReturnRegister(dtRet);
            }
            int fr = 0;
            int ir = 0;
            // The SysV calling convention specifies that there is no 
            // "reserved slot" prior to the return address on the stack, in
            // contrast with Windows where 4*8 bytes are allocated for 
            // space for the four registers
            foreach (var dtParam in dtParams)
            {
                var prim = dtParam as PrimitiveType;
                if (prim != null && prim.Domain == Domain.Real)
                {
                    if (fr >= fregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                    {
                        ccr.RegParam(fregs[fr]);
                        ++fr;
                    }
                }
                else if (dtParam.Size <= 8)
                {
                    if (ir >= iregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                    {
                        ccr.RegParam(iregs[ir]);
                        ++ir;
                    }
                }
                else
                {
                    int regsNeeded = (dtParam.Size + 7) / 8;
                    if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                        throw new NotImplementedException();
                }
            }

            ccr.StackDelta = arch.PointerType.Size;
            return ccr;
        }

        public Storage GetReturnRegister(DataType dtArg)
        {
            var pt = dtArg as PrimitiveType;
            int bitSize = dtArg.BitSize;
            if (pt != null && pt.Domain == Domain.Real)
            {
                var xmm0 = fregs[0];
                if (bitSize <= 64)
                    return xmm0;
                if (bitSize <= 128)
                {
                    var xmm1 = fregs[1];
                    return new SequenceStorage(xmm1, xmm0);
                }
                throw new NotImplementedException();
            }
            if (bitSize <= 8)
                return al;
            if (bitSize <= 16)
                return ax;
            if (bitSize <= 32)
                return eax;
            if (bitSize <= 64)
                return rax;
            if (bitSize <= 128)
            {
                return new SequenceStorage(rdx, rax);
            }
            throw new NotImplementedException();
        }
    }
}
