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
using Decompiler.Core.Types;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class LowLevelViewInteractor : IWindowPane, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider services;
        private LowLevelView control;
        private TypeMarker typeMarker;
        private Program program;
        private bool ignoreAddressChange;

        public LowLevelView Control { get { return control; } }

        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                if (value != null)
                {
                    control.MemoryView.ProgramImage = value.Image;
                    control.MemoryView.ImageMap = value.ImageMap;
                    control.MemoryView.Architecture = value.Architecture;
                    control.DisassemblyView.Image = value.Image;
                    control.DisassemblyView.StartAddress = value.Image.BaseAddress;
                    control.DisassemblyView.Architecture = value.Architecture;
                }
            }
        }

        public virtual Address SelectedAddress
        {
            get { return control.MemoryView.SelectedAddress; }
            set
            {
                ignoreAddressChange = true;
                var addrTop = value - ((int)value.Linear & 0x0F);
                control.MemoryView.SelectedAddress = value;
                control.MemoryView.TopAddress = addrTop;
                control.DisassemblyView.SelectedAddress = value;
                control.DisassemblyView.TopAddress = value;
                ignoreAddressChange = false;
            }
        }

        public Control CreateControl()
        {
            var uiService = services.RequireService<IDecompilerShellUiService>();
            var uiPrefsSvc = services.RequireService<IUiPreferencesService>();
            this.control = new LowLevelView();
            this.Control.MemoryView.SelectionChanged += MemoryView_SelectionChanged;
            this.Control.Font = uiPrefsSvc.DisassemblyFont ?? new Font("Lucida Console", 10F);
            this.Control.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);
            this.Control.ToolBarGoButton.Click += ToolBarGoButton_Click;
            this.Control.ToolBarAddressTextbox.KeyDown += ToolBarAddressTextbox_KeyDown;
            this.control.MemoryView.Services = this.services;

            typeMarker = new TypeMarker(control.MemoryView);
            typeMarker.TextChanged += typeMarker_FormatType;
            typeMarker.TextAccepted += typeMarker_TextAccepted;

            return control;
        }

        private void typeMarker_TextAccepted(object sender, TypeMarkerEventArgs e)
        {
            var item = SetTypeAtAddressRange(GetSelectedAddressRange().Begin, e.UserText);
            if (item == null)
                return;
            // Advance selection to beyond item.
            this.SelectedAddress = item.Address + item.Size;
        }

        public void SetSite(IServiceProvider sp)
        {
            services = sp;
        }

        public void Close()
        {
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewGoToAddress:
                case CmdIds.ActionMarkType:
                case CmdIds.ViewFindWhatPointsHere:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
                case CmdIds.EditCopy:
                    status.Status = ValidSelection()
                        ? MenuStatus.Visible | MenuStatus.Enabled
                        : MenuStatus.Visible;
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
                case CmdIds.EditCopy: return CopySelection();
                case CmdIds.ViewGoToAddress: GotoAddress(); return true;
                case CmdIds.ActionMarkType: return MarkType();
                case CmdIds.ActionMarkProcedure: MarkAndScanProcedure(); return true;
                case CmdIds.ViewFindWhatPointsHere: return ViewWhatPointsHere();
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the range of address that are selected in the memory view.
        /// </summary>
        /// <returns></returns>
        public virtual AddressRange GetSelectedAddressRange()
        {
            return control.MemoryView.GetAddressRange();
        }

        public void GotoAddress()
        {
            var addrRange = GetSelectedAddressRange();
            if (addrRange == null)
                return;
            var arch = program.Architecture;
            var rdr = arch.CreateImageReader(program.Image, addrRange.Begin);
            var addrDst = rdr.Read(arch.PointerType);
            var txt = control.ToolBarAddressTextbox;
            txt.Text = addrDst.ToString();
            txt.SelectAll();
            txt.Focus();
        }

        public void InvalidateControl()
        {
            control.Invalidate();
        }

        public void MarkAndScanProcedure()
        {
            AddressRange addrRange;
            if (!TryGetSelectedAddressRange(out addrRange))
                return;

            try
            {
                var address = addrRange.Begin;
                MarkAndScanProcedure(address);
            }
            catch (Exception ex)
            {
                services.RequireService<IDecompilerShellUiService>().ShowError(ex, "An error happened while scanning the procedure.");
            }
            control.MemoryView.Invalidate();
            control.DisassemblyView.Invalidate();
        }

        private void MarkAndScanProcedure(Address address)
        {
            var decompiler = services.GetService<IDecompilerService>().Decompiler;
            var proc = decompiler.ScanProcedure(program, address);
            var userp = new Decompiler.Core.Serialization.Procedure_v1
            {
                Address = address.ToString(),
                Name = proc.Name,
            };
            //$REVIEW: need to know what InputFile is in play.
            var inputFile = (InputFile)decompiler.Project.InputFiles[0];
            var ups = inputFile.UserProcedures;
            if (!ups.ContainsKey(address))
            {
                ups.Add(address, userp);
            }
        }

        private bool TryGetSelectedAddressRange(out AddressRange addrRange)
        {
            addrRange = null;
            if (control.MemoryView.Focused)
            {
                addrRange = control.MemoryView.GetAddressRange();
                if (!addrRange.IsValid)
                    return false;
            }
            else if (control.DisassemblyView.Focused)
            {
                var addr = control.DisassemblyView.SelectedAddress;
                if (addr == null)
                    return false;
                addrRange = new AddressRange(addr, addr);
                return true;
            }
            return true;
        }

        private bool CopySelection()
        {
            AddressRange range;
            if (!TryGetSelectedAddressRange(out range))
                return true;
            if (control.MemoryView.Focused)
            {
                 var decompiler = services.GetService<IDecompilerService>().Decompiler;
                 var dumper = new Dumper(decompiler.Programs.First().Architecture);
                var sb = new StringWriter();
                dumper.DumpData(control.MemoryView.ProgramImage, range, sb);
                Clipboard.SetText(sb.ToString());       //$TODO: abstract this.
            }
            return true;
        }
        public bool MarkType()
        {
            var addrRange = control.MemoryView.GetAddressRange();
            if (addrRange.IsValid)
            {
                typeMarker.Show(control.MemoryView.AddressToPoint(addrRange.Begin));
            }
            return true;
        }

        private bool ValidSelection()
        {
            if (control.MemoryView.Focused)
            {
                var addrRange = control.MemoryView.GetAddressRange();
                if (addrRange.IsValid)
                    return true;
            }
            return false;
        }
        public void typeMarker_FormatType(object sender, TypeMarkerEventArgs e)
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

        public ImageMapItem SetTypeAtAddressRange(Address address, string userText)
        {
            var parser = new HungarianParser();
            var dataType = parser.Parse(userText);
            if (dataType == null)
                return null;

            var item = program.AddUserGlobalItem(address, dataType);
            control.MemoryView.Invalidate();
            return item;
        }

        
        public bool ViewWhatPointsHere()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid)
                return true;
            if (program == null)
                return true;
            var resultSvc = services.GetService<ISearchResultService>();
            if (resultSvc == null)
                return true;

            var arch = program.Architecture;
            var image = program.Image;
            var rdr = program.Architecture.CreateImageReader(program.Image, 0);
            var addrControl = arch.CreatePointerScanner(
                rdr,
                new HashSet<uint> { addrRange.Begin.Linear },
                PointerScannerFlags.All);
            resultSvc.ShowSearchResults(new AddressSearchResult(services, addrControl.Select(lin => new AddressSearchHit(program, lin))));
            return true;
        }

        private void MemoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreAddressChange)
                return;
            this.ignoreAddressChange = true;
            this.Control.DisassemblyView.SelectedAddress = e.AddressRange.Begin;
            this.Control.DisassemblyView.TopAddress = e.AddressRange.Begin;
            this.SelectionChanged.Fire(this, e);
            this.ignoreAddressChange = false;
        }

        void ToolBarAddressTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Return)
                return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            GotoToolbarAddress();
        }

        void ToolBarGoButton_Click(object sender, EventArgs e)
        {
            if (ignoreAddressChange)
                return;
            GotoToolbarAddress();
        }

        private void GotoToolbarAddress()
        {
            Address addr;
            var txtAddr = Control.ToolBarAddressTextbox.Text.Trim();
            if (txtAddr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                txtAddr = txtAddr.Substring(2);
            if (!Address.TryParse(txtAddr, 16, out addr))
                return;
            if (!program.Image.IsValidAddress(addr))
                return;
            this.ignoreAddressChange = true;
            this.Control.MemoryView.SelectedAddress = addr;
            this.Control.MemoryView.TopAddress = addr;
            this.Control.DisassemblyView.SelectedAddress = addr;
            this.Control.DisassemblyView.TopAddress = addr;
            this.ignoreAddressChange = false;
        }
    }
}
