#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Deserializes serialized arguments into Identifiers.
    /// </summary>
    public class ArgumentDeserializer
    {
        private readonly ProcedureSerializer procSer;
        private readonly IProcessorArchitecture arch;
        private readonly Frame frame;
        private readonly int retAddressOnStack;  // number of bytes on the stack occupied by return address
        private readonly int stackAlignment;
        private Argument_v1 argCur;

        public ArgumentDeserializer(
            ProcedureSerializer procSer, 
            IProcessorArchitecture arch, 
            Frame frame, 
            int retAddressOnStack,
            int stackAlign)
        {
            this.procSer = procSer;
            this.arch = arch;
            this.frame = frame;
            this.retAddressOnStack = retAddressOnStack;
            this.stackAlignment = stackAlign;
        }

        public Identifier VisitRegister(Register_v1 reg)
        {
            var regStorage = arch.GetRegister(reg.Name.Trim());
            DataType dt;
            if (this.argCur.Type != null)
                dt = this.argCur.Type.Accept(procSer.TypeLoader);
            else
                dt = regStorage.DataType;
            if (dt is VoidType)
                return null;
            var idArg = procSer.CreateId(
                argCur.Name ?? regStorage.Name,
                dt,
                regStorage);
            if (argCur.OutParameter)
            {
                //$REVIEW: out arguments are weird, as they are synthetic. It's possible that 
                // future versions of reko will opt to model multiple values return from functions
                // explicitly instead of using destructive updates of this kind.
                idArg = frame.EnsureOutArgument(idArg, PrimitiveType.Create(Domain.Pointer, arch.FramePointerType.BitSize));
            }
            return idArg;
        }

        public Identifier Deserialize(StackVariable_v1 ss)
        {
            if (argCur.Name == "...")
            {
                return procSer.CreateId(
                    "...",
                    new UnknownType(),
                    new StackArgumentStorage(
                        procSer.StackOffset + retAddressOnStack,
                        new UnknownType()));
            }
            if (argCur.Type == null)
                throw new ApplicationException(string.Format("Argument '{0}' has no type.", argCur.Name));
            var dt = this.argCur.Type.Accept(procSer.TypeLoader);
            if (dt is VoidType)
            {
                return null;
            }
            var name = NamingPolicy.Instance.StackArgumentName(
                dt,
                procSer.StackOffset + retAddressOnStack,
                argCur.Name);
            var idArg = procSer.CreateId(
                name,
                dt,
                new StackArgumentStorage(procSer.StackOffset + retAddressOnStack, dt));
            int words = (dt.Size + (stackAlignment - 1)) / stackAlignment;
            procSer.StackOffset += words * stackAlignment;
            return idArg;
        }

        public Identifier Deserialize(FpuStackVariable_v1 fs)
        {
            var idArg = procSer.CreateId(
                argCur.Name ?? "fpArg" + procSer.FpuStackOffset, 
                PrimitiveType.Real64,
                new FpuStackStorage(procSer.FpuStackOffset, PrimitiveType.Real64));
            ++procSer.FpuStackOffset;
            return idArg;
        }

        public Identifier Deserialize(FlagGroup_v1 flag)
        {
            var flags = arch.GetFlagGroup(flag.Name);
            return frame.EnsureFlagGroup(flags.FlagRegister, flags.FlagGroupBits, flags.Name, flags.DataType);
        }

        public Identifier Deserialize(SerializedSequence sq)
        {
            var h = arch.GetRegister(sq.Registers[0].Name.Trim());
            var t = arch.GetRegister(sq.Registers[1].Name.Trim());
            DataType dt;
            if (this.argCur.Type != null)
                dt = this.argCur.Type.Accept(procSer.TypeLoader);
            else 
                dt = PrimitiveType.CreateWord(h.DataType.BitSize + h.DataType.BitSize);
            return frame.EnsureSequence(dt, h, t);
        }

        public Identifier Deserialize(Argument_v1 arg)
        {
            argCur = arg;
            return arg.Kind.Deserialize(this);
        }

        public Identifier Deserialize(Argument_v1 arg, SerializedKind kind)
        {
            argCur = arg;
            return kind.Deserialize(this);
        }
    }
}
