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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Machine;

namespace Reko.Environments.SysV.ArchSpecific
{
    //$REVIEW: this was taken verbatim from the Windows calling convention.
    // There is no guarantee that this is correct for POSIX. 
    //$TODO: find a description of the correct POSIX calling convention and
    // implement it.
    public class AlphaCallingConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] iRegs;
        private readonly RegisterStorage[] fRegs;
        private RegisterStorage iRet;

        public AlphaCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.iRegs = new[] { "r16", "r17", "r18", "r19", "r20", "r21" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.iRet = arch.GetRegister("r0")!;
            this.fRegs = new[] { "f12", "f13", "f14", "f15" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(8, 0);      //$BUGBUG: the '0' is incorrect, but we need a reliable spec for WinAlpha to determine exact value.
            if (dtRet is not null)
            {
                ccr.RegReturn(iRet);
            }
            int iReg = 0;
            foreach (var dtParam in dtParams)
            {
                if (iReg < iRegs.Length)
                {
                    ccr.RegParam(iRegs[iReg]);
                    ++iReg;
                }
                else
                {
                    ccr.StackParam(PrimitiveType.Word64);
                }
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iRegs.Contains(reg) || fRegs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return iRet == stg;
        }
    }
}
