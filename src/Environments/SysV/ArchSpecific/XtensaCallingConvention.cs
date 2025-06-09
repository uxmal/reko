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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;

// http://wiki.linux-xtensa.org/index.php/ABI_Interface

// Arguments are passed in both registers and memory. The first six incoming
// arguments are stored in registers a2 through a7, and additional arguments
// are stored on the stack starting at the current stack pointer a1. Because
// Xtensa uses register windows that rotate during a function call, outgoing
// arguments that will become the incoming arguments must be stored to 
// different register numbers. Depending on the call instruction and, thus,
// the rotation of the register window, the arguments are passed starting
// starting with register a(2+N), where N is the size of the window rotation.
// Therefore, the first argument in case of a call4 instruction is placed into
// a6, and for a call8 instruction into a10. Large arguments (8-bytes) are
// always passed in an even/odd register pair even if that means to omit a
// register for alignment. The return values are stored in a2 through a5 (so
// a function with return value occupying more than 2 registers may not be
// called with call12).

//           return addr  stack ptr       arg0, arg1, arg2, arg3, arg4, arg5
//           -----------  ---------       ----------------------------------
//             a0           a1              a2,   a3,   a4,   a5,   a6,   a7
// 
// call4       a4           a5              a6,   a7,   a8,   a9,  a10,  a11
// call8       a8           a9             a10,  a11,  a12,  a13,  a14,  a15
// call12     a12          a13             a14,  a15   ---   ---   ---   --- 



// Syscall ABI
// Linux takes system-call arguments in registers.The ABI and Xtensa software
// conventions require the system-call number in a2.For improved efficiency,
// we try not to shift all parameters one register up to maintain the original
// order.Register a2 is, therefore, moved to a6, a6 to a8, and a7 to a9, if
// the system call requires these arguments.

//syscall number arg0, arg1, arg2, arg3, arg4, arg5
//a2             a6,   a3,   a4,   a5,   a8,   a9


namespace Reko.Environments.SysV.ArchSpecific
{
    public class XtensaCallingConvention : AbstractCallingConvention
    {
        private static readonly BitRange r32 = new BitRange(0, 31);

        private IProcessorArchitecture arch;

        public XtensaCallingConvention(IProcessorArchitecture arch) : base("")
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
            ccr.LowLevelDetails(4, 0);
            if (dtRet is not null)
            {
                var a2 = (StorageDomain) 2;
                //$TODO: size > 4 bytes?
                ccr.RegReturn(arch.GetRegister(a2, r32)!);
            }
            int iReg = 2;
            foreach (var dtParam in dtParams)
            {
                //$TODO: size > 4 bytes?
                //$TODO: iReg > 6?
                var arg = (StorageDomain) iReg;
                ccr.RegParam(arch.GetRegister(arg, r32)!);
                ++iReg;
            }
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                //$TODO: need info on how Xtensa passes args.
                return true;
            }
            //$TODO: handle stack args.
            return false;
        }


        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return 2 <= reg.Number && reg.Number <= 5;
            }
            return false;
        }
    }
}