#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.Analysis
{
	public class SsaIdentifierCollection : IEnumerable<SsaIdentifier>
	{
        private List<SsaIdentifier> sids = new List<SsaIdentifier>();
		public SsaIdentifier Add(Identifier idOld, Statement stmDef, Expression exprDef, bool isSideEffect)
		{
			int i = sids.Count;
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
			var sid = new SsaIdentifier(idNew, idOld, stmDef, exprDef, isSideEffect);
			sids.Add(sid);
			return sid;
		}

		public SsaIdentifier this[Identifier id]
		{
			get { return sids[id.Number]; }
			set { sids[id.Number] = value; }
		}

        [Obsolete("used only in unit test")]
        public void Add(SsaIdentifier sid)
        {
            sids.Add(sid);
        }

        [Obsolete("Find all uses and make sure they are not depending on numbers")]
        public int Count
        {
            get { return sids.Count; }
        }

        public IEnumerator<SsaIdentifier> GetEnumerator()
        {
            return sids.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //$TODO: this is much better implemented as a hash table.
        public bool TryGetValue(Identifier id, out SsaIdentifier sid)
        {
            if (id.Number >= sids.Count)
            {
                sid = null;
                return false;
            }
            else
            {
                sid = sids[id.Number];
                return true;
            }
        }

		public string FormatSsaName(string prefix, int v)
		{
			return string.Format("{0}_{1}", prefix, v);
		}


    }
}
