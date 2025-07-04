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
using System.Diagnostics;

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
        private readonly List<Identifier> outputs;
		private readonly IStorageBinder binder;
		private readonly IProcessorArchitecture arch;

        /// <summary>
        /// Constructs an instance of <see cref="SignatureBuilder"/>.
        /// </summary>
        /// <param name="binder"><see cref="IStorageBinder"/> to use for introducing
        /// new identifiers in the caller's scope.</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> being used in the call.
        /// </param>
        public SignatureBuilder(IStorageBinder binder, IProcessorArchitecture arch)
		{
			this.binder = binder;
			this.arch = arch;
            parameters = [];
            outputs = [];
        }

        /// <summary>
        /// </summary>
		public void AddFlagGroupReturnValue(KeyValuePair<RegisterStorage, uint> bits, IStorageBinder binder)
		{
            var grf = arch.GetFlagGroup(bits.Key, bits.Value)!;
            Debug.Assert(outputs.Count == 0, "Flag group return value should be the first output");  
            var ret = binder.EnsureFlagGroup(grf);
            if (Bits.IsSingleBitSet(grf.FlagGroupBits))
                ret.DataType = PrimitiveType.Bool;
            outputs.Add(ret);
        }

        /// <summary>
        /// Adds an FPU stack parameter to the signature being built.
        /// </summary>
        /// <param name="x">FPU stack offset.</param>
        /// <param name="id">Identifier of the stack variable.
        /// </param>
        public void AddFpuStackArgument(int x, Identifier id)
		{
			AddInParam(binder.EnsureFpuStackVariable(x, id.DataType));
		}

        /// <summary>
        /// Adds a register parameter to the signature being built.
        /// </summary>
        /// <param name="reg">Register storage.</param>
		public void AddRegisterArgument(RegisterStorage reg)
		{
			AddInParam(binder.EnsureRegister(reg));
		}

        /// <summary>
        /// Adds a sequence parameter to the signature being built.
        /// </summary>
        /// <param name="seq">Sequence argument.</param>
        public void AddSequenceArgument(SequenceStorage seq)
        {
			AddInParam(binder.EnsureSequence(seq.DataType, seq.Elements));
        }

        /// <summary>
        /// Adds an output parameter to the signature being built.
        /// </summary>
        /// <param name="idOrig">Identifier of the output parameter.</param>

        public Identifier AddOutParam(Identifier idOrig)
        {
            var idOut = idOrig;
            if (this.outputs.Count > 0)
            {
                idOut = binder.EnsureOutArgument(idOrig, idOrig.DataType);
            }
            this.outputs.Add(idOut);
            return idOut;
        }

        /// <summary>
        /// Adds the identifier as an input parameter to the signature being built.
        /// </summary>
        /// <param name="p">Parameter to add.</param>
        public void AddInParam(Identifier p)
        {
            parameters.Add(p);
        }

        /// <summary>
        /// After all parameters have been added, this method builds the
        /// <see cref="FunctionType"/> for the procedure.
        /// </summary>
        /// <returns><see cref="FunctionType"/> constructed from the parameters
        /// added previously.
        /// </returns>
		public FunctionType BuildSignature()
		{
			return new FunctionType(
                parameters.ToArray(),
                outputs.ToArray());
		}

        /// <summary>
        /// After all parameters have been added, this method builds the
        /// <see cref="FunctionType"/> for the procedure, optionally 
        /// ordering the input parameters according to the calling convention.
        /// </summary>
        /// <param name="cconv">Optional <see  cref="ICallingConvention">
        /// calling convention</see> to use for ordering the parameters.
        /// </param>
        /// <returns></returns>
        public FunctionType BuildSignature(ICallingConvention? cconv)
        {
            var rawArgs = this.parameters.ToArray();
            var cmp = cconv?.InArgumentComparer;
            if (cmp is not null)
                Array.Sort(rawArgs, cmp);
            return new FunctionType(rawArgs, outputs.ToArray());
        }
    }
}
