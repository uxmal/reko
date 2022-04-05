using Moq;
using NUnit.Framework;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2.UnitTests
{
    [TestFixture]
    public class ShingleScannerTests : AbstractScannerTests
    {
        private Cfg cfg = default!;

        [SetUp]
        public void Setup()
        {
            base.Setup(16, 8);
            this.cfg = new Cfg();
        }

        private void RunTest(string sExpected)
        {
            var scanner = new ShingleScanner(program, cfg, new Mock<DecompilerEventListener>().Object);
            var cfgNew = scanner.ScanProgram();
            var sw = new StringWriter();
            DumpCfg(cfgNew, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        private void DumpCfg(Cfg cfg, StringWriter sw)
        {
            var g = new CfgGraph(cfg);
            var cfgs = cfg.Blocks.Values.OrderBy(b => b.Address);
            sw.WriteLine();
            foreach (var block in cfgs)
            {
                DumpBlock(block, g, sw);
            }
        }

        [Test]
        public void ShScanner_LinearBlock()
        {
            // No previous discoveries in the block, all instructions 4 bytes.
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, m.IAdd(r2, 3)),
                m => m.Assign(r2, m.Mem32(r1)),
                m => m.Assign(r1, m.Mem32(r2)),
                m => m.Return(0, 0)
            });

            string sExpected =
            #region Expected
                @"
l00001000:
    // pred:
    r1 = r2 + 3<32>
    r2 = Mem0[r1:word32]
    r1 = Mem0[r2:word32]
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }
    }
}
