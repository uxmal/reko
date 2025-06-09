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
    /// The <c>__cdecl</c> calling convention on X86 passes all parameters on the 
    /// and returns any results in eax or edx:eax
    /// </summary>
    public class X86CallingConvention : AbstractCallingConvention
    {
        private readonly int stackAlignment;
        private readonly int pointerSize;
        private readonly Storage fpuReturnStorage;
        private readonly bool callerCleanup;
        private readonly bool reverseArguments;

        public X86CallingConvention(
            int stackAlignment, 
            int pointerSize, 
            bool callerCleanup,
            bool reverseArguments)
             : base("__cdecl")
        {
            this.stackAlignment = stackAlignment;
            this.pointerSize = pointerSize;
            //$TODO: not strictly correct, should be real80, but 
            // existing code was generating real64. Look into fixing this
            // later.
            this.fpuReturnStorage = new FpuStackStorage(0, PrimitiveType.Real64);
            this.callerCleanup = callerCleanup;
            this.reverseArguments = reverseArguments;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack, 
            DataType? dtRet, 
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(stackAlignment, retAddressOnStack);
            SetReturnStorage(ccr, dtRet, stackAlignment);

            if (dtThis is not null)
            {
                ccr.ImplicitThisStack(dtThis);
            }
            if (reverseArguments)
            {
                for (int i = dtParams.Count - 1; i >= 0; --i)
                {
                    ccr.StackParam(dtParams[i]);
                }
                ccr.ReverseParameters();
            }
            else
            {
                for (int i = 0; i < dtParams.Count; ++i)
                {
                    ccr.StackParam(dtParams[i]);
                }
            }
            if (callerCleanup)
            {
                ccr.CallerCleanup(retAddressOnStack);
            }
            else
            {
                ccr.CalleeCleanup();
            }
        }

        public static void SetReturnStorage(ICallingConventionBuilder ccr, DataType? dtRet, int stackAlignment)
        {
            if (dtRet is null)
                return;

            int retSize = dtRet.Size;
            if (retSize > 8)
            {
                // returns a pointer to the stack-allocated large return value
                ccr.RegReturn(Registers.eax);
                return;
            }
            else
            {
                if (dtRet.Domain == Domain.Real)
                {
                    ccr.FpuReturn(-1, PrimitiveType.Real64);
                    return;
                }
                if (retSize > 4)
                {
                    ccr.SequenceReturn(Registers.edx, Registers.eax);
                    return;
                }
                if (retSize > 2)
                {
                    if (stackAlignment == 4)
                        ccr.RegReturn(Registers.eax);
                    else
                        ccr.SequenceReturn(Registers.dx, Registers.ax);
                    return;
                }
                if (retSize > 1)
                {
                    ccr.RegReturn(Registers.ax);
                    return;
                }
                ccr.RegReturn(Registers.al);
                return;
            }
        }

        public override bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
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
