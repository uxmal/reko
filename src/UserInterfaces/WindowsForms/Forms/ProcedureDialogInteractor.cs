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
using Reko.Core.Hll.C;
using Reko.Core.Serialization;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ProcedureDialogInteractor
    {
        protected ProcedureDialog dlg;

        private readonly Program program;
        private readonly UserProcedure proc;

        public ProcedureDialogInteractor(
            Program program,
            UserProcedure userProc)
        {
            this.program = program;
            this.proc = userProc;
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
            dlg.Allocator.Checked = proc.Characteristics.Allocator;
            dlg.IsAlloca.Checked = proc.Characteristics.IsAlloca;
            dlg.Terminates.Checked = proc.Characteristics.Terminates;
            dlg.VarargsFormatParser.Text = proc.Characteristics.VarargsParserClass ?? "";
        }

        private void EnableProcedureName()
        {
            dlg.ProcedureName.Enabled = string.IsNullOrEmpty(dlg.Signature.Text);
        }

        public UserProcedure ApplyChanges()
        {
            if (dlg.DialogResult != DialogResult.OK)
                return null;
            var CSignature = dlg.Signature.Text.Trim();
            if (string.IsNullOrEmpty(CSignature))
                CSignature = null;

            var procName = dlg.ProcedureName.Text;
            if (string.IsNullOrEmpty(procName))
            {
                procName = program.NamingPolicy.ProcedureName(proc.Address);
            }
            var usb = new UserSignatureBuilder(program);
            var procNew = new UserProcedure(proc.Address, procName);
            procNew.CSignature = CSignature;
            procNew.Name = procName;
            procNew.Signature = usb.ParseFunctionDeclaration(CSignature)?.Signature;
            procNew.Decompile = dlg.Decompile.Checked;

            procNew.Characteristics = new ProcedureCharacteristics
            {
                Allocator = dlg.Allocator.Checked,
                Terminates = dlg.Terminates.Checked,
                IsAlloca = dlg.IsAlloca.Checked,
                VarargsParserClass = dlg.VarargsFormatParser.Text.Length > 0
                    ? dlg.VarargsFormatParser.Text
                    : null
            };
            return procNew;
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
                isValid = (sProc is not null);
            } else
            {
                CSignature = null;
                isValid = true;
            }
            EnableControls(isValid);
            if (isValid)
            {
                if (sProc is not null)
                    dlg.ProcedureName.Text = sProc.Name;
                EnableProcedureName();
            }
        }
    }
}
