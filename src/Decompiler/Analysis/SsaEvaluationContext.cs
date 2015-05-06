#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Evaluation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Analysis
{
    public class SsaEvaluationContext : EvaluationContext
    {
        private SsaIdentifierCollection ssaIds;

        public SsaEvaluationContext(SsaIdentifierCollection ssaIds)
        {
            this.ssaIds = ssaIds;
        }

        public Statement Statement { get; set; }

        public Expression GetValue(Identifier id)
        {
            if (id == null)
                return null;
            var sid = ssaIds[id];
            if (sid.DefStatement == null)
                return null;
            var ass = sid.DefStatement.Instruction as Assignment;
            if (ass != null && ass.Dst == sid.Identifier)
            {
                return ass.Src;
            }
            return null;
        }

        public Expression GetValue(Application appl)
        {
            return appl;
        }

        public Expression GetValue(MemoryAccess access)
        {
            return access;
        }

        public Expression GetValue(SegmentedAccess access)
        {
            return access;
        }

        public Expression GetDefiningExpression(Identifier id)
        {
            return ssaIds[id].DefExpression;
        }

        public void RemoveIdentifierUse(Identifier id)
        {
            if (id != null)
                ssaIds[id].Uses.Remove(Statement);
        }

        public void SetValue(Identifier id, Expression value)
        {
            throw new NotSupportedException();
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            throw new NotSupportedException();
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            throw new NotSupportedException();
        }

        public void UseExpression(Expression exp)
        {
            if (Statement == null)
                return;
            var xu = new ExpressionUseAdder(Statement, ssaIds);
            exp.Accept(xu);
        }

        public bool IsUsedInPhi(Identifier id)
        {
            var src = ssaIds[id].DefStatement;
            if (src == null)
                return false;
            var assSrc = src.Instruction as Assignment;
            if (assSrc == null)
                return false;
            new DefinedIdentifierFinder();
            return UsedIdentifierFinder.Find(ssaIds, assSrc.Src)
                .Select(c => ssaIds[c].DefStatement)
                .Where(d => d != null)
                .Select(ph => ph.Instruction as PhiAssignment)
                .Where(ph => ph != null)
                .Any();
                //.SelectMany(ph => ssaIds[ph.Ops].DefStatement)
                //.Where(opDef => IsOverwriting(opDef) &&
                //    ( (opDef == src || 
                //        PathFromTo(src, opDef) && PathFromTo(opDef, src))))
                //.Any();
            /*
             function shouldPropagateInto(r )
src := the assignment dening r
for each subscripted component c of the RHS of r
  if the denition for c is a phi-function ph then
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
