#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

        public void Return()
        {
            var ret = new AbsynReturn(null);
            stmts.Add(ret);
        }

        public void If(Identifier id, Action<AbsynCodeEmitter> then)
        {
            var thenStmts = new List<AbsynStatement>();
            var thenEmitter = new AbsynCodeEmitter(thenStmts);
            then(thenEmitter);
            stmts.Add(new AbsynIf(id, thenStmts));
        }

        public void SideEffect(Expression e)
        {
            var s = new AbsynSideEffect(e);
            stmts.Add(s);
        }
    }
}