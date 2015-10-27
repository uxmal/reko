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

namespace Reko.Environments.Windows
{
    public class MipsProcedureSerializer : ProcedureSerializer
    {
        private ArgumentSerializer argser;
        private int ir;
        private int fr;
        private static string[] iregs = { "rdi", "rsi", "rdx", "rcx", "r8", "r9" };
        private static string[] fregs = { "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7" };

        public MipsProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
            : base(arch, typeLoader, defaultCc)
        {
        }

        public void ApplyConvention(SerializedSignature ssig, ProcedureSignature sig)
        {
            string d = ssig.Convention;
            if (d == null || d.Length == 0)
                d = DefaultConvention;
            sig.StackDelta = 0;
            sig.FpuStackDelta = FpuStackOffset;
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
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                    args.Add(arg);
                }
                fpuDelta -= FpuStackOffset;
            }
            FpuStackOffset = fpuDelta;

            var sig = new ProcedureSignature(ret, args.ToArray());
            ApplyConvention(ss, sig);
            return sig;
        }

        public Identifier DeserializeArgument(Argument_v1 sArg, int idx, string convention)
        {
            if (sArg.Kind != null)
            {
                return argser.Deserialize(sArg, sArg.Kind);
            }
            Identifier arg;
            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                if (this.fr >= iregs.Length)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new Register_v1 { Name= fregs[fr] });
                }
                ++this.fr;
                return arg;
            }
            if (dtArg.Size <= 8)
            {
                if (this.ir >= iregs.Length)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new Register_v1 { Name = iregs[ir] });
                }
                ++this.ir;
                return arg;
            }
            int regsNeeded = (dtArg.Size + 7) / 8;
            if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
            {
                return argser.Deserialize(sArg, new StackVariable_v1());
            }
            throw new NotImplementedException();
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            var dtArg = sArg.Type.Accept(TypeLoader) as PrimitiveType;
            if (dtArg != null && dtArg.Domain == Domain.Real)
            {
                var xmm0 = Architecture.GetRegister("xmm0");
                if (bitSize <= 64)
                    return xmm0;
                if (bitSize <= 128)
                {
                    var xmm1 = Architecture.GetRegister("xmm1");
                    return new SequenceStorage(
                        new Identifier(xmm1.Name, xmm1.DataType, xmm1),
                        new Identifier(xmm0.Name, xmm0.DataType, xmm0));
                }
                throw new NotImplementedException();
            }
            var rax = Architecture.GetRegister("rax");
            if (bitSize <= 64)
                return rax;
            if (bitSize <= 128)
            {
                var rdx = Architecture.GetRegister("rdx");
                return new SequenceStorage(
                    new Identifier(rdx.Name, rdx.DataType, rdx),
                    new Identifier(rax.Name, rax.DataType, rax));
            }
            throw new NotImplementedException();
        }
    }
}
