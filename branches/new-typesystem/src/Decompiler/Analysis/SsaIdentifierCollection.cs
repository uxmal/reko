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

namespace Decompiler.Analysis
{
	public class SsaIdentifierCollection : CollectionBase
	{
		public void Add(SsaIdentifier sid)
		{
			List.Add(sid);
		}

		public SsaIdentifier Add(Identifier idOld, Statement stmDef)
		{
			int i = InnerList.Count;
			Identifier idNew;
			if (stmDef != null)
			{
				idNew = idOld is MemoryIdentifier
					? new MemoryIdentifier(i, idOld.DataType)
					: new Identifier(FormatSsaName(idOld.Name, i), i, idOld.DataType, idOld.Storage);
			}
			else
			{
				idNew = idOld;
			}
			SsaIdentifier id = new SsaIdentifier(idNew, idOld, stmDef);
			List.Add(id);
			return id;
		}

		public SsaIdentifier this[int i]
		{
			get { return (SsaIdentifier) List[i]; }
			set { List[i] = value; }
		}

		
		public SsaIdentifier this[Identifier id]
		{
			get { return (SsaIdentifier) List[id.Number]; }
			set { List[id.Number] = value; }
		}

		public string FormatSsaName(string prefix, int v)
		{
			return string.Format("{0}_{1}", prefix, v);
		}
	}
}
