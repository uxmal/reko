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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// A web is the union of one or more <see cref="SsaIdentifier">SSA identifiers</see>.
    /// </summary>
    public class Web
	{
        /// <summary>
        /// Constructs an empty web.
        /// </summary>
		public Web()
		{
			this.Members = [];
			this.Definitions = [];
			this.Uses = [];
		}

        /// <summary>
        /// Identifier of the web.
        /// </summary>
        public Identifier? Identifier { get; private set; }

        /// <summary>
        /// <see cref="SsaIdentifier"/>s that are members of this web.
        /// </summary>
        public HashSet<SsaIdentifier> Members { get; }

        /// <summary>
        /// All statements that use one or more identifiers in this web.
        /// </summary>
        public HashSet<Statement> Uses { get; }

        /// <summary>
        /// All statements that define one or more identifiers in this web.
        /// </summary>
        public HashSet<Statement> Definitions { get; }

        /// <summary>
        /// Adds a <see cref="SsaIdentifier"/> to the web.
        /// </summary>
        /// <param name="sid"></param>
        public void Add(SsaIdentifier sid)
		{
            if (!Members.Add(sid))
                return;
			if (this.Identifier is null)
			{
				this.Identifier = sid.Identifier;
			}
			else
			{
				if (string.Compare(sid.Identifier.Name, this.Identifier.Name) < 0)
				{
					this.Identifier = sid.Identifier;
				}

				if (InductionVariable is null)
				{
					InductionVariable = sid.InductionVariable;
				}
				else if (sid.InductionVariable is null)
				{
					sid.InductionVariable = InductionVariable ;
				}
				else 
				{
					InductionVariable  = LinearInductionVariable.Merge(sid.InductionVariable, InductionVariable );
					if (InductionVariable  is null)
					{
						// Warning(string.Format("{0} and {1} are conflicting induction variables: {2} {3}", 
					}
					sid.InductionVariable = InductionVariable ;
				}
			}
			Definitions.Add(sid.DefStatement);
			foreach (Statement u in sid.Uses)
				Uses.Add(u);
		}

        /// <summary>
        /// The induction variable associated with this web, or null
        /// of no such association exists.
        /// </summary>
		public LinearInductionVariable? InductionVariable { get; private set; }

        /// <summary>
        /// Writes a human-readable representation of the web to the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/> instance to write to.</param>
		public void Write(TextWriter writer)
		{
			writer.Write("{0}: {{ ", Identifier!.Name);
			foreach (SsaIdentifier m in Members.OrderBy(mm => mm.Identifier.Name))
			{
				writer.Write("{0} ", m.Identifier.Name);
			}
			writer.WriteLine("}");
		}
	}
}
