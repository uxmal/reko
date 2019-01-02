#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// For a specific procedure, maintains the mapping between
    /// aliased registers or memory locations.
    /// </summary>
    public class AliasState
    {
        private Dictionary<Storage, Entry> entries;

        public AliasState()
        {
            entries = new Dictionary<Storage, Entry>(
                new StorageComparer());
        }

        public class Entry
        {
            public Entry Rep;
            public Storage Storage;
            internal int Rank;
        }

        public void Add(Storage stg)
        {
            if (!entries.ContainsKey(stg))
            {
                var e = MakeSet(stg);
                entries.Add(stg, e);
                foreach (var de in entries)
                {
                    if (IsAlias(stg, de.Key))
                    {
                        Union(stg, de.Key);
                    }
                }
            }
        }

        private bool IsAlias(Storage stA, Storage stB)
        {
            if (stA.Domain != stB.Domain)
                return false;
            var aStart = stA.BitAddress;
            var aEnd = aStart + stA.BitSize;
            var bStart = stB.BitAddress;
            var bEnd = bStart + stB.BitSize;
            return aStart < bEnd && bStart < aEnd;
        }

        public IEnumerable<Storage> GetAliases(Storage stg)
        {
            var e = Find(stg);
            foreach (var k in entries.Keys)
            {
                if (Find(k) == e)
                    yield return k;
            }
        }

        public Entry MakeSet(Storage stg)
        {
            var e = new Entry { Storage = stg, Rank = 1 };
            e.Rep = e;
            return e;
        }

        public Entry Union(Storage stgA, Storage stgB)
        {
            var eA = Find(stgA);
            var eB = Find(stgB);
            if (eA == eB)
                return eA;
            if (eA.Rank < eB.Rank)
            {
                eA.Rep = eB;
                return eB;
            }
            else if (eA.Rank > eB.Rank)
            {
                eB.Rep = eA;
                return eA;
            }
            else
            {
                eB.Rep = eA;
                eA.Rank += 1;
            }
            return eA;
        }

        public Entry Find(Storage s)
        {
            Entry x;
            if (!entries.TryGetValue(s, out x))
                return null;
            return Find(x);
        }

        public Entry Find(Entry x)
        { 
            if (x.Rep != x)
                x.Rep = Find(x.Rep.Storage);
            return x.Rep;
        }

        public class StorageComparer : IEqualityComparer<Storage>
        {
            public bool Equals(Storage x, Storage y)
            {
                if (x.Domain != y.Domain)
                    return false;
                if (x.BitSize != y.BitSize)
                    return false;
                return x.BitAddress == y.BitAddress;
            }

            public int GetHashCode(Storage obj)
            {
                return
                   ((int)obj.Domain
                    | ((int)obj.BitAddress << 8)
                    | ((int)obj.BitSize << 16)).GetHashCode();
            }
        }
    }
}
