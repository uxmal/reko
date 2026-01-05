#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers;
using Reko.Core;
using Reko.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks.Core.Collections
{
    [Config(typeof(Config))]
    public class ConcurrentBTreeDictionaryBenchmarks
    {
        private const int N = 10000;

        private readonly BTreeDictionary<Address, object> bTree;
        private readonly ConcurrentBTreeDictionary<Address, object> cbTree;
        private readonly SortedList<Address, object> slist;
        private readonly Address addr;

        public ConcurrentBTreeDictionaryBenchmarks()
        {
            bTree = new BTreeDictionary<Address, object>();
            cbTree = new ConcurrentBTreeDictionary<Address, object>();
            slist = new SortedList<Address, object>();
            Populate(bTree);
            Populate(cbTree);
            Populate(slist);
            addr = Address.Ptr32(N * 5 - 1);
        }


        private void Populate(IDictionary<Address, object> dict)
        {
            for (uint i = 0; i < N; ++i)
            {
                dict.Add(Address.Ptr32(i * 0x10), new object());
            }
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                AddDiagnoser(MemoryDiagnoser.Default);
            }
        }



        [Benchmark]
        public bool BTree_TryGetLowerBoundIndex()
        {
            return bTree.TryGetLowerBoundIndex(addr, out int index);
        }

        [Benchmark]
        public bool CBTree_TryGetLowerBoundIndex()
        {
            return cbTree.TryGetLowerBoundIndex(addr, out int index);
        }

        [Benchmark]
        public bool SortedList_TryGetLowerBoundIndex()
        {
            return slist.TryGetLowerBoundIndex(addr, out int index);
        }
    }
}
