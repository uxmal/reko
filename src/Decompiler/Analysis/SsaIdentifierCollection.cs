#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Analysis
{
	public class SsaIdentifierCollection : IEnumerable<SsaIdentifier>
	{
        private Dictionary<Identifier, SsaIdentifier> sids =
            new Dictionary<Identifier, SsaIdentifier>();

		public SsaIdentifier Add(Identifier idOld, Statement stmDef, Expression exprDef, bool isSideEffect)
		{
			int i = sids.Count;
			Identifier idNew;
			if (stmDef != null)
			{
				idNew = idOld is MemoryIdentifier
					? new MemoryIdentifier(i, idOld.DataType)
					: new Identifier(FormatSsaName(idOld, i), idOld.DataType, StorageOf(idOld));
			}
			else
			{
				idNew = idOld;
			}
			var sid = new SsaIdentifier(idNew, idOld, stmDef, exprDef, isSideEffect);
			sids.Add(idNew, sid);
			return sid;
		}

		public SsaIdentifier this[Identifier id]
		{
			get { return sids[id]; }
			set { sids[id] = value; }
		}

        public void Add(Identifier id, SsaIdentifier sid)
        {
            sids.Add(id, sid);
        }

        public int Count
        {
            get { return sids.Count; }
        }

        public IEnumerator<SsaIdentifier> GetEnumerator()
        {
            return sids.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(SsaIdentifier sid)
        {
            sids.Remove(sid.Identifier);
        }

        public bool TryGetValue(Identifier id, out SsaIdentifier sid)
        {
            return sids.TryGetValue(id, out sid);
        }

		public string FormatSsaName(Expression e, int v)
		{
            string prefix = null;
            var id = e as Identifier;
            if (id != null)
            {
                prefix = id.Name;
            }
            var bin = e as BinaryExpression;
            if (bin != null)
            {
                int offset = ((Constant)bin.Right).ToInt32();
                if (offset < 0)
                    prefix = "loc";
                else
                    prefix = "arg";

            }
            if (prefix == null)
                throw new NotImplementedException(e.ToString());
            return string.Format("{0}_{1}", prefix, v);
		}

        private Storage StorageOf(Expression e)
        {
            var id = e as Identifier;
            if (id != null)
                return id.Storage;
            var bin = (BinaryExpression) e;
            int offset = ((Constant)bin.Right).ToInt32();
            if (offset < 0)
                return new StackLocalStorage(offset, e.DataType);
            else 
                return new StackArgumentStorage(offset, e.DataType);
        }
    }
}
