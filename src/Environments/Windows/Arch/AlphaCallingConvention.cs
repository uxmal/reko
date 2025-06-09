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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Types;
using System.Linq;
using Reko.Core.Machine;

namespace Reko.Environments.Windows
{
    // https://blogs.msdn.microsoft.com/oldnewthing/20170807-00/?p=96766
    /*
    There are 32 integer registers, all 64 bits wide. Formally, they are known by the names r0 through r31, but Win32 assigns them the following mnemonics which correspond to their use in the Win32 calling convention
Register    Mnemonic    Meaning             Preserved?  Notes
r0          v0          value               No          On function exit, contains the return value.
r1…r8       t0…t7       temporary           No          
r9…r14      s0…s5       saved               Yes
r15         fp          frame pointer       Yes         For functions with variable-sized stacks.
r16…r21     a0…a5       argument            No          On function entry, contains function parameters.
r22…r25     t8…t11      temporary           No
r26         ra          return address      Not normally
r27         t12         temporary           No
r28         at          assembler temporary Volatile    Long jump assist.
r29         gp          global pointer      Special     Not used by 32-bit code.
r30         sp          stack pointer       Yes
r31         zero        reads as zero       N/A         Writes are ignored.
    */
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
            if (stg is RegisterStorage reg)
            {
                return iRet.Equals(reg);
            }
            return false;
        }
    }
}
 