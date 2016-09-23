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

namespace Reko.Environments.RT11
{
    public class Rt11ProcedureSerializer : ProcedureSerializer
    {
        private int gr;

        public Rt11ProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention) : base(arch, typeLoader, defaultConvention)
        {
        }

        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return new FunctionType();
            if (ss == null)
                return null;
            var argser = new ArgumentDeserializer(this, Architecture, frame, 0, Architecture.WordWidth.Size);
            Identifier ret = null;

            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            this.gr = 0;
            if (ss.Arguments != null)
            {
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    Identifier arg = DeserializeArgument(argser, sArg);
                    args.Add(arg);
                }
            }

            var sig = new FunctionType(ret, args.ToArray());
            return sig;
        }

        private Identifier DeserializeArgument(ArgumentDeserializer argser, Argument_v1 sArg)
        {
            Identifier arg;
            if (sArg.Kind != null)
                return argser.Deserialize(sArg);

            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
            arg = argser.Deserialize(sArg, new Register_v1("r" + gr));
            ++this.gr;
            return arg;
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            throw new NotImplementedException();
        }
    }
}