#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.Drawing;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class CommentFormInteractor
    {
        private IServiceProvider services;

        private IDeclarationForm commentForm;
        private bool closing;

        private Program program;
        private Address? address;

        public CommentFormInteractor(IServiceProvider services)
        {
            this.services = services;
        }

        private void CreateCommentForm()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            this.commentForm = dlgFactory.CreateCommentForm();

            commentForm.TextBox.LostFocus += Text_LostFocus;
            commentForm.TextBox.KeyDown += Text_KeyDown;
        }

        private string LabelText()
        {
            var addrStr = (address is null) ? "<null>" : address.ToString();
            return "Enter comment at the address " + addrStr;
        }

        void Text_KeyDown(object sender, Gui.Controls.KeyEventArgs e)
        {
            switch (e.KeyData)
            {
            case Keys.Enter | Keys.Shift:
                // Shift+Enter doesn't dismiss the comment form,
                // but is forwarded to the edit control which 
                // inserts a new line in the annotation.
                return;
            case Keys.Enter:
                Close(true);
                break;
            case Keys.Escape:
                Close(false);
                break;
            default:
                return;
            }
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        public void Show(Point location, Program program, Address address)
        {
            this.program = program;
            this.address = address;
            CreateCommentForm();
            commentForm.HintText = LabelText();
            commentForm.TextBox.Text = GetCommentText();
            commentForm.ShowAt(location);
        }

        private string GetCommentText()
        {
            if (address is null)
                return null;
            return program.User.Annotations[address.Value];
        }

        public void HideControls()
        {
            commentForm.Hide();
            commentForm.Dispose();
            commentForm = null;
        }

        private void Close(bool save)
        {
            if (closing)
                return;
            closing = true;
            if (save)
                ModifyComment();
            HideControls();
            closing = false;
        }

        void Text_LostFocus(object sender, EventArgs e)
        {
            Close(true);
        }

        private void ModifyComment()
        {
            var comment = commentForm.TextBox.Text.Trim();
            if (!string.IsNullOrEmpty(comment))
            {
                program.User.Annotations[address.Value] = comment;
            }
            else
            {
                program.User.Annotations.Remove(address.Value);
            }
        }
    }
}
