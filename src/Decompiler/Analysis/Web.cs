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
	public class Web
	{
		public Identifier id;
		private ArrayList members;
		public ArrayList defs;
		public ArrayList uses;
		private LinearInductionVariable iv;

		public Web()
		{
			members = new ArrayList();
			defs = new ArrayList();
			uses = new ArrayList();
		}

		public void Add(SsaIdentifier sid)
		{
			if (!members.Contains(sid))		// should be a set!
			{
				members.Add(sid);
				if (this.id == null)
				{
					this.id = sid.id;
				}
				else
				{
					if (sid.id.Number < this.id.Number)
					{
						this.id = sid.id;
					}

					if (iv == null)
					{
						iv = sid.iv;
					}
					else if (sid.iv == null)
					{
						sid.iv = iv;
					}
					else 
					{
						iv = LinearInductionVariable.Merge(sid.iv, iv);
						if (iv == null)
						{
							// Warning(string.Format("{0} and {1} are conflicting induction variables: {2} {3}", 
						}
						sid.iv = iv;
					}
				}
				defs.Add(sid.def);
				foreach (Statement u in sid.uses)
					uses.Add(u);
			}
		}

		public LinearInductionVariable InductionVariable
		{
			get { return iv; }
		}

		public ArrayList Members
		{
			get { return members; }
		}

		public void Write(TextWriter writer)
		{
			writer.Write("{0}: {{ ", id.Name);
			foreach (SsaIdentifier m in members)
			{
				writer.Write("{0} ", m.id.Name);
			}
			writer.WriteLine("}");
		}
	}
}
