using Moq;
using NUnit.Framework;
using Reko.Core.Intrinsics;
using Reko.Core.Types;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.UnitTests.Mocks;
using Reko.Analysis;
using Reko.Core.Memory;
using Reko.Evaluation;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Decompiler.Evaluation
{
    [TestFixture]
    public class ExpressionSimplifier_ConvertConvertTests
    {
        private static readonly PrimitiveType bool1 = PrimitiveType.Bool;
        private static readonly PrimitiveType int8 = PrimitiveType.Int8;
        private static readonly PrimitiveType int16 = PrimitiveType.Int16;
        private static readonly PrimitiveType int32 = PrimitiveType.Int32;
        private static readonly PrimitiveType int64 = PrimitiveType.Int64;
        private static readonly PrimitiveType uint8 = PrimitiveType.UInt8;
        private static readonly PrimitiveType uint16 = PrimitiveType.UInt16;
        private static readonly PrimitiveType uint32 = PrimitiveType.UInt32;
        private static readonly PrimitiveType uint64 = PrimitiveType.UInt64;
        private static readonly PrimitiveType byte8 = PrimitiveType.Byte;
        private static readonly PrimitiveType word16 = PrimitiveType.Word16;
        private static readonly PrimitiveType word32 = PrimitiveType.Word32;
        private static readonly PrimitiveType real32 = PrimitiveType.Real32;
        private static readonly PrimitiveType real64 = PrimitiveType.Real64;
        private static readonly PrimitiveType real80 = PrimitiveType.Real80;
        private static readonly PrimitiveType real96 = PrimitiveType.Real96;

        private ProcedureBuilder m;
        private Mock<IProcessorArchitecture> arch;
        private ExpressionSimplifier simplifier;
        private Expression e_bool;
        private Expression e_byte;
        private Expression e_real32;
        private Expression e_real64;
        private Expression e_real96;
        private Expression e_int32;
        private Expression e_word16;
        private Expression e_word32;


        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.MemoryGranularity).Returns(8);
            var dynamicLinker = new Mock<IDynamicLinker>();
            var mem = new Mock<IMemory>();
            var listener = new FakeDecompilerEventListener();
            var ssaIds = new SsaIdentifierCollection();
            e_bool = MakeId(ssaIds, bool1);
            e_byte = MakeId(ssaIds, byte8);
            e_real32 = MakeId(ssaIds, real32);
            e_real64 = MakeId(ssaIds, real64);
            e_real96 = MakeId(ssaIds, real96);
            e_int32 = MakeId(ssaIds, int32);
            e_word16 = MakeId(ssaIds, word16);
            e_word32 = MakeId(ssaIds, word32);

            var ssaCtx = new SsaEvaluationContext(arch.Object, ssaIds, dynamicLinker.Object);
            simplifier = new ExpressionSimplifier(mem.Object, ssaCtx, listener);
        }

        private Expression MakeId(SsaIdentifierCollection ssaIds, PrimitiveType dt)
        {
            var id = Identifier.CreateTemporary($"e_{dt.Name}", dt);
            var sid = ssaIds.Add(id, null, false);
            return sid.Identifier;
        }

        private void RunTest(string sExpected, Expression e)
        {
            var result = e.Accept(simplifier);
            var (actual, changed) = result;
            Assert.AreEqual(sExpected, actual.ToString());
        }

        [Test]
        public void ConvConv_bool1_int32_word32_uint64()
        {
            RunTest("CONVERT(e_bool, bool, uint64)", m.Convert(m.Convert(e_bool, bool1, int32), word32, uint64));
        }

        [Test]
        public void ConvConv_bool1_int8_uint8_word32()
        {
            RunTest("CONVERT(e_bool, bool, word32)", m.Convert(m.Convert(e_bool, bool1, int8), uint8, word32));
        }

        [Test]
        public void ConvConv_bool1_word16_word16_uint32()
        {
            RunTest("CONVERT(e_bool, bool, uint32)", m.Convert(m.Convert(e_bool, bool1, word16), word16, uint32));
        }

        [Test]
        public void ConvConv_bool1_word32_word32_uint64()
        {
            RunTest("CONVERT(e_bool, bool, uint64)", m.Convert(m.Convert(e_bool, bool1, word32), word32, uint64));
        }

        [Test]
        public void ConvConv_byte8_int32_uint32_uint64()
        {
            RunTest("CONVERT(e_byte, byte, uint64)", m.Convert(m.Convert(e_byte, byte8, int32), uint32, uint64));
        }

        [Test]
        public void ConvConv_byte8_word16_word16_int32()
        {
            RunTest("CONVERT(e_byte, byte, int32)", m.Convert(m.Convert(e_byte, byte8, word16), word16, int32));
        }

        [Test]
        public void ConvConv_byte8_word16_word16_word32()
        {
            RunTest("CONVERT(e_byte, byte, word32)", m.Convert(m.Convert(e_byte, byte8, word16), word16, word32));
        }

        [Test]
        public void ConvConv_byte8_word32_word32_uint64()
        {
            RunTest("CONVERT(e_byte, byte, uint64)", m.Convert(m.Convert(e_byte, byte8, word32), word32, uint64));
        }

        [Test]
        public void ConvConv_int8_int16_int16_int32()
        {
            RunTest("CONVERT(e_byte, int8, int32)", m.Convert(m.Convert(e_byte, int8, int16), int16, int32));
        }

        [Test]
        public void ConvConv_uint8_int16_int16_int32()
        {
            RunTest("CONVERT(e_byte, uint8, int32)", m.Convert(m.Convert(e_byte, uint8, int16), int16, int32));
        }

        [Test]
        public void ConvConv_uint8_uint16_int16_int32()
        {
            RunTest("CONVERT(e_byte, uint8, int32)", m.Convert(m.Convert(e_byte, uint8, uint16), int16, int32));
        }

        [Test]
        public void ConvConv_uint8_uint32_uint32_uint64()
        {
            RunTest("CONVERT(e_byte, uint8, uint64)", m.Convert(m.Convert(e_byte, uint8, uint32), uint32, uint64));
        }

        [Test]
        public void ConvConv_real64_real32_real32_real64()
        {
            RunTest("CONVERT(e_real64, real64, real32)", m.Convert(m.Convert(m.Convert(e_real64, real64, real32), real32, real64), real64, real32));
        }

        [Test]
        public void ConvConv_real32_real64_real64_real32()
        {
            RunTest("e_real32", m.Convert(m.Convert(e_real32, real32, real64), real64, real32));
        }

        [Test]
        public void ConvConv_int32_real32_real32_real64()
        {
            RunTest("CONVERT(e_int32, int32, real32)", m.Convert(m.Convert(e_int32, int32, real32), real32, real32));
        }

        [Test]
        public void ConvConv_real64_real80_real80_real64()
        {
            RunTest("e_real64", m.Convert(m.Convert(e_real64, real64, real80), real80, real64));
        }

        [Test]
        public void ConvConv_real96_real80_real80_real96()
        {
            RunTest("CONVERT(CONVERT(e_real96, real96, real80), real80, real96)", m.Convert(m.Convert(e_real96, real96, real80), real80, real96));
        }

        [Test]
        public void ConvConv_word16_word32_word32_uint64()
        {
            RunTest("CONVERT(e_word16, word16, uint64)", m.Convert(m.Convert(e_word16, word16, word32), word32, uint64));
        }

        [Test]
        public void ConvConv_int32_real96_real96_real80()
        {
            RunTest("CONVERT(e_word32, int32, real80)", m.Convert(m.Convert(e_word32, int32, real96), real96, real80));
        }
    }
}

