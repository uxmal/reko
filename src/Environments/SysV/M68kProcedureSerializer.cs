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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV
{
    public class M68kProcedureSerializer : ProcedureSerializer
    {
        private ArgumentDeserializer argser;

        public M68kProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
            : base(arch, typeLoader, defaultConvention)
        {
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
            if (bitSize <= 32)
                return Architecture.GetRegister("d0");
            throw new NotImplementedException();
        }
    }
}
