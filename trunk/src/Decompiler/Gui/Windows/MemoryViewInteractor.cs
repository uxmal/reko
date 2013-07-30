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
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows.Forms;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class MemoryViewInteractor : IWindowPane, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider services;
        private MemoryControl ctrl;
        private TypeMarker typeMarker;

        public Control CreateControl()
        {
            ctrl = new MemoryControl();
            Control.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ctl_SelectionChanged);
            Control.Font = new Font("Lucida Console", 10F);     //$TODO: make this user configurable.
            var uiService = services.RequireService<IDecompilerShellUiService>();
            Control.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);
            typeMarker = new TypeMarker(Control);
            typeMarker.TextChanged += FormatType;
            typeMarker.TextAccepted += SetTypeAtAddressRange;

            return Control;
        }

        public virtual MemoryControl Control { get { return ctrl; } }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }

        public void Close()
        {
        }

        public LoadedImage ProgramImage
        {
            get { return Control.ProgramImage; }
            set { Control.ProgramImage = value; }
        }

        public ImageMap ImageMap
        {
            get { return Control.ImageMap; }
            set { Control.ImageMap = value; }
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
            Debug.Print("GotoAddress invoked");
            var uiSvc = services.GetService<IDecompilerShellUiService>();
            var dlgSvc = services.RequireService<IDialogFactory>();
            using (IAddressPromptDialog dlg = dlgSvc.CreateAddressPromptDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    Control.ShowAddress(dlg.Address);
                }
            }
        }

        public void MarkType()
        {
            var addrRange = Control.GetAddressRange();
            if (!addrRange.IsValid)
                return;
            typeMarker.Show(Control.AddressToPoint(addrRange.Begin));
        }

        public void FormatType(object sender, TypeMarkerEventArgs e)
        {
            try
            {
                var parser = new HungarianParser();
                var dataType = parser.Parse(e.UserText);
                if (dataType == null)
                    e.FormattedType = " - Null - ";
                else 
                    e.FormattedType = dataType.ToString();
            }
            catch
            {
                e.FormattedType = " - Error - ";
            }
        }

        private void SetTypeAtAddressRange(object sender, TypeMarkerEventArgs e)
        {
            SetTypeAtAddressRange(GetSelectedAddressRange().Begin, e.UserText);
        }

        public void SetTypeAtAddressRange(Address address, string userText)
        {
           var parser = new HungarianParser();
           var dataType = parser.Parse(userText);
           if (dataType == null)
               return;
            ImageMap.AddItem(address, new ImageMapItem
            {
                Size = (uint) dataType.Size,
                DataType = dataType,
            });
            Control.Invalidate();
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
                case CmdIds.ViewGoToAddress:
                case CmdIds.ActionMarkType:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
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
                case CmdIds.ActionMarkType: MarkType(); return true;
                case CmdIds.ActionMarkProcedure: MarkAndScanProcedure(); return true;
                }
            }
            return false;
        }

        public void MarkAndScanProcedure()
        {
            AddressRange addrRange = Control.GetAddressRange();
            if (addrRange.IsValid)
            {
                var decompiler = services.GetService<IDecompilerService>().Decompiler;
                var proc = decompiler.ScanProcedure(addrRange.Begin);
                var userp = new Decompiler.Core.Serialization.SerializedProcedure
                {
                    Address = addrRange.Begin.ToString(),
                    Name = proc.Name,
                };
                decompiler.Project.UserProcedures.Add(addrRange.Begin, userp);
                Control.Invalidate();
            }
        }
        #endregion
    }
}
