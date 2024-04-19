#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Diagnostics;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    // ebx   ecx   edx   esi   edi   ebp  
    public class i386KernelCallingConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] iregs;
        private readonly RegisterStorage eax;
        private readonly RegisterStorage edx;

        public i386KernelCallingConvention(IProcessorArchitecture arch)
        {
            this.iregs = new[] { "ebx", "ecx", "edx", "esi", "edi", "ebp" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.eax = arch.GetRegister("eax")!;
            this.edx = iregs[2];
        }

        public override void Generate(
            ICallingConventionEmitter ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 6 * 4);
            // Linus hates C++
            Debug.Assert(dtThis is null);

            if (dtRet is not null)
            {
                if (dtRet.BitSize <= (int)eax.BitSize)
                {
                    ccr.RegReturn(eax);
                }
                else
                {
                    ccr.SequenceReturn(edx, eax);
                }
            }
            int iReg = 0;
            foreach (var dtParam in dtParams)
            {
                if (dtParam.BitSize <= (int)iregs[iReg].BitSize)
                {
                    ccr.RegParam(iregs[iReg]);
                    ++iReg;
                }
                else
                {
                    ccr.SequenceParam(iregs[iReg], iregs[iReg + 1]);
                    iReg += 2;
                }
            }
            ccr.CallerCleanup(4);
        }

        public override bool IsArgument(Storage stg)
        {
            return stg is RegisterStorage reg &&
                iregs.Contains(reg);
        }

        public override bool IsOutArgument(Storage stg)
        {
            return false;
        }
    }
}