/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class ProcedureDialogInteractorTests
    {
        private TestProcedureDialogInteractor interactor;
        private SerializedProcedure proc;
 
        [SetUp]
        public void Setup()
        {
            proc = new SerializedProcedure();
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
            sig.ReturnValue = new SerializedArgument();
            sig.ReturnValue.Kind = new SerializedRegister("eax");
            sig.ReturnValue.Type = "int32";
        }

        [Test]
        public void SelectReturnValue()
        {
            proc.Name = "x";
            proc.Signature = new SerializedSignature();
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


        private class TestProcedureDialogInteractor : ProcedureDialogInteractor
        {
            public TestProcedureDialogInteractor(SerializedProcedure proc)
                : base(proc)
            {
            }

            public void UserSelectedItem(ListView listView, int p)
            {
                listView.Items[0].Selected = true;
                base.ArgumentList_SelectedIndexChanged(listView, EventArgs.Empty);
            }

            internal void UserSwitchTab(int p)
            {
                dlg.TabControl.SelectedIndex = p;
            }
        }
    }
}
