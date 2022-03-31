using Reko.Core;
using Reko.Core.Code;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class Cfg
    {
        public ConcurrentDictionary<Address, Block> Blocks;
        public ConcurrentDictionary<Address, List<Edge>> Edges;
        public ConcurrentDictionary<Address, Proc> Procedures;
        public ConcurrentDictionary<Address, ExternalProcedure> Stubs;
        public ConcurrentDictionary<Address, ExternalProcedure> NoDecompiles;
    }

    public class Proc
    {
        public Address Address { get; set; }
        public ProvenanceType Provenance { get; set; }
        public IProcessorArchitecture Architecture { get; internal set; }
    }

    public class Block
    {
        public IProcessorArchitecture Architecture;
        public string Id;
        public Address Address;
        public List<(Address, Instruction)> Instructions;
    }

    public enum EdgeType
    {
        DirectJump,
        DirectCall,
        IndirectJump,
        IndirectCall,
        Return,
    }

    public class Edge
    {
        public Address From;
        public Address To;
        public EdgeType Type;
    }
}
