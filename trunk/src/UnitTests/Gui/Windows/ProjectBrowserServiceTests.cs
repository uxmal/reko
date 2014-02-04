#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Controls;
using System.ComponentModel.Design;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class ProjectBrowserServiceTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private ITreeView mockTree;
        private ITreeNodeCollection mockNodes;
        private FakeTreeView fakeTree;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            mockTree = mr.StrictMock<ITreeView>();
            mockNodes = mr.StrictMock<ITreeNodeCollection>();
            mockTree.Stub(t => t.Nodes).Return(mockNodes);
            fakeTree = new FakeTreeView();
        }

        private void Expect(string sExp)
        {
            var x = new XElement("foo");
            var render = new Func<ITreeNode, XNode>(n =>
            {
                var e = new XElement(
                    "node",
                    new XAttribute[] {
                        n.Text != null ? new XAttribute("text", n.Text): null,
                        n.ToolTipText != null ? new XAttribute("tip", n.ToolTipText) : null,
                        n.Tag != null ? new XAttribute("tag", n.Tag.GetType().Name) : null
                    }.Where(a => a != null));
                return e;
            });
            var sb = new StringWriter();
            var xdoc = new XDocument(
                new XElement("root",
                    fakeTree.Nodes.Select(n => render(n))));
            xdoc.RemoveAnnotations<XProcessingInstruction>();
            xdoc.WriteTo(new XmlTextWriter(sb));
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }


        private class FakeTreeView : ITreeView
        {
            public FakeTreeView()
            {
                this.Nodes = new FakeTreeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }

            public object SelectedItem
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool ShowRootLines { get; set; }
            public bool ShowNodeToolTips { get; set; }

            public ITreeNode CreateNode()
            {
                return new FakeTreeNode();
            }

            public ITreeNode CreateNode(string text)
            {
                return new FakeTreeNode { Text = text };
            }
        }

        private class FakeTreeNodeCollection : List<ITreeNode>,  ITreeNodeCollection
        {
            public ITreeNode Add(string text)
            {
                var node = new FakeTreeNode { Text = text };
                base.Add(node);
                return node;
            }
        }

        private class FakeTreeNode : ITreeNode
        {
            public FakeTreeNode()
            {
                Nodes = new List<ITreeNode>();
            }

            public object Tag { get; set; }
            public IList<ITreeNode> Nodes { get; private set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }
        }


        [Test]
        public void PBS_NoProject()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            pbs.Load(null);

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?><root><node text=\"(No project loaded)\" /></root>");
            Assert.IsFalse(fakeTree.ShowRootLines);
            Assert.IsFalse(fakeTree.ShowNodeToolTips);
        }

        [Test]
        public void PBS_SingleBinary()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            pbs.Load(new Project
            {
                InputFiles = {
                    new InputFile {
                         Filename = "/home/fnord/project/executable",
                         BaseAddress = new Address(0x12340000),
                    }
                }
            });

            Assert.IsTrue(fakeTree.ShowNodeToolTips);
            var cr = Environment.NewLine == "\r\n"
                ? "&#xD;&#xA;"
                : "&#xA;";
            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"executable\" " +
                    "tip=\"/home/fnord/project/executable" + cr + "12340000\" " +
                    "tag=\"InputFile\" />" +
                "</root>");

        } 

    }
}
