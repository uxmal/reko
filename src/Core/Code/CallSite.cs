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

using System;
using System.Text;

namespace Reko.Core.Code
{
    /// <summary>
    /// Interface between a calling procedure and a callee procedure. All registers
    /// used or defined by the called procedure are stored here, as is the stack
    /// depth before the call. The stack depth includes any return address pushed
    /// on the stack before control transfers to the callee. 
    /// </summary>
    public class CallSite
    {
        //$REVIEW: perhaps this can be architecture-specific? M68K doesn't need fpuStack,
        // and PowerPC doesn't need sizeOfReturnAddressOnStack.
        public CallSite(int sizeOfReturnAddressOnStack, int fpuStackDepthBefore)
        {
            this.SizeOfReturnAddressOnStack = sizeOfReturnAddressOnStack;
            this.FpuStackDepthBefore = fpuStackDepthBefore;
        }

        /// <summary>
        /// Depth of FPU stack before call.
        /// </summary>
        public int FpuStackDepthBefore { get; private set; }

        /// <summary>
        /// Size of the return address on the stack. Some architectures don't pass the continuation
        /// address on the stack, in which case this property should have the value 0.
        /// </summary>
        public int SizeOfReturnAddressOnStack { get; private set; }

        public int StackDepthOnEntry { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("retsize: {0};", SizeOfReturnAddressOnStack);
            if (StackDepthOnEntry != 0)
            {
                sb.AppendFormat(" depth: {0}", StackDepthOnEntry);
            }
            if (FpuStackDepthBefore != 0)
            {
                sb.AppendFormat(" FPU: {0};", FpuStackDepthBefore);
            }
            return sb.ToString();
        }
    }
}