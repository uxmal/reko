using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Tools.C2Xml.UnitTests
{
    [TestFixture]
    public class TypedefTests

    
    {
        private CLexer lexer;

        private void CreateLexer(string text)
        {
            lexer = new CLexer(new StringReader(text));
        }

        [Test]
        public void ParseTypedef()
        {
            CreateLexer("typedef int GOO");
            CParser parser = new CParser(lexer.GetEnumerator());
            Typedef td = parser.ParseTypedef();
            Assert.AreEqual("int", td.TypeSpecifier.ToString());
            Assert.AreEqual("GOO", td.Declarators[0]);
        }
    }
}
