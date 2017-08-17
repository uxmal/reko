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

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            int fpuStackDelta = 0;
            Storage stgRet = GetReturnStorage(dtRet);

            if (stgRet is FpuStackStorage)
                fpuStackDelta = 1;

            Storage stgThis = null;
            List<Storage> stgParams = new List<Storage>();

            int stackOffset = retAddressOnStack;

            if (dtThis != null)
            {
                stgThis = new StackArgumentStorage(stackOffset, PrimitiveType.Create(Domain.Pointer, pointerSize));
                stackOffset += Align(pointerSize, stackAlignment);
            }
            if (reverseArguments)
            {
                for (int i = dtParams.Count - 1; i >= 0; --i)
                {
                    int alignedSize = Align(dtParams[i].Size, stackAlignment);
                    stgParams.Add(new StackArgumentStorage(stackOffset, PrimitiveType.CreateWord(alignedSize)));
                    stackOffset += alignedSize;
                }
                stgParams.Reverse();
            }
            else
            {
                for (int i = 0; i < dtParams.Count; ++i)
                {
                    int alignedSize = Align(dtParams[i].Size, stackAlignment);
                    if (alignedSize == 0)
                        alignedSize = stackAlignment;
                    stgParams.Add(new StackArgumentStorage(stackOffset, dtParams[i]));
                    stackOffset += alignedSize;
                }
            }
            if (callerCleanup)
            {
                stackOffset = retAddressOnStack;
            }
            return new CallingConventionResult
            {
                Return = stgRet,
                ImplicitThis = stgThis,
                Parameters = stgParams,
                FpuStackDelta = fpuStackDelta,
                StackDelta = stackOffset
            };
        }

        public static Storage GetReturnStorage(DataType dtRet)
        {
            Storage stgRet = null;
            int retSize = dtRet != null ? dtRet.Size : 0;
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
                else if (retSize > 0)
                {
                    stgRet = Registers.eax;
                }
            }
            return stgRet;
        }
    }
}
