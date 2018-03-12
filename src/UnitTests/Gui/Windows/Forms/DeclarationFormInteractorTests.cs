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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Reko.Gui;
using Reko.Gui.Commands;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms.Forms;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using System.Drawing;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    class DeclarationFormInteractorTests
    {
        MockRepository mr;
        private ServiceContainer services;
        private DeclarationFormInteractor interactor;
        private IDeclarationForm declarationForm;
        private FakeTextBox textBox;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            services = new ServiceContainer();
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
            var segmentMap = new SegmentMap(Address32.Ptr32(0x05), seg);
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var platform = new Win32Platform(null, arch);
            program = new Program(segmentMap, arch, platform);
            program.ImageMap = segmentMap.CreateImageMap();
        }

        private void When_FormClosed()
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
            Given_ProcedureName(addr, "<unnamed>");

            var address = Address32.Ptr32(addr);
            var sProc = new Procedure_v1()
            {
                CSignature = CSignature,
            };
            program.User.Procedures[address] = sProc;
        }

        private void Given_CommandFactory()
        {
            var markProcedureCmd = mr.Stub<ICommand>();
            markProcedureCmd.Stub(c => c.Do()).Do(new Action(() => 
            {
                program.Procedures.Values[0].Name = 
                    program.User.Procedures.Values[0].Name;
            }));

            var cmdFactory = mr.Stub<ICommandFactory>();
            cmdFactory.Stub(f => f.MarkProcedure(null)).IgnoreArguments().Do(
                new Func<ProgramAddress, ICommand>((pa) => 
            {
                var program = pa.Program;
                var addr = pa.Address;
                program.Procedures[addr] = new Procedure("<unnamed>", null);
                return markProcedureCmd;
            })); 

            services.AddService<ICommandFactory>(cmdFactory);
            mr.ReplayAll();
        }

        private void When_DeclarationFormCreated(uint addr)
        {
            interactor.Show(new Point(0, 0), program, Address32.Ptr32(addr));
        }

        [Test]
        public void Dfi_CreateGlobalInt32()
        {
            When_DeclarationFormCreated(0x13);

            Assert.AreEqual(
                "Enter procedure or global variable declaration at the address 00000013",
                declarationForm.HintText);

            Assert.AreEqual("", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "int a(";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
            declarationForm.TextBox.Text = "int a";
            Assert.AreEqual(SystemColors.ControlText, declarationForm.TextBox.ForeColor);
            When_FormClosed();
            Assert.AreEqual(3, program.ImageMap.Items.Count);
            Assert.AreEqual(1, program.User.Globals.Count);
            Assert.AreEqual("00000013", program.User.Globals.Keys[0].ToString());
            Assert.AreEqual("prim(SignedInt,4)", program.User.Globals.Values[0].DataType.ToString());
            Assert.AreEqual("a", program.User.Globals.Values[0].Name);
        }

        [Test]
        public void Dfi_EditGlobalReal64()
        {
            Given_ImageMapItem(0x14, PrimitiveType.Real64, "DB");
            When_DeclarationFormCreated(0x14);

            Assert.AreEqual(
                "Enter procedure or global variable declaration at the address 00000014",
                declarationForm.HintText);

            Assert.AreEqual("double DB", declarationForm.TextBox.Text);
        }

        [Test]
        public void Dfi_EditProcedureSignature()
        {
            Given_ProcedureSignature(0x16, "float test(float b)");
            When_DeclarationFormCreated(0x16);
            Assert.AreEqual("float test(float b)", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "float test(floatb b)";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
        }

        [Test]
        public void Dfi_EditProcedureName()
        {
            Given_ProcedureName(0x15, "fn123");
            When_DeclarationFormCreated(0x15);
            Assert.AreEqual("fn123", declarationForm.TextBox.Text);
            declarationForm.TextBox.Text = "fn123456";
            Assert.AreEqual(SystemColors.ControlText, declarationForm.TextBox.ForeColor);
            When_FormClosed();
            Assert.AreEqual(1, program.User.Procedures.Count);
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("fn123456", program.User.Procedures.Values[0].Name);
            Assert.AreEqual("fn123456", program.Procedures.Values[0].Name);
        }

        [Test(Description = "When a previously uncustomized procedure is displayed, show its name in the " +
    "declaration box")]
        public void Dfi_EditProcedure_ShowFnName()
        {
            Given_ProcedureName(0x17, "fnName");

            When_DeclarationFormCreated(0x17);

            Assert.AreEqual("fnName", declarationForm.TextBox.Text);
        }

        [Test(Description = "Just entering a (valid) name should be OK.")]
        public void Dfi_Accept_JustName()
        {
            Given_ProcedureName(0x17, "fnName");

            When_DeclarationFormCreated(0x17);

            Assert.AreEqual(
                "Enter procedure declaration at the address 00000017",
                declarationForm.HintText);

            declarationForm.TextBox.Text = "foo";
            Assert.AreEqual(SystemColors.ControlText, declarationForm.TextBox.ForeColor);
            When_FormClosed();

            Assert.AreEqual("foo", program.Procedures.Values[0].Name);
            Assert.AreEqual("foo", program.User.Procedures.Values[0].Name);
            Assert.IsNull(program.User.Procedures.Values[0].CSignature);
        }

        [Test(Description = "Entering an invalid name should change nothing.")]
        public void Dfi_Reject_Invalid_Name()
        {
            Given_ProcedureName(0x18, "fnTest");

            When_DeclarationFormCreated(0x18);

            Assert.AreEqual(
                "Enter procedure declaration at the address 00000018",
                 declarationForm.HintText);

            declarationForm.TextBox.Text = "f@oo";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
            When_FormClosed();

            Assert.AreEqual("fnTest", program.Procedures.Values[0].Name);
        }

        [Test(Description = "Entering an valid function declaration should change both name and signature.")]
        public void Dfi_Accept_Declaration()
        {
            Given_ProcedureSignature(0x17, "int fnTest()");

            When_DeclarationFormCreated(0x17);

            declarationForm.TextBox.Text = "int foo(char *, float)";
            When_FormClosed();

            Assert.AreEqual("foo", program.Procedures.Values[0].Name);
            Assert.AreEqual("int foo(char *, float)", program.User.Procedures.Values[0].CSignature);
        }

        [Test(Description = "Char * functions are valid, of course")]
        public void Dfi_Accept_Declaration_Returning_CharPtr()
        {
            Given_ProcedureSignature(0x17, "int fnTest()");

            When_DeclarationFormCreated(0x17);

            declarationForm.TextBox.Text = "char * test(int)";
            When_FormClosed();

            Assert.AreEqual("test", program.Procedures.Values[0].Name);
            Assert.AreEqual("char * test(int)", program.User.Procedures.Values[0].CSignature);
        }

        [Test]
        public void Dfi_Accept_Declaration_PlatformTypes()
        {
            program.EnvironmentMetadata.Types.Add(
                    "BYTE",
                    PrimitiveType.Create(PrimitiveType.Byte.Domain, 1));
            Given_ProcedureName(0x17, "fnTest");

            When_DeclarationFormCreated(0x17);

            declarationForm.TextBox.Text = "BYTE foo(BYTE a, BYTE b)";
            When_FormClosed();

            Assert.AreEqual("foo", program.Procedures.Values[0].Name);
            Assert.AreEqual("BYTE foo(BYTE a, BYTE b)", program.User.Procedures.Values[0].CSignature);
        }

        [Test]
        public void Dfi_CreateProcedure()
        {
            Given_CommandFactory();

            When_DeclarationFormCreated(0x20);

            declarationForm.TextBox.Text = "int funcA(double v)";
            When_FormClosed();

            Assert.AreEqual(1, program.User.Procedures.Count);
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("funcA", program.Procedures.Values[0].Name);
            Assert.AreEqual("funcA", program.User.Procedures.Values[0].Name);
            Assert.AreEqual("int funcA(double v)", program.User.Procedures.Values[0].CSignature);
        }

        [Test]
        public void Dfi_ReplaceGlobalWithProcedure()
        {
            Given_ImageMapItem(0x21, PrimitiveType.Real32, "rVal");
            Given_CommandFactory();

            When_DeclarationFormCreated(0x21);

            Assert.AreEqual("float rVal", declarationForm.TextBox.Text);

            declarationForm.TextBox.Text = "float abc(char ch)";
            When_FormClosed();

            Assert.AreEqual(1, program.ImageMap.Items.Count);
            Assert.AreEqual(1, program.User.Procedures.Count);
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("abc", program.Procedures.Values[0].Name);
            Assert.AreEqual("abc", program.User.Procedures.Values[0].Name);
            Assert.AreEqual("float abc(char ch)", program.User.Procedures.Values[0].CSignature);
        }

        [Test(Description = "Reject global variable declartion if there is procedure at the same address.")]
        public void Dfi_ReplaceProcedureWithGlobal()
        {
            Given_ProcedureSignature(0x19, "int fn19()");

            When_DeclarationFormCreated(0x19);

            declarationForm.TextBox.Text = "int a";
            Assert.AreEqual(Color.Red, declarationForm.TextBox.ForeColor);
            When_FormClosed();

            Assert.AreEqual(0, program.User.Globals.Count);
            Assert.AreEqual(1, program.User.Procedures.Count);
            Assert.AreEqual(1, program.Procedures.Count);
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

            public event EventHandler<KeyEventArgs> KeyDown;
            public event EventHandler TextChanged;
            public event EventHandler LostFocus;

            public void FireLostFocus()
            {
                LostFocus.Fire(this);
            }

            public void FireKeyDown(KeyEventArgs e)
            {
                KeyDown(this, e);
            }
        }
    }
}
