#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Analysis
{
    /// <summary>
    /// Undoes the SSA renaming by replacing each ssa identifier with its
    /// original identifier and removing 'use' and 'def' statements, which
    /// no longer serve a purpose.
    /// </summary>
    public class UnSsaTransform : InstructionTransformer
    {
        private SsaState ssa;
        private readonly bool renameVariables;

        /// <summary>
        /// Creates a new instance of the <see cref="UnSsaTransform"/> class.
        /// </summary>
        /// <param name="renameVariables">If true, renames identifiers
        /// back to their original names before SSA analysis; otherwise
        /// the identifiers are left with their SSA names.
        /// </param>
        public UnSsaTransform(bool renameVariables)
        {
            this.ssa = default!;
            this.renameVariables = renameVariables;
        }

        /// <summary>
        /// Performs the transformation of the SSA state by removing
        /// <see cref="DefInstruction">'def' instructions</see> and
        /// <see cref="PhiAssignment">'phi' instructions</see>.
        /// </summary>
        /// <param name="ssa"><see cref="SsaState"/> to transform.</param>
        /// <returns>A <see cref="Procedure"/> with its SSA state elmininated.
        /// </returns>
        public Procedure Transform(SsaState ssa)
        {
            this.ssa = ssa;
            var proc = ssa.Procedure;
            foreach (Block block in proc.ControlGraph.Blocks)
            {
                for (int st = 0; st < block.Statements.Count; ++st)
                {
                    Instruction instr = block.Statements[st].Instruction;
                    if (instr is PhiAssignment || instr is DefInstruction)
                    {
                        block.Statements.RemoveAt(st);
                        --st;
                    }
                    else if (renameVariables)
                    {
                        instr.Accept(this);
                    }
                }
            }

            // Remove any instructions in the return block, used only 
            // for computation of reaching definitions.

            proc.ExitBlock.Statements.Clear();
            return proc;
        }

        /// <inheritdoc/>
        public override Expression VisitIdentifier(Identifier id)
        {
            return ssa.Identifiers[id].OriginalIdentifier;
        }
    }
}
