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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class CSkyCallingConvention : CallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;

        public CSkyCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = new string[] { "r0", "r1", "r2", "r3" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
        }

        /*
The C-SKY V2 CPU uses four registers (r0–r3) to pass the first four words of arguments from the caller to
the called routine. If additional argument space is required, the caller is responsible for allocating this space
on the stack. This space (if needed by a particular caller) is typically allocated upon entry to a subroutine,
reused for each of the calls made from that subroutine that have more arguments than fit into the four
registers used for subroutine calls, and deallocated only at the caller’s exit point. All argument overflow
allocation and deallocation is the responsibility of the caller.
At entry to a subroutine, the first word of any argument overflow can be found at the address contained in
the stack pointer. Subsequent overflow words are located at successively larger addresses.
2.2.3.1 Scalar Arguments
Arguments are passed using registers r0 through r3, with no more than one argument assigned per register.
Argument values that are smaller than a 32-bit register occupy a full register.
In addition, small argument values are right justified and possibly extended within the register. Small
signed arguments (e.g., shorts) are sign extended; small unsigned arguments (e.g., unsigned shorts) are zero
extended, while other small values (e.g., structures of less than four bytes) are not extended, leaving the upper
bits of the register undefined. The caller is responsible for sign and zero extensions. Small arguments that
are passed via the argument overflow mechanism are placed in the overflow word with the same orientation
they would have if passed in a register; a char is passed in the low-order byte of an overflow word. Such
small overflow arguments need not be sign extended within the argument word as they would be if passed
in a register. Arguments larger than a register must be assigned to multiple argument registers as long as
there are argument registers available. Arguments that would be aligned on 4-byte boundaries in memory
(double, long double, long long, or structures or unions containing a double, long double or long long) can
begin in any numbered register. Once all the argument registers are used, or if there are not enough registers
left to hold a large argument, the argument and any subsequent arguments must be placed in the overflow
area described above.
Large arguments can be split in register and in the overflow area when there are too few argument registers
to hold the entire argument.
The caller is responsible for allocating argument overflow space and for deallocating any space needed for
argument overflow. The only argument space that may be allocated or deallocated by the called routine
is space used to place the register arguments in memory. This may be necessary for stdargs or structure
parameters. Alignment is forced for atomic data types; fundamental data types are not split.
2.2.3.2 Structure Arguments
Structures passed as arguments can be partially or wholly passed through the argument registers. A structure
argument may overflow onto the stack only when all argument registers are full. In these cases, the caller
must adjust the stack pointer to allocate theoverflow area.
Structure arguments that are smaller than 32 bits have their value right justified within the argument register.
The unused upper bits within the register are undefined.
Structure arguments larger than 32 bits are packed into consecutive registers. Structures that are not integral
multiples of 32 bits in size have their final bits left justified within the appropriate register. This allows those
bits to be stored with a 32-bit operation and be adjacent to the preceding portion of the structur  


2.2.5 Return Values
2.2.5.1 Scalar Values
Subroutines return values in the argument registers. Return values smaller than 32 bits occupy a full register.
These must be right justified and zero or sign extended to 32 bits before return (refer to “Scalar Arguments”).
Return values of 32 bits or fewer are returned in register r0.
Return values between 33 and 64 bits are returned in the register pair r0/r1. The portion of the data
that would reside at a lower address if stored in memory is in r0. For example, r0 would contain the most
significant 32 bits of the long long data type.
Return values larger than eight bytes are treated as structure return values and are returned through memory.
The return value is placed in a caller-supplied buffer. The buffer address is passed from the caller to the
called routine as a hidden first argument in register r0.
2.2.5.2 Structure Values
Structures can be returned in one of two ways. Small structures (eight bytes or fewer) are returned in the
register pair r0/r1. If the structure consists of four or fewer bytes, the value is returned in r0, right justified.
This matches the way it would be justified when passed as an argument. If the structure consists of five to
eight bytes, the first four bytes are returned in r0 and the trailing portion of the structure is returned left
justified in r1.
This alignment is chosen to generate good code for code sequences such as
wom(..., bat(), ...)
where wom takes a structure argument of the same type returned by bat. The only work required is to
perhaps change registers if the call to wom has the structure in some place other than r0/r1.
Structures larger than eight bytes are placed in a buffer provided by the caller. The caller must provide with
a buffer with sufficient size. The buffer is typically allocated on the stack, in order to provide re-entrancy
and to avoid any race conditions where a static buffer may be overwritten. The address of the buffer is
passed to the called function as a hidden first argument and assigned in register r0. The normal arguments
start in register r1 instead of in r0, restricted by as same constraints as fundamental data type.
The caller must provide this buffer for large structures even when the caller does not use the return value
(e.g., the function was called to achieve a side-effect). The called routine can thus assume that the buffer
pointer is valid and need not validate the pointer value passed in r0.
When r0 is used to pass a buffer address, the called routine must preserve the value passed through r0. The
caller can thus assume that r0 is preserved when the buffer address of a large structure is passed in r0. This
is similar to the way where strcat and memcpy return their respective destination addresses.
In generaly, the temporary buffer, used for such structure returns, is immediately used as a source for a
memcpy to a final destination. For example, the sequence
struct s {...}s, sfunc();
s = sfunc();
will often be compiled with sfunc returning into a temporary buffer, which is immediately copied into s.
Although the caller must know the address of the temporary buffer so as to supply it for the called routine,
the address need not be recalculated. In turn, the called routine can use the address to copy the results into
the temporary buffer using memcpy, which returns the destination address (e.g., r0 has the desired value),
or passes it to in-line code which uses r0 as a base register.
*/
        public void Generate(ICallingConventionEmitter ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 16);
            int iReg = 0;
            foreach (var dtParam in dtParams)
            {
                if (iReg < iregs.Length)
                {
                    ccr.RegParam(iregs[iReg++]);
                }
                else
                {
                    ccr.StackParam(dtParam);
                }
            }
            if (dtRet is not null)
            {
                //$BUGBUG: 64-bit return value
                ccr.RegReturn(iregs[0]);
            }
        }

        public bool IsArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }

        public bool IsOutArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }
    }
}