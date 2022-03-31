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
        public Cfg()
        {
            this.Blocks = new();
            this.Edges = new();
            this.Procedures = new();
            this.Stubs = new();
            this.NoDecompiles = new();
        }

        public ConcurrentDictionary<Address, Block> Blocks { get; }
        public ConcurrentDictionary<Address, List<Edge>> Edges { get; }
        public ConcurrentDictionary<Address, Proc> Procedures { get; }
        public ConcurrentDictionary<Address, ExternalProcedure> Stubs { get; }
        public ConcurrentDictionary<Address, ExternalProcedure> NoDecompiles { get; }
    }

    public record Proc(
        Address Address,
        ProvenanceType Provenance,
        IProcessorArchitecture Architecture,
        string Name);

    public record Block(
        IProcessorArchitecture Architecture,
        string Id,
        Address Address,
        List<(Address, Instruction)> Instructions);

    public enum EdgeType
    {
        DirectJump,
        DirectCall,
        IndirectJump,
        IndirectCall,
        Return,
    }

    public record Edge(
        Address From,
        Address To,
        EdgeType Type);
}
