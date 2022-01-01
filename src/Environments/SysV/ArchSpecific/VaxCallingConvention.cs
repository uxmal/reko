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
    public class VaxCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage regRet;

        public VaxCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regRet = arch.GetRegister("r0")!;
        }

        public void Generate(ICallingConventionEmitter ccr, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 4);
            foreach (var dtParam in dtParams)
            {
                ccr.StackParam(dtParam);
            }
            if (dtRet != null)
            {
                ccr.RegReturn(regRet);
            }
        }

        public bool IsArgument(Storage stg)
        {
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            return false;
        }
    }
}
