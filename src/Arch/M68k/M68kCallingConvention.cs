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

using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.M68k
{
    public class M68kCallingConvention : AbstractCallingConvention
    {
        private readonly M68kArchitecture arch;

        public M68kCallingConvention(M68kArchitecture arch) : base("")
        {
            this.arch = arch;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            int stackOffset = 4;        // return address
            ccr.LowLevelDetails(4, stackOffset);
            if (dtRet is not null)
            {
                ccr.RegReturn(Registers.d0);
            }

            for (int i = 0; i < dtParams.Count; ++i)
            {
                ccr.StackParam(dtParams[i]);
            }
            ccr.CallerCleanup(4);
        }

        public override bool IsArgument(Storage stg)
        {
            // Arguments are passed on stack.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return Registers.d0 == stg;
        }
    }
}
