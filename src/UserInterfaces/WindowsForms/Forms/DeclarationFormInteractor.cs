#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using System;
using System.Drawing;
using System.IO;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class DeclarationFormInteractor
    {
        private IServiceProvider services;

        private IDeclarationForm declarationForm;
        private bool closing;

        private Program program;
        private Address address;

        private bool editProcedure;

        public DeclarationFormInteractor(IServiceProvider services)
        {
            this.services = services;
        }

        private void CreateDeclarationForm()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            this.declarationForm = dlgFactory.CreateDeclarationForm();

            declarationForm.TextBox.LostFocus += text_LostFocus;
            declarationForm.TextBox.TextChanged += text_TextChanged;
            declarationForm.TextBox.KeyDown += text_KeyDown;
        }

        private string LabelText()
        {
            var addrStr = (address == null) ? "<null>" : address.ToString();
            var titleStr = editProcedure ? 
                "Enter procedure declaration at the address" : 
                "Enter procedure or global variable declaration at the address";
            return titleStr + " " + addrStr;
        }

        void text_KeyDown(object sender, Gui.Controls.KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                case Keys.Escape:
                    SaveAndClose();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;
            }
        }

        public void Show(Point location, Program program, Address address)
        {
            this.program = program;
            this.address = address;
            this.editProcedure = program.Procedures.ContainsKey(address);
            CreateDeclarationForm();
            declarationForm.HintText = LabelText();
            declarationForm.TextBox.Text = GetDeclarationText();
            declarationForm.ShowAt(location);
        }

        private string GetDeclarationText()
        {
            if (address == null)
                return null;
            Procedure_v1 uProc;
            if (program.User.Procedures.TryGetValue(address, out uProc))
            {
                if (!string.IsNullOrEmpty(uProc.CSignature))
                    return uProc.CSignature;
            }
            Procedure proc;
            if (program.Procedures.TryGetValue(address, out proc))
            {
                return proc.Name;
            }
            ImageMapItem item;
            if (program.ImageMap.TryFindItemExact(address, out item) &&
                !(item.DataType is UnknownType))
            {
                return RenderGlobalDeclaration(item.DataType, item.Name ?? "<unnamed>");
            }
            return null;
        }

        /// <summary>
        /// Convert a declaration to its C string representation.
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string RenderGlobalDeclaration(DataType dataType, string name)
        {
            var sw = new StringWriter();
            var tf = new TextFormatter(sw);
            var tyreffo = new CTypeReferenceFormatter(program.Platform, tf);
            tyreffo.WriteDeclaration(dataType, name);
            return sw.ToString();
        }

        public void HideControls()
        {
            declarationForm.Hide();
            declarationForm.Dispose();
            declarationForm = null;
        }

        private void SaveAndClose()
        {
            if (closing)
                return;
            closing = true;
            ModifyDeclaration();
            HideControls();
            closing = false;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        void text_LostFocus(object sender, EventArgs e)
        {
            SaveAndClose();
        }

        private void EnableControls()
        {
            ProcedureBase_v1 sProc;
            GlobalDataItem_v2 global;
            var procText = declarationForm.TextBox.Text;
            if (TryParseSignature(procText, out sProc))
            {
                declarationForm.TextBox.ForeColor = SystemColors.ControlText;
                return;
            }
            if (!editProcedure && TryParseGlobal(procText, out global))
            {
                declarationForm.TextBox.ForeColor = SystemColors.ControlText;
                return;
            }
            // If parser failed, perhaps it's simply a valid name? 
            if (UserSignatureBuilder.IsValidCIdentifier(procText))
            {
                declarationForm.TextBox.ForeColor = SystemColors.ControlText; ;
                return;
            }
            // Not valid name either, die.
            declarationForm.TextBox.ForeColor = Color.Red;
        }

        private bool TryParseSignature(string txtSignature, out ProcedureBase_v1 sProc)
        {
            sProc = null;
            if (program == null || program.Platform == null)
            {
                return false;
            }

            // Attempt to parse the signature.
            var usb = new UserSignatureBuilder(program);
            sProc = usb.ParseFunctionDeclaration(txtSignature);
            return sProc != null;
        }

        private bool TryParseGlobal(string txtGlobal, out GlobalDataItem_v2 global)
        {
            global = null;
            if (program == null || program.Platform == null)
            {
                return false;
            }

            // Attempt to parse the global declaration.
            var usb = new UserSignatureBuilder(program);
            global = usb.ParseGlobalDeclaration(txtGlobal);
            return global != null;
        }

        private void ModifyDeclaration()
        {
            var declText = declarationForm.TextBox.Text.Trim();
            if (!program.Procedures.TryGetValue(address, out Procedure proc))
                proc = null;
            string procName = null;
            string CSignature = null;
            if (TryParseSignature(declText, out var sProc))
            {
                procName = sProc.Name;
                CSignature = declText;
            }
            else if (UserSignatureBuilder.IsValidCIdentifier(declText) &&
                    (proc == null || proc.Name != declText))
            {
                procName = declText;
            }
            else if (!editProcedure && TryParseGlobal(declText, out var parsedGlobal))
            {
                program.User.Procedures.Remove(address);
                program.ModifyUserGlobal(
                    program.Architecture,
                    address,
                    parsedGlobal.DataType, 
                    parsedGlobal.Name
                );
            }

            if (procName != null)
            {
                program.RemoveUserGlobal(address);
                var up = program.EnsureUserProcedure(address, procName);
                if (CSignature != null)
                    up.CSignature = CSignature;
                if (proc != null)
                    proc.Name = procName;
                else
                {
                    var pAddr = new ProgramAddress(program, address);
                    services.RequireService<ICommandFactory>().MarkProcedure(pAddr).Do();
                }
            }
        }
    }
}
