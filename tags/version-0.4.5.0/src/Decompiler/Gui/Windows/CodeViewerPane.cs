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
using Decompiler.Core.Output;
using Decompiler.Gui.Windows.Controls;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Pane that displays decompiled code.
    /// </summary>
    public class CodeViewerPane : IWindowPane, ICommandTarget
    {
        private CodeView codeView; 
        private IServiceProvider services;
        private Procedure proc;

        public TextView TextView { get { return codeView.TextView; } }

        #region IWindowPane Members

        public Control CreateControl()
        {
            var uiPrefsSvc = services.RequireService<IUiPreferencesService>();

            this.codeView = new CodeView();
            this.codeView.Dock = DockStyle.Fill;
            this.TextView.Font = uiPrefsSvc.DisassemblerFont ?? new Font("Lucida Console", 10F);
            this.TextView.BackColor = SystemColors.Window;

            this.TextView.ContextMenu = services.RequireService<IDecompilerShellUiService>().GetContextMenu(MenuIds.CtxCodeView);

            this.TextView.Styles.Add("kw", new EditorStyle
            {
                Foreground = new SolidBrush(Color.Blue),
            });
            this.TextView.Styles.Add("link", new EditorStyle
            {
                Foreground = new SolidBrush(Color.FromArgb(0x00, 0x80, 0x80)),
                Cursor = Cursors.Hand,
            });
            this.TextView.Styles.Add("cmt", new EditorStyle
            {
                Foreground = new SolidBrush(Color.FromArgb(0x00, 0x80, 0x00)),
            });
            this.TextView.Styles.Add("type", new EditorStyle
            {
                Foreground = new SolidBrush(Color.FromArgb(0x00, 0x60, 0x60)),
            });

            this.TextView.Navigate += textView_Navigate;
            return this.codeView;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
            if (codeView!=null)
                ((Control)codeView).Dispose();
            codeView = null;
        }

        #endregion

        public void DisplayProcedure(Procedure proc)
        {
            if (codeView == null || proc == null)
                return;

            this.proc = proc;
            var tsf = new TextSpanFormatter();
            var fmt = new CodeFormatter(tsf);
            fmt.InnerFormatter.UseTabs = false;
            fmt.Write(proc);
            this.TextView.Model = tsf.GetModel();
        }

        void textView_Navigate(object sender, EditorNavigationArgs e)
        {
            var procDst = e.Destination as Procedure;
            if (procDst == null)
                return;
            DisplayProcedure(procDst);
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:   //$TODO: once the TextViewer supports selections, these two 
                                        // verbs will need to differ.
                case CmdIds.EditCopyAll:
                    status.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                case CmdIds.EditCopyAll:
                    CopyAll();
                    return true;
                }
            }
            return false;
        }

        public void CopyAll()
        {
            if (this.proc == null)
                return;
            var sw = new StringWriter();
            var writer = new TextFormatter(sw);
            var fmt = new CodeFormatter(writer);
            fmt.Write(proc);
            sw.Flush();
            Clipboard.SetText(sw.ToString());
        }
    }
}
