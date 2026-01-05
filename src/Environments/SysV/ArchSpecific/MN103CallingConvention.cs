#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Environments.SysV.ArchSpecific;

//$BUG: the implementation of this calling convention is pure fantasy,
// due to the lack of documentation.
public class MN103CallingConvention : AbstractCallingConvention
{
    private readonly IProcessorArchitecture arch;
    private readonly RegisterStorage[] iregs;

    public MN103CallingConvention(IProcessorArchitecture arch)
        : base("")
    {
        this.arch = arch;
        this.iregs = arch.GetRegisters();
    }

    public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
    {
        ccr.LowLevelDetails(4, 0);
        if (dtRet is null)
        {
            ccr.RegReturn(iregs[0]);
        }
        int i = 0;
        foreach (var dtParam in dtParams)
        {
            ccr.RegParam(iregs[++i]);
        }
    }

    public override bool IsArgument(Storage stg)
    {
        //$BUG: clearly not correct.
        return stg is RegisterStorage;
    }

    public override bool IsOutArgument(Storage stg)
    {
        throw new NotImplementedException();
    }
}
