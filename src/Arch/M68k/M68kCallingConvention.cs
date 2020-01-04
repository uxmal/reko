#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    public class M68kCallingConvention : CallingConvention
    {
        private M68kArchitecture arch;

        public M68kCallingConvention(M68kArchitecture arch)
        {
            this.arch = arch;
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            int stackOffset = 4;        // return address
            ccr.LowLevelDetails(4, stackOffset);
            if (dtRet != null)
            {
                ccr.RegReturn(Registers.d0);
            }

            for (int i = 0; i < dtParams.Count; ++i)
            {
                ccr.StackParam(dtParams[i]);
            }
            ccr.CallerCleanup(4);
        }

        public bool IsArgument(Storage stg)
        {
            // Arguments are passed on stack.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            return Registers.d0 == stg;
        }
    }
}
