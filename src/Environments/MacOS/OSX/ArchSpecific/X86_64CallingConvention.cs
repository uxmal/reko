#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.MacOS.OSX.ArchSpecific
{
    public class X86_64CallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs;
        private RegisterStorage al;
        private RegisterStorage ax;
        private RegisterStorage eax;
        private RegisterStorage rax;
        private RegisterStorage rdx;

        public X86_64CallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.iregs = new[] { "rdi", "rsi", "rdx", "rcx", "r8", "r9" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.fregs = new[] { "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.al = arch.GetRegister("al")!;
            this.ax = arch.GetRegister("ax")!;
            this.eax = arch.GetRegister("eax")!;
            this.rax = arch.GetRegister("rax")!;
            this.rdx = arch.GetRegister("rdx")!;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.PointerType.Size, 0x0008);
            if (dtRet is not null)
            {
                SetReturnRegister(ccr, dtRet);
            }
            int fr = 0;
            int ir = 0;
            // The SysV calling convention specifies that there is no 
            // "reserved slot" prior to the return address on the stack, in
            // contrast with Windows where 4*8 bytes are allocated for 
            // space for the four registers
            foreach (var dtParam in dtParams)
            {
                if (dtParam.Domain == Domain.Real)
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
            ccr.CallerCleanup(arch.PointerType.Size);
        }

        public void SetReturnRegister(ICallingConventionBuilder ccr, DataType dtArg)
        {
            int bitSize = dtArg.BitSize;
            if (dtArg.Domain == Domain.Real)
            {
                var xmm0 = fregs[0];
                if (bitSize <= 64)
                {
                    ccr.RegReturn(xmm0);
                    return;
                }
                if (bitSize <= 128)
                {
                    var xmm1 = fregs[1];
                    ccr.SequenceReturn(xmm1, xmm0);
                    return;
                }
                throw new NotImplementedException();
            }
            if (bitSize <= 8)
            {
                ccr.RegReturn(al);
                return;
            }
            if (bitSize <= 16)
            {
                ccr.RegReturn(ax);
                return;
            }
            if (bitSize <= 32)
            {
                ccr.RegReturn(eax);
                return;
            }
            if (bitSize <= 64)
            {
                ccr.RegReturn(rax);
                return;
            }
            if (bitSize <= 128)
            {
                ccr.SequenceReturn(rdx, rax);
                return;
            }
            throw new NotImplementedException();
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg) || fregs.Contains(reg);
            }
            return stg is StackStorage;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return
                    reg.Domain == rax.Domain ||
                    reg.Domain == rdx.Domain ||
                    reg.Domain == fregs[0].Domain ||
                    reg.Domain == fregs[1].Domain;
            }
            return false;
        }
    }
}
