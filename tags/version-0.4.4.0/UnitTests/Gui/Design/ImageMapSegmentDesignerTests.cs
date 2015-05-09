#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Gui;
using Decompiler.Gui.Controls;
using Decompiler.Gui.Design;
using Decompiler.Core;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui.Design
{
    [TestFixture]
    public class ImageMapSegmentDesignerTests
    {
        private MockRepository mr;
        private ImageMapSegment seg1;
        private ImageMapSegment seg2;
        private ImageMap map;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            seg1 = new ImageMapSegment("seg1", AccessMode.Execute) { Address = Address.Ptr32(0x01000) };
            seg2 = new ImageMapSegment("seg2", AccessMode.Execute) { Address = Address.Ptr32(0x02000) };
            map = new ImageMap(seg1.Address, 0x4000);
        }

        [Test]
        public void Imd_Add_Empty()
        {
            var host = mr.StrictMock<ITreeNodeDesignerHost>();
            var treeNode = mr.StrictMock<ITreeNode>();
            treeNode.Expect(t => t.Text = "seg1");
            treeNode.Expect(t => t.ImageName = "RoSection.ico");
            treeNode.Expect(t => t.ToolTipText = "seg1\r\nAddress: 00001000\r\nSize: 0\r\n--x");
            var des = new ImageMapSegmentDesigner();
            des.Host = host;
            des.TreeNode = treeNode;
            des.Component = seg1;
            mr.ReplayAll();

            des.Initialize(seg1);

            mr.VerifyAll();
        }
    }
}
