#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core;
using Reko.Gui;
using Reko.Gui.Controls;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keys = System.Windows.Forms.Keys;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public partial class TypeMarker : UserControl, ITypeMarker
    {
        private const string TypeMarkerEnterType = "<Enter type>";
        private TaskCompletionSource<string> closeTask;

        public TypeMarker()
        {
            InitializeComponent();

            txtUserText.TextChanged += text_TextChanged;
            btnClose.Click += btnClose_Click;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
            case Keys.Escape:
                HideControls();
                this.closeTask.SetResult("");
                return true;
            case Keys.Enter:
                if (!string.IsNullOrEmpty(txtUserText.Text))
                {
                    this.closeTask.SetResult(txtUserText.Text);
                }
                HideControls();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public Task<string> ShowAsync(Program program, Address addr, Point location)
        {
            this.Location = new Point(location.X, location.Y + txtUserText.Height + 3);
            this.BringToFront();
            this.Visible = true;
            this.txtUserText.Focus();
            this.closeTask = new TaskCompletionSource<string>();
            this.lblCaption.Text = "Enter the &type and, optionally, the name for object at address " + addr;
            return this.closeTask.Task;
        }

        public void HideControls()
        {
            this.Visible = false;
        }

        public string FormatText(string text)
        {
            return text;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            var formattedText = FormatType(txtUserText.Text);
            if (formattedText.Length > 0)
            {
                lblRenderedType.ForeColor = SystemColors.ControlText;
                lblRenderedType.Text = formattedText;
            }
            else
            {
                lblRenderedType.ForeColor = SystemColors.GrayText;
                lblRenderedType.Text = TypeMarkerEnterType;
            }
        }

        public string FormatType(string text)
        {
            try
            {
                var dataType = HungarianParser.Parse(text);
                if (dataType is null)
                    return " - Null - ";
                else
                    return dataType.ToString();
            }
            catch
            {
                return " - Error - ";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.closeTask?.SetResult(txtUserText.Text);
        }
    }
}
