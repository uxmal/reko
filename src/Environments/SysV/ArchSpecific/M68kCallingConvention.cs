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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class M68kCallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage d0;
        private readonly RegisterStorage fp0;

        public M68kCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.d0 = arch.GetRegister("d0")!;
            this.fp0 = arch.GetRegister("fp0")!;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 4);
            if (dtRet is not null)
            {
                if (dtRet.Domain == Domain.Real)
                    ccr.RegReturn(fp0);
                if (dtRet.BitSize > 64)
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

        public override bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return this.d0 == stg;
        }
    }
}
