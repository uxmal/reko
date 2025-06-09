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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Arm.AArch64
{
    // https://docs.microsoft.com/en-us/cpp/build/arm64-windows-abi-conventions?view=msvc-170
    // Seems identical to the official ARM 64 ABI.
    public class AArch64CallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] argRegs;
        private readonly RegisterStorage[] floatRegs;

        public AArch64CallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            argRegs = new[] { "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7" }
                .Select(r => arch.GetRegister(r)!).ToArray();
            floatRegs = new[] { "q0", "q1", "q2", "q3", "q4", "q5", "q6", "q7" }
                .Select(r => arch.GetRegister(r)!).ToArray();
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(8, 0x0040);

            int iReg = 0;
            int iFloat = 0;
            int iStackOffset = 0;

            var adjusted = PrepadExtendParameters(dtParams);
            foreach (var (dom, bs) in adjusted)
            {
                var byteSize = bs;
                if (dom == Domain.Real)
                {
                    if (dom == Domain.Real && iFloat < floatRegs.Length)
                    {
                        ccr.RegParam(floatRegs[iFloat]);
                        ++iFloat;
                    }
                    else
                    {
                        // HFA / HVA's not supported yet.
                        if (byteSize >= 8)
                        {
                            iStackOffset = AlignUp(iStackOffset, Math.Max(8, byteSize));
                        }
                        if (byteSize < 8)
                        {
                            byteSize = 8;
                        }
                        ccr.StackParam(PrimitiveType.CreateWord(byteSize * 8));
                    }
                }
                else
                {
                    if (byteSize <= 8 && iReg < argRegs.Length)
                    {
                        ccr.RegParam(argRegs[iReg]);
                        ++iReg;
                    }
                    else if (byteSize == 16 && iReg < argRegs.Length - 1 && (iReg & 1) == 1)
                    {
                        ++iReg;
                        if (iReg < argRegs.Length - 1)
                        {
                            ccr.SequenceParam(argRegs[iReg], argRegs[iReg + 1]);
                            iReg += 2;
                        }
                    }
                    else if (byteSize <= (8 - iReg) * 8)
                    {
                        throw new NotImplementedException("Need to allow arbitrary sequences of regs");
                    }
                    else
                    {
                        iReg = 8;
                        if (byteSize >= 8)
                        {
                            iStackOffset = AlignUp(iStackOffset, Math.Max(8, byteSize));
                        }
                        if (byteSize < 8)
                        {
                            byteSize = 8;
                        }
                        ccr.StackParam(PrimitiveType.CreateWord(byteSize * 8));
                    }
                }
            }

            if (dtRet is not null)
            {
                if (dtRet.Domain == Domain.Real)
                {
                    if (dtRet.BitSize < 64)
                    {
                        ccr.RegReturn(floatRegs[0]);
                    }
                    else if (dtRet.BitSize < 128)
                    {
                        ccr.SequenceReturn(floatRegs[1], floatRegs[0]);
                    }
                    else
                        throw new NotImplementedException();
                }
                else
                {
                    if (dtRet.BitSize <= 64)
                    {
                        ccr.RegReturn(argRegs[0]);
                    }
                    else if (dtRet.BitSize <= 128)
                    {
                        ccr.SequenceReturn(argRegs[1], argRegs[0]);
                    }
                    else
                        throw new NotImplementedException();
                }
            }
        }

        private static int AlignUp(int n, int alignment)
        {
            return (n + (alignment - 1)) / alignment;
        }

        private (Domain, int)[] PrepadExtendParameters(List<DataType> dtParams)
        {
            var result = new (Domain, int)[dtParams.Count];
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dt = dtParams[i];
                // Replace large arguments with pointers
                if (dt.Size > 16)
                    result[i] = (Domain.Pointer, arch.PointerType.Size);
                else if (dt is PrimitiveType pt)
                    result[i] = (pt.Domain, pt.Size);
                else if (dt is Pointer ptr)
                    result[i] = (Domain.Pointer, ptr.Size);
                else 
                    result[i] = (Domain.Integer, AlignUp(dt.Size, 8));
            }
            return result;
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs[0] == reg;
            }
            return false;
        }

    }
}