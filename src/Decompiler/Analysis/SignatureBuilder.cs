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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
    /// <summary>
    /// Builds a procedure signature argument by argument. In particular, keeps
    /// track of how many output registers/FPU stack registers have been seen,
    /// and makes them either the return value of the ProcedureSignature or an
    /// "out" parameter.
    /// </summary>
    public class SignatureBuilder
	{
		private readonly List<Identifier> parameters;
		private readonly IStorageBinder binder;
		private readonly IProcessorArchitecture arch;
        private Identifier? ret = null;

        public SignatureBuilder(IStorageBinder binder, IProcessorArchitecture arch)
		{
			this.binder = binder;
			this.arch = arch;
			parameters = new List<Identifier>();
		}

		public void AddFlagGroupReturnValue(KeyValuePair<RegisterStorage, uint> bits, IStorageBinder binder)
		{
            var grf = arch.GetFlagGroup(bits.Key, bits.Value)!;
			ret = binder.EnsureFlagGroup(grf);
            if (Bits.IsSingleBitSet(grf.FlagGroupBits))
                ret.DataType = PrimitiveType.Bool;
		}

		public void AddFpuStackArgument(int x, Identifier id)
		{
			AddInParam(binder.EnsureFpuStackVariable(x, id.DataType));
		}

		public void AddRegisterArgument(RegisterStorage reg)
		{
			AddInParam(binder.EnsureRegister(reg));
		}

        public void AddSequenceArgument(SequenceStorage seq)
        {
			AddInParam(binder.EnsureSequence(seq.DataType, seq.Elements));
        }

        public Identifier AddOutParam(Identifier idOrig)
        {
            if (this.ret is null)
            {
                this.ret = idOrig;
                return ret;
            }
            else
            {
                //$REVIEW: out arguments are weird, as they are synthetic. It's possible that 
                // future versions of reko will opt to model multiple values return from functions
                // explicitly instead of using destructive updates of this kind.
                var parameter = binder.EnsureOutArgument(idOrig, idOrig.DataType);
                parameters.Add(parameter);
                return parameter;
            }
        }

        public void AddInParam(Identifier arg)
        {
            parameters.Add(arg);
        }

		public FunctionType BuildSignature()
		{
			return FunctionType.Create(ret, parameters.ToArray());
		}

        public FunctionType BuildSignature(ICallingConvention? cconv)
        {
            var rawArgs = this.parameters.ToArray();
            var cmp = cconv?.InArgumentComparer;
            if (cmp is not null)
                Array.Sort(rawArgs, cmp);
            return FunctionType.Create(ret, rawArgs);
        }
    }
}
