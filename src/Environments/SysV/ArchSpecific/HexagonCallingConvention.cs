#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class HexagonCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;

        public HexagonCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public void Generate(ICallingConventionEmitter ccr, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            //$BUG: this is all just to get the ELF loader up and running.
            ccr.LowLevelDetails(4, 0);
            if (dtRet != null && !(dtRet is VoidType))
            {
                ccr.RegReturn(arch.GetRegister("r0")!);
            }
            foreach (var dt in dtParams)
            {
                ccr.RegParam(arch.GetRegister("r0")!);
            }
        }

        public bool IsArgument(Storage stg)
        {
            throw new NotImplementedException();
        }

        public bool IsOutArgument(Storage stg)
        {
            throw new NotImplementedException();
        }
    }
}
