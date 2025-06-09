#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using Reko.Gui.TextViewing;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Manages a preview window that pops up when the user hovers over an
    /// address.
    /// </summary>
    public class PreviewInteractor
    {
        private IServiceProvider services;
        private MixedCodeDataControl mixedCodeDataControl;
        private Control previewWindow;
        private bool insidePreview;
        private Address? addressPreview;
        private LayoutSpan previewSpan;
        private Timer previewTimer;
        private Program program;
        private Size szPreview;

        public PreviewInteractor(
            IServiceProvider services,
            Program program,
            Timer previewTimer,
            MixedCodeDataControl mixedCodeDataControl)
        {
            this.services = services;
            this.previewTimer = previewTimer;
            this.program = program;
            this.mixedCodeDataControl = mixedCodeDataControl;
            this.szPreview = new Size(500, 200); //$REVIEW: should this be a user preference?

            this.mixedCodeDataControl.MouseDown += MixedCodeDataControl_MouseDown;
            this.mixedCodeDataControl.SpanEnter += MixedCodeDataControl_SpanEnter;
            this.mixedCodeDataControl.SpanLeave += MixedCodeDataControl_SpanLeave;
            this.previewTimer.Tick += PreviewTimer_Tick;
        }

        private void MixedCodeDataControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (previewWindow is not null)
            {
                DestroyPreviewWindow();
            }
        }

        private void MixedCodeDataControl_SpanLeave(object sender, SpanEventArgs e)
        {
            if (!insidePreview)
                DestroyPreviewWindow();
        }

        private void MixedCodeDataControl_SpanEnter(object sender, SpanEventArgs e)
        {
            if (previewWindow is not null)
            {
                // Preview window already visible.
                return;
            }

            if (e.Span.Style is null || !e.Span.Style.Contains("dasm-addrText"))
                return;
            this.addressPreview = e.Span.Tag as Address?;
            if (this.addressPreview is null)
                return;

            // Start the timer; when it ticks, it will pop up the window.
            this.previewTimer.Enabled = true;
            this.previewSpan = e.Span;
        }

        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            // Timer needs to be disabled now so we don't get more ticks.
            this.previewTimer.Enabled = false;

            // Popup a sizeable tool window with a mixed code data control inside.
            var rcF = previewSpan.ContentExtent;
            var rc = mixedCodeDataControl.ClientRectangle;
            var ptScreen = mixedCodeDataControl.PointToScreen(
                new Point(100, (int)rcF.Bottom));

            var nested = new MixedCodeDataControl
            {
                Model = ((MixedCodeDataModel)this.mixedCodeDataControl.Model).Clone(),
                Program = this.mixedCodeDataControl.Program,      //$TODO: would be nice to avoid triggering recalc
                Dock = DockStyle.Fill,
                Services = this.services,
                StyleClass = this.mixedCodeDataControl.StyleClass,
                Padding = new Padding(3),
            };
            nested.VScrollBar.Visible = false;

            var parentForm = this.mixedCodeDataControl.FindForm();
            var frame = new Form
            {
                AutoSize = false,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Size = szPreview,
                StartPosition = FormStartPosition.Manual,
                Location = ptScreen,
                Controls = { nested },
                ShowInTaskbar = false,
                ControlBox = false,
            };
            parentForm.AddOwnedForm(frame);
            frame.Show();
            this.previewWindow = frame;
            nested.TopAddress = this.addressPreview;

            frame.MouseLeave += Lbl_MouseLeave;
            frame.MouseEnter += Lbl_MouseEnter;
            this.insidePreview = false;
        }

        private void Lbl_MouseLeave(object sender, EventArgs e)
        {
            DestroyPreviewWindow();
        }

        private void Lbl_MouseEnter(object sender, EventArgs e)
        {
            insidePreview = true;
        }

        private void DestroyPreviewWindow()
        {
            this.previewTimer.Enabled = false;
            if (previewWindow is null)
                return;
            this.mixedCodeDataControl.Controls.Remove(previewWindow);
            szPreview = previewWindow.Size;
            previewWindow.Dispose();
            previewWindow = null;
            insidePreview = false;
            previewSpan = null;
            addressPreview = null;

        }
    }
}
