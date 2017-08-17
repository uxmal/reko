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
using Reko.Core.Code;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV
{
    // Based on algorithm described in
    // http://www.cygwin.com/ml/binutils/2003-06/msg00436.html

    public class MipsProcedureSerializer : ProcedureSerializer
    {
        private int ir;
        private bool firstArgIntegral;

        public MipsProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
            : base(arch, typeLoader, defaultConvention)
        {
        }

        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            var argser = new ArgumentDeserializer(this, Architecture, frame, 0, Architecture.WordWidth.Size);
            Identifier ret = null;

            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            this.ir =  0;
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
            if (sArg.Name == "...")
            {
                return this.CreateId(
                    sArg.Name,
                    new UnknownType(),
                    null);
            }
            if (sArg.Kind != null)
                return argser.Deserialize(sArg);

            Identifier arg;
            var dtArg = sArg.Type.Accept(TypeLoader);
            var prim = dtArg as PrimitiveType;
            if (prim != null && prim.Domain == Domain.Real && !firstArgIntegral)
            {
                if ((ir % 2) != 0)
                    ++ir;
                if (this.ir >= 4)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    if (prim.Size == 4)
                    {
                        arg = argser.Deserialize(sArg, new Register_v1("f" + (this.ir + 12)));
                        this.ir += 1;
                    }
                    else if (prim.Size == 8)
                    {
                        arg = argser.Deserialize(sArg, new SerializedSequence
                        {
                            Registers = new[] {
                                new Register_v1("f" + (this.ir + 12)),
                                new Register_v1("f" + (this.ir + 13))
                            }
                        });
                        this.ir += 2;
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Real type of size {0} not supported.", prim.Size));
                    }
                }
                return arg;
            }
            if (ir == 0)
                firstArgIntegral = true;
            if (dtArg.Size <= 4)
            {
                if (this.ir >= 4)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new Register_v1("r" + (ir+4)));
                    ++this.ir;
                }
                return arg;
            }
            if (dtArg.Size <= 8)
            {
                if ((ir & 1) != 0)
                    ++ir;
                if (this.ir >= 4)
                {
                    arg = argser.Deserialize(sArg, new StackVariable_v1());
                }
                else
                {
                    arg = argser.Deserialize(sArg, new SerializedSequence
                    {
                        Registers = new Register_v1[] {
                            new Register_v1("r" + (ir+4)),
                            new Register_v1("r" + (ir+5))
                        }
                    });
                    ir += 2;
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
                    return Architecture.GetRegister("f1");
            }
            return Architecture.GetRegister("r3");
        }
    }
}
