using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    public class AliasStateTests
    {
        private RegisterStorage eax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
        private RegisterStorage ah = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte) { BitAddress =8 };
        private RegisterStorage al = new RegisterStorage("al", 0, 0, PrimitiveType.Byte);

        [Test]
        public void Alst_Self()
        {
            var alst = new AliasState();
            alst.Add(eax);
            var aliases = alst.GetAliases(eax);
            Assert.AreEqual("eax", string.Join(" ", aliases.Select(a => ((RegisterStorage)a).Name).OrderBy(r => r)));
        }

        [Test]
        public void Alst_Subset()
        {
            var alst = new AliasState();
            alst.Add(eax);
            alst.Add(al);
            var aliases = alst.GetAliases(eax);
            Assert.AreEqual("al eax", string.Join(" ", aliases.Select(a => ((RegisterStorage)a).Name).OrderBy(r => r)));
        }

        [Test]
        public void Alst_NonOverlap()
        {
            var alst = new AliasState();
            alst.Add(ah);
            alst.Add(al);
            var aliases = alst.GetAliases(ah);
            Assert.AreEqual("ah", string.Join(" ", aliases.Select(a => ((RegisterStorage)a).Name).OrderBy(r => r)));
        }
    }
}
