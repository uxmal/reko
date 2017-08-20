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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.Environments.SysV.ArchSpecific
{
    /*
     * http://www.atmel.com/webdoc/AVRLibcReferenceManual/FAQ_1faq_reg_usage.html
     * 
     * What registers are used by the C compiler?

    Data types: char is 8 bits, int is 16 bits, long is 32 bits, long long is
    64 bits, float and double are 32 bits (this is the only supported floating
    point format), pointers are 16 bits (function pointers are word addresses,
    to allow addressing up to 128K program memory space). There is a -mint8
    option (see Options for the C compiler avr-gcc) to make int 8 bits, but
    that is not supported by avr-libc and violates C standards (int must be at
    least 16 bits). It may be removed in a future release.

    Call-used registers (r18-r27, r30-r31): May be allocated by gcc for local
    data. You may use them freely in assembler subroutines. Calling C 
    subroutines can clobber any of them - the caller is responsible for saving
    and restoring.

    Call-saved registers (r2-r17, r28-r29): May be allocated by gcc for local
    data. Calling C subroutines leaves them unchanged. Assembler subroutines
    are responsible for saving and restoring these registers, if changed.
    r29:r28 (Y pointer) is used as a frame pointer (points to local data on
    stack) if necessary. The requirement for the callee to save/preserve the
    contents of these registers even applies in situations where the compiler
    assigns them for argument passing.

    Fixed registers (r0, r1): Never allocated by gcc for local data, but often
    used for fixed purposes: 

r0 - temporary register, can be clobbered by any C code (except interrupt
handlers which save it), may be used to remember something for a while within
one piece of assembler code

r1 - assumed to be always zero in any C code, may be used to remember
something for a while within one piece of assembler code, but must then be
cleared after use (clr r1). This includes any use of the [f]mul[s[u]] 
instructions, which return their result in r1:r0. Interrupt handlers save
and clear r1 on entry, and restore r1 on exit (in case it was non-zero).

    Function call conventions: Arguments - allocated left to right, r25 to r8.
    All arguments are aligned to start in even-numbered registers (odd-sized
    arguments, including char, have one free register above them). This allows
    making better use of the movw instruction on the enhanced core. 

If too many, those that don't fit are passed on the stack.

Return values: 8-bit in r24 (not r25!), 16-bit in r25:r24, up to 32 bits in
r22-r25, up to 64 bits in r18-r25. 8-bit return values are zero/sign-extended
to 16 bits by the called function (unsigned char is more efficient than signed
char - just clr r25). Arguments to functions with variable argument lists
(printf etc.) are all passed on stack, and char is extended to int. 
*/
    public class Avr8CallingConvention : CallingConvention
    {
        public Avr8CallingConvention(IProcessorArchitecture arch)
        {
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            throw new NotImplementedException();
        }
    }
}
