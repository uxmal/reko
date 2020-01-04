#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ProcedureDialogInteractor
    {
        protected ProcedureDialog dlg;

        private Program program;
        private Procedure_v1 proc;

        public ProcedureDialogInteractor(Program program, Procedure_v1 proc)
        {
            this.program = program;
            this.proc = proc;
        }

        public ProcedureDialog CreateDialog()
        {
            dlg = new ProcedureDialog(this);
            PopulateFields();
            dlg.Signature.TextChanged += Signature_TextChanged;
            return dlg;
        }

        private void PopulateFields()
        {
            dlg.Signature.Text = proc.CSignature;
            dlg.ProcedureName.Text = proc.Name;
            EnableProcedureName();
            dlg.Decompile.Checked = proc.Decompile;
            var characteristics = (proc.Characteristics != null) ?
                proc.Characteristics : DefaultProcedureCharacteristics.Instance;
            dlg.Allocator.Checked = characteristics.Allocator;
            dlg.Terminates.Checked = characteristics.Terminates;
        }

        private void EnableProcedureName()
        {
            dlg.ProcedureName.Enabled = string.IsNullOrEmpty(dlg.Signature.Text) ?
                true: false;
        }

        public void ApplyChanges()
        {
            var CSignature = dlg.Signature.Text.Trim();
            if (string.IsNullOrEmpty(CSignature))
                CSignature = null;
            proc.CSignature = CSignature;
            proc.Name = dlg.ProcedureName.Text;
            proc.Decompile = dlg.Decompile.Checked;
            if (proc.Characteristics == null)
                proc.Characteristics = new ProcedureCharacteristics();
            proc.Characteristics.Allocator = dlg.Allocator.Checked;
            proc.Characteristics.Terminates = dlg.Terminates.Checked;
        }

        private void EnableControls(bool signatureIsValid)
        {
            dlg.OkButton.Enabled = signatureIsValid;
            dlg.Signature.ForeColor = signatureIsValid ? SystemColors.WindowText : Color.Red;
        }

        protected void Signature_TextChanged(object sender, EventArgs e)
        {
            // Attempt to parse the signature.
            var CSignature = dlg.Signature.Text.Trim();
            ProcedureBase_v1 sProc = null;
            bool isValid;
            if (!string.IsNullOrEmpty(CSignature))
            {
                var usb = new UserSignatureBuilder(program);
                sProc = usb.ParseFunctionDeclaration(CSignature);
                isValid = (sProc != null);
            } else
            {
                CSignature = null;
                isValid = true;
            }
            EnableControls(isValid);
            if (isValid)
            {
                if (sProc != null)
                    dlg.ProcedureName.Text = sProc.Name;
                EnableProcedureName();
            }
        }

    }
}
