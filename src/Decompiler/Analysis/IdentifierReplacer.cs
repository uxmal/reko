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

namespace Reko.Analysis
{
    /// <summary>
    /// Replaces all occurences of an identifier with an expression.
    /// </summary>
    public class IdentifierReplacer : InstructionTransformer
    {
        private readonly SsaIdentifierCollection ssaIds;
        private readonly Statement use;
        private readonly Identifier idOld;
        private readonly Expression exprNew;
        private readonly bool replaceDefinitions;

        /// <summary>
        /// Constructs an instance of <see cref="IdentifierReplacer"/>
        /// </summary>
        /// <param name="ssaIds">Current SSA state</param>
        /// <param name="use">The statement where the replacement is being made.</param>
        /// <param name="idOld">The <see cref="Identifier"/> to replace.</param>
        /// <param name="exprNew">The <see cref="Expression"/> that replaces all occurrences <paramref name="idOld"/>.</param>
        /// <param name="replaceDefinitions">If true, identifiers will be replaced even
        /// in expression contexts when the identifier is being defined; otherwise such 
        /// identifers will not be resplaced.
        /// </param>
        public IdentifierReplacer(SsaIdentifierCollection ssaIds, Statement use, Identifier idOld, Expression exprNew, bool replaceDefinitions)
        {
            this.ssaIds = ssaIds;
            this.use = use;
            this.idOld = idOld;
            this.exprNew = exprNew;
            this.replaceDefinitions = replaceDefinitions;
        }

        /// <inheritdoc/>
        public override Instruction TransformAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(this);
            if (replaceDefinitions)
            {
                a.Dst = (Identifier)a.Dst.Accept(this);
            }
            return a;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override Expression VisitIdentifier(Identifier id)
        {
            if (idOld == id)
            {
                ssaIds[id].Uses.Remove(use);
                var usedIds = ExpressionIdentifierUseFinder.Find(exprNew);
                foreach(var usedId in usedIds)
                {
                    ssaIds[usedId].Uses.Add(use);
                }
                return exprNew;
            }
            else
                return id;
        }

        /// <inheritdoc/>
        public override Instruction TransformCallInstruction(CallInstruction ci)
        {
            foreach (var use in ci.Uses)
            {
                use.Expression = use.Expression.Accept(this);
            }
            return base.TransformCallInstruction(ci);
        }

        /// <inheritdoc/>
        public override Expression VisitOutArgument(OutArgument outArg)
        {
            if (!this.replaceDefinitions && outArg.Expression is Identifier)
                return outArg;
            var eNew = outArg.Expression.Accept(this);
            return new OutArgument(outArg.DataType, eNew); 
        }
    }
}
