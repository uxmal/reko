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

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Seralizes and deserializes MIPS signatures on Windows. 
    /// </summary>
    public class MipsProcedureSerializer : ProcedureSerializer
    {
        private ArgumentDeserializer argDeser;
        private int ir;
        private int fr;
        private static string[] iregs = { "r4", "r5", "r6", "r7" };
        private static string[] fregs = { };

        public MipsProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
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
            this.argDeser = new ArgumentDeserializer(this, Architecture, frame, 0, Architecture.WordWidth.Size);
            Identifier ret = null;
            int fpuDelta = FpuStackOffset;

            FpuStackOffset = 0;
            if (ss.ReturnValue != null)
            {
                ret = argDeser.DeserializeReturnValue(ss.ReturnValue);
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
                return argDeser.Deserialize(sArg, sArg.Kind);
            }
            Identifier arg;
            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                if (this.fr >= fregs.Length)
                {
                    arg = argDeser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argDeser.Deserialize(sArg, new Register_v1 { Name= fregs[fr] });
                }
                ++this.fr;
                return arg;
            }
            if (dtArg.Size <= 4)
            {
                if (this.ir >= iregs.Length)
                {
                    arg = argDeser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argDeser.Deserialize(sArg, new Register_v1 { Name = iregs[ir] });
                }
                ++this.ir;
                arg.DataType = dtArg;
                return arg;
            }
            int regsNeeded = (dtArg.Size + 3) / 4;
            if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
            {
                return argDeser.Deserialize(sArg, new StackVariable_v1());
            }
            if (regsNeeded == 2)
            {
                arg = argDeser.Deserialize(sArg, new SerializedSequence
                {
                    Registers = new[]
                    {
                        new Register_v1 { Name = iregs[ir] },
                        new Register_v1 { Name = iregs[ir+1] },
                    }
                });
                ir += 2;
                arg.DataType = dtArg;
                return arg;
            }
            throw new NotImplementedException();
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            var dtArg = sArg.Type.Accept(TypeLoader) as PrimitiveType;
            if (dtArg != null && dtArg.Domain == Domain.Real)
            {
                var f0 = Architecture.GetRegister("f0");
                if (bitSize <= 64)
                    return f0;
                throw new NotImplementedException();
            }
            var v0 = Architecture.GetRegister("r2");
            if (bitSize <= 32)
                return v0;
            if (bitSize <= 64)
            {
                var v1 = Architecture.GetRegister("r3");
                return new SequenceStorage(v1, v0);
            }
            throw new NotImplementedException();
        }
    }
}
