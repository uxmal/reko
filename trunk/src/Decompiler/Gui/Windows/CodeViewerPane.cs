#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public interface IWebBrowser
    {
        IHtmlDocument Document { get; }
        string DocumentText {get;set;}
        void SetInnerHtmlOfElement(string elementId, string innerHtml);

        event WebBrowserDocumentCompletedEventHandler DocumentCompleted;
        event WebBrowserNavigatingEventHandler Navigating;
    }

    public interface IHtmlDocument
    {
        HtmlElement GetElementById(string elementId);
        void Write(string html);
    }

    public class HtmlDocumentAdapter : IHtmlDocument
    {
        private HtmlDocument doc;

        public static HtmlDocumentAdapter Create(HtmlDocument doc)
        {
            return (doc != null) ? new HtmlDocumentAdapter(doc) : null;
        }

        private HtmlDocumentAdapter(HtmlDocument doc)
        {
            this.doc = doc;
        }

        public HtmlElement GetElementById(string elementId) { return doc.GetElementById(elementId); }
        public void Write(string html) { doc.Write(html); }
    }

    public class CodeViewerPane : IWindowPane
    {
        private IWebBrowser webControl; 
        private IDecompilerService decompilerSvc;
        private bool initialLoad;

        public IWebBrowser WebControl
        {
            get { return webControl; }
        }

        public HtmlElement Contents
        {
            get { return webControl.Document.GetElementById("contents"); }
        }

        protected virtual IWebBrowser CreateDockedWebBrowser()
        {
            return new WebBrowserAdapter();
        }

        #region IWindowPane Members

        public Control CreateControl()
        {
            this.webControl = CreateDockedWebBrowser();
            ((Control) this.webControl).Dock = DockStyle.Fill;
            ((Control) this.webControl).Name = "webBrowser";
            this.webControl.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webControl_DocumentCompleted);
            this.webControl.Navigating += new WebBrowserNavigatingEventHandler(webControl_Navigating);
            return (Control) webControl;
        }

        public void SetSite(IServiceProvider sp)
        {
            decompilerSvc = sp.GetService<IDecompilerService>();
        }

        public void Close()
        {
            if (webControl!=null)
                ((Control)webControl).Dispose();
        }

        #endregion

        public void DisplayProcedure(Procedure proc)
        {
            if (webControl == null)
                return;

            var sb = new StringWriter();
            var fmt = new HtmlCodeFormatter(sb, decompilerSvc.Decompiler.Program.Procedures);
            fmt.Write(proc);
            if (webControl.Document != null)
            {
                webControl.SetInnerHtmlOfElement("contents", sb.ToString());
            }
            else
            {
                SetDocumentText();
                webControl.Document.Write("<html><head>" +
                    "<style>.kw { color:blue } .comment { color:green } " +
                    "</style>" +
                    "</head>" + Environment.NewLine +
                    "<body><pre id=\"contents\">" +

                    "</pre></body></html>");
                webControl.SetInnerHtmlOfElement("contents", sb.ToString());
            }
        }

        void webControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!initialLoad)
            {
                initialLoad = true;
                SetDocumentText();
            }

        }

        private void SetDocumentText()
        {
            this.webControl.DocumentText = (
                    "<html><head>" +
                    "<style>.kw { color:blue } .comment { color:green } " +
                    "</style>" +
                    "</head>" + Environment.NewLine +
                    "<body><pre id=\"contents\">" +

                    "</pre></body></html>");
        }

        void webControl_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (webControl.Document == null)
                return;

            e.Cancel = true;    // this disables the annoying "click" of the IE web control
            if (e.Url.AbsolutePath == "blank")
                return;
            DisplayProcedure(GetProcedureFromUrl(e.Url));
        }

        private Procedure GetProcedureFromUrl(Uri uri)
        {
            Address addr = Address.ToAddress(uri.PathAndQuery, 16);
            return decompilerSvc.Decompiler.Program.Procedures[addr];
        }

        private class WebBrowserAdapter : WebBrowser, IWebBrowser
        {
            public WebBrowserAdapter()
            {
                AllowNavigation = true;
            }

            public new IHtmlDocument Document { get { return HtmlDocumentAdapter.Create(base.Document); } }

            public void SetInnerHtmlOfElement(string elementId, string innerHtml)
            {
                var contents = Document.GetElementById("contents");
                if (contents == null)
                    return;
                contents.InnerHtml = innerHtml;
            }
        }
    }
}
