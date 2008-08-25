/* 
* Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class AnalyzedPageInteractor : PhasePageInteractor
	{
		private AnalyzedPage page;
        private MainFormInteractor mainInteractor;
        private RichEditFormatter formatter;

		public AnalyzedPageInteractor(AnalyzedPage page, MainFormInteractor form)
			: base(page, form)
		{
			this.page = page;
            mainInteractor = form;
            page.ProcedureText.MouseClick += new MouseEventHandler(ProcedureText_MouseClick);
		}

        private void DisplayProcedure(Procedure proc)
        {
            page.ProcedureText.Text = "";
            if (proc != null)
            {
                formatter = new RichEditFormatter(page.ProcedureText);
                formatter.Write(proc);
            }
        }


		public override void EnterPage()
		{
            mainInteractor.MainForm.BrowserList.Items.Clear();
            mainInteractor.MainForm.BrowserList.Visible = true;
            mainInteractor.MainForm.BrowserList.MultiSelect = false;
            mainInteractor.MainForm.BrowserTree.Visible = false;

			Decompiler.RewriteMachineCode();
			Decompiler.AnalyzeDataFlow();

            PopulateBrowserListWithProcedures();
			page.PerformTypeRecovery.Checked = Decompiler.Project.Output.TypeInference;
            mainInteractor.MainForm.BrowserList.SelectedIndexChanged += new EventHandler(BrowserList_SelectedIndexChanged);
		}


		public override bool LeavePage()
		{
            mainInteractor.MainForm.BrowserList.SelectedIndexChanged -= new EventHandler(BrowserList_SelectedIndexChanged);
			Decompiler.Project.Output.TypeInference = page.PerformTypeRecovery.Checked;
			return true;
        }

        private void PopulateBrowserListWithProcedures()
        {
            foreach (KeyValuePair<Address, Procedure> entry in Decompiler.Program.Procedures)
            {
                ListViewItem item = new ListViewItem();
                item.Text = entry.Value.Name;
                item.Tag = entry;
                mainInteractor.MainForm.BrowserList.Items.Add(item);
            }
        }

        public KeyValuePair<Address, Procedure> SelectedProcedureEntry
        {
            get
            {
                ListView browserList = mainInteractor.MainForm.BrowserList;
                if (browserList.SelectedItems.Count <= 0)
                    return new KeyValuePair<Address,Procedure>(null, null);
                KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) browserList.SelectedItems[0].Tag;
                return entry;
            }
        }

        #region ICommandTarget interface 
        public override bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ActionEditSignature:
                    status.Status = MenuStatus.Visible;
                    if (SelectedProcedureEntry.Key != null)
                        status.Status |= MenuStatus.Enabled;
                    return true;
                }
            }
            return base.QueryStatus(ref cmdSet, cmdId, status, text);
        }

        public override bool Execute(ref Guid cmdSet, int cmdId)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ActionEditSignature:
                    {
                        ProcedureSerializer ser = new ProcedureSerializer(Decompiler.Program.Architecture, "stdapi");
                        SerializedProcedure proc = ser.Serialize(SelectedProcedureEntry.Value, SelectedProcedureEntry.Key);
                        ProcedureDialogInteractor i = new ProcedureDialogInteractor(proc);
                        using (ProcedureDialog dlg = i.CreateDialog())
                        {
                            if (DialogResult.OK == dlg.ShowDialog(this.page))
                            {
                                i.ApplyChangesToProcedure(SelectedProcedureEntry.Value);
                                //$TODO: prohibit stepping forward, only go back to previous steps.
                            }
                        }
                    }
                    break;
                }
            }
            return base.Execute(ref cmdSet, cmdId);
        }

        #endregion

        public void BrowserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayProcedure(SelectedProcedureEntry.Value);
        }

        private void ProcedureText_MouseClick(object sender, MouseEventArgs e)
        {
            int i = page.ProcedureText.GetCharIndexFromPosition(e.Location);
            Procedure proc = formatter.GetProcedureAtIndex(i);
            if (proc == null)
                return;
            DisplayProcedure(proc);
        }

	}
}
