#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DataObject = System.Windows.Forms.DataObject;
using DragDropEffects = Reko.Gui.Controls.DragDropEffects;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using DragEventHandler = System.Windows.Forms.DragEventHandler;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [NUnit.Framework.Category(Categories.UserInterface)]
    public class ProjectBrowserServiceTests
    {
        private readonly string cr = Environment.NewLine == "\r\n"
                ? "&#xD;&#xA;"
                : "&#xA;";
        private readonly char sep = Path.DirectorySeparatorChar;

        private ServiceContainer sc;
        private FakeTreeView fakeTree;
        private Mock<ITreeView> mockTree;
        private Mock<ITreeNodeCollection> mockNodes;
        private Program program;
        private Project project;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<IUiPreferencesService> uiPrefSvc;
        private Mock<ITabPage> tabPage;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            mockTree = new Mock<ITreeView>();
            mockNodes = new Mock<ITreeNodeCollection>();
            uiSvc = new Mock<IDecompilerShellUiService>();
            uiPrefSvc = new Mock<IUiPreferencesService>();
            tabPage = new Mock<ITabPage>();
            mockTree.Setup(t => t.Nodes).Returns(mockNodes.Object);
            uiSvc.Setup(u => u.SetContextMenu(It.IsAny<object>(), It.IsAny<int>()));
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
            sc.AddService<IUiPreferencesService>(uiPrefSvc.Object);
            fakeTree = new FakeTreeView();
        }

        private void Expect(string sExp)
        {
            Func<ITreeNode, XNode> render = null;
            render = new Func<ITreeNode, XNode>(n =>
            {
                var e = new XElement(
                    "node",
                    new XAttribute[] {
                        n.Text is not null ? new XAttribute("text", n.Text): null,
                        n.ToolTipText is not null ? new XAttribute("tip", n.ToolTipText) : null,
                        n.Tag is not null ? new XAttribute("tag", n.Tag.GetType().Name) : null
                    }.Where(a => a is not null),
                    n.Nodes.Select(c => render(c)));
                    
                return e;
            });
            var sb = new StringWriter();
            var xdoc = new XDocument(
                new XElement("root",
                    fakeTree.Nodes.Select(n => render(n))));
            xdoc.RemoveAnnotations<XProcessingInstruction>();
            xdoc.WriteTo(new XmlTextWriter(sb));
            try
            {
                Assert.AreEqual(sExp, sb.ToString());
            }
            catch
            {
                Console.WriteLine(sb.ToString());
                throw;
            }
        }

        #region Fake TreeView (Move to UnitTests.Fakes?)


        private class FakeTreeView : ITreeView
        {
            public event EventHandler AfterSelect;
            public event EventHandler<Reko.Gui.Controls.TreeViewEventArgs> AfterExpand;
            public event EventHandler<Reko.Gui.Controls.TreeViewEventArgs> BeforeExpand;
            public event DragEventHandler DragEnter;
            public event DragEventHandler DragOver;
            public event DragEventHandler DragDrop;
            public event EventHandler DragLeave;
            public event Reko.Gui.Controls.MouseEventHandler MouseWheel;
            public event EventHandler GotFocus;
            public event EventHandler LostFocus;

            public FakeTreeView()
            {
                this.Nodes = new FakeTreeNodeCollection();
            }

            event Reko.Gui.Controls.DragEventHandler ITreeView.DragEnter
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event Reko.Gui.Controls.DragEventHandler ITreeView.DragOver
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event Reko.Gui.Controls.DragEventHandler ITreeView.DragDrop
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            public object ContextMenu { get; set; }
            public ITreeNodeCollection Nodes { get; private set; }

            public ITreeNode SelectedNode { get { return selectedItem; } set { selectedItem = value; AfterSelect.Fire(this); } }
            private ITreeNode selectedItem;

            public bool Focused { get; set; }
            public bool Enabled { get; set; }
            public bool ShowRootLines { get; set; }
            public bool ShowNodeToolTips { get; set; }

            public Color ForeColor { get; set; }
            public Color BackColor { get; set; }

            public void CollapseAll()
            {

            }

            public ITreeNode CreateNode()
            {
                return new FakeTreeNode();
            }

            public ITreeNode CreateNode(string text)
            {
                return new FakeTreeNode { Text = text };
            }

            public void PerformAfterExpand(Reko.Gui.Controls.TreeViewEventArgs e)
            {
                AfterExpand?.Invoke(this, e);
            }

            public void PerformBeforeExpand(Reko.Gui.Controls.TreeViewEventArgs e)
            {
                BeforeExpand(this, e);
            }

            public void PerformDragEnter(DragEventArgs e)
            {
                DragEnter(this, e);
            }

            public void PerformDragOver(Reko.Gui.Controls.DragEventArgs e)
            {
                DragOver(this, new DragEventArgs(
                    (IDataObject)e.Data,
                    e.KeyState,
                    e.X,
                    e.Y,
                    (System.Windows.Forms.DragDropEffects) e.AllowedEffect,
                    (System.Windows.Forms.DragDropEffects) e.Effect));
            }

            public void PerformDragDrop(Reko.Gui.Controls.DragEventArgs e)
            {
                DragDrop(this, new DragEventArgs(
                    (IDataObject) e.Data,
                    e.KeyState,
                    e.X,
                    e.Y,
                    (System.Windows.Forms.DragDropEffects) e.AllowedEffect,
                    (System.Windows.Forms.DragDropEffects) e.Effect));
            }

            public void PerformDragLeave(EventArgs e)
            {
                DragLeave(this, e);
            }

            public void PerformMouseWheel(Reko.Gui.Controls.MouseEventArgs e)
            {
                MouseWheel(this, e);
            }

            public void PerformGotFocus(EventArgs e)
            {
                GotFocus(this, e);
            }

            public void PerformLostFocus(EventArgs e)
            {
                LostFocus(this, e);
            }

            public void Invoke(Action action)
            {
                throw new NotImplementedException();
            }

            public void Focus()
            {
                throw new NotImplementedException();
            }
        }

        public class FakeNodeCollection : ITreeNodeCollection
        {
            private List<ITreeNode> nodes = new List<ITreeNode>();

            public ITreeNode Add(string text)
            {
                var node = new FakeNode { Text = text };
                nodes.Add(node);
                return node;
            }

            public void AddRange(IEnumerable<ITreeNode> nodes)
            {
                this.nodes.AddRange(nodes);
            }

            public int IndexOf(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public ITreeNode this[int index]
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

            public void Add(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ITreeNode[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<ITreeNode> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class FakeNode : ITreeNode
        {
            public FakeNode()
            {
                Nodes = new FakeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public object Tag { get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Collapse()
            {
            }

            public void Expand()
            {
            }

            public void Invoke(Action a) { a();  }

            public void Remove()
            {
            }
        }

        [Designer(typeof(TestDesigner))]
        public class TestComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class ParentComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class GrandParentComponent
        {
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
                Nodes = new FakeTreeNodeCollection();
            }

            public object Tag { get; set; }
            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Collapse() { }
            public void Expand() { }
            public void Invoke(Action a) { a(); }

            public void Remove() { }
        }

        #endregion

        [Test]
        public void PBS_NoProject()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            pbs.Load(null);

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?><root><node text=\"(No project loaded)\" /></root>");
            Assert.IsFalse(fakeTree.ShowRootLines);
            Assert.IsFalse(fakeTree.ShowNodeToolTips);
        }

        private void Given_ProgramWithOneSegment()
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x12340000), new byte[0x1000]);
            var segmentMap = new SegmentMap(Address.Ptr32(0x12300000));
            segmentMap.AddSegment(mem, ".text", AccessMode.ReadExecute);
            var arch = new Mock<ProcessorArchitecture>(sc, "mmix", new Dictionary<string, object>());
            arch.Object.Description = "Foo Processor";
            var platform = new DefaultPlatform(sc, arch.Object);
            this.program = new Program(segmentMap, arch.Object, platform);
            this.program.Name = "foo.exe";
            this.program.Filename = @"c:\test\foo.exe";
            project.Programs.Add(program);
        }

        private void Given_ImportedFunction(Address addr, string module, string functionName)
        {
            Assert.IsNotNull(program);
            this.program.ImportReferences.Add(addr, new NamedImportReference(addr, module, functionName, SymbolType.ExternalProcedure));
        }

        private void Given_ImageMapItem(uint address)
        {
            this.program.ImageMap.AddItemWithSize(
                Address.Ptr32(address),
                new ImageMapItem(Address.Ptr32(address))
                {
                    Size = 4,
                    DataType = PrimitiveType.Int32,
                });
        }

        [Test]
        public void PBS_SingleBinary()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            Given_ImageMapItem(0x12340000);

            pbs.Load(project);

            Assert.IsTrue(fakeTree.ShowNodeToolTips);
            
            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12300000\" " +
                    "tag=\"ProgramDesigner\">" +
                    "<node text=\"Architectures\" tag=\"TreeNodeCollectionDesigner\">" +
                        "<node text=\"Foo Processor\" tag=\"ArchitectureDesigner\" />" +
                    "</node>" +
                    "<node text=\"(Unknown operating environment)\" tag=\"PlatformDesigner\" />" +
                    "<node " + 
                        "text=\".text\" " +
                        "tip=\".text" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "r-x" + "\" " +
                        "tag=\"ImageMapSegmentNodeDesigner\">" +
                            "<node " +
                                "text=\"Global variables\" " +
                                "tag=\"GlobalVariablesNodeDesigner\" />" +
                    "</node>" +
                    "<node tag=\"ProgramResourceGroupDesigner\" />" +
                "</node>" +
                "</root>");
        }

        [Test]
        public void PBS_SingleBinary_NoGlobals()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();

            pbs.Load(project);

            Assert.IsTrue(fakeTree.ShowNodeToolTips);

            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12300000\" " +
                    "tag=\"ProgramDesigner\">" +
                    "<node text=\"Architectures\" tag=\"TreeNodeCollectionDesigner\">"  +
                        "<node text=\"Foo Processor\" tag=\"ArchitectureDesigner\" />"  +
                    "</node>" +
                    "<node text=\"(Unknown operating environment)\" tag=\"PlatformDesigner\" />" +
                    "<node " +
                        "text=\".text\" " +
                        "tip=\".text" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "r-x" + "\" " +
                        "tag=\"ImageMapSegmentNodeDesigner\" />" +
                    "<node tag=\"ProgramResourceGroupDesigner\" />" +
                "</node>" +
                "</root>");
        }

        [Test]
        public void PBS_AddBinary()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            Given_ImageMapItem(0x12340000);

            pbs.Load(project);

            var mem = new ByteMemoryArea(Address.Ptr32(0x1231300), new byte[128]);
            
            project.Programs.Add(new Program
            {
                Filename = "bar.exe",
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(".text", mem, AccessMode.ReadExecute))
            });

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root><node text=\"foo.exe\" tip=\"c:\\test\\foo.exe" + cr + "12300000\" tag=\"ProgramDesigner\">" +
                    "<node text=\"Architectures\" tag=\"TreeNodeCollectionDesigner\">" +
                        "<node text=\"Foo Processor\" tag=\"ArchitectureDesigner\" />" +
                    "</node>" +
                    "<node text=\"(Unknown operating environment)\" tag=\"PlatformDesigner\" />" +
                    "<node text=\".text\" tip=\".text" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "r-x\" tag=\"ImageMapSegmentNodeDesigner\">" +
                        "<node text=\"Global variables\" tag=\"GlobalVariablesNodeDesigner\" />" +
                    "</node>" +
                    "<node tag=\"ProgramResourceGroupDesigner\" />" +
                 "</node>" +
                 "</root>");
        }

        private void Given_Project()
        {
            this.project = new Project
            {
            };
        }

        private void Given_UserProcedure(uint addr, string name)
        {
            program.User.Procedures.Add(
                Address.Ptr32(addr), new Reko.Core.Serialization.Procedure_v1
                {
                    Address = addr.ToString(),
                    Name = name
                });
        }

        [Test]
        public void PBS_UserProcedures()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            Given_ImageMapItem(0x12340000);
            Given_UserProcedure(0x12340500, "MyFoo");

            pbs.Load(project);

            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12300000\" " +
                    "tag=\"ProgramDesigner\">" +
                    "<node text=\"Architectures\" tag=\"TreeNodeCollectionDesigner\">" +
                        "<node text=\"Foo Processor\" tag=\"ArchitectureDesigner\" />" +
                    "</node>" +
                    "<node text=\"(Unknown operating environment)\" tag=\"PlatformDesigner\" />" +
                    "<node " +
                        "text=\".text\" " +
                        "tip=\".text" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "r-x" + "\" " +
                        "tag=\"ImageMapSegmentNodeDesigner\">" +
                        "<node " +
                            "text=\"Global variables\" " +
                            "tag=\"GlobalVariablesNodeDesigner\" />" +
                        "<node " +
                            "text=\"MyFoo\" " +
                            "tip=\"12340500\" " +
                            "tag=\"ProcedureDesigner\" />" +
                    "</node>" +
                    "<node tag=\"ProgramResourceGroupDesigner\" />" +
                "</node>" +
                "</root>");
        }

         
        [Test]
        public void PBS_AfterSelect_Calls_DoDefaultAction()
        {
            var des = new Mock<TreeNodeDesigner> { CallBase = true };
            des.Setup(d => d.DoDefaultAction()).Verifiable();
            des.Setup(d => d.Initialize(It.IsAny<object>()));
            des.Object.Component = "foo";
            var mockTree = new FakeTreeView();
            
            var pbs = new ProjectBrowserService(sc, tabPage.Object, mockTree);
            pbs.AddComponents(new object[] { des.Object });
            var desdes = pbs.GetDesigner(des.Object);
            Assert.IsNotNull(desdes);

            mockTree.SelectedNode = des.Object.TreeNode;

            des.Verify();
        }

        public class TestDesigner : TreeNodeDesigner
        {
        }

        [Test]
        public void PBS_FindGrandParent()
        {
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, tabPage.Object, mockTree);
            var gp = new GrandParentComponent();
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { gp });
            pbs.AddComponents(gp, new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.AreSame(gp, o);
        }

        [Test]
        public void PBS_NoGrandParent()
        {
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, tabPage.Object, mockTree);
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.IsNull(o);
        }

        [Test]
        public void PBS_AddTypeLib()
        {
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, tabPage.Object, mockTree);
            var project = new Project();
            pbs.Load(project);

            project.MetadataFiles.Add(new MetadataFile
            {
                Filename = ".." + sep + "foo.tlb"
            });

            Assert.AreEqual(1, mockTree.Nodes.Count);
            Assert.AreEqual("foo.tlb", mockTree.Nodes[0].Text);
        }

        [Test]
        public void PBS_AcceptFiles()
        {
            var mockTree = new FakeTreeView();
            var pbs = new WindowsProjectBrowserService(sc, tabPage.Object, mockTree);
            var e = Given_DraggedFile();

            var project = new Project();
            pbs.Load(project);
            var winEvent = new System.Windows.Forms.DragEventArgs(
                (IDataObject)e.Data,
                e.KeyState,
                e.X,
                e.Y,
                (System.Windows.Forms.DragDropEffects)e.AllowedEffect,
                (System.Windows.Forms.DragDropEffects)e.Effect);
            mockTree.PerformDragEnter(winEvent);
            Assert.AreEqual(DragDropEffects.Copy, e.Effect);
        }

        [Test]
        public void PBS_RejectTextDrop()
        {
            var mockTree = new FakeTreeView();
            var pbs = new WindowsProjectBrowserService(sc, tabPage.Object, mockTree);
            var e = Given_DraggedText();

            var project = new Project();
            pbs.Load(project);
            mockTree.PerformDragEnter(e);
            var winEvent = TreeViewWrapper.Convert(e);
            Assert.AreEqual(DragDropEffects.None, (DragDropEffects)(int) winEvent.Effect);
        }

        [Test]
        public void PBS_AcceptDrop()
        {
            string filename = null;
            var mockTree = new FakeTreeView();
            var pbs = new WindowsProjectBrowserService(sc, tabPage.Object, mockTree);
            pbs.FileDropped += (sender, ee) => { filename = ee.Filename; };
            var e = Given_DraggedFile();

            var project = new Project();
            pbs.Load(project);
            mockTree.PerformDragDrop(e);
            Assert.AreEqual("/home/bob/foo.exe", filename);
        }

        private Reko.Gui.Controls.DragEventArgs Given_DraggedFile()
        {
            var dObject = new DataObject(
                DataFormats.FileDrop,
                "/home/bob/foo.exe");

            return new Reko.Gui.Controls.DragEventArgs(
                    dObject, 0, 40, 40,
                    Reko.Gui.Controls.DragDropEffects.All,
                    Reko.Gui.Controls.DragDropEffects.All);
        }

        private DragEventArgs Given_DraggedText()
        {
            var dObject = new DataObject(
                DataFormats.UnicodeText,
                "hello world");

            return new DragEventArgs(
                    dObject, 0, 40, 40,
                    (System.Windows.Forms.DragDropEffects) (int) DragDropEffects.All,
                    (System.Windows.Forms.DragDropEffects) (int) DragDropEffects.All);
        }

        [Test]
        public void PBS_SingleBinary_Imports()
        {
            var pbs = new ProjectBrowserService(sc, tabPage.Object, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            Given_ImportedFunction(Address.Ptr32(0x123400), "KERNEL32", "LoadLibraryA");

            pbs.Load(project);

            Assert.IsTrue(fakeTree.ShowNodeToolTips);

            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12300000\" " +
                    "tag=\"ProgramDesigner\">" +
                    "<node text=\"Architectures\" tag=\"TreeNodeCollectionDesigner\">" +
                        "<node text=\"Foo Processor\" tag=\"ArchitectureDesigner\" />" +
                    "</node>" +
                    "<node text=\"(Unknown operating environment)\" tag=\"PlatformDesigner\" />" +
                    "<node " +
                        "text=\".text\" " +
                        "tip=\".text" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "r-x" + "\" " +
                        "tag=\"ImageMapSegmentNodeDesigner\" />" +
                    "<node text=\"Imports\" tag=\"ImportDesigner\" />" +
                    "<node tag=\"ProgramResourceGroupDesigner\" />" +
                "</node>" +
                "</root>");
        }
    }
}
