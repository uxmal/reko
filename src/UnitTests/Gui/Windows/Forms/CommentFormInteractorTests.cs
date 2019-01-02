#region License
/* 
 * Copyright (C) 1999-2019 Pavel Tomin.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms.Forms;
using Rhino.Mocks;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    public class CommentFormInteractorTests
    {
        MockRepository mr;
        private ServiceContainer services;
        private CommentFormInteractor interactor;
        private IDeclarationForm commentForm;
        private FakeTextBox textBox;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            services = new ServiceContainer();
            commentForm = mr.Stub<IDeclarationForm>();
            textBox = new FakeTextBox();
            commentForm.Stub(f => f.TextBox).Return(textBox);
            commentForm.Stub(f => f.ShowAt(new Point()));
            commentForm.Stub(f => f.Hide());
            commentForm.Stub(f => f.Dispose());
            var dlgFactory = mr.Stub<IDialogFactory>();
            dlgFactory
                .Stub(f => f.CreateCommentForm())
                .Return(commentForm);
            services.AddService<IDialogFactory>(dlgFactory);
            mr.ReplayAll();
            interactor = new CommentFormInteractor(services);
            program = new Program();
        }

        private void Given_Comment(uint addr, string text)
        {
            program.User.Annotations[Address.Ptr32(addr)] = text;
        }

        private void ShowCommentForm(uint addr)
        {
            interactor.Show(new Point(0, 0), program, Address.Ptr32(addr));
        }

        private void LostFocus()
        {
            textBox.FireLostFocus();
        }

        private string CommentAt(uint addr)
        {
            return program.User.Annotations[Address.Ptr32(addr)];
        }

        private int NumberOfComments()
        {
            return program.User.Annotations.Count();
        }

        [Test]
        public void Cfi_HintText()
        {
            ShowCommentForm(0x14);

            var expected = "Enter comment at the address 00000014";
            Assert.AreEqual(expected, commentForm.HintText);
        }

        [Test]
        public void Cfi_ShowComment()
        {
            Given_Comment(0x13, "This is a comment");

            ShowCommentForm(0x13);

            Assert.AreEqual("This is a comment", commentForm.TextBox.Text);
        }

        [Test]
        public void Cfi_NoCommentsAtSpecifiedAddress()
        {
            Given_Comment(0x13, "This is a comment");

            ShowCommentForm(0x14);

            Assert.AreEqual("", commentForm.TextBox.Text);
        }

        [Test]
        public void Cfi_EditComment()
        {
            Given_Comment(0x15, "This is a comment");

            ShowCommentForm(0x15);
            commentForm.TextBox.Text += " [edited]";
            LostFocus();

            var expected = "This is a comment [edited]";
            Assert.AreEqual(expected, CommentAt(0x15));
        }

        [Test]
        public void Cfi_CreateComment()
        {
            ShowCommentForm(0x16);
            commentForm.TextBox.Text = "New comment";
            LostFocus();

            Assert.AreEqual("New comment", CommentAt(0x16));
        }

        [Test(Description ="Remove comment if user sets emptry string")]
        public void Cfi_RemoveComment()
        {
            Given_Comment(0x17, "This is a comment");

            ShowCommentForm(0x17);
            commentForm.TextBox.Text = "";
            LostFocus();

            Assert.AreEqual(null, CommentAt(0x17));
            Assert.AreEqual(0, NumberOfComments());
        }
    }
}
