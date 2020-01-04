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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;

namespace Reko.Environments.SysV.ArchSpecific
{
    /// <summary>
    /// For MSP430 and MSP430X, the ABI designates R4-R10 as callee-saved
    /// registers. That is, a called function is expected to preserve them
    /// so they have the same value on return from a function as they had
    /// at the point of the call.
        /*
All other
registers
are
caller-save
registers.
That
is, they are not preserved
across
a call, so if their
value
is
needed
following
the call, the caller
is responsible
for saving
and restoring
their
contents.

For MSP430/430X,
up to four arguments
to a function
are passed
in registers.
The number
of arguments
passed
in registers
depends
on the size and type of each
argument.
Arguments
are assigned,
in declared
order,
to the first available
register
single,
pair,
or quad
from
the following
list
into which
it fits (with
the special
case
exceptions
noted
below).
For MSP430
and MSP430X,
the argument
registers
are:
R12,
R13,
R14,
R15
The size of the CPU
registers
is different
on each
architecture.
The ways
the argument
registers
are used
differs
accordingly.
A single
register
cannot
contain
multiple
arguments.
A compiler
may promote
arguments
with a type
smaller
than
int (16 bits) to the size of a register
when
they are passed
in a register.
The TI compiler
does
promote
such
arguments.
Note
that the TI compiler
does
not promote
such
arguments
if they are passed
on the stack,
unless
the arguments
are passed
to a variadic
function
or a prototype
is not in scope
(default
argument
promotions).
When
a narrow
value
is to be passed
in a register,
the caller
is responsible
for
correctly
sign-
or zero-extending
it to fill the register
width.

        Arguments
with a type that fits in a single
CPU
register
are passed
in a single
CPU
register.
For MSP430
and MSP430X,
types
up to 16 bits are passed
in a single
register.
Pointer
types
are also
passed
in a single
register,
regardless
of size.
For MSP430X,
pointer
types
can be 20 bits when
using
large
code
or large
data
memory
models,
but CPU
registers
are also 20 bits, so pointer
values
always
fit in a single
register.
For non-pointer
values,
MSP430X
CPU
registers
are treated
as if they had only 16 bits.A consequence
of this is that the registers
used
to implement
argument
passing
are the same
for MSP430
and MSP430X,
regardless
of the memory
model
used.

    Arguments
with a type that is larger
than
a single
register,
but no larger
than
twice
the size of a single
register,
are passed
in a register
pair.
The lowest-numbered
register
holds
the LSW
(least
significant
word).
For MSP430
and MSP430X,
register
pairs
do not need
to be aligned,
so R12:R13,
R13:R14,
and R14:R15
are the valid
register
pairs.
Types
up to 32 bits are passed
in a register
pair.
This includes
"long
int", "float"
and structs
of up to 32 bits size passed
by value.

        For MSP430
and MSP430X,
a 32-bit
argument
may be split between
the stack
and memory.
If an
argument
would
be passed
in a register
pair,
but only one register
is available
(always
R15),
the compiler
will split the argument
between
R15 and one register-sized
spot on the stack.

        For MSP430
and MSP430X,
arguments
whose
size is greater
than
32 bits and up to 64 bits use register
quads.
For example,
R8::R11
is the notation
used
in this manual
for a register
quad
consisting
of the
registers
R8, R9, R10,
and R11 in sequence.
The lowest-numbered
register
holds
the LSW.
Register
quads
must
be aligned.
Therefore,
only R8::R11
and R12::R15
are valid
register
quads.
Register
quads
require
special
handling.
If there
are enough
argument
registers
remaining
(all four)
to pass
the 64-bit
value,
all four will be used.
Otherwise,
the 64-bit
value
will be passed
entirely
on the stack.
This may leave
unused
argument
registers
(in other
words,
a "hole").
The calling
convention
will try to back-fill
the hole with subsequent
arguments,
but only if they fit entirely
in registers
(see
example
2 that follows).
That
is, after
any argument
has been
placed
on the stack,
no argument
will be placed
in a "split
pair".

3.3.6
C++ Argument
Passing
In C++,
the "this"
pointer
is passed
to non-static
member
functions
in R12 as an implicit
first argument.
(If
a non-static
member
function
returns
a struct
by reference,
the order
is "&struct",
"this".)
3.3.7
Passing
Structs
and Unions
Structures
and unions
larger
than
32 bits are passed
by reference,
as described
in Section
3.5.
3.3.8
Stack
Layout
of Arguments
Not Passed
in Registers
Any arguments
not passed
in registers
are placed
on the stack
at increasing
addresses,
starting
at 0(SP).
Each
argument
is placed
at the next available
address
correctly
aligned
for its type,
subject
to the
following
additional
considerations:
•  The stack
alignment
of a scalar
is that of its declared
type.
•  Regardless
of the alignment
required
by its members,
the stack
alignment
of a structure
passed
by
value
is the smallest
power
of two greater
than
or equal
to its size.
(This
cannot
exceed
2 bytes,
which
is the largest
allowable
size for a structure
passed
by value).
This is to allow
loading
arguments
with
aligned
loads,
even
if the type is not naturally
aligned
strictly
enough,
which
might
be the case
with
struct
of size 32 containing
an array
of char.
•  Each
argument
reserves
an amount
of stack
space
equal
to its size rounded
up to the next multiple
of
its stack
alignment.
For a variadic
C function
(that
is, a function
declared
with an ellipsis
indicating
that it is called
with varying
numbers
of arguments),
the last explicitly
declared
argument
and all remaining
arguments
are passed
on
the stack,
so that its stack
address
can act as a reference
for accessing
the undeclared
arguments.
Undeclared
scalar
arguments
to a variadic
function
that are smaller
than
int are promoted
to and passed
as int, in accordance
with the C language.
Alignment
"holes"
can occur
between
arguments
passed
on the stack,
but "back-fill"
does
not occur.
3.3.9
Frame
Pointer
MSP430
does
not use a frame
pointer.
This effectively
limits
a single
call frame
to 0x7fff
bytes,
which
is
the minimum
SP offset
supported
by any instruction.

Return
Values
The function
return
value
is placed
in the same
register
as the usual
first argument
register,
based
on its
type and size.
For MSP430 and MSP430X, the return value is placed in R12, R12:13, or R12::R15.
Return
values
with a
type that fits in a single
CPU
register
are placed
in R12;
this includes
pointer
types.
32-bit
return
values
are placed
in R12:R13.
64-bit
return
values
are placed
R12::R15.
The LSW
is always
in R12.
Aggregates
larger
than
32 bits are returned
by reference.
3.5
Structures
or Unions
Passed
and Returned
by Reference
Structures
(including
classes)
and unions
larger
than
32 bits are passed
and returned
by reference.
To pass a structure or union by reference, the caller places its address in
the appropriate location: either in a register or on the stack, according
to its position in the argument list. To preserve pass-by-value semantics
(required for C and C++), the callee may need to make its own copy of the
pointed-to object. In some cases, the callee need not make a copy, such as if
the callee is a leaf and it does not modify the pointed-to object.
If the called
function
returns
a structure
or union
larger
than
32 bits, the caller
must
pass
an additional
argument
containing
a destination
address
for the returned
value,
or NULL
if the returned
value
is not
used.
This additional
argument
is passed
in the first argument
register
as an implicit
first argument.
The callee
returns
the object
by copying
it to the given
address.
The caller
is responsible
for allocating
memory
if
required.
Typically
this involves
reserving
space
on the stack,
but in some
cases
the address
of an
already-existing
object
can be passed
and no allocation
is required.
For example,
if f returns
a structure,
the assignment
s = f() can be compiled
by passing
&s in the first argument
register*/
    /// </summary>
    public class Msp430CallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;

        public Msp430CallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new[] { "r12", "r13", "r14", "r15" }
                .Select(r => arch.GetRegister(r)).ToArray();
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(2, 0);
            if (dtRet != null && !(dtRet is VoidType))
            {
                if (dtRet.Size <= 2)
                    ccr.RegReturn(iregs[0]);
                else if (dtRet. Size <= 4)
                    ccr.SequenceReturn(iregs[1], iregs[0]);
                else
                    throw new NotImplementedException();
            }

            int iReg = 0;
            for (int i = 0; i < dtParams.Count; ++i)
            {
                //$BUG: clearly not correct yet, but we need to start somewhere.
                if (iReg < iregs.Length)
                {
                    ccr.RegParam(iregs[iReg]);
                    ++iReg;
                }
                else
                {
                    ccr.StackParam(dtParams[i]);
                }
            }
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg);
            }
            return false;
        }
    }
}
