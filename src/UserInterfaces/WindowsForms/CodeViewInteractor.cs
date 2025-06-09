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

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Reko.Core;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using Reko.Gui.ViewModels;
using Reko.UserInterfaces.WindowsForms.Controls;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class CodeViewInteractor : IWindowPane, ICommandTarget
    {
        private readonly TextSpanFactory factory;
        private IServiceProvider services;
        private Program program;
        private Procedure proc;
        private TextView codeView;
        //private NavigationInteractor<Address> navInteractor;

        private SortedList<Address, MixedCodeDataModel.DataItemNode> nodeByAddress;
        private NestedTextModel nestedTextModel;
        private GViewer gViewer;
        private DeclarationFormInteractor declarationFormInteractor;

        //private ImageSegment segment;
        private bool showProcedures;

        public CodeViewInteractor(TextSpanFactory factory)
        {
            this.factory = factory;
        }

        public IWindowFrame Frame { get; set; }

        public void DisplayStatement(Program program, Statement stm)
        {
            this.program = program;
            this.proc = stm.Block.Procedure;
            this.showProcedures = true;
            ProgramChanged();
            if (program is not null)
            {
                //var addr = program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress);
                //SelectedAddress = addr;
            }
        }

        public void DisplayProcedure(Program program, Procedure proc)
        {
            this.program = program;
            this.proc = proc;
            this.showProcedures = true;
            ProgramChanged();
        }

        private void ProgramChanged()
        {
            CreateNestedTextModel();
        }

        private void MixedCodeDataView_ModelChanged(object sender, EventArgs e)
        {
            //if (combinedCodeView is null)
            //    return;

            //CreateNestedTextModel();

            //MixedCodeDataView_TopAddressChanged();
        }

        private void CreateNestedTextModel()
        {
            this.nestedTextModel = new NestedTextModel();

            this.nodeByAddress = new SortedList<Address, MixedCodeDataModel.DataItemNode>();
            foreach (var proc in program.Procedures.Values)
            {
                var model = new ProcedureCodeModel(proc, factory, services.RequireService<ISelectedAddressService>());
                //$TODO: make spacing between globals / procedures user adjustable
                model.NumEmptyLinesAfter = 2;
                nestedTextModel.Nodes.Add(model);
            }
            codeView.Model = nestedTextModel;
        }

        private bool ShowItem(MixedCodeDataModel.DataItemNode item)
        {
            if (!showProcedures && item.Proc is not null)
                return false;

            //if (segment is not null && !segment.IsInRange(item.StartAddress))
            //    return false;

            return true;
        }

        private bool ShowAllItems()
        {
            //return (segment is null && showProcedures);
            return showProcedures;
        }

        public object CreateControl()
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();

            this.codeView = new TextView();
            this.codeView.Services = services;
            this.codeView.Dock = DockStyle.Fill;
            //this.codeView.VScrollValueChanged += CodeView_VScrollValueChanged;
            //this.codeView.MouseDown += CodeView_MouseDown;
            this.codeView.Navigate += TextView_Navigate;

            uiSvc.SetContextMenu(this.codeView, MenuIds.CtxCodeView);

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
            var iViewer = (IViewer) gViewer;
            iViewer.MouseUp += IViewer_MouseUp;
            iViewer.MouseDown += IViewer_MouseDown;

            //this.navInteractor = new NavigationInteractor<Address>();
            //this.navInteractor.Attach(this.combinedCodeView);

            declarationFormInteractor = new DeclarationFormInteractor(services);
            return codeView;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
            if (codeView is not null)
                codeView.Dispose();
            codeView = null;
        }

        private TextView FocusedTextView
        {
            get
            {
                if (codeView.Focused)
                    return codeView;
                return null;
            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                if (!ShowAllItems())
                {
                    switch ((CmdIds)cmdId.ID)
                    {
                    case CmdIds.EditDeclaration:
                    case CmdIds.ViewCfgGraph:
                        status.Status = MenuStatus.Visible;
                        return true;
                    }
                }
                switch ((CmdIds)cmdId.ID)
                {
                case CmdIds.TextEncodingChoose:
                    status.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.EditCopy:
                    status.Status = FocusedTextView is null || FocusedTextView.Selection.IsEmpty
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
                    //status.Status = GetAnchorAddress() is null
                    //    ? MenuStatus.Visible
                    //    : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        public async ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds)cmdId.ID)
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
                    return await ChooseTextEncoding();
                case CmdIds.EditDeclaration:
                    //EditDeclaration();
                    return true;
                }
            }
            return false;
        }

        public void Copy()
        {
            if (this.proc is null)
                return;

            if (FocusedTextView is null)
                return;

            var ms = new MemoryStream();
            FocusedTextView.Selection.Save(ms, DataFormats.UnicodeText);
            var text = new string(Encoding.Unicode.GetChars(ms.ToArray()));
            Clipboard.SetData(DataFormats.UnicodeText, text);
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
                    program.User.TextEncoding = enc;
                    this.codeView.RecomputeLayout();
                }
            }
            return true;
        }

        [Obsolete("", true)]
        private Address? CodeView_GetAnchorAddress()
        {
            var pt = codeView.GetAnchorMiddlePoint();
            var tag = codeView.GetTagFromPoint(pt);
            var addr = tag as Address?;
            var proc = tag as Procedure;

            if (proc is not null)
                addr = proc.EntryAddress;

            return addr;
        }

            /*
        private Address MixedCodeDataView_GetAnchorAddress()
        {
            var addr = combinedCodeView.MixedCodeDataView.GetAnchorAddress();
            ImageMapItem item;
            if (program.ImageMap.TryFindItem(addr, out item))
            {
                var blockItem = item as ImageMapBlock;
                if (blockItem is not null)
                {
                    addr = program.GetProcedureAddress(blockItem.Block.Procedure);
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
            if (addr is null)
                return;
            var anchorPt = FocusedTextView.GetAnchorTopPoint();
            var screenPoint = FocusedTextView.PointToScreen(anchorPt);
            declarationFormInteractor.Show(screenPoint, program, addr);
        }
        */

        public void ViewGraph()
        {
            gViewer.Parent = codeView.Parent;
            using (var g = codeView.CreateGraphics())
            {
                var uiPreferences = services.RequireService<IUiPreferencesService>();
                //gViewer.Graph = CfgGraphGenerator.Generate(uiPreferences, proc, g, codeView.Font);
            }
            codeView.Visible = false;
            gViewer.Visible = true;
            gViewer.BringToFront();
        }

        public void ViewCode()
        {
            //gViewer.Graph = null;
            gViewer.Visible = false;
            codeView.Visible = true;
            codeView.BringToFront();
        }

        void TextView_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address?;
            var proc = e.Destination as Procedure;

            if (proc is not null)
                addr = proc.EntryAddress;

            if (addr is null)
                return;

            //UserNavigateToAddress(combinedCodeView.MixedCodeDataView.TopAddress, addr);
        }

        private void GViewer_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.Print("{0} {1:X} {2}", e.KeyCode, e.KeyValue, e.KeyData);
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
            {
                //gViewer.ZoomF *= 1.2;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                //gViewer.ZoomF /= 1.2;
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
            if (userObj is null)
                return;
        }

        private void GViewer_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.Print("Mouseup");
            if (gViewer.PanButtonPressed)
                return;
            var userObj = gViewer.SelectedObject as Node;
            if (userObj is null)
                return;
            var blockData = userObj.UserData as CfgBlockNode;
            Debug.Print("Node: {0}", blockData.Block.DisplayName);
        }

    }
}
