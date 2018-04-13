#region License
/* 
 * Copyright (C) 1999-2018 Pavel Tomin.
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
using Reko.Core.Types;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Gui.Forms;
using Reko.Analysis;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Reko.Gui.Windows.Forms
{
    public class CommentFormInteractor
    {
        private IServiceProvider services;

        private IDeclarationForm commentForm;
        private bool closing;

        private Program program;
        private Address address;

        public CommentFormInteractor(IServiceProvider services)
        {
            this.services = services;
        }

        private void CreateCommentForm()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            this.commentForm = dlgFactory.CreateDeclarationForm();

            commentForm.TextBox.LostFocus += Text_LostFocus;
            commentForm.TextBox.KeyDown += Text_KeyDown;
        }

        private string LabelText()
        {
            var addrStr = (address == null) ? "<null>" : address.ToString();
            return "Enter comment at the address " + addrStr;
        }

        void Text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
            case Keys.Enter:
            case Keys.Escape:
                bool save = (e.KeyCode == Keys.Enter);
                Close(save);
                e.SuppressKeyPress = true;
                e.Handled = true;
                break;
            }
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
            if (address == null)
                return null;
            return program.User.Annotations[address];
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
            program.User.Annotations[address] = comment;
        }
    }
}
