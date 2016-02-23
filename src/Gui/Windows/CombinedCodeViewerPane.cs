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

namespace Reko.Gui.Windows
{
    public class CombinedCodeViewerPane : IWindowPane
    {
        private IServiceProvider services;
        private Program program;
        private CombinedCodeView combinedCodeView;
        private NavigationInteractor<Address> navInteractor;

        private Map<Address, int> nodes;
        private NestedTextModel nestedTextModel;

        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                if (value != null)
                {
                    ProgramChanged();
                }
            }
        }

        public virtual Address SelectedAddress
        {
            get
            {
                return combinedCodeView.CurrentAddress;
            }
            set
            {
                combinedCodeView.CurrentAddress = value;
            }
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
            this.combinedCodeView = new CombinedCodeView();
            this.combinedCodeView.Dock = DockStyle.Fill;

            this.combinedCodeView.CurrentAddressChanged += CombinedCodeView_CurrentAddressChanged;

            this.combinedCodeView.MixedCodeDataView.VScrollValueChanged += MixedCodeDataView_VScrollValueChanged;
            this.combinedCodeView.MixedCodeDataView.Services = services;

            this.combinedCodeView.CodeView.VScrollValueChanged += CodeView_VScrollValueChanged;
            this.combinedCodeView.CodeView.Services = services;

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
    }
}
