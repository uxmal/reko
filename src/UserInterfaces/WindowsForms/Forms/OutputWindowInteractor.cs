#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class OutputWindowInteractor : IOutputService, IWindowPane
    {
        private readonly Dictionary<string, (ITextBox, TextWriter)> outputWindows;
        private ToolStripComboBox ddlOutputSources;
        private Panel outputWindowPanel;

        public OutputWindowInteractor()
        {
            this.outputWindows = new Dictionary<string, (ITextBox, TextWriter)>();
        }

        public IWindowFrame Frame { get; set; }

        public void Attach(ToolStripComboBox ddlOutputSources, Panel outputWindowPanel)
        {
            this.ddlOutputSources = ddlOutputSources;
            this.outputWindowPanel = outputWindowPanel;
            ddlOutputSources.TextChanged += DdlOutputSources_TextChanged;
            ddlOutputSources.SelectedIndexChanged += DdlOutputSources_TextChanged;
        }

        public TextWriter EnsureOutputSource(string source)
        {
            if (outputWindows.TryGetValue(source, out var regSource))
            {
                return regSource.Item2;
            }
            TextWriter writer = null;
            outputWindowPanel.Invoke(new Action(() =>
            {
                writer = AddOutputSource(source);
            }));
            return writer;
        }

        public object CreateControl()
        {
            return this.outputWindowPanel;
        }

        public void Close()
        {
        }

        public void SetSite(IServiceProvider services)
        {
        }

        private TextWriter AddOutputSource(string source)
        {
            var window = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Visible = true,
                Margin = new Padding(4, 4, 4, 4),
                Multiline = true,
            };
            ddlOutputSources.Items.Add(source);
            ddlOutputSources.SelectedIndex = ddlOutputSources.Items.Count - 1;
            var writer = new TextBoxWriter(window);
            outputWindows.Add(source, (new TextBoxWrapper(window), writer));
            this.outputWindowPanel.Controls.Add(window);
            return writer;
        }

        private void DdlOutputSources_TextChanged(object sender, EventArgs e)
        {
            var source = ddlOutputSources.Text;
            if (!this.outputWindows.TryGetValue(source, out var window))
                return;
            window.Item1.BringToFront();
        }

        private class TextBoxWriter : TextWriter
        {
            private readonly TextBox textbox;

            public TextBoxWriter(TextBox textbox)
            {
                this.textbox = textbox;
            }

            public override Encoding Encoding => Encoding.Default;

            public override void Write(char ch)
            {
                textbox.Invoke(new Action(() =>
                {
                    textbox.AppendText(new string(ch, 1));
                    textbox.SelectionStart = textbox.TextLength;
                    textbox.ScrollToCaret();
                }));
            }

            public override void Write(string s)
            {
                textbox.Invoke(new Action(() =>
                {
                    textbox.AppendText(s);
                    textbox.SelectionStart = textbox.TextLength;
                    textbox.ScrollToCaret();
                }));
            }
        }
    }
}
