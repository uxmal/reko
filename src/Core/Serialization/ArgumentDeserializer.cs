#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Deserializes serialized arguments into <see cref="Identifier"/>s.
    /// </summary>
    public class ArgumentDeserializer
    {
        private readonly ProcedureSerializer procSer;
        private readonly IProcessorArchitecture arch;
        private readonly Frame frame;
        private readonly int retAddressOnStack;  // number of bytes on the stack occupied by return address
        private readonly int stackAlignment;
        //$REFACTOR: pass argCur as parameter. 
        private Argument_v1? argCur;

        /// <summary>
        /// Constructs an instance of the <see cref="ArgumentDeserializer"/>
        /// </summary>
        /// <param name="procSer"><see cref="ProcedureSerializer"/> instance used in conjunction
        /// with this class.</param>
        /// <param name="arch">Processor architecture to use.</param>
        /// <param name="frame">Frame of the calling procedure.</param>
        /// <param name="retAddressOnStack">The size of the return address on the stack.</param>
        /// <param name="stackAlign">Alignment requirement on stack parameters.
        /// </param>
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

        /// <inheritdoc/>
        public Identifier? VisitRegister(Register_v1 reg)
        {
            var regName = reg.Name;
            if (regName is null)
                return null;
            var regStorage = arch.GetRegister(regName.Trim());
            if (regStorage is null)
                return null;
            DataType dt;
            if (this.argCur!.Type is not null)
                dt = this.argCur.Type.Accept(procSer.TypeLoader);
            else
                dt = regStorage.DataType;
            if (dt is VoidType)
                return null;
            var idArg = new Identifier(
                argCur.Name ?? regStorage.Name,
                dt,
                regStorage);
            if (argCur.OutParameter)
            {
                //$REVIEW: out arguments are weird, as they are synthetic. It's possible that 
                // future versions of reko will opt to model multiple values return from functions
                // explicitly instead of using destructive updates of this kind.
                idArg = frame.EnsureOutArgument(idArg, dt);
            }
            return idArg;
        }

        /// <summary>
        /// Deserializes a stack variable, after which the stack offset is incremented by the size of the
        /// variable.
        /// </summary>
        /// <returns>An identifier for the next stack variable.</returns>
        public Identifier? Deserialize(StackVariable_v1 _)
        {
            if (argCur!.Name == "...")
            {
                procSer.IsVariadic = true;
                return null;
            }
            if (argCur.Type is null)
                throw new ApplicationException($"Argument '{argCur.Name}' has no type.");
            var dt = this.argCur.Type.Accept(procSer.TypeLoader);
            if (dt is VoidType)
            {
                return null;
            }
            var name = NamingPolicy.Instance.StackArgumentName(
                dt,
                procSer.StackOffset + retAddressOnStack,
                argCur.Name);
            var idArg = new Identifier(
                name,
                dt,
                new StackStorage(procSer.StackOffset + retAddressOnStack, dt));
            int words = (dt.Size + (stackAlignment - 1)) / stackAlignment;
            procSer.StackOffset += words * stackAlignment;
            return idArg;
        }

        /// <summary>
        /// Deserializes an FPU stack variable, after which the FPU stack offset is incremented by one.
        /// </summary>
        /// <returns>An identifier for the next stack variable.</returns>
        public Identifier Deserialize(FpuStackVariable_v1 _)
        {
            if (procSer.FpuStackShrinking)
            {
                --procSer.FpuStackOffset;
            }
            var idArg = new Identifier(
                argCur!.Name ?? "fpArg" + procSer.FpuStackOffset, 
                PrimitiveType.Real64,
                new FpuStackStorage(procSer.FpuStackOffset, PrimitiveType.Real64));
            if (procSer.FpuStackGrowing)
            {
                ++procSer.FpuStackOffset;
            }
            return idArg;
        }

        /// <summary>
        /// Deserializes a flag group.
        /// </summary>
        /// <param name="flag">Flag group to deserialize.</param>
        /// <returns>An <see cref="Identifier"/> for the flag group.</returns>
        public Identifier Deserialize(FlagGroup_v1 flag)
        {
            var flags = arch.GetFlagGroup(flag.Name!)!;
            return frame.EnsureFlagGroup(flags);
        }

        /// <summary>
        /// Deserializes a sequence storage.
        /// </summary>
        /// <param name="sq">Sequence to deserialize.</param>
        /// <returns>The deserialized parameter.</returns>
        public Identifier? Deserialize(SerializedSequence sq)
        {
            var hName = sq.Registers?[0].Name?.Trim();
            var tName = sq.Registers?[1].Name?.Trim();
            if (hName is null || tName is null)
                return null;
            var h = arch.GetRegister(hName);
            var t = arch.GetRegister(tName);
            if (h is null || t is null)
                return null;
            DataType dt;
            if (this.argCur!.Type is not null)
                dt = this.argCur.Type.Accept(procSer.TypeLoader);
            else 
                dt = PrimitiveType.CreateWord(h.DataType.BitSize + h.DataType.BitSize);
            var idArg = frame.EnsureSequence(dt, h, t);
            if (this.argCur.OutParameter)
            {
                //$REVIEW: out arguments are weird, as they are synthetic. It's possible that 
                // future versions of reko will opt to model multiple values return from functions
                // explicitly instead of using destructive updates of this kind.
                idArg = frame.EnsureOutArgument(idArg, dt);
            }
            return idArg;
        }

        /// <summary>
        /// Deserialize an argument.
        /// </summary>
        /// <param name="arg">Argument to deserialize.</param>
        /// <returns></returns>
        public Identifier? Deserialize(Argument_v1 arg)
        {
            argCur = arg;
            return arg.Kind?.Deserialize(this);
        }

        /// <summary>
        /// Deserializes an argument of a given storage.
        /// </summary>
        /// <param name="arg">Argument to deserialize.</param>
        /// <param name="stg">Storage for the argument.</param>
        /// <returns></returns>
        public Identifier? Deserialize(Argument_v1 arg, SerializedStorage stg)
        {
            argCur = arg;
            return stg.Deserialize(this);
        }
    }
}
