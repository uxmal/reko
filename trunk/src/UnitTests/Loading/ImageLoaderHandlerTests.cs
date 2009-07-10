using Decompiler.Core;
using Decompiler.Loading;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Loading
{
    [TestFixture]
    public class ImageLoaderHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Match()
        {
            ImageLoaderHandler h = new ImageLoaderHandler();
            h.MagicNumber = "4711";
            Assert.IsTrue(h.ImageBeginsWithMagicNumber(new byte[] { 0x47, 0x11 }));
        }

        [Test]
        public void CreateInstance()
        {
            ImageLoaderHandler h = new ImageLoaderHandler();
            h.MagicNumber = "0FCD";
            h.LoaderType = "Decompiler.UnitTests.Loading.FakeImageLoader,Decompiler.UnitTests";
            ImageLoader ldr = h.CreateLoaderInstance(new Program(), new byte[] { 0x0F, 0xCD, 0x48, 0x65 });
            Console.WriteLine(ldr.GetType().ToString());
            Assert.AreEqual("Decompiler.UnitTests.Loading.FakeImageLoader", ldr.GetType().ToString());
        }
    }

    public class FakeImageLoader : ImageLoader
    {
        public FakeImageLoader(Program prog, byte[] bytes)
            : base(prog, bytes)
        {
        }

        public override ProgramImage Load(Decompiler.Core.Address addrLoad)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override ProgramImage LoadAtPreferredAddress()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Decompiler.Core.Address PreferredBaseAddress
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override void Relocate(Decompiler.Core.Address addrLoad, System.Collections.Generic.List<Decompiler.Core.EntryPoint> entryPoints, Decompiler.Core.RelocationDictionary relocations)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
