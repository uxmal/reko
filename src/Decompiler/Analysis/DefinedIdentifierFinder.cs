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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
	/// <summary>
	/// Locates the variables defined in this block by examining each
	/// statement to find variables in L-value positions.
	/// In addition, the set deadIn for each block is calculated.
	/// These are all the variables that are known to be dead on
	/// entry to the function. Dead variables won't need phi code!
	/// </summary>
	public class DefinedIdentifierFinder
	{
        private Dictionary<Identifier, Definition> definitions;
        private InnerFinder f;

		public DefinedIdentifierFinder()
		{
            definitions = new Dictionary<Identifier, Definition>();
            f = new InnerFinder(definitions);
		}

        public Dictionary<Identifier, Definition> Definitions
        {
            get { return definitions; }
        }

        public void FindDefinitions(Statement def)
        {
            def.Instruction.Accept(f);
        }

        public class Definition
        {
            private Expression expr;
            private bool isSideEffect;

            public Definition(Expression expr, bool isSideEffect)
            {
                this.expr = expr;
                this.isSideEffect = isSideEffect;
            }


            public Expression Expression
            {
                get { return expr; }
            }

            public bool IsSideEffect
            {
                get { return isSideEffect; }
            }
        }

        private class InnerFinder : InstructionVisitorBase
        {
            private Dictionary<Identifier, Definition> defs;

            public InnerFinder(Dictionary<Identifier, Definition> defs)
            {
                this.defs = defs;
            }

            public override void VisitAssignment(Assignment ass)
            {
                Identifier id = ass.Dst as Identifier;
                if (id != null)
                {
                    Def(id, ass.Src, false);
                }
                else if (ass.Dst != null)
                {
                    ass.Dst.Accept(this);
                }
                ass.Src.Accept(this);
            }

            private void Def(Identifier id, Expression expr, bool isSideEffect)
            {
                defs.Add(id, new Definition(expr, isSideEffect));
            }

            public override void VisitApplication(Application app)
            {
                app.Procedure.Accept(this);
                foreach (Expression exp in app.Arguments)
                {
                    exp.Accept(this);
                }
            }

            public override void VisitOutArgument(OutArgument outArg)
            {
                Identifier id = outArg.Expression as Identifier;
                if (id != null)
                    Def(id, outArg, true);
                else
                    outArg.Expression.Accept(this);
            }
        }
    }
}