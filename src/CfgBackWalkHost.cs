using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Scanning;
using System.Collections.Generic;

namespace Reko.ScannerV2
{
    public class CfgBackWalkHost : IBackWalkHost<RtlBlock, RtlInstruction>
    {
        private readonly Cfg cfg;

        public CfgBackWalkHost(Core.Program program, IProcessorArchitecture arch, Cfg cfg)
        {
            this.Program = program;
            this.SegmentMap = program.SegmentMap;
            this.Architecture = arch;
            this.cfg = cfg;
        }

        public IProcessorArchitecture Architecture { get; }

        public Core.Program Program { get; }

        public SegmentMap SegmentMap { get; }

        public (Expression?, Expression?) AsAssignment(RtlInstruction instr)
        {
            throw new System.NotImplementedException();
        }

        public Expression? AsBranch(RtlInstruction instr)
        {
            throw new System.NotImplementedException();
        }

        public int BlockInstructionCount(RtlBlock rtlBlock)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<RtlInstruction?> GetBlockInstructions(RtlBlock block)
        {
            throw new System.NotImplementedException();
        }

        public Address? GetBlockStartAddress(Address addr)
        {
            throw new System.NotImplementedException();
        }

        public List<RtlBlock> GetPredecessors(RtlBlock block)
        {
            throw new System.NotImplementedException();
        }

        public RtlBlock? GetSinglePredecessor(RtlBlock block)
        {
            throw new System.NotImplementedException();
        }

        public AddressRange? GetSinglePredecessorAddressRange(Address block)
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