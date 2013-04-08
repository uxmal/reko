#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Serialization
{
	public class ArgumentSerializer
	{
		private ProcedureSerializer ps;
		private IProcessorArchitecture arch;
		private Frame frame;
		private SerializedArgument argCur;
		private Identifier idArg;

		public ArgumentSerializer(ProcedureSerializer sig, IProcessorArchitecture arch, Frame frame)
		{
			this.ps = sig;
			this.arch = arch;
			this.frame = frame;
		}

		public string ArgumentName(string argName, string argName2)
		{
			if (argName != null)
				return argName;
			else
				return argName2;
		}

		public void Deserialize(SerializedRegister reg)
		{
			idArg = frame.EnsureRegister(arch.GetRegister(reg.Name.Trim()));
			if (argCur.OutParameter)
			{
                idArg = frame.EnsureOutArgument(idArg, arch.FramePointerType);
			}
		}

		public void Deserialize(SerializedStackVariable ss)
		{
            if (argCur.Name == "...")
            {
                idArg = ps.CreateId("...", new UnknownType(), new StackArgumentStorage(ps.StackOffset, new UnknownType()));
                return;
            }
			var dt = this.argCur.Type.Accept(ps.TypeLoader);
			idArg = ps.CreateId(
				argCur.Name ?? "arg" + ps.StackOffset, 
				dt,
				new StackArgumentStorage(ps.StackOffset, dt));
			ps.StackOffset += ss.ByteSize;
		}

		public void Deserialize(SerializedFpuStackVariable fs)
		{
			idArg = ps.CreateId(argCur.Name ?? "fpArg" + ps.FpuStackOffset , PrimitiveType.Real64, new FpuStackStorage(ps.FpuStackOffset, PrimitiveType.Real64));
			++ps.FpuStackOffset;
		}

		public void Deserialize(SerializedFlag flag)
		{
			var flags = arch.GetFlagGroup(flag.Name);
			idArg = frame.EnsureFlagGroup(flags.FlagGroupBits, flags.Name, flags.DataType);
		}
			
		public void Deserialize(SerializedSequence sq)
		{
			var h = arch.GetRegister(sq.Registers[0].Name.Trim());
			var t = arch.GetRegister(sq.Registers[1].Name.Trim());
			Identifier head = frame.EnsureRegister(h);
			Identifier tail = frame.EnsureRegister(t);
			idArg = frame.EnsureSequence(head, tail, 
				PrimitiveType.CreateWord(head.DataType.Size + tail.DataType.Size));
		}

		public Identifier Deserialize(SerializedArgument arg)
		{
			argCur = arg;
            if (arg.Kind != null)
                arg.Kind.Accept(this);
            // else argByConvention();
			return idArg;
		}

        public SerializedArgument Serialize(Identifier arg)
        {
            if (arg == null)
                return null;
            SerializedArgument sarg = new SerializedArgument 
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
