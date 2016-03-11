#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Output;
using Reko.Gui.Windows.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;

namespace Reko.Gui.Windows
{
    public class CombinedCodeViewerPane : IWindowPane, ICommandTarget
    {
        private IServiceProvider services;
        private Program program;
        private Procedure proc;
        private CombinedCodeView combinedCodeView;
        private NavigationInteractor<Address> navInteractor;

        private Map<Address, int> nodes;
        private NestedTextModel nestedTextModel;
        private GViewer gViewer;

        public CombinedCodeViewerPane()
        {
        }

        public IWindowFrame Frame { get; set; }

        public virtual Address SelectedAddress
        {
            get { return combinedCodeView.CurrentAddress; }
            set { combinedCodeView.CurrentAddress = value; }
        }

        public void DisplayProcedure(Program program, Procedure proc)
        {
            this.program = program;
            this.proc = proc;
            if (program != null)
            {
                var addr = program.GetProcedureAddress(proc);
                if (addr == null)
                {
                    addr = program.ImageMap.Segments.Values
                        .Where(s => s.MemoryArea != null)
                        .Select(s => Address.Max(s.Address, s.MemoryArea.BaseAddress))
                        .FirstOrDefault();
                    if (addr == null)
                    {
                        addr = program.ImageMap.BaseAddress;
                    }
                }
                combinedCodeView.CurrentAddress = addr;
            }
            ProgramChanged();
        }

        private void ProgramChanged()
        {
            if (combinedCodeView == null)
                return;

            combinedCodeView.MixedCodeDataView.Program = program;

            CreateNestedTextModel();
            combinedCodeView.CodeView.Model = nestedTextModel;
        }

        private void CreateNestedTextModel()
        {
            nestedTextModel = new NestedTextModel();
            nodes = new Map<Address, int>();

            foreach (var item in program.ImageMap.Items)
            {
                var curAddr = item.Key;
                var addrTop = curAddr - ((int)curAddr.ToLinear() & 0x0F);

                Procedure proc;
                if (program.Procedures.TryGetValue(curAddr, out proc))
                {
                    var tsf = new TextSpanFormatter();
                    var fmt = new AbsynCodeFormatter(tsf);
                    fmt.InnerFormatter.UseTabs = false;
                    fmt.Write(proc);
                    nestedTextModel.Nodes.Add(tsf.GetModel());
                    nodes[curAddr] = nestedTextModel.Nodes.Count - 1;
                }

                GlobalDataItem_v2 globalDataItem;
                if (program.User.Globals.TryGetValue(curAddr, out globalDataItem))
                {
                    var tlDeser = program.CreateTypeLibraryDeserializer();
                    var dt = globalDataItem.DataType.Accept(tlDeser);
                    var name = globalDataItem.Name;

                    var tsf = new TextSpanFormatter();
                    var fmt = new AbsynCodeFormatter(tsf);
                    fmt.InnerFormatter.UseTabs = false;
                    var gdw = new GlobalDataWriter(program, services);
                    gdw.WriteGlobalVariable(curAddr, dt, name, tsf);
                    nestedTextModel.Nodes.Add(tsf.GetModel());
                    nodes[curAddr] = nestedTextModel.Nodes.Count - 1;
                }
            }
        }

        public Control CreateControl()
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();

            this.combinedCodeView = new CombinedCodeView();
            this.combinedCodeView.Dock = DockStyle.Fill;

            this.combinedCodeView.CurrentAddressChanged += CombinedCodeView_CurrentAddressChanged;

            this.combinedCodeView.MixedCodeDataView.VScrollValueChanged += MixedCodeDataView_VScrollValueChanged;
            this.combinedCodeView.MixedCodeDataView.Services = services;

            this.combinedCodeView.CodeView.VScrollValueChanged += CodeView_VScrollValueChanged;
            this.combinedCodeView.CodeView.Services = services;
            this.combinedCodeView.ContextMenu = uiSvc.GetContextMenu(MenuIds.CtxCodeView);

