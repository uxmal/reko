#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows.Controls;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class MemoryViewInteractor : IWindowPane, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider services;
        private LowLevelView control;
        private IProcessorArchitecture arch;
        private TypeMarker typeMarker;
        private LoadedImage image;
        private ImageMap imageMap;

        public virtual LowLevelView Control { get { return control; } }

        public IProcessorArchitecture Architecture
        {
            get { return arch; }
            set
            {
                arch = value;
                control.DisassemblyView.Architecture = value;
            }
        }

        public LoadedImage ProgramImage
        {
            get { return image; }
            set
            {
                image = value;
                control.MemoryView.ProgramImage = value;
                control.DisassemblyView.Image = value;
                control.DisassemblyView.StartAddress = value.BaseAddress;
            }
        }

        public ImageMap ImageMap
        {
            get { return imageMap; }
            set
            {
                imageMap = value;
                control.MemoryView.ImageMap = value;
                //control.DisassemblyView.ImageMap = value;
            }
        }

        public virtual Address SelectedAddress
        {
            get { return control.MemoryView.SelectedAddress; }
            set
            {
                control.MemoryView.SelectedAddress = value;
                control.MemoryView.TopAddress = value;
                control.DisassemblyView.SelectedAddress = value;
                control.DisassemblyView.TopAddress = value;
            }
        }

        public Control CreateControl()
        {
            var uiService = services.RequireService<IDecompilerShellUiService>();
            this.control = new LowLevelView();
            this.control.MemoryView.SelectionChanged += MemoryView_SelectionChanged;
            this.Control.Font = new Font("Lucida Console", 10F);     //$TODO: make this user configurable.
            this.Control.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);

            typeMarker = new TypeMarker(control.MemoryView);
            typeMarker.TextChanged += FormatType;
            typeMarker.TextAccepted += (sender, e) => { SetTypeAtAddressRange(GetSelectedAddressRange().Begin, e.UserText); };

            return control;
        }

        public void SetSite(IServiceProvider sp)
        {
            services = sp;
        }

        public void Close()
        {
        }

        public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ViewGoToAddress:
                case CmdIds.ActionMarkType:
                case CmdIds.ViewFindWhatPointsHere:
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
                case CmdIds.ViewFindWhatPointsHere: return ViewWhatPointsHere();
                }
            }
            return false;
        }

        public AddressRange GetSelectedAddressRange()
        {
            return control.MemoryView.GetAddressRange();
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
                    control.MemoryView.ShowAddress(dlg.Address);
                }
            }
        }

        public void InvalidateControl()
        {
            control.Invalidate();
        }

        public void MarkAndScanProcedure()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (addrRange.IsValid)
            {
                var decompiler = services.GetService<IDecompilerService>().Decompiler;
                var proc = decompiler.ScanProcedure(addrRange.Begin);
                var userp = new Decompiler.Core.Serialization.SerializedProcedure
                {
                    Address = addrRange.Begin.ToString(),
                    Name = proc.Name,
                };
                decompiler.Project.InputFiles[0].UserProcedures.Add(addrRange.Begin, userp);
                control.MemoryView.Invalidate();
            }
        }

        public void MarkType()
        {
            var addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid)
                return;
            typeMarker.Show(control.MemoryView.AddressToPoint(addrRange.Begin));
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
            control.MemoryView.Invalidate();
        }

        public bool ViewWhatPointsHere()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid)
                return true;
            var decompiler = services.GetService<IDecompilerService>().Decompiler;
            if (decompiler == null)
                return true;
            var resultSvc = services.GetService<ISearchResultService>();
            if (resultSvc == null)
                return true;

            var arch = decompiler.Program.Architecture;
            var image = decompiler.Program.Image;
            var rdr = decompiler.Program.Image.CreateReader(0);
            var addresses = arch.CreateCallInstructionScanner(
                rdr,
                new HashSet<uint> { addrRange.Begin.Linear },
                InstructionScannerFlags.CallsAndJumps);
            resultSvc.ShowSearchResults(new AddressSearchResult(services, image, decompiler.Program.ImageMap, addresses));
            return true;
        }

        private void MemoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Control.DisassemblyView.SelectedAddress = e.AddressRange.Begin;
            this.Control.DisassemblyView.TopAddress = e.AddressRange.Begin;
            this.SelectionChanged.Fire(this, e);
        }
    }
}
