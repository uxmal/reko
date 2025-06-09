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

namespace Reko.Environments.SysV.ArchSpecific
{
    public class BlackfinCallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;

        public BlackfinCallingConvention(IProcessorArchitecture arch) : base("")
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
            //$BUG: this is all just to get the ELF loader up and running.
            // fill in with details from 
            // https://blackfin.uclinux.org/doku.php?id=toolchain:application_binary_interface
            ccr.LowLevelDetails(4, 0);
            if (dtRet is not null && !(dtRet is VoidType))
            {
                ccr.RegReturn(arch.GetRegister("R0")!);
            }
            foreach (var dt in dtParams)
            {
                ccr.RegParam(arch.GetRegister("R0")!);
            }
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
