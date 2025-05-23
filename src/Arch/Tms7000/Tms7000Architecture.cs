using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tms7000
{
    public class Tms7000Architecture : ProcessorArchitecture
    {
        public Tms7000Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.Endianness = EndianServices.Big;
            this.InstructionBitSize = 8;
            this.MemoryGranularity = 8;
            this.GpRegs = Enumerable.Range(0, 256)
                .Select(n => RegisterStorage.Reg8($"r{n}", n))
                .ToArray();
            this.Ports = Enumerable.Range(0, 256)
                .Select(n => RegisterStorage.Reg8($"p{n}", n))
                .ToArray();
            this.a = RegisterStorage.Reg8("a", 0);
            this.b = RegisterStorage.Reg8("b", 1);
            this.st = RegisterStorage.Reg8("st", 0x100);
            this.sp = RegisterStorage.Reg8("sp", 0x101);
            this.GpRegs[0] = a;
            this.GpRegs[1] = b;
            this.StackRegister = sp;
            this.FramePointerType = (PrimitiveType) sp.DataType;
            this.PointerType = PrimitiveType.Ptr16;
        }

        public RegisterStorage a;
        public RegisterStorage b;
        public RegisterStorage st;
        public RegisterStorage sp;
        public RegisterStorage[] GpRegs;
        public RegisterStorage[] Ports;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Tms7000Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Tms7000State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Tms7000Rewriter(this, rdr, (Tms7000State) state, binder, host);
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            //$TODO: TMS7000 has an 8-bit stack pointer but a 16-bit address space.
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage st, uint grf)
        {
            var fl = new FlagGroupStorage(this.st, grf, GrfToString(this.st, "", grf));
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage st, string str, uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.IF) != 0) s.Append('I');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt16();
            return Address.Ptr16(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }
    }

    [Flags]
    public enum FlagM
    {
        CF = 0x08,
        NF = 0x04,
        ZF = 0x02,
        IF = 0x01,

        CNZ = CF|NF|ZF,
        NZ = NF|ZF,
    }
}
