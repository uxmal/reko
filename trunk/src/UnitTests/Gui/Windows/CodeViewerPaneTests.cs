#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using Rhino.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]   
    public class CodeViewerPaneTests
    {
        private string nl = Environment.NewLine;

        private TestCodeViewerPane codeViewer;
        private MockRepository repository;
        private IDecompilerService decompilerSvc;
        private IDecompiler decompiler;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            codeViewer = new TestCodeViewerPane();
            decompilerSvc = repository.Stub<IDecompilerService>();
            decompiler = repository.Stub<IDecompiler>();
            var sc = new ServiceContainer();
            decompilerSvc.Decompiler = decompiler;
            sc.AddService<IDecompilerService>(decompilerSvc);
            codeViewer.SetSite(sc);
        }

        [TearDown]
        public void TearDown()
        {
            codeViewer.Close();
        }

        [Test]
        public void Create()
        {
            using (Form f = new Form())
            {
                f.Controls.Add(codeViewer.CreateControl());
                f.Show();
            }
        }

        [Test]
        public void SetProcedure()
        {
            codeViewer.CreateControl();
            var m = new ProcedureBuilder();
            m.Return();

            using (repository.Record())
            {
                var prog = new Program();
                decompiler.Stub(x => decompiler.Program).Return(prog);
            }
            codeViewer.DisplayProcedure(m.Procedure);
            string sExp =
                "<html><head><style> </style></head>" + nl +
                "<body><pre id=\"contents\">void ProcedureMock()<br />" + nl +
                "{<br />" + nl + 
                "ProcedureMock_entry:<br />" + nl +
                "}<br />" +nl +
                "</pre></body></html>";
            Assert.AreEqual(sExp, codeViewer.WebControl.DocumentText);
        }

        private class TestCodeViewerPane : CodeViewerPane
        {
            protected override IWebBrowser CreateDockedWebBrowser()
            {
                return new FakeWebBrowser();
            }
        }

        private class FakeWebBrowser : TextBox, IWebBrowser
        {
            #region IWebBrowser Members

            public HtmlDocument Document
            {
                get { throw new NotImplementedException(); }
            }

            public string DocumentText
            {
                get { return Text; } set { Text= value; }
            }

            public void SetInnerHtmlOfElement(string elementId, string innerHtml)
            {
                string str = Text;
                int i = str.IndexOf("ents\">") + 6;
                Text = str.Substring(0, i) + innerHtml + str.Substring(i);
            }

            public event WebBrowserDocumentCompletedEventHandler DocumentCompleted;

            public event WebBrowserNavigatingEventHandler Navigating;

            #endregion
        }
    }


}
