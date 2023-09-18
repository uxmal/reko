#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Msp430
{
    public class Msp430CallingConvention : CallingConvention
    {
        /*
Arguments with a type that fits in a single CPU register are passed in a single CPU register.
For MSP430 and MSP430X, types up to 16 bits are passed in a single register. Pointer types are also passed in
a single register, regardless of size.
For MSP430X, pointer types can be 20 bits when using large code or large data memory models, but CPU
registers are also 20 bits, so pointer values always fit in a single register. For non-pointer values, MSP430X CPU
registers are treated as if they had only 16 bits. A consequence of this is that the registers used to implement
argument passing are the same for MSP430 and MSP430X, regardless of the memory model used.
*/

        /*
Arguments with a type that is larger than a single register, but no larger than twice the size of a single register,
are passed in a register pair. The lowest-numbered register holds the LSW (least significant word).
For MSP430 and MSP430X, register pairs do not need to be aligned, so R12:R13, R13:R14, and R14:R15 are
the valid register pairs. Types up to 32 bits are passed in a register pair. This includes "long int", "float" and
structs of up to 32 bits size passed by value.


3.3.3 Split Pairs
For MSP430 and MSP430X, a 
MSP430 and MSP430X example:
C source code:
 void func1(int a0, long a1, long a2);
 int a0;
 long a1, a2;
 func2(void)
 {
 func1(a0, a1, a2);
 }
Compiled assembly code:
 SUB.W #2,SP
 MOV.W &a0, R12
 MOV.W &a1+0,R13
 MOV.W &a1+2,R14
 MOV.W &a2+0,R15
 MOV.W &a2+2,0(SP)
3.3.4 Quads (Four-Register Arguments)
For MSP430 and MSP430X, arguments whose size is greater than 32 bits and up to 64 bits use register quads.
For example, R8::R11 is the notation used in this manual for a register quad consisting of the registers R8,
R9, R10, and R11 in sequence. The lowest-numbered register holds the LSW. Register quads must be aligned.
Therefore, only R8::R11 and R12::R15 are valid register quads. Register quads require special handling.
If there are enough argument registers remaining (all four) to pass the 64-bit value, all four will be used.
Otherwise, the 64-bit value will be passed entirely on the stack. This may leave unused argument registers (in
other words, a "hole"). The calling convention will try to back-fill the hole with subsequent arguments, but only
if they fit entirely in registers (see example 2 that follows). That is, after any argument has been placed on the
stack, no argument will be placed in a "split pair", as in Section 3.3.3.
MSP430 and MSP430X example 1:
C source code
 void func1(long long a0, long long a1);
 long long a0, a1;
 func2(void)
 {
 func1(a0, a1);
 }
Compiled in any model:
 SUB.W #8,SP
 MOV.W &a0+0,R12
 MOV.W &a0+2,R13
 MOV.W &a0+4,R14
 MOV.W &a0+6,R15
 MOV.W &a1+0,0(SP)
 MOV.W &a1+2,2(SP)
 MOV.W &a1+4,4(SP)
 MOV.W &a1+6,6(SP)


MSP430 and MSP430X example 2:
This example shows a 64-bit argument that doesn't fit entirely in registers, so it is passed entirely on the stack,
leaving unused registers which are then back-filled with arguments a2, a3, and a4.
C source code
 void func1(int a0, long long a1, int a2, int a3, int a4);
 int a0, a2, a3, a4;
 long long a1;
 func2(void)
 {
 func1(a0, a1, a2, a3, a4);
 }
Compiled in any model:
 SUB.W #8,SP
 MOV.W &a0,R12
 MOV.W &a1+0,0(SP)
 MOV.W &a1+2,2(SP)
 MOV.W &a1+4,4(SP)
 MOV.W &a1+6,6(SP)
 MOV.W &a2,R13
 MOV.W &a3,R14
 MOV.W &a4,R15
MSP430 and MSP430X example 3:
This example shows a 64-bit argument that doesn't fit entirely in registers, so it is passed entirely on the stack,
leaving unused registers which are then back-filled. However, the last argument (a 32-bit type) does not fit
entirely in registers, so it is passed entirely on the stack. This type would have been split and passed partially in
R15 and partially on the stack if the 64-bit argument weren't already on the stack.
C source code
 void func1(int a0, long long a1, long a2, long a3);
 int a0;
 long long a1;
 long a2, a3;
 func2(void)
 {
 func1(a0, a1, a2, a3);
 }
Compiled in any model (note that R15 is unused):
 SUB.W #12,SP
 MOV.W &a0,R12
 MOV.W &a1+0,0(SP)
 MOV.W &a1+2,2(SP)
 MOV.W &a1+4,4(SP)
 MOV.W &a1+6,6(SP)
 MOV.W &a2+0,R13
 MOV.W &a2+2,R14
 MOV.W &a3+0,8(SP)
 MOV.W &a3+2,10(SP)
On MSP430 and MSP430X, holes and back-fill can only occur when register quads are used. If only singles and
pairs are used, argument registers are used in order, there is no back-filling, and there are no holes.

For MSP430 and MSP430X, for efficiency, the compiler uses a special calling convention for certain compiler
helper functions that take two 64-bit arguments ("long long int" and "double" arithmetic).
In this special case, the compiler allows two register quads for argument passing: R8::R11 and R12::R15. This is
the only case in which R8 through R11 are used as argument registers. The first argument is passed in R8::R11
and the second argument is passed in R12::R15. The return value is in R12::R15, as usual.
See Section 6.3 for helper functions that use modified conventions.
MSP430 and MSP430X example:
C source code
 long long a1, a2;
 long long func(void)
 {
 return a1 / a2;
 }
Compiled in small code, small data model:
 func:
 PUSH.W R10; R10 is caller-saved!
 PUSH.W R9 ; R9 is caller-saved!
 PUSH.W R8 ; R8 is caller-saved!
 MOV.W &a1+0,R8
 MOV.W &a1+2,R9
 MOV.W &a1+4,R10
 MOV.W &a1+6,R11
 MOV.W &a2+0,R12
 MOV.W &a2+2,R13
 MOV.W &a2+4,R14
 MOV.W &a2+6,R15
 CALL #__mspabi_divlli
 BR #__mspabi_func_epilog_

3.3.6 C++ Argument Passing
In C++, the "this" pointer is passed to non-static member functions in R12 as an implicit first argument. (If a
non-static member function returns a struct by reference, the order is "&struct", "this".)
3.3.7 Passing Structs and Unions
Structures and unions are passed by reference, as described in Section 3.5.
3.3.8 Stack Layout of Arguments Not Passed in Registers
Any arguments not passed in registers are placed on the stack at increasing addresses, starting at 0(SP). Each
argument is placed at the next available address correctly aligned for its type, subject to the following additional
considerations:
• The stack alignment of a scalar is that of its declared type.
• Regardless of the alignment required by its members, the stack alignment of a structure is the smallest power
of two greater than or equal to its size. This is to allow loading arguments with aligned loads, even if the type
is not naturally aligned strictly enough, which might be the case with struct of size 32 containing an array of
char.
• Each argument reserves an amount of stack space equal to its size rounded up to the next multiple of its
stack alignment.
For a variadic C function (that is, a function declared with an ellipsis indicating that it is called with varying
numbers of arguments), the last explicitly declared argument and all remaining arguments are passed on the
stack, so that its stack address can act as a reference for accessing the undeclared arguments.
Calling Conventions www.ti.com
26 MSP430 Embedded Application Binary Interface SLAA534A – JUNE 2013 – REVISED JUNE 2020
Submit Document Feedback
Copyright © 2022 Texas Instruments Incorporated
Undeclared scalar arguments to a variadic function that are smaller than int are promoted to and passed as int,
in accordance with the C language.
Alignment "holes" can occur between arguments passed on the stack, but "back-fill" does not occur.

*/
        private readonly Msp430Architecture arch;
        private readonly RegisterStorage[] regs;

        public Msp430CallingConvention(Msp430Architecture arch)
        {
            this.arch = arch;
            this.regs = new[] { "r12", "r13", "r14", "r15" }
                    .Select(r => arch.GetRegister(r)!)
                    .ToArray();
        }

        public void Generate(ICallingConventionEmitter ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(2, 0);
            int iReg = 0;
            if (dtRet is not null && dtRet is not VoidType)
            {
                if (dtRet.BitSize < 32)
                    ccr.RegReturn(regs[0]);
                else if (dtRet.BitSize == 32)
                    ccr.SequenceReturn(regs[0], regs[1]);
                else if (dtRet.BitSize == 32)
                    ccr.SequenceReturn(regs[0], regs[1], regs[2], regs[3]);
                else
                {
                    ccr.RegReturn(regs[0]);
                    ccr.RegParam(regs[iReg]);
                    ++iReg;
                }
            }
            if (dtThis is not null)
                   throw new NotImplementedException();
            bool usedStack = false;
            foreach (var dtParam in dtParams)
            {
                if (dtParam.BitSize < 32)
                {
                    if (!usedStack && iReg < regs.Length)
                    {
                        ccr.RegParam(regs[iReg]);
                        ++iReg;
                    }
                    else
                    {
                        usedStack = true;
                        ccr.StackParam(dtParam);
                    }
                }
                else if (dtParam.BitSize == 32)
                {
                    int bitsRemaining = dtParam.BitSize;
                    Storage stgLo;
                    if (!usedStack && iReg < regs.Length)
                    {
                        stgLo = regs[iReg];
                        ++iReg;
                        bitsRemaining -= 16;
                    }
                    else
                    {
                        usedStack = true;
                        stgLo = ccr.AllocateStackSlot(dtParam);
                        bitsRemaining = 0;
                        ccr.StackParam(dtParam);
                        continue;
                    }
                    Storage stgHi;
                    if (!usedStack && iReg < regs.Length)
                    {
                        stgHi = regs[iReg];
                        ++iReg;
                        bitsRemaining -= 16;
                    }
                    else
                    {
                        usedStack = true;
                        var dtRemaining = PrimitiveType.CreateWord(bitsRemaining);
                        stgHi = ccr.AllocateStackSlot(dtRemaining);
                    }
                    ccr.SequenceParam(stgHi, stgLo);
                }
                else
                {
                    ccr.StackParam(dtParam);
                    iReg = regs.Length;
                }
            }
        }

        public bool IsArgument(Storage stg)
        {
            throw new NotImplementedException();
        }

        public bool IsOutArgument(Storage stg)
        {
            throw new NotImplementedException();
        }
    }
    /*
R0 PC N/A Program Counter
R1 SP yes Call Stack Pointer
R2 SR N/A Status Register
R3 CG N/A Constant Generator Register
R4 yes
R5 yes
R6 yes
R7 yes
R8 yes function argument (special case)
R9 yes function argument (special case)
R10 yes function argument (special case)
R11 no function argument (special case)
R12 no function argument, return value
R13 no function argument, return value
R14 no function argument, return value
R15 no function argument, return value
    */
}
