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
using System.Collections.Generic;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Microsoft-defined convention where the first two (small) parameters
    /// are passed in ECX, EDX respectively.
    /// </summary>
    public class FastcallConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] iArgs;
        private readonly int stackAlignment;

        public FastcallConvention(
            RegisterStorage arg1,
            RegisterStorage arg2,
            int stackAlignment)
            : base("__fascall")
        {
            this.iArgs = new[] { arg1, arg2 };
            this.stackAlignment = stackAlignment;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retSizeOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retSizeOnStack);
            ccr.CallerCleanup(retSizeOnStack);
            int iReg = 0;
            if (dtThis is not null)
            {
                ccr.ImplicitThisRegister(iArgs[iReg++]);
            }
            foreach (var dtParam in dtParams)
            {
                if (iReg < iArgs.Length)
                {
                    ccr.RegParam(iArgs[iReg++]);
                }
                else
                {
                    ccr.StackParam(dtParam);
                }
            }
            if (dtRet is not null)
            {
                ccr.RegReturn(Registers.eax);
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return 
                    reg.Domain == Registers.ecx.Domain ||
                    reg.Domain == Registers.edx.Domain;
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return
                    reg.Domain == Registers.eax.Domain ||
                    reg.Domain == Registers.edx.Domain;
            }
            return false;
        }
    }
}
