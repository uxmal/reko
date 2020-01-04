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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
	/// <summary>
	/// Rewrites a program, based on summary live-in and live-out
    /// information, so that all CALL codes are converted into procedure 
    /// calls, with the appropriate parameter lists.
	/// </summary>
	public class CallRewriter
	{
		private readonly ProgramDataFlow mpprocflow;
        private readonly DecompilerEventListener listener;
        private readonly IPlatform platform;

        public CallRewriter(IPlatform platform, ProgramDataFlow mpprocflow, DecompilerEventListener listener) 
		{
            this.platform = platform;
			this.mpprocflow = mpprocflow;
            this.listener = listener;
        }

        public void AddStackArgument(Identifier id, ProcedureFlow flow, SignatureBuilder sb)
		{
			sb.AddInParam(id);
		}

        private void AdjustLiveIn(Procedure proc, ProcedureFlow flow)
        {
            var liveDefStms = proc.EntryBlock.Statements
                .Select(s => s.Instruction)
                .OfType<DefInstruction>()
                .Select(d => d.Identifier.Storage);
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
				crw.EnsureSignature(sst.SsaState, proc.Frame, flow);
			}

			foreach (SsaTransform sst in ssts)
			{
                if (eventListener.IsCanceled())
                    return;
                crw.RewriteCalls(sst.SsaState);
				crw.RewriteReturns(sst.SsaState);
                crw.RemoveStatementsFromExitBlock(sst.SsaState);
			}
		}

        /// <summary>
        /// Creates a signature for this procedure by looking at the storages
        /// modified in the exit block, and ensures that all the registers
        /// accessed by the procedure are in the procedure Frame.
        /// </summary>
        public void EnsureSignature(SsaState ssa, IStorageBinder frame, ProcedureFlow flow)
        {
            var proc = ssa.Procedure;
            // If we already have a signature, we don't need to do this work.
            if (!proc.Signature.ParametersValid)
            {
                var sig = MakeSignature(ssa, frame, flow);
                flow.Signature = sig;
                proc.Signature = sig;
            }
        }

        /// <summary>
        /// Make a function signature based on the procedure flow <paramref name="flow"/>.
        /// </summary>
        /// <returns>A valid function signature.</returns>
        public FunctionType MakeSignature(SsaState ssa, IStorageBinder frame, ProcedureFlow flow)
        {
            var allLiveOut = flow.BitsLiveOut;
			var sb = new SignatureBuilder(frame, platform.Architecture);
            var implicitRegs = platform.CreateImplicitArgumentRegisters();

            var liveOutFlagGroups = flow.grfLiveOut.Select(de => platform.Architecture.GetFlagGroup(de.Key, de.Value));
            AddModifiedFlags(frame, liveOutFlagGroups, sb);

            var mayUseSeqs = flow.BitsUsed.Keys.OfType<SequenceStorage>().ToHashSet();
            var seqRegs = mayUseSeqs.SelectMany(s => s.Elements).Distinct().ToHashSet();

            //$BUG: should be sorted by ABI register order.
            foreach (var seq in mayUseSeqs.OrderBy(r => r.Name))
            {
                sb.AddSequenceArgument(seq);
            }
            var mayUseRegs = flow.BitsUsed.Where(b =>
                {
                    return b.Key is RegisterStorage reg && !implicitRegs.Contains(reg);
                })
                .ToDictionary(
                    de => platform.Architecture.GetSubregister(
                        (RegisterStorage) de.Key,
                        de.Value.Lsb,
                        de.Value.Extent) ?? (RegisterStorage)de.Key,
                    de=>de.Value);
            
            //$BUG: should be sorted by ABI register order. Need a new method
            // IPlatform.CreateAbiRegisterCollator().
			foreach (var reg in mayUseRegs.OrderBy(r => r.Key.Number))
			{
				if (!IsSubRegisterOfRegisters(reg.Key, mayUseRegs) &&
                    !seqRegs.Contains(reg.Key))
				{
					sb.AddRegisterArgument(reg.Key);
				}
			}

			foreach (var id in GetSortedStackArguments((Frame)frame, flow.BitsUsed))
			{
				AddStackArgument(id.Item2, flow, sb);
			}

            foreach (var oFpu in flow.BitsUsed
                .Where(f => f.Key is FpuStackStorage)
                .OrderBy(r => ((FpuStackStorage) r.Key).FpuStackOffset))
			{
                var fpu = (FpuStackStorage)oFpu.Key;
                var id = frame.EnsureFpuStackVariable(fpu.FpuStackOffset, fpu.DataType);
                sb.AddFpuStackArgument(fpu.FpuStackOffset, id);
			}

            var liveOut = allLiveOut
                .Select(de => (Key:de.Key as RegisterStorage, Value:de.Value))
                .Where(de =>
                {
                    return de.Key != null 
                        && !implicitRegs.Contains(de.Key);
                })
                .ToDictionary(
                     de => platform.Architecture.GetSubregister(
                        de.Key,
                        de.Value.Lsb,
                        de.Value.Extent) ?? de.Key,
                    de => de.Value);

            // Sort the names in a stable way to avoid regression tests failing.
            foreach (var r in liveOut.OrderBy(r => r.Key.Number).ThenBy(r => r.Key.BitAddress))
			{
				if (!IsSubRegisterOfRegisters(r.Key, liveOut))
				{
                    var regOut = sb.AddOutParam(frame.EnsureRegister(r.Key));
                    if (regOut.Storage is OutArgumentStorage && 
                        !ssa.Identifiers.TryGetValue(regOut, out var sidOut))
                    {
                        // Ensure there are SSA identifer for 'out' registers.
                        ssa.Identifiers.Add(regOut, null, null, false);
                    }
				}
			}

            foreach (var fpu in allLiveOut.Keys.OfType<FpuStackStorage>().OrderBy(r => r.FpuStackOffset))
            {
                sb.AddOutParam(frame.EnsureFpuStackVariable(fpu.FpuStackOffset, fpu.DataType));
			}

            var sig = sb.BuildSignature();
            return sig;
        }

        /// <summary>
        /// Make a function signature based on use and def information.
        /// </summary>
        /// <param name="ssa"></param>
        /// <param name="uses"></param>
        /// <param name="definitions"></param>
        /// <returns></returns>
        public FunctionType MakeSignature(SsaState ssa, IEnumerable<CallBinding> uses, IEnumerable<CallBinding> definitions)
        {
            var implicitRegs = platform.CreateImplicitArgumentRegisters();

            var arch = ssa.Procedure.Architecture;
            var frame = arch.CreateFrame();
            var sb = new SignatureBuilder(frame, arch);

            var seqs = uses.Select(u => u.Storage as SequenceStorage)
                .Where(s => s != null)
                .OrderBy(s => s.Name);
            foreach (var seq in seqs)
            {
                sb.AddSequenceArgument(seq);
            }

            //$TODO: sort these by some ABI order?
            var regs = uses.Select(u => u.Storage as RegisterStorage)
                .Where(r => r != null && !implicitRegs.Contains(r))
                .OrderBy(r => r.Number);
            foreach (var reg in regs)
            {
                sb.AddRegisterArgument(reg);
            }

            var stargs = uses.Select(u => u.Storage as StackStorage)
                .Where(s => s != null)
                .OrderBy(r => r.StackOffset);
            foreach (var arg in stargs)
            {
                var id = frame.EnsureIdentifier(arg);
                sb.AddInParam(id);
            }

            var outs = definitions.Select(d => d.Storage)
                .OfType<RegisterStorage>()
                .Where(r => !implicitRegs.Contains(r))
                .OrderBy(r => r.Number);
            foreach (var o in outs)
            {
                var id = frame.EnsureIdentifier(o);
                sb.AddOutParam(id);
            }
            var sig = sb.BuildSignature();
            return sig;
        }

        private void AddModifiedFlags(IStorageBinder frame, IEnumerable<Storage> allLiveOut, SignatureBuilder sb)
        {
            foreach (var grf in allLiveOut
                .OfType<FlagGroupStorage>()
                .OrderBy(g => g.Name))
            {
                sb.AddOutParam(frame.EnsureFlagGroup(grf));
            }
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
                        if (!arguments.TryGetValue(externalOffset, out var vOld) ||
                            vOld.DataType.BitSize < id.DataType.BitSize)
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
        public IEnumerable<(int, Identifier)> GetSortedStackArguments(Frame frame, IEnumerable<KeyValuePair<Storage, BitRange>> mayuse)
        {
            return mayuse
                .Select(kv => (stg: kv.Key as StackStorage, range: kv.Value))
                .Where(item => item.stg != null)
                .OrderBy(item => item.stg.StackOffset)
                .Select(item =>
                {
                    var id = frame.EnsureStackArgument(
                        item.stg.StackOffset,
                        PrimitiveType.CreateWord(item.range.Extent));
                    return (item.stg.StackOffset - frame.ReturnAddressSize, id);
                });

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
		private bool IsSubRegisterOfRegisters(RegisterStorage rr, Dictionary<RegisterStorage, BitRange> regs)
		{
			foreach (var r2 in regs.Keys)
			{
				if (rr.IsSubRegisterOf(r2))
					return true;
			}
			return false;
		}

        private ApplicationBuilder CreateApplicationBuilder(SsaState ssaCaller, Statement stmCaller, CallInstruction call, Expression fn)
        {
            return new CallApplicationBuilder(ssaCaller, stmCaller, call, fn);
        }

        public void RemoveStatementsFromExitBlock(SsaState ssa)
        {
            foreach (var stm in ssa.Procedure.ExitBlock.Statements.ToList())
            {
                ssa.DeleteStatement(stm);
            }
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
        /// <param name="ssaCaller">SSA state of the procedure in which the CALL instruction exists</param>
        /// <param name="stm">The particular statement of the call instruction</param>
        /// <param name="call">The actuall CALL instruction.</param>
        public bool RewriteCall(SsaState ssaCaller, Statement stm, CallInstruction call)
        {
            if (call.Callee is ProcedureConstant callee)
            {
                var procCallee = callee.Procedure;
                var sigCallee = procCallee.Signature;
                var fn = new ProcedureConstant(platform.PointerType, procCallee);
                if (sigCallee == null || !sigCallee.ParametersValid)
                    return false;
                ApplicationBuilder ab = CreateApplicationBuilder(ssaCaller, stm, call, fn);
                var instr = ab.CreateInstruction(sigCallee, procCallee.Characteristics);
                stm.Instruction = instr;
                var ssam = new SsaMutator(ssaCaller);
                ssam.AdjustSsa(stm, call);
                return true;
            }
            else
            {
                return false;
#if NOT_READY_YET       //$TODO
                // We have an indirect call with an unknown signature. 
                // Use the guessed `uses` and `defs` to construct a signature.
                // It's likely going to be wrong, but it can be overridden with
                // user-provided metadata.
                var sigCallee = MakeSignature(ssaCaller, call.Uses, call.Definitions);
                var ab = CreateApplicationBuilder(ssaCaller, stm, call, call.Callee);
                var instr = ab.CreateInstruction(sigCallee, Core.Serialization.DefaultProcedureCharacteristics.Instance);
                stm.Instruction = instr;
                var ssam = new SsaMutator(ssaCaller);
                ssam.AdjustSsa(stm, call);
                return true;
#endif
            }
        }

        /// <summary>
        // Statements of the form:
        //		call	<ssaCaller-operand>
        // become redefined to 
        //		ret = <ssaCaller-operand>(bindings)
        // where ret is the return register (if any) and the
        // bindings are the bindings of the procedure.
        /// </summary>
        /// <param name="ssaCeller">SSA state of the calling procedure.</param>
        /// <returns>The number of calls that couldn't be converted</returns>
        public int RewriteCalls(SsaState ssaCaller)
        {
            int unConverted = 0;
            foreach (Statement stm in ssaCaller.Procedure.Statements.ToList())
            {
                if (stm.Instruction is CallInstruction ci)
                {
                    if (!RewriteCall(ssaCaller, stm, ci))
                        ++unConverted;
                }
            }
            return unConverted;
        }

        /// <summary>
        /// Having identified the return variable -- if any -- rewrite all
        /// return statements to return that variable.
        /// </summary>
        /// <param name="ssa"></param>
        public void RewriteReturns(SsaState ssa)
        {
            // For each basic block reaching the exit block, get all reaching
            // definitions and then either replace the return expression or 
            // inject out variable assignments as Stores.

            var reachingBlocks = ssa.PredecessorPhiIdentifiers(ssa.Procedure.ExitBlock);
            var sig = ssa.Procedure.Signature;
            foreach (var reachingBlock in reachingBlocks)
            {
                var block = reachingBlock.Key;
                var idRet = sig.ReturnValue;
                if (idRet != null && !(idRet.DataType is VoidType))
                {
                    var idStg = reachingBlock.Value
                        .Where(cb => cb.Storage.Covers(idRet.Storage))
                        .FirstOrDefault();
                    SetReturnExpression(ssa, block, idRet, idStg);
                }
                int insertPos = block.Statements.FindIndex(s => s.Instruction is ReturnInstruction);
                Debug.Assert(insertPos >= 0);
                foreach (var p in sig.Parameters.Where(p => p.Storage is OutArgumentStorage))
                {
                    var outStg = (OutArgumentStorage)p.Storage;
                    var idStg = reachingBlock.Value
                        .Where(cb => cb.Storage == outStg.OriginalIdentifier.Storage)
                        .FirstOrDefault();
                    InsertOutArgumentAssignment(
                        ssa,
                        p,
                        idStg?.Expression ?? Constant.Invalid,
                        block,
                        insertPos);
                    ++insertPos;
                }
            }
        }

        private void SetReturnExpression(
            SsaState ssa,
            Block block,
            Identifier idRet,
            CallBinding idStg)
        {
            var e = idStg?.Expression ?? Constant.Invalid;
            for (int i = block.Statements.Count-1; i >=0; --i)
            {
                var stm = block.Statements[i];
                if (stm.Instruction is ReturnInstruction ret)
                {
                    if (idRet.DataType.BitSize < e.DataType.BitSize)
                    {
                        int offset = idStg.Storage.OffsetOf(idRet.Storage);
                        e = new Slice(idRet.DataType, e, offset);
                    }
                    ret.Expression = e;
                    ssa.AddUses(stm);
                }
            }
        }

        private void InsertOutArgumentAssignment(
            SsaState ssa,
            Identifier parameter,
            Expression e,
            Block block,
            int insertPos)
        {
            var iAddr = block.Statements[insertPos].LinearAddress;
            var stm = block.Statements.Insert(
                insertPos, iAddr, 
                new Store(parameter, e));
            ssa.AddUses(stm);
        }
    }
}
