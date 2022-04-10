using Reko.Core;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ScanResultsGraph : DirectedGraph<Address>
    {
        private ScanResultsV2 cfg;

        public ScanResultsGraph(ScanResultsV2 cfg)
        {
            this.cfg = cfg;
        }

        public ICollection<Address> Nodes => cfg.Blocks.Keys;

        public void AddEdge(Address nodeFrom, Address nodeTo)
        {
            List<Edge>? succs;
            while (!cfg.Successors.TryGetValue(nodeFrom, out succs))
            {
                succs = new List<Edge>();
                if (cfg.Successors.TryAdd(nodeFrom, succs))
                    break;
            }
            var e = new Edge(nodeFrom, nodeTo, EdgeType.Jump);
            lock (succs)
            {
                succs.Add(e);
            }

            List<Edge>? preds;
            while (!cfg.Predecessors.TryGetValue(nodeTo, out preds))
            {
                preds = new List<Edge>();
                if (cfg.Predecessors.TryAdd(nodeTo, preds))
                    break;
            }
            lock (preds)
            {
                preds.Add(e);
            }
        }

        public bool ContainsEdge(Address nodeFrom, Address nodeTo)
        {
            if (cfg.Successors.TryGetValue(nodeFrom, out var succs))
            {
                foreach (var e in succs)
                {
                    if (e.To == nodeTo)
                        return true;
                }
            }
            return false;
        }

        public ICollection<Address> Predecessors(Address node)
        {
            if (cfg.Predecessors.TryGetValue(node, out var preds))
                return preds.Select(s => s.From).ToArray();
            else
                return Array.Empty<Address>();
        }

        // Not thread-safe
        public void RemoveEdge(Address nodeFrom, Address nodeTo)
        {
            if (cfg.Successors.TryGetValue(nodeFrom, out var succs))
            {
                var i = succs.FindIndex(e => e.To == nodeTo);
                if (i != -1)
                {
                    succs.RemoveAt(i);
                }
            }
            if (cfg.Predecessors.TryGetValue(nodeTo, out var preds))
            {
                var i = preds.FindIndex(e => e.From == nodeFrom);
                if (i != -1)
                {
                    preds.RemoveAt(i);
                }
            }
        }

        public ICollection<Address> Successors(Address node)
        {
            if (cfg.Successors.TryGetValue(node, out var succs))
                return succs.Select(s => s.To).ToArray();
            else
                return Array.Empty<Address>();
        }
    }
}
