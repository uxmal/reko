#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.UnitTests.Core
{
    [TestFixture]
	public class ProgramTests
	{
		private Program program;
        private Mock<IProcessorArchitecture> arch;
        private Address addrBase;

        [SetUp]
        public void Setup()
        {
            program = new Program();
        }

        private void Given_Architecture()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            program.Architecture = arch.Object;
        }

        private void Given_Image(params byte[] bytes)
        {
            addrBase = Address.Ptr32(0x00010000);
            var mem = new MemoryArea(addrBase, bytes);
            program.SegmentMap = new SegmentMap(addrBase);
            program.SegmentMap.AddSegment(mem, ".text", AccessMode.ReadWriteExecute);
            program.ImageMap = program.SegmentMap.CreateImageMap();
            program.Platform = new DefaultPlatform(null, arch.Object);
            arch.Setup(a => a.CreateImageReader(mem, addrBase)).Returns(new LeImageReader(mem, 0));
        }

        private void Given_ImageMapItem(Address address, DataType dataType)
        {
            this.program.ImageMap.AddItemWithSize(
                address,
                new ImageMapItem
                {
                    Address = address,
                    Size = (uint)dataType.Size,
                    DataType = dataType,
                });
        }

        private void Given_ImageMapBlock(Address address, uint size)
        {
            this.program.ImageMap.AddItemWithSize(
                address,
                new ImageMapBlock
                {
                    Address = address,
                    Size = size,
                });
        }

        [Test]
		public void Prog_EnsurePseudoProc()
		{
			var ppp = program.EnsurePseudoProcedure("foo", VoidType.Instance, new Identifier("", PrimitiveType.Int32, null));
			Assert.IsNotNull(ppp);
			Assert.AreEqual("foo", ppp.Name);
			Assert.AreEqual(1, program.PseudoProcedures.Count);

            var ppp2 = program.EnsurePseudoProcedure("foo", VoidType.Instance, new Identifier("", PrimitiveType.Int32, null));
			Assert.IsNotNull(ppp2);
			Assert.AreSame(ppp, ppp2);
			Assert.AreEqual("foo", ppp.Name);
			Assert.AreEqual(1, program.PseudoProcedures.Count);
		}

        [Test]
        public void Prog_ModifyUserGlobal()
        {
            Given_Architecture();
            Given_Image(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);

            var gbl1 = program.ModifyUserGlobal(arch.Object, addrBase, new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 8 }, "dValue");
            Assert.IsNotNull(gbl1);
            Assert.AreEqual("dValue", gbl1.Name);
            Assert.AreEqual(addrBase.ToString(), gbl1.Address.ToString());
            Assert.AreEqual("prim(Real,8)", gbl1.DataType.ToString());
            Assert.AreEqual(1, program.User.Globals.Count);
            Assert.AreSame(gbl1, program.User.Globals.Values[0]);
            Assert.AreEqual(1, program.ImageMap.Items.Count);
            var item = program.ImageMap.Items.Values[0];
            Assert.AreEqual("dValue", item.Name);
            Assert.AreEqual("real64", item.DataType.ToString());
            Assert.AreEqual("00010000", item.Address.ToString());
            Assert.AreEqual(8, item.Size);

            var gbl2 = program.ModifyUserGlobal(arch.Object, addrBase, new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 }, "fValue");
            Assert.IsNotNull(gbl2);
            Assert.AreSame(gbl1, gbl2);
            Assert.AreEqual("fValue", gbl2.Name);
            Assert.AreEqual(addrBase.ToString(), gbl2.Address.ToString());
            Assert.AreEqual("prim(Real,4)", gbl2.DataType.ToString());
            Assert.AreEqual(1, program.User.Globals.Count);
            Assert.AreSame(gbl2, program.User.Globals.Values[0]);
            Assert.AreEqual(2, program.ImageMap.Items.Count);
            var firstItem = program.ImageMap.Items.Values[0];
            Assert.AreEqual("fValue", firstItem.Name);
            Assert.AreEqual("real32", firstItem.DataType.ToString());
            Assert.AreEqual("00010000", firstItem.Address.ToString());
            Assert.AreEqual(4, firstItem.Size);
            var lastItem = program.ImageMap.Items.Values[1];
            Assert.AreEqual("<unknown>", lastItem.DataType.ToString());
            Assert.AreEqual("00010004", lastItem.Address.ToString());
            Assert.AreEqual(4, lastItem.Size);

            program.RemoveUserGlobal(addrBase);
            Assert.AreEqual(0, program.User.Globals.Count);
            Assert.AreEqual(1, program.ImageMap.Items.Count);
            item = program.ImageMap.Items.Values[0];
            Assert.AreEqual("<unknown>", item.DataType.ToString());
            Assert.AreEqual("00010000", item.Address.ToString());
            Assert.AreEqual(8, item.Size);
        }

        [Test]
        public void Prog_ModifyUserGlobal_Int32Item()
        {
            Given_Architecture();
            Given_Image(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
            Given_ImageMapItem(addrBase, PrimitiveType.Int32);

            var gbl = program.ModifyUserGlobal(
                arch.Object,
                addrBase,
                new PrimitiveType_v1
                {
                    Domain = Domain.Character,
                    ByteSize = 1
                },
                "ch");
            Assert.IsNotNull(gbl);
            var item = program.ImageMap.Items.Values[0];
            Assert.AreEqual(2, program.ImageMap.Items.Count);
            Assert.AreEqual("ch", item.Name);
            Assert.AreEqual("char", item.DataType.ToString());
            Assert.AreEqual("00010000", item.Address.ToString());
            Assert.AreEqual(1, item.Size);
        }

        [Test]
        public void Prog_RemoveUserGlobal()
        {
            Given_Architecture();
            Given_Image(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
            Given_ImageMapItem(addrBase, PrimitiveType.Int32);

            program.RemoveUserGlobal(addrBase);
            Assert.AreEqual(1, program.ImageMap.Items.Count);
            var item = program.ImageMap.Items.Values[0];
            Assert.AreEqual("<unknown>", item.DataType.ToString());
            Assert.AreEqual("00010000", item.Address.ToString());
            Assert.AreEqual(8, item.Size);
        }

        [Test]
        public void Prog_RemoveUserGlobal_BlockItem()
        {
            Given_Architecture();
            Given_Image(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
            Given_ImageMapBlock(addrBase, 5);

            program.RemoveUserGlobal(addrBase);
            /* block items should not be removed*/
            Assert.AreEqual(2, program.ImageMap.Items.Count);
            var firstItem = program.ImageMap.Items.Values[0];
            Assert.AreEqual("code", firstItem.DataType.ToString());
            Assert.AreEqual("00010000", firstItem.Address.ToString());
            Assert.AreEqual(5, firstItem.Size);
            var lastItem = program.ImageMap.Items.Values[1];
            Assert.AreEqual("<unknown>", lastItem.DataType.ToString());
            Assert.AreEqual("00010005", lastItem.Address.ToString());
            Assert.AreEqual(3, lastItem.Size);
        }

        [Test]
        public void Prog_GetDataSize_of_Integer()
        {
            Given_Architecture();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);

            Assert.AreEqual(4u, program.GetDataSize(program.Architecture, addrBase, PrimitiveType.Int32));
        }


        [Test]
        public void Prog_GetDataSize_of_ZeroTerminatedString()
        {
            Given_Architecture();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);

            var dt = StringType.NullTerminated(PrimitiveType.Char);
            Assert.AreEqual(6u, program.GetDataSize(program.Architecture, addrBase, dt), "5 bytes for 'hello' and 1 for the terminating null'");
        }

        [Test(Description = "GitHub issue #829")]
        public void Prog_EnsureProcedure_Override_ImageSymbol_Name()
        {
            Given_Architecture();
            Given_Image(0xC3);
            var addr = program.SegmentMap.BaseAddress;
            var symbol = ImageSymbol.Procedure(this.arch.Object, addr, name: "NameToOverride");

            var proc = program.EnsureProcedure(this.arch.Object, addr, "NewName");

            Assert.AreEqual("NewName", proc.Name);
        }
	}
}
