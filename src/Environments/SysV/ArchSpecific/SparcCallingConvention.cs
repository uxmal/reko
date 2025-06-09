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

namespace Reko.Environments.SysV.ArchSpecific
{
    public class SparcCallingConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] regs;
        private readonly RegisterStorage iret;
        private readonly SequenceStorage iret2;
        private readonly RegisterStorage fret0;
        private readonly RegisterStorage fret1;
        private readonly int wordSize;

        private IProcessorArchitecture arch;

        public SparcCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.regs = new[] { "o0", "o1", "o2", "o3", "o4", "o5" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.iret = arch.GetRegister("o0")!;
            this.iret2 = new SequenceStorage(
                PrimitiveType.CreateWord(this.arch.WordWidth.BitSize * 2),
                arch.GetRegister("o0")!,
                arch.GetRegister("o1")!);
            this.fret0 = arch.GetRegister("f0")!;
            this.fret1 = arch.GetRegister("f1")!;
            this.wordSize = this.arch.WordWidth.Size;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(wordSize, 0x0018);
            if (dtRet is not null)
            {
                SetReturnRegister(ccr, dtRet);
            }

            int ir = 0;
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtArg = dtParams[iArg];
                if (dtArg.Size <= wordSize)
                {
                    if (ir >= regs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(regs[ir]);
                        ++ir;
                    }
                }
                else if (dtArg.Size <= wordSize * 2)
                {
                    if (ir >= regs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else if (ir == regs.Length - 1)
                    {
                        var hi = regs[ir];
                        var lo = ccr.AllocateStackSlot(arch.WordWidth);
                        ccr.SequenceParam(hi, lo);
                        ++ir;
                    }
                    else
                    {
                        ccr.SequenceParam(regs[ir], regs[ir+1]);
                        ir += 2;
                    }
                }
                else
                    throw new NotImplementedException();
            }
        }

        public void SetReturnRegister(ICallingConventionBuilder ccr, DataType dtArg)
        {
            var ptArg = dtArg as PrimitiveType;
            if (ptArg is not null)
            {
                if (ptArg.Domain == Domain.Real)
                {
                    var f0 = fret0;
                    if (ptArg.Size <= 4)
                    {
                        ccr.RegReturn(f0);
                        return;
                    }
                    var f1 = fret1;
                    ccr.SequenceReturn(f1, f0);
                    return;
                }
                if (dtArg.Size <= wordSize)
                {
                    ccr.RegReturn(iret);
                    return;
                }
                if (dtArg.Size <= wordSize * 2)
                {
                    ccr.SequenceReturn(iret2);
                    return;
                }
                throw new NotImplementedException();
            }
            else if (dtArg is Pointer)
            {
                ccr.RegReturn(iret);
                return;
            }
            else if (dtArg.Size <= wordSize)
            {
                ccr.RegReturn(iret);
                return;
            }
            else if (dtArg.Size <= wordSize * 2)
            {
                ccr.SequenceReturn(iret2);
                return;
            }
            throw new NotImplementedException();
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return regs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return iret == stg || fret0 == stg || fret1 == stg;
        }
    }
}
