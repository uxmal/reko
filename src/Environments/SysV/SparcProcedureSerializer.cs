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

using System;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Environments.SysV
{
    public class SparcProcedureSerializer : ProcedureSerializer
    {
        private static string[] iregs = new string[]
        {
            "o0","o1","o2","o3","o4","o5",
        };

        private ArgumentDeserializer argser;

        public SparcProcedureSerializer(
            IProcessorArchitecture arch,
            ISerializedTypeVisitor<DataType> typeLoader, 
            string defaultConvention)
        : base(arch, typeLoader, defaultConvention)
        {
        }

        public void ApplyConvention(SerializedSignature ssig, FunctionType sig)
        {
            string d = ssig.Convention;
            if (d == null || d.Length == 0)
                d = DefaultConvention;
            sig.StackDelta = 0;
            sig.FpuStackDelta = 0;
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

            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
            }

            this.ir = 0;
            var args = new List<Identifier>();
            if (ss.Arguments != null)
            {
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                    args.Add(arg);
                }
            }

            var sig = new FunctionType(ret, args.ToArray());
            ApplyConvention(ss, sig);
            return sig;
        }

        private int ir;

        public Identifier DeserializeArgument(Argument_v1 sArg, int idx, string convention)
        {
            if (sArg.Name == "...")
            {
                return this.CreateId(
                    sArg.Name,
                    new UnknownType(),
                    null);
            }
            if (sArg.Kind != null)
            {
                return argser.Deserialize(sArg, sArg.Kind);
            }
            Identifier arg;
            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
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
            //int regsNeeded = (dtArg.Size + 7) / 8;
            //if (regsNeeded > 4 || ir + regsNeeded >= iregs.Length)
            //{
            //    return argser.Deserialize(sArg, new StackVariable_v1());
            //}
            throw new NotImplementedException();
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            var dtArg = sArg.Type.Accept(TypeLoader);
            var ptArg = dtArg as PrimitiveType;
            if (ptArg != null)
            {
                if (ptArg.Domain == Domain.Real)
                {
                    var f0 = Architecture.GetRegister("f0");
                    if (ptArg.Size <= 4)
                        return f0;
                    var f1 = Architecture.GetRegister("f1");
                    return new SequenceStorage(f1, f0);
                }
                return Architecture.GetRegister("o0");
            }
            else if (dtArg is Pointer)
            {
                return Architecture.GetRegister("o0");
            }
            else if (dtArg.Size <= this.Architecture.WordWidth.Size)
            {
                return Architecture.GetRegister("o0");
            }
            throw new NotImplementedException();
        }
    }
}