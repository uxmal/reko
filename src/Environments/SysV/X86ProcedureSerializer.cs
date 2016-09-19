#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

namespace Reko.Environments.SysV
{
    public class X86ProcedureSerializer : ProcedureSerializer
    {
        private ArgumentDeserializer argser;

        public X86ProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
            : base(arch, typeLoader, defaultCc)
        {
        }

        public void ApplyConvention(SerializedSignature ssig, FunctionType sig)
        {
            string d = ssig.Convention;
            if (d == null || d.Length == 0)
                d = DefaultConvention;
            sig.StackDelta = 0;
            sig.FpuStackDelta = FpuStackOffset;
        }

        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            this.argser = new ArgumentDeserializer(
                this,
                Architecture,
                frame, 
                Architecture.PointerType.Size,
                Architecture.WordWidth.Size);
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
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                    args.Add(arg);
                }
                fpuDelta -= FpuStackOffset;
            }
            FpuStackOffset = fpuDelta;

            var sig = new FunctionType(ret, args.ToArray());
            ApplyConvention(ss, sig);
            return sig;
        }

        public Identifier DeserializeArgument(Argument_v1 sArg, int idx, string convention)
        {
            if (sArg.Kind != null)
            {
                return argser.Deserialize(sArg, sArg.Kind);
            }
            return argser.Deserialize(sArg, new StackVariable_v1());
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            var dtArg = sArg.Type.Accept(TypeLoader) as PrimitiveType;
            if (dtArg != null && dtArg.Domain == Domain.Real)
            {
                return argser.Deserialize(new FpuStackVariable_v1 { ByteSize = dtArg.Size }).Storage;
            }
            var eax = Architecture.GetRegister("eax");
            if (bitSize <= 32)
                return eax;
            if (bitSize <= 64)
            {
                var edx = Architecture.GetRegister("edx");
                return new SequenceStorage(edx, eax);
            }
            throw new NotImplementedException();
        }
    }
}
