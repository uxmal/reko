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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Documents;
using Reko.Gui.Visualizers;
using Reko.Services;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// This class manages user interaction with the LowLevelView control.
    /// </summary>
    public class LowLevelViewInteractor : ILowLevelViewInteractor, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private readonly IServiceProvider services;
        private LowLevelView control;
        private TypeMarker typeMarker;
        private Program program;
        private bool ignoreAddressChange;
        private NavigationInteractor<Address> navInteractor;

        public LowLevelViewInteractor(IServiceProvider services)
        {
            this.services = services;
        }

        public LowLevelView Control { get { return control; } }
        public IWindowFrame Frame { get; set; }

        public Program Program
        {
            get { return program; }
            set
            {
                if (program == value)
                    return;
                program = value;
                OnProgramChanged(value);
            }
        }

        private void OnProgramChanged(Program value)
        {
            if (value is not null)
            {
                control.MemoryView.ImageMap = value.ImageMap;
                control.MemoryView.Procedures = value.Procedures;
                control.MemoryView.SegmentMap = value.SegmentMap;
                control.MemoryView.Architecture = value.Architecture;
                control.DisassemblyView.Program = value;
                var seg = program.SegmentMap.Segments.Values.FirstOrDefault();
                if (seg is not null)
                    return;
                control.DisassemblyView.Program = value;
                control.DisassemblyView.Model = new DisassemblyTextModel(value, seg);
                control.ImageMapView.ImageMap = value.ImageMap;
                control.ImageMapView.SegmentMap = value.SegmentMap;
                control.ImageMapView.Granularity = value.SegmentMap.GetExtent();
                control.VisualizerControl.Program = value;
            }
            return;
        }

        public virtual Address SelectedAddress
        {
            get { return control.MemoryView.SelectedAddress; }
            set
            {
                control.CurrentAddress = value;
            }
        }

        public object CreateControl()
        {
            var uiService = services.RequireService<IDecompilerShellUiService>();
            this.control = new LowLevelView();
            this.Control.Font = new Font("Lucida Console", 10F); //$TODO: use user preference
            this.Control.CurrentAddressChanged += LowLevelView_CurrentAddressChanged;

            this.Control.ImageMapView.SelectedAddressChanged += ImageMapView_SelectedAddressChanged;

            this.Control.MemoryView.SelectionChanged += MemoryView_SelectionChanged;
            uiService.SetContextMenu(this.Control.MemoryView, MenuIds.CtxMemoryControl);
            this.control.MemoryView.Services = this.services;

            this.Control.DisassemblyView.StyleClass = UiStyles.Disassembler;
            this.Control.DisassemblyView.SelectedObjectChanged += DisassemblyView_SelectedObjectChanged;
            uiService.SetContextMenu(this.Control.DisassemblyView, MenuIds.CtxDisassembler);
            this.Control.DisassemblyView.Services = this.services;
            this.Control.DisassemblyView.Navigate += DisassemblyControl_Navigate;

            this.Control.VisualizerControl.Services = services;
            PopulateVisualizers();
            this.Control.VisualizerList.SelectedIndexChanged += VisualizerList_SelectedIndexChanged;
            this.control.VisualizerList.SelectedIndex = 0;

            this.Control.ToolBarGoButton.Click += ToolBarGoButton_Click;
            this.Control.ToolBarAddressTextbox.KeyDown += ToolBarAddressTextbox_KeyDown;

            this.navInteractor = new NavigationInteractor<Address>();
            this.navInteractor.Attach(this.Control);

            typeMarker = new TypeMarker(control.MemoryView);

            return control;
        }


        public void SetSite(IServiceProvider services)
        {
        }

        public void Close()
        {
        }

        private void NavigateToToolbarAddress()
        {
            var txtAddr = Control.ToolBarAddressTextbox.Text;
            if (txtAddr.Length == 0)
                return;
            if (txtAddr[0] == 0xFEFF)
            {
                // Get rid of UTF-16 BOM Windows insists on prepending
                 txtAddr = txtAddr[1..];
            }
            txtAddr = txtAddr.Trim();
            if (txtAddr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                txtAddr = txtAddr[2..];
            if (!program.Architecture.TryParseAddress(txtAddr, out Address addr))
                return;
            UserNavigateToAddress(Control.MemoryView.TopAddress, addr);
        }

        private void PopulateVisualizers()
        {
            //$REVIEW: load the visualizers from a config file?
            this.Control.VisualizerList.Items.Add(
                new ListOption("ASCII strings", new AsciiStringVisualizer()));
            this.control.VisualizerList.Items.Add(
                new ListOption("Code and data", new CodeDataVisualizer()));
            this.Control.VisualizerList.Items.Add(
                new ListOption("Heat map", new HeatmapVisualizer()));
        }

        private void UserNavigateToAddress(Address addrFrom, Address addrTo)
        {
            if (!program.SegmentMap.IsValidAddress(addrTo))
                return;
            navInteractor.RememberAddress(addrFrom);
            control.CurrentAddress = addrTo;        // ...and move to the new position.
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (Control.MemoryView.Focused)
            {
                var addrSelStart = Control.MemoryView.SelectedAddress;
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch ((CmdIds) cmdId.ID)
                    {
                    case CmdIds.ViewGoToAddress:
                    case CmdIds.ActionMarkType:
                    case CmdIds.ViewFindWhatPointsHere:
                    case CmdIds.ActionMarkProcedure:
                    case CmdIds.TextEncodingChoose:
                        status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                        return true;
                    case CmdIds.EditCopy:
                    case CmdIds.ViewFindPattern:
                        status.Status = ValidSelection()
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                        return true;
                    case CmdIds.ActionEditSignature:
                        status.Status = IsProcedureStart(addrSelStart)
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                        return true;
                    }
                }
            }
            else if (Control.DisassemblyView.Focused)
            {
                var selAddress = Control.DisassemblyView.SelectedObject as Address;
                var instr = Control.DisassemblyView.SelectedObject as MachineInstruction;
                
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch ((CmdIds) cmdId.ID)
                    {
                    case CmdIds.EditCopy:
                        status.Status = ValidDisassemblerSelection()
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                        return true;
                    case CmdIds.OpenLink:
                    case CmdIds.OpenLinkInNewWindow:
                        status.Status = selAddress != null ? MenuStatus.Visible | MenuStatus.Enabled : 0;
                        return true;
                    case CmdIds.EditAnnotation:
                    case CmdIds.EditRegisterValues:
                        status.Status = instr != null ? MenuStatus.Visible | MenuStatus.Enabled : 0;
                        return true;
                    case CmdIds.ActionCallTerminates:
                        if (instr != null)
                        {
                            if ((instr.InstructionClass &  InstrClass.Call) != 0)
                            {
                                status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                            }
                            else
                            {
                                status.Status = MenuStatus.Visible;
                            }
                        }
                        else
                        {
                            status.Status = 0;
                        }
                        return true;
                    case CmdIds.ViewPcRelative:
                        status.Status = MenuStatus.Visible | MenuStatus.Enabled |
                            (control.DisassemblyView.ShowPcRelative ? MenuStatus.Checked : 0);
                        return true;
                    case CmdIds.TextEncodingChoose:
                        return true;
                    }
                }
            }
            return false;
        }

        public async ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            if (Control.MemoryView.Focused)
            {
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch ((CmdIds)cmdId.ID)
                    {
                    case CmdIds.EditCopy: return CopySelectionToClipboard();
                    case CmdIds.ViewGoToAddress: GotoAddress(); return true;
                    case CmdIds.ActionMarkType: return MarkType();
                    case CmdIds.ActionMarkProcedure: await MarkAndScanProcedure(); return true;
                    case CmdIds.ActionEditSignature: await EditProcedureSignature(); return true;
                    case CmdIds.ViewFindWhatPointsHere: return await ViewWhatPointsHere();
                    case CmdIds.ViewFindPattern: return await ViewFindPattern();
                    case CmdIds.TextEncodingChoose: return await ChooseTextEncoding();
                    }
                }
            }
            else if (Control.DisassemblyView.Focused)
            {
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch ((CmdIds)cmdId.ID)
                    {
                    case CmdIds.EditCopy: return CopyDisassemblerSelectionToClipboard();
                    case CmdIds.EditAnnotation: return EditDasmAnnotation();
                    case CmdIds.EditRegisterValues: return await EditRegisterValues();
                    case CmdIds.TextEncodingChoose: return await ChooseTextEncoding();
                    case CmdIds.ActionCallTerminates: return await EditCallSite();
                    case CmdIds.ViewPcRelative: return ToggleShowPcRelative();
                    }
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
            AddressRange addrRange = GetSelectedAddressRange();
            if (addrRange is null)
                return;
            var rdr = program.CreateImageReader(program.Architecture, addrRange.Begin);
            if (!rdr.TryRead(program.Platform.PointerType, out var addrDst))
                return;
            var txt = control.ToolBarAddressTextbox;
            txt.Text = addrDst.ToString();
            txt.SelectAll();
            txt.Focus();
        }

        public void InvalidateControl()
        {
            control.Invalidate();
        }

        public ValueTask MarkAndScanProcedure()
        {
            if (!TryGetSelectedAddressRange(out var addrRange))
                return ValueTask.CompletedTask;
            var address = new ProgramAddress(program, addrRange.Begin);
            return services.RequireService<ICommandFactory>().MarkProcedure(address).DoAsync();
        }

        public async ValueTask EditProcedureSignature()
        {
            var addr = this.Control.MemoryView.SelectedAddress;
            if (addr is null || program is null)
                return;
            if (!program.Procedures.TryGetValue(addr, out var proc))
                return;
            await services.RequireService<ICommandFactory>().EditSignature(
                program, proc , proc.EntryAddress).DoAsync();
            control.Invalidate();
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
                var addr = control.DisassemblyView.SelectedObject as Address;
                if (addr == null)
                    return false;
                addrRange = new AddressRange(addr, addr);
                return true;
            }
            return true;
        }

        /// <summary>
        /// Copies the selected range of bytes into the clipboard.
        /// </summary>
        /// <returns></returns>
        private bool CopySelectionToClipboard()
        {
            if (!TryGetSelectedAddressRange(out var range))
                return true;
            if (control.MemoryView.Focused)
            {
                var decompiler = services.GetService<IDecompilerService>().Decompiler;
                var dumper = new Dumper(decompiler.Project.Programs.First());
                var sb = new StringWriter();
                dumper.DumpData(control.MemoryView.SegmentMap, program.Architecture, range, new TextFormatter(sb));
                var text = sb.ToString();
                if (text.Length > 0)
                {
                    Clipboard.SetText(text);       //$TODO: abstract this.
                }
            }
            return true;
        }

        private bool CopyDisassemblerSelectionToClipboard()
        {
            var ms = new MemoryStream();
            control.DisassemblyView.Selection.Save(ms, DataFormats.UnicodeText);
            var text = new string(Encoding.Unicode.GetChars(ms.ToArray()));
            Clipboard.SetData(DataFormats.UnicodeText, text);
            return true;
        }

        public bool MarkType()
        {
            var addrRange = control.MemoryView.GetAddressRange();
            if (addrRange.IsValid)
            {
                typeMarker.Show(
                    control.MemoryView.AddressToPoint(addrRange.Begin),
                    typeMarker_TextAccepted);
            }
            return true;
        }

        private void typeMarker_TextAccepted(string text)
        {
            var item = SetTypeAtAddressRange(GetSelectedAddressRange().Begin, text);
            if (item is null)
                return;
            // Advance selection to beyond item.
            this.SelectedAddress = item.Address + item.Size;

            // Github #1219: need to reestablish focus after TypeMarker window is closed.
            this.Control.MemoryView.Focus();
        }

        private bool IsProcedureStart(Address addr)
        {
            return this.program.Procedures.ContainsKey(addr);
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

        private bool ValidDisassemblerSelection()
        { 
            return !control.DisassemblyView.Selection.IsEmpty;
        }

        public ImageMapItem SetTypeAtAddressRange(Address address, string userText)
        {
            var dataType = HungarianParser.Parse(userText);
            if (dataType == null)
                return null;
            if (dataType is ArrayType arr && arr.ElementType.Size != 0)
            {
                var range = control.MemoryView.GetAddressRange();
                if (range.IsValid)
                {
                    long size = (range.End - range.Begin) + 1;
                    int nElems = (int)(size / arr.ElementType.Size);
                    arr.Length = nElems;
                }
            }
            var arch = program.Architecture;
            var item = program.AddUserGlobalItem(arch, address, dataType);
            control.MemoryView.Invalidate();
            return item;
        }

        public async ValueTask<bool> ViewWhatPointsHere()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid)
                return true;
            if (program == null)
                return true;
            await services.RequireService<ICommandFactory>().ViewWhatPointsHere(program, addrRange.Begin).DoAsync();
            return true;
        }

        public async ValueTask<bool> ViewFindPattern()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid || program == null)
                return true;
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var srSvc = services.RequireService<ISearchResultService>();

            using (ISearchDialog dlg = dlgFactory.CreateSearchDialog())
            {
                dlg.InitialPattern = SelectionToHex(addrRange);
                if (await uiSvc.ShowModalDialog(dlg) == Gui.Services.DialogResult.OK)
                {
                    var re = Core.Dfa.Automaton.CreateFromPattern(dlg.Patterns.Text);
                    var hits =
                        //$BUG: wrong result
                        program.SegmentMap.Segments.Values
                        .SelectMany(s => GetMatches(s, re))
                        .Select(offset => new AddressSearchHit(
                            program,
                            program.ImageMap.BaseAddress + offset,
                            1));
                    srSvc.ShowAddressSearchResults(hits, new CodeSearchDetails());
                }
            }
            return true;
        }

        private IEnumerable<int> GetMatches(ImageSegment s, Core.Dfa.Automaton re)
        {
            var mem = (ByteMemoryArea) s.MemoryArea;
            return re.GetMatches(mem.Bytes, 0);
        }

        public async ValueTask<bool> ChooseTextEncoding()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            using (ITextEncodingDialog dlg = dlgFactory.CreateTextEncodingDialog())
            {
                if (await uiSvc.ShowModalDialog(dlg) == Gui.Services.DialogResult.OK)
                {
                    var enc = dlg.GetSelectedTextEncoding();
                    this.control.MemoryView.Encoding = enc;
                    program.User.TextEncoding = enc;
                    this.control.DisassemblyView.RecomputeLayout();
                }
            }
            return true;
        }

        public bool ToggleShowPcRelative()
        {
            var show = control.DisassemblyView.ShowPcRelative;
            control.DisassemblyView.ShowPcRelative = !show;
            control.DisassemblyView.RecomputeLayout();
            return true;
        }

        public bool EditDasmAnnotation()
        {
            return true;
        }

        public async ValueTask<bool> EditRegisterValues()
        {
            if (Control.DisassemblyView.SelectedObject is MachineInstruction instr)
            {
                if (!program.User.RegisterValues.TryGetValue(instr.Address, out var regValues))
                {
                    regValues = new List<UserRegisterValue>();
                }
                var dlgFactory = services.RequireService<IDialogFactory>();
                var uiSvc = services.RequireService<IDecompilerShellUiService>();
                using (var dlg = dlgFactory.CreateRegisterValuesDialog(this.program.Architecture, regValues))
                {
                    if (Gui.Services.DialogResult.OK == await uiSvc.ShowModalDialog(dlg))
                    {
                        regValues = dlg.RegisterValues;
                        program.User.RegisterValues[instr.Address] = regValues;
                    }
                }
            }
            return true;
        }

        public async ValueTask<bool> EditCallSite()
        {
            var instr = (MachineInstruction)Control.DisassemblyView.SelectedObject;
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var ucd = GetUserCallDataFromAddress(instr.Address);
            using (var dlg = dlgFactory.CreateCallSiteDialog(this.program, ucd))
            {
                if (Gui.Services.DialogResult.OK == await uiSvc.ShowModalDialog(dlg))
                {
                    ucd = dlg.GetUserCallData(null);
                    SetUserCallData(ucd);
                }
            }
            return true;
        }

        private UserCallData GetUserCallDataFromAddress(Address addr)
        {
            if (!program.User.Calls.TryGetValue(addr, out UserCallData ucd))
            {
                ucd = new UserCallData { Address = addr };
            }
            return ucd;
        }

        private void SetUserCallData(UserCallData ucd)
        {
            program.User.Calls[ucd.Address] = ucd;
        }

        private string SelectionToHex(AddressRange addr)
        {
            var sb = new StringBuilder();
            var rdr = program.CreateImageReader(program.Architecture, addr.Begin);
            var sep = "";
            while (rdr.Address <= addr.End)
            {
                sb.Append(sep);
                sep = " ";
                sb.AppendFormat("{0:X2}", (uint)rdr.ReadByte());
            }
            return sb.ToString();
        }

        void ImageMapView_SelectedAddressChanged(object sender, EventArgs e)
        {
            if (ignoreAddressChange)
                return;
            var addr = Control.ImageMapView.SelectedAddress; 
            this.ignoreAddressChange = true;
            this.Control.MemoryView.SelectedAddress = addr;
            this.Control.MemoryView.TopAddress = addr;

            if (program.SegmentMap.TryFindSegment(addr, out ImageSegment seg))
            {
                this.Control.DisassemblyView.Model = new DisassemblyTextModel(program, seg);
                this.Control.DisassemblyView.SelectedObject = addr;
                this.control.DisassemblyView.TopAddress = addr;
            }
            this.SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new AddressRange(addr, addr)));
            UserNavigateToAddress(Control.MemoryView.TopAddress, addr);
            this.ignoreAddressChange = false;
        }

        private void MemoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreAddressChange)
                return;
            this.ignoreAddressChange = true;
            this.Control.DisassemblyView.SelectedObject = e.AddressRange.Begin;
            this.Control.DisassemblyView.TopAddress = e.AddressRange.Begin;
            this.SelectionChanged?.Invoke(this, e);
            this.ignoreAddressChange = false;
        }

        void DisassemblyView_SelectedObjectChanged(object sender, EventArgs e)
        {
            var selectedAddr = Control.DisassemblyView.SelectedObject as Address;
            if (ignoreAddressChange || selectedAddr == null)
                return;
            this.ignoreAddressChange = true;
            this.Control.MemoryView.SelectedAddress = selectedAddr;
            this.Control.MemoryView.TopAddress = selectedAddr;
            this.ignoreAddressChange = false;
        }

        void LowLevelView_CurrentAddressChanged(object sender, EventArgs e)
        {
            ignoreAddressChange = true;
            var value = Control.CurrentAddress;
            if (value != null)
            {
                var addrTop = value - ((int)value.ToLinear() & 0x0F);
                control.MemoryView.SelectedAddress = value;
                control.MemoryView.TopAddress = addrTop;
                control.DisassemblyView.TopAddress = value;
            }
            ignoreAddressChange = false;
        }

        void ToolBarAddressTextbox_KeyDown(object sender, Gui.Controls.KeyEventArgs e)
        {
            if (e.KeyData != Gui.Controls.Keys.Return)
                return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            NavigateToToolbarAddress();
        }

        void ToolBarGoButton_Click(object sender, EventArgs e)
        {
            if (ignoreAddressChange)
                return;
            NavigateToToolbarAddress();
        }

        private void DisassemblyControl_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address;
            if (addr == null)
                return;
            UserNavigateToAddress(Control.DisassemblyView.TopAddress, addr);
        }

        private void VisualizerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (ListOption) this.control.VisualizerList.SelectedItem;
            if (item == null)
                return;
            this.Control.VisualizerControl.Visualizer = (Visualizer)item.Value;
        }

    }
}
