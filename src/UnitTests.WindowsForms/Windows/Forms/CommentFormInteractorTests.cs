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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms.Forms;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    public class CommentFormInteractorTests
    {
        private ServiceContainer services;
        private CommentFormInteractor interactor;
        private Mock<IDeclarationForm> commentForm;
        private FakeTextBox textBox;
        private Program program;

        [SetUp]
        public void Setup()
        {
            services = new ServiceContainer();
            commentForm = new Mock<IDeclarationForm>();
            textBox = new FakeTextBox();
            commentForm.Setup(f => f.TextBox).Returns(textBox);
            commentForm.Setup(f => f.ShowAt(new Point()));
            commentForm.Setup(f => f.Hide());
            commentForm.Setup(f => f.Dispose());
            commentForm.SetupProperty(f => f.HintText);
            var dlgFactory = new Mock<IDialogFactory>();
            dlgFactory
                .Setup(f => f.CreateCommentForm())
                .Returns(commentForm.Object);
            services.AddService<IDialogFactory>(dlgFactory.Object);
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
            Assert.AreEqual(expected, commentForm.Object.HintText);
        }

        [Test]
        public void Cfi_ShowComment()
        {
            Given_Comment(0x13, "This is a comment");

            ShowCommentForm(0x13);

            Assert.AreEqual("This is a comment", commentForm.Object.TextBox.Text);
        }

        [Test]
        public void Cfi_NoCommentsAtSpecifiedAddress()
        {
            Given_Comment(0x13, "This is a comment");

            ShowCommentForm(0x14);

            Assert.AreEqual("", commentForm.Object.TextBox.Text);
        }

        [Test]
        public void Cfi_EditComment()
        {
            Given_Comment(0x15, "This is a comment");

            ShowCommentForm(0x15);
            commentForm.Object.TextBox.Text += " [edited]";
            LostFocus();

            var expected = "This is a comment [edited]";
            Assert.AreEqual(expected, CommentAt(0x15));
        }

        [Test]
        public void Cfi_CreateComment()
        {
            ShowCommentForm(0x16);
            commentForm.Object.TextBox.Text = "New comment";
            LostFocus();

            Assert.AreEqual("New comment", CommentAt(0x16));
        }

        [Test(Description ="Remove comment if user sets emptry string")]
        public void Cfi_RemoveComment()
        {
            Given_Comment(0x17, "This is a comment");

            ShowCommentForm(0x17);
            commentForm.Object.TextBox.Text = "";
            LostFocus();

            Assert.AreEqual(null, CommentAt(0x17));
            Assert.AreEqual(0, NumberOfComments());
        }
    }
}
