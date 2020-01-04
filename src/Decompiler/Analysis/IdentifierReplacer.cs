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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// Replaces all occurences of an identifier with an expression.
    /// </summary>
    public class IdentifierReplacer : InstructionTransformer
    {
        private SsaIdentifierCollection ssaIds;
        private Statement use;
        private Identifier idOld;
        private Expression exprNew;
        private bool replaceDefinitions;

        public IdentifierReplacer(SsaIdentifierCollection ssaIds, Statement use, Identifier idOld, Expression exprNew, bool replaceDefinitions)
        {
            this.ssaIds = ssaIds;
            this.use = use;
            this.idOld = idOld;
            this.exprNew = exprNew;
            this.replaceDefinitions = replaceDefinitions;
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(this);
            ssaIds[a.Dst].DefExpression = a.Src;
            if (replaceDefinitions)
            {
                a.Dst = (Identifier)a.Dst.Accept(this);
            }
            return a;
        }

        public override Instruction TransformPhiAssignment(PhiAssignment phi)
        {
            var args = phi.Src.Arguments;
            for (int i = 0; i < args.Length; ++i)
            {
                var value = args[i].Value.Accept(this);
                args[i] = new PhiArgument(args[i].Block, value);
            }
            if (replaceDefinitions)
            {
                phi.Dst = (Identifier)phi.Dst.Accept(this);
            }
            return phi;
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            if (idOld == id)
            {
                ssaIds[id].Uses.Remove(use);
                var usedIds = ExpressionIdentifierUseFinder.Find(ssaIds, exprNew);
                foreach(var usedId in usedIds)
                {
                    ssaIds[usedId].Uses.Add(use);
                }
                return exprNew;
            }
            else
                return id;
        }

        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            foreach (var use in ci.Uses)
            {
                use.Expression = use.Expression.Accept(this);
            }
            return base.TransformCallInstruction(ci);
        }
    }
}
