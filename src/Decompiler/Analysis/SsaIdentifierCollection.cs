#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Diagnostics.CodeAnalysis;

namespace Reko.Analysis
{
	public class SsaIdentifierCollection : ICollection<SsaIdentifier>
	{
        private readonly Dictionary<Identifier, SsaIdentifier> sids = new();
        private int serialNumber = 0;

		public SsaIdentifier Add(Identifier idOld, Statement? stmDef, Expression? exprDef, bool isSideEffect)
		{
			int i = ++serialNumber;
			Identifier idNew;
			if (stmDef != null)
			{
				idNew = idOld is MemoryIdentifier
					? new MemoryIdentifier(ReplaceNumericSuffix(idOld.Name, i), idOld.DataType, idOld.Storage)
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
            ++serialNumber;
            sids.Add(id, sid);
        }

        public int Count => sids.Count;

        public bool IsReadOnly => false;

        public void Clear()
        {
            sids.Clear();
            serialNumber = 0;
        }

        public bool Contains(Identifier id)
        {
            return sids.ContainsKey(id);
        }

        void ICollection<SsaIdentifier>.CopyTo(SsaIdentifier[] array, int arrayIndex)
        {
            sids.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SsaIdentifier> GetEnumerator()
        {
            return sids.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(SsaIdentifier sid)
        {
            return sids.Remove(sid.Identifier);
        }

        public bool TryGetValue(Identifier id, [MaybeNullWhen(false)] out SsaIdentifier sid)
        {
            return sids.TryGetValue(id, out sid);
        }

		private static string FormatSsaName(Identifier id, int v)
		{
            return string.Format("{0}_{1}", id.Name, v);
		}

        private static string ReplaceNumericSuffix(string str, int newSuffix)
        {
            for (int i = str.Length-1; i >= 0; --i)
            {
                if (!Char.IsDigit(str[i]))
                {
                    if (i >= str.Length - 1)
                        return str + newSuffix;
                    else
                        return str.Remove(i + 1) + newSuffix;
                }
            }
            return "" + newSuffix;
        }

        private static Storage StorageOf(Expression e)
        {
            if (e is Identifier id)
                return id.Storage;
            var bin = (BinaryExpression) e;
            int offset = ((Constant)bin.Right).ToInt32();
            return new StackStorage(offset, e.DataType);
        }

        // The following methods are part of ICollection. Either no code in the
        // decompiler needs needs them, or they are unsafe.
        void ICollection<SsaIdentifier>.Add(SsaIdentifier item)
        {
            throw new NotSupportedException("Use other overloads of Add instead of this method.");
        }

        bool ICollection<SsaIdentifier>.Contains(SsaIdentifier item)
        {
            throw new NotImplementedException();
        }
    }
}
