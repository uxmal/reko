#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Serializes and deserializes procedure signatures.
    /// </summary>
    /// <remarks>
    /// This code is aware of the different calling convetions on 
    /// x86 processors. Should your ABI be different, you need to
    /// make the Platform that loaded the binary return an object
    /// that derives from this class.
    /// </remarks>
    public class X86ProcedureSerializer : ProcedureSerializer
    {
        private ArgumentSerializer argser;

        public X86ProcedureSerializer(IntelArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
            : base(arch, typeLoader, defaultCc)
        {
        }

        public void ApplyConvention(SerializedSignature ssig, ProcedureSignature sig)
        {
            string d = ssig.Convention;
            if (d == null || d.Length == 0)
                d = DefaultConvention;
            sig.StackDelta = Architecture.PointerType.Size;  //$BUG: far/near pointers?
            if (d == "stdapi" || d == "__stdcall" || d == "pascal")
                sig.StackDelta += StackOffset;
            sig.FpuStackDelta = FpuStackOffset;
            sig.ReturnAddressOnStack = Architecture.PointerType.Size;   //$BUG: x86 real mode?
        }

        public override ProcedureSignature Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            this.argser = new ArgumentSerializer(this, Architecture, frame, ss.Convention);
            Identifier ret = null;
            int fpuDelta = FpuStackOffset;

            FpuStackOffset = 0;
            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
                fpuDelta += FpuStackOffset;
            }

            FpuStackOffset = 0;
            var args = new List<Identifier>();
            if (ss.Arguments != null)
            {
                if (ss.Convention == "pascal")
                {
                    for (int iArg = ss.Arguments.Length -1; iArg >= 0; --iArg)
                    {
                        var sArg = ss.Arguments[iArg];
                        var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                        args.Add(arg);
                    }
                    args.Reverse();
                }
                else
                {
                    for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                    {
                        var sArg = ss.Arguments[iArg];
                        var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                        args.Add(arg);
                    }
                }
                fpuDelta -= FpuStackOffset;
            }
            FpuStackOffset = fpuDelta;
            var sig = new ProcedureSignature(ret, args.ToArray());
            ApplyConvention(ss, sig);
            return sig;
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
                return argser.Deserialize(arg, arg.Kind);
            }
            if (convention == null)
                return argser.Deserialize(arg, new StackVariable_v1());
            switch (convention)
            {
            case "":
            case "stdapi":
            case "__cdecl":
            case "__stdcall":
            case "pascal":
                return argser.Deserialize(arg, new StackVariable_v1 { });
            case "__thiscall":
                if (idx == 0)
                    return argser.Deserialize(arg, new Register_v1("ecx"));
                else
                    return argser.Deserialize(arg, new StackVariable_v1());
            }
            throw new NotSupportedException(string.Format("Unsupported calling convention '{0}'.", convention));
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            if (bitSize == 0)
                bitSize = Architecture.WordWidth.BitSize;
            switch (bitSize)
            {
            case 32: 
                if (Architecture.WordWidth.BitSize == 16)
                    return new SequenceStorage(
                        new Identifier("dx", PrimitiveType.Word16, Architecture.GetRegister("dx")),
                        new Identifier("ax", PrimitiveType.Word16, Architecture.GetRegister("ax")));
                break;
            case 64: if (Architecture.WordWidth.BitSize == 32)
                    return new SequenceStorage(
                        new Identifier("edx", PrimitiveType.Word16, Architecture.GetRegister("edx")),
                        new Identifier("eax", PrimitiveType.Word16, Architecture.GetRegister("eax")));
                break;
            }
            var reg = Architecture.GetRegister("rax");
            return Architecture.GetSubregister(reg, 0, bitSize);
        }
    }
}
