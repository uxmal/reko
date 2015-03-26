#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class ProcedureDialogInteractor
    {
        protected ProcedureDialog dlg;

        private IProcessorArchitecture arch;
        private Procedure_v1 proc;

        public ProcedureDialogInteractor(IProcessorArchitecture arch, Procedure_v1 proc)
        {
            this.arch = arch;
            this.proc = proc;
            if (proc.Signature != null && proc.Signature.Arguments == null)
            {
                proc.Signature.Arguments = new Argument_v1[0];
            }
        }

        public ProcedureDialog CreateDialog()
        {
            dlg = new ProcedureDialog();
            PopulateFields();
            dlg.ArgumentList.SelectedIndexChanged += new EventHandler(ArgumentList_SelectedIndexChanged);
            dlg.Signature.TextChanged += new EventHandler(Signature_TextChanged);
            return dlg;
        }

        public Procedure_v1 SerializedProcedure { get { return proc; } }

        private void PopulateFields()
        {
            dlg.ProcedureName.Text = proc.Name.Trim();
            if (proc.Signature != null)
            {
                dlg.Signature.Text = SignatureParser.UnparseSignature(proc.Signature, proc.Name);
                PopulateSignatureFields(proc.Signature);
            }
        }



        private string StringizeSignature(SerializedSignature sig, string name)
        {
            return SignatureParser.UnparseSignature(sig, name);
        }

        private void PopulateSignatureFields(SerializedSignature sig)
        {
            if (sig.ReturnValue != null)
            {
                ListViewItem item = new ListViewItem("<Return value>"); 
                item.Tag = sig.ReturnValue;
                dlg.ArgumentList.Items.Add(item);
            }
        }

        public void ApplyChangesToProcedure(Procedure procedure)
        {
            var ser = arch.CreateProcedureSerializer(new TypeLibraryLoader(arch, true), "stdapi");          //BUG:Where does convetion come from? Platform?
            var sp = new SignatureParser(arch);
            sp.Parse(dlg.Signature.Text);
            Debug.Assert(sp.IsValid);
            procedure.Signature = ser.Deserialize(sp.Signature, procedure.Frame);
        }

        private void EnableControls(bool signatureIsValid)
        {
            dlg.OkButton.Enabled = signatureIsValid;
            dlg.Signature.ForeColor = signatureIsValid ? SystemColors.WindowText : Color.Red;
        }

        protected void ArgumentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlg.ArgumentList.SelectedItems.Count != 0)
            {
                dlg.ArgumentProperties.SelectedObjects = new object[] {
                    dlg.ArgumentList.SelectedItems[0].Tag
                };
            }
            else
                dlg.ArgumentProperties.SelectedObjects = new object[0];
        }

        protected void Signature_TextChanged(object sender, EventArgs e)
        {
            var parser = new SignatureParser(arch);
            parser.Parse(dlg.Signature.Text);
            EnableControls(parser.IsValid);
            if (parser.IsValid)
            {
                proc.Signature = parser.Signature;
            }
        }

    }
}
