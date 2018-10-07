#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using CallRewriter = Reko.Core.CallRewriter;
using FpuStackStorage = Reko.Core.FpuStackStorage;
using IStorageBinder = Reko.Core.IStorageBinder;
using Identifier = Reko.Core.Expressions.Identifier;
using OutArgumentStorage = Reko.Core.OutArgumentStorage;
using PrimtiveType = Reko.Core.Types.PrimitiveType;
using Procedure = Reko.Core.Procedure;
using Program = Reko.Core.Program;
using RegisterStorage = Reko.Core.RegisterStorage;
using SignatureBuilder = Reko.Core.SignatureBuilder;
using StackArgumentStorage = Reko.Core.StackArgumentStorage;
using UseInstruction = Reko.Core.Code.UseInstruction;
using Reko.Core;

namespace Reko.Analysis
{
	/// <summary>
	/// Rewrites a program, based on summary live-in and live-out
    /// information, so that all CALL codes are converted into procedure 
    /// calls, with the appropriate parameter lists.
	/// </summary>
	/// <remarks>
	/// Call Rewriting should take place before SSA conversion and dead 
    /// code removal.
	/// </remarks>
	public class GlobalCallRewriter : CallRewriter
	{
		private ProgramDataFlow mpprocflow;

		public GlobalCallRewriter(Program program, ProgramDataFlow mpprocflow, DecompilerEventListener eventListener) : base(program)
		{
			this.mpprocflow = mpprocflow;
		}

		public void AddStackArgument(Identifier id, ProcedureFlow flow, SignatureBuilder sb)
		{
			object o = flow.StackArguments[id];
			if (o != null)
			{
				int bitWidth = (int) o;
				if (bitWidth < id.DataType.BitSize)
				{
					PrimtiveType pt = id.DataType as PrimtiveType;
					if (pt != null)
					{
						id.DataType = PrimtiveType.Create(pt.Domain, bitWidth);
					}
				}
			}
			sb.AddInParam(id);
		}

		public void AddUseInstructionsForOutArguments(Procedure proc)
		{
			foreach (Identifier id in proc.Signature.Parameters)
			{
				var os = id.Storage as OutArgumentStorage;
				if (os == null)
					continue;
				var r = os.OriginalIdentifier.Storage as RegisterStorage;
				if (r == null)
					continue;

                proc.ExitBlock.Statements.Add(
                    //$TODO: should be procedure exit address here
                    proc.EntryAddress.ToLinear(),
                    new UseInstruction(os.OriginalIdentifier, id));
			}
		}

		/// <summary>
		/// Adjusts LiveOut values for use as out registers.
		/// </summary>
		/// <remarks>
		/// LiveOut sets contain registers that aren't modified by the procedure. When determining
		/// the returned registers, those unmodified registers must be filtered away.
		/// </remarks>
		/// <param name="flow"></param>
		private void AdjustLiveOut(ProcedureFlow flow)
		{
			flow.grfLiveOut &= flow.grfTrashed;
			flow.LiveOut.IntersectWith(flow.TrashedRegisters);
		}

		public static void Rewrite(
            Program program, 
            ProgramDataFlow summaries,
            DecompilerEventListener eventListener)
		{
			GlobalCallRewriter crw = new GlobalCallRewriter(program, summaries, eventListener);
			foreach (Procedure proc in program.Procedures.Values)
			{
                if (eventListener.IsCanceled())
                    return;
				ProcedureFlow flow = crw.mpprocflow[proc];
                flow.Dump(proc.Architecture);
				crw.AdjustLiveOut(flow);
				crw.EnsureSignature(proc, flow);
				crw.AddUseInstructionsForOutArguments(proc);
			}

			foreach (Procedure proc in program.Procedures.Values)
			{
                if (eventListener.IsCanceled())
                    return;
                crw.RewriteCalls(proc);
				crw.RewriteReturns(proc);
			}
		}

