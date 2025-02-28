namespace Reko.UnitTests.Decompiler.Scanning;

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Text;


public class FragmentFinderTests
{
    private readonly FakeArchitecture arch = new FakeArchitecture(
        new ServiceContainer(),
        "fake",
        [],
        [],
        []);

    [SetUp]
    public void Setup()
    {
    }

    private RtlBlock B(uint uAddr, int length)
    {
        var addrFt = Address.Ptr32(uAddr + (uint) length);
        var b = RtlBlock.Create(
            arch,
            Address.Ptr32(uAddr),
            $"l{uAddr:X8}",
            length,
            addrFt,
            ProvenanceType.Scanning,
            []);
        return b;
    }

    private ImageSegment S(uint uAddr, uint length)
    {
        Address addr = Address.Ptr32(uAddr);
        var mem = new ByteMemoryArea(addr, new byte[length]);
        var s = new ImageSegment($"s{uAddr:X}", mem, AccessMode.ReadExecute);
        return s;
    }

    private void TestResolveOverlaps(string sExpected, RtlBlock[] blocks)
    {
        var frf = new ChunkEnumerator();

        AssertResult(sExpected, frf.ResolveOverlaps(blocks));
    }

    private void TestGenerateGapFragments(string sExpected, ImageSegment[] segments, RtlBlock[] blocks)
    {
        var frf = new ChunkEnumerator();

        AssertResult(sExpected, frf.EnumerateFragments(segments, blocks));
    }

    private void AssertResult(string sExpected, IEnumerable<Chunk> actual)
    {
        var sb = new StringBuilder("[");
        var sep = "";
        foreach (var f in actual)
        {
            sb.Append(sep);
            sep = ", ";
            char type = f.Architecture is null ? 'G' : 'B';
            sb.Append($"{type}(0x{f.Address.Offset:X}, 0x{f.Length:X})");
        }
        sb.Append(']');
        var sActual = sb.ToString();
        if (sExpected != sActual)
        {
            Console.WriteLine(sActual);
            Assert.That(sActual, Is.EqualTo(sExpected));
        }
    }

    [Test]
    public void Frf_ResolveOverlaps_NoOverlaps()
    {
        TestResolveOverlaps("[B(0x1000, 0x100), B(0x1200, 0x100)]", [B(0x1000, 0x100), B(0x1200, 0x100)]);
    }

    [Test]
    public void Frf_ResolveOverlaps_LongBlockFirst()
    {
        TestResolveOverlaps("[B(0x1000, 0x1000)]", [B(0x1000, 0x1000), B(0x1200, 0x100)]);
    }

    [Test]
    public void Frf_ResolveOverlaps_AdjacentBlocks()
    {
        TestResolveOverlaps("[B(0x1000, 0x1000), B(0x2000, 0x100)]", [B(0x1000, 0x1000), B(0x2000, 0x100)]);
    }

    [Test]
    public void Frf_GenerateGapFragments_NoBlocks()
    {
        TestGenerateGapFragments("[G(0x1000, 0x1000)]", [S(0x1000, 0x1000)], []);
    }

    [Test]
    public void Frf_GenerateGapFragments_BlockAtStartOfSegment()
    {
        TestGenerateGapFragments("[B(0x1000, 0x100), G(0x1100, 0xF00)]", [S(0x1000, 0x1000)], [B(0x1000, 0x100)]);
    }

    [Test]
    public void Frf_GenerateGapFragments_BlockInMiddleOfSegment()
    {
        TestGenerateGapFragments("[G(0x1000, 0x100), B(0x1100, 0x100), G(0x1200, 0xE00)]", [S(0x1000, 0x1000)], [B(0x1100, 0x100)]);
    }

    [Test]
    public void Frf_GenerateGapFragments_BlockAtEndOfSegment()
    {
        TestGenerateGapFragments("[G(0x1000, 0x100), B(0x1100, 0xF00)]", [S(0x1000, 0x1000)], [B(0x1100, 0xF00)]);
    }


    public class FakeArchitecture : Reko.Core.ProcessorArchitecture
    {
        public FakeArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options, Dictionary<string, RegisterStorage>? regsByName, Dictionary<StorageDomain, RegisterStorage>? regsByDomain) :
         base(services, archId, options, regsByName, regsByDomain)
        {
            this.CodeMemoryGranularity = 8;
            this.InstructionBitSize = 32;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
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

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            throw new NotImplementedException();
        }
    }
}


class ChunkEnumeratorTests
{
}
