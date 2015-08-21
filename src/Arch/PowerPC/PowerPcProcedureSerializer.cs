﻿#region License
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

namespace Reko.Arch.PowerPC
{
    public class PowerPcProcedureSerializer : ProcedureSerializer
    {
        private PowerPcArchitecture arch;
        private int gr;
        private int fr;

        public PowerPcProcedureSerializer(
            PowerPcArchitecture arch,
            ISerializedTypeVisitor<DataType> typeLoader,
            string defaultCc)
            : base(arch, typeLoader, defaultCc)
        {
            this.arch = arch;
        }

        public override ProcedureSignature Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            var argser = new ArgumentSerializer(this, Architecture, frame, ss.Convention);
            Identifier ret = null;

            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            this.fr = 1;
            this.gr = 3;
            if (ss.Arguments != null)
            {
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    Identifier arg = DeserializeArgument(argser, sArg);
                    args.Add(arg);
                }
            }

            var sig = new ProcedureSignature(ret, args.ToArray());
            return sig;
        }

        private Identifier DeserializeArgument(ArgumentSerializer argser, Argument_v1 sArg)
        {
            Identifier arg;
            if (sArg.Kind != null)
                return sArg.Kind.Accept(argser);

            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real)
            {
                if (this.fr > 8)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new Register_v1("f" + this.fr));
                    ++this.fr;
                }
                return arg;
            }
            if (dtArg.Size <= 4)
            {
                if (this.gr > 10)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new Register_v1("r" + gr));
                    ++this.gr;
                }
                return arg;
            }
            if (dtArg.Size <= 8)
            {
                if (this.gr > 9)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    if ((gr & 1) == 0)
                        ++gr;
                    arg = argser.Deserialize(sArg, new SerializedSequence
                    {
                        Registers = new Register_v1[] { 
                            new Register_v1("r" + gr), 
                            new Register_v1("r" + (gr+1))
                        }
                    });
                    gr += 2;
                }
                return arg;
            }
            throw new NotImplementedException();
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            var prim = sArg.Type as PrimitiveType_v1;
            if (prim != null)
            {
                if (prim.Domain == Domain.Real)
                    return arch.GetRegister("f1");
            }
            return arch.GetRegister(3);
        }
    }
}
