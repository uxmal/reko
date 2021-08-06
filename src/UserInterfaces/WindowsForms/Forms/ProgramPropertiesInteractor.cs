#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ProgramPropertiesInteractor
    {
        private ProgramPropertiesDialog dlg;
        private Dictionary<string, string> heuristicDescriptions = new Dictionary<string, string>
        {
            { "shingle", "Shingle heuristic" },
            { "usermode", "Only allow user mode instructions" }
        };

        public void Attach(ProgramPropertiesDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
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
            if (loadedScript != null)
            {
                dlg.EnableScript.Checked = loadedScript.Enabled;
                dlg.LoadScript.Text = loadedScript.Script;
            }
            dlg.Heuristics.Items.AddRange(new object[]
                {
                    "(None)",
                    "Shingle heuristic"
                });
            if (dlg.Program.User.Heuristics.Count == 0)
                dlg.Heuristics.SelectedIndex = 0;
            else
                dlg.Heuristics.SelectedItem = heuristicDescriptions[dlg.Program.User.Heuristics.First()];

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
            if (dlg.Heuristics.SelectedIndex != 0)
            {
                var item = heuristicDescriptions.First(
                    de => de.Value == (string)dlg.Heuristics.SelectedItem);
                dlg.Program.User.Heuristics.Add(item.Key);
            }
            else
            {
                dlg.Program.User.Heuristics.Clear();
            }
            var optPolicy = (ListOption) dlg.OutputPolicies.SelectedItem;
            if (optPolicy != null)
            {
                dlg.Program.User.OutputFilePolicy = (string) optPolicy.Value;
            }
            dlg.Program.User.AggressiveBranchRemoval = dlg.AggressiveBranchRemoval.Checked;
        }

        void OutputPolicies_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
