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
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Analysis
{
	/// <summary>
	/// Chases a chain of statements to locate the expression that
	/// defines the value of a condition code.
	/// </summary>
    //$REVIEW: appears unused / folded into ConditionCodeEliminator?
	public class GrfDefinitionFinder : InstructionVisitorBase
	{
		private readonly SsaIdentifierCollection ssaIds;
		private SsaIdentifier? sid;
		private bool negated;
		private Statement? stm;
		private Expression? defExpr;

        /// <summary>
        /// Constructs an instance of <see cref="GrfDefinitionFinder"/>.
        /// </summary>
        /// <param name="ssaIds"></param>
		public GrfDefinitionFinder(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		/// <summary>
		/// Chases a chain statements to locate the expression that
		/// defines the value of a condition code.
		/// </summary>
		/// <param name="sid">SSA identifier whose original definition
        /// is sought.</param>
		public void FindDefiningExpression(SsaIdentifier sid)
		{
			this.sid = sid;
			negated = false;
			stm = sid.DefStatement;
			if (stm is not null)
			{
				defExpr = null;
				while (stm is not null && defExpr is null)
				{
					var stmOld = stm;
					stm = null;
					stmOld.Instruction.Accept(this);
				}
			}
		}

        /// <summary>
        /// The statement that defines <see cref="GrfDefinitionFinder.sid"/>.
        /// </summary>
		public Expression? DefiningExpression
		{
			get { return defExpr; }
		}

        /// <summary>
        /// True if the expression should be negated, false if not.
        /// </summary>
		public bool IsNegated
		{
			get { return negated; }
		}

        /// <inheritdoc/>
		public override void VisitApplication(Application appl)
		{
			defExpr = appl;
		}

        /// <inheritdoc/>
		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

        /// <inheritdoc/>
        public override void VisitCallInstruction(CallInstruction ci)
        {
            foreach (var di in ci.Definitions)
            {
                if (di.Expression == sid!.Identifier)
                {
                    defExpr = new Application(ci.Callee, new UnknownType());
                    return;
                }
            }
        }

        /// <inheritdoc/>
		public override void VisitIdentifier(Identifier id)
		{
			sid = ssaIds[id];
			stm = sid.DefStatement;
		}

        /// <inheritdoc/>
		public override void VisitBinaryExpression(BinaryExpression binExp)
		{
            Identifier? id = ConditionCodeEliminator.FindSlicedFlagRegister(binExp);
            if (id is not null)
            {
                id.Accept(this);
            }
            else
            {
                defExpr = binExp;
            }
		}

        /// <inheritdoc/>
		public override void VisitConditionOf(ConditionOf cof)
		{
			defExpr = cof;
		}

        /// <inheritdoc/>
		public override void VisitDefInstruction(DefInstruction def)
		{
            defExpr = def.Identifier;
        }

        /// <inheritdoc/>
        public override void VisitMemoryAccess(MemoryAccess access)
        {
            defExpr = access;
        }

        /// <inheritdoc/>
        public override void VisitPhiFunction(PhiFunction phi)
		{
			defExpr = phi;
		}

        /// <inheritdoc/>
        public override void VisitUnaryExpression(UnaryExpression unary)
		{
            if (unary is not null && unary.Operator.Type == OperatorType.Not)
            {
                negated = !negated;
                if (unary.Expression is Identifier id)
                {
                    stm = ssaIds[id].DefStatement;
                }
                else
                {
                    unary.Expression.Accept(this);
                }
            }
		}
	}
}
