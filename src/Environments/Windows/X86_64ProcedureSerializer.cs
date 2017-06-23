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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Environments.Windows
{
    /*
     * The first four integer arguments are passed in registers. Integer
     *  values are passed (in order left to right) in RCX, RDX, R8, and R9.
     *  Arguments five and higher are passed on the stack. All arguments 
     *  are right-justified in registers. This is done so the callee can
     *  ignore the upper bits of the register if need be and can access 
     *  only the portion of the register necessary.

     * Floating-point and double-precision arguments are passed in XMM0 – XMM3
     * (up to 4) with the integer slot (RCX, RDX, R8, and R9) that would
     * normally be used for that cardinal slot being ignored (see example)
     * and vice versa.

     * __m128 types, arrays and strings are never passed by immediate value
     * but rather a pointer is passed to memory allocated by the caller.
     * Structs/unions of size 8, 16, 32, or 64 bits and __m64 are passed as if
     * they were integers of the same size. Structs/unions other than these
     * sizes are passed as a pointer to memory allocated by the caller. For
     * these aggregate types passed as a pointer (including __m128), 
     * the caller-allocated temporary memory will be 16-byte aligned. 

     * Intrinsic functions that do not allocate stack space and do not call
     * other functions can use other volatile registers to pass additional
     * register arguments because there is a tight binding between the
     * compiler and the intrinsic function implementation. This is a further
     * opportunity for improving performance.

     * A scalar return value that can fit into 64 bits is returned through
     * RAX—this includes __m64 types. Non-scalar types including floats
     * doubles, and vector types such as __m128, __m128i, __m128d are 
     * returned in XMM0. The state of unused bits in the value returned in
     * RAX or XMM0 is undefined.

     * User-defined types can be returned by value from global functions and
     * static member functions. To be returned by value in RAX, user-defined
     * types must have a length of 1, 2, 4, 8, 16, 32, or 64 bits; no user-
     * defined constructor, destructor, or copy assignment operator; no 
     * private or protected non-static data members; no non-static data 
     * members of reference type; no base classes; no virtual functions; and
     * no data members that do not also meet these requirements. (This is 
     * essentially the definition of a C++03 POD type. Because the definition
     * has changed in the C++11 standard, we do not recommend using 
     * std::is_pod for this test.) Otherwise, the caller assumes the 
     * responsibility of allocating memory and passing a pointer for the
     * return value as the first argument. Subsequent arguments are then
     * shifted one argument to the right. The same pointer must be returned
     * by the callee in RAX. 
     */
    public class X86_64ProcedureSerializer : ProcedureSerializer
    {
        private static readonly string[] iregs = new[] { "rcx", "rdx", "r8", "r9" };
        private static readonly string[] fregs = new[] { "xmm0", "xmm1", "xmm2", "xmm3" };

        private ArgumentDeserializer argDeser;

        public X86_64ProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention) 
            : base(arch, typeLoader, defaultConvention)
        {
        }

        public void ApplyConvention(SerializedSignature ssig, FunctionType sig)
        {
            string d = ssig.Convention;
            if (d == null || d.Length == 0)
                d = DefaultConvention;
            sig.StackDelta = Architecture.PointerType.Size;
            if (ssig.StackDelta != 0)
                sig.StackDelta = ssig.StackDelta;
            sig.ReturnAddressOnStack = Architecture.PointerType.Size;
        }

        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            this.argDeser = new ArgumentDeserializer(
                this,
                Architecture,
                frame,
                Architecture.PointerType.Size,
                Architecture.WordWidth.Size);
            Identifier ret = null;

            if (ss.ReturnValue != null)
            {
                ret = argDeser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            int iArg = 0;
            if (ss.EnclosingType != null && ss.IsInstanceMethod)
            {
                var arg = DeserializeImplicitThisArgument(ss);
                args.Add(arg);
                ++iArg;
            }
            if (ss.Arguments != null)
            {
                for (iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                    args.Add(arg);
                }
            }
            var sig = ss.ParametersValid ?
                new FunctionType(ret, args.ToArray()) :
                new FunctionType();
            sig.IsInstanceMetod = ss.IsInstanceMethod;
            ApplyConvention(ss, sig);
            return sig;
        }

        private Identifier DeserializeImplicitThisArgument(SerializedSignature ss)
        {
            var sArg = new Argument_v1
            {
                Type = new PointerType_v1(ss.EnclosingType),
                Name = "this",
                Kind = new Register_v1("rcx")
            };
            var arg = argDeser.Deserialize(sArg);
            return arg;
        }

        /// <summary>
        /// Deserializes an argument.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="idx"></param>
        /// <param name="convention"></param>
        /// <returns></returns>
        public Identifier DeserializeArgument(Argument_v1 arg, int idx, string convention)
        {
            if (arg.Kind != null)
            {
                return argDeser.Deserialize(arg, arg.Kind);
            }
            Debug.Assert(iregs.Length == fregs.Length);
            if (idx < fregs.Length)
            {
                var reg = (IsReal(arg.Type) ? fregs : iregs)[idx];
                return argDeser.Deserialize(arg, new Register_v1(reg));
            }
            if (convention == null)
                return argDeser.Deserialize(arg, new StackVariable_v1());
            switch (convention)
            {
            case "":
            case "stdapi":
            case "cdecl":
            case "__cdecl":
            case "__stdcall":
            case "pascal":
            case "__thiscall":
                return argDeser.Deserialize(arg, new StackVariable_v1 { });
            }
            throw new NotSupportedException(string.Format("Unsupported calling convention '{0}'.", convention));
        }

        protected bool IsReal(SerializedType sType)
        {
            var prim = sType as PrimitiveType_v1;
            if (prim == null)
                return false;
            return prim.Domain == Domain.Real;
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            if (bitSize > 64)
                throw new NotImplementedException("Large return values not implemented yet");
            return Architecture.GetRegister(IsReal(sArg.Type) ? "xmm0" : "rax");
        }
    }
}
