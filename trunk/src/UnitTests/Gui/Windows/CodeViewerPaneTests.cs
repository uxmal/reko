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

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Controls;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using Rhino.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class CodeViewerPaneTests
    {
        private string nl = Environment.NewLine;

        private CodeViewerPane codeViewer;
        private MockRepository mr;
        private IDecompilerService decompilerSvc;
        private IDecompiler decompiler;
        private IUiPreferencesService uiPreferencesSvc;
        private IDecompilerShellUiService uiSvc;
        private Font font;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            codeViewer = new CodeViewerPane();
            decompilerSvc = mr.Stub<IDecompilerService>();
            decompiler = mr.Stub<IDecompiler>();
            uiPreferencesSvc = mr.Stub<IUiPreferencesService>();
            uiSvc = mr.Stub<IDecompilerShellUiService>();
            font = new Font("Arial", 10);
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
                foreach (var span in line)
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

        [Test]
        public void Cvp_Create()
        {
            using (Form f = new Form())
            {
                f.Controls.Add(codeViewer.CreateControl());
                f.Show();
            }
        }

        [Test]
        public void Cvp_SetProcedure()
        {
            codeViewer.CreateControl();
            var m = new ProcedureBuilder();
            m.Return();

            using (mr.Record())
            {
                var project = new Project { Programs = { new Program() } };
                decompiler.Stub(d => d.Project).Return(project);
                uiPreferencesSvc.SourceCodeFont = font;
            }

            codeViewer.DisplayProcedure(m.Procedure);

            string sExp =
                "void ProcedureBuilder()" + nl +
                "{" + nl +
                "ProcedureBuilder_entry:" + nl +
                "l1:" + nl +
                "    'return'" + nl +
                "ProcedureBuilder_exit:" + nl +
                "}" + nl;
            Assert.AreEqual(sExp, Flatten(codeViewer.TextView.Model));
        }
    }
}