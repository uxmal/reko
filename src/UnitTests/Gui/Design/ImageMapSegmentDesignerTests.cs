#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Design;
using Reko.Core;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ImageMapSegmentDesignerTests
    {
        private static string nl = Environment.NewLine;

        private MockRepository mr;
        private ImageSegment seg1;
        private ImageSegment seg2;
        private SegmentMap map;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            seg1 = new ImageSegment("seg1", new MemoryArea(Address.Ptr32(0x01000), new byte[0x1000]), AccessMode.Execute);
            seg2 = new ImageSegment("seg2", new MemoryArea(Address.Ptr32(0x02000), new byte[0x1000]), AccessMode.Execute);
            map = new SegmentMap(seg1.Address,
                seg1, seg2);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Imd_Add_Empty()
        {
            var host = mr.StrictMock<ITreeNodeDesignerHost>();
            var treeNode = mr.StrictMock<ITreeNode>();
            treeNode.Expect(t => t.Text = "seg1");
            treeNode.Expect(t => t.ImageName = "RoSection.ico");
            treeNode.Expect(t => t.ToolTipText = "seg1" + nl  + "Address: 00001000" + nl  + "Size: 1000" + nl  + "--x");
            var des = new ImageMapSegmentNodeDesigner();
            des.Host = host;
            des.TreeNode = treeNode;
            des.Component = seg1;
            mr.ReplayAll();

            des.Initialize(seg1);

            mr.VerifyAll();
        }
    }
}
