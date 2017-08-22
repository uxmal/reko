#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
    /// The __cdecl calling convention on X86 passes all parameters on the 
    /// and returns any results in eax or edx:eax
    /// </summary>
    public class X86CallingConvention : CallingConvention
    {
        private readonly int retAddressOnStack;
        private readonly int stackAlignment;
        private readonly int pointerSize;
        private readonly Storage fpuReturnStorage;
        private readonly bool callerCleanup;
        private readonly bool reverseArguments;

        public X86CallingConvention(
            int retAddressOnStack,
            int stackAlignment, 
            int pointerSize, 
            bool callerCleanup,
            bool reverseArguments)
        {
            this.retAddressOnStack = retAddressOnStack;
            this.stackAlignment = stackAlignment;
            this.pointerSize = pointerSize;
            //$TODO: not strictly correct, should be real80, but 
            // existing code was generating real64. Look into fixing this
            // later.
            this.fpuReturnStorage = new FpuStackStorage(0, PrimitiveType.Real64);
            this.callerCleanup = callerCleanup;
            this.reverseArguments = reverseArguments;
        }

        public override ICallingConventionEmitter Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retAddressOnStack);
            int fpuStackDelta = 0;
            ccr.Return = GetReturnStorage(dtRet, stackAlignment);

            if (ccr.Return is FpuStackStorage)
                fpuStackDelta = 1;

            if (dtThis != null)
            {
                ccr.StackParam(dtThis);
                ccr.ImplicitThis = ccr.Parameters[0];
                ccr.Parameters.Clear();
            }
            if (reverseArguments)
            {
                for (int i = dtParams.Count - 1; i >= 0; --i)
                {
                    ccr.StackParam(dtParams[i]);
                }
                ccr.Parameters.Reverse();
            }
            else
            {
                for (int i = 0; i < dtParams.Count; ++i)
                {
                    ccr.StackParam(dtParams[i]);
                }
            }
            ccr.FpuStackDelta = fpuStackDelta;
            ccr.StackDelta = callerCleanup ? retAddressOnStack : ccr.stackOffset;
            return ccr;
        }

        public static Storage GetReturnStorage(DataType dtRet, int stackAlignment)
        {
            if (dtRet == null)
                return null;

            Storage stgRet = null;
            int retSize = dtRet.Size;
            if (retSize > 8)
            {
                // returns a pointer to the stack-allocated large return value
                stgRet = Registers.eax;
            }
            else
            {
                var pt = dtRet.ResolveAs<PrimitiveType>();
                if (pt != null && pt.Domain == Domain.Real)
                {
                    stgRet = new FpuStackStorage(0, PrimitiveType.Real64);
                }
                else if (retSize > 4)
                {
                    stgRet = new SequenceStorage(Registers.edx, Registers.eax);
                }
                else if (retSize > 2)
                {
                    if (stackAlignment == 4)
                        stgRet = Registers.eax;
                    else
                        stgRet = new SequenceStorage(Registers.dx, Registers.ax);
                }
                else if (retSize > 1)
                {
                    stgRet = Registers.ax;
                }
                else
                {
                    stgRet = Registers.al;
                }
            }
            return stgRet;
        }
    }
}
