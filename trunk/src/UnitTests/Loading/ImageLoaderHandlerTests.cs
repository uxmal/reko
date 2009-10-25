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
