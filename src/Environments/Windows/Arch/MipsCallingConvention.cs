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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Seralizes and deserializes MIPS signatures on Windows. 
    /// </summary>
    /// <remarks>
    /// According to Microsoft:
    /// The calling function allocates space on the stack for all
    /// arguments, even though it may pass some of the arguments in registers. 
    /// The calling function should reserve enough space on the stack for the 
    /// maximum argument list required by calls from the calling function.
    /// The function must allocate space for at least four words, even if it passes
    /// fewer parameters.
    /// Functions should allocate space for all arguments, regardless of whether
    ///  the function passes the arguments in registers.This provides a save area f
    /// or the called function for saving argument registers if these registers need 
    /// to be preserved.
    /// The function allocates argument registers for the first argument.It 
    /// allocates any argument registers remaining to the second argument, and so
    ///  on until it uses all the argument registers or exhausts the argument list. 
    /// All remaining parts of an argument and remaining arguments go on the stack.
    /// When thinking about argument register allocation, imagine the argument list
    /// as an unpacked structure in memory — the argument call area on the stack — 
    /// where each argument is an appropriately aligned member of the structure.
    /// The argument register allocation preserves that same alignment as if in 
    /// memory. When mapping some argument lists some argument registers may not
    /// contain anything relevant the same as padding space in memory.
    /// </remarks>
    // https://msdn.microsoft.com/en-us/library/aa448706.aspx
    public class MipsCallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs = { };
        private RegisterStorage iretLo;
        private RegisterStorage iretHi;
        private RegisterStorage fret;

        public MipsCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.iregs = new[] { "r4", "r5", "r6", "r7" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.fregs = new[] { "f12", "f13", "f14", "f15" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.iretLo = arch.GetRegister("r2")!;
            this.iretHi = arch.GetRegister("r3")!;
            this.fret = arch.GetRegister("f0")!;
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.WordWidth.Size, 0x10);
            if (dtRet is not null)
            {
                SetReturnRegister(ccr, dtRet);
            }

            int ir = 0;
            int fr = 0;
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtArg = dtParams[iArg];
                var prim = dtArg as PrimitiveType;
                if (prim is not null && prim.Domain == Domain.Real)
                {
                    if (fr >= fregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                       ccr.RegParam(fregs[fr]);
                       ++fr;
                    }
                }
                else if (dtArg.Size <= 4)
                {
                    if (ir >= iregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(iregs[ir]);
                        ++ir;
                    }
                }
                else
                {
                    int regsNeeded = (dtArg.Size + 3) / 4;
                    if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else if (regsNeeded == 2)
                    {
                        ccr.SequenceParam(iregs[ir], iregs[ir + 1]);
                        ir += 2;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public void SetReturnRegister(ICallingConventionBuilder ccr, DataType dtArg)
        {
            int bitSize = dtArg.BitSize;
            var pt = dtArg as PrimitiveType;
            if (pt is not null && pt.Domain == Domain.Real)
            {
                if (bitSize > 64)
                    throw new NotImplementedException();
                ccr.RegReturn(fret);
                return;
            }
            if (bitSize <= 32)
            {
                ccr.RegReturn(iretLo);
                return;
            }
            if (bitSize <= 64)
            {
                ccr.SequenceReturn(iretHi, iretLo);
                return;
            }
            throw new NotImplementedException();
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return iregs.Contains(reg) || fregs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return reg == iretLo || reg == iretHi ||
                    reg == fret;
            }
            return false;
        }
    }
}
