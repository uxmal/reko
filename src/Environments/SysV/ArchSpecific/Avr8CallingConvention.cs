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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Lib;
using Reko.Core.Machine;

namespace Reko.Environments.SysV.ArchSpecific
{
    /*
     * https://gcc.gnu.org/wiki/avr-gcc
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
    public class Avr8CallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] argRegs;

        public Avr8CallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.argRegs = (from reg in arch.GetRegisters()
                     where 8 <= reg.Number && reg.Number <= 26
                     orderby reg.Number
                     select reg).ToArray();
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            /*
             * To find the register where a function argument is passed, initialize the register number 
Rn with R26 and follow this procedure: 

If the argument size is an odd number of bytes, round up the size to the next even number. 
Subtract the rounded size from the register number Rn. 

If the new Rn is at least R8 and the size of the object is non-zero, then the low-byte of 
the argument is passed in Rn. Subsequent bytes of the argument are passed in the subsequent
registers, i.e. in increasing register numbers. 

If the new register number Rn is smaller than R8 or the size of the argument is zero, the
argument will be passed in memory. 

If the current argument is passed in memory, stop the procedure: All subsequent arguments
will also be passed in memory. 
If there are arguments left, goto 1. and proceed with the next argument.

Return values with a size of 1 byte up to and including a size of 8 bytes will be returned
in registers. Return values whose size is outside that range will be returned in memory. 
If a return value cannot be returned in registers, the caller will allocate stack space and
pass the address as implicit first pointer argument to the callee. The callee will put the 
return value into the space provided by the caller. 

If the return value of a function is returned in registers, the same registers are used as 
if the value was the first parameter of a non-varargs function. For example, an 8-bit value is returned in R24 and an 32-bit value is returned R22...R25. 
Arguments of varargs functions are passed on the stack. This applies even to the named arguments. 
*/
            ccr.LowLevelDetails(1, 2);

            if (dtRet is not null && dtRet != VoidType.Instance)
            {
                GenerateReturnValue(dtRet, ccr);
            }

            int iReg = 26;
            foreach (var dtParam in dtParams)
            {
                int size = dtParam.Size;
                if ((size & 1) != 0) // odd sized register occupies two regs
                {
                    // Round size to even # of bytes.
                    size = dtParam.Size + 1;
                }
                iReg -= size;
                if (iReg >= 8)
                {
                    var reg = argRegs[iReg - 8];
                    if (dtParam.Size == 1)
                    {
                        ccr.RegParam(reg);
                        continue;
                    }

                    SequenceStorage? seq = null;
                    for (int r = iReg + 1, i = 1; i < dtParam.Size; ++i, ++r)
                    {
                        var regNext = argRegs[r - 8];
                        if (seq is not null)
                        {
                            seq = new SequenceStorage(PrimitiveType.CreateWord(regNext.DataType.BitSize + seq.DataType.BitSize), regNext, seq);
                        }
                        else
                        {
                            seq = new SequenceStorage(PrimitiveType.CreateWord(regNext.DataType.BitSize + reg.DataType.BitSize), regNext, reg);
                        }
                    }
                    ccr.SequenceParam(seq!);
                }
                else
                {
                    ccr.StackParam(dtParam);
                }
            }
     
        }

        private void GenerateReturnValue(DataType dtRet, ICallingConventionBuilder ccr)
        {
            int size = dtRet.Size;
            if ((size & 1) != 0) // odd sized register occupies two regs
            {
                // Round size to even # of bytes.
                size = dtRet.Size + 1;
            }

            var iReg = 26 - size;
            if (dtRet.Size <= 8)
            {
                var reg = argRegs[iReg - 8];
                if (dtRet.Size == 1)
                {
                    ccr.RegReturn(reg);
                    return;
                }

                var retRegs = new List<RegisterStorage> { reg };
                for (int r = iReg + 1, i = 1; i < dtRet.Size; ++i, ++r)
                {
                    var regNext = argRegs[r - 8];
                    retRegs.Insert(0, regNext);
                }
                var seq = new SequenceStorage(retRegs.ToArray());
                ccr.SequenceReturn(seq);
            }
            else
            {
                throw new NotImplementedException("Large AVR8 return values not implemented yet.");
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs.Contains(reg);
            }
            //$TODO: stack vars?
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                var n = reg.Number;
                return 18 <= n && n <= 25;
            }
            return false;
        }
    }
}
