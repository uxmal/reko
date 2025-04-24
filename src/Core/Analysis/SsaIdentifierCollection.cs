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

using Reko.Core.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// A collection of <see cref="SsaIdentifier"/>s. New 
    /// identifiers are added using the <see cref="Add(Identifier, Statement?, bool)"/>
    /// method.
    /// </summary>
	public class SsaIdentifierCollection : ICollection<SsaIdentifier>
	{
        private readonly Dictionary<Identifier, SsaIdentifier> sids = new();
        private int serialNumber = 0;

        /// <summary>
        /// Creates a new <see cref="SsaIdentifier"/> and adds it to the collection.
        /// Fresh identifiers are created by appending a unique serial number to
        /// the original identifier name.
        /// </summary>
        /// <param name="idOld">The original identifier.</param>
        /// <param name="stmDef"><see cref="Statement"/> where the identifier is 
        /// defined.</param>
        /// <param name="isSideEffect">True if this identifier is the result of
        /// a side effect.</param>
        /// <returns>A freshly created <see cref="SsaIdentifier"/></returns>
		public SsaIdentifier Add(Identifier idOld, Statement? stmDef, bool isSideEffect)
		{
			int i = ++serialNumber;
			Identifier idNew;
			if (stmDef is not null)
			{
                string name = idOld.Storage is MemoryStorage
					? ReplaceNumericSuffix(idOld.Name, i)
					: FormatSsaName(idOld, i);
				idNew = new Identifier(name, idOld.DataType, idOld.Storage);
			}
			else
			{
				idNew = idOld;
			}
			var sid = new SsaIdentifier(idNew, idOld, stmDef, isSideEffect);
			sids.Add(idNew, sid);
			return sid;
		}

        /// <summary>
        /// Find the <see cref="SsaIdentifier"/> corresponding to the given identifier.
        /// </summary>
        /// <param name="id">Identifier to look up.</param>
        /// <returns>The corresponding <see cref="SsaIdentifier"/>.</returns>
		public SsaIdentifier this[Identifier id]
		{
			get { return sids[id]; }
		}

        /// <summary>
        /// Associates a given <see cref="Identifier"/> with an <see cref="SsaIdentifier"/>
        /// provided by the caller.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sid"></param>
        /// <remarks>
        /// You are recommended to use the other overload of the <see cref="Add(Identifier, Statement?, bool)"/> 
        /// method whenever possible.</remarks>
        public void Add(Identifier id, SsaIdentifier sid)
        {
            ++serialNumber;
            sids.Add(id, sid);
        }

        /// <summary>
        /// The number of identifiers in the collection.
        /// </summary>
        public int Count => sids.Count;

        /// <summary>
        /// Clears all the identifiers in the collection.
        /// </summary>
        public void Clear()
        {
            sids.Clear();
            serialNumber = 0;
        }

        /// <summary>
        /// Returns true if the given identifier is in the collection.
        /// </summary>
        /// <param name="id"><see cref="Identifier"/> to look for.</param>
        /// <returns>True if the identifier is present, otherwise false.</returns>
        public bool Contains(Identifier id)
        {
            return sids.ContainsKey(id);
        }

        void ICollection<SsaIdentifier>.CopyTo(SsaIdentifier[] array, int arrayIndex)
        {
            sids.Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<SsaIdentifier> GetEnumerator()
        {
            return sids.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Removes the given <see cref="SsaIdentifier"/> from the collection.
        /// </summary>
        /// <param name="sid"><see cref="SsaIdentifier"/> to remove.</param>
        /// <returns>True if the identifier existed and was removed; otherwise false.</returns>
        public bool Remove(SsaIdentifier sid)
        {
            return sids.Remove(sid.Identifier);
        }

        /// <summary>
        /// Given an identifier, returns the corresponding <see cref="SsaIdentifier"/>.
        /// </summary>
        /// <param name="id">The <see cref="Identifier"/> being used as a key.</param>
        /// <param name="sid">The corresponding <see cref="SsaIdentifier"/> if present.</param>
        /// <returns>True if a corresponding <see cref="SsaIdentifier "/> was found,
        /// false if not.
        /// </returns>
        public bool TryGetValue(Identifier id, [MaybeNullWhen(false)] out SsaIdentifier sid)
        {
            return sids.TryGetValue(id, out sid);
        }

		private static string FormatSsaName(Identifier id, int v)
		{
            return $"{id.Name}_{v}";
		}

        private static string ReplaceNumericSuffix(string str, int newSuffix)
        {
            for (int i = str.Length-1; i >= 0; --i)
            {
                if (!char.IsDigit(str[i]))
                {
                    if (i >= str.Length - 1)
                        return str + newSuffix;
                    else
                        return str.Remove(i + 1) + newSuffix;
                }
            }
            return "" + newSuffix;
        }

        // The following methods are part of ICollection. Either no code in the
        // decompiler needs needs them, or they are unsafe.

        bool ICollection<SsaIdentifier>.IsReadOnly => false;

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
