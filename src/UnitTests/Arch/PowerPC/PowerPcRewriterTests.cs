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
        public void PPCRW_Oris()
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
        public void PPCRW_Add()
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
        public void PPCRW_Add_()
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

        [Test]
        public void PPCRW_lwzu()
        {
            RunTest((m) =>
            {
                m.Lwzu(m.r2, 4, m.r1); 
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|r2 = Mem0[r1 + 4:word32]",
                "2|r1 = r1 + 4"
                );
        }

        [Test]
        public void PPCRW_lwz_r0()
        {
            RunTest((m) =>
            {
                m.Lwz(m.r2, -4, m.r0);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|r2 = Mem0[0xFFFFFFFC:word32]"
                );
        }
        [Test]
        public void PPCRW_stbu()
        {
            RunTest((m) =>
            {
                m.Stbu(m.r2, 18, m.r3);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|Mem0[r3 + 18:byte] = (byte) r2",
                "2|r3 = r3 + 18"
                );
        }

        [Test]
        public void PPCRW_stbux()
        {
            RunTest((m) =>
            {
                m.Stbux(m.r2, m.r3, m.r0);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|Mem0[r3 + r0:byte] = (byte) r2",
                "2|r3 = r3 + r0"
                );
        }
    }
}

