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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Structure
{
    [Obsolete("", true)]
    public abstract class Conditional
    {
        private StructureNode follow;

        public Conditional(StructureNode follow)
        {
            this.follow = follow;
        }

        public StructureNode Follow
        {
            get { return follow; }
        }

    }

    [Obsolete("", true)]
    public abstract class IfConditional : Conditional
    {
        public IfConditional(StructureNode follow) : base(follow) { } 


        [Obsolete("", true)]
        private AbsynIf EmitIfCondition(Expression exp, Conditional cond, AbsynStatementEmitter emitter)
        {
            if (cond is IfElse || cond is IfThenElse)
            {
                exp = exp.Invert();
            }
            AbsynIf ifStm = new AbsynIf();
            ifStm.Condition = exp;
            emitter.EmitStatement(ifStm);

            return ifStm;
        }

    [Obsolete("", true)]

        private bool HasSingleIfThenElseStatement(List<AbsynStatement> stms)
        {
            if (stms.Count == 0)
                return false;
            AbsynIf ifStm = stms[0] as AbsynIf;
            if (ifStm == null)
                return false;
            return ifStm.Then.Count > 0 && ifStm.Else.Count > 0;
        }

        public abstract StructureNode FirstBranch(StructureNode node);
        public abstract StructureNode SecondBranch(StructureNode node);

    }

    [Obsolete("", true)]
    public class IfThen : IfConditional
    {
        public IfThen(StructureNode follow) : base(follow) { } 
        
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Then;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Else;
        }
    }

    [Obsolete("", true)]
    public class IfElse : IfConditional
    {
        public IfElse(StructureNode follow) : base(follow) { } 
        
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Else;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Then;
        }

    }

    [Obsolete("", true)]
    public class IfThenElse : IfConditional
    {
        public IfThenElse(StructureNode follow) : base(follow) { } 

        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Else;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Then;
        }
    }

    [Obsolete("", true)]
    public class Case : Conditional
    {
        public Case(StructureNode follow) : base(follow) { }

    }
}
