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
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
    public class ProcedureDialogInteractor
    {
        private ProcedureDialog dlg;
        private SerializedProcedure proc;


        public ProcedureDialogInteractor(SerializedProcedure proc)
        {
            this.proc = proc;
        }

        public ProcedureDialog CreateDialog()
        {
            dlg = new ProcedureDialog();
            PopulateFields();
            dlg.ArgumentList.SelectedIndexChanged += new EventHandler(ArgumentList_SelectedIndexChanged);
            return dlg;
        }

        private void PopulateFields()
        {
            dlg.ProcedureName.Text = proc.Name.Trim();
            if (proc.Signature != null)
            {
                PopulateSignatureFields(proc.Signature);
            }
        }

        private void PopulateSignatureFields(SerializedSignature sig)
        {
            if (sig.ReturnValue != null)
            {
                ListViewItem item = dlg.ArgumentList.Items.Add("<Return value>");
                item.Tag = sig.ReturnValue;
            }
        }

        public void ApplyChangesToProcedure(Procedure selectedProcedure)
        {
            //$TODO: actually apply changes!
        }

        private void ArgumentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
