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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Gui.Windows.Forms;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class ProcedureDialogInteractorTests
    {
        private TestProcedureDialogInteractor interactor;
        private Procedure_v1 proc;
 
        [SetUp]
        public void Setup()
        {
            proc = new Procedure_v1();
            interactor = new TestProcedureDialogInteractor(proc);
        }

        [Test]
        public void OnProcedure()
        {
            proc.Name = " foo ";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("foo", dlg.ProcedureName.Text); 
            }
        }

        [Test]
        public void PopulateWithEmptySignature()
        {
            proc.Name = "foo";
            proc.Signature = new SerializedSignature();
            proc.Signature.Arguments = new Argument_v1[0];
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
            }
        }

        [Test]
        public void InvalidSignatureText()
        {
            proc.Name = "foo";
            proc.Signature = new SerializedSignature();
            proc.Signature.Arguments = new Argument_v1[0];
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
                interactor.UserChangeSignatureText("void fbn(");
                Assert.AreEqual(System.Drawing.Color.Red, dlg.Signature.ForeColor);
                Assert.IsFalse(dlg.OkButton.Enabled);
            }
        }

        [Test]
        public void ValidSignatureText()
        {
            proc.Name = "foo";
            proc.Signature = new SerializedSignature();
            proc.Signature.Arguments = new Argument_v1[0];
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
                interactor.UserChangeSignatureText("void foo(word32 eax)");
                Assert.AreEqual(SystemColors.WindowText, dlg.Signature.ForeColor);
                Assert.IsTrue(dlg.OkButton.Enabled);
            }
        }

        private class TestProcedureDialogInteractor : ProcedureDialogInteractor
        {
            public TestProcedureDialogInteractor(Procedure_v1 proc)
                : base(new X86ArchitectureFlat32(), proc)
            {
            }

            internal void UserChangeSignatureText( string text)
            {
                dlg.Signature.Text = text;
                base.Signature_TextChanged(dlg.Signature, EventArgs.Empty);
            }
        }
    }
}
