using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using DecompilerHost = Decompiler.DecompilerHost;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests
{
    [TestFixture]
    public class DecompilerTests
    {
        FakeLoader loader;
        TestDecompiler decompiler;

        [SetUp]
        public void Setup()
        {
            var config = new FakeDecompilerConfiguration();
            loader = new FakeLoader();
            var host = new FakeDecompilerHost();
            var sp = new ServiceContainer();
            sp.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            decompiler = new TestDecompiler(loader, host, sp);
        }


        [Test]
        public void LoadIncompleteProject()
        {
            loader.ImageBytes = Encoding.UTF8.GetBytes(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<project xmlns=""http://schemata.jklnet.org/Decompiler"">
</project>");
            loader.ThrowOnLoadImage(new FileNotFoundException("file.dcproject"), 1);

            try
            {
                decompiler.LoadProgram("file.dcproject");
                Assert.Fail("Should have thrown exception.");
            }
            catch
            {
                Assert.IsNotNull(decompiler.Project);
                Assert.IsTrue(string.IsNullOrEmpty(decompiler.Project.Input.Filename));
                Assert.IsNull(decompiler.Program);
            }
        }

        [Test]
        public void LoadProjectFileNoBom()
        {
            loader.ImageBytes = new UTF8Encoding(false).GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><project xmlns=\"http://schemata.jklnet.org/Decompiler\">" +
                "<input><filename>foo.bar</filename></input></project>");
            decompiler.LoadProgram("test.dcproject");
            Assert.AreEqual("foo.bar", decompiler.Project.Input.Filename);
        }

    }

    public class TestDecompiler : DecompilerDriver
    {
        public TestDecompiler(LoaderBase loader, DecompilerHost host, IServiceProvider sp)
            : base(loader, host, sp)
        {
        }

    }

}
