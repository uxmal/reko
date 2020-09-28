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
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Design;
using System;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ImageMapSegmentDesignerTests
    {
        private static string nl = Environment.NewLine;

        private ImageSegment seg1;
        private ImageSegment seg2;
        private SegmentMap map;

        [SetUp]
        public void Setup()
        {
            seg1 = new ImageSegment("seg1", new MemoryArea(Address.Ptr32(0x01000), new byte[0x1000]), AccessMode.Execute);
            seg2 = new ImageSegment("seg2", new MemoryArea(Address.Ptr32(0x02000), new byte[0x1000]), AccessMode.Execute);
            map = new SegmentMap(seg1.Address,
                seg1, seg2);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Imd_Add_Empty()
        {
            var host = new Mock<ITreeNodeDesignerHost>();
            var treeNode = new Mock<ITreeNode>();
            treeNode.SetupSet(t => t.Text = "seg1").Verifiable();
            treeNode.SetupSet(t => t.ImageName = "RoSection.ico").Verifiable();
            treeNode.SetupSet(t => t.ToolTipText = "seg1" + nl  + "Address: 00001000" + nl  + "Size: 1000" + nl  + "--x");
            var des = new ImageMapSegmentNodeDesigner();
            des.Host = host.Object;
            des.TreeNode = treeNode.Object;
            des.Component = seg1;

            des.Initialize(seg1);

            treeNode.VerifyAll();
        }
    }
}
