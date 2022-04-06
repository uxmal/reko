using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ScannerV2
{
    public class CfgBackWalkHost : IBackWalkHost<RtlBlock, RtlInstruction>
    {
        private readonly Cfg cfg;
        private readonly Dictionary<Address,List<Address>> backEdges;

        public CfgBackWalkHost(
            Core.Program program,
            IProcessorArchitecture arch,
            Cfg cfg, 
            Dictionary<Address, List<Address>> backEdges)
        {
            this.Program = program;
            this.SegmentMap = program.SegmentMap;
            this.Architecture = arch;
            this.cfg = cfg;
            this.backEdges = backEdges;
        }

        public IProcessorArchitecture Architecture { get; }

        public Core.Program Program { get; }

        public SegmentMap SegmentMap { get; }

        public (Expression?, Expression?) AsAssignment(RtlInstruction instr)
        {
            if (instr is RtlAssignment ass)
                return (ass.Dst, ass.Src);
            return (null, null);
        }

        public Expression? AsBranch(RtlInstruction instr)
        {
            if (instr is RtlBranch bra)
                return bra.Condition;
            return null;
        }

        public int BlockInstructionCount(RtlBlock rtlBlock)
        {
            return rtlBlock.Instructions.Sum(i => i.Instructions.Length);
        }

        public IEnumerable<RtlInstruction?> GetBlockInstructions(RtlBlock block)
        {
            return block.Instructions.SelectMany(c => c.Instructions);
        }

        public List<RtlBlock> GetPredecessors(RtlBlock block)
        {
            if (!backEdges.TryGetValue(block.Address, out var preds))
                return new List<RtlBlock>();
            var pp = preds.Select(p => cfg.Blocks[p]).ToList();
            return pp;
        }

        public RtlBlock? GetSinglePredecessor(RtlBlock block)
        {
            throw new System.NotImplementedException();
        }

        public RegisterStorage GetSubregister(RegisterStorage rIdx, BitRange range)
        {
            throw new System.NotImplementedException();
        }

        public bool IsFallthrough(RtlInstruction instr, RtlBlock block)
        {
            throw new System.NotImplementedException();
        }

        public bool IsStackRegister(Storage storage)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidAddress(Address addr)
        {
            throw new System.NotImplementedException();
        }

        public Address? MakeAddressFromConstant(Constant c)
        {
            throw new System.NotImplementedException();
        }

        public Address MakeSegmentedAddress(Constant selector, Constant offset)
        {
            throw new System.NotImplementedException();
        }
    }
}