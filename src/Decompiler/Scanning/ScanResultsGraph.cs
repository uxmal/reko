#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Directed graph wrapper around a <see cref="ScannerV2" /> object
    /// </summary>
    public class ScanResultsGraph : DirectedGraph<Address>
    {
        private readonly ScanResultsV2 cfg;

        public ScanResultsGraph(ScanResultsV2 cfg)
        {
            this.cfg = cfg;
        }

        public ICollection<Address> Nodes => cfg.Blocks.Keys;

        public void AddEdge(Address nodeFrom, Address nodeTo)
        {
            if (nodeTo.Offset == 0xA578)
                _ = this; //$DEBUG
            List<Address>? succs;
            while (!cfg.Successors.TryGetValue(nodeFrom, out succs))
            {
                succs = new List<Address>();
                if (cfg.Successors.TryAdd(nodeFrom, succs))
                    break;
            }
            lock (succs)
            {
                succs.Add(nodeTo);
            }

            List<Address>? preds;
            while (!cfg.Predecessors.TryGetValue(nodeTo, out preds))
            {
                preds = new List<Address>();
                if (cfg.Predecessors.TryAdd(nodeTo, preds))
                    break;
            }
            lock (preds)
            {
                preds.Add(nodeFrom);
            }
        }

        public bool ContainsEdge(Address nodeFrom, Address nodeTo)
        {
            if (cfg.Successors.TryGetValue(nodeFrom, out var succs))
            {
                foreach (var e in succs)
                {
                    if (e == nodeTo)
                        return true;
                }
            }
            return false;
        }

        public ICollection<Address> Predecessors(Address node)
        {
            if (cfg.Predecessors.TryGetValue(node, out var preds))
                return preds;
            else
                return Array.Empty<Address>();
        }

        // Not thread-safe
        public void RemoveEdge(Address nodeFrom, Address nodeTo)
        {
            if (nodeTo.Offset == 0xA578)
                _ = this; //$DEBUG

            if (cfg.Successors.TryGetValue(nodeFrom, out var succs))
            {
                var i = succs.FindIndex(e => e == nodeTo);
                if (i != -1)
                {
                    succs.RemoveAt(i);
                }
            }
            if (cfg.Predecessors.TryGetValue(nodeTo, out var preds))
            {
                var i = preds.FindIndex(e => e == nodeFrom);
                if (i != -1)
                {
                    preds.RemoveAt(i);
                }
            }
        }

        public ICollection<Address> Successors(Address node)
        {
            if (cfg.Successors.TryGetValue(node, out var succs))
                return succs;
            else
                return Array.Empty<Address>();
        }
    }
}
