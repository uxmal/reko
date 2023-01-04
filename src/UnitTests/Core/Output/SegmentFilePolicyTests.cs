#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class SegmentFilePolicyTests
    {
        private FakeDecompilerEventListener listener;
        private Program program;

        [SetUp]
        public void Setup()
        {
            var segs = new SegmentMap(Address.Ptr32(0x00100000));
            var sc = new ServiceContainer();
            var platform = new Mocks.FakePlatform(sc, new Mocks.FakeArchitecture(sc));
            this.listener = new FakeDecompilerEventListener();
            this.program = new Program(segs, platform.Architecture, platform)
            {
                Name = "myprogram.exe"
            };
            var treeNode = new StructureType();
            treeNode.Fields.Add(0, new Pointer(treeNode, 32), "left");
            treeNode.Fields.Add(4, new Pointer(treeNode, 32), "right");
            treeNode.Fields.Add(8, PrimitiveType.Int32, "data");
        }

        private void Given_Executable(string name, uint uAddr, uint uSize)
        {
            var addr = Address.Ptr32(uAddr);
            Given_Executable(name, addr, uSize);
        }

        private void Given_Executable(string name, Address addr, uint uSize)
        {
            var seg = new ImageSegment(
                name,
                new ByteMemoryArea(addr, new byte[uSize]),
                AccessMode.Execute);
            program.SegmentMap.AddSegment(seg);
        }

        private void Given_Segment(string name, uint uAddr, uint uSize)
        {
            var addr = Address.Ptr32(uAddr);
            Given_Segment(name, addr, uSize);
        }

        private void Given_Segment(string name, Address addr, uint uSize)
        {
            var seg = new ImageSegment(
                name,
                new ByteMemoryArea(addr, new byte[uSize]),
                AccessMode.ReadWrite);
            program.SegmentMap.AddSegment(seg);
        }

        private void Given_Procedure(uint uAddr)
        {
            var addr = Address.Ptr32(uAddr);
            var proc = Procedure.Create(program.Architecture, addr, program.Architecture.CreateFrame());
            program.Procedures.Add(addr, proc);
        }

        private void Given_UserProcedure(uint uAddr, string procName, string placement)
        {
            var addr = Address.Ptr32(uAddr);
            var uProc = new UserProcedure(addr, procName)
            {
                OutputFile = placement
            };
            program.User.Procedures.Add(addr, uProc);
        }

        private void Given_Data(uint uAddr, string hexBytes)
        {
            var addr = Address.Ptr32(uAddr);
            Given_Data(addr, hexBytes);
        }

        private void Given_Data(Address addr, string hexBytes)
        {
            bool foundSeg = program.SegmentMap.TryFindSegment(addr, out var seg);
            Assert.IsTrue(foundSeg);
            var w = seg.MemoryArea.CreateLeWriter(addr);
            w.WriteBytes(BytePattern.FromHexBytes(hexBytes));
        }

        private void Given_GlobalVariable(int uOffset, DataType dt)
        {
            program.GlobalFields.Fields.Add(uOffset, dt);
        }

        private void Given_SegmentVariable(ushort uSeg, uint off, DataType dt)
        {
            var addr = Address.SegPtr(uSeg, off);
            bool found = program.SegmentMap.TryFindSegment(addr, out var seg);
            Assert.IsTrue(found, "Segment is not found");
            seg.Fields.Fields.Add((int)off, dt);
            CreateSegmentTypeVariable(seg);
        }

        private void CreateSegmentTypeVariable(ImageSegment seg)
        {
            var factory = new TypeFactory();
            var tv = program.TypeStore.CreateTypeVariable(factory);
            program.TypeStore.SetTypeVariable(seg.Identifier, tv);
            tv.Class.DataType = seg.Fields;
        }

        [Test]
        public void Segfp_Single_Segment()
        {
            Given_Executable(".text", 0x00100000, 0x4000);
            Given_Procedure(0x00101000);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".asm", listener).ToArray();
            Assert.AreEqual(1, placements.Length);
            Assert.AreEqual("myprogram_text.asm", placements[0].Key);
            var procs = placements[0].Value;
            Assert.AreEqual(1, procs.Count);
            Assert.AreEqual("fn00101000", procs.Values.Cast<Procedure>().First().Name);
        }

        [Test]
        public void Segfp_Single_Segment_Two_procs()
        {
            Given_Executable(".text", 0x00100000, 0x4000);
            Given_Procedure(0x00101000);
            Given_Procedure(0x00101400);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".asm", listener).ToArray();
            Assert.AreEqual(1, placements.Length);
            Assert.AreEqual("myprogram_text.asm", placements[0].Key);
            var procs = placements[0].Value.Values.Cast<Procedure>().ToArray();
            Assert.AreEqual(2, procs.Length);
            Assert.AreEqual("fn00101000", procs[0].Name);
            Assert.AreEqual("fn00101400", procs[1].Name);
        }

        [Test]
        public void Segfp_Two_segments_two_procs()
        {
            Given_Executable(".text", 0x00100000, 0x4000);
            Given_Executable(".init", 0x00200000, 0x4000);
            Given_Procedure(0x00101000);
            Given_Procedure(0x00201400);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".asm", listener).ToArray();
            Assert.AreEqual(2, placements.Length);
            Assert.AreEqual("myprogram_text.asm", placements[0].Key);
            Assert.AreEqual("myprogram_init.asm", placements[1].Key);
            var procs = placements[0].Value.Values.Cast<Procedure>().ToArray();
            Assert.AreEqual(1, procs.Length);
            Assert.AreEqual("fn00101000", procs[0].Name);
            procs = placements[1].Value.Values.Cast<Procedure>().ToArray();
            Assert.AreEqual(1, procs.Length);
            Assert.AreEqual("fn00201400", procs[0].Name);
        }

        // In binaries with large segments, we divide the segment up in to subregions,
        // arbitrarily at 64-kiB boundaries. Users can always override this if they want.
        [Test]
        public void Segfp_large_segment()
        {
            Given_Executable(".text", 0x0010_0000, 0x10_0000);
            Given_Procedure(0x00101000);
            Given_Procedure(0x00113F00);
            Given_Procedure(0x00133F00);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".asm", listener).ToArray();
            Assert.AreEqual(3, placements.Length);
            Assert.AreEqual("myprogram_text_0000.asm", placements[0].Key);
            Assert.AreEqual("myprogram_text_0001.asm", placements[1].Key);
            Assert.AreEqual("myprogram_text_0003.asm", placements[2].Key);
            Assert.IsTrue(placements.All(p => p.Value.Count == 1));
        }

        // Allow users to override the default placement.
        [Test]
        public void Segfp_user_placement()
        {
            Given_Executable(".text", 0x0010_0000, 0x4000);
            Given_Procedure(0x00101000);
            Given_Procedure(0x00101100);
            Given_UserProcedure(0x00101000, "myproc", "myproc.c");

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".asm", listener).ToArray();
            Assert.AreEqual(2, placements.Length);
            Assert.AreEqual("myproc.asm", placements[0].Key);
            Assert.AreEqual("myprogram_text.asm", placements[1].Key);
        }

        [Test]
        public void SegFp_object_placement()
        {
            Given_Executable(".text", 0x0010_0000, 0x4000);
            Given_Segment(".data", 0x0020_0000, 0x4000);
            Given_Data(0x0020_0010, "78 56 34 12");
            Given_GlobalVariable(0x0020_0010, PrimitiveType.Int32);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".c", listener).ToArray();
            Assert.AreEqual(1, placements.Length);
            Assert.AreEqual("myprogram_data.c", placements[0].Key);
            var objs = placements[0].Value.OrderBy(k => k.Key).ToArray();
            Assert.AreEqual(1, objs.Length);
        }

        [Test]
        public void SegFp_segmented_global_placement()
        {
            Given_Executable(".text", Address.SegPtr(0x0010, 0x0000), 0x4000);
            Given_Segment(".data", Address.SegPtr(0x0020, 0x0000), 0x4000);
            Given_Data(Address.SegPtr(0x0020, 0010), "78 56 34 12");
            Given_SegmentVariable(0x0020, 0010, PrimitiveType.Int32);

            var ofp = new SegmentFilePolicy(program);
            var placements = ofp.GetObjectPlacements(".c", listener).ToArray();
            Assert.AreEqual(1, placements.Length);
            Assert.AreEqual("myprogram_data.c", placements[0].Key);
            var objs = placements[0].Value.OrderBy(k => k.Key).ToArray();
            Assert.AreEqual(1, objs.Length);
        }
    }
}
