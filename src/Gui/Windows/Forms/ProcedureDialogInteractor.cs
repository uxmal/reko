#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Forms
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
            dlg.Signature.TextChanged += Signature_TextChanged;
            return dlg;
        }

        public Procedure_v1 SerializedProcedure { get { return proc; } }

        private void PopulateFields()
        {
            dlg.ProcedureName.Text = proc.Name.Trim();
            if (proc.Signature != null)
            {
                dlg.Signature.Text = SignatureParser.UnparseSignature(proc.Signature, proc.Name);
            }
        }

        public void ApplyChangesToProcedure(Program program, Procedure procedure)
        {
            var ser = program.CreateProcedureSerializer();
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
