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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Environments.Windows;
using Reko.UserInterfaces.WindowsForms.Forms;
using System.Drawing;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class ProcedureDialogInteractorTests
    {
        private ProcedureDialogInteractor interactor;
        private Procedure_v1 proc;

        [SetUp]
        public void Setup()
        {
            proc = new Procedure_v1();
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var platform = new Win32Platform(null, arch);
            var program = new Program
            {
                Architecture = arch,
                Platform = platform
            };
            interactor = new ProcedureDialogInteractor(program, proc);
        }

        [Test]
        public void OnProcedure()
        {
            proc.Name = "foo";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("foo", dlg.ProcedureName.Text);
                Assert.IsTrue(dlg.ProcedureName.Enabled);
                Assert.AreEqual("", dlg.Signature.Text);
            }
        }

        [Test]
        public void PopulateWithEmptySignature()
        {
            proc.Name = "foo";
            proc.CSignature = "void foo()";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("foo", dlg.ProcedureName.Text);
                Assert.IsFalse(dlg.ProcedureName.Enabled);
                Assert.AreEqual("void foo()", dlg.Signature.Text);
            }
        }

        [Test]
        public void InvalidSignatureText()
        {
            proc.Name = "foo";
            proc.CSignature = "void foo()";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
                dlg.Signature.Text = "void fbn(";
                Assert.AreEqual(Color.Red, dlg.Signature.ForeColor);
                Assert.IsFalse(dlg.OkButton.Enabled);
            }
        }

        [Test]
        public void ValidSignatureText()
        {
            proc.Name = "foo";
            proc.CSignature = "void foo()";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
                dlg.Signature.Text = "void foo_new(int eax)";
                Assert.AreEqual(SystemColors.WindowText, dlg.Signature.ForeColor);
                Assert.IsTrue(dlg.OkButton.Enabled);
                interactor.ApplyChanges();
                Assert.AreEqual("void foo_new(int eax)", proc.CSignature);
                Assert.AreEqual("foo_new", proc.Name);
            }
        }

        [Test]
        public void EnterProcedureName()
        {
            proc.Name = "foo";
            proc.CSignature = "void foo()";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("void foo()", dlg.Signature.Text);
                Assert.IsFalse(dlg.ProcedureName.Enabled);
                dlg.Signature.Text = "";
                Assert.IsTrue(dlg.ProcedureName.Enabled);
                dlg.ProcedureName.Text = "test123";
                Assert.AreEqual(SystemColors.WindowText, dlg.Signature.ForeColor);
                Assert.IsTrue(dlg.OkButton.Enabled);
                interactor.ApplyChanges();
                Assert.IsNull(proc.CSignature);
                Assert.AreEqual("test123", proc.Name);
            }
        }

        [Test]
        public void CheckBoxesDefault()
        {
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.IsTrue(dlg.Decompile.Checked);
                Assert.IsFalse(dlg.Allocator.Checked);
                Assert.IsFalse(dlg.Terminates.Checked);
                dlg.Decompile.Checked = false;
                interactor.ApplyChanges();
            }
            Assert.IsFalse(proc.Decompile);
            Assert.IsFalse(proc.Characteristics.Allocator);
            Assert.IsFalse(proc.Characteristics.Terminates);
        }

        [Test]
        public void CheckBoxesTerminates()
        {
            proc.Characteristics = new ProcedureCharacteristics();
            proc.Characteristics.Terminates = true;
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.IsTrue(dlg.Decompile.Checked);
                Assert.IsFalse(dlg.Allocator.Checked);
                Assert.IsTrue(dlg.Terminates.Checked);
                dlg.Allocator.Checked = true;
                interactor.ApplyChanges();
            }
            Assert.IsTrue(proc.Decompile);
            Assert.IsTrue(proc.Characteristics.Allocator);
            Assert.IsTrue(proc.Characteristics.Terminates);
        }

        [Test]
        public void CheckBoxesAllChecked()
        {
            proc.Decompile = false;
            proc.Characteristics = new ProcedureCharacteristics();
            proc.Characteristics.Terminates = true;
            proc.Characteristics.Allocator = true;
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.Decompile.Checked);
                Assert.IsTrue(dlg.Allocator.Checked);
                Assert.IsTrue(dlg.Terminates.Checked);
                dlg.Terminates.Checked = false;
                interactor.ApplyChanges();
            }
            Assert.IsFalse(proc.Decompile);
            Assert.IsTrue(proc.Characteristics.Allocator);
            Assert.IsFalse(proc.Characteristics.Terminates);
        }

        [Test]
        public void CheckBoxesCanceled()
        {
            proc.Decompile = false;
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.Decompile.Checked);
                Assert.IsFalse(dlg.Allocator.Checked);
                Assert.IsFalse(dlg.Terminates.Checked);
                dlg.Allocator.Checked = true;
                dlg.Terminates.Checked = true;
                dlg.Decompile.Checked = true;
            }
            Assert.IsFalse(proc.Decompile);
            Assert.IsNull(proc.Characteristics);
        }
    }
}
