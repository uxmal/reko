#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Try to guess FPU stack delta of call instructions.
    /// </summary>
    /// <remarks>
    /// FPU stack delta of hell nodes is unknown. But we can guess it based on
    /// FPU stack variable uses after call.
    /// </remarks>
    /// <example>
    /// If we have
    /// <code>
    ///     call eax_1()
    ///         defs: Top_3
    ///     // Extract return from FPU stack
    ///     edx_4 = ST_2[Top_3]
    ///     Top_5 = Top_3 + 1
    /// </code>
    /// then we can assume that FPU stack delta is -1. Of course,
    /// `ST_2[Top_3]` could be defined before indirect call, but it's unlikely
    /// </example>
    public class FpuStackReturnGuesser
    {
        private readonly SsaState ssa;
        private readonly SsaMutator ssam;
        private readonly SsaIdentifierTransformer ssaIdTransformer;

        public FpuStackReturnGuesser(SsaState ssa)
        {
            this.ssa = ssa;
            this.ssam = new SsaMutator(ssa);
            this.ssaIdTransformer = new SsaIdentifierTransformer(ssa);
        }

        public void Rewrite()
        {
            var fpuStack = ssa.Procedure.Architecture.FpuStackRegister;
            if (fpuStack == null)
                return;
            var fpuStackIds = ssa.Identifiers
                .Where(
                    sid => sid.DefStatement != null &&
                    sid.Identifier.Storage == fpuStack)
                .ToList();
            foreach (var sid in fpuStackIds)
            {
                if (!(sid.DefStatement.Instruction is CallInstruction ci))
                    continue;
                var callStm = sid.DefStatement;
                // If FPU stack variable was not used after call then assume
                // that FPU stack was preserved. Assume that offset is -1
                // otherwise.
                int delta = WasUsed(sid) ? -1 : 0;
                ssam.AdjustRegisterAfterCall(callStm, ci, fpuStack, delta);
                var fpuDefs = CreateFpuStackTemporaryBindings(delta);
                AddFpuToCallDefs(fpuDefs, callStm, ci);
                InsertFpuStackAccessAssignments(fpuDefs, callStm, ci);
            }
        }

        private bool WasUsed(SsaIdentifier sid)
        {
            var fpuStackFinder = new FpuStackUsesFinder();
            foreach (var stm in sid.Uses)
            {
                if (fpuStackFinder.WasUsed(stm.Instruction, sid.Identifier))
                    return true;
            }
            return false;
        }

        private List<CallBinding> CreateFpuStackTemporaryBindings(
            int fpuStackDelta)
        {
            var fpuDefs = new List<CallBinding>();
            for (int offset = fpuStackDelta; offset < 0; offset++)
            {
                var stg = new FpuStackStorage(
                    offset,
                    PrimitiveType.Real64);
                var name = $"rRet{stg.FpuStackOffset - offset}";
                var id = ssa.Procedure.Frame.CreateTemporary(
                    name,
                    stg.DataType);
                var fpuDefSid = ssa.Identifiers.Add(id, null, null, false);
                var fpuDefId = fpuDefSid.Identifier;
                fpuDefs.Add(new CallBinding(stg, fpuDefId));
            }
            return fpuDefs;
        }

        private void AddFpuToCallDefs(
            IEnumerable<CallBinding> fpuDefs,
            Statement stm,
            CallInstruction ci)
        {
            ci.Definitions.UnionWith(fpuDefs);
            foreach (var fpuDef in fpuDefs)
            {
                var fpuDefId = (Identifier)fpuDef.Expression;
                var fpuDefSid = ssa.Identifiers[fpuDefId];
                fpuDefSid.DefExpression = ci.Callee;
                fpuDefSid.DefStatement = stm;
            }
        }

        private void InsertFpuStackAccessAssignments(
            List<CallBinding> fpuDefs,
            Statement stmAfter,
            CallInstruction ci)
        {
            var arch = ssa.Procedure.Architecture;
            foreach (var fpuDef in fpuDefs)
            {
                var fpuStackStorage = (FpuStackStorage) fpuDef.Storage;
                var fpuAccess = arch.CreateFpuStackAccess(
                    ssa.Procedure.Frame,
                    fpuStackStorage.FpuStackOffset,
                    PrimitiveType.Real64);
                var fpuAss = new Store(fpuAccess, fpuDef.Expression);
                var fpuStm = ssam.InsertStatementAfter(fpuAss, stmAfter);
                ssaIdTransformer.Transform(fpuStm, ci);
            }
        }

        private class FpuStackUsesFinder : InstructionVisitorBase
        {
            private Identifier id;
            private bool wasUsed;

            public bool WasUsed(Instruction instr, Identifier id)
            {
                this.id = id;
                this.wasUsed = false;
                instr.Accept(this);
                return wasUsed;
            }

            public override void VisitMemoryAccess(MemoryAccess access)
            {
                if (access.EffectiveAddress == id)
                {
                    wasUsed = true;
                }
            }
        }
    }
}
