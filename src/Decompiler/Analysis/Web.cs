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
		public Web()
		{
			this.Members = new HashSet<SsaIdentifier>();
			this.Definitions = new HashSet<Statement>();
			this.Uses = new HashSet<Statement>();
		}

        public Identifier? Identifier { get; private set; }
        public HashSet<SsaIdentifier> Members { get; }
        public HashSet<Statement> Uses { get; }
        public HashSet<Statement> Definitions { get; }

        public void Add(SsaIdentifier sid)
		{
            if (Members.Contains(sid))		// should be a set!
                return;

			Members.Add(sid);
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

		public LinearInductionVariable? InductionVariable { get; private set; }

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
