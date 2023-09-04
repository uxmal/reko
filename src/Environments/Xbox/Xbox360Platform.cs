#region License
/* 
 * Copyright (C) 2018-2023 Stefano Moioli <smxdev4@gmail.com>.
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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Reko.Core.Loading;
using Reko.Core.Machine;

namespace Reko.Environments.Xbox
{
    /// <remarks>
    /// A particularity of this platform is that even though the PowerPC 
    /// processor is a 64-bit CPU, all pointers are by convention 32-bit.
    /// This means that special care has to be taken to make sure no
    /// 64-bit pointers or addresses are introduced into the 
    /// decompilation process.
    /// </remarks>
    public class Xbox360Platform : Platform
    {
        public Xbox360Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "xbox360")
        {
            EnsureTypeLibraries(this.PlatformIdentifier);
            this.StructureMemberAlignment = 8;
            //$TODO: find out what registers are always trashed
        }

        public override string DefaultCallingConvention => "";

        public override PrimitiveType PointerType { get { return PrimitiveType.Ptr32; } }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            // Right now we are not aware of any way to locate WinMain
            // on Xbox360 binaries.
            return null;
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            //$TODO: investigate whether the calling
            // convention on Xbox deviates from the convention
            // specified by the PowerPC specs.
            // https://github.com/OGRECave/ogitor/blob/master/Dependencies/Angelscript/source/as_callfunc_xenon.cpp
#if XBOX_SPEC
// XBox 360 calling convention
// ===========================
// I've yet to find an official document with the ABI for XBox 360, 
// but I'll describe what I've gathered from the code and tests
// performed by the AngelScript community.
//
// Arguments are passed in the following registers:
// r3  - r10   : integer/pointer arguments (each register is 64bit)
// fr1 - fr13  : float/double arguments    (each register is 64bit)
// 
// Arguments that don't fit in the registers will be pushed on the stack.
// 
// When a float or double is passed as argument, its value will be placed
// in the next available float register, but it will also reserve general
// purpose register. 
// 
// Example: void foo(float a, int b). a will be passed in fr1 and b in r4.
//
// For each argument passed to a function an 8byte slot is reserved on the 
// stack, so that the function can offload the value there if needed. The
// first slot is at r1+20, the next at r1+28, etc.
//
// If the function is a class method, the this pointer is passed as hidden 
// first argument. If the function returns an object in memory, the address
// for that memory is passed as hidden first argument.
//
// Return value are placed in the following registers:
// r3  : integer/pointer values
// fr1 : float/double values
//
// Rules for registers
// r1          : stack pointer
// r14-r31     : nonvolatile, i.e. their values must be preserved
// fr14-fr31   : nonvolatile, i.e. their values must be preserved
// r0, r2, r13 : dedicated. I'm not sure what it means, but it is probably best not to use them
//
// The stack pointer must always be aligned at 8 bytes
#endif
            return new PowerPcCallingConvention((PowerPcArchitecture)Architecture);
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            // pointers are 32-bit on this 64-bit platform.
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32((uint)uAddr);
        }

        public override bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(sAddress, out addr);
        }
    }
}
