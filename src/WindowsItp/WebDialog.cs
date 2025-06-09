using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class WebDialog : Form
    {
        public WebDialog()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            var doc = webBrowser1.Document;
            var mem = new MemoryStream();
            var writer = new StreamWriter(mem);
            writer.Write(textBox1.Text);
            writer.Flush();
            webBrowser1.DocumentText = textBox1.Text;
        }

        private void WebDialog_Load(object sender, EventArgs e)
        {
            textBox1.Text = @"<html>
<body>
<pre id=""contents"">
</pre>
</body>
</html>";
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            webBrowser1.Navigating += webBrowser1_Navigating;

        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var spanner = webBrowser1.Document.GetElementById("spanner");
            if (spanner is not null)
                spanner.Click += spanner_Click;
            Contents.InnerHtml = GenerateContent("fnFoo");

        }

        private string GenerateContent(string proc)
        {
            return string.Format(
            @"void fnFoo()<br />
{{<br />
   <a href=""0123131"">{0}</a>();<br />
}}<br/>
",
                proc);
        }

        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (webBrowser1.Document is not null)
            {
                e.Cancel = true;
                HtmlElement contents = Contents;
                contents.InnerHtml = GenerateContent(e.Url.PathAndQuery);
            }
        }

        private HtmlElement Contents
        {
            get { return webBrowser1.Document.GetElementById("contents"); }
        }

        private void spanner_Click(object sender, HtmlElementEventArgs e)
        {
            ((HtmlElement)sender).Style = "color:red";
        }
    }
}
