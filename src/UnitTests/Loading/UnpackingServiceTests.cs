using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Loading;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Loading
{
    [TestFixture]
    public class UnpackingServiceTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private IDecompilerConfigurationService cfgSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            cfgSvc = mr.Stub<IDecompilerConfigurationService>();
        }

        [Test]
        public void Upsvc_Find_Match()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x00, 0x00 };
          
            var le = mr.Stub<LoaderElement>();
            le.Label = "LoaderKey";
            le.TypeName = "Decompiler.UnitTests.Loading.TestImageLoader,Decompiler.UnitTests";
            cfgSvc.Stub(c => c.GetImageLoaders()).Return(new List<LoaderElement> { le });
            sc.AddService(typeof(IDecompilerConfigurationService), cfgSvc);
            mr.ReplayAll();

            var upSvc = new UnpackingService(sc);
            upSvc.Signatures.Add(new ImageSignature
            {
                Name = "LoaderKey",
                EntryPointPattern = "1234",
            });
            var loader = upSvc.FindUnpackerBySignature("foo.exe", image, 4);
            Assert.IsInstanceOf<TestImageLoader>(loader);
        }

        [Test]
        public void Upsvc_Match()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x00 };
            var sig = new ImageSignature
            {
                EntryPointPattern = "1234"
            };
            var upsvc = new UnpackingService(sc);

            Assert.IsTrue(upsvc.Matches(sig, image, 4));
        }

        [Test]
        public void Upsvc_Match_Wildcard()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x00 };
            var sig = new ImageSignature
            {
                EntryPointPattern = "??34"
            };
            var upsvc = new UnpackingService(sc);

            Assert.IsTrue(upsvc.Matches(sig, image, 4));
        }
    }

    public class TestImageLoader : ImageLoader
    {
        public TestImageLoader(IServiceProvider services, string filename, byte[] bytes) : base(services, filename, bytes)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override LoaderResults Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
