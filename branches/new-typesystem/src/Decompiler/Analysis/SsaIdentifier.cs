/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Collections;
using System.IO;

namespace Decompiler.Analysis
{
	public class SsaIdentifier
	{
		public readonly Identifier id;			// The name of the identifier.
		public readonly Identifier idOrig;		// The original name of the identifier.
		public Statement def;			// Statement that defines the identifier.
		public readonly ArrayList uses;			// _bag_ of Statements that use the identifier, since a = i * i uses i twice.
		public LinearInductionVariable iv; // If not null, the induction variable associated with this identifier.

		public SsaIdentifier(Identifier id, Identifier idOrig, Statement stmDef)
		{
			if (id == null)
				throw new ArgumentNullException("id");
			if (idOrig == null)
				throw new ArgumentNullException("idOrig");
			this.id = id;
			this.idOrig = idOrig;
			this.def = stmDef;
			this.uses = new ArrayList();
		}

		public bool IsOriginal
		{
			get { return id.Number == idOrig.Number; }
		}

		public override string ToString()
		{
			StringWriter sb = new StringWriter();
			Write(sb);
			return sb.ToString();
		}

		public void Write(TextWriter writer)
		{
			if (IsOriginal)
			{
				//$TODO: duplication is completely unnecesary; reduce to
				//writer.Write(id + ":");
				writer.Write("{0}: {1}:", id, idOrig);
				idOrig.Storage.Write(writer);
			}
			else
			{
				writer.Write("{0}: orig: {1}", id, idOrig);
			}
			if (def != null)
			{
				writer.Write(", def: {{{0}}}", def.Instruction);
			}
			if (uses.Count > 0)
			{
				writer.Write(", uses: ");
				foreach (Statement u in uses)
				{
					writer.Write("{{{0}}}", u.Instruction);
				}
			}
		}
	}
}
