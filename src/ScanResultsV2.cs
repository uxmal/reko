using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RtlBlock = Reko.Scanning.RtlBlock;

namespace Reko.ScannerV2
{
    /// <summary>
    /// The informationg collected by scanning a binary.
    /// </summary>
    public class ScanResultsV2
    {
        public ScanResultsV2()
        {
            this.Blocks = new();
            this.Successors = new();
            this.Predecessors = new();
            this.Procedures = new();
            this.SpeculativeBlocks = new();
            this.SpeculativeProcedures = new();
            this.Stubs = new();
            this.NoDecompiles = new();
            this.ICFG = new ScanResultsGraph(this);
        }

        /// <summary>
        /// Maps addresses to the <see cref="RtlBlock"/>s at those addresses.
        /// </summary>
        public ConcurrentDictionary<Address, RtlBlock> Blocks { get; }

        /// <summary>
        /// Maps start ("from") addresses to <see cref="Edge"/>s leaving those
        /// addresses.
        /// </summary>
        public ConcurrentDictionary<Address, List<Edge>> Successors { get; }

        /// <summary>
        /// Maps end ("to") addresses to <see cref="Edge"/>s arriving at those
        /// addresses.
        /// </summary>
        public ConcurrentDictionary<Address, List<Edge>> Predecessors { get; }

        /// <summary>
        /// Maps the entry point address to <see cref="Proc"/>s.
        /// </summary>
        /// <remarks>
        /// These procedures were found either from the binary or during
        /// recursive scanning, and are considered "reliable".
        /// </remarks>
        public ConcurrentDictionary<Address, Proc> Procedures { get; }

        /// <summary>
        /// Addresses of blocks that were discovered during speculative scanning.
        /// </summary>
        /// <remarks>
        /// These blocks were found during speculative scaning, and aren't 
        /// considered "reliable" without more information.
        /// </remarks>
        public ConcurrentDictionary<Address, int> SpeculativeBlocks { get; }

        /// <summary>
        /// Addresses of possible procedures that were discovered during 
        /// speculative scanning. The dictionary maintains a count of how many
        /// times the address was called.
        /// </summary>
        /// <remarks>
        /// These procedures were found during speculative scanning, and aren't
        /// considered "reliable" without more information.
        /// </remarks>
        public ConcurrentDictionary<Address, int> SpeculativeProcedures { get; }

        /// <summary>
        /// Short sequences of instructions identified as dynamic link stubs
        /// (such as those in an ELF PLT)
        /// </summary>
        public ConcurrentDictionary<Address, ExternalProcedure> Stubs { get; }
        public ConcurrentDictionary<Address, ExternalProcedure> NoDecompiles { get; }
        public ScanResultsGraph ICFG { get; }
    }

    public enum BlockFlags
    {
        Valid = 1,      // Contains valid instructions.
        Invalid = 2,    // Contains invalid instructions.
        Privileged = 3, // Contains privileged instructions.
        Padding = 4,    // Consists only of padding instructions.
    }

    public record Proc(
        Address Address,
        ProvenanceType Provenance,
        IProcessorArchitecture Architecture,
        string Name);

    /// <summary>
    /// This class models a basic block consisting of <see cref="RtlInstruction"/>s.
    /// </summary>
    public class BlockQ
    { 
        public BlockQ(
            IProcessorArchitecture arch,
            Address addr,
            string id,
            int length,
            Address addrFallThrough,
            List<RtlInstructionCluster> instructions)
        {
            this.Architecture = arch;
            this.Name = id;
            this.Address = addr;
            this.Length = length;
            this.FallThrough = addrFallThrough;
            this.Instructions = instructions;
            this.IsValid = true;
        }

        public BlockQ(Address addr, string id) : this(
            default!,
            addr,
            id,
            0,
            default!,
            new List<RtlInstructionCluster>())
        {
        }

        /// <summary>
        /// Address at which the block starts.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// CPU architecture used to disassemble this block.
        /// </summary>
        public IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// The address after the block if control flow falls through.
        /// Note that this is not necessarily <see cref="Address"/> + <see cref="Length"/>, because
        /// control instructions with delay slots may require skipping one extra instruction.
        /// </summary>
        public Address FallThrough { get; }

        /// <summary>
        /// Invariant identifier used for this block.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// <param name="Length">The size of the basic block starting 
        /// at <see cref="Address"/> and including the length of the final
        /// instruction.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Indicates whether this block is valid or not.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The instructions this block consists of.</param>
        /// <summary>
        public List<RtlInstructionCluster> Instructions { get; }
    }

    public enum EdgeType
    {
        Jump,
        Fallthrough,
        Call,
        Return,
    }

    /// <summary>
    /// Represents an edge in the <see cref="ScanResultsV2"/>.
    /// </summary>
    /// <param name="From">The address from which the edge goes.</param>
    /// <param name="To">The address to which the edge goes.</param>
    /// <param name="Type">The type of edge.</param>
    public record Edge(
        Address From,
        Address To,
        EdgeType Type);
}
