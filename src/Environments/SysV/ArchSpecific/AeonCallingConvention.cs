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
    // https://openrisc.io/or1k.html
    public class AeonCallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;

        public AeonCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.iregs = Enumerable.Range(3, 6).Select(i => arch.GetRegister((StorageDomain) i, default)!).ToArray();
        }

        public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 24);
            if (dtRet is not null)
            {
                SetReturnRegister(dtRet, ccr);
            }
            int iReg = 0;
            foreach (var dtParam in dtParams)
            {
                if (iReg < iregs.Length)
                {
                    ccr.RegParam(iregs[iReg]);
                    ++iReg;
                }
                else
                {
                    ccr.StackParam(dtParam);
                }
            }
        }

        private void SetReturnRegister(DataType dtRet, ICallingConventionBuilder ccr)
        {
            var r3 = arch.GetRegister((StorageDomain) 3, default)!;
            if (dtRet.BitSize <= 32)
            {
                ccr.RegReturn(r3);
            }
            else if (dtRet.BitSize <= 64)
            {
                var r4 = arch.GetRegister((StorageDomain) 4, default)!;
                ccr.SequenceReturn(r4, r3);
            }
            else
                throw new NotImplementedException("Return values > 64 bits not implemented yet.");
        }

        public override bool IsArgument(Storage stg)
        {
            throw new NotImplementedException();
        }

        public override bool IsOutArgument(Storage stg)
        {
            throw new NotImplementedException();
        }
    }
}
