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
    /// Implements the Win32 __thiscall calling convention.
    /// </summary>
    public class ThisCallConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage ecxThis;
        private readonly int stackAlignment;

        public ThisCallConvention(RegisterStorage ecxThis, int stackAlignment)
            : base("__thiscall")
        {
            this.ecxThis = ecxThis;
            this.stackAlignment = stackAlignment;
        }

        /// <summary>
        /// If dtThis is supplied, it is known that it is the `this` 
        /// corresponding to an enclosing C++ class. If dtThis is null, then
        /// the first of the dtParams will be treated as a `this`.
        /// </summary>
        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retAddressOnStack);
            X86CallingConvention.SetReturnStorage(ccr, dtRet, stackAlignment);

            int i = 0;
            if (dtThis is not null)
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

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return reg.Domain == Registers.ecx.Domain;
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
                    reg.Domain == Registers.eax.Domain;
            }
            return false;
        }
    }
}
