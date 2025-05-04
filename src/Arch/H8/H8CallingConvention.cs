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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.H8
{
    public class H8CallingConvention : AbstractCallingConvention
    {
        private readonly RegisterStorage[] argRegs;

        // http://www.bound-t.com/app_notes/an-h8-300.pdf

        public H8CallingConvention(PrimitiveType wordSize) : base("")
        {
            RegisterStorage[] regs;
            if (wordSize.BitSize == 16)
            {
                regs = Registers.RRegisters;
            }
            else
            {
                regs = Registers.GpRegisters;
            }
            this.argRegs = new[] { regs[0], regs[1], regs[2] };
        }

        /*
Introduction
This section explains how Bound­T supports the procedure calling protocol used by the GNU
H8/300 C compiler, using the default compilation options. This section is based on the H8/300
GCC Application Binary Interface description [8] and on observation of compiled code.
The GCC calling protocol has the following features:
• The native instructions BSR/JSR and RTS are used.
• Parameters are passed in registers and (if many) on the stack.
• The callee can alter the values of registers R0, R1, R2, R3.
• The callee must not alter the values of registers  R4, R5, R6, R7 or must save the original
value and restore it before returning to the caller.
Parameter passing
Up to three parameters are passed in R0, R1 and R2. The rest of the parameters are passed on
the stack, pushed there before the call. Parameter sizes on the stack are rounded to an even
number of octets. For example, an octet parameter is held in one word of stack space.
Subprogram call
The subprogram call sequence consists of pushing the stack­based parameters on the stack,
loading register­based parameters into registers, and executing a BSR or JSR.
Use of the stack in the callee
The callee subprogram usually allocates stack space for its local and temporary variables. It can
do this by PUSH instructions or by decreasing the SP in some other way (eg. by subtraction).
The callee uses the stack also to save the values of the callee­save registers  R4­R6  when
necessary. The exact lay­out of the stack frame in the callee depends on the GCC version
(see [8]) and is not relevant to Bound­T.
Of course, if the callee itself performs calls to other subprograms, it may use the stack for
parameters to these other subprograms. This means that SP can vary quite dynamically during
the execution of a subprogram.
Access to stacked parameters and locals
Under the GCC option ­fomit­frame­pointer there is no frame pointer. This means that all stackbased data must be accessed using  SP­relative addressing (register indirect based on  SP).
However, as the value of SP  can vary during the execution of the subprogram, so must the
offset (displacement) be varied. For example, if the address  SP + 8 accesses a given local
variable before a PUSH instruction, the same variable must be accessed with SP + 10 after the
PUSH instruction decreases SP by two.
Bound-T for H8/300 Calling Protocols 33
Bound­T tracks the changes in SP and translates SP­relative addresses with varying offsets to
addresses relative to the SP on entry (after the BSR or JSR) with fixed offsets. The fixed offset
for a parameter is 2 or more (offset 0 refers to the return address). The fixed offset for local
variables is negative.
Bound­T does not yet support the use of frame pointers.
Return from subprogram
To return, the subprogram pops its local variables from the stack, restores the caller­save
registers from the stack, and executes RTS.
There may be more than one such return point in a subprogram         * 
         */
        public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(2, 2);
            int iParam = 0;
            if (dtRet is not null)
            {
                ccr.RegParam(argRegs[iParam++]);
            }
            foreach (var dtParam in dtParams)
            {
                if (iParam < argRegs.Length)
                {
                    //$TODO: args whose size is larger than a reg?
                    // Docs say nothing.
                    ccr.RegParam(argRegs[iParam++]);
                }
                else
                {
                    ccr.StackParam(dtParam);
                }
            }
            if (dtRet is {})
            {
                ccr.RegReturn(argRegs[0]);
            }
        }

        public override bool IsArgument(Storage stg)
        {
            return stg is RegisterStorage reg &&
                0 <= reg.Number && reg.Number <= 3;
        }

        public override bool IsOutArgument(Storage stg)
        {
            return stg is RegisterStorage reg && reg.Number == 0;
        }
    }
}
