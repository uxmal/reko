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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Core.Serialization
{
	public class ArgumentSerializer
	{
		private ProcedureSerializer procSer;
		private IProcessorArchitecture arch;
		private Frame frame;
		private Argument_v1 argCur;
        private int retAddressOnStack;

        public ArgumentSerializer(ProcedureSerializer procSer, IProcessorArchitecture arch, Frame frame, int retAddressOnStack)
		{
			this.procSer = procSer;
			this.arch = arch;
			this.frame = frame;
            this.retAddressOnStack = retAddressOnStack;
        }
        
		public string ArgumentName(string argName, string argName2)
		{
			if (argName != null)
				return argName;
			else
				return argName2;
		}

		public Identifier Deserialize(Register_v1 reg)
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
                idArg = frame.EnsureOutArgument(idArg, arch.FramePointerType);
            }
            return idArg;
		}

		public Identifier Deserialize(StackVariable_v1 ss)
		{
            if (argCur.Name == "...")
            {
                return procSer.CreateId("...", new UnknownType(), new StackArgumentStorage(procSer.StackOffset, new UnknownType()));
            }
            if (argCur.Type == null)
                throw new ApplicationException(string.Format("Argument '{0}' has no type.", argCur.Name));
			var dt = this.argCur.Type.Accept(procSer.TypeLoader);
            if (dt is VoidType)
            {
                return null;
            }
            var name = frame.FormatStackAccessName(
                dt,
                "Arg",
                procSer.StackOffset + retAddressOnStack,
                argCur.Name);
            var idArg = procSer.CreateId(
                name,
				dt,
				new StackArgumentStorage(procSer.StackOffset, dt));
            procSer.StackOffset += dt.Size;
            return idArg;
		}

		public Identifier Deserialize(FpuStackVariable_v1 fs)
		{
			var idArg = procSer.CreateId(argCur.Name ?? "fpArg" + procSer.FpuStackOffset , PrimitiveType.Real64, new FpuStackStorage(procSer.FpuStackOffset, PrimitiveType.Real64));
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
			Identifier head = frame.EnsureRegister(h);
			Identifier tail = frame.EnsureRegister(t);
			return frame.EnsureSequence(head, tail, 
				PrimitiveType.CreateWord(head.DataType.Size + tail.DataType.Size));
		}

        public Identifier DeserializeReturnValue(Argument_v1 arg)
        {
            argCur = arg;
            DataType dt = null;
            if (this.argCur.Type != null)
			    dt = this.argCur.Type.Accept(procSer.TypeLoader);
            if (dt is VoidType)
                return null;
            Identifier id;
            if (arg.Kind != null)
            {
                id = arg.Kind.Accept(this);
                id.DataType = dt ?? id.DataType;
            }
            else
            {
                var reg = procSer.GetReturnRegister(arg, dt.BitSize);
                id = new Identifier(reg.ToString(), dt, reg);
            }
            return id;
        }

		public Identifier Deserialize(Argument_v1 arg)
        {
            argCur = arg;
            return arg.Kind.Accept(this);
        }

        public Identifier Deserialize(Argument_v1 arg, SerializedKind kind)
        {
            argCur = arg;
            return kind.Accept(this);
        }

        public Argument_v1 Serialize(Identifier arg)
        {
            if (arg == null)
                return null;
            if (arg.DataType == null)
                throw new ArgumentNullException("arg.DataType");
            Argument_v1 sarg = new Argument_v1 
            {
			    Name = arg.Name,
			    Kind = arg.Storage.Serialize(),
                OutParameter = arg.Storage is OutArgumentStorage,
                Type = arg.DataType.Accept(new DataTypeSerializer()),
            };
            return sarg;
        }
    }
}
