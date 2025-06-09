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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Environments.RT11
{
    public class Rt11CallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;

        public Rt11CallingConvention(IProcessorArchitecture arch) : base("")
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
            ccr.LowLevelDetails(2, 2);
            if (dtRet is not null)
            {
                SetReturnRegisters(ccr, dtRet);
            }

            int gr = 0;
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var arg = arch.GetRegister("r" + gr)!;
                ++gr;
                ccr.RegParam(arg);
            }
            ccr.CallerCleanup(arch.PointerType.Size);
        }

        public void SetReturnRegisters(ICallingConventionBuilder ccr, DataType dtRet)
        {
            //$TODO: use dtRet?
            ccr.RegReturn(arch.GetRegister("r0")!);
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                //$TODO: need more info here.
                return true;
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
                return reg.Number == 0;
            return false;
        }
    }
}