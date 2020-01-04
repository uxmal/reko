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
    /// Implements the Win32 __thiscall calling convention.
    /// </summary>
    public class ThisCallConvention : CallingConvention
    {
        private RegisterStorage ecxThis;
        private int retAddressOnStack;
        private int stackAlignment;

        public ThisCallConvention(RegisterStorage ecxThis, int stackAlignment, int retAddressOnStack)
        {
            this.ecxThis = ecxThis;
            this.stackAlignment = stackAlignment;
            this.retAddressOnStack = retAddressOnStack;
        }

        /// <summary>
        /// If dtThis is supplied, it is known that it is the `this` 
        /// corresponding to an enclosing C++ class. If dtThis is null, then
        /// the first of the dtParams will be treated as a `this`.
        /// </summary>
        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retAddressOnStack);
            X86CallingConvention.SetReturnStorage(ccr, dtRet, stackAlignment);

            int i = 0;
            if (dtThis != null)
            {
                ccr.ImplicitThisRegister(this.ecxThis);
            }
            else if (dtParams.Count > 0)
            {
                ccr.RegParam(this.ecxThis);
                i = 1;
            }
            for (; i < dtParams.Count; ++i)
            {
                ccr.StackParam(dtParams[i]);
            }
            ccr.CalleeCleanup();
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return reg.Domain == Registers.ecx.Domain;
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
                    reg.Domain == Registers.eax.Domain;
            }
            return false;
        }
    }
}
