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

using Reko.Core;
using Reko.Core.Output;
using Reko.Gui.Windows.Controls;
using Reko.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows
{
    /// <summary>
    /// Pane that displays decompiled code, allows the user to navigate code,
    /// and annotate it.
    /// </summary>
    public class CodeViewerPane : IWindowPane, ICommandTarget
    {
        private CodeView codeView; 
        private IServiceProvider services;
        private Procedure proc;
        private NavigationInteractor<Procedure> navInteractor;
        private bool ignoreEvents;

        public TextView TextView { get { return codeView.TextView; } }

        #region IWindowPane Members

        public Control CreateControl()
        {
            var uiPrefsSvc = services.RequireService<IUiPreferencesService>();

            this.codeView = new CodeView();
            this.codeView.Dock = DockStyle.Fill;
            this.codeView.CurrentAddressChanged += codeView_CurrentAddressChanged;
            this.TextView.Font = new Font("Lucida Console", 10F);
            this.TextView.BackColor = SystemColors.Window;
            this.TextView.Services = services;

            this.TextView.ContextMenu = services.RequireService<IDecompilerShellUiService>().GetContextMenu(MenuIds.CtxCodeView);

            this.navInteractor = new NavigationInteractor<Procedure>();
            this.navInteractor.Attach(codeView);
            this.TextView.Navigate += textView_Navigate;
            return this.codeView;
        }

        void codeView_CurrentAddressChanged(object sender, EventArgs e)
        {
            if (ignoreEvents)
                return;
            var value = codeView.CurrentAddress;
            DisplayProcedure(value);
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

        /// <summary>
        /// Displace the procedure <paramref name="proc"/> in the code window.
        /// </summary>
        /// <param name="proc"></param>
        public void DisplayProcedure(Procedure proc)
        {
            if (codeView == null || proc == null)
                return;

            this.proc = proc;
            var tsf = new TextSpanFormatter();
            var fmt = new AbsynCodeFormatter(tsf);
            fmt.InnerFormatter.UseTabs = false;
            fmt.Write(proc);
            this.TextView.Model = tsf.GetModel();
            this.ignoreEvents = true;
            this.codeView.CurrentAddress = proc;
            ignoreEvents = false;
        }

        void textView_Navigate(object sender, EditorNavigationArgs e)
        {
            var procDst = e.Destination as Procedure;
            if (procDst == null)
                return;
            navInteractor.RememberAddress(this.proc);
            DisplayProcedure(procDst);    // ...and move to the new position.

            //$REVIEW: should this fire an event on a ISelectionService interface
            // and let interested parties track that?
            var pbSvc = services.GetService<IProjectBrowserService>();
            if (pbSvc != null)
            {
                pbSvc.SelectedObject = procDst;
            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
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
            if (cmdId.Guid == CmdSets.GuidReko)
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
