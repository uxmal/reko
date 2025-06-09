#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Gui;
using Reko.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ProgramPropertiesInteractor
    {
        private ProgramPropertiesDialog dlg;
        private Gui.ViewModels.ProgramPropertiesViewModel programProperties;
        public void Attach(ProgramPropertiesDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            this.programProperties = new ProgramPropertiesViewModel(dlg.Program?.User?.Heuristics);
            dlg.EnableScript.CheckedChanged += delegate { EnableControls(); };
            dlg.OkButton.Click += OkButton_Click;
            dlg.OutputPolicies.SelectedIndexChanged += OutputPolicies_SelectedIndexChanged;
        }

        private void EnableControls()
        {
            dlg.LoadScript.Enabled = dlg.EnableScript.Checked;
        }

        private void dlg_Load(object sender, EventArgs e)
        {
            var loadedScript = dlg.Program.User.OnLoadedScript;
            if (loadedScript is not null)
            {
                dlg.EnableScript.Checked = loadedScript.Enabled;
                dlg.LoadScript.Text = loadedScript.Script;
            }
            dlg.ScanHeuristics.Items.AddRange(
                programProperties.ScanHeuristics
                .Select(h => new ListOption(h.Text, h.Id))
                .OrderBy(x => x.Text)
                .Cast<object>()
                .ToArray());
            dlg.AnalysisHeuristics.Items.AddRange(
                programProperties.AnalysisHeuristics
                .Select(h => new ListOption(h.Text, h.Id))
                .OrderBy(x => x.Text)
                .Cast<object>()
                .ToArray());

            if (dlg.Program.User.Heuristics.Count == 0)
            {
                dlg.ScanHeuristics.SelectedIndex = 0;
                dlg.AnalysisHeuristics.SelectedIndex = 0;
            }
            else
            {
                for (int j = 0; j < dlg.ScanHeuristics.Items.Count; ++j)
                {
                    var item = (ListOption) dlg.ScanHeuristics.Items[j];
                    if (dlg.Program.User.Heuristics.Contains((string) item.Value))
                    {
                        dlg.ScanHeuristics.SetItemCheckState(j, System.Windows.Forms.CheckState.Checked);
                    }
                }
                for (int j = 0; j < dlg.AnalysisHeuristics.Items.Count; ++j)
                {
                    var item = (ListOption) dlg.AnalysisHeuristics.Items[j];
                    if (dlg.Program.User.Heuristics.Contains((string) item.Value))
                    {
                        dlg.AnalysisHeuristics.SetItemCheckState(j, System.Windows.Forms.CheckState.Checked);
                    }
                }
            }
            
            dlg.OutputPolicies.Items.Add(new ListOption("Separate file per segment", Program.SegmentFilePolicy));
            dlg.OutputPolicies.Items.Add(new ListOption("Single file per program", Program.SingleFilePolicy));
            var i = dlg.OutputPolicies.Items.Cast<ListOption>().ToList().FindIndex(x => (string)((ListOption)x).Value == dlg.Program.User.OutputFilePolicy);
            if (i >= 0)
            {
                dlg.OutputPolicies.SelectedIndex = i;
            }
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            dlg.Program.User.OnLoadedScript = new Core.Serialization.Script_v2
            {
                Enabled = dlg.EnableScript.Checked,
                Script = dlg.LoadScript.Text,
            };
            dlg.Program.User.Heuristics.Clear();
            foreach (ListOption sItem in dlg.ScanHeuristics.CheckedItems)
            {
                dlg.Program.User.Heuristics.Add((string)sItem.Value);
            }
            foreach (ListOption sItem in dlg.AnalysisHeuristics.CheckedItems)
            {
                dlg.Program.User.Heuristics.Add((string)sItem.Value);
            }
            var optPolicy = (ListOption) dlg.OutputPolicies.SelectedItem;
            if (optPolicy is not null)
            {
                dlg.Program.User.OutputFilePolicy = (string) optPolicy.Value;
            }
        }

        void OutputPolicies_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