		/// <summary>
		/// Creates a signature for this procedure, and ensures that all 
        /// registers accessed by the procedure are in the procedure
		/// Frame.
		/// </summary>
		public void EnsureSignature(Procedure proc, ProcedureFlow flow)
		{
			if (proc.Signature.ParametersValid)
				return;

			var sb = new SignatureBuilder(proc.Frame, proc.Architecture);
			var frame = proc.Frame;
			if (flow.grfLiveOut != 0)
			{
				sb.AddFlagGroupReturnValue(flow.grfLiveOut, frame);
			}

            var implicitRegs = Program.Platform.CreateImplicitArgumentRegisters();
            var mayUse = new HashSet<RegisterStorage>(flow.MayUse);
            mayUse.ExceptWith(implicitRegs);
			foreach (var reg in mayUse.OrderBy(r => r.Number))
			{
				if (!IsSubRegisterOfRegisters(reg, mayUse))
				{
					sb.AddRegisterArgument(reg);
				}
			}

			foreach (var id in GetSortedStackArguments(proc.Frame).Values)
			{
				AddStackArgument(id, flow, sb);
			}

            foreach (KeyValuePair<int, Identifier> de in GetSortedFpuStackArguments(proc.Frame, 0))
			{
				sb.AddFpuStackArgument(de.Key, de.Value);
			}

            var liveOut = new HashSet<RegisterStorage>(flow.LiveOut);
            liveOut.ExceptWith(implicitRegs);

            // Sort the names in a stable way to avoid regression tests failing.
			foreach (var r in liveOut.OrderBy(r => r.Number).ThenBy(r => r.BitAddress))
			{
				if (!IsSubRegisterOfRegisters(r, liveOut))
				{
					sb.AddOutParam(frame.EnsureRegister(r));
				}
			}

            foreach (KeyValuePair<int, Identifier> de in GetSortedFpuStackArguments(proc.Frame, -proc.Signature.FpuStackDelta))
			{
				int i = de.Key;
				if (i <= proc.Signature.FpuStackOutArgumentMax)
				{
					sb.AddOutParam(frame.EnsureFpuStackVariable(i, de.Value.DataType));
				}
			}

            var sig = sb.BuildSignature();
            flow.Signature = sig;
			proc.Signature = sig;
		}

		public SortedList<int, Identifier> GetSortedArguments(Frame f, Type type, int startOffset)
		{
			SortedList<int, Identifier> arguments = new SortedList<int,Identifier>();
			foreach (Identifier id in f.Identifiers)
			{
				if (id.Storage.GetType() == type)
				{
					int externalOffset = f.ExternalOffset(id);		//$REFACTOR: do this with BindToExternalFrame.
					if (externalOffset >= startOffset)
					{
						Identifier vOld;
                        if (!arguments.TryGetValue(externalOffset, out vOld) ||
                            vOld.DataType.Size < id.DataType.Size)
                        {
                            arguments[externalOffset] = id;
                        }
					}
				}
			}
			return arguments;
		}

		/// <summary>
		/// Returns a list of all stack arguments accessed, indexed by their offsets
		/// as seen by a caller. I.e. the first argument is at offset 0, &c.
		/// </summary>
        public SortedList<int, Identifier> GetSortedStackArguments(Frame frame)
		{
			return GetSortedArguments(frame, typeof (StackArgumentStorage), 0);
		}

        public SortedList<int, Identifier> GetSortedFpuStackArguments(Frame frame, int d)
		{
			return GetSortedArguments(frame, typeof (FpuStackStorage), d);
		}

		/// <summary>
		/// Returns true if the register is a strict subregister of one of the registers in the bitset.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="regs"></param>
		/// <returns></returns>
		private bool IsSubRegisterOfRegisters(RegisterStorage rr, HashSet<RegisterStorage> regs)
		{
			foreach (var r2 in regs)
			{
				if (rr.IsSubRegisterOf(r2))
					return true;
			}
			return false;
		}
	}
}
