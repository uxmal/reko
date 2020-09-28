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

using System;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System.Collections.Generic;
using Reko.Core.Expressions;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class Arm64CallingConvention : CallingConvention
    {
        private RegisterStorage[] argRegs;

        public Arm64CallingConvention(IProcessorArchitecture arch)
        {
            argRegs = new[] { "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7" }
                .Select(r => arch.GetRegister(r)).ToArray();
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(8, 0x0010);

            //$TODO: need to deal with large args and stack parameters.
            int iReg = 0;
            foreach (var dtParam in dtParams)
            {
                ccr.RegParam(argRegs[iReg]);
                ++iReg;
            }
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs[0] == reg;
            }
            return false;
        }

    }
}