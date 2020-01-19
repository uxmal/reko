#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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

using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms.Controls;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class CombinedCodeViewInteractor : IWindowPane, ICommandTarget
    {
        private IServiceProvider services;
        private Program program;
        private Procedure proc;
        private CombinedCodeView combinedCodeView;
        private NavigationInteractor<Address> navInteractor;

        private SortedList<Address, MixedCodeDataModel.DataItemNode> nodeByAddress;
        private NestedTextModel nestedTextModel;
        private GViewer gViewer;

        private DeclarationFormInteractor declarationFormInteractor;
        private CommentFormInteractor commentFormInteractor;

        private ImageSegment segment;
        private bool showProcedures;

        private PreviewInteractor previewInteractor;

        public CombinedCodeViewInteractor()
        {
        }

        public IWindowFrame Frame { get; set; }

        public virtual Address SelectedAddress
        {
            get { return combinedCodeView.CurrentAddress; }
            set { combinedCodeView.CurrentAddress = value; }
        }

        public void DisplayStatement(Program program, Statement stm)
        {
            this.program = program;
            this.proc = stm.Block.Procedure;
            this.showProcedures = true;
            ProgramChanged();
            if (program != null)
            {
                var addr = program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress);
                SelectedAddress = addr;
            }
        }

        public void DisplayProcedure(Program program, Procedure proc)
        {
            this.program = program;
            this.proc = proc;
            this.showProcedures = true;
            ProgramChanged();
            if (program != null)
            {
                var addr = proc.EntryAddress;
                if (addr == null)
                {
                    addr = program.SegmentMap.Segments.Values
                        .Where(s => s.MemoryArea != null)
                        .Select(s => Address.Max(s.Address, s.MemoryArea.BaseAddress))
                        .FirstOrDefault();
                    if (addr == null)
                    {
                        addr = program.ImageMap.BaseAddress;
                    }
                }
                SelectedAddress = addr;
            }
        }

        public void DisplayGlobals(Program program, ImageSegment segment)
        {
            this.program = program;
            this.segment = segment;
            this.showProcedures = false;
            ProgramChanged();
            SelectedAddress = segment.Address;
        }

        private void ProgramChanged()
        {
            if (combinedCodeView == null)
                return;

            combinedCodeView.MixedCodeDataView.Program = program;
        }

        private void MixedCodeDataView_ModelChanged(object sender, EventArgs e)
        {
            if (combinedCodeView == null)
                return;

            CreateNestedTextModel();

            MixedCodeDataView_TopAddressChanged();
        }

        private void CreateNestedTextModel()
        {
            this.nestedTextModel = new NestedTextModel();

            var mixedCodeDataModel = combinedCodeView.MixedCodeDataView.Model as MixedCodeDataModel;
            if (mixedCodeDataModel == null)
                return;

            var dataItemNodes = mixedCodeDataModel.GetDataItemNodes();

            this.nodeByAddress = new SortedList<Address, MixedCodeDataModel.DataItemNode>();
            foreach (var dataItemNode in dataItemNodes)
            {
                var curAddr = dataItemNode.StartAddress;

                bool nodeCreated = false;
                ImageMapItem item;
                Procedure proc = dataItemNode.Proc;
                if (ShowItem(dataItemNode))
                {
                    if (proc != null)
                    {
                        var model = new ProcedureCodeModel(proc);
                        //$TODO: make spacing between globals / procedures user adjustable
                        model.NumEmptyLinesAfter = 2;
                        nestedTextModel.Nodes.Add(model);
                        nodeCreated = true;
                    }
                    else if (program.ImageMap.TryFindItem(curAddr, out item) &&
                              item.DataType != null &&
                            !(item.DataType is UnknownType))
                    {
                        var dt = item.DataType;
                        var name = item.Name ?? "<unnamed>";

                        var tsf = new TextSpanFormatter();
                        var fmt = new AbsynCodeFormatter(tsf);
                        fmt.InnerFormatter.UseTabs = false;
                        var gdw = new GlobalDataWriter(program, services);
                        gdw.WriteGlobalVariable(curAddr, dt, name, tsf);
                        //$TODO: make spacing between globals / procedures user adjustable
                        tsf.WriteLine("");
                        nestedTextModel.Nodes.Add(tsf.GetModel());
                        nodeCreated = true;
                    }
                }

                if (nodeCreated)
                {
                    dataItemNode.ModelNode = nestedTextModel.Nodes.Last();
                    this.nodeByAddress[curAddr] = dataItemNode;
                }
            }

            combinedCodeView.CodeView.Model = nestedTextModel;
        }

        private bool ShowItem(MixedCodeDataModel.DataItemNode item)
        {
            if (!showProcedures && item.Proc != null)
                return false;

            if (segment != null && !segment.IsInRange(item.StartAddress))
                return false;

            return true;
        }

        private bool ShowAllItems()
        {
            return (segment == null && showProcedures);
        }

        public object CreateControl()
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();

            this.combinedCodeView = new CombinedCodeView();
            this.combinedCodeView.Dock = DockStyle.Fill;

            this.combinedCodeView.CurrentAddressChanged += CombinedCodeView_CurrentAddressChanged;

            this.combinedCodeView.MixedCodeDataView.VScrollValueChanged += MixedCodeDataView_VScrollValueChanged;
            this.combinedCodeView.MixedCodeDataView.Services = services;
            this.combinedCodeView.MixedCodeDataView.MouseDown += MixedCodeDataView_MouseDown;
            this.combinedCodeView.MixedCodeDataView.ModelChanged += MixedCodeDataView_ModelChanged;
            this.combinedCodeView.MixedCodeDataView.Navigate += TextView_Navigate;

            this.combinedCodeView.CodeView.VScrollValueChanged += CodeView_VScrollValueChanged;
            this.combinedCodeView.CodeView.Services = services;
            this.combinedCodeView.CodeView.MouseDown += CodeView_MouseDown;
            this.combinedCodeView.CodeView.Navigate += TextView_Navigate;

            uiSvc.SetContextMenu(this.combinedCodeView, MenuIds.CtxCodeView);

            this.combinedCodeView.ToolBarGoButton.Click += ToolBarGoButton_Click;
            this.combinedCodeView.ToolBarAddressTextbox.KeyDown += ToolBarAddressTextbox_KeyDown;

            this.gViewer = new GViewer();
            this.gViewer.Dock = DockStyle.Fill;
            this.gViewer.Visible = false;
            this.gViewer.PanButtonPressed = true;
            this.gViewer.ToolBarIsVisible = true;
            this.gViewer.KeyDown += GViewer_KeyDown;
            uiSvc.SetContextMenu(this.gViewer, MenuIds.CtxCodeView);
            this.gViewer.LayoutAlgorithmSettingsButtonVisible = false;
            this.gViewer.LayoutEditingEnabled = false;
            this.gViewer.EdgeInsertButtonVisible = false;
            this.gViewer.SaveButtonVisible = false;
            this.gViewer.SaveGraphButtonVisible = false;
            this.gViewer.SaveAsMsaglEnabled = false;
            this.gViewer.UndoRedoButtonsVisible = false;
            this.gViewer.KeyDown += GViewer_KeyDown;
            uiSvc.SetContextMenu(this.gViewer, MenuIds.CtxCodeView);
            this.gViewer.MouseUp += GViewer_MouseUp;
            this.gViewer.DrawingPanel.MouseUp += GViewer_MouseUp;
            var iViewer = (IViewer)gViewer;
            iViewer.MouseUp += IViewer_MouseUp;
            iViewer.MouseDown += IViewer_MouseDown;

            this.navInteractor = new NavigationInteractor<Address>();
            this.navInteractor.Attach(this.combinedCodeView);

            declarationFormInteractor = new DeclarationFormInteractor(services);
            commentFormInteractor = new CommentFormInteractor(services);
            previewInteractor = new PreviewInteractor(
                services, 
                this.program,
                this.combinedCodeView.PreviewTimer,
                this.combinedCodeView.MixedCodeDataView);

            return combinedCodeView;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
            program = null;
            ProgramChanged();
            if (combinedCodeView != null)
                combinedCodeView.Dispose();
            combinedCodeView = null;
        }

        private TextView FocusedTextView {
            get
            {
                if (combinedCodeView.MixedCodeDataView.Focused)
                    return combinedCodeView.MixedCodeDataView;

                if (combinedCodeView.CodeView.Focused)
                    return combinedCodeView.CodeView;

                return null;
            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                if (!ShowAllItems())
                {
                    switch (cmdId.ID)
                    {
                    case CmdIds.EditDeclaration:
                    case CmdIds.EditComment:
                    case CmdIds.ViewCfgGraph:
                        status.Status = MenuStatus.Visible;
                        return true;
                    }
                }
                switch (cmdId.ID)
                {
                case CmdIds.TextEncodingChoose:
                    status.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.EditCopy:
                    status.Status = FocusedTextView == null || FocusedTextView.Selection.IsEmpty
                        ? MenuStatus.Visible
                        : MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                case CmdIds.ViewCfgGraph:
                    status.Status = gViewer.Visible
                        ? MenuStatus.Visible | MenuStatus.Enabled | MenuStatus.Checked
                        : MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                case CmdIds.ViewCfgCode:
                    status.Status = gViewer.Visible
                        ? MenuStatus.Visible | MenuStatus.Enabled
                        : MenuStatus.Visible | MenuStatus.Enabled | MenuStatus.Checked;
                    return true;
                case CmdIds.EditDeclaration:
                case CmdIds.EditComment:
                    status.Status = GetAnchorAddress() == null 
                        ? MenuStatus.Visible
                        : MenuStatus.Enabled | MenuStatus.Visible;
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
                    Copy();
                    return true;
                case CmdIds.ViewCfgGraph:
                    ViewGraph();
                    return true;
                case CmdIds.ViewCfgCode:
                    ViewCode();
                    return true;
                case CmdIds.TextEncodingChoose:
                    return ChooseTextEncoding();
                case CmdIds.EditDeclaration:
                    EditDeclaration();
                    return true;
                case CmdIds.EditComment:
                    EditComment();
                    return true;
                }
            }
            return false;
        }

        public void Copy()
        {
            if (this.proc == null)
                return;

            if (FocusedTextView == null)
                return;

            var ms = new MemoryStream();
            FocusedTextView.Selection.Save(ms, DataFormats.UnicodeText);
            var text = new string(Encoding.Unicode.GetChars(ms.ToArray()));
            Clipboard.SetData(DataFormats.UnicodeText, text);
        }

        public bool ChooseTextEncoding()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            using (ITextEncodingDialog dlg = dlgFactory.CreateTextEncodingDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) == Gui.DialogResult.OK)
                {
                    var enc = dlg.GetSelectedTextEncoding();
                    program.User.TextEncoding = enc;
                    this.combinedCodeView.MixedCodeDataView.RecomputeLayout();
                    this.combinedCodeView.CodeView.RecomputeLayout();
                }
            }
            return true;
        }

        private Address CodeView_GetAnchorAddress()
        {
            var pt = combinedCodeView.CodeView.GetAnchorMiddlePoint();
            var tag = combinedCodeView.CodeView.GetTagFromPoint(pt);
            var addr = tag as Address;
            var proc = tag as Procedure;

            if (proc != null)
                addr = proc.EntryAddress;

            return addr;
        }

        private Address MixedCodeDataView_GetAnchorAddress()
        {
            var addr = combinedCodeView.MixedCodeDataView.GetAnchorAddress();
            ImageMapItem item;
            if (program.ImageMap.TryFindItem(addr, out item))
            {
                var blockItem = item as ImageMapBlock;
                if (blockItem != null)
                {
                    addr = blockItem.Block.Procedure.EntryAddress;
                }
                else if (!(item.DataType is UnknownType))
                {
                    addr = item.Address;
                }
            }
            return addr;
        }

        private Address GetAnchorAddress()
        {
            if (combinedCodeView.CodeView.Focused)
                return CodeView_GetAnchorAddress();
            if (combinedCodeView.MixedCodeDataView.Focused)
                return MixedCodeDataView_GetAnchorAddress();
            return null;
        }

        private void EditDeclaration()
        {
            var addr = GetAnchorAddress();
            if (addr == null)
                return;
            var anchorPt = FocusedTextView.GetAnchorTopPoint();
            var screenPoint = FocusedTextView.PointToScreen(anchorPt);
            declarationFormInteractor.Show(screenPoint, program, addr);
        }

        private Address GetCommentAnchorAddress()
        {
            if (combinedCodeView.MixedCodeDataView.Focused)
                return combinedCodeView.MixedCodeDataView.GetAnchorAddress();
            return null;
        }

        private void EditComment()
        {
            var addr = GetCommentAnchorAddress();
            if (addr == null)
                return;
            var anchorPt = FocusedTextView.GetAnchorTopPoint();
            var screenPoint = FocusedTextView.PointToScreen(anchorPt);
            commentFormInteractor.Show(screenPoint, program, addr);
        }

        private void MixedCodeDataView_MouseDown(object sender, MouseEventArgs e)
        {
            combinedCodeView.CodeView.ClearSelection();
        }

        private void CodeView_MouseDown(object sender, MouseEventArgs e)
        {
            combinedCodeView.MixedCodeDataView.ClearSelection();
        }

        public void ViewGraph()
        {
            gViewer.Parent = combinedCodeView.Parent;
            using (var g = combinedCodeView.CreateGraphics())
            {
                var uiPreferences = services.RequireService<IUiPreferencesService>();
                gViewer.Graph = CfgGraphGenerator.Generate(uiPreferences, proc, g, combinedCodeView.Font);
            }
            combinedCodeView.Visible = false;
            gViewer.Visible = true;
            gViewer.BringToFront();
        }

        public void ViewCode()
        {
            gViewer.Graph = null;
            gViewer.Visible = false;
            combinedCodeView.Visible = true;
            combinedCodeView.BringToFront();
        }


        void CombinedCodeView_CurrentAddressChanged(object sender, EventArgs e)
        {
            combinedCodeView.MixedCodeDataView.TopAddress = SelectedAddress;
            MixedCodeDataView_TopAddressChanged();
        }

        private void MixedCodeDataView_TopAddressChanged()
        {
            if (!(combinedCodeView.MixedCodeDataView.Model is MixedCodeDataModel mixedCodeDataModel))
                return;

            var topAddress = combinedCodeView.MixedCodeDataView.TopAddress;
            MixedCodeDataModel.DataItemNode dataItemNode = null;
            if (nodeByAddress == null ||
                !nodeByAddress.TryGetLowerBound(topAddress, out dataItemNode))
                return;

            int numer;
            int denom;
            if (topAddress < dataItemNode.EndAddress)
            {
                var startAddr = dataItemNode.StartAddress;
                var endAddr = topAddress;
                var startPos = MixedCodeDataModel.Position(startAddr, 0);
                var endPos = MixedCodeDataModel.Position(endAddr, 0);
                numer = mixedCodeDataModel.CountLines(startPos, endPos);
                denom = dataItemNode.NumLines;
                if (denom == 0)
                {
                    Debug.Print("dataItem.NumLines = 0");
                    numer = 1;
                    denom = 1;
                }
            }
            else
            {
                numer = 1;
                denom = 1;
            }

            nestedTextModel.SetPositionAsNode(dataItemNode.ModelNode, numer, denom);
            combinedCodeView.CodeView.InvalidateModel();
        }

        private void CodeView_PositionChanged()
        {
            var pos = nestedTextModel.GetPositionAsNode();
            var node = pos.Item1;
            var numer = pos.Item2;
            var denom = pos.Item3;

            var dataItemNode = nodeByAddress.Where(n => n.Value.ModelNode == node).
                Select(n => n.Value).SingleOrDefault();

            long numLines = dataItemNode.NumLines;
            var offset = (int)((numLines * numer) / denom);
            var startAddr = dataItemNode.StartAddress;
            var startPos = MixedCodeDataModel.Position(startAddr, 0);
            combinedCodeView.MixedCodeDataView.Model.MoveToLine(startPos, offset);
            combinedCodeView.MixedCodeDataView.InvalidateModel();
        }

        private void MixedCodeDataView_VScrollValueChanged(object sender, EventArgs e)
        {
            MixedCodeDataView_TopAddressChanged();
        }

        private void CodeView_VScrollValueChanged(object sender, EventArgs e)
        {
            CodeView_PositionChanged();
        }

        private void NavigateToToolbarAddress()
        {
            Address addr;
            var txtAddr = combinedCodeView.ToolBarAddressTextbox.Text.Trim();
            if (txtAddr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                txtAddr = txtAddr.Substring(2);
            if (!program.Architecture.TryParseAddress(txtAddr, out addr))
                return;
            UserNavigateToAddress(combinedCodeView.MixedCodeDataView.TopAddress, addr);
        }

        private void UserNavigateToAddress(Address addrFrom, Address addrTo)
        {
            if (!program.SegmentMap.IsValidAddress(addrTo))
                return;
            navInteractor.RememberAddress(addrTo);
            this.SelectedAddress = addrTo;        // ...and move to the new position.
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
            NavigateToToolbarAddress();
        }

        void TextView_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address;
            var proc = e.Destination as Procedure;

            if (proc != null)
                addr = proc.EntryAddress;

            if (addr == null)
                return;

            UserNavigateToAddress(combinedCodeView.MixedCodeDataView.TopAddress, addr);
        }

        private void GViewer_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.Print("{0} {1:X} {2}", e.KeyCode, e.KeyValue, e.KeyData);
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
            {
                gViewer.ZoomF *= 1.2;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                gViewer.ZoomF /= 1.2;
                e.Handled = true;
            }
        }

        private void IViewer_MouseDown(object sender, MsaglMouseEventArgs e)
        {
            Debug.Print("Mousedown");
        }

        private void IViewer_MouseUp(object sender, MsaglMouseEventArgs e)
        {
            Debug.Print("IViewer.Up");
            if (gViewer.PanButtonPressed)
                return;
            var userObj = gViewer.SelectedObject as Node;
            if (userObj == null)
                return;
        }

        private void GViewer_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.Print("Mouseup");
            if (gViewer.PanButtonPressed)
                return;
            var userObj = gViewer.SelectedObject as Node;
            if (userObj == null)
                return;
            var blockData = userObj.UserData as CfgBlockNode;
			Debug.Print("Node: {0}", blockData.Block.Name);
        }

    }
}
