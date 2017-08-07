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
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Arch.M68k;
using System;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Environments.AtariTOS
{
    public class TOSProcedureSerializer : ProcedureSerializer
    {
        private ArgumentDeserializer argDeser;

        public TOSProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
            : base(arch, typeLoader, defaultConvention)
        {
        }

        // The TOSCall calling convention takes all arguments on stack, ignoring 
        // the first one since it is the system call selector.
        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            if (ss.Convention == "TOScall")
            {
                base.StackOffset = 4;   // Skip the system call selector.
            }
            this.argDeser = new ArgumentDeserializer(
                this,
                Architecture,
                frame,
                Architecture.PointerType.Size,
                Architecture.WordWidth.Size);
            Identifier ret = null;
            int fpuDelta = FpuStackOffset;

            if (ss.ReturnValue != null)
            {
                ret = argDeser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            if (ss.EnclosingType != null)
            {
                var arg = DeserializeImplicitThisArgument(ss);
                args.Add(arg);
            }
            if (ss.Arguments != null)
            {
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
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

        private void ApplyConvention(SerializedSignature ss, FunctionType sig)
        {
            // AFAIK the calling convention on Atari TOS is caller-cleanup, 
            // so the only thing we clean up is the return value on the stack.
            sig.StackDelta = 4; 
        }

        private Identifier DeserializeArgument(Argument_v1 sArg, int iArg, string convention)
        {
            return argDeser.Deserialize(sArg, new StackVariable_v1 { });
        }

        private Identifier DeserializeImplicitThisArgument(SerializedSignature ss)
        {
            throw new NotImplementedException("C++ implicit `this` arguments are not implemented for Atari TOS.");
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            return Registers.d0;
        }
    }
}