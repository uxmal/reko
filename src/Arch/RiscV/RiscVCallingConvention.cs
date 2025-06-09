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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Machine;

namespace Reko.Arch.RiscV
{
    /*
    The RISC-V calling convention passes arguments in registers when possible.
    Up to eight integer registers, a0–a7, and up to eight floating-point 
    registers, fa0–fa7, are used for this purpose. If the arguments to a 
    function are conceptualized as fields of a C struct, each with pointer 
    alignment, the argument registers are a shadow of the first eight pointer-
    words of that struct. If argument i < 8 is a floating-point type, it is 
    passed in floating-point register fai; otherwise, it is passed in integer
    register ai. However, floating-point arguments that are part of unions or
    array fields of structures are passed in integer registers. Additionally,
    floating-point arguments to variadic functions (except those that are
    explicitly named in the parameter list) are passed in integer registers.

    Arguments smaller than a pointer-word are passed in the least-significant
    bits of argument registers. Correspondingly, sub-pointer-word arguments 
    passed on the stack appear in the lower addresses of a pointer-word, since
    RISC-V has a little-endian memory system. When primitive arguments twice
    the size of a pointer-word are passed on the stack, they are naturally
    aligned. When they are passed in the integer registers, they reside in an
    aligned even-odd register pair, with the even register holding the least-
    significant bits. In RV32, for example, the function
        void foo(int, long long) 
    is passed its first argument in a0 and its second in a2 and a3. Nothing is
    passed in a1.

    Arguments more than twice the size of a pointer-word are passed by
    reference. The portion of the conceptual struct that is not passed in 
    argument registers is passed on the stack. The stack pointer sp points to
    the first argument not passed in a register. Values are returned from 
    functions in integer registers a0 and a1 and floating-point registers fa0
    and fa1. Floating-point values are returned in floating-point registers 
    only if they are primitives or members of a struct consisting of only one
    or two floating-point values. Other return values that fit into two
    pointer-words are returned in a0 and a1. Larger return values are passed 
    entirely in memory; the caller allocates this memory region and passes a
    pointer to it as an implicit first parameter to the callee.

    In the standard RISC-V calling convention, the stack grows downward and 
    the stack pointer is always kept 16-byte aligned.

    In addition to the argument and return value registers, seven integer 
    registers t0–t6 and twelve floating-point registers ft0–ft11 are temporary
    registers that are volatile across calls and must be saved by the caller
    if later used. Twelve integer registers s0–s11 and twelve floating-point
    registers fs0–fs11 are preserved across calls and must be saved by the
    callee if used. 
     */
    public class RiscVCallingConvention : AbstractCallingConvention
    {
        private readonly IProcessorArchitecture arch;
        private readonly RegisterStorage[] iregs;
        private readonly RegisterStorage[] fregs;
        private int floatAbi;

        public RiscVCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.iregs = new[] { "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            this.fregs = new[] { "fa0", "fa1", "fa2", "fa3", "fa4", "fa5", "fa6", "fa7" }
                .Select(r => arch.GetRegister(r)!)
                .ToArray();
            if (((ProcessorArchitecture)arch).Options.TryGetValue("FloatAbi", out var oFloatAbi) &&
                oFloatAbi is int floatAbi)
            {
                this.floatAbi = floatAbi;
            }
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.WordWidth.Size, 0);
            if (dtRet is not null)
            {
                var pt = dtRet.ResolveAs<PrimitiveType>();
                if (pt is not null && pt.Domain == Domain.Real && this.floatAbi > 0)
                {
                    //$TODO floats > 64 bits
                    ccr.RegReturn(fregs[0]);
                }
                else
                {
                    if (dtRet.Size <= arch.PointerType.Size)
                    {
                        ccr.RegReturn(iregs[0]);
                    }
                    else if (dtRet.Size <= arch.PointerType.Size * 2)
                    {
                        ccr.SequenceReturn(iregs[1], iregs[0]);
                    }
                    else
                        //$TODO: return values > 128 bits.
                        throw new NotImplementedException("Return values > 128 bits not supported yet.");
                }
            }
            int ir = 0;
            for (int i = 0; i < dtParams.Count; ++i)
            {
                var dtParam = dtParams[i];
                var pt = dtParam .ResolveAs<PrimitiveType>();
                if (pt is not null && pt.Domain == Domain.Real && floatAbi > 0)
                {
                    if (ir >= fregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                    {
                        ccr.RegParam(fregs[ir]);
                        ++ir;
                    }
                }
                else if (ir >= iregs.Length)
                {
                    ccr.StackParam(dtParam);
                }
                else if (dtParam.Size <= arch.PointerType.Size)
                {
                    ccr.RegParam(iregs[ir]);
                    ++ir;
                }
                else if (dtParam.Size <= arch.PointerType.Size * 2)
                {
                    if ((ir & 1) != 0)
                        ++ir;
                    if (ir >= iregs.Length)
                    {
                        ccr.StackParam(dtParam);
                    }
                    else
                    {
                        ccr.SequenceParam(iregs[ir + 1], iregs[ir]);
                        ir += 2;
                    }
                }
                else
                    throw new NotImplementedException();
            }
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
            return iregs[0] == stg || iregs[1] == stg &&
                   fregs[0] == stg || fregs[1] == stg;
        }
    }
}
