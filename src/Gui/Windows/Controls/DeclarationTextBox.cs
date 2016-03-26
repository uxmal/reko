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

using Reko.Gui.Controls;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Analysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
{
    public class DeclarationTextBox : IDisposable
    {
        private TextBox text;
        private Label label;

        private Program program;
        private Address address;

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
            return "Enter procedure or global variable declaration at address " + addrStr;
        }

        void text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
            label.Text = LabelText();
            label.Location = new Point(location.X, location.Y - text.Height - label.Height - 9);
            text.Text = GetDeclarationText();
            text.Location = new Point(location.X, location.Y - text.Height - 9);
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
                return uProc.CSignature;
            }
            Procedure proc;
            if (program.Procedures.TryGetValue(address, out proc))
            {
                return proc.Name;
            }
            return null;
        }

        public void HideControls()
        {
            text.Visible = false;
            label.Visible = false;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            EnableControls();
            ModifyDeclaration();
        }

        void text_LostFocus(object sender, EventArgs e)
        {
            HideControls();
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
            var procText = text.Text;
            if (!TryParseSignature(procText, out sProc))
            {
                // If parser failed, perhaps it's simply a valid name? 
                if (!IsValidCIdentifier(procText))
                {
                    // Not valid name either, die.
                    text.ForeColor = Color.Red;
                    return;
                }
            }
            text.ForeColor = SystemColors.ControlText;
        }

        private bool IsValidCIdentifier(string id)
        {
            return Regex.IsMatch(id, "^[_a-zA-Z][_a-zA-Z0-9]*$");
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

        private void ModifyDeclaration()
        {
            var addr = address;
            var declText = text.Text.Trim();
            Procedure proc;
            if (!program.Procedures.TryGetValue(addr, out proc))
                return;
            ProcedureBase_v1 sProc;
            if (TryParseSignature(declText, out sProc))
            {
                var up = program.EnsureUserProcedure(addr, sProc.Name);
                up.CSignature = declText;
                proc.Name = sProc.Name;
            }
            else if (IsValidCIdentifier(declText) &&
                     proc.Name != declText)
            {
                var up = program.EnsureUserProcedure(addr, proc.Name);
                proc.Name = declText;
            }
        }
    }
}
