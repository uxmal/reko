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
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Analysis;
using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
{
    public class DeclarationTextBox : IDisposable
    {
        public event EventHandler<DeclarationEventArgs> ProcedureAdded;
        public event EventHandler<DeclarationEventArgs> GlobalEdited;

        private TextBox text;
        private Label label;

        private Program program;
        private Address address;

        private bool editProcedure;

        public DeclarationTextBox(Control bgControl)
        {
            Debug.Print(bgControl.GetType().FullName);
            text = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Parent = bgControl,
                Visible = false,
            };
            label = new Label
            {
                ForeColor = SystemColors.ControlText,
                BackColor = SystemColors.Info,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Parent = bgControl,
                Visible = false,
            };

            text.LostFocus += text_LostFocus;
            text.TextChanged += text_TextChanged;
            text.KeyDown += text_KeyDown;
        }

        private string LabelText()
        {
            var addrStr = (address == null) ? "<null>" : address.ToString();
            var titleStr = editProcedure ? 
                "Enter procedure declaration at the address" : 
                "Enter procedure or global variable declaration at the address";
            return titleStr + " " + addrStr;
        }

        void text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Escape:
                    HideControls();
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
            label.Text = LabelText();
            label.Location = new Point(location.X, location.Y - text.Height - label.Height);
            text.Text = GetDeclarationText();
            text.Location = new Point(location.X, location.Y - text.Height);
            text.Width = label.Width;
            label.BringToFront();
            text.Visible = true;
            label.Visible = true;
            text.BringToFront();
            text.Focus();
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
            GlobalDataItem_v2 global;
            if(program.User.Globals.TryGetValue(address, out global))
            {
                return GetGlobalDeclaration(global.DataType, global.Name);
            }
            return null;
        }

        private string GetGlobalDeclaration(SerializedType dataType, string name)
        {
            var tlDeser = program.CreateTypeLibraryDeserializer();
            var dt = dataType.Accept(tlDeser);
            var sw = new StringWriter();
            var tf = new TextFormatter(sw);
            var tyreffo = new CTypeReferenceFormatter(program.Platform, tf, true);
            tyreffo.WriteDeclaration(dt, name);
            return sw.ToString();
        }

        public void HideControls()
        {
            text.Visible = false;
            label.Visible = false;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        void text_LostFocus(object sender, EventArgs e)
        {
            HideControls();
            ModifyDeclaration();
        }

        public void Dispose()
        {
            if (text != null) text.Dispose();
            if (label != null) label.Dispose();
            text = null;
            label = null;
        }

        private void EnableControls()
        {
            ProcedureBase_v1 sProc;
            GlobalDataItem_v2 global;
            var procText = text.Text;
            if (TryParseSignature(procText, out sProc))
            {
                text.ForeColor = SystemColors.ControlText;
                return;
            }
            if (!editProcedure && TryParseGlobal(procText, out global))
            {
                text.ForeColor = SystemColors.ControlText;
                return;
            }
            // If parser failed, perhaps it's simply a valid name? 
            if (UserSignatureBuilder.IsValidCIdentifier(procText))
            {
                text.ForeColor = SystemColors.ControlText; ;
                return;
            }
            // Not valid name either, die.
            text.ForeColor = Color.Red;
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
            sProc = usb.ParseFunctionDeclaration(txtSignature, program.Architecture.CreateFrame());
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
            var declText = text.Text.Trim();
            Procedure proc;
            if (!program.Procedures.TryGetValue(address, out proc))
                proc = null;
            ProcedureBase_v1 sProc;
            GlobalDataItem_v2 parsedGlobal;
            string procName = null;
            string CSignature = null;
            if (TryParseSignature(declText, out sProc))
            {
                procName = sProc.Name;
                CSignature = declText;
            }
            else if (UserSignatureBuilder.IsValidCIdentifier(declText) &&
                    (proc == null || proc.Name != declText))
            {
                procName = declText;
            }
            else if (!editProcedure && TryParseGlobal(declText, out parsedGlobal))
            {
                program.User.Procedures.Remove(address);
                program.ModifyUserGlobal(
                    address, parsedGlobal.DataType, parsedGlobal.Name
                );
                GlobalEdited.Fire(this, new DeclarationEventArgs(address));
            }

            if (procName != null)
            {
                program.User.Globals.Remove(address);
                var up = program.EnsureUserProcedure(address, procName);
                if (CSignature != null)
                    up.CSignature = CSignature;
                if (proc != null)
                    proc.Name = procName;
                else
                    ProcedureAdded.Fire(this, new DeclarationEventArgs(address));
            }
        }
    }

    public class DeclarationEventArgs : EventArgs
    {
        public DeclarationEventArgs(Address address)
        {
            this.Address = address;
        }

        public Address Address { get; private set; }
    }
}
