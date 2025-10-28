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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Mips
{
    // Based on algorithm described in
    // http://www.cygwin.com/ml/binutils/2003-06/msg00436.html

    public class O32CallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;
        private readonly RegisterStorage[] fregs;
        private readonly RegisterStorage iret;
        private readonly RegisterStorage iret2;
        private readonly RegisterStorage fret;
        private readonly bool allowFpRegs;

        public O32CallingConvention(IProcessorArchitecture arch, bool allowFpRegs) : base("")
        {
            this.arch = arch;
            this.iregs = new[] { "r4", "r5", "r6", "r7" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.fregs = new[] { "f12", "f13", "f14", "f15" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.iret = arch.GetRegister("r2")!;
            this.fret = arch.GetRegister("f1")!;
            this.iret2 = arch.GetRegister("r3")!;
            this.allowFpRegs = allowFpRegs;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.WordWidth.Size, 0x10);

            if (dtRet is not null)
            {
                SetReturnRegister(ccr, dtRet);
            }

            int ir =  0;
            bool firstArgIntegral = false;
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dtParam = dtParams[i];
                var prim = dtParam as PrimitiveType;
                if (prim is not null && prim.Domain == Domain.Real && this.allowFpRegs && !firstArgIntegral)
                {
                    if ((ir % 2) != 0)
                        ++ir;
                    if (ir >= fregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                    {
                        if (prim.Size == 4)
                        {
                            ccr.RegParam(fregs[ir]);
                            ir += 1;
                        }
                        else if (prim.Size == 8)
                        {
                            ccr.SequenceParam(fregs[ir], fregs[ir + 1]);
                            ir += 2;
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Real type of size {0} not supported.", prim.Size));
                        }
                    }
                }
                else
                {
                    if (ir == 0)
                        firstArgIntegral = true;
                    if (dtParam.Size <= arch.WordWidth.Size)
                    {
                        if (ir >= 4)
                        {
                            ccr.StackParam(dtParam);
                        }
                        else
                        {
                            ccr.RegParam(iregs[ir]);
                            ++ir;
                        }
                    }
                    else if (dtParam.Size <= arch.WordWidth.Size * 2)
                    {
                        if ((ir & 1) != 0)
                            ++ir;
                        if (ir >= 4)
                        {
                            ccr.StackParam(dtParam);
                        }
                        else
                        {
                            ccr.SequenceParam(iregs[ir], iregs[ir + 1]);
                            ir += 2;
                        }
                    }
                    else
                        throw new NotImplementedException();
                }
            }
        }

        public void SetReturnRegister(ICallingConventionBuilder ccr, DataType dt)
        {
            int bitSize = dt.BitSize;
            if (dt.Domain == Domain.Real && allowFpRegs)
            {
                ccr.RegReturn(fret);
            }
            else
            {
                if (dt.Domain == Domain.Real)
                    _ = this; //$DEBUG
                if (dt.BitSize <= arch.WordWidth.BitSize)
                {
                    ccr.RegReturn(iret);
                    return;
                }
                else if (dt.BitSize <= arch.WordWidth.BitSize * 2)
                {
                    ccr.SequenceReturn(iret, iret2);
                }
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg);
            }
            if (stg is StackStorage stk)
                return true;
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return iret == stg || fret == stg ||
                stg is SequenceStorage seq &&
                seq.Elements[0] == iret &&
                seq.Elements[1] == iret2;
        }
    }
}
