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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Microsoft-defined convention where the first two (small) parameters
    /// are passed in ECX, EDX respectively.
    /// </summary>
    public class FastcallConvention : CallingConvention
    {
        private RegisterStorage[] iArgs;
        private int stackAlignment;
        private int retSizeOnStack;

        public FastcallConvention(
            RegisterStorage arg1,
            RegisterStorage arg2,
            int stackAlignment,
            int retSizeOnStack)
        {
            this.iArgs = new[] { arg1, arg2 };
            this.stackAlignment = stackAlignment;
            this.retSizeOnStack = retSizeOnStack;
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retSizeOnStack);
            ccr.CallerCleanup(retSizeOnStack);
            int iReg = 0;
            if (dtThis != null)
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
            if (dtRet != null)
            {
                ccr.RegReturn(Registers.eax);
            }
        }

        public bool IsArgument(Storage stg)
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

        public bool IsOutArgument(Storage stg)
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
