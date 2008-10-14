using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
    [TestFixture]
    public class FindDialogInteractorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EnableFindButton()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            using (FindDialog dlg = i.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABE";
                Assert.IsTrue(dlg.FindButton.Enabled);
            }
        }

        [Test]
        public void ToHexadecimal()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            Assert.AreEqual(new byte[] { 0x0A }, i.ToHexadecimal("0A"));
            Assert.AreEqual(new byte[] { 0x0A, 0x0B }, i.ToHexadecimal("0A0b"));
            Assert.AreNotEqual(new byte[] { 0x0A, 0x0B }, i.ToHexadecimal("0A0C"));
            Assert.IsNull(i.ToHexadecimal("AGFA"));
        }

        [Test]
        public void EnableFindButtonValidText()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            using (FindDialog dlg = i.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABF";
                Assert.IsTrue(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABG";
                Assert.IsFalse(dlg.FindButton.Enabled);
            }
        }

    }
}
