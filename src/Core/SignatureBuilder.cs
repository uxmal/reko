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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core
{
	/// <summary>
	/// Builds a procedure signature argument by argument. In particular, keeps
    /// track of how many output registers/FPU stack registers have been seen,
    /// and makes them either the return value of the ProcedureSignature or an
    /// "out" parameter.
	/// </summary>
	public class SignatureBuilder
	{
		private List<Identifier> args;
		private Identifier ret = null;
		private IStorageBinder binder;
		private IProcessorArchitecture arch;

		public SignatureBuilder(IStorageBinder binder, IProcessorArchitecture arch)
		{
			this.binder = binder;
			this.arch = arch;
			args = new List<Identifier>();
		}

		public void AddFlagGroupReturnValue(uint bitMask, IStorageBinder frame)
		{
			PrimitiveType dt = Bits.IsSingleBitSet(bitMask) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var grf = arch.GetFlagGroup(bitMask);
			ret = frame.EnsureFlagGroup(grf.FlagRegister, bitMask, grf.Name, dt);
		}

		public void AddFpuStackArgument(int x, Identifier id)
		{
			AddInParam(binder.EnsureFpuStackVariable(x, id.DataType));
		}

		public void AddRegisterArgument(RegisterStorage reg)
		{
			AddInParam(binder.EnsureRegister(reg));
		}

        public void AddOutParam(Identifier idOrig)
        {
            if (ret == null)
            {
                ret = idOrig;
            }
            else
            {
                //$REVIEW: out arguments are weird, as they are synthetic. It's possible that 
                // future versions of reko will opt to model multiple values return from functions
                // explicitly instead of using destructive updates of this kind.
                var arg = binder.EnsureOutArgument(idOrig, PrimitiveType.Create(Domain.Pointer, arch.FramePointerType.Size));
                args.Add(arg);
            }
        }

        public void AddInParam(Identifier arg)
        {
            args.Add(arg);
        }

		public FunctionType BuildSignature()
		{
			return new FunctionType(ret, args.ToArray());
		}

		public Identifier CreateOutIdentifier(Procedure proc, Identifier id)
		{
			return proc.Frame.CreateTemporary(id.Name + "Out", id.DataType);
		}
	}
}
