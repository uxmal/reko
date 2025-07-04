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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using System;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Implementation of the <see cref="EvaluationContext"/> interface
    /// for use with the <see cref="SsaState">SSA state</see> of a procedure.
    /// </summary>
    public class SsaEvaluationContext : EvaluationContext
    {
        private readonly IProcessorArchitecture arch;
        private readonly SsaIdentifierCollection ssaIds;
        private readonly IDynamicLinker dynamicLinker;

        /// <summary>
        /// Constructs an instance of <see cref="SsaEvaluationContext"/>.
        /// </summary>
        /// <param name="arch">Current <see cref="IProcessorArchitecture"/>.</param>
        /// <param name="ssaIds">The <see cref="SsaIdentifier"/></param>
        /// <param name="dynamicLinker"></param>
        public SsaEvaluationContext(
            IProcessorArchitecture arch, 
            SsaIdentifierCollection ssaIds, 
            IDynamicLinker dynamicLinker)
        {
            this.arch = arch;
            this.ssaIds = ssaIds;
            this.dynamicLinker = dynamicLinker;
        }

        /// <inheritdoc/>
        public EndianServices Endianness => arch.Endianness;

        /// <inheritdoc/>
        public int MemoryGranularity => arch.MemoryGranularity;

        /// <inheritdoc/>
        public Statement? Statement { get; set; }

        /// <inheritdoc/>
        public Expression? GetValue(Identifier id)
        {
            if (id is null)
                return null;
            var sid = ssaIds[id];
            if (sid.DefStatement is null)
                return null;
            if (sid.DefStatement.Instruction is Assignment ass &&
                ass.Dst == sid.Identifier)
            {
                return ass.Src;
            }
            if (sid.DefStatement.Instruction is PhiAssignment phiAss &&
                phiAss.Dst == sid.Identifier)
            {
                return phiAss.Src;
            }
            return null;
        }

        /// <inheritdoc/>
        public Expression GetValue(Application appl)
        {
            return appl;
        }

        /// <inheritdoc/>
        public Expression GetValue(MemoryAccess access, IMemory memory)
        {
            if (access.EffectiveAddress is Constant c &&
                // Search imported procedures only in Global Memory
                access.MemoryId.Storage == MemoryStorage.Instance)
            {
                var pc = dynamicLinker.ResolveToImportedValue(this.Statement!, c);
                if (pc is not null)
                    return pc;
            }
            return access;
        }

        /// <inheritdoc/>
        public Expression? GetDefiningExpression(Identifier id)
        {
            return ssaIds[id].GetDefiningExpression();
        }

        /// <inheritdoc/>
        public Expression MakeSegmentedAddress(Constant seg, Constant off)
        {
            return arch.MakeSegmentedAddress(seg, off);
        }


        /// <inheritdoc/>
        public Constant ReinterpretAsFloat(Constant rawBits)
        {
            return arch.ReinterpretAsFloat(rawBits);
        }

        /// <inheritdoc/>
        public void RemoveExpressionUse(Expression exp)
        {
            if (Statement is null)
                return;
            var xu = new ExpressionUseRemover(Statement, ssaIds);
            exp.Accept(xu);
        }

        /// <inheritdoc/>
        public void SetValue(Identifier id, Expression value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void SetValueEa(Expression ea, Expression value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void UseExpression(Expression exp)
        {
            if (Statement is null)
                return;
            var xu = new InstructionUseAdder(Statement, ssaIds);
            exp.Accept(xu);
        }

        /// <inheritdoc/>
        public bool IsUsedInPhi(Identifier id)
        {
            var src = ssaIds[id].DefStatement;
            if (src is null)
                return false;
            if (src.Instruction is not Assignment assSrc)
                return false;
            return ExpressionIdentifierUseFinder.Find(assSrc.Src)
                .Select(c => ssaIds[c].DefStatement)
                .Where(d => d is not null)
                .Select(ph => ph!.Instruction as PhiAssignment)
                .Where(ph => ph is not null)
                .Any();
                //.SelectMany(ph => ssaIds[ph.Ops].DefStatement)
                //.Where(opDef => IsOverwriting(opDef) &&
                //    ( (opDef == src || 
                //        PathFromTo(src, opDef) && PathFromTo(opDef, src))))
                //.Any();
            /*
             function shouldPropagateInto(r )
src := the assignment defining r
for each subscripted component c of the RHS of r
  if the definition for c is a phi-function ph then
    for each operand op of ph
      opdef := the denition for op
      if opdef is an overwriting statement and either
          (opdef = src) or
          (there exists a CFG path from src to opdef and
               from opdef to the statement containing r ) then
         return false
             */
        }
    }
}
