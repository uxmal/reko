#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
    }
}
