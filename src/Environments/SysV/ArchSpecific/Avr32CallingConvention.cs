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

namespace Reko.Environments.SysV.ArchSpecific
{
    public class Avr32CallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;

        /*
* https://www.avrfreaks.net/forum/avr32-c-compiler-calling-conventions
* The CPU has 2+14 registers that can be grouped like this:

SR - Status Register, contains CPU flags and stuff (see documentation)
PC - Program Counter, shouldn't be modified directly unless you want to do something really crazy
LR - Link Register, for calling functions, can be used as a general purpose register
SP - Stack Pointer, can be used as a general purpose register if you don't care about the stack :P
R12 - General purpose register with special role in function calls
R11-R8 - General purpose registers, used in function calls, clobberable
R7-R0 - General purpose registers, mustn't be clobbered

A normal function call works like this:
- Function arguments are put in R12 to R8 (in that order); arguments 6+ are put on the stack in some way (not sure about order).
- One of the ?CALL instructions is used to call the function. This moves the PC of the following instruction into LR.
- Inside the function R8 to R12 may be used freely without having to care about previous contents. This means that if you call another function from your function, you can't rely on the contents of these registers any more (R12 is special. See below.)
- If you need more registers, especially for stuff that should persist over calls to sub-functions, you must push some of the registers R0 to R7 onto the stack. Then you can use those freely.
- If you call a function, you need to push LR too, as the ?CALL instruction will clobber it. You should push LR before R0 to R7, as mentioned below. PUSHM/STM can do that for you.
- When you're done, place the function's return value in R12 (No idea about functions that return structs). The RET* instruction can do that for you if it's -1, 0 or 1.
- Restore any register (of R0 to R7) that you pushed onto the stack.
- If you pushed LR onto the stack first, pop it into PC. If you didn't call any functions and LR's contents are unmodified, call a flavour of the RET* instruction, which moves LR's contents to PC after setting R12's return value for you.

Interrupt handlers are similar, but the CPU will push R8 to R12 and LR onto the stack for you so that you can use them freely. Other registers must be saved by you. ISRs must be exited via the RETE instruction to restore R8 to R12 and LR.

What the different instructions do is explained in the Atmel AVR 32-bit Architecture Manual. The following instructions should have some insightful information: ACALL, ICALL, MCALL, RCALL, RET{cond4}, LDM, STM, POPM, PUSHM. If you want to write your own functions in assembly code, you should probably read up on all the arithmetic, multiplication, logic, bit, shift and data transfer instructio*/

        public Avr32CallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.iregs = new RegisterStorage[]
            {
                arch.GetRegister("r12")!,
                arch.GetRegister("r11")!,
                arch.GetRegister("r10")!,
                arch.GetRegister("r9")!,
                arch.GetRegister("r8")!,
            };
            this.InArgumentComparer = new StorageCollator(iregs);
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 0);
            foreach (var dt in dtParams)
            {
                ccr.StackParam(dt);
            }
            if (dtRet is not null)
            {
                ccr.RegReturn(arch.GetRegister("r12")!);
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return 8 <= reg.Number && reg.Number <= 12;
            }
            if (stg is StackStorage stack)
            {
                return stack.StackOffset > 0;
            }
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return 11 <= reg.Number && reg.Number <= 12;
            }
            return false;
        }
    }
}
