/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections;

namespace Decompiler.Core
{
	/// <summary>
	/// Builds a procedure signature argument by argument. In particular, keeps track of how many
	/// output registers/FPU stack registers have been seen, and makes them either the return 
	/// value of the ProcedureSignature or an "out" parameter.
	/// </summary>
	public class SignatureBuilder
	{
		private ArrayList args;
		private Identifier ret = null;
		private Procedure proc;
		private IProcessorArchitecture arch;

		public SignatureBuilder(Procedure proc, IProcessorArchitecture arch)
		{
			this.proc = proc;
			this.arch = arch;
			args = new ArrayList();
		}

		private bool SingleBitSet(uint w)
		{
			return ((w & (w - 1)) == 0);
		}

		public void AddFlagGroupReturnValue(uint grf, Frame frame)
		{
			PrimitiveType dt = SingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
			ret = frame.EnsureFlagGroup(grf, arch.GrfToString(grf), dt);
		}

		public void AddFpuStackArgument(int x, Identifier id)
		{
			AddArgument(proc.Frame.EnsureFpuStackVariable(x, id.DataType), false);
		}

		public void AddRegisterArgument(int r)
		{
			AddArgument(proc.Frame.EnsureRegister(arch.GetRegister(r)), false);
		}

		public void AddArgument(Identifier idOrig, bool isOut)
		{
			Identifier arg;
			if (isOut)
			{
				if (ret == null)
				{
					ret = idOrig;
					return;
				}
				arg = proc.Frame.EnsureOutArgument(idOrig);
			}
			else
			{
				arg = idOrig;
			}
			args.Add(arg);
		}
	

		public void AddStackArgument(int stackOffset, Identifier id)
		{
			args.Add(new Identifier(id.Name, id.Number, id.DataType, new StackArgumentStorage(stackOffset, id.DataType)));
		}

		public ProcedureSignature BuildSignature()
		{
			Identifier [] a = (Identifier []) args.ToArray(typeof (Identifier));
			return new ProcedureSignature(ret, a);
		}

		public Identifier CreateOutIdentifier(Procedure proc, Identifier id)
		{
			return proc.Frame.CreateTemporary(id.Name + "Out", id.DataType);
		}
	}
}
