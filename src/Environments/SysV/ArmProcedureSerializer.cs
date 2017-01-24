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
using System.Collections.Generic;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV
{
    public class Arm32ProcedureSerializer : ProcedureSerializer
    {
        private static string[] argRegs = { "r0", "r1", "r2", "r3" };

        public Arm32ProcedureSerializer(
            IProcessorArchitecture arch,
            ISerializedTypeVisitor<DataType> typeLoader, 
            string defaultConvention) 
            : base(arch, typeLoader, defaultConvention)
        {
        }

        public override FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            int ncrn = 0;
            int nsaa = 0;
            // mem arg forb ret val

            var argser = new ArgumentDeserializer(
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
                foreach (var sArg in ss.Arguments)
                {
                    if (sArg.Name == "...")
                    {
                        argser.Deserialize(sArg);
                        continue;
                    }
                    var dt = sArg.Type.Accept(TypeLoader);
                    var sizeInWords = (dt.Size + 3) / 4;

                    if (sizeInWords == 2 && (ncrn & 1) == 1)
                        ++ncrn;
                    Identifier arg;
                    if (sizeInWords <= 4 - ncrn)
                    {
                        if (sizeInWords == 2)
                        {
                            arg = frame.EnsureSequence(
                                Architecture.GetRegister(argRegs[ncrn]),
                                Architecture.GetRegister(argRegs[ncrn + 1]),
                                dt);
                            ncrn += 2;
                        }
                        else
                        {
                            arg = frame.EnsureRegister(
                                Architecture.GetRegister(argRegs[ncrn]));
                            ncrn += 1;
                        }
                    }
                    else
                    {
                        arg = frame.EnsureStackArgument(nsaa, dt);
                        nsaa += AlignedStackArgumentSize(dt);
                    }
                    args.Add(arg);
                }
            }
            return new FunctionType(ret, args.ToArray());
        }

        private int AlignedStackArgumentSize(DataType dt)
        {
            return ((dt.Size + 3) / 4) * 4; 
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            if (bitSize <= 32)
            {
                return Architecture.GetRegister("r0");
            }
            else if (bitSize <= 64)
            {
                return new SequenceStorage(
                    Architecture.GetRegister("r1"),
                    Architecture.GetRegister("r0"));
            }
            else
                throw new NotSupportedException(string.Format("Return values of {0} bits are not supported.", bitSize));
        }
    }
}