/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Analysis
{
	public class SsaIdentifier
	{
        private Identifier id;
        private Identifier idOrig;
        private Statement def;
        private Expression exprDef;
        private List<Statement> uses;
        private LinearInductionVariable iv; 
        private bool isSideEffect;

		public SsaIdentifier(Identifier id, Identifier idOrig, Statement stmDef, Expression exprDef, bool isSideEffect)
		{
			if (id == null)
				throw new ArgumentNullException("id");
			if (idOrig == null)
				throw new ArgumentNullException("idOrig");
			this.id = id;
			this.idOrig = idOrig;
			this.def = stmDef;
            this.exprDef = exprDef;
            this.isSideEffect = isSideEffect;
			this.uses = new List<Statement>();
		}

        /// <summary>
        /// Expression that defines identifier.
        /// </summary>
        public Expression DefExpression
        {
            get { return exprDef; }
            set { exprDef = value; }
        }

        /// <summary>
        /// Statement that defines the identifier
        /// </summary>
        public Statement DefStatement
        {
            get { return def; }
            set { def = value; }
        }

        /// <summary>
        /// The Identifier itself
        /// </summary>
        public Identifier Identifier
        {
            get { return id; }
        } 

		public bool IsOriginal
		{
			get { return id.Number == idOrig.Number; }
		}

        public bool IsSideEffect
        {
            get { return isSideEffect; }
        }

        /// <summary>
        /// If not null, the induction variable associated with this identifier.
        /// </summary>
        public LinearInductionVariable InductionVariable
        {
            get { return iv; }
            set { iv = value; }
        }

        /// <summary>
        /// The original name of the identifier.
        /// </summary>
        public Identifier OriginalIdentifier
        {
            get { return idOrig; }
        } 

		public override string ToString()
		{
			StringWriter sb = new StringWriter();
			Write(sb);
			return sb.ToString();
		}

        /// <summary>
        /// _Bag_ of Statements that use the identifier. A bag is needed, since the statement a = i * i uses i twice.
        /// </summary>
        public List<Statement> Uses
        {
            get { return uses; }
        } 


		public void Write(TextWriter writer)
		{
			if (IsOriginal)
			{
				writer.Write("{0}:", id);
				idOrig.Storage.Write(writer);
			}
			else
			{
				writer.Write("{0}: orig: {1}", id, idOrig);
			}
			if (def != null)
			{
                writer.WriteLine();
				writer.Write("    def:  {0}", def.Instruction);
			}
			if (uses.Count > 0)
			{
                bool first = true;
				foreach (Statement u in uses)
				{
                    writer.WriteLine();
                    if (first)
                    {
                        writer.Write("    uses: ");
                        first = false;
                    }
                    else
                    {
                        writer.Write("          ");
                    }
					writer.Write(u.Instruction);
				}
			}
		}
	}
}
