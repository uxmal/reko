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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class M68kCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage d0;

        public M68kCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.d0 = arch.GetRegister("d0");
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 4);
            if (dtRet != null)
            {
                if (dtRet.BitSize > 32)
                    throw new NotImplementedException();
                ccr.RegReturn(d0);
            }

            var args = new List<Storage>();
            int stOffset = arch.PointerType.Size;
            foreach (var dtParam in dtParams)
            {
                ccr.StackParam(dtParam);
            }
            ccr.CallerCleanup(arch.PointerType.Size);
        }

        public bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
        }

        public bool IsOutArgument(Storage stg)
        {
            return this.d0 == stg;
        }
    }
}
