using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Arch.PowerPC;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    class PowerPcRewriterTests
    {
        private InstructionBuilder b;


        private void RunTest(Action<InstructionBuilder> m)
        {
            b = new InstructionBuilder(new Address(0x01000000));
            m(b);
        }

        private void AssertCode(params string[] s)
        {
            int i = 0;
            foreach (var instr in new PowerPcRewriter(b.Instructions, new Frame(PrimitiveType.Word32)))
            {
                var str = string.Format("{0}|{1}", i, instr);
                Assert.AreEqual(s[i], str);
                ++i;
                foreach (var rtl in instr.Instructions)
                {
                    str = string.Format("{0}|{1}", i, rtl);
                    Assert.AreEqual(s[i], str);
                    ++i;
                }
            }
            if (s.Length > i)
                Assert.Fail("Not enough expected statements");
        }


        [Test]
        public void Oris()
        {
            RunTest((m) =>
            {
                m.Oris(m.r4, m.r0, 0x1234);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|r4 = r0 | 0x12340000");
        }


        [Test]
        public void Add()
        {
            RunTest((m) =>
            {
                m.Add(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|r4 = r1 + r3");
        }

        [Test]
        public void Add_()
        {
            RunTest((m) =>
            {
                m.Add_(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|r4 = r1 + r3",
                "2|SCZO = cond(r4)");
        }
    }
}

