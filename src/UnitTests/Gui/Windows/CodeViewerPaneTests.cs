#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Windows;
using Reko.Gui.Windows.Controls;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category("UserInterface")]
    public class CodeViewerPaneTests
    {
        private string nl = Environment.NewLine;

        private CodeViewerPane codeViewer;
        private MockRepository mr;
        private MockFactory mockFactory;
        private IDecompilerService decompilerSvc;
        private IDecompiler decompiler;
        private IUiPreferencesService uiPreferencesSvc;
        private IDecompilerShellUiService uiSvc;
        private Font font;
        private Program program;
        private Procedure proc;
        private IWindowFrame frame;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            mockFactory = new MockFactory(mr);
            var platform = mockFactory.CreatePlatform(); ;

            program = new Program
            {
                Architecture = platform.Architecture,
                Platform = platform,
            };
            codeViewer = new CodeViewerPane();
            decompilerSvc = mr.Stub<IDecompilerService>();
            decompiler = mr.Stub<IDecompiler>();
            uiPreferencesSvc = mr.Stub<IUiPreferencesService>();
            uiSvc = mr.Stub<IDecompilerShellUiService>();
            frame = mr.Stub<IWindowFrame>();
            font = new Font("Arial", 10);
            var styles = new Dictionary<string, UiStyle>()
            {
                {
                    UiStyles.CodeWindow,
                    new UiStyle
                    {
                        Background = new SolidBrush(Color.White),
                    }
                }
            };
            uiPreferencesSvc.Stub(u => u.Styles).Return(styles);
            var sc = new ServiceContainer();
            decompilerSvc.Decompiler = decompiler;
            sc.AddService<IDecompilerService>(decompilerSvc);
            sc.AddService<IUiPreferencesService>(uiPreferencesSvc);
            sc.AddService<IDecompilerShellUiService>(uiSvc);
            codeViewer.SetSite(sc);
        }

        [TearDown]
        public void TearDown()
        {
            codeViewer.Close();
            if (font != null) font.Dispose();
        }

        private string Flatten(TextViewModel model)
        {
            var sb = new StringBuilder();
            var lines = model.GetLineSpans(model.LineCount);
            foreach (var line in lines)
            {
                foreach (var span in line.TextSpans)
                {
                    EmitSpanWrapper(span, sb);
                    sb.Append(span.GetText());
                    EmitSpanWrapper(span, sb);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void EmitSpanWrapper(TextSpan span, StringBuilder sb)
        {
            if (span.Style == "kw")
                sb.Append("'");
            if (span.Style == "cmt")
                sb.Append("rem ");
            if (span.Style == "link")
                sb.Append("_");
            if (span.Style == "type")
                sb.Append("`");
        }

        private void When_CodeViewCreated()
        {
            codeViewer.CreateControl();
            codeViewer.FrameWindow = frame;
        }

        [Test(Description = "When a user browses to the procedure, we should see it")]
        public void Cvp_SetProcedure()
        {
            Given_StubProcedure();
            Given_Program();
            mr.ReplayAll();

            When_CodeViewCreated();
            codeViewer.DisplayProcedure(this.program, this.proc);

            string sExp =
                "void fnTest()" + nl +
                "{" + nl +
                "fnTest_entry:" + nl +
                "l1:" + nl +
                "    'return'" + nl +
                "fnTest_exit:" + nl +
                "}" + nl;
            Assert.AreEqual(sExp, Flatten(codeViewer.TextView.Model));
        }

        private void Given_StubProcedure()
        {
            var m = new ProcedureBuilder("fnTest");
            m.Return();
            this.proc = m.Procedure;
            this.program.Procedures[Address.Ptr32(0x1234)] = proc;
        }

        private void Given_Program()
        {
            var project = new Project { Programs = { program } };
            decompiler.Stub(d => d.Project).Return(project);
        }

        [Test(Description = "When a previously uncustomized procedure is displayed, show its name in the "+
            "declaration box")]
        public void Cvp_SetProcedure_ShowFnName()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            codeViewer.DisplayProcedure(program, proc);

            Assert.AreEqual(proc.Name, codeViewer.Declaration.Text);
        }

        [Test]
        public void Cvp_SetProcedure_EditingMakesDirty()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            codeViewer.DisplayProcedure(program, proc);
            Assert.IsFalse(codeViewer.IsDirty, "Shouldn't be dirty right after loading");        
            codeViewer.Declaration.Text = "foo";

            Assert.IsTrue(codeViewer.IsDirty, "Should be dirty after editing");
        }

        [Test(Description = "Just entering a (valid) name should be OK.")]
        public void Cvp_Accept_JustName()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            codeViewer.DisplayProcedure(program, proc);
            codeViewer.Declaration.Text = "foo";

            Assert.AreEqual("foo", proc.Name);
            Assert.IsNull(program.User.Procedures.Values.First().CSignature);
        }

        [Test(Description = "Entering an invalid name should change nothing.")]
        public void Cvp_Reject_Invalid_Name()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            var oldSig = proc.Signature;
            codeViewer.DisplayProcedure(program, proc);
            codeViewer.Declaration.Text = "f@oo";

            Assert.AreEqual("fnTest", proc.Name);
        }

        [Test(Description = "Entering an valid function declaration should change both name and signature.")]
        public void Cvp_Accept_Declaration()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            var oldSig = proc.Signature;
            codeViewer.DisplayProcedure(program, proc);
            codeViewer.Declaration.Text = "int foo(char *, float)";

            Assert.AreEqual("foo", proc.Name);
            Assert.AreEqual("int foo(char *, float)", program.User.Procedures.Values.First().CSignature);
        }

        [Test(Description = "Char * functions are valid, of course")]
        public void Cvp_Accept_Declaration_Returning_CharPtr()
        {
            Given_Program();
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            var oldSig = proc.Signature;
            codeViewer.DisplayProcedure(program, proc);
            codeViewer.Declaration.Text = "char * foo(int)";

            Assert.AreEqual("foo", proc.Name);
            Assert.AreEqual("char * foo(int)", program.User.Procedures.Values.First().CSignature);
        }

        [Test]
        public void Cvp_Accept_Declaration_PlatformTypes()
        {
            var types = new Dictionary<string, DataType>()
            {
                { "BYTE", PrimitiveType.Create(PrimitiveType.Byte.Domain, 1) },
            };
            Given_Program();
            mockFactory.Given_PlatformTypes(types);
            Given_StubProcedure();
            mr.ReplayAll();

            When_CodeViewCreated();
            var oldSig = proc.Signature;
            codeViewer.DisplayProcedure(program, proc);
            codeViewer.Declaration.Text = "BYTE foo(BYTE a, BYTE b)";

            Assert.AreEqual("foo", proc.Name);
            Assert.AreEqual("BYTE foo(BYTE a, BYTE b)", program.User.Procedures.Values.First().CSignature);
        }
    }
}
