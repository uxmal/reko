#if DEBUG
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.ImageLoaders.OdbgScript
{
    [TestFixture]
    class OllyLangTests
    {
        private OllyLang engine;

        [Test]
        public void Ose_var()
        {
            Given_Engine();
            Given_Script("var foo\r\n");

            engine.Run();

            Assert.IsTrue(engine.variables.ContainsKey("foo"));

        }

        private void Given_Script(string script)
        {
            engine.script.clear();
            engine.script.load_buff(script, ".");
        }

        private void Given_Engine()
        {
            engine = new OllyLang(new Host(null), new Debugger(null));
        }

        [Test]
        public void Ose_LineArgs()
        {
            var line = new OllyLang.OllyScript.Line();
            OllyLang.OllyScript.ParseArgumentsIntoLine( " hello,world", line);
            Assert.AreEqual(2, line.args.Length);
        }

        [Test]
        public void Ose_LineArgString()
        {
            var line = new OllyLang.OllyScript.Line();
            OllyLang.OllyScript.ParseArgumentsIntoLine(" \"hello,world\"", line);
            Assert.AreEqual(1, line.args.Length);
        }
    }
}

#endif
