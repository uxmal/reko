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
 
using System;
using System.Drawing;
using System.Windows.Forms;
using Rhino.Mocks;
using Reko.Arch.X86;
using Reko.Environments.Windows;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Windows.Forms;
using System.ComponentModel.Design;
using NUnit.Framework;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    class DeclarationFormInteractorTests
    {
        MockRepository mr;
        private DeclarationFormInteractor interactor;
        private IDeclarationForm declarationForm;
        private FakeTextBox textBox;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            var services = new ServiceContainer();
            declarationForm = mr.Stub<IDeclarationForm>();
            textBox = new FakeTextBox();
            declarationForm.Stub(f => f.TextBox).Return(textBox);
            declarationForm.Stub(f => f.ShowAt(new Point()));
            declarationForm.Stub(f => f.Hide());
            declarationForm.Stub(f => f.Dispose());
            var dlgFactory = mr.Stub<IDialogFactory>();
            dlgFactory.Stub(f => f.CreateDeclarationForm()).Return(declarationForm);
            services.AddService<IDialogFactory>(dlgFactory);
            mr.ReplayAll();
            interactor = new DeclarationFormInteractor(services);
            var mem = new MemoryArea(Address32.Ptr32(0x10), new byte[40]);
            var seg = new ImageSegment(".text", mem, AccessMode.ReadWrite);
            var imageMap = new ImageMap(Address32.Ptr32(0x05), seg);
            var arch = new X86ArchitectureFlat32();
            var platform = new Win32Platform(null, arch);
            program = new Program(imageMap, arch, platform);
        }

        private void CloseForm()
        {
            textBox.FireLostFocus();
        }

        private void Given_ImageMapItem(uint addr, DataType dataType, string name)
        {
            var address = Address32.Ptr32(addr);
            var item = new ImageMapItem
            {
                Address = address,
                DataType = dataType,
                Name = name,
                Size = (uint)dataType.Size,
            };
            program.ImageMap.AddItem(address, item);
        }

        private void Given_ProcedureName(uint addr, string name)
        {
            var address = Address32.Ptr32(addr);
            var proc = new Procedure(name, null);
            program.Procedures[address] = proc;
        }

        private void Given_ProcedureSignature(uint addr, string CSignature)
        {
            var address = Address32.Ptr32(addr);
            var sProc = new Procedure_v1()
            {
                CSignature = CSignature,
            };
            program.User.Procedures[address] = sProc;
        }

        [Test]
        public void Dfi_CreateGlobalInt32()
        {
            interactor.Show(new Point(0, 0), program, Address32.Ptr32(0x13));
            Assert.AreEqual("", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "int a(";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
            declarationForm.TextBox.Text = "int a";
            Assert.AreEqual(SystemColors.ControlText, declarationForm.TextBox.ForeColor);
            CloseForm();
            Assert.AreEqual(1, program.User.Globals.Count);
            Assert.AreEqual("00000013", program.User.Globals.Keys[0].ToString());
            Assert.AreEqual("prim(SignedInt,4)", program.User.Globals.Values[0].DataType.ToString());
            Assert.AreEqual("a", program.User.Globals.Values[0].Name);
        }

        [Test]
        public void Dfi_EditGlobalReal64()
        {
            Given_ImageMapItem(0x14, PrimitiveType.Real64, "DB");
            interactor.Show(new Point(0, 0), program, Address32.Ptr32(0x14));
            Assert.AreEqual("double DB", declarationForm.TextBox.Text);
        }

        [Test]
        public void Dfi_EditProcedureSignature()
        {
            Given_ProcedureSignature(0x16, "float test(float b)");
            interactor.Show(new Point(0, 0), program, Address32.Ptr32(0x16));
            Assert.AreEqual("float test(float b)", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "float test(floatb b)";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
        }

        [Test]
        public void Dfi_EditProcedureName()
        {
            Given_ProcedureName(0x15, "fn123");
            interactor.Show(new Point(0, 0), program, Address32.Ptr32(0x15));
            Assert.AreEqual("fn123", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "fn123456";
            Assert.AreEqual(SystemColors.ControlText, declarationForm.TextBox.ForeColor);
            CloseForm();
            Assert.AreEqual(1, program.User.Procedures.Count);
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("fn123456", program.User.Procedures.Values[0].Name);
            Assert.AreEqual("fn123456", program.Procedures.Values[0].Name);
        }

        private class FakeTextBox : ITextBox
        {
            private string text;

            public bool Enabled { get; set; }
            public string Text
            {
                get { return text == null ? "" : text; }
                set { text = value; TextChanged.Fire(this); }
            }
            public Color BackColor { get; set; }
            public Color ForeColor { get; set; }

            public void SelectAll()
            {
                throw new NotImplementedException();
            }

            public void Focus()
            {
                throw new NotImplementedException();
            }

            public event KeyEventHandler KeyDown;
            public event EventHandler TextChanged;
            public event EventHandler LostFocus;

            public void FireLostFocus()
            {
                LostFocus.Fire(this);
            }
        }
    }
}
