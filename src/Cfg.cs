using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Reko.ScannerV2
{
    /// <summary>
    /// The Control Flow Graph of a program.
    /// </summary>
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

        /// <summary>
        /// Maps addresses to the <see cref="Block"/>s at those addresses.
        /// </summary>
        public ConcurrentDictionary<Address, Block> Blocks { get; }
        /// <summary>
        /// Maps start ("from") addresses to <see cref="Edge"/>s leaving those
        /// addresses.
        /// </summary>
        public ConcurrentDictionary<Address, List<Edge>> Edges { get; }
        /// <summary>
        /// Maps the entry point address to <see cref="Proc"/>s.
        /// </summary>
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
        int Length,
        List<(Address, RtlInstruction)> Instructions);

    public enum EdgeType
    {
        Jump,
        Fallthrough,
        Call,
        Return,
    }

    /// <summary>
    /// Represents an edge in the <see cref="Cfg"/>.
    /// </summary>
    /// <param name="From">The address from which the edge goes.</param>
    /// <param name="To">The address to which the edge goes.</param>
    /// <param name="Type">The type of edge.</param>
    public record Edge(
        Address From,
        Address To,
        EdgeType Type);
}
