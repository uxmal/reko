#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Machine;
using Reko.Core.Expressions;

namespace Reko.Environments.Windows
{
    /*
n the PowerPC ABI for Windows NT, registers are classified as either callee-saved or caller-saved, based on their preservation requirements across function calls.

Callee-Saved Registers
These registers must be preserved by the called function (callee). If the callee modifies them, it must restore their original values before returning control to the caller.

General Purpose Registers (GPRs):
r13 to r31
Floating Point Registers (FPRs):
f14 to f31
Caller-Saved Registers
These registers do not need to be preserved by the callee. The caller is responsible for saving and restoring these registers if it needs their values after a function call.

General Purpose Registers (GPRs):
r0 to r12
Floating Point Registers (FPRs):
f0 to f13
Special Registers:
LR (Link Register): Used to store the return address. The caller should save it if needed after the call.
CTR (Count Register): Caller-saved, often used for loop counters.
CR (Condition Register): Caller-saved, used for conditional operations.
Additional Notes:
Stack Pointer (r1): Managed by the calling convention. Functions adjust it but must restore it before returning.
Parameter Passing:
r3 to r10 are typically used for passing arguments.
f1 to f13 are used for floating-point arguments.
Return Values:
Integer return values are placed in r3.
Floating-point return values are placed in f1.     



Windows NT PowerPC Stack Frame Layout
Offset  	    Content
0x00 (SP)	    Back chain pointer (previous stack pointer)
0x04	        Saved Link Register (LR)
0x08	        Saved Condition Register (CR)
0x0C	        Reserved for the compiler or alignment
0x10 to 0x4C	Saved general-purpose registers (r13 to r31)
0x50 to 0x8C	Saved floating-point registers (f14 to f31)
...	Local variables, temporaries, and stack space for outgoing arguments (aligned to 16 bytes)

Details on Key Components
Back Chain Pointer (0x00):

Points to the previous stack frame.
This is useful for walking the stack during debugging or exception handling.
Link Register (LR, 0x04):

The return address of the caller function.
The callee saves this register at the beginning of the function and restores it before returning.
Condition Register (CR, 0x08):

The condition code for conditional branches.
Saved to maintain the state of the calling function.
Saved Registers:

General Purpose Registers (r13 to r31, 0x10 to 0x4C):
These are callee-saved registers, meaning the function must preserve their values across calls.
Floating Point Registers (f14 to f31, 0x50 to 0x8C):
These are also callee-saved registers, used for floating-point computations.
Local Variables and Temporaries:

The stack frame includes space for local variables, temporaries, and any additional data required by the function.
Compiler-generated alignment ensures the stack pointer (SP) remains aligned to 16 bytes.
Outgoing Arguments:

If there are more arguments than can fit in registers (r3 to r10 for integer arguments and f1 to f13 for floating-point arguments), additional arguments are placed on the stack.
Alignment:

The stack is aligned to 16 bytes to satisfy the requirements of the PowerPC ABI.
Register Save Area Offsets
Register	Offset (from SP)
LR	0x04
CR	0x08
r13	0x10
r14	0x14
...	...
r31	0x4C
f14	0x50
f15	0x58
...	...
f31	0x8C
Function Prologue and Epilogue
Prologue:
Save the back chain pointer (SP).
Save the LR and CR registers.
Save callee-saved registers (r13 to r31 and f14 to f31).
Allocate space for local variables.
Epilogue:
Restore callee-saved registers.
Restore LR and CR.
Restore the back chain pointer (SP).
Return to the caller.

     */
    public class PowerPcCallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;

        public PowerPcCallingConvention(IProcessorArchitecture arch) : base("")
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
            //$TODO: finding it hard to locate information about the calling
            // convention on PowerPC Win32. May have to reverse engineer it.
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                //$TODO: see comment above.
                return true;
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            //$TODO
            return false;
        }
    }
}
