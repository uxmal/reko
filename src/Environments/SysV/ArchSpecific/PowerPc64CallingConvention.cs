#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Xml.Schema;

namespace Reko.Environments.SysV.ArchSpecific
{

    /*
     * From https://refspecs.linuxfoundation.org/ELF/ppc64/PPC-elf64abi.html#RELOC-TYPE
     
3.2.2. The Stack Frame
In addition to the registers, each function may have a stack frame on the runtime stack. This stack grows downward from high addresses. The following figure shows the stack frame organization. SP in the figure denotes the stack pointer (general purpose register r1) of the called function after it has executed code establishing its stack frame.

Figure 3-17. Stack Frame Organiztion

High Address

          +-> Back chain
          |   Floating point register save area
          |   General register save area
          |   VRSAVE save word (32-bits)
          |   Alignment padding (4 or 12 bytes)
          |   Vector register save area (quadword aligned)
          |   Local variable space
          |   Parameter save area    (SP + 48)
          |   TOC save area          (SP + 40)
          |   link editor doubleword (SP + 32)
          |   compiler doubleword    (SP + 24)
          |   LR save area           (SP + 16)
          |   CR save area           (SP + 8)
SP  --->  +-- Back chain             (SP + 0)

Low Address
The following requirements apply to the stack frame:

The stack pointer shall maintain quadword alignment.

The stack pointer shall point to the first word of the lowest allocated stack frame, the "back chain" word. The stack shall grow downward, that is, toward lower addresses. The first word of the stack frame shall always point to the previously allocated stack frame (toward higher addresses), except for the first stack frame, which shall have a back chain of 0 (NULL).

The stack pointer shall be decremented by the called function in its prologue, if required, and restored prior to return.

The stack pointer shall be decremented and the back chain updated atomically using one of the "Store Double Word with Update" instructions, so that the stack pointer always points to the beginning of a linked list of stack frames.

The sizes of the floating-point and general register save areas may vary within a function and are as determined by the traceback table described below.

Before a function changes the value in any nonvolatile floating-point register, frn, it shall save the value in frn in the double word in the floating-point register save area 8*(32-n) bytes before the back chain word of the previous frame. The floating-point register save area is always doubleword aligned. The size of the floating-point register save area depends upon the number of floating point registers which must be saved. It ranges from 0 bytes to a maximum of 144 bytes (18 * 8).

Before a function changes the value in any nonvolatile general register, rn, it shall save the value in rn in the word in the general register save area 8*(32-n) bytes before the low addressed end of the floating-point register save area. The general register save area is always doubleword aligned. The size of the general register save area depends upon the number of general registers which must be saved. It ranges from 0 bytes to a maximum of 144 bytes (18 * 8).

Functions must ensure that the appropriate bits in the vrsave register are set for any vector registers they use. A function that changes the value of the vrsave register shall save the original value of vrsave into the word below the low address end of the general register save area. Below the vrsave save area will be 4 or 12 bytes of alignment padding as needed to ensure that the vector register save area is quadword aligned.

Before a function changes the value in any nonvolatile vector register, vrn, it shall save the value in vrn in the word in the vector register save area 16*(32-n) bytes before the low addressed end of the vrsave save area plus alignment padding. The vector register save area is always quadword aligned. The size of the vector register save area depends upon the number of vector registers which must be saved; it ranges from 0 bytes to a maximum of 192 bytes (12 * 16).

The local variable space contains any local variable storage required by the function. If vector registers are saved the local variable space area will be padded so that the vector register save area is quadword aligned.

The parameter save area shall be allocated by the caller. It shall be doubleword aligned, and shall be at least 8 doublewords in length. If a function needs to pass more than 8 doublewords of arguments, the parameter save area shall be large enough to contain the arguments that the caller stores in it. Its contents are not preserved across function calls.

The TOC save area is used by global linkage code to save the TOC pointer register. See The TOC section later in the chapter.

The link editor doubleword is reserved for use by code generated by the link editor. This ABI does not specify any usage; the AIX link editor uses this space under certain circumstances.

The compiler doubleword is reserved for use by the compiler. This ABI does not specify any usage; the AIX compiler uses this space under certain circumstances.

Before a function calls any other functions, it shall save the value in the LR register in the LR save area.

Before a function changes the value in any nonvolatile field in the condition register, it shall save the values in all the nonvolatile fields of the condition register at the time of entry to the function in the CR save area.

The 288 bytes below the stack pointer is available as volatile storage which is not preserved across function calls. Interrupt handlers and any other functions that might run without an explicit call must take care to preserve this region. If a function does not need more stack space than is available in this area, it does not need to have a stack frame.

The stack frame header consists of the back chain word, the CR save area, the LR save area, the compiler and link editor doublewords, and the TOC save area, for a total of 48 bytes. The back chain word always contains a pointer to the previously allocated stack frame. Before a function calls another function, it shall save the contents of the link register at the time the function was entered in the LR save area of its caller's stack frame and shall establish its own stack frame.

Except for the stack frame header and any padding necessary to make the entire frame a multiple of 16 bytes in length, a function need not allocate space for the areas that it does not use. If a function does not call any other functions and does not require any of the other parts of the stack frame, it need not establish a stack frame. Any padding of the frame as a whole shall be within the local variable area; the parameter save area shall immediately follow the stack frame header, and the register save areas shall contain no padding except as noted for VRSAVE.

3.2.3. Parameter Passing

For a RISC machine such as 64-bit PowerPC, it is generally more efficient to 
pass arguments to called functions in registers (both general and floating-
point registers) than to construct an argument list in storage or to push them
onto a stack. Since all computations must be performed in registers anyway,
memory traffic can be eliminated if the caller can compute arguments into
registers and pass them in the same registers to the called function, where
the called function can then use them for further computation in the same
registers. The number of registers implemented in a processor architecture
naturally limits the number of arguments that can be passed in this manner.

For the 64-bit PowerPC, up to eight doublewords are passed in general purpose
registers, loaded sequentially into general purpose registers r3 through r10.
Up to thirteen floating-point arguments can be passed in floating-point
registers f1 through f13. If VMX is supported, up to twelve vector parameters
can be passed in v2 through v13. If fewer (or no) arguments are passed, the
unneeded registers are not loaded and will contain undefined values on entry
to the called function.

The parameter save area, which is located at a fixed offset of 48 bytes from
the stack pointer, is reserved in each stack frame for use as an argument list.
A minimum of 8 doublewords is always reserved. The size of this area must be
sufficient to hold the longest argument list being passed by the function which
owns the stack frame. Although not all arguments for a particular call are 
located in storage, consider them to be forming a list in this area, with each
argument occupying one or more doublewords.

If more arguments are passed than can be stored in registers, the remaining
arguments are stored in the parameter save area. The values passed on the 
stack are identical to those that have been placed in registers; thus, the
stack contains register images.

For variable argument lists, this ABI uses a va_list type which is a pointer
to the memory location of the next parameter. Using a simple va_list type
means that variable arguments must always be in the same location regardless
of type, so that they can be found at runtime. This ABI defines the location
to be general registers r3 through r10 for the first eight doublewords and the
stack parameter save area thereafter. Alignment requirements such as those for
vector types may require the va_list pointer to first be aligned before
accessing a value.

The rules for parameter passing are as follows:

Each argument is mapped to as many doublewords of the parameter save area as are required to hold its value.

Single precision floating point values are mapped to the second word in a single doubleword.

Double precision floating point values are mapped to a single doubleword.

Extended precision floating point values are mapped to two consecutive doublewords.

Simple integer types (char, short, int, long, enum) are mapped to a single
doubleword. Values shorter than a doubleword are sign or zero extended 
as necessary.

Complex floating point and complex integer types are mapped as if the argument
was specified as separate real and imaginary parts.

Pointers are mapped to a single doubleword.

Vectors are mapped to a single quadword, quadword aligned. This may result in
skipped doublewords in the parameter save area.

Fixed size aggregates and unions passed by value are mapped to as many
doublewords of the parameter save area as the value uses in memory. 
Aggregrates and unions are aligned according to their alignment requirements.
This may result in doublewords being skipped for alignment.

An aggregate or union smaller than one doubleword in size is padded so that
it appears in the least significant bits of the doubleword. All others are 
padded, if necessary, at their tail. Variable size aggregates or unions are
passed by reference.

Other scalar values are mapped to the number of doublewords required by their size.

If the callee has a known prototype, arguments are converted to the type of 
the corresponding parameter before being mapped into the parameter save area.
For example, if a long is used as an argument to a float double parameter, the
value is converted to double-precision and mapped to a doubleword in the
parameter save area.

Floating point registers f1 through f13 are used consecutively to pass up to
13 floating point values, one member aggregates passed by value containing a
floating point value, and to pass complex floating point values. The first 13
of all doublewords in the parameter save area that map floating point
arguments, except for arguments corresponding to the variable argument part of
a callee with a prototype containing an ellipsis, will be passed in floating
point registers. A single precision value occupies one register as does a
double precision value. Extended precision values occupy two consecutively
numbered registers. The corresponding complex values occupy twice as many
registers. Note that for one member aggregates, "containing" extends to
aggregates within aggregates ad infinitum.

Vector registers v2 through v13 are used to consecutively pass up to 12 vector
values, except for arguments corresponding to the variable argument part of a
callee with a prototype containing an ellipsis. As for floating point 
arguments, an aggregate passed by value containing one vector value is treated
as if the value were not wrapped in an aggregate.

If there is no known function prototype for a callee, or if the function
prototype for a callee contains an ellipsis and the argument value is not part
of the fixed arguments described by the prototype, then floating point and
vector values are passed according to the following rules for non-floating,
non-vector types. In the case of no known prototype this may result in two
copies of floating and vector argument values being passed.

General registers are used to pass some values. The first eight doublewords
mapped to the parameter save area correspond to the registers r3 through r10.
An argument other than floating point and vector values fully described by a
prototype, that maps to this area either fully or partially, is passed in the
corresponding general registers.

All other arguments (or parts thereof) not already covered must be stored in 
the parameter save area following the first eight doublewords. The first
eight doublewords mapped to the parameter save area are never stored in the
parameter save area by the calling function.

If the callee takes the address of any of its parameters, then values passed 
in registers are stored into the parameter save area by the callee. If the
compilation unit for the caller contains a function prototype, but the callee
has a mismatching definition, this may result in the wrong values being stored.

Figure 3-18. Parameter Passing

typedef struct {
  int    a;
  double dd;
} sparm;
sparm   s, t;
int     c, d, e;
long double ld;
double  ff, gg, hh;

x = func(c, ff, d, ld, s, gg, t, e, hh);
Parameter     Register     Offset in parameter save area
c             r3           0-7    (not stored in parameter save area)
ff            f1           8-15   (not stored)
d             r5           16-23  (not stored)
ld            f2,f3        24-39  (not stored)
s             r8,r9        40-55  (not stored)
gg            f4           56-63  (not stored)
t             (none)       64-79  (stored in parameter save area)
e             (none)       80-87  (stored)
hh            f5           88-95  (not stored)
Note	Note
 	
If a prototype is not in scope, then the floating point argument ff is also 
passed in r4, the long double argument ld is also passed in r6 and r7, the 
floating point argument gg is also passing in r10, and the floating point
argument gg is also stored into the parameter save area. If a prototype
containing an ellipsis describes any of these floating point arguments as
being part of the variable argument part, then the general registers and
parameter save area are used as when no prototype is in scope, and the
floating point register(s) are not used.

3.2.4. Return Values
Functions shall return float or double values in f1, with float values rounded to single precision.

When the VMX facility is supported, functions shall return vector data type values in v2.

Functions shall return values of type int, long, enum, short, and char, or a
pointer to any type, as unsigned or signed integers as appropriate, zero- or
sign-extended to 64 bits if necessary, in r3. Character arrays of length 8
bytes or less, or bit strings of length 64 bits or less, will be returned
right justified in r3. Aggregates or unions of any length, and character
strings of length longer than 8 bytes, will be returned in a storage buffer
allocated by the caller. The caller will pass the address of this buffer as a
hidden first argument in r3, causing the first explicit argument to be passed
in r4. This hidden argument is treated as a normal formal parameter, and
corresponds to the first doubleword of the parameter save area.

Functions shall return floating point scalar values of size 16 or 32 bytes in
f1:f2 and f1:f4, respectively.

Functions shall return floating point complex values of size 16 (four or eight
byte complex) in f1:f2 and floating point complex values of size 32 (16 byte 
complex) in f1:f4.

 */
    public class PowerPc64CallingConvention : CallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] fregs;
        private readonly RegisterStorage[] iregs;

        public PowerPc64CallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "r3", "r4", "r5", "r6", "r7", "r8", "r9", "r10" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.fregs = new[] { "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
        }

        public void Generate(
            ICallingConventionEmitter ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            //$TODO: full implementation needed. Currently can't handle varargs, args whose bit size > 64, or more than 8 args.
            int stackSaveOffset = 0x48;
            ccr.LowLevelDetails(arch.WordWidth.Size, stackSaveOffset);
            GenerateReturn(ccr, dtRet);
            int iReg = 0;
            int fReg = 0;
            foreach (var dtParam in dtParams)
            {
                if (dtParam.Domain == Domain.Real)
                {
                    ccr.RegParam(fregs[fReg]);
                    ++fReg;
                }
                else
                {
                    if (iReg < iregs.Length)
                    {
                        ccr.RegParam(iregs[iReg]);
                    }
                    else
                    {
                        ccr.StackParam(dtParam);
                    }
                    ++iReg;
                }
            }
        }

        private (int, int)  GenerateReturn(ICallingConventionEmitter ccr, DataType? dtRet)
        {
            if (dtRet is null || dtRet is VoidType)
            {
                return (0, 0);
            }
            if (dtRet.Domain == Domain.Real)
            {
                ccr.RegReturn(fregs[0]);
                return (0, 1);
            }
            else
            {
                ccr.RegReturn(iregs[0]);
                return (1, 0);
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
}
