#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
		private Argument_v1 argCur;
        private string convention;

		public ArgumentSerializer(ProcedureSerializer sig, IProcessorArchitecture arch, Frame frame, string callingConvention)
		{
			this.ps = sig;
			this.arch = arch;
			this.frame = frame;
            this.convention = callingConvention;
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
			var idArg = frame.EnsureRegister(arch.GetRegister(reg.Name.Trim()));
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
                return ps.CreateId("...", new UnknownType(), new StackArgumentStorage(ps.StackOffset, new UnknownType()));
            }
            if (argCur.Type == null)
                throw new ApplicationException(string.Format("Argument '{0}' has no type.", argCur.Name));
			var dt = this.argCur.Type.Accept(ps.TypeLoader);
            if (dt is VoidType)
            {
                return null;
            }
			var idArg = ps.CreateId(
				argCur.Name ?? "arg" + ps.StackOffset, 
				dt,
				new StackArgumentStorage(ps.StackOffset, dt));
            ps.StackOffset += dt.Size;
            return idArg;
		}

		public Identifier Deserialize(FpuStackVariable_v1 fs)
		{
			var idArg = ps.CreateId(argCur.Name ?? "fpArg" + ps.FpuStackOffset , PrimitiveType.Real64, new FpuStackStorage(ps.FpuStackOffset, PrimitiveType.Real64));
			++ps.FpuStackOffset;
            return idArg;
		}

        public Identifier Deserialize(SerializedFlag flag)
		{
			var flags = arch.GetFlagGroup(flag.Name);
			return frame.EnsureFlagGroup(flags.FlagGroupBits, flags.Name, flags.DataType);
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
            if (arg.Kind != null)
                return arg.Kind.Accept(this);
            //$PLATFORM-SPECIFIC!!!!
			var dt = this.argCur.Type.Accept(ps.TypeLoader);
            if (dt is VoidType)
                return null;
            var reg = arch.GetRegister("eax").GetSubregister(0, dt.BitSize);
            return frame.EnsureRegister(reg);
        }

		public Identifier Deserialize(Argument_v1 arg, int idx)
		{
			argCur = arg;
            if (arg.Kind != null)
            {
                var a = arg.Kind.Accept(this);
                return a;
            }
            //$PLATFORM-specifiC!!! We're encoding Microsoft + x86 conventions here.
            if (convention == "stdapi" || convention == "__cdecl" || convention == "__stdcall")
            {
                return Deserialize(new StackVariable_v1 { });
            }
            if (convention == "__thiscall")
            {
                if (idx == 0)
                    return Deserialize(new Register_v1("ecx"));
                else
                    return Deserialize(new StackVariable_v1());
            }
            throw new NotImplementedException();
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