            this.gViewer = new GViewer();
            this.gViewer.Dock = DockStyle.Fill;
            this.gViewer.Visible = false;
            this.gViewer.PanButtonPressed = true;
            this.gViewer.ToolBarIsVisible = true;
            this.gViewer.KeyDown += GViewer_KeyDown;
            this.gViewer.ContextMenu = uiSvc.GetContextMenu(MenuIds.CtxCodeView);
            this.gViewer.LayoutAlgorithmSettingsButtonVisible = false;
            this.gViewer.LayoutEditingEnabled = false;
            this.gViewer.EdgeInsertButtonVisible = false;
            this.gViewer.SaveButtonVisible = false;
            this.gViewer.SaveGraphButtonVisible = false;
            this.gViewer.SaveAsMsaglEnabled = false;
            this.gViewer.UndoRedoButtonsVisible = false;
            this.gViewer.KeyDown += GViewer_KeyDown;
            this.gViewer.ContextMenu = uiSvc.GetContextMenu(MenuIds.CtxCodeView);
            this.gViewer.MouseUp += GViewer_MouseUp;
            this.gViewer.DrawingPanel.MouseUp += GViewer_MouseUp;
            var iViewer = (IViewer)gViewer;
            iViewer.MouseUp += IViewer_MouseUp;
            iViewer.MouseDown += IViewer_MouseDown;

            this.navInteractor = new NavigationInteractor<Address>();
            this.navInteractor.Attach(this.combinedCodeView);

            return combinedCodeView;
        }



        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
            if (combinedCodeView != null)
                combinedCodeView.Dispose();
            combinedCodeView = null;
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    //status.Status = combinedCodeView.Selection.IsEmpty
                    //    ? MenuStatus.Visible
                    //    : MenuStatus.Visible | MenuStatus.Enabled;
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
                }
            }
            return false;
        }

        public void Copy()
        {
            if (this.proc == null)
                return;
            if (combinedCodeView.Focused)
            {
                //$TODO: @ptomin -- how to select text in the combined code viewer pane?
                //var ms = new MemoryStream();
                //this.codeView.TextView.Selection.Save(ms, System.Windows.Forms.DataFormats.UnicodeText);
                //Debug.Print(Encoding.Unicode.GetString(ms.ToArray()));
                //Clipboard.SetData(DataFormats.UnicodeText, ms);
            }
            else if (false) // combinedCodeView.ProcedureDeclaration.Focused)
            {
                //Clipboard.SetText(combinedCodeView.ProcedureDeclaration.SelectedText);
            }
        }

        public void ViewGraph()
        {
            gViewer.Parent = combinedCodeView.Parent;
            using (var g = combinedCodeView.CreateGraphics())
            {
                gViewer.Graph = CfgGraphGenerator.Generate(proc, g);
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
        }

        private void MixedCodeDataView_VScrollValueChanged(object sender, EventArgs e)
        {
            var topAddress = combinedCodeView.MixedCodeDataView.TopAddress;
            int nodeIndex;
            if (!nodes.TryGetLowerBound(topAddress, out nodeIndex))
                return;
            if (nodeIndex >= nestedTextModel.Nodes.Count)
                return;

            int pos = 0;
            for(int i = 0; i < nodeIndex; i++)
                pos += nestedTextModel.Nodes[i].cLines;

            combinedCodeView.CodeView.SetPositionAsFraction(pos, nestedTextModel.LineCount);
        }

        private void CodeView_VScrollValueChanged(object sender, EventArgs e)
        {
            var frac = nestedTextModel.GetPositionAsFraction();
            var line = (int)(Math.BigMul(frac.Item1, nestedTextModel.LineCount) / frac.Item2);

            Address addr = null;
            foreach (var node in nodes)
            {
                addr = node.Key;
                var nodeIndex = node.Value;
                if (nodeIndex >= nestedTextModel.Nodes.Count)
                    continue;
                var cLines = nestedTextModel.Nodes[nodeIndex].cLines;
                if (line < cLines)
                    break;
                line -= cLines;
            }

            if (addr == null)
                return;

            combinedCodeView.MixedCodeDataView.TopAddress = addr;
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
            Debug.Print("Node: {0}");
        }

    }
}
