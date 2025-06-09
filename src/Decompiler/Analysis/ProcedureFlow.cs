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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
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
        public FunctionType? Signature { get; set; }

        /// <summary>
        /// A collection of each storage that is live-in to the procedure,
        /// and the bits that are live in.
        /// </summary>
        public Dictionary<Storage, BitRange> BitsUsed { get; set; }

        /// <summary>
        /// The data types inferred for each live-in storage.
        /// </summary>
        public Dictionary<Storage, DataType> LiveInDataTypes { get; }

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
        public Dictionary<Storage, LiveOutUse> BitsLiveOut { get; set; }

        /// <summary>
        /// A collection of all flag groups that are live-out after the 
        /// procedure is called, grouped by the flag register in which
        /// they are located.
        /// </summary>
        public Dictionary<RegisterStorage, LiveOutFlagsUse> LiveOutFlags { get; set; }

        /// <summary>
        /// Registers explicitly preserved by the procedure.
        /// </summary>
        /// <remarks>
        /// Registers are typically preserved when register values are pushed
        /// to the stack at the entry of the procedure, and popped before 
        /// leaving.
        /// </remarks>
        public HashSet<Storage> Preserved { get; }

        /// <summary>
        /// Flag bits explicitly preserved by the procedure, organized
        /// by the status register in which they are stored.
        /// </summary>
        /// <remarks>
        /// This typically occurs register values are pushed to the
        /// stack at the entry of the procedure, and popped before 
        /// leaving.
        /// </remarks>
		public Dictionary<RegisterStorage, uint> PreservedFlags { get; }

        /// <summary>
        /// Registers that have been modified at the end of the execution of
        /// the <see cref="Procedure"/>.
        /// </summary>
        public HashSet<Storage> Trashed { get; }

        /// <summary>
        /// Condition code flags that have been modified at the end of the execution of
        /// the <see cref="Procedure"/>, grouped by the status register they
        /// are part of.
        /// </summary>
        public Dictionary<RegisterStorage, uint> grfTrashed { get; }

        /// <summary>
        /// If present, indicates a register always has a constant value
        /// leaving the procedure.
        /// </summary>
        public Dictionary<Storage, Constant> Constants { get; }

        /// <summary>
        /// True if calling this procedure terminates the thread/process. This implies
        /// that no code path reached the exit block without first terminating the process.
        /// </summary>
        public bool TerminatesProcess { get; set; }

        public ProcedureFlow(Procedure proc)
        {
            this.Procedure = proc;

            Preserved = new HashSet<Storage>();
            Trashed = new HashSet<Storage>();
            Constants = new Dictionary<Storage, Constant>();

            grfTrashed = new Dictionary<RegisterStorage, uint>();
            PreservedFlags = new Dictionary<RegisterStorage, uint>();
            LiveOutFlags = new Dictionary<RegisterStorage, LiveOutFlagsUse>();

            BitsLiveOut = new Dictionary<Storage, LiveOutUse>();

            this.BitsUsed = new Dictionary<Storage, BitRange>();
            this.LiveInDataTypes = [];
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
            EmitStorageDataTypes("// DataTypes: ", LiveInDataTypes, writer);
			EmitRegisters(arch, "// LiveOut:", LiveOutFlags, BitsLiveOut, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Trashed:", grfTrashed, Trashed, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Preserved:", PreservedFlags, Preserved, writer);
			writer.WriteLine();
            if (TerminatesProcess)
                writer.WriteLine("// Terminates process");
		}

        private void EmitStorageDataTypes(
            string caption,
            Dictionary<Storage, DataType> dataTypes,
            TextWriter writer)
        {
            if (dataTypes.Count == 0)
                return;
            writer.WriteLine(caption);
            foreach (var (stg, dt) in dataTypes
                .Select(de => (de.Key.ToString(), de.Value))
                .OrderBy(de => de.Item1))
            {
                writer.WriteLine("//   {0}: {1}", stg, dt);
            }
        }

		public bool IsLiveOut(Identifier id)
		{
            if (id.Storage is FlagGroupStorage flags)
			{
                if (!this.LiveOutFlags.TryGetValue(flags.FlagRegister, out var grf))
                    return false;
                return ((grf.Flags & flags.FlagGroupBits) != 0);
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
            if (fpuStackReg is null ||
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
                CallBinding? callBinding = null;
                switch (use.Key)
                {
                case RegisterStorage reg:
                    callBinding = IntersectRegisterBinding(reg, callBindings);
                    break;
                case StackStorage stArg:
                    callBinding = IntersectStackRegisterBinding(stArg, callBindings);
                    break;
                }
                if (callBinding is not null)
                    yield return callBinding;
            }
        }

        private static CallBinding? IntersectStackRegisterBinding(StackStorage stArg, IEnumerable<CallBinding> callBindings)
        {
            foreach (var binding in callBindings)
            {
                if (binding.Storage is StackStorage)
                {
                    if (binding.Storage.Equals(stArg))
                        return binding;
                }
            }
            return null;
        }

        private static CallBinding? IntersectRegisterBinding(RegisterStorage regCallee, IEnumerable<CallBinding> callBindings)
        {
            var dom = regCallee.Domain;
            var regRange = regCallee.GetBitRange();
            foreach (var binding in callBindings)
            {
                if (binding.Storage.Domain == dom)
                {
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
