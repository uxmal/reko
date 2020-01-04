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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Analysis
{
	/// <summary>
	/// Describes the flow of registers in and out of a procedure. 
	/// We are usually interested in registers modified, live in, &c.
	/// </summary>
	public class ProcedureFlow : DataFlow
	{
        public Procedure Procedure { get; }
        public FunctionType Signature;

        /// <summary>
        /// A collection of all each storage that is live-in to the procedure,
        /// and the bits that are live in.
        /// </summary>
        public Dictionary<Storage, BitRange> BitsUsed;

        /// <summary>
        /// A collection of all storages that are live-out after the procedure 
        /// is called.
        /// </summary>
        /// <remarks>
        /// For instance, in the code fragment:
        /// <code>
        /// call foo
        /// Mem0[0x00123400:word16] = r3
        /// </code>
        /// (at least) the 16 least significant bits of r3 will be 
        /// live-out.
        /// </remarks>
        public Dictionary<Storage, BitRange> BitsLiveOut;

        public Dictionary<RegisterStorage, uint> grfLiveOut;


        public HashSet<Storage> Preserved;			// Registers explicitly preserved by the procedure.
		public Dictionary<RegisterStorage, uint> grfPreserved;

		public Dictionary<RegisterStorage,uint> grfTrashed;
		public HashSet<Storage> Trashed;        // Registers globally trashed by procedure and/or callees.
        
        /// <summary>
        /// If present, indicates a register always has a constant value
        /// leaving the procedure.
        /// </summary>
        public Dictionary<Storage, Constant> Constants; 

        // True if calling this procedure terminates the thread/process. This implies
        // that no code path reached the exit block without first terminating the process.
        public bool TerminatesProcess;

        public ProcedureFlow(Procedure proc)
        {
            this.Procedure = proc;

            Preserved = new HashSet<Storage>();
            Trashed = new HashSet<Storage>();
            Constants = new Dictionary<Storage, Constant>();

            grfTrashed = new Dictionary<RegisterStorage, uint>();
            grfPreserved = new Dictionary<RegisterStorage, uint>();
            grfLiveOut = new Dictionary<RegisterStorage, uint>();

            BitsLiveOut = new Dictionary<Storage, BitRange>();

            this.BitsUsed = new Dictionary<Storage, BitRange>();
        }

        [Conditional("DEBUG")]
        public void Dump(IProcessorArchitecture arch)
        {
            var sb = new StringWriter();
            Emit(arch, sb);
            Debug.WriteLine(sb.ToString());
        }

		public override void Emit(IProcessorArchitecture arch, TextWriter writer)
		{
			EmitRegisterValues("// MayUse: ", BitsUsed, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// LiveOut:", grfLiveOut, BitsLiveOut.Keys, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Trashed:", grfTrashed, Trashed, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Preserved:", grfPreserved, Preserved, writer);
			writer.WriteLine();
            if (TerminatesProcess)
                writer.WriteLine("// Terminates process");
		}

		public bool IsLiveOut(Identifier id)
		{
            if (id.Storage is FlagGroupStorage flags)
			{
                if (!this.grfLiveOut.TryGetValue(flags.FlagRegister, out uint grf))
                    return false;
                return ((grf & flags.FlagGroupBits) != 0);
			}
			if (id.Storage is RegisterStorage reg)
			{
                return BitsLiveOut.ContainsKey(reg);
			}
			return false;
		}

        /// <summary>
        /// Returns the number of slots the FPU stack grew.
        /// If the architecture doesn't support FPU stacks, 
        /// returns 0.
        /// </summary>
        /// <param name="arch"></param>
        /// <returns></returns>
        public int GetFpuStackDelta(IProcessorArchitecture arch)
        {
            var fpuStackReg = arch.FpuStackRegister;
            if (fpuStackReg == null ||
                !Constants.TryGetValue(fpuStackReg, out var c))
            {
                return 0;
            }
            return c.ToInt32();
        }

        public static IEnumerable<CallBinding> IntersectCallBindingsWithUses(
            IEnumerable<CallBinding> callBindings,
            IDictionary<Storage, BitRange> uses)
        {
            //$TODO: this is an O(n^2) implementation, which will be teh suck performancewise.
            // If it can be improved, do so.
            foreach (var use in uses)
            {
                CallBinding callBinding = null;
                switch (use.Key)
                {
                case RegisterStorage reg:
                    callBinding = IntersectRegisterBinding(reg, callBindings);
                    break;
                case StackArgumentStorage stArg:
                    callBinding = IntersectStackRegisterBinding(stArg, callBindings);
                    break;
                }
                if (callBinding != null)
                    yield return callBinding;
            }
        }

        private static CallBinding IntersectStackRegisterBinding(StackArgumentStorage stArg, IEnumerable<CallBinding> callBindings)
        {
            var stRange = stArg.GetBitRange();
            foreach (var binding in callBindings)
            {
                if (binding.Storage is StackArgumentStorage stBinding)
                {
                    if (binding.Storage.Equals(stArg))
                        return binding;
                }
            }
            return null;
        }

        private static CallBinding IntersectRegisterBinding(RegisterStorage regCallee, IEnumerable<CallBinding> callBindings)
        {
            var dom = regCallee.Domain;
            var regRange = regCallee.GetBitRange();
            foreach (var binding in callBindings)
            {
                if (binding.Storage.Domain == dom)
                {
                    var isect = binding.BitRange | regRange;
                    if (binding.BitRange == regRange)
                        return binding;
                    if (binding.BitRange.Extent > regRange.Extent)
                    {
                        var dt = PrimitiveType.CreateWord(regCallee.DataType.BitSize);
                        var exp = new Slice(dt, binding.Expression, regRange.Lsb);
                        return new CallBinding(regCallee, exp);
                    }
                }
            }
            return null;
        }
    }
}
