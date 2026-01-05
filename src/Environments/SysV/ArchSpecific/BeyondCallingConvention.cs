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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class BeyondCallingConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] regs;

        public BeyondCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.regs = new string[] { "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7", "r8", "r9", "r10", "r11", "r12" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
        }

        public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 0);
            if (dtRet is not null && dtRet is not VoidType)
            {
                ccr.RegReturn(regs[0]);
            }
            int ireg = 0;
            foreach (var dtParam in dtParams)
            {
                ccr.RegParam(regs[ireg++]);
            }
        }

        public override bool IsArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsOutArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }
    }
}