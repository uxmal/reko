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

namespace Reko.Environments.SysV.ArchSpecific;

//      r0       - constant 0 (hardware)
//      r1       - return address(set by hardware)
//      r2-r9    - called procedure parameter registers
//      r10-r13  - called procedure temporary registers(scratch)
//      r14-r25  - calling procedure reserved registers(preserved by
//                 callee, if used)
//      r26-r29  - reserved by linker
//      r30(fp) - frame pointer
//      r31(sp) - stack pointer
public class M88kCallingConvention : AbstractCallingConvention
{
    private readonly RegisterStorage[] regs;

    public M88kCallingConvention(IProcessorArchitecture arch)
        : base("")
    {
        this.regs = Enumerable.Range(2, 8)
            .Select(i => arch.GetRegister($"r{i}")!)
            .ToArray();
    }

    public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
    {
        ccr.LowLevelDetails(4, 0);
        int ireg = 0;
        foreach (var dt in dtParams)
        {
            if (ireg < regs.Length)
            {
                ccr.RegParam(regs[ireg]);
                ++ireg;
            }
            else
            {
                ccr.StackParam(dt);
            }
        }
        if (dtRet is not null)
        {
            if (dtRet.BitSize <= 32)
                ccr.RegReturn(regs[0]);
            else if (dtRet.BitSize <= 64)
                ccr.SequenceReturn(regs[0], regs[1]);
            else 
                throw new NotImplementedException("Need documentation on how to return >64 bit values");
        }
    }

    public override bool IsArgument(Storage stg)
    {
        switch (stg)
        {
        case RegisterStorage reg:
            return 2 <= reg.Number && reg.Number <= 9;
        case SequenceStorage seq:
            return
                seq.Elements.Length == 2 &&
                seq.Elements[0] is RegisterStorage r0 &&
                seq.Elements[1] is RegisterStorage r1 &&
                2 <= r0.Number && r0.Number <= 8 &&
                r1.Number == r0.Number + 1;
        case StackStorage stk:
            return stk.StackOffset > 0;
        }
        return false;
    }

    public override bool IsOutArgument(Storage stg)
    {
        switch (stg)
        {
        case RegisterStorage reg:
            return reg.Number == 2;
        case SequenceStorage seq:
            return
                seq.Elements.Length == 2 &&
                seq.Elements[0] is RegisterStorage r0 &&
                seq.Elements[1] is RegisterStorage r1 &&
                r0.Number == 2 &&
                r1.Number == 3;
        }
        return false;
    }
}
