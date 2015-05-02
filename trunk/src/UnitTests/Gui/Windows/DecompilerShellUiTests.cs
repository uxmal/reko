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
        MockRepository mr;

        [SetUp]
        public void Setup()
        {
            form = new Form();
            form.IsMdiContainer = true;
            sc = new ServiceContainer();
            svc = new DecompilerShellUiService(form, null, null, null, sc);
            mr = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
            form = null;
        }

        [Test]
        public void Dsu_CreateWindow()
        {
            Setup();

            Form mdiForm = new Form();

            var pane = mr.StrictMock<IWindowPane>();
            Expect.Call(() => pane.SetSite(sc));
            Expect.Call(pane.CreateControl()).IgnoreArguments().Return(new Control());

            mr.ReplayAll();

            IWindowFrame window = svc.CreateWindow("testWin", "Test Window", pane);
            Assert.IsNotNull(window);
            Assert.AreEqual(1, form.MdiChildren.Length);
            Assert.AreEqual("Test Window", form.MdiChildren[0].Text);
            window.Show();
            mr.VerifyAll();
        }


        [Test]
        public void Dsu_UserCloseWindow()
        {
            form.Show();

            var pane = mr.StrictMock<IWindowPane>();
            Expect.Call(() => pane.SetSite(sc));
            Expect.Call(pane.CreateControl()).IgnoreArguments().Return(new Control());

            Expect.Call(() => pane.Close());

            mr.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            Assert.AreEqual(1, form.MdiChildren.Length);
            Assert.IsNotNull(svc.FindWindow("testWindow"));
            frame.Close();
            Assert.IsNull(svc.FindWindow("testWindow"));

            mr.VerifyAll();

        }

        [Test]
        public void Dsu_MultipleCallsToShowShouldntCreateNewPaneControl()
        {
            form.Show();

            var pane = mr.StrictMock<IWindowPane>();
            var ctrl1 = new Control();
            pane.Expect(s => s.SetSite(Arg<IServiceProvider>.Is.Anything));
            pane.Expect(s => s.CreateControl()).Return(ctrl1);
            pane.Expect(s => s.Close());
            mr.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            frame.Show();
            frame.Close();

            mr.VerifyAll();

        }

    }
}
