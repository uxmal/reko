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

using System;
using System.Collections.Generic;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core;
using Reko.Core.Code;

namespace Reko.UnitTests.Structure
{
    internal class AbsynCodeEmitter : ExpressionEmitter
    {
        private List<AbsynStatement> stmts;

        public AbsynCodeEmitter(List<AbsynStatement> stmts)
        {
            this.stmts = stmts;
        }

        public AbsynAssignment Assign(Expression dst, Expression src)
        {
            var ass = new AbsynAssignment(dst, src);
            stmts.Add(ass);
            return ass;
        }

        public void Declare(Identifier id, Expression initializer=null)
        {
            var decl = new AbsynDeclaration(id, initializer);
            stmts.Add(decl);
        }

        public void DoWhile(Action<AbsynCodeEmitter> bodyGen, BinaryExpression cond)
        {
            var bodyStmts = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(bodyStmts);
            bodyGen(m);
            stmts.Add(new AbsynDoWhile(bodyStmts, cond));
        }

        public void For(
            Func<AbsynCodeEmitter, AbsynAssignment> init,
            Func<AbsynCodeEmitter, Expression> cond,
            Func<AbsynCodeEmitter, AbsynAssignment> update,
            Action<AbsynCodeEmitter> body)
        {
            var initStms = new List<AbsynStatement>();
            var initEmitter = new AbsynCodeEmitter(initStms);
            var initStm = init(initEmitter);
            var condExp = cond(initEmitter);
            var updateStm = update(initEmitter);
            var bodyStms = new List<AbsynStatement>();
            var bodyEmitter = new AbsynCodeEmitter(bodyStms);
            body(bodyEmitter);
            this.stmts.Add(new AbsynFor(initStm, condExp, updateStm, bodyStms));
        }

        public void If(Expression id, Action<AbsynCodeEmitter> then)
        {
            var thenStmts = new List<AbsynStatement>();
            var thenEmitter = new AbsynCodeEmitter(thenStmts);
            then(thenEmitter);
            stmts.Add(new AbsynIf(id, thenStmts));
        }

        public void If(Expression id, Action<AbsynCodeEmitter> then, Action<AbsynCodeEmitter> els)
        {
            var thenStmts = new List<AbsynStatement>();
            var thenEmitter = new AbsynCodeEmitter(thenStmts);
            then(thenEmitter);
            var elseStmts = new List<AbsynStatement>();
            var elseEmitter = new AbsynCodeEmitter(elseStmts);
            els(elseEmitter);
            stmts.Add(new AbsynIf(id, thenStmts, elseStmts));
        }

        public void Return(Expression expr = null)
        {
            var ret = new AbsynReturn(expr);
            stmts.Add(ret);
        }

        public void SideEffect(Expression e)
        {
            var s = new AbsynSideEffect(e);
            stmts.Add(s);
        }

        public void While(Expression cond, Action<AbsynCodeEmitter> bodyGen)
        {
            var bodyStmts = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(bodyStmts);
            bodyGen(m);
            stmts.Add(new AbsynWhile(cond, bodyStmts));
        }

   
    }
}