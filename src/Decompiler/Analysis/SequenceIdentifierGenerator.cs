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

using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core;

namespace Reko.Analysis
{
    /// <summary>
    /// Attempts to propagate sequence identifiers to the inputs
    /// and outputs of a procedure.
    /// </summary>
    public class SequenceIdentifierGenerator : InstructionTransformer
    {
        private SsaTransform2 sst;
        private SsaState ssa;
        private Statement stmCur;

        public SequenceIdentifierGenerator(SsaTransform2 sst)
        {
            this.sst = sst;
            this.ssa = sst.SsaState;
        }

        public void Transform()
        {
            foreach (var stm in sst.SsaState.Procedure.Statements)
            {
                this.stmCur = stm;
                stm.Instruction.Accept(this);
            }
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            var idHead = seq.Head as Identifier;
            var idTail = seq.Tail as Identifier;
            if (idHead != null && idTail != null)
            {
                var sidHead = ssa.Identifiers[idHead];
                var sidTail = ssa.Identifiers[idTail];
                if (sidHead.DefStatement.Instruction is DefInstruction &&
                    sidTail.DefStatement.Instruction is DefInstruction)
                {
                    var idSeq = ssa.Procedure.Frame.EnsureSequence(
                        sidHead.OriginalIdentifier,
                        sidTail.OriginalIdentifier,
                        seq.DataType);
                    var def = ssa.Procedure.EntryBlock.Statements.Add(0, null);
                    var sidSeq = ssa.Identifiers.Add(idSeq, null, null, false);
                    sidSeq.DefStatement = def;
                    def.Instruction = new DefInstruction(sidSeq.Identifier);
                    sidSeq.Uses.Add(this.stmCur);
                    RemoveUse(sidHead);
                    RemoveUse(sidTail);
                    return sidSeq.Identifier;
                }
            }
            return seq;
        }

        private void RemoveUse(SsaIdentifier sid)
        {
            sid.Uses.Remove(stmCur);
        }
    }
}
