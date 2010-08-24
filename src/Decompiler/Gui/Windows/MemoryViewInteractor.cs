/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Windows.Forms;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class MemoryViewInteractor : IWindowPane, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider sp;
        private MemoryControl ctrl;

        public Control CreateControl()
        {
            ctrl = new MemoryControl();
            Control.Font = new Font("Lucida Console", 10F);     //$TODO: make this user configurable.
            Control.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ctl_SelectionChanged);
            var uiService = (IDecompilerShellUiService)sp.GetService(typeof(IDecompilerShellUiService));
            Control.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);
            return Control;
        }

        public virtual MemoryControl Control { get { return ctrl; } }

        public void SetSite(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public void Close()
        {
        }


        public ProgramImage ProgramImage
        {
            get { return Control.ProgramImage; }
            set { Control.ProgramImage = value; }
        }

        void ctl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        public void InvalidateControl()
        {
            Control.Invalidate();
        }

        public AddressRange GetSelectedAddressRange()
        {
            return Control.GetAddressRange();
        }

        public void GotoAddress()
        {
            var uiSvc = sp.GetService<IDecompilerShellUiService>();
            using (IAddressPromptDialog dlg = CreateAddressPromptDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    Control.ShowAddress(dlg.Address);
                }
            }
        }

        //$REVIEW: consider moving this to a ICommonDialogFactoryService
        public virtual IAddressPromptDialog CreateAddressPromptDialog()
        {
            return new AddressPromptDialog();
        }

        public virtual Address SelectedAddress
        {
            get { return Control.SelectedAddress; }
            set
            {
                Control.SelectedAddress = value;
                Control.TopAddress = value;
            }
        }
        #region ICommandTarget Members

        public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ViewGoToAddress: status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
                }
            }
            return false;
        }

        public bool Execute(ref Guid cmdSet, int cmdId)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ViewGoToAddress: GotoAddress(); return true;
                }
            }
            return false;
        }

        #endregion
    }
}
