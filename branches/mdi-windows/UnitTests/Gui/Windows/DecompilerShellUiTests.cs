using Decompiler.Gui;
using Decompiler.Gui.Windows;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class DecompilerShellUiTests
    {
        Form form;
        DecompilerShellUiService svc;
        ServiceContainer sc;
        MockRepository repository;

        [Test]
        public void CreateWindow()
        {
            Setup();

            Form mdiForm = new Form();

            var pane = repository.StrictMock<IWindowPane>();
            Expect.Call(() => pane.SetSite(sc));
            Expect.Call(pane.CreateControl()).IgnoreArguments().Return(new Control());

            repository.ReplayAll();

            IWindowFrame window = svc.CreateWindow("testWin", "Test Window", pane);
            Assert.IsNotNull(window);
            Assert.AreEqual(1, form.MdiChildren.Length);
            Assert.AreEqual("Test Window", form.MdiChildren[0].Text);
            window.Show();
            repository.VerifyAll();
        }


        [Test]
        public void UserCloseWindow()
        {
            form.Show();

            var pane = repository.StrictMock<IWindowPane>();
            Expect.Call(() => pane.SetSite(sc));
            Expect.Call(pane.CreateControl()).IgnoreArguments().Return(new Control());

            Expect.Call(() => pane.Close());

            repository.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            Assert.AreEqual(1, form.MdiChildren.Length);
            Assert.IsNotNull(svc.FindWindow("testWindow"));
            frame.Close();
            Assert.IsNull(svc.FindWindow("testWindow"));

            repository.VerifyAll();

        }

        [Test]
        public void MultipleCallsToShowShouldntCreateNewPaneControl()
        {
            form.Show();

            var pane = repository.StrictMock<IWindowPane>();
            var ctrl1 = new Control();
            pane.Expect(s => s.SetSite(Arg<IServiceProvider>.Is.Anything));
            pane.Expect(s => s.CreateControl()).Return(ctrl1);
            pane.Expect(s => s.Close());
            repository.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            frame.Show();
            frame.Close();

            repository.VerifyAll();

        }

        [SetUp]
        public void Setup()
        {
            form = new Form();
            form.IsMdiContainer = true;
            sc = new ServiceContainer();
            svc = new DecompilerShellUiService(form, null, null, null, sc);
            repository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
            form = null;
        }

    }
}
