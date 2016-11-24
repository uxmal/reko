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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using CallInstruction = Reko.Core.Code.CallInstruction;
using FpuStackStorage = Reko.Core.FpuStackStorage;
using Frame = Reko.Core.Frame;
using Identifier = Reko.Core.Expressions.Identifier;
using OutArgumentStorage = Reko.Core.OutArgumentStorage;
using PrimtiveType = Reko.Core.Types.PrimitiveType;
using Procedure = Reko.Core.Procedure;
using Program = Reko.Core.Program;
using RegisterStorage = Reko.Core.RegisterStorage;
using ReturnInstruction = Reko.Core.Code.ReturnInstruction;
using SignatureBuilder = Reko.Core.SignatureBuilder;
using StackArgumentStorage = Reko.Core.StackArgumentStorage;
using UseInstruction = Reko.Core.Code.UseInstruction;
using VoidType = Reko.Core.Types.VoidType;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Code;

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
	public class CallRewriter
	{
		private ProgramDataFlow mpprocflow;
        private DecompilerEventListener listener;
        private IPlatform platform;

        public CallRewriter(IPlatform platform, ProgramDataFlow mpprocflow, DecompilerEventListener listener) 
		{
            this.platform = platform;
			this.mpprocflow = mpprocflow;
            this.listener = listener;
        }

        public void AddStackArgument(int x, Identifier id, ProcedureFlow flow, SignatureBuilder sb)
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
						id.DataType = PrimtiveType.Create(pt.Domain, bitWidth/8);
					}
				}
			}
			sb.AddStackArgument(x, id);
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

				proc.ExitBlock.Statements.Add(0, new UseInstruction(os.OriginalIdentifier, id));
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
			flow.LiveOut.IntersectWith(flow.Trashed);
		}

        private void AdjustLiveIn(Procedure proc, ProcedureFlow flow)
        {
            var liveDefStms = proc.EntryBlock.Statements
                .Select(s => s.Instruction)
                .OfType<DefInstruction>()
                .Select(d => d.Expression)
                .OfType<Identifier>()
                .Select(i => i.Storage);
            var dead = flow.BitsUsed.Keys
                .Except(liveDefStms).ToList();
            foreach (var dd in dead)
            {
                flow.BitsUsed.Remove(dd);
            }
        }

		public static void Rewrite(
            IPlatform platform, 
            List<SsaTransform> ssts,
            ProgramDataFlow summaries,
            DecompilerEventListener eventListener)
		{
			CallRewriter crw = new CallRewriter(platform, summaries, eventListener);
			foreach (SsaTransform sst in ssts)
			{
                if (eventListener.IsCanceled())
                    return;
                var proc = sst.SsaState.Procedure;
				ProcedureFlow flow = crw.mpprocflow[proc];
                flow.Dump(platform.Architecture);
				crw.EnsureSignature(proc, flow);
				crw.AddUseInstructionsForOutArguments(proc);
			}

			foreach (SsaTransform sst in ssts)
			{
                if (eventListener.IsCanceled())
                    return;
                crw.RewriteCalls(sst.SsaState.Procedure);
				crw.RewriteReturns(sst.SsaState);
			}
		}

		/// <summary>
		/// Creates a signature for this procedure by looking at the storages
        /// modified in the exit block, and ensures that all the registers
        /// accessed by the procedure are in the procedure Frame.
		/// </summary>
		public void EnsureSignature(Procedure proc, ProcedureFlow flow)
		{
            // If we already have a signature, we don't need to do this work.
			if (proc.Signature != null && proc.Signature.ParametersValid)
				return;

            var allLiveOut = proc.ExitBlock.Statements
                .Select(s => s.Instruction)
                .OfType<UseInstruction>()
                .Select(u => ((Identifier)u.Expression).Storage);
			var sb = new SignatureBuilder(proc, platform.Architecture);
			var frame = proc.Frame;
			foreach (var grf in allLiveOut
                .OfType<FlagGroupStorage>()
                .OrderBy(g => g.Name))
			{
                sb.AddOutParam(frame.EnsureFlagGroup(grf));
			}

            var liveOut = allLiveOut.OfType<RegisterStorage>().ToHashSet();

            var implicitRegs = platform.CreateImplicitArgumentRegisters();
            var mayUse = new HashSet<RegisterStorage>(flow.BitsUsed.Keys.OfType<RegisterStorage>());
            mayUse.ExceptWith(implicitRegs);
            //$BUG: should be sorted by ABI register order. Need a new method
            // IPlatform.CreateAbiRegisterCollator().
			foreach (var reg in mayUse.OfType<RegisterStorage>().OrderBy(r => r.Number))
			{
				if (!IsSubRegisterOfRegisters(reg, mayUse))
				{
					sb.AddRegisterArgument(reg);
				}
			}

			foreach (KeyValuePair<int,Identifier> de in GetSortedStackArguments(proc.Frame))
			{
				AddStackArgument(de.Key, de.Value, flow, sb);
			}

            foreach (KeyValuePair<int, Identifier> de in GetSortedFpuStackArguments(proc.Frame, 0))
			{
				sb.AddFpuStackArgument(de.Key, de.Value);
			}

            liveOut.ExceptWith(implicitRegs);
			foreach (var r in liveOut.OfType<RegisterStorage>().OrderBy(r => r.Number))
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


        //protected virtual ApplicationBuilder CreateApplicationBuilder(Procedure proc, CallInstruction call, ProcedureConstant fn)
        //{
        //    return new FrameApplicationBuilder(
        //        Program.Architecture,
        //        proc.Frame,
        //        call.CallSite,
        //        fn,
        //        true);
        //}

        private ApplicationBuilder CreateApplicationBuilder(Procedure proc, CallInstruction call, ProcedureConstant fn)
        {
            return new CallApplicationBuilder(platform.Architecture, call, fn);
        }

        /// <summary>
        /// Rewrites CALL instructions to function applications.
        /// </summary>
        /// <remarks>
        /// Converts an opcode:
        /// <code>
        ///   call procExpr 
        /// </code>
        /// to one of:
        /// <code>
        ///	 ax = procExpr(bindings);
        ///  procEexpr(bindings);
        /// </code>
        /// </remarks>
        /// <param name="proc">Procedure in which the CALL instruction exists</param>
        /// <param name="stm">The particular statement of the call instruction</param>
        /// <param name="call">The actuall CALL instruction.</param>
        /// <returns>True if the conversion was possible, false if the procedure didn't have
        /// a signature yet.</returns>
        public bool RewriteCall(Procedure proc, Statement stm, CallInstruction call)
        {
            var callee = call.Callee as ProcedureConstant;
            if (callee == null)
                return false;          //$REVIEW: what happens with indirect calls?
            var procCallee = callee.Procedure;
            var sigCallee = procCallee.Signature;
            var fn = new ProcedureConstant(platform.PointerType, procCallee);
            if (sigCallee == null || !sigCallee.ParametersValid)
                return false;
            ApplicationBuilder ab = CreateApplicationBuilder(proc, call, fn);
            var instr = ab.CreateInstruction(sigCallee, procCallee.Characteristics);
            stm.Instruction = instr;
            return true;
        }


        /// <summary>
        // Statements of the form:
        //		call	<proc-operand>
        // become redefined to 
        //		ret = <proc-operand>(bindings)
        // where ret is the return register (if any) and the
        // bindings are the bindings of the procedure.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns>The number of calls that couldn't be converted</returns>
        public int RewriteCalls(Procedure proc)
        {
            int unConverted = 0;
            foreach (Statement stm in proc.Statements)
            {
                CallInstruction ci = stm.Instruction as CallInstruction;
                if (ci != null)
                {
                    if (!RewriteCall(proc, stm, ci))
                        ++unConverted;
                }
            }
            return unConverted;
        }

        /// <summary>
        /// Having identified the return variable -- if any, rewrite all 
        /// return statements to return that variable.
        /// </summary>
        /// <param name="ssa"></param>
        public void RewriteReturns(SsaState ssa)
        {
            Identifier idRet = ssa.Procedure.Signature.ReturnValue;
            if (idRet == null || idRet.DataType is VoidType)
                return;

            // Find the returned identifier
            var exitBlock = ssa.Procedure.ExitBlock;
            var expRet = exitBlock.Statements
                .Select(s => s.Instruction)
                .OfType<UseInstruction>()
                .Select(u => new
                {
                    Identifier = u.Expression as Identifier,
                    Instruction = u
                })
                .Where(w => w.Identifier != null && w.Identifier.Storage == idRet.Storage)
                .Single();

            // Single definition
            var sid = ssa.Identifiers[expRet.Identifier];
            var phi = sid.DefStatement.Instruction as PhiAssignment;
            if (phi != null)
            {
                // Multiple reaching definitions.
                for (int i = 0; i < phi.Src.Arguments.Length; ++i)
                {
                    var pred = exitBlock.Pred[i];
                    SetReturnExpression(
                        pred, 
                        ssa.Identifiers[(Identifier)phi.Src.Arguments[i]]);
                }
                // Delete the phi statement
                ssa.DeleteStatement(sid.DefStatement);
            }
            else
            {
                // Single reaching definition.
                var block = exitBlock.Pred[0];
                SetReturnExpression(block, sid);
            }

            var stmUse = sid.Uses
                .Where(u => u.Instruction == expRet.Instruction)
                .Single();
            ssa.DeleteStatement(stmUse);
        }

        private void SetReturnExpression(Block block, SsaIdentifier sid)
        {
            for (int i = block.Statements.Count-1; i >=0; --i)
            {
                var stm = block.Statements[i];
                var ret = stm.Instruction as ReturnInstruction;
                if (ret != null)
                {
                    ret.Expression = sid.Identifier;
                    sid.Uses.Add(stm);
                }
            }
        }
    }
}
