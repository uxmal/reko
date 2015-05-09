#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
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
        public void ReturnValue()
        {
            proc.Name = "x";
            proc.Signature = new SerializedSignature();
            proc.Signature.Arguments = new Argument_v1[0];
            CreateReturnValue(proc.Signature);
            //proc.Signature.Arguments = new SerializedArgument[1];
            //proc.Signature.Arguments[0] = new SerializedArgument();
            //proc.Signature.Arguments[0].Name = "arg0";
            //proc.Signature.Arguments[0].Kind = new SerializedStackVariable(4);
            //proc.Signature.Arguments[0].Type = "int32";

            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual(1, dlg.ArgumentList.Items.Count);
                Assert.AreEqual("<Return value>", dlg.ArgumentList.Items[0].Text);
            }
        }

        private static void CreateReturnValue(SerializedSignature sig)
        {
            sig.ReturnValue = new Argument_v1();
            sig.ReturnValue.Kind = new Register_v1("eax");
            sig.ReturnValue.Type = new SerializedTypeReference("int32");
        }

        [Test]
        public void SelectReturnValue()
        {
            proc.Name = "x";
            proc.Signature = new SerializedSignature();
            proc.Signature.Arguments = new Argument_v1[0];
            CreateReturnValue(proc.Signature);
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                interactor.UserSwitchTab(1);
                interactor.UserSelectedItem(dlg.ArgumentList, 0);

                ListViewItem item = dlg.ArgumentList.Items[0];
                Assert.AreEqual("<Return value>", item.Text);
                Assert.AreSame(item.Tag, dlg.ArgumentProperties.SelectedObject);
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
                : base(new IntelArchitecture(ProcessorMode.Protected32), proc)
            {
            }

            public void UserSelectedItem(ListView listView, int p)
            {
                listView.Items[0].Selected = true;
                base.ArgumentList_SelectedIndexChanged(listView, EventArgs.Empty);
            }

            internal void UserChangeSignatureText( string text)
            {
                dlg.Signature.Text = text;
                base.Signature_TextChanged(dlg.Signature, EventArgs.Empty);
            }

            internal void UserSwitchTab(int p)
            {
                dlg.TabControl.SelectedIndex = p;
            }
        }
    }
}
